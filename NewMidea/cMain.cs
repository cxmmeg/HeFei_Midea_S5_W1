//#define WinCe
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Data;
namespace NewMideaProgram
{
    public  class cMain
    {
        //总设置
        public static bool isComPuter;
        public static bool isDebug = false;//是否调试模式
        public const int DataAll = 36;//读的数据点总数(不超过40)
        public const int DataShow = 50;//显示数据点总数(不超过20)多了没地放
        public const int DataProtect = 20;//保护的个数(不超过20)多了要改界面 
        public const int DataUseProtect = 16;//正在使用的
        public const int DataSystem = 13;//系统设置个数
        public const int DataKaiGuang = 10;//开关量个数(为保持数据格式兼容,最好这里不要改)
        public const int DataPlcMPoint = 28;//PLC输出M点个数
        public const int DataPlcDPoint = 0;//Plc输出D点个数
        public static bool isDanXiang = false;//是否是单相
        public const int AllCount = 4;//小车总数
        public static string[] DataAllTitleStr = new string[2]{"Air Inlet1,Air Outlet1,Air Inlet2,Air Outlet2,Air Inlet3,Air Outlet3,Air Inlet4,Air Outlet4,Pressure1,Pressure2,Pressure3,Pressure4,Voltage A,Voltage B,Voltage C,Current A,Current B,Current C,Power A,Power B,Power C", 
            "A相电压(V),B相电压(V),C相电压(V),A相电流(A),B相电流(A),C相电流(A),A相功率(W),B相功率(W),C相功率(W),"+
            "1#进管温度A(℃),1#出管温度A(℃),1#进管温度B(℃),1#出管温度B(℃),"+
            "2#进管温度A(℃),2#出管温度A(℃),2#进管温度B(℃),2#出管温度B(℃),"+
            "3#进管温度A(℃),3#出管温度A(℃),3#进管温度B(℃),3#出管温度B(℃),"+
            "排气温度(℃),吸气温度(℃),外机风速A,外机风速B,"+
            "1#进风温度(℃),1#出风温度(℃),2#进风温度(℃),2#出风温度(℃),3#进风温度(℃),3#出风温度(℃),"+
            "进管压力(Mpa),出管压力(Mpa),排气压力(Mpa),吸气压力(Mpa),外机压力(Mpa)"
            };
        public static string[] DataShowTitleStr = new string[2] { "Voltage(V),Current(A),Power(W),Pressure(Mpa),Air Inlet(℃),Air Outlet(℃),Temperature Diff(℃),Hz,T1(℃),T2(℃),T3(℃)",
            "A相电压(V),B相电压(V),C相电压(V),A相电流(A),B相电流(A),C相电流(A),功率(W),"+
            "1#进风温度(℃),1#出风温度(℃),1#温差(℃),1#进管温度A(℃),1#出管温度A(℃),1#进管温度B(℃),1#出管温度B(℃),"+
            "2#进风温度(℃),2#出风温度(℃),2#温差(℃),2#进管温度A(℃),2#出管温度A(℃),2#进管温度B(℃),2#出管温度B(℃),"+
            "3#进风温度(℃),3#出风温度(℃),3#温差(℃),3#进管温度A(℃),3#出管温度A(℃),3#进管温度B(℃),3#出管温度B(℃),"+
            "外机风速A,外机风速B,进管压力(Mpa),出管压力(Mpa),排气压力(Mpa),吸气压力(Mpa),排气温度(℃),吸气温度(℃),"+
            "外机压力(Mpa),W.室外温度(℃),W.冷凝器温度A(℃),W.冷凝器温度B(℃),W.排气温度A(℃),W.排气温度B(℃),W.排气温度C(℃),"+
            "W.电流A(A),W.电流B(A),W.电流C(A),W.频率(Hz),W.电子膨胀阀A,W.电子膨胀阀B,W.电量表"
            };
        public static string staticIsShow = "0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0";//1为要显示的曲线,0为不显示
        public static string[] BiaoZhunJiStr = new string[2] { "unit1(A),unit2(B)", "1#标准机A,1#标准机B,2#标准机A,2#标准机B,3#标准机A,3#标准机B" };//标准机3(C),标准机4(D)
        public static string[] BuZhouMingStr = new string[2] { "Low Vol,Heating,Cooling,Stop, ", "低启,制热,制冷,停机,待机" };
        public const string DianYuanStr = "220V,110V";
        public const string DianXinHao = "220V,24V";
        public static string[] JiQiStr = new string[2] { "Normal,SN 600 Old,SN 600 Out,SN 600 New,Normal S(55),Normal S(AA),UL", "定频机,变频SN机,定频SN机,V4+,V6,两管制,三管制" };//,变频M型,变频N型,数码机(1023),数码机(1200单冷),数码机(1200冷暖),天花机(1200冷暖),数码机(12F00×1023),东芝机,窗机";
        public static string[] XiTongStr = new string[2]{"Last barcode,Last ID,Bound,Doing,Plc Com,485 Com,BP Com," +
            "UL Com,Address,Language,PassWord,AutoSn,Pressure ratio K1,Pressure ratio K2,Pressure ratio K3,"+
            "Pressure ratio K4,Pressure ratio B1,Pressure ratio B2,Pressure ratio B3,Pressure ratio B4",
            "PLC串口,485串口,1#PQ串口,2#PQ串口,XY串口,OA串口,K1K2串口,H1H2串口,SN串口,条码串口,7440串口,ESC125串口,抽空串口"};
        
        public static string[] KaiGuangStr = new string[2] { "High wind,Four-way Value,Fan,Compressor,Spare", "高风,低风,压缩机,风机,四通阀,旁通阀" };

