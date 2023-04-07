namespace SCADA.NewApp
{
    partial class QualityMainForm
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPagerecord = new System.Windows.Forms.TabPage();
            this.tabPageshow = new System.Windows.Forms.TabPage();
            this.tabPageset = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageshow);
            this.tabControl1.Controls.Add(this.tabPagerecord);
            this.tabControl1.Controls.Add(this.tabPageset);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControl1.ItemSize = new System.Drawing.Size(100, 40);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1364, 681);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 0;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPagerecord
            // 
            this.tabPagerecord.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPagerecord.Location = new System.Drawing.Point(4, 44);
            this.tabPagerecord.Name = "tabPagerecord";
            this.tabPagerecord.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagerecord.Size = new System.Drawing.Size(1356, 633);
            this.tabPagerecord.TabIndex = 0;
            this.tabPagerecord.Tag = "SCADA.NewApp.QualityRecordForm";
            this.tabPagerecord.Text = "测量记录";
            this.tabPagerecord.UseVisualStyleBackColor = true;
            // 
            // tabPageshow
            // 
            this.tabPageshow.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPageshow.Location = new System.Drawing.Point(4, 44);
            this.tabPageshow.Margin = new System.Windows.Forms.Padding(4);
            this.tabPageshow.Name = "tabPageshow";
            this.tabPageshow.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageshow.Size = new System.Drawing.Size(1356, 633);
            this.tabPageshow.TabIndex = 1;
            this.tabPageshow.Tag = "SCADA.NewApp.QualityShowForm";
            this.tabPageshow.Text = "测量报告";
            this.tabPageshow.UseVisualStyleBackColor = true;
            // 
            // tabPageset
            // 
            this.tabPageset.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabPageset.Location = new System.Drawing.Point(4, 44);
            this.tabPageset.Name = "tabPageset";
            this.tabPageset.Size = new System.Drawing.Size(1356, 633);
            this.tabPageset.TabIndex = 2;
            this.tabPageset.Tag = "SCADA.NewApp.QualitySetForm";
            this.tabPageset.Text = "测量模板";
            this.tabPageset.UseVisualStyleBackColor = true;
            // 
            // QualityMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1364, 681);
            this.Controls.Add(this.tabControl1);
            this.Name = "QualityMainForm";
            this.Text = "QualityMainForm";
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPagerecord;
        private System.Windows.Forms.TabPage tabPageshow;
        private System.Windows.Forms.TabPage tabPageset;
    }
}