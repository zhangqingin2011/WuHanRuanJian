namespace SCADA.NewApp
{
    partial class SYSSetForm
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
            this.tabControlSYS = new System.Windows.Forms.TabControl();
            this.tabPageset = new System.Windows.Forms.TabPage();
            this.tabPagelog = new System.Windows.Forms.TabPage();
            this.tabPagerfid = new System.Windows.Forms.TabPage();
            this.tabControlSYS.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlSYS
            // 
            this.tabControlSYS.Controls.Add(this.tabPageset);
            this.tabControlSYS.Controls.Add(this.tabPagelog);
            this.tabControlSYS.Controls.Add(this.tabPagerfid);
            this.tabControlSYS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlSYS.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlSYS.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControlSYS.ItemSize = new System.Drawing.Size(100, 40);
            this.tabControlSYS.Location = new System.Drawing.Point(0, 0);
            this.tabControlSYS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControlSYS.Multiline = true;
            this.tabControlSYS.Name = "tabControlSYS";
            this.tabControlSYS.SelectedIndex = 0;
            this.tabControlSYS.Size = new System.Drawing.Size(1579, 828);
            this.tabControlSYS.TabIndex = 2;
            this.tabControlSYS.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControlSYS_DrawItem);
            this.tabControlSYS.SelectedIndexChanged += new System.EventHandler(this.tabControlSYS_SelectedIndexChanged);
            // 
            // tabPageset
            // 
            this.tabPageset.Location = new System.Drawing.Point(4, 44);
            this.tabPageset.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageset.Name = "tabPageset";
            this.tabPageset.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPageset.Size = new System.Drawing.Size(1571, 780);
            this.tabPageset.TabIndex = 0;
            this.tabPageset.Tag = "SCADA.NewApp.SetForm2";
            this.tabPageset.Text = "设置";
            this.tabPageset.UseVisualStyleBackColor = true;
            // 
            // tabPagelog
            // 
            this.tabPagelog.Location = new System.Drawing.Point(4, 44);
            this.tabPagelog.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPagelog.Name = "tabPagelog";
            this.tabPagelog.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPagelog.Size = new System.Drawing.Size(1571, 780);
            this.tabPagelog.TabIndex = 1;
            this.tabPagelog.Tag = "SCADA.NewApp.LOGForm";
            this.tabPagelog.Text = "日志";
            this.tabPagelog.UseVisualStyleBackColor = true;
            this.tabPagelog.Visible = false;
            // 
            // tabPagerfid
            // 
            this.tabPagerfid.Location = new System.Drawing.Point(4, 44);
            this.tabPagerfid.Name = "tabPagerfid";
            this.tabPagerfid.Size = new System.Drawing.Size(1571, 780);
            this.tabPagerfid.TabIndex = 2;
            this.tabPagerfid.Tag = "SCADA.NewApp.RFIDForm";
            this.tabPagerfid.Text = "RFID测试";
            this.tabPagerfid.UseVisualStyleBackColor = true;
            // 
            // SYSSetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1579, 828);
            this.Controls.Add(this.tabControlSYS);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "SYSSetForm";
            this.Text = "SYSSetForm";
            this.Load += new System.EventHandler(this.SYSSetForm_Load);
            this.tabControlSYS.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlSYS;
        private System.Windows.Forms.TabPage tabPageset;
        private System.Windows.Forms.TabPage tabPagelog;
        private System.Windows.Forms.TabPage tabPagerfid;
    }
}