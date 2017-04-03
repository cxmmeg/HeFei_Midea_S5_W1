using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;
namespace NewMideaProgram
{
    public class cHYAnGui
    {
        const string 接线 = "仪表235-电脑325:串口9600,n,8,1";
        const string mName = "华仪安规";
        SerialPort comPort;
        /// <summary>
        /// 安规使用的串口
        /// </summary>
        public SerialPort ComPort
        {
            get { return comPort; }
            set { comPort = value; }
        }
        int timeOut = 700;
        /// <summary>
        /// 读取串口数据超时时间
        /// </summary>
        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }
        string errStr = "";
        /// <summary>
        /// 读取串口过程中返回错误
        /// </summary>
        public string ErrStr
        {
            get { return errStr; }
            set { errStr = value; }
        }
        public enum AnGuiList
        {
            HY7440,
            HY7623,
            HY125,
            BothTwo
        }
        public enum AnGuiTestList : int
        {
            JieDi,
            JueYuan,
            NaiYa,
            XieLou
        }
        public struct TempAnGuiData
        {
            public bool[] isReadOk;
            public float[] ReadData;
            public bool[] isPass;
            public int[] index;
        }
        static cHYAnGui.TempAnGuiData tempAnGuiData = new cHYAnGui.TempAnGuiData();
        bool isHYAnGuiInit = false;

