namespace NewMideaProgram
{
    partial class frmAnGui
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAnGui));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblJieDi = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblJueYuan = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblNaiYa = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblXieLou = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnAgain = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.lstShow = new System.Windows.Forms.ListBox();
            this.timFlush = new System.Windows.Forms.Timer(this.components);
            this.ledXieLou = new NewMideaProgram.BlinkLed();
            this.ledNaiYa = new NewMideaProgram.BlinkLed();
            this.ledJueYuan = new NewMideaProgram.BlinkLed();
            this.ledJieDi = new NewMideaProgram.BlinkLed();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 11;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.panel4, 10, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 7, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblJieDi, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblJueYuan, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblNaiYa, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this.label7, 9, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblXieLou, 10, 0);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label10, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.label11, 6, 1);
            this.tableLayoutPanel1.Controls.Add(this.label12, 9, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 40);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(717, 80);
            this.tableLayoutPanel1.TabIndex = 16;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.ledXieLou);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(640, 43);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(73, 33);
            this.panel4.TabIndex = 34;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.ledNaiYa);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(465, 43);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(71, 33);
            this.panel3.TabIndex = 33;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ledJueYuan);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(290, 43);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(71, 33);
            this.panel2.TabIndex = 32;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(4, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 38);
            this.label1.TabIndex = 15;
            this.label1.Text = "接地值(mΩ)";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblJieDi
            // 
            this.lblJieDi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblJieDi.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblJieDi.Location = new System.Drawing.Point(105, 1);
            this.lblJieDi.Name = "lblJieDi";
            this.lblJieDi.Size = new System.Drawing.Size(71, 38);
            this.lblJieDi.TabIndex = 16;
            this.lblJieDi.Text = "0";
            this.lblJieDi.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(189, 1);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 38);
            this.label3.TabIndex = 17;
            this.label3.Text = "绝缘值(MΩ)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblJueYuan
            // 
            this.lblJueYuan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblJueYuan.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblJueYuan.Location = new System.Drawing.Point(290, 1);
            this.lblJueYuan.Name = "lblJueYuan";
            this.lblJueYuan.Size = new System.Drawing.Size(71, 38);
            this.lblJueYuan.TabIndex = 18;
            this.lblJueYuan.Text = "0";
            this.lblJueYuan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(374, 1);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 38);
            this.label5.TabIndex = 19;
            this.label5.Text = "耐压值(mA)";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNaiYa
            // 
            this.lblNaiYa.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNaiYa.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblNaiYa.Location = new System.Drawing.Point(465, 1);
            this.lblNaiYa.Name = "lblNaiYa";
            this.lblNaiYa.Size = new System.Drawing.Size(71, 38);
            this.lblNaiYa.TabIndex = 20;
            this.lblNaiYa.Text = "0";
            this.lblNaiYa.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(549, 1);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 38);
            this.label7.TabIndex = 21;
            this.label7.Text = "泄漏值(uA)";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblXieLou
            // 
            this.lblXieLou.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblXieLou.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblXieLou.Location = new System.Drawing.Point(640, 1);
            this.lblXieLou.Name = "lblXieLou";
            this.lblXieLou.Size = new System.Drawing.Size(73, 38);
            this.lblXieLou.TabIndex = 22;
            this.lblXieLou.Text = "0";
            this.lblXieLou.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(4, 40);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(94, 39);
            this.label9.TabIndex = 23;
            this.label9.Text = "接地判定";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(189, 40);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(94, 39);
            this.label10.TabIndex = 24;
            this.label10.Text = "绝缘判定";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(374, 40);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(84, 39);
            this.label11.TabIndex = 25;
            this.label11.Text = "耐压判定";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(549, 40);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(84, 39);
            this.label12.TabIndex = 26;
            this.label12.Text = "泄漏判定";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ledJieDi);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(105, 43);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(71, 33);
            this.panel1.TabIndex = 31;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.btnAgain);
            this.panel5.Controls.Add(this.btnCancel);
            this.panel5.Controls.Add(this.btnOk);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(2, 367);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(717, 39);
            this.panel5.TabIndex = 17;
            // 
            // btnAgain
            // 
            this.btnAgain.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAgain.Image = ((System.Drawing.Image)(resources.GetObject("btnAgain.Image")));
            this.btnAgain.Location = new System.Drawing.Point(433, 5);
            this.btnAgain.Name = "btnAgain";
            this.btnAgain.Size = new System.Drawing.Size(76, 29);
            this.btnAgain.TabIndex = 10;
            this.btnAgain.Text = "重测[&R]";
            this.btnAgain.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAgain.UseVisualStyleBackColor = true;
            this.btnAgain.Click += new System.EventHandler(this.btnAgain_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(619, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(76, 29);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "取消[&C]";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.Location = new System.Drawing.Point(526, 5);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(76, 29);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "跳过[&J]";
            this.btnOk.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // lstShow
            // 
            this.lstShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstShow.FormattingEnabled = true;
            this.lstShow.ItemHeight = 12;
            this.lstShow.Location = new System.Drawing.Point(2, 120);
            this.lstShow.Name = "lstShow";
            this.lstShow.Size = new System.Drawing.Size(717, 247);
            this.lstShow.TabIndex = 19;
            // 
            // timFlush
            // 
            this.timFlush.Interval = 1000;
            this.timFlush.Tick += new System.EventHandler(this.timFlush_Tick);
            // 
            // ledXieLou
            // 
            this.ledXieLou.Color = System.Drawing.Color.Empty;
            this.ledXieLou.InterVal = 100;
            this.ledXieLou.Location = new System.Drawing.Point(17, 0);
            this.ledXieLou.Name = "ledXieLou";
            this.ledXieLou.Size = new System.Drawing.Size(36, 33);
            this.ledXieLou.State = NewMideaProgram.BlinkLed.LedState.On;
            this.ledXieLou.TabIndex = 1;
            // 
            // ledNaiYa
            // 
            this.ledNaiYa.Color = System.Drawing.Color.Empty;
            this.ledNaiYa.InterVal = 100;
            this.ledNaiYa.Location = new System.Drawing.Point(17, 0);
            this.ledNaiYa.Name = "ledNaiYa";
            this.ledNaiYa.Size = new System.Drawing.Size(35, 33);
            this.ledNaiYa.State = NewMideaProgram.BlinkLed.LedState.On;
            this.ledNaiYa.TabIndex = 1;
            // 
            // ledJueYuan
            // 
            this.ledJueYuan.Color = System.Drawing.Color.Empty;
            this.ledJueYuan.InterVal = 100;
            this.ledJueYuan.Location = new System.Drawing.Point(17, 0);
            this.ledJueYuan.Name = "ledJueYuan";
            this.ledJueYuan.Size = new System.Drawing.Size(35, 33);
            this.ledJueYuan.State = NewMideaProgram.BlinkLed.LedState.On;
            this.ledJueYuan.TabIndex = 1;
            // 
            // ledJieDi
            // 
            this.ledJieDi.Color = System.Drawing.Color.Empty;
            this.ledJieDi.InterVal = 100;
            this.ledJieDi.Location = new System.Drawing.Point(16, 0);
            this.ledJieDi.Name = "ledJieDi";
            this.ledJieDi.Size = new System.Drawing.Size(35, 33);
            this.ledJieDi.State = NewMideaProgram.BlinkLed.LedState.On;
            this.ledJieDi.TabIndex = 1;
            // 
            // frmAnGui
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 408);
            this.Controls.Add(this.lstShow);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.tableLayoutPanel1);
            this.IsExitButton = false;
            this.IsMaxButton = false;
            this.IsMinButton = false;
            this.Name = "frmAnGui";
            this.Padding = new System.Windows.Forms.Padding(2, 40, 2, 2);
            this.Text = "安检测试";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAnGui_FormClosing);
            this.Load += new System.EventHandler(this.frmAnGui_Load);
            this.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
            this.Controls.SetChildIndex(this.panel5, 0);
            this.Controls.SetChildIndex(this.lstShow, 0);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel4;
        private NewMideaProgram.BlinkLed ledXieLou;
        private System.Windows.Forms.Panel panel3;
        private NewMideaProgram.BlinkLed ledNaiYa;
        private System.Windows.Forms.Panel panel2;
        private NewMideaProgram.BlinkLed ledJueYuan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblJieDi;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblJueYuan;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblNaiYa;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblXieLou;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panel1;
        private NewMideaProgram.BlinkLed ledJieDi;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnAgain;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.ListBox lstShow;
        private System.Windows.Forms.Timer timFlush;

    }
}