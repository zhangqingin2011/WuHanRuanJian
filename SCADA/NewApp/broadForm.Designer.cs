namespace SCADA.NewApp
{
    partial class BoardForm
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
            this.tabControlAsi = new System.Windows.Forms.TabControl();
            this.tabPageboard = new System.Windows.Forms.TabPage();
            this.tabPageMset = new System.Windows.Forms.TabPage();
            this.tabPageMShow = new System.Windows.Forms.TabPage();
            this.tabPageMRecord = new System.Windows.Forms.TabPage();
            this.tabControlAsi.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlAsi
            // 
            this.tabControlAsi.Controls.Add(this.tabPageboard);
            this.tabControlAsi.Controls.Add(this.tabPageMShow);
            this.tabControlAsi.Controls.Add(this.tabPageMRecord);
            this.tabControlAsi.Controls.Add(this.tabPageMset);
            this.tabControlAsi.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlAsi.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlAsi.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControlAsi.ItemSize = new System.Drawing.Size(100, 40);
            this.tabControlAsi.Location = new System.Drawing.Point(0, 0);
            this.tabControlAsi.Multiline = true;
            this.tabControlAsi.Name = "tabControlAsi";
            this.tabControlAsi.SelectedIndex = 0;
            this.tabControlAsi.Size = new System.Drawing.Size(1045, 496);
            this.tabControlAsi.TabIndex = 3;
            this.tabControlAsi.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControlAsi_DrawItem);
            this.tabControlAsi.SelectedIndexChanged += new System.EventHandler(this.tabControlAsi_SelectedIndexChanged);
            // 
            // tabPageboard
            // 
            this.tabPageboard.Location = new System.Drawing.Point(4, 44);
            this.tabPageboard.Name = "tabPageboard";
            this.tabPageboard.Size = new System.Drawing.Size(1037, 448);
            this.tabPageboard.TabIndex = 0;
            this.tabPageboard.Tag = "SCADA.NewApp.StatisticsForm";
            this.tabPageboard.Text = "物料看板";
            this.tabPageboard.UseVisualStyleBackColor = true;
            // 
            // tabPageMset
            // 
            this.tabPageMset.Location = new System.Drawing.Point(4, 44);
            this.tabPageMset.Name = "tabPageMset";
            this.tabPageMset.Size = new System.Drawing.Size(1037, 448);
            this.tabPageMset.TabIndex = 1;
            this.tabPageMset.Tag = "SCADA.NewApp.QualitySetForm";
            this.tabPageMset.Text = "质量模板";
            this.tabPageMset.UseVisualStyleBackColor = true;
            this.tabPageMset.Visible = false;
            // 
            // tabPageMShow
            // 
            this.tabPageMShow.Location = new System.Drawing.Point(4, 44);
            this.tabPageMShow.Name = "tabPageMShow";
            this.tabPageMShow.Size = new System.Drawing.Size(1037, 448);
            this.tabPageMShow.TabIndex = 2;
            this.tabPageMShow.Tag = "SCADA.NewApp.QualityShowForm";
            this.tabPageMShow.Text = "质量看板";
            this.tabPageMShow.UseVisualStyleBackColor = true;
            // 
            // tabPageMRecord
            // 
            this.tabPageMRecord.Location = new System.Drawing.Point(4, 44);
            this.tabPageMRecord.Name = "tabPageMRecord";
            this.tabPageMRecord.Size = new System.Drawing.Size(1037, 448);
            this.tabPageMRecord.TabIndex = 3;
            this.tabPageMRecord.Tag = "SCADA.NewApp.QualityRecordForm";
            this.tabPageMRecord.Text = "质量查询";
            this.tabPageMRecord.UseVisualStyleBackColor = true;
            // 
            // BoardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1045, 496);
            this.Controls.Add(this.tabControlAsi);
            this.Name = "BoardForm";
            this.Text = "BoardForm";
            this.tabControlAsi.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlAsi;
        private System.Windows.Forms.TabPage tabPageboard;
        private System.Windows.Forms.TabPage tabPageMset;
        private System.Windows.Forms.TabPage tabPageMShow;
        private System.Windows.Forms.TabPage tabPageMRecord;
    }
}