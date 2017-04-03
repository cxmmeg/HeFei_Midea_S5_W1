using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO.Ports;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Data;
namespace NewMideaProgram
{
    public delegate void SendTestDataHandle(string data);
    public delegate void SendBarCodeHandle(string data);
    public delegate void FlushControlTextHandle(Label lbl, string data,ErrDataEnum errDataEnum);
    public delegate void SetLabelTextHandle(Label lbl,string str);
    public delegate void SetCheckBoxHandle(CheckBox chk,bool value);
    public delegate bool GetCheckBoxValueHandle(CheckBox chk);
    //public delegate void SendDiQiHandle(string data);
    public partial class frmMain : Form
    {
        public static bool Plc = true;
        public static bool Ft2010 = true;
        public static bool Mod7017_1 = true;
        public static bool Mod7017_2 = true;
        public static bool Mod7017_3 = true;
        public static bool Mod7017_4 = true;
        public static bool AnGui7440 = true;
        public static bool AnGui7623 = true;
        public static bool ChouKong = true;
        public static int NowStatue = 0;

        public static All.Class.LockMain<string> lockMainAnGui;
        public static All.Class.LockClient<string> lockClientAnGui;
        public static All.Class.LockMultiChannleMain<string> lockMainEmpty;
        public static All.Class.LockClient<string> lockClientEmpty;

        uint[] QuXianColor = new uint[] {0xFF,0xFFFF, 0xFF0000, 0xFF00 };
        Chanel[] chanelCurve = new Chanel[5];//电流,功率,压力,温差
        public static cMain mMain = new cMain();
        SerialPort Com1;
        SerialPort Com2;
        //SerialPort Com9;
        SerialPort Com10;
        SerialPort Com11;
        SerialPort Com12;
        SerialPort Com13;

        public static cMain.NowStatue nowStatue = new cMain.NowStatue();
        int Read2010Err = 0;//, ReadWenDuErr2 = 0, ReadWenDuErr3 = 0, ReadWenDuErr4 = 0, ReadWenDuErr5 = 0;
        int ReadPlcErr = 0, readDataBarErr = 0, Read7017Err1 = 0, Read7017Err2 = 0, Read7017Err3 = 0, Read7017Err4 = 0;
        long HighElectErr = 0, HighPresErr = 0;
        int UpdataCount = 0;//网络更新按键计数

        
        S7Modbus mPlc;
        S7Modbus mEmpty;
        c7017 m7017_1;
        c7017 m7017_2;
        c7017 m7017_3;
        c7017 m7017_4;
        cQz8902F m8902;
        cBar mBarCode;
        cHYAnGui m7440;
        cHYAnGui m7623;
        All.Machine.Media.PQ PQ1 = new All.Machine.Media.PQ(cMain.mSysSet.ComPQ1, All.Machine.Media.AllMachine.Machines.商用V4);
        All.Machine.Media.PQ PQ2 = new All.Machine.Media.PQ(cMain.mSysSet.ComPQ2, All.Machine.Media.AllMachine.Machines.商用三管制);
        All.Machine.Media.H1H2 H1H2 = new All.Machine.Media.H1H2(cMain.mSysSet.ComH1H2, All.Machine.Media.AllMachine.Machines.商用三管制);
        All.Machine.Media.XY XY = new All.Machine.Media.XY(cMain.mSysSet.ComXY, All.Machine.Media.AllMachine.Machines.商用三管制);
        All.Machine.Media.K1K2 K1K2 = new All.Machine.Media.K1K2(cMain.mSysSet.ComK1K2, All.Machine.Media.AllMachine.Machines.商用三管制);


        public static bool[] ReadPlcMValue = new bool[cMain.DataPlcMPoint];

        cUdpSock udpSend;
        bool is_J_OK = false;
        bool is_R_OK = false;
        long PreFlushDataTime = 0;
        public string ErrStr = "";//出错信息
        bool IsOutSystem = false;//是否要退出程序
        bool IsInitOver = false;//是否完成启动输出,在等待低启时,以免重复输出 写PLC数据
        bool IsStartOut = false;
        //bool IsEmptyOver = false;//抽真空动作是否输出完毕
        //bool IsAnGuiOver = false;
        public static bool AnGuiStart = false, AnGuiStop = false;
        string[] emptyStart;
        frmAnGui fAnGui;
        frmEmpty fEmpty;

        bool IsEndStepOut = false;//是否完成步骤结束控制.
        string ReadBarCode = "";
        public static bool Plc_Start = false, Handle_Start = false, Net_Start = false;
        public static bool Plc_Stop = false, Handle_Stop = false, Net_Stop = false, Pro_Stop = false;
        public static bool Plc_Next = false, Handle_Next = false, Net_Next = false;
        public static bool Plc_OK = false, Plc_NG = false, Plc_Reset = false, Plc_Pause = false;
        public static bool[] Plc_AnGui = new bool[4];
        public static bool[] Plc_Empty = new bool[3];
        public static bool Plc_Test = false;
        public static bool[] Plc_Out = new bool[cMain.DataPlcMPoint];
        


        //PLC M点输出
        public static Hashtable Plc_Out_MPoint = new Hashtable();
        public static Hashtable Temp_Out_MPoint = new Hashtable();
        public static Hashtable Plc_M_MPoint = new Hashtable();
        //PLC D点输出
        public static Hashtable Plc_Out_DPoint = new Hashtable();
        public static Hashtable Temp_Out_DPoint = new Hashtable();
        public static Hashtable Plc_D_DPoint = new Hashtable();


        public static string Temp_Step_SnCode = "";//步骤设置指令
        public static int InitSnJiQi = 0;

        //要读的D点
        public static long[] Plc_In_D = new long[14];//共14个D点(M点打包,备用,压力1,压力2,压力3,压力4,温度1,温度2,温度3,温度4,温度5,温度6,温度7,温度8)
        public static bool[] Plc_In_M;
        //电参数
        public static double[] Ft2010_Read = new double[9];//电参数
        //SN板
        public static long[] SnBoard_Read = new long[21];//SN板
        public static double[] SnBoard_Show = new double[6];//SN板
        //压力
        public static double[] Plc_In_YaLi = new double[4];//压力
        //温度
        public static double[] Plc_In_WenDu = new double[8];//温度

