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
    public partial class frmAnGui : All.Window.BaseForm
    {
        /// <summary>
        /// 条码
        /// </summary>
        public string Barcode
        { get; set; }
        All.Class.LockMain<string>.GetStatueList nowStatue = All.Class.LockMain<string>.GetStatueList.收到请求;
        AnGuiData anguiResult = new AnGuiData();
        public frmAnGui()
        {
            InitializeComponent();
        }

        private void frmAnGui_Load(object sender, EventArgs e)
        {

            frmMain.lockClientAnGui.GetRemotHostInfo += lockClient_GetRemotHostInfo;
            frmMain.lockClientAnGui.GetRemotHostTestValue += lockClient_GetRemotHostTestValue;
            frmMain.lockClientAnGui.SwitchOpen += lockClient_SwitchOpen;
            frmMain.lockClientAnGui.SwitchClose += lockClient_SwitchClose;
            timFlush.Enabled = true;
            btnAgain_Click(btnAgain, new EventArgs());
        }
        private void frmAnGui_FormClosing(object sender, FormClosingEventArgs e)
        {

            timFlush.Enabled = false;
            timFlush.Stop();
            frmMain.lockClientAnGui.SwitchOpen -= lockClient_SwitchOpen;
            frmMain.lockClientAnGui.GetRemotHostTestValue -= lockClient_GetRemotHostTestValue;
            frmMain.lockClientAnGui.GetRemotHostInfo -= lockClient_GetRemotHostInfo;
            frmMain.lockClientAnGui.SwitchClose -= lockClient_SwitchClose;

            frmMain.Plc_Out_MPoint["安检"] = false;
            frmMain.Temp_Out_MPoint["安检"] = true;
        }

        delegate void SetBtnEnableHandle(Button btn,bool value);
        private void SetBtnEnable(Button btn,bool value)
        {
            if (btn.InvokeRequired)
            {
                this.Invoke(new SetBtnEnableHandle(SetBtnEnable), btn, value);
            }
            else
            {
                btn.Enabled = value;
            }
        }
        delegate void SetLabelTextHandle(Label lbl, string value);
        private void SetLabelText(Label lbl, string value)
        {
            if (lbl.InvokeRequired)
            {
                lbl.Invoke(new SetLabelTextHandle(SetLabelText), lbl, value);
            }
            else
            {
                lbl.Text = value;
            }
        }
        delegate void SetLedColorHandle(BlinkLed led, Color color);
        private void SetLedColor(BlinkLed led, Color color)
        {
            if (led.InvokeRequired)
            {
                led.Invoke(new SetLedColorHandle(SetLedColor), led, color);
            }
            else
            {
                led.Color = color;
            }
        }
        void lockClient_SwitchClose()
        {
            SetBtnEnable(btnAgain, true);

            frmMain.Plc_Out_MPoint["安检"] = false;
            frmMain.Temp_Out_MPoint["安检"] = true;
            isSwitchClose = true;
        }
        private void lockClient_SwitchOpen()
        {
            if (frmMain.Plc_AnGui[1]
                || frmMain.Plc_AnGui[2]
                || frmMain.Plc_AnGui[3])
            {
                ListAddItem("当前正在有其他工位测试安检，本工位不能按允许的情况进行正常检测");
            }
            else
            {
                frmMain.Plc_Out_MPoint["安检"] = true;
                frmMain.Temp_Out_MPoint["安检"] = true;
                isSwitchOpen = true;
            }
        }

        void lockClient_GetRemotHostTestValue(string value)
        {
            anguiResult = anguiResult.GetValue(value);
            SetLabelText(lblJieDi, anguiResult.TestData[0].ToString("F3"));
            SetLabelText(lblJueYuan, anguiResult.TestData[1].ToString("F1"));
            SetLabelText(lblNaiYa, anguiResult.TestData[2].ToString("F3"));
            SetLabelText(lblXieLou, anguiResult.TestData[3].ToString("F3"));

            SetLedColor(ledJieDi, anguiResult.TestResult[0] ? Color.Green : Color.Red);
            SetLedColor(ledJueYuan, anguiResult.TestResult[1] ? Color.Green : Color.Red);
            SetLedColor(ledNaiYa, anguiResult.TestResult[2] ? Color.Green : Color.Red);
            SetLedColor(ledXieLou, anguiResult.TestResult[3] ? Color.Green : Color.Red);
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
        bool isSwitchOpen = false;
        bool isSwitchClose = false;
        int delayTime = 0;
        private void timFlush_Tick(object sender, EventArgs e)
        {
            switch (nowStatue)
            {
                case All.Class.LockMain<string>.GetStatueList.等待:
                    ListAddItem(string.Format("正在其他工位安检测试结束,等待时间{0}", delayTime++));
                    break;
                case All.Class.LockMain<string>.GetStatueList.允许执行互锁要求:
                    if (isSwitchOpen)
                    {
                        if (frmMain.Plc_AnGui[0])
                        {
                            if (frmMain.Plc_AnGui[1] || frmMain.Plc_AnGui[2] || frmMain.Plc_AnGui[3])
                            {
                                ListAddItem("出现故障，应该是本工位进行安检测试，但其他工位有安检动作");
                            }
                            else
                            {
                                isSwitchOpen = false;
                                frmMain.lockClientAnGui.Start();
                            }
                        }
                        else
                        {
                            ListAddItem(string.Format("正在等待PLC切换安检电路,等待时间{0}", delayTime++));
                        }
                    }
                    break;
                case All.Class.LockMain<string>.GetStatueList.正在执行互锁动作:
                    ListAddItem(string.Format("正在进行安检测试,请等待测试结束,等待时间{0}", delayTime++));
                    break;
                case All.Class.LockMain<string>.GetStatueList.互锁动作执行完毕:
                    if (isSwitchClose)
                    {
                        if (frmMain.Plc_Test)
                        {
                            isSwitchClose = false;
                            frmMain.lockClientAnGui.Stop();
                        }
                        else
                        {
                            ListAddItem(string.Format("安检测试完毕,正在等待PLC切换安检电路,等待时间{0}", delayTime++));
                        }
                    }
                    break;
                case All.Class.LockMain<string>.GetStatueList.互锁请求正常结束:
                    if (anguiResult.isPass && (anguiResult.TestData[0] != 0 || anguiResult.TestData[1] != 0 || anguiResult.TestData[2] != 0))
                    {
                        ListAddItem(string.Format("安检测试完毕,请按OK进入程序测试"));
                        if (frmMain.Plc_OK)
                        {
                            cData.SaveAnGuiData(Barcode, anguiResult.TestData, anguiResult.TestResult);
                            frmMain.Plc_OK = false;
                            this.DialogResult = DialogResult.Yes;
                            this.Close();
                        }
                    }
                    else
                    {
                        ListAddItem("安检数据出现异常,请选择【重测】,【跳过】或【取消】");
                    }
                    break;
                case All.Class.LockMain<string>.GetStatueList.删除请求成功:
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            frmMain.lockClientAnGui.Delete();
            this.DialogResult = DialogResult.No;
            this.Close();
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            frmMain.lockClientAnGui.Delete();
            DialogResult = DialogResult.Yes;
            this.Close();
        }

        private void btnAgain_Click(object sender, EventArgs e)
        {
            btnAgain.Enabled = false;
            lblJieDi.Text = "";
            lblJueYuan.Text = "";
            lblNaiYa.Text = "";
            lblXieLou.Text = "";
            ledJieDi.Color = Color.Yellow;
            ledJueYuan.Color = Color.Yellow;
            ledNaiYa.Color = Color.Yellow;
            ledXieLou.Color = Color.Yellow;
            frmMain.lockClientAnGui.Please();
        }
    }
}
