using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
namespace NewMideaProgram
{
    public class cHuaWeiMachine
    {
        /// <summary>
        /// 使用的串口
        /// </summary>
        public SerialPort Com
        { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error
        { get; set; }
        /// <summary>
        /// 环境温度
        /// </summary>
        public float HuangJingWenDu
        { get; set; }
        /// <summary>
        /// 排气温度
        /// </summary>
        public float PaiQiWenDu
        { get; set; }
        /// <summary>
        /// 吸气温度
        /// </summary>
        public float XiQiWenDu
        { get; set; }
        /// <summary>
        /// 排气压力
        /// </summary>
        public float PaiQiYaLi
        { get; set; }
        /// <summary>
        /// 高压开关
        /// </summary>
        public float GaoYaKaiGuan
        { get; set; }
        /// <summary>
        /// 低压开关
        /// </summary>
        public float DiYaKaiGuan
        { get; set; }

        cStandarBoard fengJi;
        cStandarBoard yaSuoJi;
        Thread thRead;

        string barcode = "";
        bool writeBarcode = false;

        bool writeStart = false;


        int fengJiErrorCount = 0;
        int yaSuoJiErrorCount = 0;
        bool exit = false;
        public cHuaWeiMachine(string com)
        {
            this.Com = new SerialPort(com, 9600, Parity.None, 8, StopBits.One);
        }
        public void Init()
        {
            try
            {
                this.Com.BaudRate = 9600;
                this.Com.Parity = Parity.None;
                this.Com.DataBits = 8;
                this.Com.StopBits = StopBits.One;

                this.Com.Open();
                exit = false;
                yaSuoJi = new cStandarBoard(Com, 1, 400);
                fengJi = new cStandarBoard(Com, 2, 400);

                thRead = new Thread(() => Flush());
                thRead.IsBackground = true;
                thRead.Start();
            }
            catch (Exception e)
            {
                Error = e.Message;
            }
        }
        public void Close()
        {
            exit = true;
            if (thRead != null)
            {
                thRead.Abort();
                thRead.Join(100);
                thRead = null;
            }
            if (this.Com != null && this.Com.IsOpen)
            {
                this.Com.Close();
            }
        }
        public void WriteBar(string barcode)
        {
            barcode = barcode.Trim();
            if ((barcode.Length % 2) == 1)
            {
                barcode = string.Format("{0} ", barcode);
            }
            if (barcode.Length > 30)
            {
                barcode = barcode.Substring(0, 30);
            }
            this.barcode = barcode;
            writeBarcode = true;
        }
        public void WriteStart()
        {
            writeStart = true;
        }
        public void Flush()
        {
            long[] tmpValue;
            while (!exit)
            {
                if (writeBarcode)//把条码写入到空调里面
                {
                    if (yaSuoJi.StandarBoardWritePoint(2926, barcode.Length / 2, Encoding.ASCII.GetBytes(barcode)))
                    {
                        writeBarcode = false;
                        Thread.Sleep(50);
                    }
                    else
                    {
                        Error = yaSuoJi.ErrStr;
                    }
                }
                if (writeStart)
                {
                    if (yaSuoJi.StandarBoardWritePoint(1929, 1, 6))//开始整机快检
                    {
                        writeStart = false;
                        Thread.Sleep(50);
                    }
                    else
                    {
                        Error = yaSuoJi.ErrStr;
                    }
                }
                tmpValue = new long[1];
                if (fengJi.StandarBoardRead(204, 1, ref tmpValue))//读环境温度
                {
                    HuangJingWenDu = tmpValue[0] / 10.0f;
                    fengJiErrorCount = 0;
                }
                else
                {
                    fengJiErrorCount++;
                    if (fengJiErrorCount > 3)
                    {
                        fengJiErrorCount = 3;
                        HuangJingWenDu = -99;
                    }
                }
                Thread.Sleep(50);
                tmpValue = new long[8];
                if (yaSuoJi.StandarBoardRead(2971, 8, ref tmpValue))//读压缩机信息
                {
                    PaiQiWenDu = tmpValue[0] / 10.0f;
                    XiQiWenDu = tmpValue[1] / 10.0f;
                    PaiQiYaLi = tmpValue[2] / 100.0f;
                    GaoYaKaiGuan = tmpValue[5];
                    DiYaKaiGuan = tmpValue[6];
                    yaSuoJiErrorCount = 0;
                }
                else
                {
                    yaSuoJiErrorCount++;
                    if (yaSuoJiErrorCount > 3)
                    {
                        yaSuoJiErrorCount = 3;
                        PaiQiWenDu = -99;
                        PaiQiYaLi = -99;
                        XiQiWenDu = -99;
                    }
                }
                Thread.Sleep(100);
            }
        }
    }
}
