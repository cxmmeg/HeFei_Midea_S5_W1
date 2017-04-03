using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
namespace NewMideaProgram
{
    public delegate void SendText(int startId, int endId);

    public partial class frmSend : Form
    {
        cModeSet modeSet;
        public frmSend(cModeSet mModeSet)
        {
            modeSet = mModeSet;
            InitializeComponent();
        }
        bool is_B_Ok = false;
        bool is_TimeOut = false;
        long timeOut = 1500;
        cUdpSock mUdp;
        tempClass tc;
        Thread SendThread;
        private void frmSend_Load(object sender, EventArgs e)
        {
            int i;
            cbbStart.Items.Clear();
            cbbEnd.Items.Clear();
            for (i = 1; i <= cMain.AllCount; i++)
            {
                cbbStart.Items.Add(i);
                cbbEnd.Items.Add(i);
            }
            cbbStart.Text = "1";
            cbbEnd.Text = cMain.AllCount.ToString();

            frmInit();

        }
        private void frmInit()
        {
            if (!cMain.isComPuter)
            {
                this.Height = Screen.PrimaryScreen.Bounds.Height;
                this.Width = Screen.PrimaryScreen.Bounds.Width;
            }
            cMain.initFrom(this.Controls);
            btnStop.Enabled = false;
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (mUdp != null)
            {
                mUdp.fUdpClose();
                mUdp = null;
            }
            if (SendThread != null)
            {
                SendThread.Abort();
                SendThread = null;
            }
            tc = null;
            this.Close();
            this.Dispose();

        }
        delegate void SetBtnEnableHandle(ToolStripButton sender, bool value);
        private void SetBtnEnable(ToolStripButton sender, bool value)
        {
            if (toolStrip1.InvokeRequired)
            {
                toolStrip1.Invoke(new SetBtnEnableHandle(SetBtnEnable),sender, value);
            }
            else
            {
                sender.Enabled = value;
            }
        }
        private void listAdditem(object sender, EventArgs e)
        {
            string itemStr = sender.ToString();
            if (itemStr == "Clear")
            {
                listBox1.Items.Clear();
            }
            else
            {
                listBox1.Items.Add(itemStr);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }
        private void SendModeSet(int startId, int endId)
        {
            SetBtnEnable(btnSend,false);
            int i;// startId, endId;
            string remotHost = "  ";
            string[] tempStr;
            
            mUdp = new cUdpSock(10000);
            mUdp.mDataReciveString += new cUdpSock.mDataReciveStringEventHandle(mUdp_mDataReciveString);
            string ModeStr = "";
            ModeStr = DataClassToStr(modeSet);
            tempStr = cMain.RemoteHostName.Split('.');
            if (tempStr.Length == 4)
            {
                remotHost = tempStr[0] + "." + tempStr[1] + "." + tempStr[2] + ".";
            }
            long startTime = 0;
            listBox1.BeginInvoke(new EventHandler(listAdditem), "Clear");
            for (i = startId; i <= endId; i++)
            {
                for (int k = 0; k < 3; k++)
                {
                    listBox1.BeginInvoke(new EventHandler(listAdditem), "     " + string.Format("{0}{1}#", "", i) + "开始发送数据....");

                    is_B_Ok = false;
                    mUdp.fUdpSend(string.Format("{0}{1}", remotHost, i + 160), 3000, ModeStr);
                    startTime = Environment.TickCount;
                    is_TimeOut = false;
                    do
                    {
                        Thread.Sleep(20);
                        if (Environment.TickCount - startTime > timeOut)
                        {
                            is_TimeOut = true;
                        }
                    } while (!is_B_Ok && !is_TimeOut);
                    if (is_B_Ok)
                    {
                        k = 3;
                        listBox1.BeginInvoke(new EventHandler(listAdditem), "               " + i.ToString() + "#检测数据发送成功....");
                    }
                    else
                    {
                        listBox1.BeginInvoke(new EventHandler(listAdditem), "     " +  i.ToString() + "#检测数据发送失败..................");
                    }
                }
                
            }
            SetBtnEnable(btnSend, true);
            SetBtnEnable(btnStop, false);

            if (mUdp != null)
            {
                mUdp.fUdpClose();
                mUdp = null;
            }
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            btnSend.Enabled = false;
            tc = new tempClass(Convert.ToInt16(cbbStart.Text), Convert.ToInt16(cbbEnd.Text));
            tc.st = new SendText(SendModeSet);
            SendThread = new Thread(new ThreadStart(tc.send));
            SendThread.IsBackground = true;
            SendThread.Start();
            btnStop.Enabled = true;
        }
        private void mUdp_mDataReciveString(object o, RecieveStringArgs e)
        {
            if (e.ReadStr == "B~OK")
            {
                is_B_Ok = true;
            }
        }
        private string DataClassToStr(cModeSet modeSet)
        {
            string SendStr = "B~~0~";
            SendStr = SendStr + modeSet.ToNetStr();
            return SendStr;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            btnStop.Enabled = false;
            if (mUdp != null)
            {
                mUdp.fUdpClose();
                mUdp = null;
            }
            if (SendThread != null)
            {
                SendThread.Abort();
                SendThread = null;
            }
            tc = null;
            btnSend.Enabled = true;
        }
    }
    public class tempClass
    {
        public SendText st;
        int _StartId, _EndId;
        public tempClass(int startId, int endId)
        {
            _EndId = endId;
            _StartId = startId;
        }
        public void send()
        {
            st(_StartId, _EndId);
        }
    }
}