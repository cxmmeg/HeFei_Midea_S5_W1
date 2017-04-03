using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewMideaProgram
{
    public partial class frmEmpty : All.Window.BaseForm
    {
        All.Class.LockMain<string>.GetStatueList nowStatue = All.Class.LockMain<string>.GetStatueList.收到请求;
        public frmEmpty()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            frmMain.lockClientEmpty.Delete();
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            frmMain.lockClientEmpty.Delete();
            this.DialogResult = DialogResult.Yes;
            this.Close();
        }
        private void frmEmpty_Load(object sender, EventArgs e)
        {
            frmMain.lockClientEmpty.GetRemotHostInfo += lockClient_GetRemotHostInfo;
            frmMain.lockClientEmpty.GetRemotHostTestValue += lockClient_GetRemotHostTestValue;
            frmMain.lockClientEmpty.SwitchOpen += lockClient_SwitchOpen;
            frmMain.lockClientEmpty.SwitchClose += lockClient_SwitchClose;
            frmMain.lockClientEmpty.GetRemotHostTestingValue += lockClientEmpty_GetRemotHostTestingValue;
            timFlush.Enabled = true;

            lblEmpty.Text = "";
            lblTime.Text = "0";
            frmMain.lockClientEmpty.Please();
        }

        void lockClientEmpty_GetRemotHostTestingValue(string value)
        {
            lblEmpty.Text = value;
        }
        private void frmEmpty_FormClosing(object sender, FormClosingEventArgs e)
        {
            timFlush.Enabled = false;
            timFlush.Stop();
            frmMain.lockClientEmpty.SwitchOpen -= lockClient_SwitchOpen;
            frmMain.lockClientEmpty.GetRemotHostTestValue -= lockClient_GetRemotHostTestValue;
            frmMain.lockClientEmpty.GetRemotHostInfo -= lockClient_GetRemotHostInfo;
            frmMain.lockClientEmpty.SwitchClose -= lockClient_SwitchClose;
            frmMain.lockClientEmpty.GetRemotHostTestingValue -= lockClientEmpty_GetRemotHostTestingValue;


            frmMain.Plc_Out_MPoint["真空阀"] = false;
            frmMain.Temp_Out_MPoint["真空阀"] = true;
        }
        void lockClient_SwitchClose()
        {
            frmMain.Plc_Out_MPoint["真空阀"] = false;
            frmMain.Temp_Out_MPoint["真空阀"] = true;
            isSwitchClose = true;
        }
        delegate void ListAddItemHandle(string value);
        private void ListAddItem(string value)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ListAddItemHandle(ListAddItem), value);
            }
            else
            {
                lstShow.Items.Add(value);
                lstShow.Items.Add("");
                lstShow.SelectedIndex = lstShow.Items.Count - 1;
            }
        }
        private void lockClient_SwitchOpen()
        {
            if (frmMain.Plc_Empty[0])
            {
                ListAddItem("当前正在有其他工位测试安检，本工位不能按允许的情况进行正常检测");
            }
            else
            {
                frmMain.Plc_Out_MPoint["真空阀"] = true;
                frmMain.Temp_Out_MPoint["真空阀"] = true;
                isSwitchOpen = true;
            }
        }

        void lockClient_GetRemotHostTestValue(string value)
        {
            lblEmpty.Text = value;
        }

        void lockClient_GetRemotHostInfo(All.Class.LockMain<string>.GetStatueList statue, string value)
        {
            if (nowStatue != statue && statue != All.Class.LockMain<string>.GetStatueList.收到请求)
            {
                delayTime = 0;
            }
            switch (statue)
            {
                case All.Class.LockMain<string>.GetStatueList.收到请求:
                    break;
                case All.Class.LockMain<string>.GetStatueList.等待:
                    nowStatue = statue;
                    break;
                case All.Class.LockMain<string>.GetStatueList.允许执行互锁要求:
                    nowStatue = statue;
                    break;
                case All.Class.LockMain<string>.GetStatueList.正在执行互锁动作:
                    nowStatue = statue;
                    break;
                case All.Class.LockMain<string>.GetStatueList.互锁动作执行完毕:
                    nowStatue = statue;
                    break;
                case All.Class.LockMain<string>.GetStatueList.互锁请求正常结束:
                    nowStatue = statue;
                    break;
                case All.Class.LockMain<string>.GetStatueList.删除请求成功:
                    nowStatue = statue;
                    break;
            }
        }

        bool isSwitchOpen = false;
        bool isSwitchClose = false;
        int delayTime = 0;
        int allDelayTime = 0;

        int openOverDelay = 0;
        private void timFlush_Tick(object sender, EventArgs e)
        {
            lblTime.Text = string.Format("{0}", allDelayTime++);
            switch (nowStatue)
            {
                case All.Class.LockMain<string>.GetStatueList.等待:
                    ListAddItem(string.Format("正在其他工位抽空结束,等待时间  {0}秒", delayTime++));
                    break;
                case All.Class.LockMain<string>.GetStatueList.允许执行互锁要求:
                    if (isSwitchOpen)
                    {
                        if (openOverDelay == 0)
                        {
                            openOverDelay = Environment.TickCount;
                        }
                        else
                        {

                            if ((Environment.TickCount - openOverDelay) >= 3000)
                            {
                                openOverDelay = 0;
                                isSwitchOpen = false;
                                frmMain.lockClientEmpty.Start();
                            }
                            else
                            {
                                ListAddItem("正在检测阀体是否打开");
                            }
                        }
                    }
                    break;
                case All.Class.LockMain<string>.GetStatueList.正在执行互锁动作:
                    ListAddItem(string.Format("正在进行抽空,请等待抽空结束,等待时间  {0}秒", delayTime++));
                    break;
                case All.Class.LockMain<string>.GetStatueList.互锁动作执行完毕:
                    if (isSwitchClose)
                    {
                        isSwitchClose = false;
                        frmMain.lockClientEmpty.Stop();
                    }
                    break;
                case All.Class.LockMain<string>.GetStatueList.互锁请求正常结束:
                    ListAddItem(string.Format("抽空完毕,程序即将开始测试,请按OK继续"));
                    if (frmMain.Plc_OK)
                    {
                        frmMain.Plc_OK = false;
                        this.DialogResult = DialogResult.Yes;
                        this.Close();
                    }
                    break;
                case All.Class.LockMain<string>.GetStatueList.删除请求成功:
                    break;
            }
        }


    }
}
