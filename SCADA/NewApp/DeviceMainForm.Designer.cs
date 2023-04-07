namespace SCADA.NewApp
{
    partial class DeviceMainForm
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
            this.tabControlEQ1 = new System.Windows.Forms.TabControl();
            this.tabPagecnc = new System.Windows.Forms.TabPage();
            this.tabPageRobot = new System.Windows.Forms.TabPage();
            this.tabPagePLC = new System.Windows.Forms.TabPage();
            this.tabPagemeasure = new System.Windows.Forms.TabPage();
            this.tabPagerfid = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabControlEQ1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlEQ1
            // 
            this.tabControlEQ1.Controls.Add(this.tabPagecnc);
            this.tabControlEQ1.Controls.Add(this.tabPageRobot);
            this.tabControlEQ1.Controls.Add(this.tabPagePLC);
            this.tabControlEQ1.Controls.Add(this.tabPagemeasure);
            this.tabControlEQ1.Controls.Add(this.tabPagerfid);
            this.tabControlEQ1.Controls.Add(this.tabPage2);
            this.tabControlEQ1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlEQ1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlEQ1.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.tabControlEQ1.ItemSize = new System.Drawing.Size(100, 40);
            this.tabControlEQ1.Location = new System.Drawing.Point(0, 0);
            this.tabControlEQ1.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.tabControlEQ1.Multiline = true;
            this.tabControlEQ1.Name = "tabControlEQ1";
            this.tabControlEQ1.SelectedIndex = 0;
            this.tabControlEQ1.Size = new System.Drawing.Size(1924, 1055);
            this.tabControlEQ1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControlEQ1.TabIndex = 1;
            this.tabControlEQ1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControlEQ1_DrawItem);
            this.tabControlEQ1.SelectedIndexChanged += new System.EventHandler(this.tabControlEQ1_SelectedIndexChanged);
            // 
            // tabPagecnc
            // 
            this.tabPagecnc.Font = new System.Drawing.Font("微软雅黑", 12F);
            this.tabPagecnc.Location = new System.Drawing.Point(4, 44);
            this.tabPagecnc.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.tabPagecnc.Name = "tabPagecnc";
            this.tabPagecnc.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.tabPagecnc.Size = new System.Drawing.Size(1916, 1007);
            this.tabPagecnc.TabIndex = 1;
            this.tabPagecnc.Tag = "SCADA.NewApp.CncNewForm";
            this.tabPagecnc.Text = "机床";
            this.tabPagecnc.UseVisualStyleBackColor = true;
            this.tabPagecnc.Visible = false;
            // 
            // tabPageRobot
            // 
            this.tabPageRobot.Location = new System.Drawing.Point(4, 44);
            this.tabPageRobot.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.tabPageRobot.Name = "tabPageRobot";
            this.tabPageRobot.Size = new System.Drawing.Size(2360, 1374);
            this.tabPageRobot.TabIndex = 7;
            this.tabPageRobot.Tag = "SCADA.NewApp.RobotForm";
            this.tabPageRobot.Text = "机器人";
            this.tabPageRobot.UseVisualStyleBackColor = true;
            // 
            // tabPagePLC
            // 
            this.tabPagePLC.Location = new System.Drawing.Point(4, 44);
            this.tabPagePLC.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.tabPagePLC.Name = "tabPagePLC";
            this.tabPagePLC.Size = new System.Drawing.Size(2360, 1374);
            this.tabPagePLC.TabIndex = 4;
            this.tabPagePLC.Tag = "SCADA.NewApp.PLCForm";
            this.tabPagePLC.Text = "PLC";
            this.tabPagePLC.UseVisualStyleBackColor = true;
            // 
            // tabPagemeasure
            // 
            this.tabPagemeasure.Location = new System.Drawing.Point(4, 44);
            this.tabPagemeasure.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.tabPagemeasure.Name = "tabPagemeasure";
            this.tabPagemeasure.Size = new System.Drawing.Size(2360, 1374);
            this.tabPagemeasure.TabIndex = 6;
            this.tabPagemeasure.Tag = "SCADA.NewApp.MeasureForm";
            this.tabPagemeasure.Text = "测量仪";
            this.tabPagemeasure.UseVisualStyleBackColor = true;
            // 
            // tabPagerfid
            // 
            this.tabPagerfid.Location = new System.Drawing.Point(4, 44);
            this.tabPagerfid.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.tabPagerfid.Name = "tabPagerfid";
            this.tabPagerfid.Size = new System.Drawing.Size(1916, 1007);
            this.tabPagerfid.TabIndex = 5;
            this.tabPagerfid.Tag = "SCADA.NewApp.RFIDTestForm";
            this.tabPagerfid.Text = "RFID";
            this.tabPagerfid.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 44);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(2360, 1374);
            this.tabPage2.TabIndex = 8;
            this.tabPage2.Tag = "SCADA.NewApp.AlarmForm";
            this.tabPage2.Text = "报警";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // DeviceMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 1055);
            this.Controls.Add(this.tabControlEQ1);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "DeviceMainForm";
            this.Text = "DeviceForm";
            this.tabControlEQ1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlEQ1;
        private System.Windows.Forms.TabPage tabPagerfid;
        private System.Windows.Forms.TabPage tabPagecnc;
        private System.Windows.Forms.TabPage tabPagePLC;
        private System.Windows.Forms.TabPage tabPagemeasure;
        private System.Windows.Forms.TabPage tabPageRobot;
        private System.Windows.Forms.TabPage tabPage2;
    }
}