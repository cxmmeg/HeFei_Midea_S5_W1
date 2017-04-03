using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
namespace NewMideaProgram
{
    public class cQz8902F
    {
        string errInfo = "";
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrInfo
        {
            get { return errInfo; }
            set { errInfo = value; }
        }
        SerialPort port = new SerialPort();
        /// <summary>
        /// 串口
        /// </summary>
        public SerialPort Port
        {
            get { return port; }
            set { port = value; }
        }
        int timeOut = 800;
        /// <summary>
        /// 通讯超时时间
        /// </summary>
        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }
        int address = 1;
        /// <summary>
        /// 通讯地址
        /// </summary>
        public int Address
        {
            get { return address; }
            set { address = value; }
        }

        public cQz8902F(SerialPort mPort, int mAddress)
        {
            port = mPort;
            address = mAddress;
            if (!port.IsOpen)
            {
                try
                {
                    port.Open();
                    port.BaudRate = 9600;
                    port.Parity = Parity.None;
                    port.DataBits = 8;
                    port.StopBits = StopBits.One;
                }
                catch
                {

                }
            }
        }
        public bool Read(ref cReadDataFormat readData)
        {
            bool isOk = false;
            try
            {
                bool isGet = false;
                bool isTimeOut = false;
                int readLen = 0;

                byte[] sendBuff = new byte[4];
                byte[] readBuff;
                sendBuff[0] = 0x55;
                sendBuff[1] = (byte)(address & 0xFF);
                sendBuff[2] = 0x34;
                sendBuff[3] = (byte)((sendBuff[0] + sendBuff[1] + sendBuff[2]) & 0xFF);
                port.DiscardInBuffer();
                port.Write(sendBuff, 0, sendBuff.Length);
                int startTime = Environment.TickCount;

                do
                {
                    Thread.Sleep(20);
                    readLen = port.BytesToRead;
                    if (readLen == 72)
                    {
                        isGet = true;
                    }
                    if ((startTime + timeOut) < Environment.TickCount)
                    {
                        isTimeOut = true;
                    }
                } while (!isGet && !isTimeOut);
                if (!isGet && isTimeOut)
                {
                    isOk = false;
                    errInfo = string.Format("读取数据已超时,当前读取数据长度{0}", readLen);
                }
                else
                {
                    readBuff = new byte[readLen];
                    port.Read(readBuff, 0, readLen);
                    if (readBuff[0] != 0xAA)
                    {
                        isOk = false;
                        errInfo = string.Format("校验数据失败,当前校验位{0}!={1:X}", readBuff[0], 0xAA);
                    }
                    else
                    {
                        byte[] tmpByte = new byte[68];
                        Array.Copy(readBuff, 3, tmpByte, 0, 68);
                        float[] tmpValue = All.Class.Num.BytesToFloat(tmpByte, All.Class.Num.QueueList.一二三四);// ClassBySS.Class.cNum.ByteToFloat(readBuff, 3, 68, ClassBySS.Class.cNum.FloatQueueList.一二三四);
                        readData.SingleVol[0] = tmpValue[0];
                        readData.SingleVol[1] = tmpValue[4];
                        readData.SingleVol[2] = tmpValue[8];
                        readData.SingleCur[0] = tmpValue[1];
                        readData.SingleCur[1] = tmpValue[5];
                        readData.SingleCur[2] = tmpValue[9];
                        readData.SinglePow[0] = tmpValue[2];
                        readData.SinglePow[1] = tmpValue[6];
                        readData.SinglePow[2] = tmpValue[10];
                        readData.SinglePF[0] = tmpValue[3];
                        readData.SinglePF[1] = tmpValue[7];
                        readData.SinglePF[2] = tmpValue[11];
                        readData.AllValue[0] = tmpValue[12];
                        readData.AllValue[1] = tmpValue[13];
                        readData.AllValue[2] = tmpValue[14];
                        readData.AllValue[3] = tmpValue[15];
                        readData.AllValue[4] = tmpValue[16];
                        isOk = true;
                    }
                }
            }
            catch (Exception exc)
            {
                if (errInfo.IndexOf(exc.Message) < 0)
                {
                    errInfo = exc.Message;
                }
            }
            return isOk;
        }
        public class cReadDataFormat
        {
            /// <summary>
            /// A,B,C三相电压
            /// </summary>
            public float[] SingleVol = new float[3];
            /// <summary>
            /// A,B,C三相电流
            /// </summary>
            public float[] SingleCur = new float[3];
            /// <summary>
            /// A,B,C三相功率
            /// </summary>
            public float[] SinglePow = new float[3];
            /// <summary>
            /// A,B,C三相功率因素
            /// </summary>
            public float[] SinglePF = new float[3];
            /// <summary>
            /// 电压(三相平均),电流(三相平均),功率(三相和),功率因素(三相平均),频率
            /// </summary>
            public float[] AllValue = new float[5];
            public cReadDataFormat()
            {
                for (int i = 0; i < SingleVol.Length; i++)
                {
                    SingleVol[i] = 0;
                }
                for (int i = 0; i < SingleCur.Length; i++)
                {
                    SingleCur[i] = 0;
                }
                for (int i = 0; i < SinglePow.Length; i++)
                {
                    SinglePow[i] = 0;
                }
                for (int i = 0; i < SinglePF.Length; i++)
                {
                    SinglePF[i] = 0;
                }
                for (int i = 0; i < AllValue.Length; i++)
                {
                    AllValue[i] = 0;
                }
            }
        }
    }
}