        public static double[] dataRead = new double[cMain.DataAll];
        public static double[] dataShow = new double[cMain.DataShow];
        cError mError = new cError();
        public const int NeiJiCount = 2;
        Label[] nowLabel = new Label[cMain.DataShow];
        BlinkLed[] XinHao = new BlinkLed[6];
        BlinkLed[] KaiGuan = new BlinkLed[11];
        BlinkLed[] BaoHu = new BlinkLed[7];
        Label[] lblProtect = new Label[7];
        public frmMain()//构造函数
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < cMain.DataShow; i++)
            {
                nowLabel[i] = (Label)this.Controls.Find(string.Format("lblDataShow{0}", i + 1), true)[0];
            }
            for (int i = 0; i < XinHao.Length; i++)
            {
                XinHao[i] = (BlinkLed)this.Controls.Find(string.Format("blinkLed{0}", i + 1), true)[0];
            }
            for (int i = 0; i < KaiGuan.Length; i++)
            {
                KaiGuan[i] = (BlinkLed)this.Controls.Find(string.Format("blinkLed{0}", i + 7), true)[0];
            }
            for (int i = 0; i < BaoHu.Length; i++)
            {
                BaoHu[i] = (BlinkLed)this.Controls.Find(string.Format("blinkLed{0}", i + 18), true)[0];
                lblProtect[i] = (Label)this.Controls.Find(string.Format("lblProtect{0}", i + 1), true)[0];
                lblProtect[i].Text = Enum.GetNames(typeof(All.Machine.Media.K1K2.Protects))[i];
            }
            StartLoad();
        }
        public void StartLoad()
        {
            ChangeForm(cMain.LocalSaveValue.FormIndex);
            frmInit(this);//窗体初始化
            init();//参数初始化
            ComUdpInit();
            initTestData();
            initNowStatue();
            //fs.mFrmMain = this;
            MainLoop.Enabled = true;
        }
        private static void frmInit(frmMain mFrmMain)//实现各控件自动位置,基本不用动
        {

            DataTable dt = new DataTable();
            mFrmMain.dataGridNow.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            for (int i = 0; i < cMain.DataShow; i++)
            {
                dt.Columns.Add(cMain.DataShowTitle[i], typeof(string));
                //dt.Columns[i].Caption = cMain.DataShowTitle[i];
            }
            DataRow dr = dt.NewRow();
            for (int i = 0; i < cMain.DataShow; i++)
            {
                dr[i] = "0.000";
            }
            dt.Rows.Add(dr);
            mFrmMain.dataGridNow.Font = new Font("宋体", 10);
            mFrmMain.dataGridNow.DataSource = dt;
            if (cMain.IndexLanguage == 1)
            {
                mFrmMain.dataGridNow.Rows[0].HeaderCell.Value = "数据";
            }
            else
            {
                mFrmMain.dataGridNow.Rows[0].HeaderCell.Value = "Data";
            }
            for (int i = 0; i < mFrmMain.dataGridNow.Columns.Count; i++)
            {
                mFrmMain.dataGridNow.Columns[i].Width = 80;
                //mFrmMain.dataGridNow.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            mFrmMain.dataGridNow.CurrentCell = null;//将自动选择的单元去掉,被选择时,会有个自动样式,阻碍绘画背景红色
            for (int i = 0; i < mFrmMain.dataGridNow.ColumnCount; i++)
            {
                mFrmMain.dataGridNow.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            //btnQuXianSet.Location = new Point(axiPlotX1.Location.X + 224, axiPlotX1.Location.Y+5);
        }
        private void SetLabelText(Label lbl, string str)
        {
            if (lbl.InvokeRequired)
            {
                lbl.Invoke(new SetLabelTextHandle(SetLabelText),lbl, str);
            }
            else
            {
                lbl.Text = str;
            }
        }
        private void SetCheckBox(CheckBox chk, bool value)
        {
            if (chk.InvokeRequired)
            {
                chk.Invoke(new SetCheckBoxHandle(SetCheckBox), chk, value);
            }
            else
            {
                chk.Checked = value;
            }
        }
        private bool GetCheckBoxValue(CheckBox chk)
        {
            bool result = false;
            if (chk.InvokeRequired)
            {
                result = (bool)chk.Invoke(new GetCheckBoxValueHandle(GetCheckBoxValue), chk);
            }
            else
            {
                result = chk.Checked;
            }
            return result;
        }
        private void FlushControlText(Label sender, string data,ErrDataEnum errDataEnum)
        {
            if (sender.InvokeRequired)
            {
                sender.BeginInvoke(new FlushControlTextHandle(FlushControlText), sender, data);
            }
            else
            {
                switch (errDataEnum)
                {
                    case ErrDataEnum.Info:
                        sender.ForeColor = Color.Black;
                        sender.BackColor = this.BackColor;
                        sender.Font = new Font(new Font("Times New Roman", 12), FontStyle.Regular);
                        break;
                    case ErrDataEnum.Error:
                        sender.ForeColor = Color.Red;
                        sender.BackColor = Color.LightGreen;
                        sender.Font = new Font(new Font("Times New Roman", 14), FontStyle.Bold);
                        break;
                    case ErrDataEnum.Protect:
                        sender.ForeColor = Color.Blue;
                        sender.BackColor = Color.Yellow;
                        sender.Font = new Font(new Font("Times New Roman", 14), FontStyle.Bold);
                        break;
                }
                sender.Text = data;
            }
        }
        private void MsgBox(string ErrorStr)
        {
            if (lblPartInfo.InvokeRequired)
            {
                lblPartInfo.BeginInvoke(new EventHandler(MsgBox), ErrorStr);
            }
            else
            {
                lblPartInfo.Text = ErrorStr;
            }
        }
        private void MsgBox(object sender, EventArgs e)
        {
            lblPartInfo.Text = sender.ToString();
        }
        private void PringfHelp(object sender, EventArgs e)
        {
            lblPartInfo.Text = sender.ToString();
        }
        private void ShowThis(object sender, EventArgs e)
        {
            bool b = bool.Parse(sender.ToString());
            if (b)
            {
                this.Visible = true;
            }
        }
        private bool init()//程序开始初始化
        {
            int i;
            for (i = 0; i < Plc_In_YaLi.Length; i++)
            {
                Plc_In_YaLi[i] = new long();
            }
            for (i = 0; i < Plc_In_WenDu.Length; i++)
            {
                Plc_In_WenDu[i] = new Single();
            }
            for (i = 0; i < SnBoard_Read.Length; i++)
            {
                SnBoard_Read[i] = new int();
            }
            for (i = 0; i < Ft2010_Read.Length; i++)
            {
                Ft2010_Read[i] = new Single();
            }
            for (i = 0; i < cMain.DataPlcMPoint; i++)
            {
                Plc_Out_MPoint.Add(cMain.DataPlcMPointStr[i], false);
                Temp_Out_MPoint.Add(cMain.DataPlcMPointStr[i], false);
                Plc_M_MPoint.Add(cMain.DataPlcMPointStr[i], cMain.DataPlcMPointInt[i]);
            }
            for (i = 0; i < cMain.DataPlcDPoint; i++)
            {
                Plc_Out_DPoint.Add(cMain.DataPlcDPointStr[i], 0);
                Temp_Out_DPoint.Add(cMain.DataPlcDPointStr[i], false);
                Plc_D_DPoint.Add(cMain.DataPlcDPointStr[i], cMain.DataPlcDPointInt[i]);
            }
            for (i = 0; i < Plc_AnGui.Length; i++)
            {
                Plc_AnGui[i] = false;
            }
            for (i = 0; i < Plc_Empty.Length; i++)
            {
                Plc_Empty[i] = false;
            }
            for (i = 0; i < Plc_Out.Length; i++)
            {
                Plc_Out[i] = false;
            }

            chanelCurve[0] = new Chanel(true, Color.Red, true);//电流
            chanelCurve[1] = new Chanel(true, Color.Orange, true);//电流
            chanelCurve[2] = new Chanel(true, Color.Wheat, true);//电流

            chanelCurve[3] = new Chanel(true, Color.SkyBlue, false);//
            chanelCurve[4] = new Chanel(true, Color.Yellow, false);//

            chanelCurve[0].ChanelName = " - A相电流";
            chanelCurve[1].ChanelName = " - B相电流";
            chanelCurve[2].ChanelName = " - C相电流";
            chanelCurve[3].ChanelName = " - 进管压力";
            chanelCurve[4].ChanelName = " - 出管压力";

            quXianControl1.Chanel.Add(chanelCurve[0]);
            quXianControl1.Chanel.Add(chanelCurve[1]);
            quXianControl1.Chanel.Add(chanelCurve[2]);
            quXianControl1.Chanel.Add(chanelCurve[3]);
            quXianControl1.Chanel.Add(chanelCurve[4]);
            quXianControl1.reSet();

            //axiPlotX1.get_Channel(0).TitleText = "电流";
            //axiPlotX1.get_Channel(1).TitleText = "功率";
            //axiPlotX1.get_Channel(2).TitleText = "压力";
            //axiPlotX1.get_Channel(3).TitleText = "频率";
            return true;
        }
        private bool ComUdpInit()//通讯初始化
        {
            bool returnValue = true;
            try
            {
                lblIp.Text = string.Format("IP:{0}", cMain.LocalSaveValue.LocalIp);
                if (All.Class.Check.isFix(cMain.LocalSaveValue.LocalIp, All.Class.Check.RegularList.IP地址))
                {
                    cMain.ThisNo = Num.IntParse(cMain.LocalSaveValue.LocalIp.Split('.')[3]);
                }
                int mRemotPort = cMain.ThisNo + 3000;
                //lblPort.Text = mRemotPort.ToString();
                udpSend = new cUdpSock(3000, mRemotPort, cMain.RemoteHostName);
                udpSend.fUdpSend("A~OK~OVER~" + cMain.ThisNo.ToString());
                udpSend.mDataReciveString += new cUdpSock.mDataReciveStringEventHandle(udpSend_mDataReciveString);

                if (cMain.LocalSaveValue.LocalIp == cMain.LocalSaveValue.AnGuiParentIp)
                {
                    lockMainAnGui = new All.Class.LockMain<string>(14444);
                    lockMainAnGui.AllTestNeedOk += lockMain_AllTestNeedOk;
                    lockMainAnGui.AllTestCancel += lockMainAnGui_AllTestCancel;
                }
                lockClientAnGui = new All.Class.LockClient<string>(cMain.LocalSaveValue.AnGuiParentIp, 14444, 14445, cMain.LocalSaveValue.LocalIp);
                if (cMain.LocalSaveValue.LocalIp == cMain.LocalSaveValue.EmptyParentIp)
                {
                    lockMainEmpty = new All.Class.LockMultiChannleMain<string>(14446, 2);
                    emptyStart = new string[2];
                    All.Class.LockMultiChannleMain<string>.Channel cc = new All.Class.LockMultiChannleMain<string>.Channel();
                    cc.EveryChannel.Add("192.168.1.161");
                    cc.EveryChannel.Add("192.168.1.162");
                    lockMainEmpty.AllChannel.Add(cc);
                    cc = new All.Class.LockMultiChannleMain<string>.Channel();
                    cc.EveryChannel.Add("192.168.1.163");
                    cc.EveryChannel.Add("192.168.1.164");
                    lockMainEmpty.AllChannel.Add(cc);
                    lockMainEmpty.AllTestNeedOk += lockMainEmpty_AllTestNeedOk;
                    lockMainEmpty.AllTestCancel += lockMainEmpty_AllTestCancel;
                    lockMainEmpty.AllTestOk += lockMainEmpty_AllTestOk;
                }
                lockClientEmpty = new All.Class.LockClient<string>(cMain.LocalSaveValue.EmptyParentIp, 14446, 14447, cMain.LocalSaveValue.LocalIp);
            }
            catch
            {
                cMain.WriteErrorToLog("初始化UDP端口失败");
                mError.AddErrData("UDP_ERROR", "初始化UDP端口失败");//,Udp Error".Split(',')[cMain.IndexLanguage]);
                returnValue = false;
            }
            new Thread(() => doThreadCom1())
            {
                IsBackground = true
            }.Start();
            new Thread(() => doThreadCom2())
            {
                IsBackground = true
            }.Start();
            new Thread(() => doThreadComA())
            {
                IsBackground = true
            }.Start();

            if (cMain.LocalSaveValue.LocalIp == cMain.LocalSaveValue.AnGuiParentIp)
            {
                new Thread(() => doThreadComB())
                {
                    IsBackground = true
                }.Start();
                new Thread(() => doThreadComC())
                {
                    IsBackground = true
                }.Start();
            }
            if (cMain.LocalSaveValue.LocalIp == cMain.LocalSaveValue.EmptyParentIp)
            {
                new Thread(() => doThreadComD())
                {
                    IsBackground = true
                }.Start();
            }
            
            return returnValue;
        }

        void lockMainEmpty_AllTestOk(int ChannelIndex, string Index)
        {
            emptyStart[ChannelIndex] = "";
        }

        void lockMainEmpty_AllTestCancel(int ChannelIndex, string Index)
        {
            emptyStart[ChannelIndex] = "";
        }

        void lockMainAnGui_AllTestCancel()
        {
            AnGuiStop = true;
        }

        private void lockMainEmpty_AllTestNeedOk(int ChannelIndex, string Index)
        {
            emptyStart[ChannelIndex] = Index;
        }
        
        private void lockMain_AllTestNeedOk()
        {
            AnGuiStart = true;
        }
        private void SendBarToComputer(string sendStr)
        {
            long startTime = 0;
            int timeOut = 1200;
            bool isTimeOut = false;
            for (int i = 0; i < 3; i++)
            {
                is_R_OK = false;
                isTimeOut = false;
                startTime = Environment.TickCount;
                do
                {
                    if ((Environment.TickCount - startTime) > timeOut)
                    {
                        isTimeOut = true;
                    }
                } while (!isTimeOut && (is_R_OK));
                if (is_R_OK)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(2000);
                }
            }
            Thread.CurrentThread.Abort();
        }
        private void SendDataToComputer(string sendStr)
        {
            long startTime = 0;
            int timeOut = 1200;
            bool isTimeOut = false;
            for (int i = 0; i < 3; i++)
            {
                is_J_OK = false;
                isTimeOut = false;
                startTime = Environment.TickCount;
                udpSend.fUdpSend(sendStr);
                do
                {
                    if ((Environment.TickCount - startTime) > timeOut)
                    {
                        isTimeOut = true;
                    }
                } while (!isTimeOut && (!is_J_OK));
                if (is_J_OK)
                {
                    break;
                }
                else
                {
                    Thread.Sleep(2000);
                }
            }
            Thread.CurrentThread.Abort();
        }
        private void udpSend_mDataReciveString(object o, RecieveStringArgs e)//UDP接收数据,(有点木马的原型 ^_^)
        {
            string GetString = e.ReadStr;
            if (GetString == string.Empty)
            {
                return;
            }
            string RemoteIp = udpSend.pRemoteHostIp;
            int RemotePort = udpSend.pRemoteHostPort;
            string RemotCmd = GetString.Substring(0, 1).ToUpper();
            switch (RemotCmd)
            {
                case "C":
                    cSnSet.DataClassToXml(cSnSet.DataStrToClass(GetString));
                    udpSend.fUdpSend(RemoteIp, RemotePort, "C~OK");
                    break;
                case "B"://条码与设置
                    if (DataStrToClass(GetString, out cMain.mNetModeSet))
                    {
                        cMain.mModeSet = cMain.mNetModeSet.ModeSet;
                        udpSend.fUdpSend(RemoteIp, RemotePort, "B~OK");
                    }
                    else
                    {
                        udpSend.fUdpSend(RemoteIp, RemotePort, "B~ERROR");
                    }
                    cMain.mNetModeSet.ModeSet.Save();
                    break;
                case "S"://系统设置
                    if (DataStrToClass(GetString, out cMain.mSysSet))
                    {
                        udpSend.fUdpSend(RemoteIp, RemotePort, "S~OK");
                    }
                    else
                    {
                        udpSend.fUdpSend(RemoteIp, RemotePort, "S~ERROR");
                    }
                    frmSys.DataClassToFile(cMain.mSysSet);
                    break;
                case "R"://条码上传成功后,上位机会返回此标志
                    if (GetString.IndexOf("R~OK") >= 0)
                    {
                        is_R_OK = true;
                    }
                    break;
                case "J"://检测数据上传成功后,上位机会返回此标志
                    if (GetString.IndexOf("J~OK") >= 0)
                    {
                        is_J_OK = true;
                    }
                    break;
                case "X"://行程开关,这里不要了.
                    break;
                case "D"://询问当前数据
                    string sendData = "D~OK~" + DataClassToStr(cMain.mTempNetResult);
                    udpSend.fUdpSend(RemoteIp, RemotePort, sendData);
                    break;
                case "P"://WINCE没有PING命令,这相当于PING命令
                    udpSend.fUdpSend(RemoteIp, RemotePort, "P~OK");
                    break;
                case "U"://系统更新
                    this.BeginInvoke(new EventHandler(updataByNet));
                    break;
                case "T"://停止测试
                    Net_Stop = true;
                    udpSend.fUdpSend(RemoteIp, RemotePort, "T~OK");
                    break;
                case "N"://下一步
                    Net_Next = true;
                    udpSend.fUdpSend(RemoteIp, RemotePort, "N~OK");
                    break;
                case "K"://开始测试
                    Net_Start = true;
                    udpSend.fUdpSend(RemoteIp, RemotePort, "K~OK");
                    break;
                //case "V"://显示出来,这里也没用的-.-!#
                //    this.BeginInvoke(new EventHandler(ShowThis), "ture");
                //    break;
            }
        }
        private void updataByNet(object o, EventArgs e)
        {
            if (!cMain.isComPuter)
            {
                //显示任务栏
                int TaskBarHandle;
                TaskBarHandle = cAPI.FindWindowW("HHTaskBar", null);
                if (TaskBarHandle != 0)
                {
                    TaskBarHandle = cAPI.ShowWindow(TaskBarHandle, 4);//9为恢复原来
                }
            }
            udpSend.fUdpSend(udpSend.pRemoteHostIp, udpSend.pRemoteHostPort, "U~OK");
            if (File.Exists(cMain.AppPath+"\\UpdataByNet.exe"))
            {
                Process.Start(cMain.AppPath+"\\UpdataByNet.exe", "");
            }
            frmClose();
            this.Close();
            Application.Exit();
        }
        private void doThreadCom1()
        {
            byte[] buff;
            bool[] mValue;
            int index = 0;
            if (!cMain.isDebug)
            {
                try
                {
                    Com1 = new SerialPort(cMain.mSysSet.ComPlc, 9600, Parity.None, 8, StopBits.One);
                    if (Com1.IsOpen)
                    {
                        Com1.Close();
                    }
                    Com1.Open();
                }
                catch
                {
                    mError.AddErrData(cMain.mSysSet.ComPlc + "_ERROR", "打开通讯端口" + (cMain.mSysSet.ComPlc + "失败"));//,"+cMain.mSysSet.mPLCCOM+"ERROR").Split(',')[cMain.IndexLanguage]);
                }
            }
            mPlc = new S7Modbus(Com1, 3);
            while (!IsOutSystem)
            {
                for (int i = 0; i < Temp_Out_MPoint.Count; i++)
                {
                    if (Num.BoolParse(Temp_Out_MPoint[cMain.DataPlcMPointStr[i]]))
                    {
                        ChangeM(Num.BoolParse(Plc_Out_MPoint[cMain.DataPlcMPointStr[i]]), All.Class.Num.ToString(Plc_M_MPoint[cMain.DataPlcMPointStr[i]]));
                        Temp_Out_MPoint[cMain.DataPlcMPointStr[i]] = false;
                    }
                }
                if (mPlc.ReadVB(0, 72, out buff))
                {
                    index = 0;
                    mValue = All.Class.Num.Byte2Bool(buff, 1, 2);
                    for (int i = 0; i < Plc_AnGui.Length; i++)
                    {
                        Plc_AnGui[i] = mValue[i];
                    }
                    for (int i = 0; i < Plc_Empty.Length; i++)
                    {
                        Plc_Empty[i] = mValue[i + 4];
                    }
                    Plc_Test = mValue[10];

                    if (buff[10] == 1)
                    {
                        Plc_Start = true;
                        Plc_Out_MPoint["Start"] = false; Temp_Out_MPoint["Start"] = true;
                    }
                    if (buff[12] == 1)
                    {
                        Plc_OK = true;
                        Plc_Out_MPoint["Ok"] = false; Temp_Out_MPoint["Ok"] = true;
                    }
                    if (buff[14] == 1)
                    {
                        Plc_NG = true;
                        Plc_Out_MPoint["Ng"] = false; Temp_Out_MPoint["Ng"] = true;
                    }
                    if (buff[16] == 1)
                    {
                        Plc_Reset = true;
                        Plc_Out_MPoint["Rest"] = false; Temp_Out_MPoint["Rest"] = true;
                    }
                    if (buff[18] == 1)
                    {
                        Plc_Next = true;
                        Plc_Out_MPoint["Step"] = false; Temp_Out_MPoint["Step"] = true;
                    }
                    if (buff[20] == 1)
                    {
                        Plc_Stop = true;
                        Plc_Out_MPoint["Stop"] = false; Temp_Out_MPoint["Stop"] = true;
                    }
                    if (buff[22] == 1)
                    {
                        Plc_Pause = true;
                        Plc_Out_MPoint["Pause"] = false; Temp_Out_MPoint["Pause"] = true;
                    }
                    for (int i = 21; i < 34; i = i + 2)
                    {
                        if (buff[i] == 1)
                        {
                            ReadPlcMValue[index++] = true;
                        }
                        else
                        {
                            ReadPlcMValue[index++] = false;
                        }
                    }
                    for (int i = 30; i < 71; i = i + 2)
                    {
                        if (buff[i] == 1)
                        {
                            ReadPlcMValue[index++] = true;
                        }
                        else
                        {
                            ReadPlcMValue[index++] = false;
                        }
                    }
                    ReadPlcErr = 0;
                    Plc = true;
                    mError.DelErrData("PLC_ERROR");
                }
                else
                {
                    ReadPlcErr++;
                    if (ReadPlcErr >= 3)
                    {
                        Plc = false;
                        mError.AddErrData("PLC_ERROR", "PLC通讯失败,请检查屏幕和PLC通讯连接");
                    }
                }
                //if (mPlc.FxPlc_ReadM(0, 4, out readValue))
                //{
                //    mError.DelErrData("PLC_ERROR");
                //    ReadPlcErr = 0;
                //    ReadPlcMValue = Num.Int2Bool(Num.ChangeHighAndLow(readValue));

                //    if (ReadPlcMValue[0])
                //    {
                //        Plc_Start = true;
                //        ChangeM(false, 0);
                //    }
                //    if (ReadPlcMValue[1])
                //    {
                //        Plc_Stop = true;
                //        ChangeM(false, 1);
                //    }
                //}
                //else
                //{
                //    ReadPlcErr++;
                //    if (ReadPlcErr >= 3)
                //    {
                //        mError.AddErrData("PLC_ERROR", "PLC通讯失败,请检查屏幕和PLC通讯连接");//,PLC Error".Split(',')[cMain.IndexLanguage]);
                //    }
                //}
                Thread.Sleep(50);
            }
        }
        private void doThreadCom2()
        {
            double[] tmp = new double[8];
            //double tmpPress = 0;
            int index = 0;
            cQz8902F.cReadDataFormat tmpElect = new cQz8902F.cReadDataFormat();
            if (!cMain.isDebug)
            {
                try
                {
                    Com2 = new SerialPort(cMain.mSysSet.Com485, 9600, Parity.None, 8, StopBits.One);
                    if (Com2.IsOpen)
                    {
                        Com2.Close();
                    }
                    Com2.Open();
                }
                catch
                {
                    mError.AddErrData(cMain.mSysSet.Com485 + "_ERROR", "打开通讯端口" + (cMain.mSysSet.Com485 + "失败"));//," + cMain.mSysSet.m485COM + "ERROR").Split(',')[cMain.IndexLanguage]);
                }
            }
            #region//初始化
            m7017_1 = new c7017(Com2, 1, 200);
            //if (!m7017_1.init())
            //{
            //    mError.AddErrData("7017_Error", "7017通讯失败");
            //}
            m7017_2 = new c7017(Com2, 2, 200);
            //if (!m7017_2.init())
            //{
            //    mError.AddErrData("7017_Error", "7017通讯失败");
            //}
            m7017_3 = new c7017(Com2, 3, 200);
            //if (!m7017_3.init())
            //{
            //    mError.AddErrData("7017_Error", "7017通讯失败");
            //}
            m7017_4 = new c7017(Com2, 4, 400);
            //if (!m7017_4.init())
            //{
            //    mError.AddErrData("7017_Error", "7017通讯失败");
            //}
            m8902 = new cQz8902F(Com2, 5);
            #endregion
            while (!IsOutSystem)//系统没有退出
            {
                index = 0;
                #region//读电参数
                if (m8902.Read(ref tmpElect))
                {
                    Read2010Err = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        dataRead[index++] = tmpElect.SingleVol[i];
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        dataRead[index++] = tmpElect.SingleCur[i];
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        dataRead[index++] = tmpElect.SinglePow[i];
                    }
                    Ft2010 = true;
                    mError.DelErrData("FT2010_ERROR");
                }
                else
                {
                    if (cMain.isDebug)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            dataRead[index++] = index + Num.Rand();
                        }
                    }
                    else
                    {
                        Read2010Err++;
                        if (Read2010Err >= 3)
                        {
                            mError.AddErrData("FT2010_ERROR", "电参数仪通讯失败");
                            Ft2010 = false;
                            Read2010Err = 3;
                            for (int i = 0; i < 9; i++)
                            {
                                dataRead[index++] = -99;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 9; i++)
                            {
                                index++;
                            }
                        }
                    }
                }
                #endregion
                Thread.Sleep(50);
                #region//读7017  1#
                if (m7017_1.ReadData(ref tmp))
                {
                    Read7017Err1 = 0;
                    mError.DelErrData("7017_Error");
                    Mod7017_1 = true;
                    for (int i = 0; i < 8; i++)
                    {
                        dataRead[index++] = tmp[i] * 45 - 75;
                    }
                }
                else
                {
                    if (cMain.isDebug)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            dataRead[index++] = index + Num.Rand();
                        }
                    }
                    else
                    {
                        Read7017Err1++;
                        if (Read7017Err1 >= 3)
                        {
                        Mod7017_1 = false;
                        mError.AddErrData("7017_Error", "1# 7017通讯失败");
                            Read7017Err1 = 3;
                            for (int i = 0; i < 8; i++)
                            {
                                dataRead[index++] = -99;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                index++;
                            }
                        }
                    }
                }
                #endregion
                Thread.Sleep(50);
                #region//读7017  2#
                if (m7017_2.ReadData(ref tmp))
                {
                    Read7017Err2 = 0;
                    mError.DelErrData("7017_Error");
                    Mod7017_2 = true;
                    for (int i = 0; i < 8; i++)
                    {
                        switch (i)
                        {
                            case 6:
                            case 7:
                                dataRead[index++] = tmp[i] * 5 - 5;
                                break;
                            default:
                                dataRead[index++] = tmp[i] * 45 - 75;
                                break;
                        }
                    }
                }
                else
                {
                    if (cMain.isDebug)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            dataRead[index++] = index + Num.Rand();
                        }
                    }
                    else
                    {
                        Read7017Err2++;
                        if (Read7017Err2 >= 3)
                        {
                            Mod7017_2 = false;
                            mError.AddErrData("7017_Error", "2# 7017通讯失败");
                            Read7017Err2 = 3;
                            for (int i = 0; i < 7; i++)
                            {
                                dataRead[index++] = -99;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 7; i++)
                            {
                                index++;
                            }
                        }
                    }
                }
                #endregion
                Thread.Sleep(50);
                #region//读7017  3#
                if (m7017_3.ReadData(ref tmp))
                {
                    Read7017Err3 = 0;
                    mError.DelErrData("7017_Error");
                    Mod7017_3 = true;
                    for (int i = 0; i < 6; i++)
                    {
                        dataRead[index++] = tmp[i];
                    }
                }
                else
                {
                    if (cMain.isDebug)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            dataRead[index++] = index + Num.Rand();
                        }
                    }
                    else
                    {
                        Read7017Err3++;
                        if (Read7017Err3 >= 3)
                        {
                            Read7017Err3 = 3;
                            Mod7017_3 = false;
                            mError.AddErrData("7017_Error", "3# 7017通讯失败");
                            for (int i = 0; i < 6; i++)
                            {
                                dataRead[index++] = -99;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                index++;
                            }
                        }
                    }
                }
                #endregion
                Thread.Sleep(50);
                #region//读7017  4#
                if (m7017_4.ReadData(ref tmp))
                {
                    Read7017Err4 = 0;
                    mError.DelErrData("7017_Error");
                    Mod7017_4 = true;
                    for (int i = 0; i < 5; i++)
                    {
                        switch (i)
                        {
                            default:
                                dataRead[index++] = tmp[i] * 1.275 - 1.375;
                                break;
                            case 4:
                                dataRead[index++] = (tmp[i] - 0.5) / 0.8696f;
                                break;
                        }
                    }
                }
                else
                {
                    if (cMain.isDebug)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            dataRead[index++] = index + Num.Rand();
                        }
                    }
                    else
                    {
                        Read7017Err4++;
                        if (Read7017Err4 >= 3)
                        {
                            Mod7017_4 = false;
                            mError.AddErrData("7017_Error", "4# 7017通讯失败");
                            Read7017Err4 = 3;
                            for (int i = 0; i < 5; i++)
                            {
                                dataRead[index++] = -99;
                            }
                        }
                        else
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                index++;
                            }
                        }
                    }
                }
                #endregion
                Thread.Sleep(50);
                
            }
        }
        private void doThreadComA()
        {
            string ReadDataBarCode = "";
            if (!cMain.isDebug)
            {
                try
                {
                    Com10 = new SerialPort(cMain.mSysSet.ComBar, 9600, Parity.None, 8, StopBits.One);
                    if (Com10.IsOpen)
                    {
                        Com10.Close();
                    }
                    Com10.Open();
                }
                catch
                {
                    mError.AddErrData(cMain.mSysSet.ComBar + "_ERROR", "打开通讯端口" + (cMain.mSysSet.ComBar + "失败"));//," + cMain.mSysSet.mBianPing_1 + "Error").Split(',')[cMain.IndexLanguage]);
                }
            }
            mBarCode = new cBar(Com10, 400);
            while (!IsOutSystem)
            {
                if (!mBarCode.readBarCode(ref ReadDataBarCode))
                {
                    readDataBarErr++;
                    if (readDataBarErr >= 3)
                    {
                        MsgBox("数据扫描条码读取失败");
                    }

                }
                else
                {
                    ReadDataBarCode = Num.trim(ReadDataBarCode);
                    if (ReadDataBarCode != "")
                    {
                        ReadBarCodeOver(ReadDataBarCode);
                    }

                }
                Thread.Sleep(500);
            }
        }
        private void doThreadComB()
        {
            if (!cMain.isDebug)
            {
                try
                {
                    Com11 = new SerialPort(cMain.mSysSet.Com7440, 9600, Parity.None, 8, StopBits.One);
                    if (Com11.IsOpen)
                    {
                        Com11.Close();
                    }
                    Com11.Open();
                }
                catch
                {
                    mError.AddErrData(cMain.mSysSet.Com7440 + "_ERROR", "打开通讯端口" + (cMain.mSysSet.Com7440 + "失败"));//,"+cMain.mSysSet.mPLCCOM+"ERROR").Split(',')[cMain.IndexLanguage]);
                }
                try
                {
                    Com12 = new SerialPort(cMain.mSysSet.Com125, 9600, Parity.None, 8, StopBits.One);
                    if (Com12.IsOpen)
                    {
                        Com12.Close();
                    }
                    Com12.Open();
                }
                catch
                {
                    mError.AddErrData(cMain.mSysSet.Com125 + "_ERROR", "打开通讯端口" + (cMain.mSysSet.Com125 + "失败"));//,"+cMain.mSysSet.mPLCCOM+"ERROR").Split(',')[cMain.IndexLanguage]);
                }
            }
            m7440 = new cHYAnGui(Com11);
            m7623 = new cHYAnGui(Com12);
            cHYAnGui.Hy7440 = m7440;
            cHYAnGui.Hy7623 = m7623;
            int startTime = 0;
            AnGuiData tmpAnGuiData = new AnGuiData();
            bool testOver = false;
            while (!IsOutSystem)
            {
                if (AnGuiStart)
                {
                    AnGuiStart = false;
                    AnGui7623 = m7623.HYAnGuiStart(cHYAnGui.AnGuiList.HY125);
                    startTime = Environment.TickCount;
                }
                if (AnGuiStop)
                {
                    AnGuiStop = false;
                    m7623.HYAnGuiStop(cHYAnGui.AnGuiList.HY125);
                    AnGui7440 = m7440.HYAnGuiStop(cHYAnGui.AnGuiList.HY7440);
                    startTime = 0;
                }
                if (startTime > 0)
                {
                    if ((Environment.TickCount - startTime) >= cMain.LocalSaveValue.AnGuiTimeOut)
                    {
                        cHYAnGui.ReadData(tmpAnGuiData, cHYAnGui.AnGuiList.BothTwo, ref testOver);
                        lockMainAnGui.AllTestOver(tmpAnGuiData.ToSendValue());
                        AnGuiStop = true;
                    }
                }
                Thread.Sleep(1000);
            }
        }
        private void doThreadComC()
        { }
        private void doThreadComD()
        {
            long chouKongError = 0;
            byte[] buff;
            double[] emptyValue = new double[4];
            if (!cMain.isDebug)
            {
                try
                {
                    Com13 = new SerialPort(cMain.mSysSet.ComEmpty, 9600, Parity.None, 8, StopBits.One);
                    if (Com13.IsOpen)
                    {
                        Com13.Close();
                    }
                    Com13.Open();
                }
                catch
                {
                    mError.AddErrData(cMain.mSysSet.ComEmpty + "_ERROR", "打开通讯端口" + (cMain.mSysSet.ComEmpty + "失败"));//,"+cMain.mSysSet.mPLCCOM+"ERROR").Split(',')[cMain.IndexLanguage]);
                }
            }
            mEmpty = new S7Modbus(Com13, 3);
            while (!IsOutSystem)
            {
                if (mEmpty.ReadVB(0, 16, out buff))
                {
                    chouKongError = 0;
                    ChouKong = true;
                    for (int i = 0; i < 4; i++)
                    {
                        emptyValue[i] = (float)Math.Pow(10,
                            ((buff[0 + i * 4] * 0x1000000 + buff[1 + i * 4] * 0x10000 + buff[2 + i * 4] * 0x100 + buff[3 + i * 4]) / 2764.80f
                            - 3.572f) / 1.286);
                    }
                    switch (emptyStart[0])
                    {
                        case "":
                            break;
                        case "192.168.1.161":
                            lockMainEmpty.AllTesting(0, emptyStart[0], emptyValue[0].ToString("F1"));
                            if (emptyValue[0] < cMain.LocalSaveValue.EmptyUp)
                            {
                                lockMainEmpty.AllTestOver("192.168.1.161", emptyValue[0].ToString("F1"));
                            }
                            break;
                        case "192.168.1.162":
                            lockMainEmpty.AllTesting(0, emptyStart[0], emptyValue[1].ToString("F1"));
                            if (emptyValue[1] < cMain.LocalSaveValue.EmptyUp)
                            {
                                lockMainEmpty.AllTestOver("192.168.1.162", emptyValue[1].ToString("F1"));
                            }
                            break;
                    }

                    switch (emptyStart[1])
                    {
                        case "":
                            break;
                        case "192.168.1.163":
                            lockMainEmpty.AllTesting(1, emptyStart[1], emptyValue[2].ToString("F1"));
                            if (emptyValue[2] < cMain.LocalSaveValue.EmptyUp)//最少抽5秒
                            {
                                lockMainEmpty.AllTestOver("192.168.1.163", emptyValue[2].ToString("F1"));
                            }
                            break;
                        case "192.168.1.164":
                            lockMainEmpty.AllTesting(1, emptyStart[1], emptyValue[3].ToString("F1"));
                            if (emptyValue[3] < cMain.LocalSaveValue.EmptyUp)//最少抽5秒
                            {
                                lockMainEmpty.AllTestOver("192.168.1.164", emptyValue[3].ToString("F1"));
                            }
                            break;
                    }

                }
                else
                {
                    chouKongError++;
                    if (chouKongError > 3)
                    {
                        chouKongError = 3;
                        ChouKong = false;
                    }
                }
                Thread.Sleep(500);
            }
        }
        /// <summary>
        /// 对PLC写入M点
        /// </summary>
        /// <param name="tempM">要写的M值</param>
        /// <param name="mPoint">M点的地址</param>
        /// <returns>返回写入是否成功</returns>
        private bool ChangeM(bool tempM, string mPoint)
        {
            short[] sendValue = new short[1];
            if (tempM)
                sendValue[0] = 0x100;
            else
                sendValue[0] = 0;
            string[] buff = mPoint.Split('.');
            int High = Convert.ToInt32(buff[0]) - 1000;
            if (!mPlc.WriteVW(High, sendValue))
            {
                if (!mPlc.WriteVW(High, sendValue))
                {
                    return false;
                }
            }
            return true;
        }
        private bool ChangeD(int tempD, int mPoint)
        {
            //if (!mPlc.FxPlc_WriteD(mPoint, tempD))
            //{
            //    if (!mPlc.FxPlc_WriteD(mPoint, tempD))
            //    {
            //        if (!mPlc.FxPlc_WriteD(mPoint, tempD))
            //        {
            //            return false;
            //        }
            //    }
            //}
            return true;
        }
        private bool frmClose()//系统退出时结束打开端口
        {
            //try
            //{
            IsOutSystem = true;
            if (udpSend.IsOpen)
            {
                udpSend.fUdpClose();
            }
            if (!cMain.isDebug)
            {
                if (Com1.IsOpen)
                {
                    Com1.Close();
                }
                if (Com2.IsOpen)
                {
                    Com2.Close();
                }
            }
            udpSend = null;
            Dispose(true);
            //}
            //catch
            //{
            //    MessageBox.Show("关闭串口或UDP端口失败");
            //    return false;
            //}
            return true;
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (nowStatue.isStarting)
            {
                Handle_Next = true;
            }
            if (UpdataCount == 10)
            {
                updataByNet(sender, e);
            }
        }
        private void btnStart_Click(object sender, EventArgs e)//启动按钮
        {
            Handle_Start = true;
        }

        private void btnStop_Click(object sender, EventArgs e)//停止按钮
        {
            Handle_Stop = true;
        }

        private void MainLoop_Tick(object sender, EventArgs e)
        {
            cMain.SaveDataToSQL.TmpResult = cMain.mTempNetResult;
            //刷新数据
            if (Environment.TickCount - PreFlushDataTime > cMain.FlushDataTime)
            {
                PreFlushDataTime = Environment.TickCount;
                ShowData(false, nowStatue.CurrentId);
            }
            if (nowStatue.isStarting)
            {
                if (cMain.isDanXiang)
                {
                    CheckProtect(dataShow[1], new double[] { dataShow[3] });//检测数据保护(过流,过压等)
                }
                else
                {
                    CheckProtect(dataShow[3] + dataShow[4] + dataShow[5],
                        new double[]{
                            dataShow[30],
                            dataShow[31]
                        });
                }
            }
            FlushData();

            //PLC停止,屏幕手动停止,网络停止,保护停止

            if (!Handle_Start && (Plc_Stop || Handle_Stop || Net_Stop || Pro_Stop)) //Stoping
            {
                Plc_Stop = false;
                Handle_Stop = false;
                Net_Stop = false;
                Pro_Stop = false;
                if (nowStatue.isStarting)
                {
                    toolBtnStop.Enabled = false;
                    toolBtnStart.Enabled = true;
                    Plc_Out_MPoint["总复位"] = true; Temp_Out_MPoint["总复位"] = true;
                    nowStatue.isStarting = false;
                    nowStatue.isStarted = false;
                }
                if (fEmpty != null)
                {
                    fEmpty.Close();
                    fEmpty.Dispose();
                    fEmpty = null;
                }
                if (fAnGui != null)
                {
                    fAnGui.Close();
                    fAnGui.Dispose();
                    fAnGui = null;
                }
            }
            if (nowStatue.isStarting == false && nowStatue.isStoped == false)//Stoped
            {
                StopOut();
                switch (nowStatue.StopId)
                {
                    case cMain.StopValue.ZYSFProtect:
                        mError.AddErrData("PROTECT", "Stop By Auto,正压收氟停机".Split(',')[cMain.IndexLanguage]);// + StopPress[tempBiaoZhunJi].ToString(), false);//压力校准
                        //Plc_In_ZYSF = true; Temp_In_ZYSF = false;
                        break;
                    case cMain.StopValue.LowPressProtect:
                        mError.AddErrData("PROTECT", "Low pressure protection,压力过低保护请检查".Split(',')[cMain.IndexLanguage]);
                        break;
                    case cMain.StopValue.LowCurProtect:
                        mError.AddErrData("PROTECT", "Low pressure protection,电流过低保护请检查".Split(',')[cMain.IndexLanguage]);
                        break;
                    case cMain.StopValue.HighPressProtect:
                        mError.AddErrData("PROTECT", "High pressure protection,压力过高保护请检查".Split(',')[cMain.IndexLanguage]);
                        break;
                    case cMain.StopValue.HighCurProtect:
                        mError.AddErrData("PROTECT", "High current protection,电流过高保护请检查".Split(',')[cMain.IndexLanguage]);
                        break;
                }
                mError.AddErrData("INFO", "Press the [StartTest] button to test,请按[开始]按钮开始检测".Split(',')[cMain.IndexLanguage]);
                nowStatue.StopId = cMain.StopValue.IsOk;
                nowStatue.isStoped = true;//停止完成
                nowStatue.CurrentId = -1;
            }
            if (Plc_Start || Handle_Start || Net_Start)//Starting
            {
                Plc_Start = false;
                Handle_Start = false;
                Net_Start = false;
                if (!nowStatue.isStarting)
                {
                    toolBtnStop.Enabled = true;
                    toolBtnStart.Enabled = false;
                    nowStatue.isStarting = true;//正在检测
                    nowStatue.isStoped = false;//
                }
            }
            if (nowStatue.isStarting == true && nowStatue.isStarted == false)//Started
            {
                DialogResult tmpDialogResult;
                if (!IsInitOver)//启动先初始化参数
                {
                    InitStart();
                    IsInitOver = true;
                }
                if (!chkAnGui.Checked)
                {
                    if (fAnGui != null)
                    {
                        return;
                    }
                    fAnGui = new frmAnGui();
                    fAnGui.Barcode = LblBar.Text;
                    tmpDialogResult = fAnGui.ShowDialog();
                    if (tmpDialogResult != DialogResult.Yes)
                    {
                        Handle_Stop = true;
                        return;
                    }
                    chkAnGui.Checked = true;
                    return;
                }
                else
                {
                    if (fAnGui != null)
                    {
                        fAnGui.Close();
                        fAnGui.Dispose();
                        fAnGui = null;
                    }
                }
                if (!chkEmpty.Checked && (cMain.mModeSet.BiaoZhunJi[0] || cMain.mModeSet.BiaoZhunJi[1] || cMain.mModeSet.BiaoZhunJi[2]))
                {
                    if (fEmpty != null)
                    {
                        return;
                    }
                    fEmpty = new frmEmpty();
                    tmpDialogResult = fEmpty.ShowDialog();
                    if (tmpDialogResult != DialogResult.Yes)
                    {
                        Handle_Stop = true;
                        return;
                    }
                    chkEmpty.Checked = true;
                    return;
                }
                else
                {
                    if (fEmpty != null)
                    {
                        fEmpty.Close();
                        fEmpty.Dispose();
                        fEmpty = null;
                    }
                }
                mError.DelErrData("PROTECT");
                if (!IsStartOut)
                {
                    StartOut();
                    IsStartOut = true;
                }
                if (!StartStep())
                {
                    return;
                }
                nowStatue.isStarted = true;//启动完成
            }
            if (Plc_NG)
            {
                Plc_NG = false;
                nowStatue.StepStartTime = Environment.TickCount / 1000;
                nowStatue.StepCurTime = 0;
            }
            if (nowStatue.isStarting && nowStatue.CurrentId > -1 && nowStatue.CurrentId < 10)
            {
                //下一步
                if ((nowStatue.StepCurTime >= cMain.mModeSet.Step[nowStatue.CurrentId].TestTime) || Handle_Next || Plc_Next || Net_Next)
                {
                    if ((cMain.mModeSet.Step[nowStatue.CurrentId].TestTime > 0) && (!IsEndStepOut))
                    {
                        if (!EndStep() && !Plc_OK )//步骤不合格，须要确认才能继续
                        {
                            return;
                        }
                        Plc_OK = false;
                        cData.SaveJianCeData(cMain.mNetResult);
                        IsEndStepOut = true;
                    }
                    if (!StartStep())
                    {
                        return;
                    }
                    Plc_Next = false;
                    Handle_Next = false;
                    Net_Next = false;
                }
                nowStatue.StepCurTime = Environment.TickCount / 1000 - nowStatue.StepStartTime;

                #region//画曲线
                quXianControl1.AddPoint(chanelCurve[0], Environment.TickCount / 1000 - nowStatue.TestElectStartTime, dataShow[3]);
                quXianControl1.AddPoint(chanelCurve[1], Environment.TickCount / 1000 - nowStatue.TestElectStartTime, dataShow[4]);
                quXianControl1.AddPoint(chanelCurve[2], Environment.TickCount / 1000 - nowStatue.TestElectStartTime, dataShow[5]);

                quXianControl1.AddPoint(chanelCurve[3], Environment.TickCount / 1000 - nowStatue.TestElectStartTime, dataShow[30]);
                quXianControl1.AddPoint(chanelCurve[4], Environment.TickCount / 1000 - nowStatue.TestElectStartTime, dataShow[31]);
                #endregion
                LblCurTime.Text = nowStatue.StepCurTime.ToString();
            }
        }
        private void FlushData()
        {
            int showIndex = 0, tempIndex = 0;
            double CurA = 0, CurB = 0, CurC = 0;

            tempIndex = Num.IndexMax(dataRead[3] * cMain.mKBValue.valueK[3] + cMain.mKBValue.valueB[3],
                                          dataRead[4] * cMain.mKBValue.valueK[4] + cMain.mKBValue.valueB[4],
                                          dataRead[5] * cMain.mKBValue.valueK[5] + cMain.mKBValue.valueB[5]);

            //电压
            dataShow[showIndex++] = dataRead[0] * cMain.mKBValue.valueK[0] + cMain.mKBValue.valueB[0];
            dataShow[showIndex++] = dataRead[1] * cMain.mKBValue.valueK[1] + cMain.mKBValue.valueB[1];
            dataShow[showIndex++] = dataRead[2] * cMain.mKBValue.valueK[2] + cMain.mKBValue.valueB[2];
            //电流
            CurA = dataRead[3] * cMain.mKBValue.valueK[3] + cMain.mKBValue.valueB[3];
            CurB = dataRead[4] * cMain.mKBValue.valueK[4] + cMain.mKBValue.valueB[5];
            CurC = dataRead[5] * cMain.mKBValue.valueK[4] + cMain.mKBValue.valueB[5];
            if (CurA > 0.5 && CurB > 0.5 && CurC > 0.5)
            {
                dataShow[showIndex++] = CurA;
                dataShow[showIndex++] = CurB;
                dataShow[showIndex++] = CurC;
            }
            else
            {
                switch (tempIndex)//选最大电流放到A相
                {
                    default:
                        dataShow[showIndex++] = CurA;
                        dataShow[showIndex++] = CurB;
                        dataShow[showIndex++] = CurC;
                        break;
                    case 1:
                        dataShow[showIndex++] = CurB;
                        dataShow[showIndex++] = CurA;
                        dataShow[showIndex++] = CurC;
                        break;
                    case 2:
                        dataShow[showIndex++] = CurC;
                        dataShow[showIndex++] = CurB;
                        dataShow[showIndex++] = CurA;
                        break;
                }
            }
            //功率
            dataShow[showIndex++] = dataRead[6] * cMain.mKBValue.valueK[6] + cMain.mKBValue.valueB[6] +
                            dataRead[7] * cMain.mKBValue.valueK[7] + cMain.mKBValue.valueB[7] +
                            dataRead[8] * cMain.mKBValue.valueK[8] + cMain.mKBValue.valueB[8];

            dataShow[showIndex++] = dataRead[25] * cMain.mKBValue.valueK[25] + cMain.mKBValue.valueB[25];//1#进风
            dataShow[showIndex++] = dataRead[26] * cMain.mKBValue.valueK[26] + cMain.mKBValue.valueB[26];//1#出风
            dataShow[showIndex++] = Math.Abs(dataShow[showIndex - 3] - dataShow[showIndex - 2]);
            dataShow[showIndex++] = dataRead[9] * cMain.mKBValue.valueK[9] + cMain.mKBValue.valueB[9];//1#进管
            dataShow[showIndex++] = dataRead[10] * cMain.mKBValue.valueK[10] + cMain.mKBValue.valueB[10];//1#出管
            dataShow[showIndex++] = dataRead[11] * cMain.mKBValue.valueK[11] + cMain.mKBValue.valueB[11];//1#进管2
            dataShow[showIndex++] = dataRead[12] * cMain.mKBValue.valueK[12] + cMain.mKBValue.valueB[12];//1#出管2

            dataShow[showIndex++] = dataRead[27] * cMain.mKBValue.valueK[27] + cMain.mKBValue.valueB[27];//2#进风
            dataShow[showIndex++] = dataRead[28] * cMain.mKBValue.valueK[28] + cMain.mKBValue.valueB[28];//2#出风
            dataShow[showIndex++] = Math.Abs(dataShow[showIndex - 3] - dataShow[showIndex - 2]);
            dataShow[showIndex++] = dataRead[13] * cMain.mKBValue.valueK[13] + cMain.mKBValue.valueB[13];//2#进管
            dataShow[showIndex++] = dataRead[14] * cMain.mKBValue.valueK[14] + cMain.mKBValue.valueB[14];//2#出管
            dataShow[showIndex++] = dataRead[15] * cMain.mKBValue.valueK[15] + cMain.mKBValue.valueB[15];//2#进管2
            dataShow[showIndex++] = dataRead[16] * cMain.mKBValue.valueK[16] + cMain.mKBValue.valueB[16];//2#出管2

            dataShow[showIndex++] = dataRead[29] * cMain.mKBValue.valueK[29] + cMain.mKBValue.valueB[29];//3#进风
            dataShow[showIndex++] = dataRead[30] * cMain.mKBValue.valueK[30] + cMain.mKBValue.valueB[30];//3#出风
            dataShow[showIndex++] = Math.Abs(dataShow[showIndex - 3] - dataShow[showIndex - 2]);
            dataShow[showIndex++] = dataRead[17] * cMain.mKBValue.valueK[17] + cMain.mKBValue.valueB[17];//3#进管
            dataShow[showIndex++] = dataRead[18] * cMain.mKBValue.valueK[18] + cMain.mKBValue.valueB[18];//3#出管
            dataShow[showIndex++] = dataRead[19] * cMain.mKBValue.valueK[19] + cMain.mKBValue.valueB[19];//3#进管2
            dataShow[showIndex++] = dataRead[20] * cMain.mKBValue.valueK[20] + cMain.mKBValue.valueB[20];//3#出管2

            dataShow[showIndex++] = dataRead[23] * cMain.mKBValue.valueK[23] + cMain.mKBValue.valueB[23];//风
            dataShow[showIndex++] = dataRead[24] * cMain.mKBValue.valueK[24] + cMain.mKBValue.valueB[24];//速

            dataShow[showIndex++] = dataRead[31] * cMain.mKBValue.valueK[31] + cMain.mKBValue.valueB[31];//进管
            dataShow[showIndex++] = dataRead[32] * cMain.mKBValue.valueK[32] + cMain.mKBValue.valueB[32];//出管
            dataShow[showIndex++] = dataRead[33] * cMain.mKBValue.valueK[33] + cMain.mKBValue.valueB[33];//吸气
            dataShow[showIndex++] = dataRead[34] * cMain.mKBValue.valueK[34] + cMain.mKBValue.valueB[34];//排气

            dataShow[showIndex++] = dataRead[21] * cMain.mKBValue.valueK[21] + cMain.mKBValue.valueB[21];//吸气温度
            dataShow[showIndex++] = dataRead[22] * cMain.mKBValue.valueK[22] + cMain.mKBValue.valueB[22];//排气温度

            dataShow[showIndex++] = dataRead[35] * cMain.mKBValue.valueK[35] + cMain.mKBValue.valueB[35];//外机压力

            if (K1K2 != null)
            {
                dataShow[showIndex++] = K1K2.TempHuangJin;
                dataShow[showIndex++] = K1K2.TempLengNing[0];
                dataShow[showIndex++] = K1K2.TempLengNing[1];
                dataShow[showIndex++] = K1K2.TempPaiQi[0];
                dataShow[showIndex++] = K1K2.TempPaiQi[1];
                dataShow[showIndex++] = K1K2.TempPaiQi[2];
                dataShow[showIndex++] = K1K2.CurA[0];
                dataShow[showIndex++] = K1K2.CurA[1];
                dataShow[showIndex++] = K1K2.CurA[2];
                dataShow[showIndex++] = K1K2.PinLv;
                dataShow[showIndex++] = K1K2.DianZiPengZhangFa[0];
                dataShow[showIndex++] = K1K2.DianZiPengZhangFa[1];
                dataShow[showIndex++] = K1K2.OADianLiang;

            }
            else
            {
                for (int i = showIndex; i < dataShow.Length; i++)
                {
                    dataShow[showIndex++] = 0;
                }
            }


        }
        /// <summary>
        /// 检测数据是否在保护范围内
        /// </summary>
        private void CheckProtect(double cur, double[] press)
        {
            //过压保护
            for (int i = 0; i < press.Length; i++)
            {
                if (cMain.mModeSet.Protect[1] < press[i])
                {
                    if (HighPresErr == 0)
                    {
                        HighPresErr = Environment.TickCount;
                    }
                    else
                    {
                        if (Environment.TickCount - HighPresErr > 5000)
                        {
                            Pro_Stop = true;
                            nowStatue.StopId = cMain.StopValue.HighPressProtect;
                        }
                    }
                }
                else
                {
                    HighPresErr = 0;
                }
            }
            //过流保护
            if ((cMain.mModeSet.Protect[0] < cur) && (cur > 0))
            {
                if (HighElectErr == 0)
                {
                    HighElectErr = Environment.TickCount;
                }
                else
                {
                    if (Environment.TickCount - HighElectErr > 5000)
                    {
                        Pro_Stop = true;
                        nowStatue.StopId = cMain.StopValue.HighCurProtect;
                    }
                }
            }
            else
            {
                HighElectErr = 0;
            }
        }
        /// <summary>
        /// 步骤结束输出
        /// </summary>
        private bool EndStep()
        {
            int i;
            bool result = true;
            bool isStepPass = true;//当前步骤是否合格
            if (cMain.mModeSet.Step[nowStatue.CurrentId].TestTime < 1)
            {
                for (i = 0; i < cMain.DataShow; i++)
                {
                    cMain.mNetResult.StepResult.mData[i] = 0;
                    cMain.mNetResult.StepResult.mIsDataPass[i] = 1;

                    cMain.mTestResult.StepResult[nowStatue.CurrentId].mData[i] = 0;
                    cMain.mTestResult.StepResult[nowStatue.CurrentId].mIsDataPass[i] = 1;

                }
                cMain.mNetResult.StepResult.mIsStepPass = 1;

                cMain.mTestResult.StepResult[nowStatue.CurrentId].mIsStepPass = 1;
            }
            else
            {
                for (i = 0; i < cMain.DataShow; i++)
                {
                    cMain.mNetResult.StepResult.mData[i] = dataShow[i];
                    cMain.mTestResult.StepResult[nowStatue.CurrentId].mData[i] = dataShow[i];

                    if ((cMain.mModeSet.Step[nowStatue.CurrentId].HighData[i] == 0 && cMain.mModeSet.Step[nowStatue.CurrentId].LowData[i] == 0) ||
                        ((dataShow[i] <= cMain.mModeSet.Step[nowStatue.CurrentId].HighData[i]) &&
                        (dataShow[i] >= cMain.mModeSet.Step[nowStatue.CurrentId].LowData[i]))||
                        (!cMain.mModeSet.DataShow[i]))
                    {
                        cMain.mNetResult.StepResult.mIsDataPass[i] = 1;
                        cMain.mTestResult.StepResult[nowStatue.CurrentId].mIsDataPass[i] = 1;
                    }
                    else
                    {
                        cMain.mNetResult.StepResult.mIsDataPass[i] = 0;
                        cMain.mTestResult.StepResult[nowStatue.CurrentId].mIsDataPass[i] = 0;
                        isStepPass = false;
                    }
                }
                if (isStepPass)
                {
                    cMain.mNetResult.StepResult.mIsStepPass = 1;
                    cMain.mTestResult.StepResult[nowStatue.CurrentId].mIsStepPass = 1;
                }
                else
                {
                    cMain.mNetResult.StepResult.mIsStepPass = 0;
                    cMain.mTestResult.StepResult[nowStatue.CurrentId].mIsStepPass = 0;
                }
            }

            cMain.mNetResult.RunResult.mStep = cMain.mModeSet.Step[nowStatue.CurrentId].Text;
            cMain.mNetResult.RunResult.mStepId = nowStatue.CurrentId;
            cMain.mNetResult.RunResult.mIsPass = isStepPass;
            cMain.mTempNetResult.RunResult.mIsPass = isStepPass;
            if (isStepPass)
            {
                for (i = 0; i < nowStatue.CurrentId; i++)//为了方便步骤转换,所以每一次的总合格都是从前面的检测中计算,而不用保存值
                {
                    if (cMain.mTestResult.StepResult[i].mIsStepPass == 0)
                    {
                        cMain.mNetResult.RunResult.mIsPass = false;
                        cMain.mTempNetResult.RunResult.mIsPass = false;
                        break;
                    }
                }
            }

            result = isStepPass;
            cMain.mAllResult.SetStepResult(nowStatue.CurrentId, cMain.mNetResult.StepResult);
            cMain.mAllResult.RunResult = cMain.mNetResult.RunResult;

            nowStatue.isPass = cMain.mNetResult.RunResult.mIsPass;

            return result;
            //cTestData tempTestData = new cTestData("J~OK~" + DataClassToStr(cMain.mNetResult));
            //tempTestData.sendTestDataHandle = new SendTestDataHandle(SendDataToComputer);
            //SendTestData = new Thread(new ThreadStart(tempTestData.sendData));
            //SendTestData.IsBackground = true;
            //SendTestData.Priority = ThreadPriority.BelowNormal;
            //SendTestData.Start();
            //udpSend.fUdpSend("J~OK~" + DataClassToStr(cMain.mNetResult));
            //if (isStepPass)
            //{
            //    StepLabel[nowStatue.CurrentId].BackColor = Color.Green;
            //}
            //else
            //{
            //    StepLabel[nowStatue.CurrentId].BackColor = Color.Red;
            //}
        }
        /// <summary>
        /// 启动输出
        /// </summary>
        private void InitStart()//启动输出
        {
            int KaiGuanCount = 0;
            mError.AddErrData("INFO", "Test Now,正在检测".Split(',')[cMain.IndexLanguage]);
            HighElectErr = 0;
            HighPresErr = 0;
            nowStatue.TestTime = DateTime.Now;
            nowStatue.TestStartTime = Environment.TickCount / 1000;

            initTestFrm(-1);
            initTestData();

            if (cMain.mModeSet.KaiGuan[0])
            {
                Plc_Out_MPoint["1#阀门"] = true; Temp_Out_MPoint["1#阀门"] = true;
                KaiGuanCount++;
            }
            if (cMain.mModeSet.KaiGuan[1])
            {
                Plc_Out_MPoint["2#阀门"] = true; Temp_Out_MPoint["2#阀门"] = true;
                KaiGuanCount++;
            }
            if (cMain.mModeSet.KaiGuan[2])
            {
                Plc_Out_MPoint["3#阀门"] = true; Temp_Out_MPoint["3#阀门"] = true;
                KaiGuanCount++;
            }
            if (cMain.mModeSet.KaiGuan[3])
            {
                Plc_Out_MPoint["4#阀门"] = true; Temp_Out_MPoint["4#阀门"] = true;
                KaiGuanCount++;
            }
            if (cMain.mModeSet.KaiGuan[4])
            {
                Plc_Out_MPoint["5#阀门"] = true; Temp_Out_MPoint["5#阀门"] = true;
                KaiGuanCount++;
            }
            if (cMain.mModeSet.KaiGuan[5])
            {
                Plc_Out_MPoint["6#阀门"] = true; Temp_Out_MPoint["6#阀门"] = true;
                KaiGuanCount++;
            }

            if (cMain.mModeSet.XinHao[0])
            {
                PQ1 = new All.Machine.Media.PQ(cMain.mSysSet.ComPQ1, All.Machine.Media.AllMachine.Machines.商用V4);
                PQ1.Open();
                PQ1.DoorCount = KaiGuanCount;
            }
            if (cMain.mModeSet.XinHao[1])
            {
                switch(cMain.JiQiStr[cMain.IndexLanguage].Split(',')[cMain.mModeSet.JiQi])
                {
                    case "V4+":
                        PQ2 = new All.Machine.Media.PQ(cMain.mSysSet.ComPQ2, All.Machine.Media.AllMachine.Machines.商用V4);
                        PQ2.Open();
                        PQ2.DoorCount = KaiGuanCount;
                        break;
                    case "V6":
                        PQ2 = new All.Machine.Media.PQ(cMain.mSysSet.ComPQ2, All.Machine.Media.AllMachine.Machines.商用V6);
                        PQ2.Open();
                        break;
                    case "两管制":
                        PQ2 = new All.Machine.Media.PQ(cMain.mSysSet.ComPQ2, All.Machine.Media.AllMachine.Machines.商用两管制);
                        PQ2.Open();
                        break;
                    case "三管制":
                        PQ2 = new All.Machine.Media.PQ(cMain.mSysSet.ComPQ2, All.Machine.Media.AllMachine.Machines.商用三管制);
                        PQ2.Open();
                        break;
                }
            }
            if (cMain.mModeSet.XinHao[2])
            {
                switch (cMain.JiQiStr[cMain.IndexLanguage].Split(',')[cMain.mModeSet.JiQi])
                {
                    case "V4+":
                        XY = new All.Machine.Media.XY(cMain.mSysSet.ComXY, All.Machine.Media.AllMachine.Machines.商用V4);
                        XY.Open();
                        break;
                    case "V6":
                        XY = new All.Machine.Media.XY(cMain.mSysSet.ComXY, All.Machine.Media.AllMachine.Machines.商用V6);
                        XY.Open();
                        break;
                    case "两管制":
                        XY = new All.Machine.Media.XY(cMain.mSysSet.ComXY, All.Machine.Media.AllMachine.Machines.商用两管制);
                        XY.Open();
                        break;
                    case "三管制":
                        XY = new All.Machine.Media.XY(cMain.mSysSet.ComXY, All.Machine.Media.AllMachine.Machines.商用三管制);
                        XY.Open();
                        break;
                }
            }
            if (cMain.mModeSet.XinHao[3] || cMain.mModeSet.XinHao[5])
            {
                switch (cMain.JiQiStr[cMain.IndexLanguage].Split(',')[cMain.mModeSet.JiQi])
                {
                    case "V4+":
                        K1K2 = new All.Machine.Media.K1K2(cMain.mSysSet.ComK1K2, All.Machine.Media.AllMachine.Machines.商用V4);
                        K1K2.Open();
                        break;
                    case "V6":
                        K1K2 = new All.Machine.Media.K1K2(cMain.mSysSet.ComK1K2, All.Machine.Media.AllMachine.Machines.商用V6);
                        K1K2.Open();
                        break;
                    case "两管制":
                        K1K2 = new All.Machine.Media.K1K2(cMain.mSysSet.ComK1K2, All.Machine.Media.AllMachine.Machines.商用两管制);
                        K1K2.Open();
                        break;
                    case "三管制":
                        K1K2 = new All.Machine.Media.K1K2(cMain.mSysSet.ComK1K2, All.Machine.Media.AllMachine.Machines.商用三管制);
                        K1K2.Open();
                        break;
                }
            }
            if (cMain.mModeSet.XinHao[4])
            {
                switch (cMain.JiQiStr[cMain.IndexLanguage].Split(',')[cMain.mModeSet.JiQi])
                {
                    case "V4+":
                        H1H2 = new All.Machine.Media.H1H2(cMain.mSysSet.ComH1H2, All.Machine.Media.AllMachine.Machines.商用V4);
                        H1H2.Open();
                        break;
                    case "V6":
                        H1H2 = new All.Machine.Media.H1H2(cMain.mSysSet.ComH1H2, All.Machine.Media.AllMachine.Machines.商用V6);
                        H1H2.Open();
                        break;
                    case "两管制":
                        H1H2 = new All.Machine.Media.H1H2(cMain.mSysSet.ComH1H2, All.Machine.Media.AllMachine.Machines.商用两管制);
                        H1H2.Open();
                        break;
                    case "三管制":
                        H1H2 = new All.Machine.Media.H1H2(cMain.mSysSet.ComH1H2, All.Machine.Media.AllMachine.Machines.商用三管制);
                        H1H2.Open();
                        break;
                }
            }

        }
        private void StartOut()
        {
            NowStatue = 1;
            Plc_Out_MPoint["黄灯"] = true; Temp_Out_MPoint["黄灯"] = true;
            if (cMain.mModeSet.LowSpeed)
            {
                Plc_Out_MPoint["低风"] = true; Temp_Out_MPoint["低风"] = true;
            }
            if (cMain.mModeSet.Vol110V)
            {
                Plc_Out_MPoint["110V"] = true; Temp_Out_MPoint["110V"] = true;
            }
            if (cMain.mModeSet.BiaoZhunJi[0])
            {
                Plc_Out_MPoint["1#内机"] = true; Temp_Out_MPoint["1#内机"] = true;
            }
            if (cMain.mModeSet.BiaoZhunJi[1])
            {
                Plc_Out_MPoint["2#内机"] = true; Temp_Out_MPoint["2#内机"] = true;
            }
            if (cMain.mModeSet.BiaoZhunJi[2])
            {
                Plc_Out_MPoint["3#内机"] = true; Temp_Out_MPoint["3#内机"] = true;
            }
            nowStatue.TestElectStartTime = Environment.TickCount/1000;
        }
        private bool StartStep()//中间输出
        {
            nowStatue.CurrentId++;

            nowStatue.StepStartTime = Environment.TickCount / 1000;
            nowStatue.StepCurTime = 0;
            if (nowStatue.CurrentId >= 10)
            {
                EndOut();
                mError.AddErrData("INFO", "Test is Over,检测已完成,请收氟".Split(',')[cMain.IndexLanguage]);
                return true;
            }
            if (cMain.mModeSet.Step[nowStatue.CurrentId].TestTime <= 0)
            {
                return true;
            }
            LblCurTime.Text = nowStatue.StepCurTime.ToString();
            cMain.mTempNetResult.RunResult.mStep = cMain.mModeSet.Step[nowStatue.CurrentId].Text;
            cMain.mTempNetResult.RunResult.mStepId = nowStatue.CurrentId;
            mError.AddErrData("INFO", new string[] { "Now Test ", "当前检测步骤" }[cMain.IndexLanguage] + cMain.mModeSet.Step[nowStatue.CurrentId].Text);
            LblStep.Text = cMain.mModeSet.Step[nowStatue.CurrentId].Text;
            lblStepId.Text = string.Format("一,二,三,四,五,六,七,八,九,十").Split(',')[nowStatue.CurrentId];
            if (cMain.mModeSet.Step[nowStatue.CurrentId].TestTime > 0)
            {
                #region//发送当前指令
                if (cMain.mModeSet.JiQi == 1)
                {
                    #region//判断指令是否为空，如果为空，则写入默认SN指令
                    switch (cMain.mModeSet.Step[nowStatue.CurrentId].Text)
                    {
                        case "制热":
                            if (Temp_Step_SnCode == "")
                            {
                                Temp_Step_SnCode = All.Machine.Media.SN600.GetSnCode(All.Machine.Media.AllMachine.Machines.家用旧变频, All.Machine.Media.AllMachine.Modes.制热);// cMideaSnCode.GetSn600(cMideaSnCode.CrcList.OldSn600, cMideaSnCode.StepName.Hot, cMideaSnCode.IndoorList.One);
                            }
                            break;
                        case "制冷":
                            if (Temp_Step_SnCode == "")
                            {
                                Temp_Step_SnCode= All.Machine.Media.SN600.GetSnCode(All.Machine.Media.AllMachine.Machines.家用旧变频, All.Machine.Media.AllMachine.Modes.制冷);// cMideaSnCode.GetSn600(cMideaSnCode.CrcList.OldSn600, cMideaSnCode.StepName.Cold, cMideaSnCode.IndoorList.One);
                            }
                            break;
                        case "待机":
                        case "停机":
                            if (Temp_Step_SnCode == "")
                            {
                                Temp_Step_SnCode = All.Machine.Media.SN600.GetSnCode(All.Machine.Media.AllMachine.Machines.家用旧变频, All.Machine.Media.AllMachine.Modes.待机);// cMideaSnCode.GetSn600(cMideaSnCode.CrcList.OldSn600, cMideaSnCode.StepName.Stop, cMideaSnCode.IndoorList.One);
                            }
                            break;
                    }
                    #endregion
                }
                if (cMain.mModeSet.JiQi == 2)
                {
                    #region//判断指令是否为空，如果为空，则写入默认SN指令
                    switch (cMain.mModeSet.Step[nowStatue.CurrentId].Text)
                    {
                        case "制热":
                            if (Temp_Step_SnCode == "")
                            {
                                Temp_Step_SnCode = All.Machine.Media.SN1200.GetSnCode(All.Machine.Media.AllMachine.Machines.合用定频1200, All.Machine.Media.AllMachine.Modes.制热);// cMideaSnCode.GetSn600(cMideaSnCode.CrcList.OldSn600, cMideaSnCode.StepName.Hot, cMideaSnCode.IndoorList.One);
                            }
                            break;
                        case "制冷":
                            if (Temp_Step_SnCode == "")
                            {
                                Temp_Step_SnCode = All.Machine.Media.SN1200.GetSnCode(All.Machine.Media.AllMachine.Machines.合用定频1200, All.Machine.Media.AllMachine.Modes.制冷);// cMideaSnCode.GetSn600(cMideaSnCode.CrcList.OldSn600, cMideaSnCode.StepName.Cold, cMideaSnCode.IndoorList.One);
                            }
                            break;
                        case "待机":
                        case "停机":
                            if (Temp_Step_SnCode == "")
                            {
                                Temp_Step_SnCode = All.Machine.Media.SN1200.GetSnCode(All.Machine.Media.AllMachine.Machines.合用定频1200, All.Machine.Media.AllMachine.Modes.待机);// cMideaSnCode.GetSn600(cMideaSnCode.CrcList.OldSn600, cMideaSnCode.StepName.Stop, cMideaSnCode.IndoorList.One);
                            }
                            break;
                    }
                    #endregion
                }
                if (cMain.mModeSet.XinHao[0])//PQ机
                {
                    switch (cMain.mModeSet.Step[nowStatue.CurrentId].Text)
                    {
                        case "制热":
                            PQ1.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用V4, All.Machine.Media.AllMachine.Modes.制热)
                                ,(byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                            break;
                        case "制冷":
                            PQ1.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用V4, All.Machine.Media.AllMachine.Modes.制冷)
                                , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                break;
                        default:
                            PQ1.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用V4, All.Machine.Media.AllMachine.Modes.待机)
                                , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi); 
                                break;
                    }
                }
                if (cMain.mModeSet.XinHao[1])
                {
                    switch (cMain.JiQiStr[cMain.IndexLanguage].Split(',')[cMain.mModeSet.JiQi])
                    {
                        case "V4+":
                            switch (cMain.mModeSet.Step[nowStatue.CurrentId].Text)
                            {
                                case "制热":
                                    PQ2.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用V4, All.Machine.Media.AllMachine.Modes.制热)
                                        , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                    break;
                                case "制冷":
                                    PQ2.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用V4, All.Machine.Media.AllMachine.Modes.制冷)
                                        , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                    break;
                                default:
                                    PQ2.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用V4, All.Machine.Media.AllMachine.Modes.待机)
                                        , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                    break;
                            }
                            break;
                        case "V6":
                            switch (cMain.mModeSet.Step[nowStatue.CurrentId].Text)
                            {
                                case "制热":
                                    PQ2.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用V6, All.Machine.Media.AllMachine.Modes.制热)
                                        , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                    break;
                                case "制冷":
                                    PQ2.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用V6, All.Machine.Media.AllMachine.Modes.制冷)
                                        , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                    break;
                                default:
                                    PQ2.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用V6, All.Machine.Media.AllMachine.Modes.待机)
                                        , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                    break;
                            }
                            break;
                        case "两管制":
                            switch (cMain.mModeSet.Step[nowStatue.CurrentId].Text)
                            {
                                case "制热":
                                    PQ2.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用两管制, All.Machine.Media.AllMachine.Modes.制热)
                                        , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                    break;
                                case "制冷":
                                    PQ2.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用两管制, All.Machine.Media.AllMachine.Modes.制冷)
                                        , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                    break;
                                default:
                                    PQ2.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用两管制, All.Machine.Media.AllMachine.Modes.待机)
                                        , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                    break;
                            }
                            break;
                        case "三管制":
                            switch (cMain.mModeSet.Step[nowStatue.CurrentId].Text)
                            {
                                case "制热":
                                    PQ2.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用三管制, All.Machine.Media.AllMachine.Modes.制热)
                                        , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                    break;
                                case "制冷":
                                    PQ2.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用三管制, All.Machine.Media.AllMachine.Modes.制冷)
                                        , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                    break;
                                default:
                                    PQ2.Send(All.Machine.Media.PQ.GetSnCode(All.Machine.Media.AllMachine.Machines.商用三管制, All.Machine.Media.AllMachine.Modes.待机)
                                        , (byte)cMain.mModeSet.Step[nowStatue.CurrentId].NengJi);
                                    break;
                            }
                            break;
                    }
                }
                #endregion
                switch (cMain.mModeSet.Step[nowStatue.CurrentId].Text)
                {
                    case "低启":
                    case "制热":
                    case "制冷":
                    case "待机":
                        Plc_Out_MPoint["上电"] = true; Temp_Out_MPoint["上电"] = true;
                        break;
                    case "停机":
                        Plc_Out_MPoint["上电"] = false; Temp_Out_MPoint["上电"] = true;
                        break;
                    default:
                        Plc_Out_MPoint["上电"] = false; Temp_Out_MPoint["上电"] = true;
                        break;
                }
            }
            LblSetTime.Text = cMain.mModeSet.Step[nowStatue.CurrentId].TestTime.ToString();
            IsInitOver = false;
            IsEndStepOut = false;
            //isAccessLed = true;
            return true;
        }
        private void EndOut()//收氟输出
        {
            cMain.mTempNetResult.RunResult.mStepId = 10;
            cMain.mAllResult.Save();
            if (!nowStatue.isPass)
            {
                initTestFrm(0);
                Plc_Out_MPoint["红灯"] = true; Temp_Out_MPoint["红灯"] = true;
                NowStatue = 2;
            }
            else
            {
                initTestFrm(1);
                Plc_Out_MPoint["绿灯"] = true; Temp_Out_MPoint["绿灯"] = true;
                NowStatue = 3;
            }
        }
        /// <summary>
        /// 停机输出
        /// </summary>
        private void StopOut()
        {
            NowStatue = 0;
            if (cMain.mModeSet.XinHao[0])
            {
                PQ1.Close();
            }
            if (cMain.mModeSet.XinHao[1])
            {
                PQ2.Close();
            }
            if (cMain.mModeSet.XinHao[2])
            {
                XY.Close();
            }
            if (cMain.mModeSet.XinHao[4])
            {
                H1H2.Close();
            }
            if (cMain.mModeSet.XinHao[5])
            {
                K1K2.Close();
            }
            Plc_Out_MPoint["总复位"] = true; Temp_Out_MPoint["总复位"] = true;
            Temp_Step_SnCode = "";
            Net_Next = false;
            nowStatue.PrevStep = "";

            IsStartOut = false;
            IsInitOver = false;//是否完成启动输出,在等待低启时,以免重复输出 写PLC数据
            cMain.mTempNetResult.RunResult.mStepId = -1;
        }
        /// <summary>
        /// 将数据显示出来
        /// </summary>
        /// <param name="ShowLabel">label数组,要显示刷新实时数据,还是步骤数据</param>
        /// <param name="TestStepNo">要刷新的步骤号</param>
        private void ShowData(bool isStepData, int TestStepNo)
        {
            if (!isStepData)
            {
                if (nowStatue.isStarting)
                {
                    for (int i = 0; i < KaiGuan.Length; i++)
                    {
                        if (K1K2 != null && cMain.mModeSet.XinHao[5])
                        {
                            if (i < K1K2.YaJiStatue.Length)
                            {
                                KaiGuan[i].Color = K1K2.YaJiStatue[i] ? Color.Green : Color.Black;
                            }
                            else
                            {
                                KaiGuan[i].Color = K1K2.SwitchStatue[i - K1K2.YaJiStatue.Length] ? Color.Green : Color.Black;
                            }
                        }
                    }
                    for (int i = 0; i < XinHao.Length; i++)
                    {
                        if (cMain.mModeSet.XinHao[i])
                        {
                            switch (i)
                            {
                                case 0:
                                    if (PQ1 != null)
                                    {
                                        XinHao[i].Color = PQ1.Conn ? Color.Green : Color.Red;
                                    }
                                    break;
                                case 1:
                                    if (PQ2 != null)
                                    {
                                        XinHao[i].Color = PQ2.Conn ? Color.Green : Color.Red;
                                    }
                                    break;
                                case 2:
                                    if (XY != null)
                                    {
                                        XinHao[i].Color = XY.Conn ? Color.Green : Color.Red;
                                    }
                                    break;
                                case 3:
                                    if (K1K2 != null)
                                    {
                                        XinHao[i].Color = K1K2.OAConn ? Color.Green : Color.Red;
                                    }
                                    break;
                                case 4:
                                    if (H1H2 != null)
                                    {
                                        XinHao[i].Color = H1H2.Conn ? Color.Green : Color.Red;
                                    }
                                    break;
                                case 5:
                                    if (K1K2 != null)
                                    {
                                        XinHao[i].Color = K1K2.Conn ? Color.Green : Color.Red;
                                        for (int j = 0; j < BaoHu.Length; j++)
                                        {
                                            BaoHu[i].Color = K1K2.Protect[lblProtect[j].Text] ? Color.Red : Color.Black;
                                        }
                                        lblSpeed.Text = K1K2.Speed;
                                        lblModeStep.Text = K1K2.Step;
                                        lblSanGuanZhiError.Text = K1K2.ReadError;
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            XinHao[i].Color = Color.Black;
                        }
                    }
                }
                else
                {
                    lblSpeed.Text = "";
                    lblModeStep.Text = "";
                    for (int i = 0; i < XinHao.Length; i++)
                    {
                        XinHao[i].Color = Color.Black;
                    }
                    for (int i = 0; i < KaiGuan.Length; i++)
                    {
                        KaiGuan[i].Color = Color.Black;
                    }
                    for (int i = 0; i < BaoHu.Length; i++)
                    {
                        BaoHu[i].Color = Color.Black;
                    }
                }
                DataGridViewRow dr = dataGridNow.Rows[0];
                DataGridViewCell cell;
                for (int i = 0; i < cMain.DataShow; i++)//刷新实时数据框
                {
                    cell = dr.Cells[i];
                    if (cMain.DataShowTitle[i].IndexOf("(A)") >= 0)
                    {
                        cell.Value = string.Format("{0:F2}", dataShow[i]);
                        nowLabel[i].Text = string.Format("{0:F2}", dataShow[i]);
                    }
                    else
                    {
                        if (cMain.DataShowTitle[i].IndexOf("(Mpa)") >= 0)
                        {
                            cell.Value = string.Format("{0:F3}", dataShow[i]);
                            nowLabel[i].Text = string.Format("{0:F3}", dataShow[i]);
                        }
                        else
                        {
                            cell.Value = string.Format("{0:F1}", dataShow[i]);
                            nowLabel[i].Text = string.Format("{0:F1}", dataShow[i]);
                        }
                    }
                    cMain.mTempNetResult.StepResult.mData[i] = dataShow[i];
                    if ((TestStepNo >= 0 && TestStepNo < 10) && cMain.mModeSet.DataShow[i]
                        && (cMain.mModeSet.Step[TestStepNo].HighData[i] != 0 || cMain.mModeSet.Step[TestStepNo].LowData[i] != 0)
                        && (cMain.mModeSet.Step[TestStepNo].HighData[i] < dataShow[i] || cMain.mModeSet.Step[TestStepNo].LowData[i] > dataShow[i])
                        )
                    {
                        cMain.mTempNetResult.StepResult.mIsDataPass[i] = 0;
                        cell.Style.BackColor = Color.Red;
                        nowLabel[i].BackColor = Color.Red;
                    }
                    else
                    {
                        cMain.mTempNetResult.StepResult.mIsDataPass[i] = 1;
                        cell.Style.BackColor = Color.White;
                        nowLabel[i].BackColor = Color.White;
                    }
                }
                    for (int i = 0; i < dataGridStep.Columns.Count; i++)
                    {
                        if (i < nowStatue.CurrentId)
                        {
                            for (int j = 0; j < cMain.DataShow; j++)
                            {
                                dr = dataGridStep.Rows[j];
                                cell = dr.Cells[i];
                                cell.Value = string.Format("{0:F3}", cMain.mTestResult.StepResult[i].mData[j]);
                                if (cMain.mTestResult.StepResult[i].mIsDataPass[j] == 0)
                                {
                                    cell.Style.BackColor = Color.Red;
                                }
                                else
                                {
                                    cell.Style.BackColor = Color.White;
                                }
                            }
                            dr = dataGridStep.Rows[cMain.DataShow];
                            cell = dr.Cells[i];
                            if (cMain.mTestResult.StepResult[i].mIsStepPass == 0)
                            {
                                if (cMain.IndexLanguage == 1)
                                {
                                    cell.Value = "不合格";
                                }
                                else
                                {
                                    cell.Value = "NG";
                                }
                                cell.Style.BackColor = Color.Red;
                            }
                            else
                            {
                                if (cMain.IndexLanguage == 1)
                                {
                                    cell.Value = "合格";
                                }
                                else 
                                {
                                    cell.Value = "OK";
                                }
                                cell.Style.BackColor = Color.Green;
                            }
                        }
                    }
            }
            dataGridStep.CurrentCell = null;//将自动选择的单元去掉,被选择时,会有个自动样式,阻碍绘画背景红色
            //int i;
            //int checkTime = 0;//用于决定开始实时判断的时间

            //if (TestStepNo >= 0 || (TestStepNo < 10))
            //{
            //    double[] tempValue = new double[cMain.DataShow];
            //    if (isStepData)
            //    {
            //        if (cMain.mTestResult.StepResult[TestStepNo].mIsStepPass < 0)//还没有检测
            //        {
            //            for (i = 0; i < cMain.DataShow; i++)
            //            {
            //                ShowLabel[i].Text = "";
            //                ShowLabel[i].BackColor = Color.White;
            //            }
            //            return;
            //        }
            //        else
            //        {
            //            for (i = 0; i < cMain.DataShow; i++)
            //            {
            //                tempValue[i] = cMain.mTestResult.StepResult[TestStepNo].mData[i];
            //            }
            //        }
            //    }
            //    else
            //    {
            //        for (i = 0; i < cMain.DataShow; i++)
            //        {
            //            tempValue[i] = dataShow[i];
            //            cMain.mTempNetResult.StepResult.mData[i] = tempValue[i];
            //        }
            //    }
            //    for (i = 0; i < cMain.DataShow; i++)
            //    {
            //        if ((cMain.mModeSet.mHighData[TestStepNo, i] == 0) && (cMain.mModeSet.mLowData[TestStepNo, i] == 0) ||
            //            ((tempValue[i] <= cMain.mModeSet.mHighData[TestStepNo, i]) &&
            //            (tempValue[i] >= cMain.mModeSet.mLowData[TestStepNo, i])))
            //        {
            //            ShowLabel[i].BackColor = Color.White;
            //            cMain.mTempNetResult.StepResult.mIsDataPass[i] = 1;
            //        }
            //        else
            //        {
            //            if (!isStepData)
            //            {
            //                if (cMain.mModeSet.mStepId[nowStatue.CurrentId] != "待机")
            //                {
            //                    checkTime = cMain.mSysSet.mDianYaQieHuan;
            //                }
            //            }
            //            ShowLabel[i].BackColor = Color.Red;
            //            cMain.mTempNetResult.StepResult.mIsDataPass[i] = 0;
            //        }
            //    }
            //}
        }
        private void initTestFrm(int isPass)//-1为空白,0为不合格,1为合格
        {
            switch (isPass)
            {
                case -1:
                    lblResult.Text = "测试中";
                    lblResult.BackColor = Color.LightYellow;
                    break;
                case 0:
                    lblResult.Text = "不合格";
                    lblResult.BackColor = Color.Pink;
                    udpSend.fUdpSend("O~OK~" + cMain.ThisNo.ToString() + "~false");
                    break;
                case 1:
                    lblResult.Text = "合格";
                    lblResult.BackColor = Color.LightGreen;
                    udpSend.fUdpSend("O~OK~" + cMain.ThisNo.ToString() + "~true");
                    break;
            }
        }
        private void initNowStatue()
        {
            nowStatue.CurrentId = -1;
            nowStatue.firstColdId = 0;
            nowStatue.DiQiId = 0;
            nowStatue.firstHotId = 0;
            nowStatue.isPass = true;
            nowStatue.isStarted = false;
            nowStatue.isStarting = false;
            nowStatue.isStoped = false;
            nowStatue.lastColdId = 0;
            nowStatue.lastHotId = 0;
            nowStatue.mStepShowData = cMain.StepShowData.ShowByLastHot;
            nowStatue.StepCurTime = 0;
            nowStatue.StepStartTime = 0;
            nowStatue.StopId = cMain.StopValue.IsOk;
            nowStatue.TestTime = DateTime.Now;
            nowStatue.TestStartTime = 0;
            nowStatue.TestElectStartTime = 0;
            nowStatue.PrevStep = "";
        }
        /// <summary>
        /// 初始化所有数据
        /// </summary>
        private void initTestData()
        {
            //axiPlotX1.ClearAllData();
            //axiPlotX1.get_XAxis(0).Span = 0.1 / 24;
            //axiPlotX1.get_XAxis(0).Min = DateTime.Now.ToOADate();
            //ShowData(DataNowLabel, nowStatue.CurrentId);
            initTestData(this);


        }
        public static void initTestData(frmMain mFrmMain)//初始化所有数据
        {
            Temp_Step_SnCode = "";
            int i, j;
            int AllTestCount = 0;
            cMain.mModeSet.Load(cMain.mSysSet.mPrevId);
            
            cMain.mNetResult.RunResult.mBar = cMain.mSysSet.mPrevBar;
            cMain.mNetResult.RunResult.mId = cMain.mSysSet.mPrevId;
            cMain.mNetResult.RunResult.mJiQi = cMain.mModeSet.JiQi;
            cMain.mNetResult.RunResult.mMode = cMain.mModeSet.Mode;
            cMain.mNetResult.RunResult.mTestNo = cMain.ThisNo;
            cMain.mNetResult.RunResult.mTestTime = DateTime.Now;
            cMain.mNetResult.RunResult.mIsPass = true;
            cMain.mNetResult.RunResult.mStep = "";
            cMain.mNetResult.RunResult.mStepId = -1;

            cMain.mTempNetResult.RunResult.mBar = cMain.mSysSet.mPrevBar;
            cMain.mTempNetResult.RunResult.mId = cMain.mSysSet.mPrevId;
            cMain.mTempNetResult.RunResult.mJiQi = cMain.mModeSet.JiQi;
            cMain.mTempNetResult.RunResult.mMode = cMain.mModeSet.Mode;
            cMain.mTempNetResult.RunResult.mTestNo = cMain.ThisNo;
            cMain.mTempNetResult.RunResult.mTestTime = DateTime.Now;
            cMain.mTempNetResult.RunResult.mIsPass = true;
            cMain.mTempNetResult.RunResult.mStep = "";
            cMain.mTempNetResult.RunResult.mStepId = -1;


            for ( i = 0; i < cMain.DataShow; i++)
            {
                if (cMain.mModeSet.DataShow[i])
                {
                    mFrmMain.dataGridNow.Columns[i].Visible = true;
                }
                else
                {
                    mFrmMain.dataGridNow.Columns[i].Visible = false;
                }
            }

            mFrmMain.LblBar.Text = cMain.mSysSet.mPrevBar;
            mFrmMain.LblId.Text = cMain.mSysSet.mPrevId;
            mFrmMain.LblMode.Text = cMain.mModeSet.Mode;
            string[] tempStr = cMain.BiaoZhunJiStr[cMain.IndexLanguage].Split(',');
            
            mFrmMain.LblSetTime.Text = "";
            mFrmMain.LblCurTime.Text = "";
            mFrmMain.LblStep.Text = "";
            //初始化显示DataStepGridView
            DataTable dt = new DataTable();
            int stepCount = 0;
            for (i = 0; i < 10; i++)
            {
                dt.Columns.Add("string" + i.ToString(), typeof(string));
                dt.Columns[stepCount].Caption = cMain.mModeSet.Step[i].Text;
                stepCount++;
            }
            string[] tempData = new string[stepCount];
            for (i = 0; i < cMain.DataShow + 1; i++)
            {
                DataRow dr = dt.NewRow();
                dr.ItemArray = tempData;
                dt.Rows.Add(dr);
            }
            mFrmMain.dataGridStep.Font = new Font("宋体", 10);
            mFrmMain.dataGridStep.DataSource = dt;
            ///下面2行为货币管理 ,为了隐藏不须要的行
            mFrmMain.dataGridStep.CurrentCell = null;
            BindingContext bc = new BindingContext();
            CurrencyManager cm = (CurrencyManager)bc[mFrmMain.dataGridStep.DataSource];
            cm.SuspendBinding();// 挂起数据绑定
            ///
            mFrmMain.dataGridStep.RowHeadersWidth = 150;
            for (i = 0; i < cMain.DataShow; i++)
            {
                mFrmMain.dataGridStep.Rows[i].Height = 25;
                mFrmMain.dataGridStep.Rows[i].HeaderCell.Value = cMain.DataShowTitle[i];
                if (cMain.mModeSet.DataShow[i])
                {
                    mFrmMain.dataGridStep.Rows[i].Visible = true;
                }
                else
                {
                    mFrmMain.dataGridStep.Rows[i].Visible = false;
                }
            }
            ///下面1行为货币管理恢复
            cm.ResumeBinding();
            ///
            if (cMain.IndexLanguage == 1)
            {
                mFrmMain.dataGridStep.Rows[cMain.DataShow].HeaderCell.Value = "检测结果";
            }
            else
            {
                mFrmMain.dataGridStep.Rows[cMain.DataShow].HeaderCell.Value = "Result";
            }
            for (i = 0; i < mFrmMain.dataGridStep.Columns.Count; i++)
            {
                //mFrmMain.dataGridStep.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                mFrmMain.dataGridStep.Columns[i].HeaderText = dt.Columns[i].Caption;
                if (cMain.mModeSet.Step[i].TestTime <= 0)
                {
                    mFrmMain.dataGridStep.Columns[i].Visible = false;
                }
                else
                {
                    mFrmMain.dataGridStep.Columns[i].Visible = true;
                }
                mFrmMain.dataGridStep.Columns[i].Width = 73;
            }
            for (i = 0; i < mFrmMain.dataGridStep.ColumnCount; i++)
            {
                mFrmMain.dataGridStep.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            for (i = 9; i >= 0; i--)
            {
                if (cMain.mModeSet.Step[i].TestTime > 0)
                {
                    if (cMain.mModeSet.Step[i].Text == "制冷" || cMain.mModeSet.Step[i].Text == "Cooling")
                    {
                        nowStatue.firstColdId = i;
                    }
                    if (cMain.mModeSet.Step[i].Text == "制热" || cMain.mModeSet.Step[i].Text == "Heating")
                    {
                        nowStatue.firstHotId = i;
                    }
                    if (cMain.mModeSet.Step[i].Text == "低启" || cMain.mModeSet.Step[i].Text == "Low Vol")
                    {
                        nowStatue.DiQiId = i;
                    }
                }
            }


            for (i = 0; i < 10; i++)
            {
                if (cMain.mModeSet.Step[i].TestTime > 0)
                {
                    AllTestCount += cMain.mModeSet.Step[i].TestTime;
                    if (cMain.mModeSet.Step[i].Text == "制冷" || cMain.mModeSet.Step[i].Text == "Cooling")
                    {
                        nowStatue.lastColdId = i;
                    }
                    if (cMain.mModeSet.Step[i].Text == "制热" || cMain.mModeSet.Step[i].Text == "Heating")
                    {
                        nowStatue.lastHotId = i;
                    }
                }
            }
            for (i = 0; i < cMain.DataShow; i++)
            {
                cMain.mNetResult.StepResult.mData[i] = 0;
                cMain.mNetResult.StepResult.mIsDataPass[i] = -1;
                for (j = 0; j < 10; j++)
                {
                    cMain.mTestResult.StepResult[j].mData[i] = 0;
                    cMain.mTestResult.StepResult[j].mIsDataPass[i] = -1;
                }
            }
            cMain.mNetResult.StepResult.mIsStepPass = -1;
            for (i = 0; i < 10; i++)
            {
                cMain.mTestResult.StepResult[i].mIsStepPass = -1;
            }
            cMain.mAllResult.Init();
            cMain.mAllResult.ModeSet = cMain.mModeSet;
            if (AllTestCount > 100)
            {
                mFrmMain.quXianControl1.XAxisMax = mFrmMain.quXianControl1.XPart * 15 * (Math.Ceiling((float)AllTestCount / mFrmMain.quXianControl1.XPart / 15));
                mFrmMain.quXianControl1.reSet();
            }
        }
        /// <summary>
        /// 将当前检测步骤数据发送到计算机
        /// </summary>
        /// <param name="NetResult"></param>
        private static string DataClassToStr(cNetResult NetResult)
        {
            int i = 0;
            string SendStr = "";
            try
            {
                SendStr = SendStr + NetResult.RunResult.mTestTime.ToString() + "~";//检测开始时间
                SendStr = SendStr + NetResult.RunResult.mTestNo.ToString() + "~";//台车号
                SendStr = SendStr + NetResult.RunResult.mStepId.ToString() + "~";
                SendStr = SendStr + NetResult.RunResult.mStep + "~";
                SendStr = SendStr + NetResult.RunResult.mMode + "~";
                SendStr = SendStr + NetResult.RunResult.mJiQi.ToString() + "~";
                SendStr = SendStr + NetResult.RunResult.mIsPass.ToString() + "~";
                SendStr = SendStr + NetResult.RunResult.mId + "~";
                SendStr = SendStr + NetResult.RunResult.mBar + "~";
                for (i = 0; i < cMain.DataShow; i++)
                {
                    SendStr = SendStr + Num.Format(NetResult.StepResult.mData[i], 3) + "~";
                    SendStr = SendStr + NetResult.StepResult.mIsDataPass[i].ToString() + "~";
                }
                SendStr = SendStr + NetResult.StepResult.mIsStepPass.ToString() + "~";
            }
            catch (Exception exc)
            {
                cMain.WriteErrorToLog("FrmMain DataClassToStr " + exc.ToString());
                SendStr = "";
            }
            return SendStr;
        }
        private bool LoadLocalIdByBarCode(string barCode)
        {
            bool returnResult = false;

            DirectoryInfo di = new DirectoryInfo(cMain.AppPath+"\\ID\\");
            int index = 0;
            string[] ListId = new string[di.GetFiles("*.txt").Length];
            foreach (FileInfo fi in di.GetFiles("*.txt"))
            {
                ListId[index] = fi.Name.Substring(0, fi.Name.IndexOf("."));
                index++;
            }
            int LenBar = barCode.Length;
            for (int i = 0; i < 10; i++)
            {
                if (cMain.mBarSet.mIsUse[i] && (LenBar == cMain.mBarSet.mIntBarLength[i])
                   && (cMain.mBarSet.mIntBarLength[i] > 0) && (cMain.mBarSet.mIntBarCount[i] > 0) && (cMain.mBarSet.mIntBarStart[i] > 0)
                   && (cMain.mBarSet.mIntBarLength[i] >= (cMain.mBarSet.mIntBarCount[i] + cMain.mBarSet.mIntBarStart[i] - 1))
                    )
                {
                    string fileName = barCode.Substring(cMain.mBarSet.mIntBarStart[i] - 1, cMain.mBarSet.mIntBarCount[i]);
                    bool isFind = false;
                    for (int j = 0; j < ListId.Length; j++)
                    {
                        if (ListId[j].ToUpper() == fileName.ToUpper())
                        {
                            isFind = true;
                            break;
                        }
                    }
                    if (isFind)
                    {
                        cMain.mSysSet.mPrevBar = barCode;
                        cMain.mSysSet.mPrevId = fileName;
                        frmSys.DataClassToFile(cMain.mSysSet);
                        if (cMain.mBarSet.mIsAutoStart)
                        {
                            Net_Start = true;
                        }
                        break;
                    }
                }
            }
            return returnResult;
        }
        private bool DataStrToClass(string NetString, out cNetModeSet mNetModeSet)
        {
            bool returnResult = false;
            cNetModeSet NetModeSet = new cNetModeSet();
            int i;
            try
            {
                string[] tempStr = NetString.Split('~');
                string tempNetString = "";
                for (i = 3; i < tempStr.Length; i++)
                {
                    tempNetString = tempNetString + tempStr[i] + "~";//tempStr[0]是B识别符,tempStr[1]是条码,tempStr[2]是启动符
                }
                if (tempStr[1] != "")
                {
                    NetModeSet.mBar = tempStr[1];
                }
                NetModeSet.isStart = (tempStr[2] == "1") ? true : false;

                NetModeSet.ModeSet.ToClass(tempNetString);

                cMain.mSysSet.mPrevBar = NetModeSet.mBar;
                cMain.mSysSet.mPrevId = NetModeSet.ModeSet.ID;
                frmSys.DataClassToFile(cMain.mSysSet);
                if (NetModeSet.isStart)
                {
                    Net_Start = true;
                }
                returnResult = true;
            }
            catch (Exception exc)
            {
                cMain.WriteErrorToLog("FrmMain DataStrToClass " + exc.ToString());
            }
            mNetModeSet = NetModeSet;
            return returnResult;
        }
        private bool DataStrToClass(string NetString, out cSystemSet mSystemSet)
        {
            bool returnResult = false;
            cSystemSet SystemSet = new cSystemSet();
            int i;
            try
            {
                string[] tempStr = NetString.Split('~');
                string tempNetString = "";
                for (i = 1; i < tempStr.Length; i++)
                {
                    tempNetString = tempNetString + tempStr[i] + "~";//tempStr[0]为S识别符
                }
                frmSys.DataFileToClass(tempNetString, out SystemSet, false);
                returnResult = true;
            }
            catch (Exception exc)
            {
                cMain.WriteErrorToLog("FrmMain DataStrToClass " + exc.ToString());
            }
            mSystemSet = SystemSet;
            return returnResult;
        }
        private void SetBarText(string s)
        {
            if (LblBar.InvokeRequired)
            {
                LblBar.Invoke(new SendBarCodeHandle(SetBarText), s);
            }
            else
            {
                LblBar.Text = s;
                chkEmpty.Checked = false;
                chkAnGui.Checked = false;
            }
        }
        private void frmMain_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (e.KeyChar == (char)13)
            //{
            //    LblBar.Text = ReadBarCode;
            //    cBarCode tempBarCode = new cBarCode("R~OK~" + string.Format("{0}~", cMain.ThisNo) + ReadBarCode);
            //    tempBarCode.sendBarCodeHandle = new SendBarCodeHandle(SendBarToComputer);
            //    SendBarCode = new Thread(new ThreadStart(tempBarCode.sendBar));
            //    SendBarCode.IsBackground = true;
            //    SendBarCode.Priority = ThreadPriority.BelowNormal;
            //    SendBarCode.Start();
            //    ReadBarCode = "";
            //}
            //else
            //{
            //    ReadBarCode = ReadBarCode + e.KeyChar.ToString();
            //}
            //e.Handled = true;
        }
        private void DataErrorFlush_Tick(object sender, EventArgs e)
        {
            FlushControlText(lblInfo, mError.GetError(), mError.GetErrEnum());
        }

        private void panQuXianName_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolBtnList_Click(object sender, EventArgs e)
        {
            frmList fl = new frmList();
            if (fl.ShowDialog() == DialogResult.Yes)
            {
                if (!fl.isError)
                {
                    cMain.mSysSet.mPrevId = fl.ReturnId;
                    frmSys.DataClassToFile(cMain.mSysSet);
                    frmMain.initTestData(this);
                }
            }
            fl.Dispose();
        }
        private void Chk_Click(object sender, EventArgs e)
        {
        }
        private void btnBar_Click(object sender, EventArgs e)
        {
            ReadBarCodeOver(LblBar.Text);
        }
        private void ReadBarCodeOver(string barcode)
        {
            string[] fileName,mode;
            bool isFindLength = false;//是否找到同长度条码
            bool isFindId=false;//是否找到对应ID
            string findId = "";//找到的ID号ef
            ReadBarCode = barcode.Trim();
            SetBarText(barcode);
            if (cMain.mBarSet.mIsWinCeBar)
            {
                for (int i = 0; i < cMain.mBarSet.mIsUse.Length; i++)
                {
                    if (cMain.mBarSet.mIsUse[i])
                    {
                        if (ReadBarCode.Length == cMain.mBarSet.mIntBarLength[i])
                        {
                            frmList.GetXml(out fileName, out mode);
                            foreach (string ss in fileName)
                            {
                                if (ReadBarCode.Substring(cMain.mBarSet.mIntBarStart[i] - 1, cMain.mBarSet.mIntBarCount[i])
                                    == ss)
                                {
                                    findId = ss;
                                    isFindId = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (isFindId)
                    {
                        cMain.mSysSet.mPrevBar = ReadBarCode;
                        cMain.mSysSet.mPrevId = findId;
                        frmSys.DataClassToFile(cMain.mSysSet);
                        frmMain.initTestData(this);
                        if (cMain.mBarSet.mIsAutoStart)
                        {
                            Handle_Start = true;
                        }
                        return;
                    }
                }
                if (!isFindLength)//没有找到相对应条码
                {
                    MessageBox.Show(string.Format("没有找到条码长度{0}:{1}的条码设置", ReadBarCode.Length, ReadBarCode), "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!isFindId)//没有找到相对应的ID
                {
                    MessageBox.Show(string.Format("没有找到条码:{0}对应的ID机型", ReadBarCode), "错误", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void btnQuXianSet_Click(object sender, EventArgs e)
        {
            
            //System.Resources.ResourceManager rr=new System.Resources.ResourceManager("axiPlotX1.OcxState",System.Reflection.Assembly.GetExecutingAssembly());
            //System.ComponentModel.ComponentResourceManager crm = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));

            //System.Resources.ResourceWriter rw = new System.Resources.ResourceWriter("\\1");
            //rw.AddResource("axiPlotX1.OcxState", crm.GetStream("axiplotX1.OcxState"));
           
        }

        //private void axiPlotX1_OnGotFocusChannel(object sender, AxiPlotLibrary.IiPlotXEvents_OnGotFocusChannelEvent e)
        //{
        //    uint tmpColor = axiPlotX1.get_Channel(e.index).Color;
        //    for (int i = 0; i < axiPlotX1.YAxisCount; i++)
        //    {
        //        axiPlotX1.get_YAxis(i).Visible = false;
        //        axiPlotX1.get_YAxis(i).GridLinesVisible = false;
        //    }
        //    axiPlotX1.get_YAxis(e.index).Visible = true;
        //    axiPlotX1.get_YAxis(e.index).GridLinesVisible = true;
        //    axiPlotX1.get_Channel(e.index).Color = 0;
        //    axiPlotX1.get_Channel(e.index).Color = tmpColor;
        //    axiPlotX1.get_Channel(e.index).Color = 0;
        //    axiPlotX1.get_Channel(e.index).Color = tmpColor;

        //    if (axiPlotX1.DataCursorCount > 0)
        //    {
        //        axiPlotX1.get_DataCursor(0).Style = iPlotLibrary.TxiPlotDataCursorStyle.ipcsValueY;
        //        axiPlotX1.get_DataCursor(0).ChannelName = axiPlotX1.get_Channel(e.index).Name;
        //    }
        //    QuXianShangShuo.Enabled = true;
        //    indexQuXianSelect = e.index;
        //    isShangShuoQuXian = true;
        //    countQuXianShangShuo = 0;
        //}

        private void QuXianShangShuo_Tick(object sender, EventArgs e)
        {
            //int i = indexQuXianSelect;
            //if (isShangShuoQuXian)
            //{
            //    if (countQuXianShangShuo <= 15)
            //    {
            //        if (countQuXianShangShuo == 0)
            //        {
            //            for (int j = 0; j < axiPlotX1.ChannelCount; j++)
            //            {
            //                axiPlotX1.get_Channel(j).Color = QuXianColor[j];
            //            }
            //        }
            //        if ((countQuXianShangShuo % 2) == 0)
            //        {
            //            axiPlotX1.get_Channel(i).Color = 0xFFFFFF;
            //        }
            //        else
            //        {
            //            axiPlotX1.get_Channel(i).Color = QuXianColor[i];
            //        }
            //        countQuXianShangShuo++;
            //    }
            //    else
            //    {
            //        QuXianShangShuo.Enabled = false;
            //        isShangShuoQuXian = false;
            //        for (int j = 0; j < axiPlotX1.ChannelCount; j++)
            //        {
            //            axiPlotX1.get_Channel(j).Color = QuXianColor[j];
            //        }
            //    }
            //}
        }

        private void ChkDCF_MouseClick(object sender, MouseEventArgs e)
        {
            CheckBox tmpChk = (CheckBox)sender;
        }

        private void chkDCF2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmPassWord fp=new frmPassWord();
            if(fp.ShowDialog()==DialogResult.Yes)
            {
                frmSet fs = new frmSet();
                fs.Show();
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            frmPassWord fp = new frmPassWord();
            if (fp.ShowDialog() == DialogResult.Yes)
            {
                frmKB fs = new frmKB();
                fs.Show();
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            frmPassWord fp = new frmPassWord();
            if (fp.ShowDialog() == DialogResult.Yes)
            {
                frmSys fs = new frmSys();
                fs.ShowDialog();
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            frmDataShow fd = new frmDataShow();
            fd.Show();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否确定要退出测试程序?", "请选择", MessageBoxButtons.YesNo, MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void btnHandle_Click(object sender, EventArgs e)
        {
            frmTest ft = new frmTest();
            ft.Show();
        }

        private void btnForm_Click(object sender, EventArgs e)
        {
            if (cMain.LocalSaveValue.FormIndex == 0)
            {
                cMain.LocalSaveValue.FormIndex = 1;
            }
            else
            {
                cMain.LocalSaveValue.FormIndex = 0;
            }
            cMain.LocalSaveValue.Save();
            ChangeForm(cMain.LocalSaveValue.FormIndex);
        }
        private void ChangeForm(int index)
        {
            switch (index)
            {
                case 1:
                    panNow.Height = 0;
                    tabControl1.SelectedIndex = 0;
                    break;
                case 0:
                    panNow.Height = 100;
                    tabControl1.SelectedIndex = 1;
                    break;
            }
        }

        private void panel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    panElect.Parent = tabPage1;
                    break;
                case 2:
                    panElect.Parent = tabPage3;
                    break;
            }
        }
    }
}