        static cHYAnGui hy7440;
        public static cHYAnGui Hy7440
        {
            set { hy7440 = value; }
        }
        static cHYAnGui hy7623;
        public static cHYAnGui Hy7623
        {
            set { hy7623 = value; }
        }
        public static bool ReadData(AnGuiData mAnGuiData, AnGuiList anGuiList, ref bool isReadOver)
        {
            bool isOk = false;
            bool isRead7440 = false;
            bool isRead7623 = false;
            cHYAnGui.TempAnGuiData tmpData = new TempAnGuiData();
            tmpData.isReadOk = new bool[AnGuiData.testCount];
            tmpData.ReadData = new float[AnGuiData.testCount];
            tmpData.isPass = new bool[AnGuiData.testCount];
            tmpData.index = new int[AnGuiData.testCount];
            for (int i = 0; i < tempAnGuiData.ReadData.Length; i++)
            {
                tmpData.ReadData[i] = 0;
                tmpData.isReadOk[i] = false;
                tmpData.isPass[i] = true;
                tmpData.index[i] = i + 1;
            }
            AnGuiTestList tmpAnGuiTestList = AnGuiTestList.JieDi;
            isReadOver = true;
            try
            {
                switch (anGuiList)
                {
                    case AnGuiList.HY7440:
                        isRead7440 = true;
                        break;
                    case AnGuiList.HY7623:
                        isRead7623 = true;
                        break;
                    case AnGuiList.BothTwo:
                        isRead7440 = true;
                        isRead7623 = true;
                        break;
                }
                if (isRead7623)
                {
                    if (hy7623 != null)
                    {
                        if (!tmpData.isReadOk[3])
                        {
                            if (hy7623.HYAnGuiRead(AnGuiList.HY125, 1, ref tmpData.ReadData[3], ref tmpData.isPass[3], out tmpAnGuiTestList))
                            {
                                tmpData.isReadOk[3] = true;
                                switch (tmpAnGuiTestList)
                                {
                                    case AnGuiTestList.JieDi:
                                        tmpData.index[3] = 0;
                                        break;
                                    case AnGuiTestList.JueYuan:
                                        tmpData.index[3] = 1;
                                        break;
                                    case AnGuiTestList.NaiYa:
                                        tmpData.index[3] = 2;
                                        break;
                                    case AnGuiTestList.XieLou:
                                        tmpData.index[3] = 3;
                                        break;
                                }
                            }
                        }
                    }
                }
                if (isRead7440)
                {
                    if (hy7440 != null)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            if (!tmpData.isReadOk[i])
                            {
                                Thread.Sleep(100);
                                if (hy7440.HYAnGuiRead(AnGuiList.HY7440, i + 1, ref tmpData.ReadData[i], ref tmpData.isPass[i], out tmpAnGuiTestList))
                                {
                                    tmpData.isReadOk[i] = true;
                                    switch (tmpAnGuiTestList)
                                    {
                                        case AnGuiTestList.JieDi:
                                            tmpData.index[i] = 0;
                                            break;
                                        case AnGuiTestList.JueYuan:
                                            tmpData.index[i] = 1;
                                            break;
                                        case AnGuiTestList.NaiYa:
                                            tmpData.index[i] = 2;
                                            break;
                                        case AnGuiTestList.XieLou:
                                            tmpData.index[i] = 3;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < tmpData.isReadOk.Length; i++)
                {
                    if (!tmpData.isReadOk[i])
                    {
                        isReadOver = false;
                        break;
                    }
                }
                if (isReadOver)
                {
                    mAnGuiData.isPass = true;
                    for (int i = 0; i < tmpData.ReadData.Length; i++)
                    {
                        if (tmpData.index[i] < AnGuiData.testCount)
                        {
                            mAnGuiData.TestData[tmpData.index[i]] = tmpData.ReadData[i];
                            mAnGuiData.TestResult[tmpData.index[i]] = tmpData.isPass[i];
                            mAnGuiData.isPass = (mAnGuiData.isPass && tmpData.isPass[i]);
                        }
                    }
                }
                isOk = true;
            }
            catch (Exception exc)
            {
                All.Class.Error.Add(exc);
            }
            return isOk;
        }
        /// <summary>
        /// 复位临时检测数据
        /// </summary>
        public static void Reset()
        {
            tempAnGuiData.isReadOk = new bool[AnGuiData.testCount];
            tempAnGuiData.ReadData = new float[AnGuiData.testCount];
            tempAnGuiData.isPass = new bool[AnGuiData.testCount];
            tempAnGuiData.index = new int[AnGuiData.testCount];
            for (int i = 0; i < tempAnGuiData.ReadData.Length; i++)
            {
                tempAnGuiData.ReadData[i] = 0;
                tempAnGuiData.isReadOk[i] = false;
                tempAnGuiData.isPass[i] = true;
                tempAnGuiData.index[i] = i + 1;
            }
        }
        public cHYAnGui(SerialPort serialPort)
        {
            comPort = serialPort;
            Reset();
        }

        public bool HYAnGuiRead(AnGuiList anGuiList, int testIndex, ref float testData, ref bool testResult, out AnGuiTestList anGuiTestList)
        {
            //ACW
            anGuiTestList = AnGuiTestList.JieDi;
            bool isOk = false;
            byte[] WriteBuff = new byte[9];//发送数据
            byte[] ReadBuff = new byte[50];//接收数据
            int ReturnByte = 0;//返回数据
            bool IsReturn = false;//是否成功返回
            bool IsTimeOut = false;//是否超时
            DateTime NowTime = DateTime.Now;//当前时间
            TimeSpan ts;//时间差
            try
            {
                if (!comPort.IsOpen)
                {
                    comPort.Open();
                }
                switch (anGuiList)
                {
                    case AnGuiList.HY125:
                        WriteBuff[0] = Convert.ToByte('t');
                        WriteBuff[1] = Convert.ToByte('d');
                        WriteBuff[2] = Convert.ToByte('?');
                        WriteBuff[3] = 0x0D;
                        WriteBuff[4] = 0x0A;
                        comPort.DiscardInBuffer();
                        ReturnByte = 0;
                        comPort.Write(WriteBuff, 0, 5);
                        break;
                    case AnGuiList.HY7440:
                    case AnGuiList.HY7623:
                        WriteBuff[0] = Convert.ToByte('?');
                        WriteBuff[1] = Convert.ToByte(Convert.ToChar(testIndex.ToString()));
                        WriteBuff[2] = 0x0D;
                        WriteBuff[3] = 0x0A;

                        comPort.DiscardInBuffer();
                        ReturnByte = 0;
                        comPort.Write(WriteBuff, 0, 4);
                        break;
                }

                NowTime = DateTime.Now;
                IsTimeOut = false;
                ReturnByte = comPort.BytesToRead;
                do
                {
                    Thread.Sleep(30);
                    if (ReturnByte == comPort.BytesToRead)
                    {
                        if (ReturnByte > 0)
                        {
                            IsReturn = true;
                        }
                    }
                    else
                    {
                        ReturnByte = comPort.BytesToRead;
                    }
                    ts = DateTime.Now - NowTime;
                    if (ts.TotalMilliseconds > timeOut)//时间超时
                    {
                        IsTimeOut = true;
                    }
                } while (!IsReturn && !IsTimeOut);
                if (!IsReturn && IsTimeOut)//超时
                {
                    if (errStr.IndexOf("停止失败,接收数据已超时") < 0)
                    {
                        errStr = errStr + DateTime.Now.ToString() + mName + ":停止失败,接收数据已超时" + (char)13 + (char)10;
                    }
                    isOk = false;
                }
                else
                {
                    ReturnByte = comPort.BytesToRead;
                    comPort.Read(ReadBuff, 0, ReturnByte);
                    if (ReadBuff[0] == 0x15)
                    {
                        isOk = false;
                        errStr = "正在检测";
                    }
                    else
                    {
                        switch (anGuiList)
                        {
                            case AnGuiList.HY125:
                                if (ReturnByte <23)
                                {
                                    isOk = false;
                                    errStr = "应答失败";
                                }
                                else
                                {
                                    testResult = false;
                                    testData = 0;
                                    string tempStr = Encoding.ASCII.GetString(ReadBuff, 0, ReturnByte);
                                    if (tempStr.ToLower().IndexOf("ass") >= 0)
                                    {
                                        testResult = true;
                                    }
                                    string[] buff = tempStr.Split(',');
                                    if (buff.Length >= 7)
                                    {
                                        int start = 0;
                                        for (int i = 0; i < buff[5].Length; i++)
                                        {
                                            if (buff[5][i] >= 0x30 && buff[5][i] <= 0x39)
                                            {
                                                start = i;
                                                break;
                                            }
                                        }
                                        anGuiTestList = AnGuiTestList.XieLou ;
                                        testData = All.Class.Num.ToFloat(buff[5].Substring(start));
                                        isOk = true;
                                    }
                                }
                                break;
                            case AnGuiList.HY7440:
                            case AnGuiList.HY7623:
                                if (ReturnByte <40)
                                {
                                    isOk = false;
                                    errStr = "应答失败";
                                }
                                else
                                {
                                    testResult = false;
                                    testData = 0;
                                    string tempStr = Encoding.ASCII.GetString(ReadBuff, 0, ReturnByte);
                                    switch (anGuiList)
                                    {
                                        case AnGuiList.HY7440:
                                            if (tempStr.ToLower().IndexOf("pass") >= 0)
                                            {
                                                testResult = true;
                                            }
                                            if (tempStr.ToLower().IndexOf("acw") >= 0)
                                            {
                                                anGuiTestList = AnGuiTestList.NaiYa;
                                            }
                                            if (tempStr.ToLower().IndexOf("ir") >= 0)
                                            {
                                                anGuiTestList = AnGuiTestList.JueYuan;
                                            }
                                            if (tempStr.ToLower().IndexOf("gnd") >= 0)
                                            {
                                                anGuiTestList = AnGuiTestList.JieDi;
                                            }
                                            break;
                                        case AnGuiList.HY7623:
                                            if (tempStr.ToLower().IndexOf("ass") >= 0)
                                            {
                                                testResult = true;
                                            }
                                            break;
                                        case AnGuiList.BothTwo:
                                            throw new Exception("anGuiList.BothTwo,此处不能用此参数");
                                    }
                                    bool isNumStart = false;
                                    int NumStart = 0;
                                    int NumEnd = 0;
                                    for (int i = 32; i < ReturnByte; i++)
                                    {
                                        if (!isNumStart)
                                        {
                                            if ((ReadBuff[i] >= 0x30) && (ReadBuff[i] <= 0x39))
                                            {
                                                isNumStart = true;
                                                NumStart = i;
                                            }
                                        }
                                        if (isNumStart)
                                        {
                                            if (((ReadBuff[i] != 46) && (ReadBuff[i] < 0x30)) || (ReadBuff[i] > 0x39))
                                            {
                                                isNumStart = false;
                                                NumEnd = i;
                                                break;
                                            }
                                        }
                                    }

                                    testData = All.Class.Num.ToFloat(Encoding.ASCII.GetString(ReadBuff, NumStart, NumEnd - NumStart));
                                    isOk = true;
                                }
                                break;
                        }

                    }
                }
            }
            catch (Exception exc)
            {
                if (errStr.IndexOf(exc.ToString()) < 0)
                {
                    errStr = errStr + DateTime.Now.ToString() + mName + ":" + exc.ToString() + (char)13 + (char)10;
                }
                isOk = false;
            }
            return isOk;
        }
        /// <summary>
        /// 读取安规数据
        /// </summary>
        /// <param name="anGuiList">AnGuiList,安规类型</param>
        /// <param name="testIndex">int,要读取的数据组号,从1开始</param>
        /// <param name="testData">ref double,读取到的安规数据</param>
        /// <param name="testResult">ref bool,读取到的结果</param>
        /// <returns>bool,是否读取成功</returns>
        public bool HYAnGuiRead(AnGuiList anGuiList, int testIndex, ref float testData, ref bool testResult)
        {
            AnGuiTestList anGuiTestList;
            return HYAnGuiRead(anGuiList, testIndex, ref testData, ref testResult, out anGuiTestList);
        }
        /// <summary>
        /// 启动安规
        /// </summary>
        /// <returns>bool,是否启动成功</returns>
        public bool HYAnGuiStart(AnGuiList anguiList)
        {
            bool isOk = false;
            byte[] WriteBuff = new byte[9];//发送数据
            byte[] ReadBuff = new byte[50];//接收数据
            int ReturnByte = 0;//返回数据
            bool IsReturn = false;//是否成功返回
            bool IsTimeOut = false;//是否超时
            DateTime NowTime = DateTime.Now;//当前时间
            TimeSpan ts;//时间差
            try
            {
                if (!comPort.IsOpen)
                {
                    comPort.Open();
                }
                switch (anguiList)
                {
                    case AnGuiList.HY125:
                        WriteBuff[0] = Convert.ToByte('t');
                        WriteBuff[1] = Convert.ToByte('e');
                        WriteBuff[2] = Convert.ToByte('s');
                        WriteBuff[3] = Convert.ToByte('t');
                        WriteBuff[4] = 0x0D;
                        WriteBuff[5] = 0x0A;

                        comPort.DiscardInBuffer();
                        comPort.Write(WriteBuff, 0, 6);
                        break;
                    case AnGuiList.HY7440:
                    case AnGuiList.HY7623:
                        WriteBuff[0] = Convert.ToByte('F');
                        WriteBuff[1] = Convert.ToByte('A');
                        WriteBuff[2] = 0x0D;
                        WriteBuff[3] = 0x0A;

                        comPort.DiscardInBuffer();
                        comPort.Write(WriteBuff, 0, 4);
                        break;
                }
                NowTime = DateTime.Now;
                do
                {
                    if (comPort.BytesToRead >= 3)//收到数据
                    {
                        ReturnByte = comPort.BytesToRead;
                        IsReturn = true;
                    }
                    ts = DateTime.Now - NowTime;
                    if (ts.TotalMilliseconds > timeOut)//时间超时
                    {
                        IsTimeOut = true;
                    }
                } while (!IsReturn && !IsTimeOut);
                if (!IsReturn && IsTimeOut)//超时
                {
                    if (errStr.IndexOf("停止失败,接收数据已超时") < 0)
                    {
                        errStr = errStr + DateTime.Now.ToString() + mName + ":停止失败,接收数据已超时" + (char)13 + (char)10;
                    }
                    isOk = false;
                }
                else
                {
                    isOk = true;
                }
            }
            catch (Exception exc)
            {
                if (errStr.IndexOf(exc.ToString()) < 0)
                {
                    errStr = errStr + DateTime.Now.ToString() + mName + ":" + exc.ToString() + (char)13 + (char)10;
                }
                isOk = false;
            }
            return isOk;

        }
        /// <summary>
        /// 停止安规
        /// </summary>
        /// <returns>bool,是否停止成功</returns>
        public bool HYAnGuiStop(AnGuiList anguiList)
        {
            bool isOk = false;
            byte[] WriteBuff = new byte[9];//发送数据
            byte[] ReadBuff = new byte[50];//接收数据
            int ReturnByte = 0;//返回数据
            bool IsReturn = false;//是否成功返回
            bool IsTimeOut = false;//是否超时
            DateTime NowTime = DateTime.Now;//当前时间
            TimeSpan ts;//时间差
            try
            {
                if (!comPort.IsOpen)
                {
                    comPort.Open();
                }
                switch (anguiList)
                {
                    case AnGuiList.HY125:
                        WriteBuff[0] = Convert.ToByte('r');
                        WriteBuff[1] = Convert.ToByte('e');
                        WriteBuff[2] = Convert.ToByte('s');
                        WriteBuff[3] = Convert.ToByte('e');
                        WriteBuff[4] = Convert.ToByte('t');
                        WriteBuff[5] = 0x0D;
                        WriteBuff[6] = 0x0A;

                        comPort.DiscardInBuffer();
                        comPort.Write(WriteBuff, 0, 7);
                        break;
                    case AnGuiList.HY7440:
                    case AnGuiList.HY7623:
                        WriteBuff[0] = Convert.ToByte('F');
                        WriteBuff[1] = Convert.ToByte('B');
                        WriteBuff[2] = 0x0D;
                        WriteBuff[3] = 0x0A;

                        comPort.DiscardInBuffer();
                        comPort.Write(WriteBuff, 0, 4);
                        break;
                }
                NowTime = DateTime.Now;
                do
                {
                    if (comPort.BytesToRead >= 4)//收到数据
                    {
                        ReturnByte = comPort.BytesToRead;
                        IsReturn = true;
                    }
                    ts = DateTime.Now - NowTime;
                    if (ts.TotalMilliseconds > timeOut)//时间超时
                    {
                        IsTimeOut = true;
                    }
                } while (!IsReturn && !IsTimeOut);
                if (!IsReturn && IsTimeOut)//超时
                {
                    if (errStr.IndexOf("停止失败,接收数据已超时") < 0)
                    {
                        errStr = errStr + DateTime.Now.ToString() + mName + ":停止失败,接收数据已超时" + (char)13 + (char)10;
                    }
                    isOk = false;
                }
                else
                {
                    isOk = true;
                }
            }
            catch (Exception exc)
            {
                if (errStr.IndexOf(exc.ToString()) < 0)
                {
                    errStr = errStr + DateTime.Now.ToString() + mName + ":" + exc.ToString() + (char)13 + (char)10;
                }
                isOk = false;
            }
            return isOk;

        }
        /// <summary>
        /// 初始化安规
        /// </summary>
        /// <returns>bool,是否初始化连接成功</returns>
        public bool HYAnGuiInit()
        {
            isHYAnGuiInit = false;
            byte[] WriteBuff = new byte[9];//发送数据
            byte[] ReadBuff = new byte[50];//接收数据
            int ReturnByte = 0;//返回数据
            bool IsReturn = false;//是否成功返回
            bool IsTimeOut = false;//是否超时
            DateTime NowTime = DateTime.Now;//当前时间
            TimeSpan ts;//时间差
            try
            {
                if (!comPort.IsOpen)
                {
                    comPort.Open();
                }
                WriteBuff[0] = Convert.ToByte('?');
                WriteBuff[1] = Convert.ToByte('K');
                WriteBuff[2] = 0x0D;
                WriteBuff[3] = 0x0A;

                comPort.DiscardInBuffer();
                comPort.Write(WriteBuff, 0, 4);
                NowTime = DateTime.Now;
                do
                {
                    if (comPort.BytesToRead >= 42)//收到数据
                    {
                        ReturnByte = comPort.BytesToRead;
                        IsReturn = true;
                    }
                    ts = DateTime.Now - NowTime;
                    if (ts.TotalMilliseconds > timeOut)//时间超时
                    {
                        IsTimeOut = true;
                    }
                } while (!IsReturn && !IsTimeOut);
                if (!IsReturn && IsTimeOut)//超时
                {
                    if (errStr.IndexOf("停止失败,接收数据已超时") < 0)
                    {
                        errStr = errStr + DateTime.Now.ToString() + mName + ":停止失败,接收数据已超时" + (char)13 + (char)10;
                    }
                    isHYAnGuiInit = false;
                }
                else
                {
                    comPort.Read(ReadBuff, 0, ReturnByte);
                    string s = Encoding.ASCII.GetString(ReadBuff);
                    //if ((ReadBuff[0] != WriteBuff[0]) || (ReadBuff[1] != WriteBuff[1]) || (ReadBuff[2] != WriteBuff[2]))//数据检验失败
                    //{
                    //    //comPort.Close(); 
                    //    if (ErrStr.IndexOf("停止失败,接收数据错误") < 0)
                    //    {
                    //        ErrStr = ErrStr + DateTime.Now.ToString() + mName + ":停止失败,接收数据错误" + (char)13 + (char)10;
                    //    }
                    //    return false;
                    //}
                    isHYAnGuiInit = true;
                }
            }
            catch (Exception exc)
            {
                if (errStr.IndexOf(exc.ToString()) < 0)
                {
                    errStr = errStr + DateTime.Now.ToString() + mName + ":" + exc.ToString() + (char)13 + (char)10;
                }
                isHYAnGuiInit = false;
            }
            return isHYAnGuiInit;
        }
    }
    /// <summary>
    /// 安规数据
    /// </summary>
    public class AnGuiData
    {
        /// <summary>
        /// 检测数据
        /// </summary>
        public double[] TestData = new double[testCount];
        /// <summary>
        /// 检测步骤结果
        /// </summary>
        public bool[] TestResult = new bool[testCount];
        /// <summary>
        /// 检测结果
        /// </summary>
        public bool isPass = true;
        /// <summary>
        /// 常量 ,检测数据量
        /// </summary>
        public const int testCount = 4;
        public AnGuiData()
        {
            for (int i = 0; i < testCount; i++)
            {
                TestData[i] = 0;
                TestResult[i] = true;
            }
        }
        public string ToSendValue()
        {
            string result = string.Format("{0}", isPass);
            for (int i = 0; i < TestData.Length; i++)
            {
                result = string.Format("{0}~{1:F3}", result, TestData[i]);
                result = string.Format("{0}~{1}", result, TestResult[i]);
            }
            return result;
        }
        public AnGuiData GetValue(string value)
        {
            AnGuiData result = new AnGuiData();
            string[] buff = value.Split('~');
            int index = 0;
            result.isPass = All.Class.Num.ToBool(buff[index++]);
            for (int i = 0; i < TestData.Length; i++)
            {
                result.TestData[i] = All.Class.Num.ToDouble(buff[index++]);
                result.TestResult[i] = All.Class.Num.ToBool(buff[index++]);
            }
            return result;
        }
    }
}
