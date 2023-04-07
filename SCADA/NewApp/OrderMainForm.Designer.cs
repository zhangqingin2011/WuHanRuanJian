namespace SCADA.NewApp
{
    partial class OrderMainForm
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
            this.tabControlOR = new System.Windows.Forms.TabControl();
            this.tabPageOrder = new System.Windows.Forms.TabPage();
            this.tabPageGcode = new System.Windows.Forms.TabPage();
            this.tabControlOR.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlOR
            // 
            this.tabControlOR.Controls.Add(this.tabPageOrder);
            this.tabControlOR.Controls.Add(this.tabPageGcode);
            this.tabControlOR.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlOR.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControlOR.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tabControlOR.ItemSize = new System.Drawing.Size(100, 40);
            this.tabControlOR.Location = new System.Drawing.Point(0, 0);
            this.tabControlOR.Multiline = true;
            this.tabControlOR.Name = "tabControlOR";
            this.tabControlOR.SelectedIndex = 0;
            this.tabControlOR.Size = new System.Drawing.Size(1042, 468);
            this.tabControlOR.TabIndex = 3;
            this.tabControlOR.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControlOR_DrawItem);
            this.tabControlOR.SelectedIndexChanged += new System.EventHandler(this.tabControlOR_SelectedIndexChanged);
            // 
            // tabPageOrder
            // 
            this.tabPageOrder.Location = new System.Drawing.Point(4, 44);
            this.tabPageOrder.Name = "tabPageOrder";
            this.tabPageOrder.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOrder.Size = new System.Drawing.Size(1034, 420);
            this.tabPageOrder.TabIndex = 0;
            this.tabPageOrder.Tag = "SCADA.NewApp.OrderExcuteForm";
            this.tabPageOrder.Text = "订单执行";
            this.tabPageOrder.UseVisualStyleBackColor = true;
            // 
            // tabPageGcode
            // 
            this.tabPageGcode.Location = new System.Drawing.Point(4, 44);
            this.tabPageGcode.Name = "tabPageGcode";
            this.tabPageGcode.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGcode.Size = new System.Drawing.Size(1034, 420);
            this.tabPageGcode.TabIndex = 1;
            this.tabPageGcode.Tag = "SCADA.NewApp.GcodeForm";
            this.tabPageGcode.Text = "G代码下发";
            this.tabPageGcode.UseVisualStyleBackColor = true;
            this.tabPageGcode.Visible = false;
            // 
            // OrderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1042, 468);
            this.Controls.Add(this.tabControlOR);
            this.Name = "OrderForm";
            this.Text = "OrderForm";
            this.tabControlOR.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlOR;
        private System.Windows.Forms.TabPage tabPageOrder;
        private System.Windows.Forms.TabPage tabPageGcode;
    }
}