        public static string[] DataPlcMPointStr = new string[] {"Start","Ok","Ng","Rest","Step","Stop","Pause", "1#内机","2#内机","3#内机","4#内机","低风","1#阀门","2#阀门","3#阀门","4#阀门","上电","安检","真空阀","备用","黄灯","绿灯","红灯","110V","备用2","5#阀门","6#阀门","总复位" };//控制输出点名称都从这里来
        public static string[] DataPlcMPointInt = new string[] {"1010.0","1012.0","1014.0","1016.0","1018.0","1020.0","1022.0","1030.0","1032.0","1034.0","1036.0","1038.0","1040.0","1042.0","1044.0","1046.0","1048.0","1050.0","1052.0","1054.0","1056.0","1058.0","1060.0","1062.0","1064.0","1066.0","1068.0","1070.0" };//控制输出点对应地址
        public static string[] DataPlcDPointStr =new string[]{ };
        public static int[] DataPlcDPointInt =new int[]{ };
        public const string NetBoardName = "EMAC1";//此处是网卡的名称,用来在注册表取IP地址的
        public static string RemoteHostName = "192.168.1.165";//上位机地址
        public static int IndexLanguage = 1;
        public static string strLanguage = "zh-CN";
        public const int FlushDataTime = 1000;//刷新数据显示时间间隔
        public static bool isNeedPassWord = true;//界面是否启用密码
        public static string AppPath = Application.StartupPath + "\\";
        public float xSize = 1, ySize = 1;//屏幕缩放倍数
        public string[] DataAllTitle = new string[40];
        public static string[] DataShowTitle = new string[20];
        public static Dictionary<string, int> ValueToDoor = new Dictionary<string, int>();
        //当前界面用
        public static cModeSet mModeSet = new cModeSet();//机型设置信息
        public static cSystemSet mSysSet = new cSystemSet();//系统设置信息
        public static cTestResult mTestResult = new cTestResult();//步骤检测数据,用来显示到界面
        public static cNetResult mNetResult = new cNetResult();//步骤检测数据,用来传送上位机
        public static cNetResult mTempNetResult = new cNetResult();//上传上位机实时数据
        public static cAllResult mAllResult = new cAllResult();//用于mes数据保存
        public static cKBValue mKBValue = new cKBValue();//KB值设置信息
        public static cNetModeSet mNetModeSet = new cNetModeSet();//检测条码等设置
        public static cBarSet mBarSet = new cBarSet();//本地条码识别
        public static LocalSaveValue LocalSaveValue = new LocalSaveValue();
        public static cXml mBarXml = new cXml(cMain.AppPath + "\\barset.xml");//条码存储XML
        public static int ThisNo = 0;
        static int indexFrmSetLabel = 0;
        public static cSaveDataToSQL SaveDataToSQL = new cSaveDataToSQL();
        static object o = new object();
        #region
        /// <summary>
        /// 当前检测状态
        /// </summary>
        public struct NowStatue
        {
            /// <summary>
            /// 检测开始时间,用于传回给上位机的检测开始时间
            /// </summary>
            public DateTime TestTime;
            /// <summary>
            /// 检测开始时间,用于给画曲线用的计数开始时间
            /// </summary>
            public int TestStartTime;
            /// <summary>
            /// 上电测试开始
            /// </summary>
            public int TestElectStartTime;
            /// <summary>
            /// 步骤开始时间
            /// </summary>
            public int StepStartTime;
            /// <summary>
            /// 当前步骤已运行时间
            /// </summary>
            public int StepCurTime;
            /// <summary>
            /// 当前正在检测的步骤
            /// </summary>
            public int CurrentId;//当前正在检测的步骤```
            /// <summary>
            /// 是否开始检测
            /// </summary>
            public bool isStarting;//是否开始检测
            /// <summary>
            /// 停机原因ID号
            /// </summary>
            public StopValue StopId;//停止的原因ID```
            /// <summary>
            /// 是否已完成启动输出
            /// </summary>
            public bool isStarted;//是否启动完成
            /// <summary>
            /// 是否已完成停止复位输出
            /// </summary>
            public bool isStoped;//量否停止完成`
            /// <summary>
            /// 第一个低启的步骤号
            /// </summary>
            public int DiQiId;//低启步骤号````
            /// <summary>
            /// 第一个制热的步骤号
            /// </summary>
            public int firstHotId;//第一个制热的步骤号```
            /// <summary>
            /// 第一个制冷的步骤号
            /// </summary>
            public int firstColdId;//第一个制冷的步骤号``
            /// <summary>
            /// 最后一个制热
            /// </summary>
            public int lastHotId;
            /// <summary>
            /// 最后一个制冷
            /// </summary>
            public int lastColdId;
            /// <summary>
            /// 自动显示步骤测试数据
            /// </summary>
            public StepShowData mStepShowData;
            /// <summary>
            /// 当前检测是否合格
            /// </summary>
            public bool isPass;//是否合格````
            /// <summary>
            /// 上一个测试步骤
            /// </summary>
            public string PrevStep;
        }//当前检测状态
        /// <summary>
        /// 当前检测停机原因
        /// </summary>
        public enum StopValue
        {
            /// <summary>
            /// 正常停机
            /// </summary>
           IsOk=0, 
            /// <summary>
            /// 正压收氟保护停机
            /// </summary>
            ZYSFProtect,
            /// <summary>
            /// 高压保护停机
            /// </summary>
            HighPressProtect,
            /// <summary>
            /// 低压保护停机
            /// </summary>
            LowPressProtect,
            /// <summary>
            /// 电流过大保护停机
            /// </summary>
            HighCurProtect,
            /// <summary>
            /// 电流过小保护停机
            /// </summary>
            LowCurProtect,
            /// <summary>
            /// 切换电源失败
            /// </summary>
            ChangeVolFail
        }//停机原因
        /// <summary>
        /// 步骤数据显示窗口,自动显示哪一步数据
        /// </summary>
        public enum StepShowData
        {
            /// <summary>
            /// 自动,不显示
            /// </summary>
            ShowByAuto=0,
            /// <summary>
            /// 自动显示第一个制热数据
            /// </summary>
            ShowByFirstHot,
            /// <summary>
            /// 自动显示最后一个制热的数据
            /// </summary>
            ShowByLastHot,
            /// <summary>
            /// 自动显示第一个制冷数据
            /// </summary>
            ShowByFirstCold,
            /// <summary>
            /// 自动显示最后一个制冷数据
            /// </summary>
            ShowByLastCold,
            /// <summary>
            /// 自动显示第一个不合格步骤数据
            /// </summary>
            ShowByFirstNG,
            /// <summary>
            /// 自动显示上一个测试步骤数据
            /// </summary>
            ShowByPreStep
        }
        public enum PassWord
        {
            NoPassWord=0,
            PassWord1,
            PassWord22,
            PassWord333,
            PassWord110,
            PassWord911//超级密码来着
        }
        #endregion
        public cMain()//系统初始化
        {
            if (System.Environment.OSVersion.Platform.ToString().ToUpper() == "WIN32NT")
            {
                isComPuter = true;
                AppDomain.CurrentDomain.SetData("DataDirectory",AppPath);
            }
            else
            {
                isComPuter = false;
            }
            if (!isComPuter)
            {
                xSize = (Single)640 / (Single)Screen.PrimaryScreen.Bounds.Width;
                ySize = (Single)400 / (Single)Screen.PrimaryScreen.Bounds.Height;
            }
            if (!Directory.Exists(cMain.AppPath))
            {
                Directory.CreateDirectory(cMain.AppPath);
            }
            if (!File.Exists(cMain.AppPath+"\\Log.txt"))
            {
                File.AppendText(cMain.AppPath+"\\Log.txt");
            }
            if (!Directory.Exists(cMain.AppPath+"\\ID\\"))
            {
                Directory.CreateDirectory(cMain.AppPath+"\\ID\\");
            }
            if (!mModeSet.Load("DEF"))
            {
                mModeSet.ID = "DEF";
                mModeSet.Save();
            }
            
            //if (!File.Exists(cMain.AppPath+"\\SystemInfo.txt"))
            //{
            //    frmSys.DataClassToFile(mSysSet);
            //    RemoteHostName = "200.200.200." + mSysSet.mShangWeiJi.ToString();
            //}
            //else 
            //{
            //    if (!frmSys.DataFileToClass((cMain.AppPath+"\\SystemInfo.txt"), out mSysSet,true))
            //    {
            //        frmSys.DataClassToFile(mSysSet);
            //    }
            //    RemoteHostName = "200.200.200." + mSysSet.mShangWeiJi.ToString();
            //}
            if (!File.Exists(cMain.AppPath+"\\KbValue.txt"))
            {
                frmKB.DataClassToFile(mKBValue);
            }
            else 
            {
                if (!frmKB.DataFileToClass(cMain.AppPath+"\\KBValue.txt", out mKBValue))
                {
                    frmKB.DataClassToFile(mKBValue);
                }
            }
            if (!File.Exists(cMain.AppPath + "\\BarSet.xml"))
            {
                mBarXml = new cXml(cMain.AppPath + "\\BarSet.xml");
                frmBarSet.DataClsToTxt(mBarSet);
            }
            else
            {
                if (!frmBarSet.DataTxtToCls(cMain.AppPath + "\\BarSet.xml", out mBarSet))
                {
                    frmBarSet.DataClsToTxt(mBarSet);
                }
            }
            LocalSaveValue.Load();
            if (!Directory.Exists(LocalSaveValue.MesDirectory))
            {
                try
                {
                    Directory.CreateDirectory(LocalSaveValue.MesDirectory);
                }
                catch
                {
                    MessageBox.Show("对不起，MES文件路径错误，请重新设置", "错误的路径", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            DataAllTitle = DataAllTitleStr[cMain.IndexLanguage].Split(',');
            DataShowTitle = DataShowTitleStr[cMain.IndexLanguage].Split(',');
        }
        public static void CreateFile(string FileName)
        {
            FileStream fs = File.Create(FileName);
            fs.Close();
        }
        public static string ReadFile(string FileName)//读取文本文件
        {
            string returnStr="";
            try
            {
                StreamReader sr = new StreamReader(FileName);
                returnStr = sr.ReadToEnd();
                sr.Close();
                sr = null;
            }
            catch(Exception exc)
            {
                WriteErrorToLog("cMain ReadFile is Error " + exc.ToString());
            }
            return returnStr;
        }
        /// <summary>
        /// 将指定字符串写入到指定的文件中
        /// </summary>
        /// <param name="FileName">string,文件名</param>
        /// <param name="WriteStr">string,字符串</param>
        /// <param name="append">bool,是否为追加模式,true,追加到文件中字符末尾,false,重写文件字符</param>
        public static void WriteFile(string FileName, string WriteStr,bool append)//写入文本文件
        {
            StreamWriter sr = new StreamWriter(FileName, append);
            sr.Write(WriteStr);
            sr.Close();
            sr = null;
        }
        /// <summary>
        /// 将出错信息写入日志
        /// </summary>
        /// <param name="ErrorStr">出错信息</param>
        public static void WriteErrorToLog(string ErrorStr)
        {
            lock (o)
            {
                FileInfo fi = new FileInfo(cMain.AppPath+"\\Log.txt");
                if (fi.Length > 4096 * 1024)//防止错误年月累加,把整个硬盘都用光..
                {
                    WriteFile(cMain.AppPath+"\\Log.txt", DateTime.Now.ToLocalTime() + "  " + ErrorStr + (char)13 + (char)10, false);
                }
                else
                {
                    WriteFile(cMain.AppPath+"\\Log.txt", DateTime.Now.ToLocalTime() + "  " + ErrorStr + (char)13 + (char)10, true);
                }
                fi = null;
            }

        }
        /// <summary>
        /// 缩放窗口中的控件.
        /// </summary>
        /// <param name="sender">要进行窗体控件缩放的窗体</param>
        public static void initFrom(System.Windows.Forms.Control.ControlCollection cc)//缩放窗体
        {
            int j = 0;
            Panel nowPanel;
            Button nowButton;
            TextBox nowTextBox;
            Label nowLabel;
            ComboBox nowComboBox;
            CheckBox nowCheckBox;
            DataGrid nowDataGrid;
            DataGridView nowDataGridView;
            RadioButton nowRadioButton;
            DateTimePicker nowDateTimePicker;
            IEnumerator FormEnum = cc.GetEnumerator();
            while (FormEnum.MoveNext())
            {
                if (FormEnum.Current is RadioButton)
                {
                    nowRadioButton = (RadioButton)FormEnum.Current;
                    nowRadioButton.Left = (int)(nowRadioButton.Left / frmMain.mMain.xSize);
                    nowRadioButton.Top = (int)(nowRadioButton.Top / frmMain.mMain.ySize);
                    nowRadioButton.Width = (int)(nowRadioButton.Width / frmMain.mMain.xSize);
                    nowRadioButton.Height = (int)(nowRadioButton.Height / frmMain.mMain.ySize);
                }
                if (FormEnum.Current is DateTimePicker)
                {
                    nowDateTimePicker = (DateTimePicker)FormEnum.Current;
                    nowDateTimePicker.Left = (int)(nowDateTimePicker.Left / frmMain.mMain.xSize);
                    nowDateTimePicker.Top = (int)(nowDateTimePicker.Top / frmMain.mMain.ySize);
                    nowDateTimePicker.Width = (int)(nowDateTimePicker.Width / frmMain.mMain.xSize);
                    nowDateTimePicker.Height = (int)(nowDateTimePicker.Height / frmMain.mMain.ySize);
                }
                if (FormEnum.Current is Panel)
                {
                    nowPanel = (Panel)FormEnum.Current;
                    nowPanel.Left = (int)(nowPanel.Left / frmMain.mMain.xSize);
                    nowPanel.Top = (int)(nowPanel.Top / frmMain.mMain.ySize);
                    nowPanel.Width = (int)(nowPanel.Width / frmMain.mMain.xSize);
                    nowPanel.Height = (int)(nowPanel.Height / frmMain.mMain.ySize);
                }
                if (FormEnum.Current is Button)
                {
                    nowButton = (Button)FormEnum.Current;
                    nowButton.Left = (int)(nowButton.Left / frmMain.mMain.xSize);
                    nowButton.Top = (int)(nowButton.Top / frmMain.mMain.ySize);
                    nowButton.Width = (int)(nowButton.Width / frmMain.mMain.xSize);
                    nowButton.Height = (int)(nowButton.Height / frmMain.mMain.ySize);
                }
                if (FormEnum.Current is TextBox)//文本框
                {
                    nowTextBox = (TextBox)FormEnum.Current;
                    nowTextBox.Left = (int)(nowTextBox.Left / frmMain.mMain.xSize);
                    nowTextBox.Top = (int)(nowTextBox.Top / frmMain.mMain.ySize);
                    nowTextBox.Width = (int)(nowTextBox.Width / frmMain.mMain.xSize);
                    nowTextBox.Height = (int)(nowTextBox.Height / frmMain.mMain.ySize);
                }
                if (FormEnum.Current is Label)//标签
                {
                    nowLabel = (Label)FormEnum.Current;
                    nowLabel.Left = (int)(nowLabel.Left / frmMain.mMain.xSize);
                    nowLabel.Top = (int)(nowLabel.Top / frmMain.mMain.ySize);
                    nowLabel.Width = (int)(nowLabel.Width / frmMain.mMain.xSize);
                    nowLabel.Height = (int)(nowLabel.Height / frmMain.mMain.ySize);
                    int intTag =Num.IntParse(nowLabel.Tag);
                    switch (intTag)
                    {
                        case 1:
                            nowLabel.BackColor = System.Drawing.Color.PaleTurquoise;//浅蓝色
                            break;
                        case 2:
                            nowLabel.BackColor = System.Drawing.Color.White;//白色
                            break;
                        case 3:
                            nowLabel.BackColor = System.Drawing.Color.Khaki;//
                            nowLabel.Font = new System.Drawing.Font("宋体", 13, System.Drawing.FontStyle.Bold);
                            break;
                        case 4:
                            nowLabel.BackColor = System.Drawing.Color.Silver;//银色

                            break;
                        case 5:
                            break;
                        case 9://设置界面的标签
                            nowLabel.BackColor = System.Drawing.Color.White;
                            if (indexFrmSetLabel < DataShow * 2)
                            {
                                if ((indexFrmSetLabel % 2) == 0)
                                {
                                    nowLabel.Text = DataShowTitle[(indexFrmSetLabel / 2)] + "Min,最小值".Split(',')[cMain.IndexLanguage];
                                }
                                else
                                {
                                    nowLabel.Text = DataShowTitle[((indexFrmSetLabel - 1) / 2)] + "Max,最大值".Split(',')[cMain.IndexLanguage];
                                }
                                indexFrmSetLabel++;
                            }
                            else
                            {
                                nowLabel.Visible = false;
                            }
                            break;
                        default:
                            nowLabel.BackColor = System.Drawing.Color.White;
                            break;
                    }
                }
                if (FormEnum.Current is ComboBox)//下拉框
                {
                    nowComboBox = (ComboBox)FormEnum.Current;
                    nowComboBox.Left = (int)(nowComboBox.Left / frmMain.mMain.xSize);
                    nowComboBox.Top = (int)(nowComboBox.Top / frmMain.mMain.ySize);
                    nowComboBox.Width = (int)(nowComboBox.Width / frmMain.mMain.xSize);
                    nowComboBox.Height = (int)(nowComboBox.Height / frmMain.mMain.ySize);
                    string[] tmpStr;
                    int intTag = Num.IntParse(nowComboBox.Tag);
                    switch (intTag)
                    {
                        case 1:
                            tmpStr = DianYuanStr.Split(',');
                            for (j = 0; j < tmpStr.Length; j++)
                            {
                                nowComboBox.Items.Add(tmpStr[j]);
                            }
                            break;
                        case 2:
                            tmpStr = BiaoZhunJiStr[cMain.IndexLanguage].Split(',');
                            for (j = 0; j < tmpStr.Length; j++)
                            {
                                nowComboBox.Items.Add(tmpStr[j]);
                            }
                            break;
                        case 3:
                            //tmpStr = JiQiStr[cMain.IndexLanguage].Split(',');
                            //for (j = 0; j < tmpStr.Length; j++)
                            //{
                            //    nowComboBox.Items.Add(tmpStr[j]);
                            //}
                            break;
                        case 4:
                            tmpStr = BuZhouMingStr[cMain.IndexLanguage].Split(',');
                            for (j = 0; j < tmpStr.Length; j++)
                            {
                                nowComboBox.Items.Add(tmpStr[j]);
                            }
                            break;
                        case 7:
                            tmpStr = DianXinHao.Split(',');
                            for (j = 0; j < tmpStr.Length; j++)
                            {
                                nowComboBox.Items.Add(tmpStr[j]);
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (FormEnum.Current is CheckBox)
                {
                    nowCheckBox = (CheckBox)FormEnum.Current;
                    nowCheckBox.Left = (int)(nowCheckBox.Left / frmMain.mMain.xSize);
                    nowCheckBox.Top = (int)(nowCheckBox.Top / frmMain.mMain.ySize);
                    nowCheckBox.Width = (int)(nowCheckBox.Width / frmMain.mMain.xSize);
                    nowCheckBox.Height = (int)(nowCheckBox.Height / frmMain.mMain.ySize);
                }
                if (FormEnum.Current is DataGrid)
                {
                    nowDataGrid = (DataGrid)FormEnum.Current;
                    nowDataGrid.Font = new System.Drawing.Font("宋体", 14, System.Drawing.FontStyle.Regular);
                    string[] tempStr;
                    int intTag = Num.IntParse(nowDataGrid.Tag);
                    switch (intTag)
                    {
                        // 1,保护数据表格,2,上下限数据表格,3,系统设置表格,4,KB值表格,5,单机数据表格,6,条码设置表格
                       
                        case 4:
                            DataTable dt4 = new DataTable("newTable");
                            tempStr = DataAllTitleStr[cMain.IndexLanguage].Split(',');
                            dt4.Columns.Add("name0", typeof(string));
                            dt4.Columns.Add("name1", typeof(string));
                            dt4.Columns.Add("name2", typeof(string));
                            dt4.Columns.Add("name3", typeof(string));
                            dt4.Columns.Add("name4", typeof(string));
                            dt4.Columns.Add("name5", typeof(string));
                            dt4.Columns.Add("name6", typeof(string));
                            dt4.Columns.Add("name7", typeof(string));
                            dt4.Columns.Add("name8", typeof(string));
                            dt4.Columns.Add("name9", typeof(string));
                            int rowCount = 0;
                            rowCount = (int)Math.Ceiling((double)(DataAll / 2.000));
                            for (j = 0; j < rowCount; j++)
                            {
                                DataRow row = dt4.NewRow();
                                row["name0"] = tempStr[j];
                                //row["data1"] = "0";
                                if (j + rowCount < DataAll)
                                {
                                    row["name5"] = tempStr[j + rowCount];
                                }
                                //row["data2"] = "0";
                                dt4.Rows.Add(row);
                            }
                            nowDataGrid.DataSource = dt4;
                            DataGridTableStyle ts4 = new DataGridTableStyle();
                            ts4.MappingName = dt4.TableName;  //映射style对应数据源的表名，很重要，否则无数据显示 
                            int numColumns4 = dt4.Columns.Count;
                            DataGridTextBoxColumn aColumnTextColumn4;
                            for (j = 0; j < numColumns4; j++)
                            {
                                aColumnTextColumn4 = new DataGridTextBoxColumn();
                                aColumnTextColumn4.MappingName = dt4.Columns[j].ColumnName; //映射数据源的列名，很重要，否则无数据显示
                                switch(j)
                                {
                                    case 0:
                                    case 2:
                                    case 3:
                                    case 7:
                                    case 8:
                                        aColumnTextColumn4.Width = nowDataGrid.Width / 10 - 20;
                                        break;
                                    default:
                                        aColumnTextColumn4.Width = nowDataGrid.Width / 10 + 20; ;
                                        break;
                                }
                                ts4.GridColumnStyles.Add(aColumnTextColumn4);
                            }
                            nowDataGrid.TableStyles.Add(ts4);
                            break;
                        case 6:
                            DataTable dt6 = new DataTable("newTable");
                            dt6.Columns.Add("是否启用", typeof(string));
                            dt6.Columns.Add("条码长度", typeof(string));
                            dt6.Columns.Add("条码起始位", typeof(string));
                            dt6.Columns.Add("条码识别码长度", typeof(string));
                            for (int i = 0; i < 10; i++)
                            {
                                DataRow dr = dt6.NewRow();
                                dt6.Rows.Add(dr);
                            } 
                            nowDataGrid.DataSource = dt6;
                            DataGridTableStyle ts6 = new DataGridTableStyle();
                            ts6.MappingName = dt6.TableName;  //映射style对应数据源的表名，很重要，否则无数据显示 
                            int numColumns6 = dt6.Columns.Count;
                            DataGridTextBoxColumn aColumnTextColumn6;
                            for (j = 0; j < numColumns6; j++)
                            {
                                aColumnTextColumn6 = new DataGridTextBoxColumn();
                                aColumnTextColumn6.HeaderText = dt6.Columns[j].ColumnName;
                                aColumnTextColumn6.MappingName = dt6.Columns[j].ColumnName; //映射数据源的列名，很重要，否则无数据显示
                                if (j == 0)
                                {
                                    aColumnTextColumn6.Width = 120;
                                }
                                else
                                {
                                    aColumnTextColumn6.Width = (nowDataGrid.Width - 120) / 3;
                                }
                                ts6.GridColumnStyles.Add(aColumnTextColumn6);
                            }
                            nowDataGrid.TableStyles.Add(ts6);
                            break;
                        case 5:
                            DataTable dt5 = new DataTable("newTable");
                            dt5.Columns.Add("步骤名", typeof(string));
                            dt5.Columns.Add("是否合格", typeof(string));

                            tempStr = DataShowTitleStr[cMain.IndexLanguage].Split(',');
                            for (int i = 0; i < tempStr.Length; i++)
                            {
                                dt5.Columns.Add(tempStr[i], typeof(string));
                            }
                            for (int i = 0; i < 10; i++)
                            {
                                DataRow dr = dt5.NewRow();
                                dt5.Rows.Add(dr);
                            } 
                            nowDataGrid.DataSource = dt5;
                            DataGridTableStyle ts5 = new DataGridTableStyle();
                            ts5.MappingName = dt5.TableName;  //映射style对应数据源的表名，很重要，否则无数据显示 
                            int numColumns5 = dt5.Columns.Count;
                            DataGridTextBoxColumn aColumnTextColumn5;
                            for (j = 0; j < numColumns5; j++)
                            {
                                aColumnTextColumn5 = new DataGridTextBoxColumn();
                                aColumnTextColumn5.HeaderText = dt5.Columns[j].ColumnName;
                                aColumnTextColumn5.MappingName = dt5.Columns[j].ColumnName; //映射数据源的列名，很重要，否则无数据显示
                                aColumnTextColumn5.Width = 120;
                                ts5.GridColumnStyles.Add(aColumnTextColumn5);
                            }
                            nowDataGrid.TableStyles.Add(ts5);
                            break;
                        case 2:
                            //DataTable dt1 = new DataTable("newTable");
                            //tempStr = DataShowTitleStr.Split(',');
                            //string[] tempStr2 = KaiGuangStr.Split(',');
                            //dt1.Columns.Add("步骤名", typeof(string));
                            //dt1.Columns.Add("时间", typeof(string));
                            //dt1.Columns.Add("Sn指令", typeof(string));
                            //for (int i = 0; i < DataKaiGuang; i++)
                            //{
                            //    dt1.Columns.Add(tempStr2[i], typeof(string));
                            //}
                            //for (int i = 0; i < DataShow * 2; i++)
                            //{
                            //    if (i % 2 == 0)
                            //    {
                            //        dt1.Columns.Add(tempStr[i / 2] + "Min", typeof(string));
                            //    }
                            //    else
                            //    {
                            //        dt1.Columns.Add(tempStr[(i - 1) / 2] + "Max", typeof(string));
                            //    }
                            //}
                            //for (j = 0; j < 10; j++)
                            //{
                            //    DataRow row = dt1.NewRow();
                            //    dt1.Rows.Add(row);
                            //}
                            //nowDataGrid.DataSource = dt1;
                            //DataGridTableStyle ts1 = new DataGridTableStyle();
                            //ts1.MappingName = dt1.TableName;  //映射style对应数据源的表名，很重要，否则无数据显示 
                            //int numColumns1 = dt1.Columns.Count;
                            //DataGridTextBoxColumn aColumnTextColumn1;
                            //for (j = 0; j < numColumns1; j++)
                            //{
                            //    aColumnTextColumn1 = new DataGridTextBoxColumn();
                            //    aColumnTextColumn1.HeaderText = dt1.Columns[j].ColumnName;
                            //    aColumnTextColumn1.MappingName = dt1.Columns[j].ColumnName; //映射数据源的列名，很重要，否则无数据显示
                            //    switch (j)
                            //    {
                            //        case 0:
                            //            aColumnTextColumn1.Width = 80;//步骤名
                            //            break;
                            //        case 1:
                            //            aColumnTextColumn1.Width = 50;//时间
                            //            break;
                            //        case 2:
                            //            aColumnTextColumn1.Width = 0;//sn指令
                            //            break;
                            //        case 3:
                            //        case 7:
                            //            aColumnTextColumn1.Width = 50;
                            //            break;
                            //        case 4:
                            //        case 5:
                            //        case 6:
                            //            aColumnTextColumn1.Width = 80;
                            //            break;
                            //        default:
                            //            aColumnTextColumn1.Width = 120;
                            //            break;
                            //    }
                            //    ts1.GridColumnStyles.Add(aColumnTextColumn1);
                            //}
                            //nowDataGrid.TableStyles.Add(ts1);
                            break;
                    }
                }
                if (FormEnum.Current is DataGridView)
                {
                    string[] tempStr;
                    nowDataGridView = (DataGridView)FormEnum.Current;
                    string intTag = nowDataGridView.Tag.ToString();
                    DataGridViewTextBoxColumn dataGridViewTextBoxColumn;
                    DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn;
                    switch (intTag)
                    {
                        case "KB":
                            for (int i = 0; i < 2; i++)//KB值用分左右两组数据
                            {
                                dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                                dataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                                dataGridViewTextBoxColumn.DefaultCellStyle.BackColor = System.Drawing.Color.LightPink;
                                dataGridViewTextBoxColumn.HeaderText = "Name,参数名称".Split(',')[cMain.IndexLanguage];
                                dataGridViewTextBoxColumn.ValueType = typeof(string);
                                nowDataGridView.Columns.Add(dataGridViewTextBoxColumn);

                                dataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
                                dataGridViewCheckBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                                dataGridViewCheckBoxColumn.HeaderText = "Use?,是否计量".Split(',')[cMain.IndexLanguage];
                                dataGridViewCheckBoxColumn.ValueType = typeof(bool);
                                nowDataGridView.Columns.Add(dataGridViewCheckBoxColumn);

                                dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                                dataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                                dataGridViewTextBoxColumn.DefaultCellStyle.Format = "0.000";
                                dataGridViewTextBoxColumn.HeaderText = "Read,读取值".Split(',')[cMain.IndexLanguage];
                                dataGridViewTextBoxColumn.ValueType = typeof(double);
                                nowDataGridView.Columns.Add(dataGridViewTextBoxColumn);

                                dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                                dataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                                dataGridViewTextBoxColumn.DefaultCellStyle.Format = "0.000";
                                dataGridViewTextBoxColumn.HeaderText = "K Value,计量K值".Split(',')[cMain.IndexLanguage];
                                dataGridViewTextBoxColumn.ValueType = typeof(double);
                                nowDataGridView.Columns.Add(dataGridViewTextBoxColumn);

                                dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                                dataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                                dataGridViewTextBoxColumn.DefaultCellStyle.Format = "0.000";
                                dataGridViewTextBoxColumn.HeaderText = "B Value,计量B值".Split(',')[cMain.IndexLanguage];
                                dataGridViewTextBoxColumn.ValueType = typeof(double);
                                nowDataGridView.Columns.Add(dataGridViewTextBoxColumn);


                                dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                                dataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                                dataGridViewTextBoxColumn.DefaultCellStyle.Format = "0.000";
                                dataGridViewTextBoxColumn.HeaderText = "Reality,实际值".Split(',')[cMain.IndexLanguage];
                                dataGridViewTextBoxColumn.ValueType = typeof(double);
                                nowDataGridView.Columns.Add(dataGridViewTextBoxColumn);
                            }

                            int tempLen = (int)Math.Ceiling(DataAll / 2.00);
                            tempStr = DataAllTitleStr[cMain.IndexLanguage].Split(',');
                            for (int i = 0; i < tempLen; i++)
                            {
                                nowDataGridView.Rows.Add();
                                DataGridViewRow dr = nowDataGridView.Rows[i];
                                dr.Cells[0].Value = tempStr[i];
                                if ((i + tempLen) < tempStr.Length)
                                {
                                    dr.Cells[6].Value = tempStr[i + tempLen];
                                }
                            }
                            break;
                        case "BarCodeSet": 
                            dataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
                            dataGridViewCheckBoxColumn.HeaderText = "是否启用";
                            dataGridViewCheckBoxColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dataGridViewCheckBoxColumn.ValueType = typeof(bool);
                            nowDataGridView.Columns.Add(dataGridViewCheckBoxColumn);

                            dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                            dataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridViewTextBoxColumn.HeaderText = "Barcode Length,条码长度".Split(',')[cMain.IndexLanguage];
                            dataGridViewTextBoxColumn.ValueType = typeof(int);
                            nowDataGridView.Columns.Add(dataGridViewTextBoxColumn);

                            dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                            dataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridViewTextBoxColumn.HeaderText = "ID Start,ID起始位".Split(',')[cMain.IndexLanguage];
                            dataGridViewTextBoxColumn.ValueType = typeof(int);
                            nowDataGridView.Columns.Add(dataGridViewTextBoxColumn);

                            dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                            dataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                            dataGridViewTextBoxColumn.HeaderText = "ID Length,ID长度".Split(',')[cMain.IndexLanguage];
                            dataGridViewTextBoxColumn.ValueType = typeof(int);
                            nowDataGridView.Columns.Add(dataGridViewTextBoxColumn);

                            for (int i = 0; i < 10; i++)
                            {
                                nowDataGridView.Rows.Add();
                                DataGridViewRow dr = nowDataGridView.Rows[i];
                                dr.HeaderCell.Value = string.Format("{0}", i + 1);
                            }
                            break;
                        case "DataShow":
                            tempStr = cMain.DataShowTitleStr[cMain.IndexLanguage].Split(',');
                            for (int i = 0; i < DataShow; i++)
                            {
                                dataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
                                dataGridViewCheckBoxColumn.HeaderText = tempStr[i];
                                dataGridViewCheckBoxColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                dataGridViewCheckBoxColumn.ValueType = typeof(bool);
                                nowDataGridView.Columns.Add(dataGridViewCheckBoxColumn);
                                if (DataShowTitle[i].Contains("R22"))
                                {
                                    ValueToDoor.Add(DataShowTitle[i], 0);
                                }
                                else if (DataShowTitle[i].Contains("R410"))
                                {
                                    ValueToDoor.Add(DataShowTitle[i], 1);
                                }
                                else if (DataShowTitle[i].Contains("1#"))
                                {
                                    ValueToDoor.Add(DataShowTitle[i], 2);
                                }
                                else if (DataShowTitle[i].Contains("2#"))
                                {
                                    ValueToDoor.Add(DataShowTitle[i], 3);
                                }
                                else if (DataShowTitle[i].Contains("3#"))
                                {
                                    ValueToDoor.Add(DataShowTitle[i], 4);
                                }
                                else if (DataShowTitle[i].Contains("4#"))
                                {
                                    ValueToDoor.Add(DataShowTitle[i], 5);
                                }
                                else
                                {
                                    ValueToDoor.Add(DataShowTitle[i], -1);
                                }
                            }
                            nowDataGridView.Rows.Add(1);
                            break;
                        case "UpAndDownSet":
                            DataGridViewComboBoxColumn dataGridViewComboBoxColumn = new DataGridViewComboBoxColumn();
                            tempStr = cMain.BuZhouMingStr[cMain.IndexLanguage].Split(',');
                            for (int i = 0; i < tempStr.Length; i++)
                            {
                                dataGridViewComboBoxColumn.Items.Add(tempStr[i]);
                            }
                            dataGridViewComboBoxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            dataGridViewComboBoxColumn.HeaderText = "Name,步骤名".Split(',')[cMain.IndexLanguage];
                            dataGridViewComboBoxColumn.ValueType = typeof(string);
                            nowDataGridView.Columns.Add(dataGridViewComboBoxColumn);

                            dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                            dataGridViewTextBoxColumn.HeaderText = "Time,时间".Split(',')[cMain.IndexLanguage];
                            dataGridViewTextBoxColumn.ValueType = typeof(int);
                            nowDataGridView.Columns.Add(dataGridViewTextBoxColumn);

                            dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                            dataGridViewTextBoxColumn.HeaderText = "Sn Code,SN指令".Split(',')[cMain.IndexLanguage];
                            dataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                            dataGridViewTextBoxColumn.ValueType = typeof(string);
                            nowDataGridView.Columns.Add(dataGridViewTextBoxColumn);

                            tempStr = cMain.KaiGuangStr[cMain.IndexLanguage].Split(',');//开关量
                            for (int i = 0; i < DataKaiGuang; i++)
                            {
                                dataGridViewCheckBoxColumn = new DataGridViewCheckBoxColumn();
                                dataGridViewCheckBoxColumn.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                                dataGridViewCheckBoxColumn.ValueType = typeof(bool);
                                nowDataGridView.Columns.Add(dataGridViewCheckBoxColumn);
                                if (i < tempStr.Length)
                                {
                                    dataGridViewCheckBoxColumn.HeaderText = tempStr[i];
                                }
                                else
                                {
                                    nowDataGridView.Columns[3 + i].Visible = false;
                                }
                            }
                            tempStr = cMain.DataShowTitleStr[cMain.IndexLanguage].Split(',');
                            for (int i = 0; i < DataShow * 2; i++)
                            {
                                dataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                                if ((i % 2) == 0)
                                {
                                    dataGridViewTextBoxColumn.HeaderText = string.Format("{0}_Min", tempStr[i / 2]);
                                }
                                else 
                                {
                                    dataGridViewTextBoxColumn.HeaderText = string.Format("{0}_Max", tempStr[(i - 1) / 2]);
                                }
                                dataGridViewTextBoxColumn.ValueType = typeof(double);
                                nowDataGridView.Columns.Add(dataGridViewTextBoxColumn);
                            }
                            nowDataGridView.Rows.Add(10);
                            for (int i = 0; i < 10; i++)
                            {
                                nowDataGridView.Rows[i].HeaderCell.Value = string.Format("{0}", i + 1);
                                if ((i % 2) == 1)
                                {
                                    nowDataGridView.Rows[i].DefaultCellStyle.BackColor = System.Drawing.SystemColors.ControlLight;
                                }
                            }
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 计算CRC校验
        /// </summary>
        /// <param name="mByte">要计算CRC的 buff数组</param>
        /// <param name="mLen">要计算CRC的 buff长度</param>
        /// <param name="CrcLo">CRC计算后返回低字节</param>
        /// <param name="CrcHi">CRC计算后返回高字节</param>//CRC校验说明
        public static void CRC_16(byte[] mByte, int mLen, ref byte CrcLo, ref byte CrcHi)//计算CRC校验
        {
            if (mLen <= 0)
            {
                return;
            }
            CrcHi = 0;
            CrcLo = 0;
            int i, j;
            long maa = 0xFFFF;
            long mbb = 0;
            for (i = 0; i < mLen; i++)
            {
                CrcHi = (byte)((maa >> 8) & 0xFF);
                CrcLo = (byte)((maa) & 0xFF);
                maa = (CrcHi << 8) & 0xFF00;
                maa = maa + (long)((CrcLo ^ mByte[i]) & 0xFF);
                for (j = 0; j < 8; j++)
                {
                    mbb = 0;
                    mbb = maa & 0x1;
                    maa = (maa >> 1) & 0x7FFF;
                    if (mbb != 0)
                    {
                        maa = (maa ^ 0xA001) & 0xFFFF;
                    }

                }

            }
            CrcLo = (byte)((byte)maa & (byte)0xFF);
            CrcHi = (byte)((byte)(maa >> 8) & (byte)0xFF);
        }
        
    }
    #region
    /// <summary>
    /// 读取D点解析
    /// </summary>
    public enum MPoint
    {
        /// <summary>
        /// 正收停机
        /// </summary>
        ZYSFAdd = 4,//isOK
        /// <summary>
        /// 下一步制冷
        /// </summary>
        NextColdAdd = 8,
        /// <summary>
        /// 下一步制热
        /// </summary>
        NextHotAdd = 2,
        /// <summary>
        /// 下一步低启
        /// </summary>
        NextDiQiAdd = 16,
        /// <summary>
        /// 行程开关
        /// </summary>
        XingChengAdd =1,//isOk
        /// <summary>
        /// 手动停止
        /// </summary>
        StopAdd = 64,//isOK
        /// <summary>
        /// 手动开始
        /// </summary>
        StartAdd = 32//isOk
    }
    public static class Num
    {
        /// <summary>
        /// 将字符串转化为字节数组
        /// </summary>
        /// <param name="value">string,要转化的字符串</param>
        /// <returns>by[],转化后的字节数组</returns>
        public static byte[] GetHexByte(string value)
        {
            value = value.Trim();
            if ((value.Length % 2) != 0)
            {
                return null;
            }
            byte[] b = new byte[value.Length / 2];
            for (int i = 0; i < b.Length; i++)
            {
                b[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
            }
            return b;
        }
        /// <summary>
        /// 将已知的字节数组转化为字符串
        /// </summary>
        /// <param name="value">byte[],字节数组</param>
        /// <returns>string,转化后的字符串</returns>
        public static string GetHexString(byte[] value)
        {
            string s = "";
            for (int i = 0; i < value.Length; i++)
            {
                s = string.Format("{0}{1:X2}", s, value[i]);
            }
            return s;
        }
        public static bool[] Int2Bool(UInt32 value)
        {
            bool[] result = new bool[32];
            string tmpValue = Convert.ToString(value, 2).PadLeft(32,'0');
            for (int i = 0; i < result.Length; i++)
            {
                if (tmpValue.Substring(31 - i, 1) == "1")
                {
                    result[i] = true;
                }
                else
                {
                    result[i] = false;
                }
            }
            return result;
        }
        public static UInt32 ChangeHighAndLow(UInt32 value)
        {
            UInt32 result = 0;
            byte[] buff = new byte[4];
            buff[0] = (byte)((value >> 24) & 0xFF);
            buff[1] = (byte)((value >> 16) & 0xFF);
            buff[2] = (byte)((value >> 8) & 0xFF);
            buff[3] = (byte)((value >> 0) & 0xFF);
            result =(uint)( (buff[3] << 24) + (buff[2] << 16) + (buff[1] << 8) + buff[0]);
            return result;
        }
        public static string trim(string s)
        {
            string result = "";
            byte[] b = Encoding.ASCII.GetBytes(s);
            int len = b.Length;
            for (int i = 0; i < len; i++)
            {
                if ((b[i] >= 0x30 && b[i] <= 0x39)
                    || (b[i] >= 65 && b[i] <= 90)
                    || (b[i] >= 97 && b[i] <= 122))
                {
                    result = result + Encoding.ASCII.GetString(b, i, 1);
                }
            }
            return result;
        }
        /// <summary>
        /// 将指定字符串转化成单精度数据
        /// </summary>
        /// <param name="Num">object,要转化的字符</param>
        /// <returns>Single,转化后的单精度数据</returns>
        public static Single SingleParse(object Num)
        {
            Single s = 0;
            try
            {
                s = Single.Parse(Num.ToString());
            }
            catch
            { }
            return s;
        }
        public static bool BoolParse(object Num)
        {
            bool b = false;
            try
            {
                b = bool.Parse(Num.ToString());
            }
            catch
            { }
            return b;
        }
        /// <summary>
        /// 将指定字符串转化成整形数据
        /// </summary>
        /// <param name="Num">object,要转化的字符</param>
        /// <returns>int,转化后的整形数据</returns>
        public static int IntParse(object Num)
        {
            int s = 0;
            try
            {
                s = int.Parse(Num.ToString());
            }
            catch
            {}
            return s;
        }
        /// <summary>
        /// 将指定字符串转化成双精度数据
        /// </summary>
        /// <param name="Num">object,要转化的字符</param>
        /// <returns>double,转化后的又精度数据</returns>
        public static double DoubleParse(object Num)
        {
            double s = 0;
            try
            {
                s = double.Parse(Num.ToString());
            }
            catch
            { }
            return s;
        }
        public static byte ByteParse(object Num)
        {
            byte b = 0;
            try
            {
                b = byte.Parse(Num.ToString());
            }
            catch { }
            return b;
        }
        /// <summary>
        /// 返回三个数据中最大数据的序号
        /// </summary>
        /// <param name="Num1">int,数据一</param>
        /// <param name="Num2">int,数据二</param>
        /// <param name="Num3">int,数据三</param>
        /// <returns>int,数据最大的序号</returns>
        public static int IndexMax(int Num1, int Num2, int Num3)
        {
            return IndexMax((double)Num1, (double)Num2, (double)Num3);
        }
        /// <summary>
        /// 返回三个数据中最大数据的序号
        /// </summary>
        /// <param name="Num1">double,数据一</param>
        /// <param name="Num2">double,数据二</param>
        /// <param name="Num3">double,数据三</param>
        /// <returns>int,数据最大的序号</returns>
        public static int IndexMax(double Num1, double Num2, double Num3)
        {
            int returnValue = 0;
            if (Num1 >= Num2 && Num1 >= Num3)
            {
                returnValue = 0;
            }
            if (Num2 >= Num1 && Num2 >= Num3)
            {
                returnValue = 1;
            }
            if (Num3 >= Num1 && Num3 >= Num2)
            {
                returnValue = 2;
            }
            return returnValue;
        }
        /// <summary>
        /// 返回三个数据中最小数据的序号
        /// </summary>
        /// <param name="Num1">double,数据一</param>
        /// <param name="Num2">double,数据二</param>
        /// <param name="Num3">double,数据三</param>
        /// <returns>int,数据最小的序号</returns>
        public static int IndexMin(double Num1, double Num2, double Num3)
        {
            int returnValue = 0;
            if (Num1 <= Num2 && Num1 <= Num3)
            {
                returnValue = 0;
            }
            if (Num2 <= Num1 && Num2 <= Num3)
            {
                returnValue = 1;
            }
            if (Num3 <= Num1 && Num3 <= Num2)
            {
                returnValue = 2;
            }
            return returnValue;
        }
        public static long LongParse(object Num)
        {
            long l = 0;
            try
            {
                l = long.Parse(Num.ToString());
            }
            catch { }
            return l;
        }
        /// <summary>
        /// 返回两个数据的最大值
        /// </summary>
        /// <param name="Num1">double,数据一</param>
        /// <param name="Num2">double,数据二</param>
        /// <returns>double,数据的最大值</returns>
        public static double DoubleMax(double Num1, double Num2)
        {
            return Math.Max(Num1, Num2);
        }
        public static double DoubleMax(double Num1, double Num2, double Num3)
        {
            return DoubleMax(DoubleMax(Num1, Num2), Num3);
        }
        public static double DoubleMin(double Num1, double Num2)
        {
            return Math.Min(Num1, Num2);
        }
        public static double DoubleMin(double Num1, double Num2, double Num3)
        {
            return DoubleMin(DoubleMin(Num1, Num2), Num3);
        }
        public static double Rand()
        {
            System.Threading.Thread.Sleep(1);
            Random r = new Random();
            return r.NextDouble();
        }
        public static byte ByteParseFromHex(string data)
        {
            byte returnValue = 0;
            try
            {
                returnValue = Convert.ToByte(data, 16);
            }
            catch(Exception exc)
            {
                cMain.WriteErrorToLog("cMain ByteParseFromHex is Error " + exc.ToString());
            }
            return returnValue;
        }
        /// <summary>
        /// 十六进制数据转化为十进制数据
        /// </summary>
        /// <param name="data">要转化的十六进制数据</param>
        /// <returns>转化后的字节</returns>
        public static byte ByteParseFromHex(object data)
        {
            byte returnData = 0;
            string tempStr = data.ToString();
            if (tempStr.Length <3)
            {
                for (int i = 0; i < tempStr.Length; i++)
                {
                    string temp = tempStr.Substring(i, 1);
                    switch (temp)
                    {
                        case "A":
                            returnData =(byte)( returnData * 16 + 10);
                            break;
                        case "B":
                            returnData = (byte)(returnData * 16 + 11);
                            break;
                        case "C":
                            returnData = (byte)(returnData * 16 + 12);
                            break;
                        case "D":
                            returnData = (byte)(returnData * 16 + 13);
                            break;
                        case "E":
                            returnData = (byte)(returnData * 16 + 14);
                            break;
                        case "F":
                            returnData = (byte)(returnData * 16 + 15);
                            break;
                        case "1":
                        case "2":
                        case "3":
                        case "4":
                        case "5":
                        case "6":
                        case "7":
                        case "8":
                        case "9":
                            returnData =(byte)( returnData + byte.Parse(temp));
                            break;
                        default:
                            returnData =(byte)( returnData * 16);
                            break;
                    }
                }
            }
            return returnData;
        }
        public static string Format(object data, int Len)
        {
            string returnData="";
            try
            {
                switch (Len)
                {
                    case 1:
                        returnData = string.Format("{0:F1}", data);
                        break;
                    case 2:
                        returnData = string.Format("{0:F2}", data);
                        break;
                    case 3:
                        returnData = string.Format("{0:F3}", data);
                        break;
                    default:
                        returnData = string.Format("{0:F0}", data);
                        break;
                }
            }
            catch (Exception exc)
            {
                cMain.WriteErrorToLog("Num Format " +  exc.ToString());
            }
            return returnData;
        }
    }
    #endregion
    /// <summary>
    /// 上位机发送设置
    /// </summary>
    public class cNetModeSet
    {
        /// <summary>
        /// 条码
        /// </summary>
        public string mBar = "";
        /// <summary>
        /// 是否自动启动
        /// </summary>
        public bool isStart = false;
        /// <summary>
        /// 普通机型设置
        /// </summary>
        public cModeSet ModeSet = new cModeSet();
    }
    /// <summary>
    /// 机型设置
    /// </summary>
    public class cModeSet//机型设置
    {
        /// <summary>
        /// 机型ID号
        /// </summary>
        public string ID
        { get; set; }
        /// <summary>
        /// 机型
        /// </summary>
        public string Mode
        { get; set; }
        /// <summary>
        /// 机器,变频,定频,东芝等
        /// </summary>
        public int JiQi
        { get; set; }
        public string Info
        { get; set; }
        /// <summary>
        /// 标准机序号1
        /// </summary>
        public bool[] BiaoZhunJi
        { get; set; }
        public bool LowSpeed
        { get; set; }
        public bool[] KaiGuan
        { get; set; }
        public float[] Protect
        { get; set; }
        public bool[] XinHao
        { get; set; }
        public bool Vol110V
        { get; set; }
        public bool[] DataShow
        { get; set; }
        public cStepSet[] Step
        { get; set; }

        public cModeSet()
        {
            ID = "";
            Mode = "";
            JiQi = 0;
            Info = "";
            BiaoZhunJi = new bool[3];
            for (int i = 0; i < BiaoZhunJi.Length; i++)
            {
                BiaoZhunJi[i] = false;
            }
            LowSpeed = false;
            KaiGuan = new bool[6];
            for (int i = 0; i < KaiGuan.Length; i++)
            {
                KaiGuan[i] = false;
            }
            Protect = new float[2];
            Protect[0] = 100;
            Protect[1] = 4;
            XinHao = new bool[6];
            for (int i = 0; i < XinHao.Length; i++)
            {
                XinHao[i] = false;
            }
            Vol110V = false;
            DataShow = new bool[cMain.DataShow];
            for (int i = 0; i < DataShow.Length; i++)
            {
                DataShow[i] = false;
            }
            Step = new cStepSet[10];
            for (int i = 0; i < Step.Length; i++)
            {
                Step[i] = new cStepSet();
            }
        }
        public string ToNetStr()
        {
            string result = "";
            try
            {
                result = string.Format("'{0}'~'{1}'~{2}~'{3}'~{4}~{5}~{6}~{7}~{8}~{9}~{10}~{11}~{12}",
                    this.ID, this.Mode, this.JiQi, this.Info, this.BiaoZhunJi[0], this.BiaoZhunJi[1], this.BiaoZhunJi[2], false,
                    false,false, false, false, this.LowSpeed);
                result = string.Format("{0}~{1}~{2}~{3}~{4}~{5}~{6}~{7}~{8}~{9}~{10}~{11}~{12}~{13}~{14}~{15}",
                    result, this.KaiGuan[0], this.KaiGuan[1], this.KaiGuan[2], this.KaiGuan[3], this.KaiGuan[4], this.KaiGuan[5],
                    this.XinHao[0], this.XinHao[1], this.XinHao[2], this.XinHao[3], this.XinHao[4], this.XinHao[5], this.Vol110V,
                    this.Protect[0],this.Protect[1]);
                for (int i = 0; i < DataShow.Length; i++)
                {
                    result = string.Format("{0}~{1}", result, DataShow[i]);
                }
                for (int i = 0; i < Step.Length; i++)
                {
                    result = string.Format("{0}~~{1}", result, Step[i].ToNetStr(ID, i));
                }
            }
            catch (Exception e)
            {
                All.Class.Error.Add(e);
            }
            return result;
        }
        public bool ToClass(string netStr)
        {
            bool result = false;
            try
            {
                string[] buff = netStr.Split(new string[] { "~~" }, StringSplitOptions.None);
                string[] modeBuff = buff[0].Split('~');
                int index = 0;
                this.ID = modeBuff[index++].Replace("\'", "");
                this.Mode = modeBuff[index++].Replace("\'", "");
                this.JiQi = All.Class.Num.ToInt(modeBuff[index++]);
                this.Info = modeBuff[index++].Replace("\'", "");
                for (int i = 0; i < this.BiaoZhunJi.Length; i++)
                {
                    this.BiaoZhunJi[i] = All.Class.Num.ToBool(modeBuff[index++]);
                }
                for (int i = this.BiaoZhunJi.Length; i < 8; i++)
                {
                    index++;
                }
                this.LowSpeed = All.Class.Num.ToBool(modeBuff[index++]);
                for (int i = 0; i < this.KaiGuan.Length; i++)
                {
                    this.KaiGuan[i] = All.Class.Num.ToBool(modeBuff[index++]);
                }
                for (int i = 0; i < this.XinHao.Length; i++)
                {
                    this.XinHao[i] = All.Class.Num.ToBool(modeBuff[index++]);
                }
                this.Vol110V = All.Class.Num.ToBool(modeBuff[index++]);
                this.Protect[0] = All.Class.Num.ToFloat(modeBuff[index++]);
                this.Protect[1] = All.Class.Num.ToFloat(modeBuff[index++]);
                for (int i = 0; i < this.DataShow.Length; i++)
                {
                    this.DataShow[i] = All.Class.Num.ToBool(modeBuff[index++]);
                }
                for (int i = 0; i < Step.Length; i++)
                {
                    Step[i].ToClass(buff[i + 1]);
                }
            }
            catch (Exception e)
            {
                result = false;
                All.Class.Error.Add(e);
            }
            return result;
        }
        public bool Save()
        {
            bool result = true;
            try
            {
                string sql = "";
                sql = string.Format("delete from ModeSet where ID='{0}'", this.ID);
                cData.upData(sql, cData.ConnMain);
                sql = this.ToNetStr().Split(new string[] { "~~" }, StringSplitOptions.None)[0].Replace("~", ",");
                sql = string.Format("insert into ModeSet values ({0})", sql);
                if (cData.upData(sql, cData.ConnMain) <= 0)
                {
                    result = false;
                }
                for (int i = 0; i < Step.Length; i++)
                {
                    result = result && Step[i].Save(this.ID, i);
                }
                if (this.ID == cMain.mModeSet.ID)
                {
                    cMain.mModeSet = this;
                }
            }
            catch (Exception e)
            {
                result = false;
                All.Class.Error.Add(e);
            }
            return result;
        }
        public bool Load(string ID)
        {
            bool result = true;
            try
            {
                DataSet ds = cData.readData(string.Format("select * from ModeSet where Id='{0}'", ID), cData.ConnMain);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        this.ID = ID;
                        this.Mode = All.Class.Num.ToString(ds.Tables[0].Rows[0]["Mode"]);
                        this.Info = All.Class.Num.ToString(ds.Tables[0].Rows[0]["Info"]);
                        this.JiQi = All.Class.Num.ToInt(ds.Tables[0].Rows[0]["JiQi"]);
                        for (int i = 0; i < this.BiaoZhunJi.Length; i++)
                        {
                            this.BiaoZhunJi[i] = All.Class.Num.ToBool(ds.Tables[0].Rows[0][string.Format("BiaoZhunJi{0}", i + 1)]);
                        }
                        this.LowSpeed = All.Class.Num.ToBool(ds.Tables[0].Rows[0]["LowSpeed"]);
                        for (int i = 0; i < this.KaiGuan.Length; i++)
                        {
                            this.KaiGuan[i] = All.Class.Num.ToBool(ds.Tables[0].Rows[0][string.Format("KaiGuan{0}", i + 1)]);
                        }
                        for (int i = 0; i < this.XinHao.Length; i++)
                        {
                            this.XinHao[i] = All.Class.Num.ToBool(ds.Tables[0].Rows[0][string.Format("XinHao{0}", i + 1)]);
                        }
                        this.Vol110V = All.Class.Num.ToBool(ds.Tables[0].Rows[0]["Vol110V"]);
                        for (int i = 0; i < this.Protect.Length; i++)
                        {
                            this.Protect[i] = All.Class.Num.ToFloat(ds.Tables[0].Rows[0][string.Format("Protect{0}", i + 1)]);
                        }
                        for (int i = 0; i < this.DataShow.Length; i++)
                        {
                            this.DataShow[i] = All.Class.Num.ToBool(ds.Tables[0].Rows[0][string.Format("DataShow{0}", i + 1)]);
                        }
                    }
                    else
                    {
                        All.Class.Error.Add(string.Format("读取数据行数为0，读取机型为{0}", ID), Environment.StackTrace);
                        return false;
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        result = result && Step[i].Load(ID, i);
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
                result = false;
                All.Class.Error.Add(e);
            }
            return result;
        }
    }
    public class cStepSet
    {
        public string Text
        { get; set; }
        public int TestTime
        { get; set; }
        public int NengJi
        { get; set; }
        public int PinLv
        { get; set; }
        public string SnCode
        { get; set; }
        public string StartInfo
        { get; set; }
        public string EndInfo
        { get; set; }
        public float[] LowData
        { get; set; }
        public float[] HighData
        { get; set; }
        public bool StartCheck
        { get; set; }
        public bool EndCheck
        { get; set; }
        public cStepSet()
        {
            Text = "停机";
            TestTime = 0;
            NengJi = 0;
            PinLv = 0;
            SnCode = "";
            StartInfo = "";
            EndInfo = "";
            StartCheck = false;
            EndCheck = false;
            LowData = new float[cMain.DataShow];
            HighData = new float[cMain.DataShow];
            for (int i = 0; i < cMain.DataShow; i++)
            {
                LowData[i] = 0;
                HighData[i] = 0;
            }
        }
        public string ToNetStr(string ID,int index)
        {
            string result = "{0}{1}{2}";
            string title = "";
            string lowData = "";
            string highData = "";

            title = string.Format("'{0}'~{1}~'{2}'~{3}~{4}~{5}~'{6}'~{7}~{8}~'{9}'~'{10}'",
                ID, index, this.Text, this.TestTime, this.NengJi, this.PinLv, this.SnCode,this.StartCheck,this.EndCheck, this.StartInfo, this.EndInfo);
            for (int i = 0; i < cMain.DataShow; i++)
            {
                lowData = string.Format("{0}~{1}", lowData, this.LowData[i]);
                highData = string.Format("{0}~{1}", highData, this.HighData[i]);
            }
            result = string.Format(result, title, lowData, highData);
            return result;
        }
        public bool ToClass(string netStr)
        {
            bool result = true;
            try
            {
                string[] buff = netStr.Split('~');
                int index = 2;
                this.Text = buff[index++].Replace("\'", "");
                this.TestTime = All.Class.Num.ToInt(buff[index++]);
                this.NengJi = All.Class.Num.ToInt(buff[index++]);
                this.PinLv = All.Class.Num.ToInt(buff[index++]);
                this.SnCode = buff[index++].Replace("\'", "");
                this.StartCheck = All.Class.Num.ToBool(buff[index++]);
                this.EndCheck = All.Class.Num.ToBool(buff[index++]);
                this.StartInfo = buff[index++].Replace("\'", "");
                this.EndInfo = buff[index++].Replace("\'", "");
                for (int i = 0; i < cMain.DataShow; i++)
                {
                    this.LowData[i] = All.Class.Num.ToFloat(buff[index++]);
                }
                for (int i = 0; i < cMain.DataShow; i++)
                {
                    this.HighData[i] = All.Class.Num.ToFloat(buff[index++]);
                }
            }
            catch (Exception e)
            {
                All.Class.Error.Add(e);
            }
            return result;
        }
        public bool Save(string ID,int index)
        {
            bool result = true;
            string sql = "";
            try
            {
                sql = string.Format("delete from StepSet where ID='{0}' and StepIndex={1}", ID, index);
                cData.upData(sql, cData.ConnMain);


                sql = "insert into StepSet Values({0})";
                sql = string.Format(sql, ToNetStr(ID, index).Replace('~', ','));

                if (cData.upData(sql, cData.ConnMain) <= 0)
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
                result = false;
                All.Class.Error.Add(e);
            }
            return result;
        }
        public bool Load(string ID, int index)
        {
            bool result = true;
            try
            {
                DataSet ds = cData.readData(string.Format("select * from StepSet where ID='{0}' and StepIndex={1}  order by stepIndex", ID, index), cData.ConnMain);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        this.Text = All.Class.Num.ToString(ds.Tables[0].Rows[0]["StepText"]);
                        this.TestTime = All.Class.Num.ToInt(ds.Tables[0].Rows[0]["TestTime"]);
                        this.NengJi = All.Class.Num.ToInt(ds.Tables[0].Rows[0]["NengJi"]);
                        this.PinLv = All.Class.Num.ToInt(ds.Tables[0].Rows[0]["PinLv"]);
                        this.SnCode = All.Class.Num.ToString(ds.Tables[0].Rows[0]["SnCode"]);
                        this.StartCheck = All.Class.Num.ToBool(ds.Tables[0].Rows[0]["StartCheck"]);
                        this.EndCheck = All.Class.Num.ToBool(ds.Tables[0].Rows[0]["EndCheck"]);
                        this.StartInfo = All.Class.Num.ToString(ds.Tables[0].Rows[0]["StartInfo"]);
                        this.EndInfo = All.Class.Num.ToString(ds.Tables[0].Rows[0]["EndInfo"]);
                        for (int i = 0; i < cMain.DataShow; i++)
                        {
                            this.LowData[i] = All.Class.Num.ToFloat(ds.Tables[0].Rows[0][string.Format("L{0}", i + 1)]);
                            this.HighData[i] = All.Class.Num.ToFloat(ds.Tables[0].Rows[0][string.Format("H{0}", i + 1)]);
                        }
                    }
                    else
                    {
                        All.Class.Error.Add(string.Format("读取数据行数为0，读取机型为{0}，读取步骤序号为{1}", ID, index), Environment.StackTrace);
                    }
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception e)
            {
                All.Class.Error.Add(e);
            }
            return result;
        }

    }
    public class cSystemSet//系统设置
    {

        /// <summary>
        /// 上一次的条码
        /// </summary>
        public string mPrevBar = "DEF1234567890123";
        /// <summary>
        /// 上一次的ID号;
        /// </summary>
        public string mPrevId = "DEF";
        /// <summary>
        /// 正压收氟模式
        /// </summary>
        public int mZysfMode = 1;
        /// <summary>
        /// 正压收氟动作
        /// </summary>
        public int mZysfDoing = 0;
        /// <summary>
        /// 电压多少后才开始启动
        /// </summary>
        public string[] ComPort = new string[20];
        public string mPassWord = "";


        public string ComPlc
        { get { return ComPort[0]; } }
        public string Com485
        { get { return ComPort[1]; } }

        public string ComPQ1
        { get { return ComPort[2]; } }
        public string ComPQ2
        { get { return ComPort[3]; } }
        public string ComXY
        { get { return ComPort[4]; } }
        public string ComOA
        { get { return ComPort[5]; } }
        public string ComK1K2
        { get { return ComPort[6]; } }
        public string ComH1H2
        { get { return ComPort[7]; } }
        public string ComSn
        { get { return ComPort[8]; } }
        public string ComBar
        { get { return ComPort[9]; } }
        public string Com7440
        { get { return ComPort[10]; } }
        public string Com125
        { get { return ComPort[11]; } }
        public string ComEmpty
        { get { return ComPort[12]; } }

        public cSystemSet()
        {
            for (int i = 0; i < ComPort.Length; i++)
            {
                ComPort[i] = string.Format("COM{0}", i + 1);
            }
            ComPort[1] = "COM1";
            ComPort[1] = "COM3";
            ComPort[2] = "COM4";
            ComPort[3] = "COM5";
            ComPort[4] = "COM6";
            ComPort[5] = "COM7";
            ComPort[6] = "COM8";
            ComPort[7] = "COM9";
            ComPort[8] = "COM10";
            ComPort[9] = "COM11";
            ComPort[10] = "COM13";
            ComPort[11] = "COM14";
            ComPort[12] = "COM2";
        }
    }
    public class cKBValue//KB值
    {
        public double[] valueK = new double[cMain.DataAll];
        public double[] valueB = new double[cMain.DataAll];
        public cKBValue()
        {
            int i;
            for (i = 0; i < cMain.DataAll; i++)
            {
                valueK[i] = 1;
                valueB[i] = 0;
            }
        }
    }
    public class cAllResult
    {
        public cModeSet ModeSet
        { get; set; }
        public cRunResult RunResult
        { get; set; }
        public cStepResult[] StepResult
        { get; set; }
        public cAllResult()
        {
            Init();
        }
        public void Init()
        {
            ModeSet = new cModeSet();
            RunResult = new cRunResult();
            StepResult = new cStepResult[10];
            for (int i = 0; i < StepResult.Length; i++)
            {
                StepResult[i] = new cStepResult();
            }
        }
        public void SetStepResult(int index,cStepResult stepResult)//堆栈差别
        {
            StepResult[index].mIsStepPass = stepResult.mIsStepPass;
            for (int i = 0; i < cMain.DataShow; i++)
            {
                StepResult[index].mData[i] = stepResult.mData[i];
                StepResult[index].mIsDataPass[i] = stepResult.mIsDataPass[i];
            }
        }
        public void Save()
        {
            //DateTime now = DateTime.Now;
            //int stepIndex = 0;
            //int testCount = 0;
            //string testValue = "";
            //string fileName = string.Format("{0}\\{1:yyyyMMddHHmmss}{2}.txt", cMain.LocalSaveValue.MesDirectory,now, RunResult.mBar);
            //string fileValue = string.Format("{0};", RunResult.mBar); //条码
            //fileValue = string.Format("{0}{1:yyyy-MM-dd};{1:HH:mm:ss};", fileValue, now);//时间
            //fileValue = string.Format("{0}{1};", fileValue, RunResult.mIsPass);//结果
            //fileValue = string.Format("{0}{1};", fileValue, "MD");//用户
            //fileValue = string.Format("{0}{1};", fileValue, cUdpSock.LastIp());
            //fileValue = string.Format("{0}{1};", fileValue, "性能测试");
            //fileValue = string.Format("{0}{1};", fileValue, cUdpSock.LoaclIp().ToString());
            //for (int i = 0; i < StepResult.Length && i < ModeSet.mStepId.Length; i++)
            //{
            //    if (ModeSet.mSetTime[i] > 0)
            //    {
            //        fileValue = string.Format("{0}\r\n", fileValue);
            //        stepIndex++;
            //        fileValue = string.Format("{0}{1};", fileValue, stepIndex);
            //        fileValue = string.Format("{0}{1};", fileValue, ModeSet.mStepId[i]);
            //        fileValue = string.Format("{0}{1};", fileValue, (StepResult[i].mIsStepPass != 0));
            //        testCount = 0;
            //        testValue = "";
            //        for (int j = 0; j < ModeSet.mShow.Length; j++)
            //        {
            //            if (ModeSet.mShow[j])
            //            {
            //                testCount++;
            //                testValue = string.Format("{0}\r\n\t{1};{2:F2};{3};{4:F2};{5:F2}",
            //                    testValue,cMain.DataShowTitle[j], StepResult[i].mData[j],
            //                    (StepResult[i].mIsDataPass[j] != 0), ModeSet.mLowData[i,j], ModeSet.mHighData[i,j]);
            //            }
            //        }
            //        fileValue = string.Format("{0}{1}{2}", fileValue, testCount, testValue);
            //    }
            //}
            //cMain.WriteFile(fileName, fileValue, false);
        }
    }
    public class cNetResult//传回上位机数据
    {
        public cRunResult RunResult = new cRunResult();
        public cStepResult StepResult = new cStepResult();
    }
    public class cTestResult//当前检测所有步骤数据
    {
        /// <summary>
        /// 当前检测步数的结果,10元素1维数组
        /// </summary>
        public cStepResult[] StepResult=new cStepResult[10];
        public cTestResult()
        {
            int i;
            for (i = 0; i < 10; i++)
            {
                StepResult[i] = new cStepResult();
            }
        }
    }
    public class cRunResult//正在运行机器信息结果
    {
        /// <summary>
        /// 条码
        /// </summary>
        public string mBar = "";
        /// <summary>
        /// 机型ID号
        /// </summary>
        public string mId = "";
        /// <summary>
        /// 机型
        /// </summary>
        public string mMode = "";
        /// <summary>
        /// 检测台车号
        /// </summary>
        public int mTestNo = 0;
        /// <summary>
        /// 机器,变频,定频,东芝等
        /// </summary>
        public int mJiQi = 0;
        /// <summary>
        /// 检测日期,时间
        /// </summary>
        public DateTime mTestTime = DateTime.Now;
        /// <summary>
        /// 检测总结果是否合格
        /// </summary>
        public bool mIsPass = true;
        /// <summary>
        /// 步骤ID ,当前检测第几步
        /// </summary>
        public int mStepId = -1; //步骤ID 1~10
        /// <summary>
        /// 步骤号,当前检测步骤检测项目
        /// </summary>
        public string mStep = ""; //步骤号 
        /// <summary>
        /// 曲线存放路径
        /// </summary>
        public string mQuXianImage = "";
        /// <summary>
        /// 曲线图值　
        /// </summary>
        public string QuXianValue = "";
    }
    /// <summary>
    /// 单步检测数据
    /// </summary>
    public class cStepResult
    {
        /// <summary>
        /// 检测数据,20元素1维数组
        /// </summary>
        public double[] mData = new double[cMain.DataShow];
        /// <summary>
        /// 检测各项数据是否合格,20元素1维数组,1为合格,0为不合格,-1为未开始检测
        /// </summary>
        public int[] mIsDataPass = new int[cMain.DataShow];
        /// <summary>
        /// 当前步骤是否合格,1为合格,0为不合格,-1为未开始检测
        /// </summary>
        public int mIsStepPass = -1;
        public cStepResult()
        {
            int i;
            for(i=0;i<cMain.DataShow;i++)
            {
                mData[i]=0;
                mIsDataPass[i]=-1;
            }
        }
    }
    /// <summary>
    /// 条码设置
    /// </summary>
    public class cBarSet
    {
        /// <summary>
        /// 扫描条码后是否自动启动
        /// </summary>
        public bool mIsAutoStart = true;
        /// <summary>
        /// 是否使用本机条码识别方法,或者使用远程电脑识别方法
        /// </summary>
        public bool mIsWinCeBar = true;
        /// <summary>
        /// 此条码设置是否使用
        /// </summary>
        public bool[] mIsUse = new bool[10];
        /// <summary>
        /// 条码长度
        /// </summary>
        public int[] mIntBarLength = new int[10];
        /// <summary>
        /// 条码识别码开始位
        /// </summary>
        public int[] mIntBarStart = new int[10];
        /// <summary>
        /// 条码识别码长度
        /// </summary>
        public int[] mIntBarCount = new int[10];
        public cBarSet()
        {
            for (int i = 0; i < 10; i++)
            {
                mIsUse[i] = false;
                mIntBarCount[i] = 0;
                mIntBarStart[i] = 0;
                mIntBarLength[i] = 0;
            }
        }
    }
    public class LocalSaveValue
    {
        static string filePath = string.Format("{0}\\Data\\LocalSaveValue.txt", Application.StartupPath);
        /// <summary>
        /// 界面选择
        /// </summary>
        public int FormIndex
        { get; set; }
        /// <summary>
        /// MES文件夹
        /// </summary>
        public string MesDirectory
        { get; set; }
        /// <summary>
        /// 安检主机IP地址
        /// </summary>
        public string AnGuiParentIp
        { get; set; }
        /// <summary>
        /// 安检读取数据延时
        /// </summary>
        public int AnGuiTimeOut
        { get; set; }
        /// <summary>
        /// 抽真空主机IP地址
        /// </summary>
        public string EmptyParentIp
        { get; set; }
        /// <summary>
        /// 抽空上限
        /// </summary>
        public int EmptyUp
        { get; set; }

        public string LocalIp
        { get; set; }

        public LocalSaveValue()
        {
            FormIndex = 0;
            MesDirectory = "D:\\datatxt\\";
            AnGuiParentIp = "192.168.1.161";
            EmptyParentIp = "192.168.1.161";
            LocalIp = "192.168.1.161";
            AnGuiTimeOut = 12000;
            EmptyUp = 500;
        }
        public void Load()
        {
            LocalSaveValue tmp = (LocalSaveValue)cXml.readXml(filePath, typeof(LocalSaveValue), new LocalSaveValue());
            this.FormIndex = tmp.FormIndex;
            this.MesDirectory = tmp.MesDirectory;
            this.AnGuiParentIp = tmp.AnGuiParentIp;
            this.LocalIp = tmp.LocalIp;
            this.AnGuiTimeOut = All.Class.Num.ToInt(tmp.AnGuiTimeOut);
            this.EmptyParentIp = tmp.EmptyParentIp;
            this.EmptyUp = All.Class.Num.ToInt(tmp.EmptyUp);
            Save();
        }
        public void Save()
        {
            cXml.saveXml(filePath, typeof(LocalSaveValue), this);
        }

    }
}
