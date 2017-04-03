using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NewMideaProgram
{
    public partial class frmTest : Form
    {
        Button[] BtnOn = new Button[cMain.DataPlcMPoint];
        Button[] BtnOff = new Button[cMain.DataPlcMPoint];
        BlinkLed[] Led = new BlinkLed[cMain.DataPlcMPoint];
        Label[] Title = new Label[cMain.DataAll];
        Label[] Value = new Label[cMain.DataAll];
        public frmTest()
        {
            InitializeComponent();
        }
        public void StartLoad()
        {
            for (int i = 0; i < cMain.DataPlcMPoint; i++)
            {
                Led[i] = (BlinkLed)panMdi.Controls.Find(string.Format("blinkLed{0}", i + 1), true)[0];
                BtnOn[i] = (Button)panMdi.Controls.Find(string.Format("button{0}", i * 2 + 1), true)[0];
                BtnOff[i] = (Button)panMdi.Controls.Find(string.Format("button{0}", i * 2 + 2), true)[0];


                BtnOn[i].Text = string.Format("{0}->开", cMain.DataPlcMPointStr[i]);
                BtnOff[i].Text = string.Format("{0}->关", cMain.DataPlcMPointStr[i]);


                BtnOn[i].Click += frmTest_Click_On;
                BtnOff[i].Click += frmTest_Click_Off;

                BtnOn[i].Tag = i;
                BtnOff[i].Tag = i;

                Led[i].Color = Color.DarkGray;
            }
            for (int i = 0; i < cMain.DataAll; i++)
            {
                Title[i] = (Label)panMdi.Controls.Find(string.Format("label{0}", i * 2 + 2), true)[0];
                Value[i] = (Label)panMdi.Controls.Find(string.Format("label{0}", i * 2 + 1), true)[0];
                Title[i].Text = cMain.DataAllTitleStr[cMain.IndexLanguage].Split(',')[i];
                Title[i].BackColor = Color.Silver;
            }
        }

        private void frmTest_Click_Off(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            frmMain.Plc_Out_MPoint[cMain.DataPlcMPointStr[(int)btn.Tag]] = false;
            frmMain.Temp_Out_MPoint[cMain.DataPlcMPointStr[(int)btn.Tag]] = true;
        }

        private void frmTest_Click_On(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            frmMain.Plc_Out_MPoint[cMain.DataPlcMPointStr[(int)btn.Tag]] = true;
            frmMain.Temp_Out_MPoint[cMain.DataPlcMPointStr[(int)btn.Tag]] = true;
        }

        private void panMdi_Paint(object sender, PaintEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < cMain.DataPlcMPoint; i++)
            {
                if (frmMain.ReadPlcMValue[i])
                {
                    Led[i].Color = Color.Green;
                }
                else
                {
                    Led[i].Color = Color.DarkGray;
                }
            }
            for (int i = 0; i < cMain.DataAll; i++)
            {
                Value[i].Text = string.Format("{0:F3}", frmMain.dataRead[i]);
            }
        }

        private void frmTest_Load(object sender, EventArgs e)
        {
            StartLoad();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
