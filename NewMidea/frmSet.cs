using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
namespace NewMideaProgram
{
    public partial class frmSet : Form
    {
        cTmpStep[] TmpStepStr = new cTmpStep[10];
        CheckBox[] chkShow = new CheckBox[cMain.DataShow];
        TextBox[] txtLow = new TextBox[cMain.DataShow];
        TextBox[] txtHigh = new TextBox[cMain.DataShow];
        CheckBox[] chkXinHao = new CheckBox[6];
        CheckBox[] chkSwitch = new CheckBox[6];
        bool changeValue = true;
        public frmSet()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
            this.UpdateStyles();
        }
        private void frmSet_Load(object sender, EventArgs e)
        {
            frmInit();
            DataClassToFrm(cMain.mModeSet);
        }
        private void frmInit()
        {
            for (int i = 0; i < cMain.DataShow; i++)
            {
                chkShow[i] = (CheckBox)this.Controls.Find(string.Format("chkS{0}", i + 1), true)[0];
                chkShow[i].Tag = i;
                txtLow[i] = (TextBox)this.Controls.Find(string.Format("txtL{0}", i + 1), true)[0];
                txtHigh[i] = (TextBox)this.Controls.Find(string.Format("txtH{0}", i + 1), true)[0];
            }
            for (int i = 0; i < chkXinHao.Length; i++)
            {
                chkXinHao[i] = (CheckBox)this.Controls.Find(string.Format("chkXinHao{0}", i + 1), true)[0];
            }
            for (int i = 0; i < chkSwitch.Length; i++)
            {
                chkSwitch[i] = (CheckBox)this.Controls.Find(string.Format("chkSwitch{0}", i + 1), true)[0];
            }
            cbbMachine.Items.Clear();
            cbbMachine.DataSource = cMain.JiQiStr[cMain.IndexLanguage].Split(',');
            cbbStep.Items.Clear();
            cbbStep.DataSource = cMain.BuZhouMingStr[cMain.IndexLanguage].Split(',');
            for (int i = 0; i < TmpStepStr.Length; i++)
            {
                TmpStepStr[i] = new cTmpStep();
            }
            for (int i = 0; i < cMain.DataShow; i++)
            {
                txtLow[i].TextChanged += frmSet_TextChanged;
                txtHigh[i].TextChanged += frmSet_TextChanged;
                chkShow[i].CheckedChanged += frmSet_ChkShowChange;
            }
            cbbStep.SelectedIndexChanged+=cbbStep_SelectedIndexChanged;
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            frmBarSet fb = new frmBarSet();
            fb.ShowDialog();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            frmList fl = new frmList();
            if (fl.ShowDialog() == DialogResult.Yes  )
            {
                if (!fl.isError)
                {
                    cModeSet ModeSet = new cModeSet();
                    if (ModeSet.Load(fl.ReturnId.Trim()))
                    {
                        DataClassToFrm(ModeSet);
                    }
                }
            }
            fl.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            cModeSet tmpMode = new cModeSet();

            if (DataFrmToClass(out tmpMode))
            {
                if (tmpMode.Save())
                {
                    MessageBox.Show("当前机型设置数据已成功保存.", "保存成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            MessageBox.Show("数据保存错误,请检测数据是否正确后重新保存", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// 将界面上的值写入到设定类
        /// </summary>
        /// <param name="ModeSet">界面上的值写入到类,返回设置</param>
        /// <returns>bool,返回是否转换成功</returns>
        public bool DataFrmToClass(out cModeSet ModeSet)
        {
            bool isOk = false;
            cModeSet tmpModeSet = new cModeSet();
            try
            {
                tmpModeSet.ID = cbbId.Text;
                tmpModeSet.Mode = txtMode.Text;
                tmpModeSet.Info = txtInfo.Text;
                tmpModeSet.JiQi = cbbMachine.SelectedIndex;
                tmpModeSet.LowSpeed = rbtDiFeng.Checked;
                tmpModeSet.BiaoZhunJi[0] = chkDoor1.Checked;
                tmpModeSet.BiaoZhunJi[1] = chkDoor2.Checked;
                tmpModeSet.BiaoZhunJi[2] = chkDoor3.Checked;
                for (int i = 0; i < tmpModeSet.KaiGuan.Length; i++)
                {
                    tmpModeSet.KaiGuan[i] = chkSwitch[i].Checked;
                }
                tmpModeSet.Protect[0] = All.Class.Num.ToFloat(txtHiCur.Text);
                tmpModeSet.Protect[1] = All.Class.Num.ToFloat(txtHiPress.Text);
                for (int i = 0; i < tmpModeSet.XinHao.Length; i++)
                {
                    tmpModeSet.XinHao[i] = chkXinHao[i].Checked;
                }
                tmpModeSet.Vol110V = rbt110V.Checked;
                for (int i = 0; i < tmpModeSet.DataShow.Length; i++)
                {
                    tmpModeSet.DataShow[i] = chkShow[i].Checked;
                }
                for (int i = 0; i < 10; i++)
                {
                    tmpModeSet.Step[i].Text = TmpStepStr[i].Text;
                    tmpModeSet.Step[i].TestTime = All.Class.Num.ToInt(TmpStepStr[i].TestTime);
                    tmpModeSet.Step[i].NengJi = All.Class.Num.ToInt(TmpStepStr[i].NengJi);
                    tmpModeSet.Step[i].PinLv = All.Class.Num.ToInt(TmpStepStr[i].PinLv);
                    tmpModeSet.Step[i].SnCode = TmpStepStr[i].SnCode;
                    tmpModeSet.Step[i].StartCheck = All.Class.Num.ToBool(TmpStepStr[i].StartCheck);
                    tmpModeSet.Step[i].EndCheck = All.Class.Num.ToBool(TmpStepStr[i].EndCheck);
                    tmpModeSet.Step[i].StartInfo = TmpStepStr[i].StartInfo;
                    tmpModeSet.Step[i].EndInfo = TmpStepStr[i].EndInfo;
                    for (int j = 0; j < cMain.DataShow; j++)
                    {
                        tmpModeSet.Step[i].LowData[j] = All.Class.Num.ToFloat(TmpStepStr[i].LowData[j]);
                        tmpModeSet.Step[i].HighData[j] = All.Class.Num.ToFloat(TmpStepStr[i].HighData[j]);
                    }
                }
                isOk = true;
            }
            catch(Exception e)
            {
                All.Class.Error.Add(e);
                isOk = false;
            }
            ModeSet = tmpModeSet;
            return isOk;
        }
        private bool DataClassToFrm(cModeSet mModeSet)
        {
            changeValue = false;
            bool isOk = false;
            cbbId.Text = mModeSet.ID;
            cbbMachine.SelectedIndex = mModeSet.JiQi;
            txtMode.Text = mModeSet.Mode;
            txtInfo.Text = mModeSet.Info;
            chkDoor1.Checked = mModeSet.BiaoZhunJi[0];
            chkDoor2.Checked = mModeSet.BiaoZhunJi[1];
            chkDoor3.Checked = mModeSet.BiaoZhunJi[2];
            rbtDiFeng.Checked = mModeSet.LowSpeed;
            rbtGaoFeng.Checked = !mModeSet.LowSpeed;
            for (int i = 0; i < mModeSet.KaiGuan.Length; i++)
            {
                chkSwitch[i].Checked = mModeSet.KaiGuan[i];
            }
            txtHiCur.Text = mModeSet.Protect[0].ToString("F2");
            txtHiPress.Text = mModeSet.Protect[1].ToString("F3");
            for (int i = 0; i < mModeSet.XinHao.Length; i++)
            {
                chkXinHao[i].Checked = mModeSet.XinHao[i];
            }
            rbt110V.Checked = mModeSet.Vol110V;
            rbt220V.Checked = !mModeSet.Vol110V;
            for (int i = 0; i < mModeSet.DataShow.Length; i++)
            {
                chkShow[i].Checked = mModeSet.DataShow[i];
                txtLow[i].Enabled = mModeSet.DataShow[i];
                txtHigh[i].Enabled = mModeSet.DataShow[i];
            }
            for (int i = 0; i < mModeSet.Step.Length; i++)
            {
                if (mModeSet.Step[i].Text != "")
                {
                    tabControl1.TabPages[i + 1].Text = string.Format("{0}:{1}", i + 1, mModeSet.Step[i].Text);
                }
                else
                {
                    tabControl1.TabPages[i + 1].Text = string.Format("{0}:空", i + 1);
                }
            }
            for (int i = 0; i < 10; i++)
            {
                TmpStepStr[i].Text = mModeSet.Step[i].Text;
                TmpStepStr[i].TestTime = mModeSet.Step[i].TestTime.ToString();
                TmpStepStr[i].NengJi = mModeSet.Step[i].NengJi.ToString();
                TmpStepStr[i].PinLv = mModeSet.Step[i].PinLv.ToString();
                TmpStepStr[i].SnCode = mModeSet.Step[i].SnCode;
                TmpStepStr[i].StartCheck = mModeSet.Step[i].StartCheck.ToString();
                TmpStepStr[i].EndCheck = mModeSet.Step[i].EndCheck.ToString();
                TmpStepStr[i].StartInfo = mModeSet.Step[i].StartInfo;
                TmpStepStr[i].EndInfo = mModeSet.Step[i].EndInfo;
                for (int j = 0; j < cMain.DataShow; j++)
                {
                    TmpStepStr[i].LowData[j] = mModeSet.Step[i].LowData[j].ToString();
                    TmpStepStr[i].HighData[j] = mModeSet.Step[i].HighData[j].ToString();
                }
            }
            DataClassToStepFrm();
            changeValue = true;
            return isOk;
        }
        private void DataClassToStepFrm()
        {
            if (tabControl1.SelectedIndex > 0)
            {
                changeValue = false;
                panStep.Parent = tabControl1.SelectedTab;
                int index = tabControl1.SelectedIndex - 1;
                cbbStep.Text = TmpStepStr[index].Text;
                txtTime.Text = TmpStepStr[index].TestTime;
                txtNengJi.Text = TmpStepStr[index].NengJi;
                txtPinLv.Text = TmpStepStr[index].PinLv;
                txtSnCode.Text = TmpStepStr[index].SnCode;
                chkStartCheck.Checked = All.Class.Num.ToBool(TmpStepStr[index].StartCheck);
                chkEndCheck.Checked = All.Class.Num.ToBool(TmpStepStr[index].EndCheck);
                txtStart.Text = TmpStepStr[index].StartInfo;
                txtEnd.Text = TmpStepStr[index].EndInfo;
                for (int i = 0; i < TmpStepStr[index].LowData.Length; i++)
                {
                    txtLow[i].Text = TmpStepStr[index].LowData[i];
                }
                for (int i = 0; i < TmpStepStr[index].HighData.Length; i++)
                {
                    txtHigh[i].Text = TmpStepStr[index].HighData[i];
                }
                changeValue = true;
            }
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            cModeSet modeSet = new cModeSet();
            modeSet.Load(cbbId.Text);
            frmSend f = new frmSend(modeSet);
            f.ShowDialog();
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
        public class cTmpStep
        {
            public string Text
            { get; set; }
            public string TestTime
            {get;set;}
            public string NengJi
            { get; set; }
            public string PinLv
            { get; set; }
            public string SnCode
            { get; set; }
            public string StartCheck
            { get; set; }
            public string EndCheck
            { get; set; }
            public string StartInfo
            { get; set; }
            public string EndInfo
            { get; set; }
            public string[] LowData
            { get; set; }
            public string[] HighData
            { get; set; }
            public cTmpStep()
            {
                Text = "";
                TestTime = "";
                NengJi = "";
                PinLv = "";
                SnCode = "";
                StartCheck = string.Format("{0}", false);
                EndCheck = string.Format("{0}", false);
                StartInfo = "";
                EndInfo = "";
                LowData = new string[cMain.DataShow];
                HighData = new string[cMain.DataShow];
                for (int i = 0; i < cMain.DataShow; i++)
                {
                    LowData[i] = "";
                    HighData[i] = "";
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataClassToStepFrm();
        }

        private void cbbStep_SelectedIndexChanged(object sender, EventArgs e)
        {
            ChangeTmpValue();
        }

        private void txtSnCode_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (txt.Text == "" || All.Class.Check.isFix(txt.Text, All.Class.Check.RegularList.十六进制字符))
            {
                txt.ForeColor = Color.Black;
                ChangeTmpValue();
            }
            else
            {
                txt.ForeColor = Color.Red;
                MessageBox.Show("请输入正确格式的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt.Focus();
            }
        }
        private void txtNengJi_TextChanged(object sender, EventArgs e)
        {
            ChangeTmpValue();
        }
        private void txtTime_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (txt.Text == "" || All.Class.Check.isFix(txt.Text, All.Class.Check.RegularList.非负整数))
            {
                txt.ForeColor = Color.Black;
                ChangeTmpValue();
            }
            else
            {
                txt.ForeColor = Color.Red;
                MessageBox.Show("请输入正确格式的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt.Focus();
            }
        }
        private void frmSet_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = (TextBox)sender;
            if (txt.Text == "" || All.Class.Check.isFix(txt.Text, All.Class.Check.RegularList.输入中的浮点数 ))
            {
                txt.ForeColor = Color.Black;
                ChangeTmpValue();
            }
            else
            {
                txt.ForeColor = Color.Red;
                MessageBox.Show("请输入正确格式的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txt.Focus();
            }
        }
        private void ChangeTmpValue()
        {
            if (changeValue)
            {
                int index = tabControl1.SelectedIndex - 1;
                if (index < 0 || index >= 10)
                {
                    All.Class.Error.Add(string.Format("当前打开页面为{0},不应该有数据更新,须要检查", index), Environment.StackTrace);
                    return;
                }
                tabControl1.TabPages[index + 1].Text = string.Format("{0}:{1}", index + 1, cbbStep.Text);

                TmpStepStr[index].Text = cbbStep.Text;
                TmpStepStr[index].TestTime = txtTime.Text;
                TmpStepStr[index].NengJi =txtNengJi.Text;
                TmpStepStr[index].PinLv = txtPinLv.Text;
                TmpStepStr[index].SnCode = txtSnCode.Text;
                TmpStepStr[index].StartCheck = chkStartCheck.Checked.ToString();
                TmpStepStr[index].EndCheck = chkEndCheck.Checked.ToString();
                TmpStepStr[index].StartInfo = txtStart.Text;
                TmpStepStr[index].EndInfo = txtEnd.Text;
                for (int i = 0; i < cMain.DataShow; i++)
                {
                    TmpStepStr[index].LowData[i] = txtLow[i].Text;
                    TmpStepStr[index].HighData[i] = txtHigh[i].Text;
                }
            }
        }
        private void txtHiCur_TextChanged(object sender, EventArgs e)
        {
            if (changeValue)
            {
                TextBox txt = (TextBox)sender;
                if (txt.Text == "" || All.Class.Check.isFix(txt.Text, All.Class.Check.RegularList.输入中的浮点数))
                {
                    txt.ForeColor = Color.Black;
                }
                else
                {
                    txt.ForeColor = Color.Red;
                    MessageBox.Show("请输入正确格式的数据", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt.Focus();
                }
            }
        }
        private void frmSet_ChkShowChange(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            int index = (int)chk.Tag;
            txtLow[index].Enabled = chk.Checked;
            txtHigh[index].Enabled = chk.Checked;
        }

        private void chkStartCheck_CheckedChanged(object sender, EventArgs e)
        {
            ChangeTmpValue();
        }

        private void chkEndCheck_CheckedChanged(object sender, EventArgs e)
        {
            ChangeTmpValue();
        }

        private void frmSet_FormClosing(object sender, FormClosingEventArgs e)
        {
            for (int i = 0; i < cMain.DataShow; i++)
            {
                txtLow[i].TextChanged -= frmSet_TextChanged;
                txtHigh[i].TextChanged -= frmSet_TextChanged;
                chkShow[i].CheckedChanged -= frmSet_ChkShowChange;
            }
            cbbStep.SelectedIndexChanged -= cbbStep_SelectedIndexChanged;
        }
    }
}