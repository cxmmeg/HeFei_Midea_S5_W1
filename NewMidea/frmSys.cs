using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace NewMideaProgram
{
    public partial class frmSys : Form
    {
        Label[] lblTitle = new Label[cMain.DataSystem];
        ComboBox[] lblCom = new ComboBox[cMain.DataSystem];

        public frmSys()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (DataFrmToClass(this, out cMain.mSysSet))
            {
                if (DataClassToFile(cMain.mSysSet))
                {
                    MessageBox.Show("数据保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }
            MessageBox.Show("数据保存失败,请检查数据输入是否有效","错误",MessageBoxButtons.OK,MessageBoxIcon.Error);
            
        }

        private void frmSys_Load(object sender, EventArgs e)
        {
            StartLoad();
        }
        /// <summary>
        /// 因为主frmMdi页面添加此页面过去时不能自动调用本页的Load(object sender,EventArgs e)事件，所以要添加此全局函数供frmMdi手动调用
        /// </summary>
        public void StartLoad()
        {
            frmInit();
            cSystemSet css;
            DataFileToClass(cMain.AppPath+"\\SystemInfo.txt", out css, true);
            DataClassToFrm(this, css);
        }
        private void frmInit()
        {
            for (int i = 0; i < lblTitle.Length; i++)
            {
                lblTitle[i] = (Label)this.Controls.Find(string.Format("label{0}", i + 1), true)[0];
                lblTitle[i].Text = cMain.XiTongStr[cMain.IndexLanguage].Split(',')[i];

                lblCom[i] = (ComboBox)this.Controls.Find(string.Format("comboBox{0}", i + 1), true)[0];

                string[] portName = System.IO.Ports.SerialPort.GetPortNames();
                if (portName != null && portName.Length > 0)
                {
                    lblCom[i].DataSource = portName;
                }
                else
                {
                    for (int j = 0; j < 15; j++)
                    {
                        lblCom[i].Items.Add(string.Format("COM{0}", j + 1));
                    }
                }
            }

        }
        public static bool DataClassToFrm(frmSys mFrmSys, cSystemSet SystemSet)
        {
            bool isOk = false;
            try
            {
                for (int i = 0; i < mFrmSys.lblCom.Length; i++)
                {
                    mFrmSys.lblCom[i].Text = SystemSet.ComPort[i];
                }
                mFrmSys.textBox1.Text = SystemSet.mPassWord;
                isOk = true;
            }
            catch (Exception exc)
            {
                cMain.WriteErrorToLog("FrmSys DataClassToFrm is Error " + exc.ToString());
                isOk = false;
            }
            return isOk;
        }
        public static bool DataFrmToClass(frmSys mFrmSys, out cSystemSet SystemSet)
        {
            bool isOk = false;
            cSystemSet mSystemSet = new cSystemSet();
            try
            {
                for (int i = 0; i < mFrmSys.lblCom.Length; i++)
                {
                    mSystemSet.ComPort[i] = mFrmSys.lblCom[i].Text;
                }
                mSystemSet.mPassWord = mFrmSys.textBox1.Text;
                isOk = true;
            }
            catch (Exception exc)
            {
                cMain.WriteErrorToLog("FrmSys DataFrmToClass is Error " + exc.ToString());
                isOk = false;
            }
            SystemSet = mSystemSet;
            return isOk;
        }

        public static bool DataFileToClass(string FileStr, out cSystemSet SystemSet, bool isPath)
        {
            bool isOk = false;
            cSystemSet mSystemSet;
            string readFile;
            if (isPath)
            {
                readFile = cMain.ReadFile(FileStr);
            }
            else
            {
                readFile = FileStr;
            }
            try
            {
                mSystemSet = new cSystemSet();
                string[] tempStr;
                tempStr = readFile.Split('~');
                mSystemSet.mPrevBar = tempStr[0];
                mSystemSet.mPrevId = tempStr[1];
                mSystemSet.mZysfMode = Num.IntParse(tempStr[2]);
                mSystemSet.mZysfDoing = Num.IntParse(tempStr[3]);
                mSystemSet.mPassWord = tempStr[4];
                for (int i = 0, j = 5; i < mSystemSet.ComPort.Length && j < tempStr.Length; i++, j++)
                {
                    mSystemSet.ComPort[i] = tempStr[j];
                }
                isOk = true;
            }
            catch (Exception exc)
            {
                mSystemSet = new cSystemSet();
                cMain.WriteErrorToLog("FrmSys DataFileToClass is Error " + exc.ToString());
                isOk = false;
            }
            SystemSet = mSystemSet;
            return isOk;
        }
        public static bool DataClassToFile(cSystemSet SystemSet)
        {
            bool isOk = false;
            string tempStr = "";
            try
            {
                tempStr = tempStr + SystemSet.mPrevBar + "~";
                tempStr = tempStr + SystemSet.mPrevId + "~";
                tempStr = tempStr + SystemSet.mZysfMode.ToString() + "~";
                tempStr = tempStr + SystemSet.mZysfDoing.ToString() + "~";
                tempStr = tempStr + SystemSet.mPassWord + "~";
                for (int i = 0; i < SystemSet.ComPort.Length; i++)
                {
                    tempStr = tempStr + SystemSet.ComPort[i] + "~";
                }
                cMain.WriteFile(cMain.AppPath+"\\SystemInfo.txt", tempStr, false);
                isOk = true;
            }
            catch (Exception exc)
            {
                cMain.WriteErrorToLog("FrmSys DataClassToFile is Error " + exc.ToString());
                isOk = false;
            }
            return isOk;
        }

        private void GridSys_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void toolBtnLanguage_Click(object sender, EventArgs e)
        {
            frmSys.DataClassToFile(cMain.mSysSet);
            Application.Exit();
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolBtnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}