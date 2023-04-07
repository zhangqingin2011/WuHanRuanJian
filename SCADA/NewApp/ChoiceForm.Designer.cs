namespace SCADA.NewApp
{
    partial class ChoiceForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.roundButton3 = new SCADA.NewApp.RoundButton();
            this.roundButton2 = new SCADA.NewApp.RoundButton();
            this.roundButton1 = new SCADA.NewApp.RoundButton();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.roundButton3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.roundButton2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.roundButton1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 461F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 461F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1184, 396);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // roundButton3
            // 
            this.roundButton3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.roundButton3.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.roundButton3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundButton3.FlatAppearance.BorderSize = 0;
            this.roundButton3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.roundButton3.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.roundButton3.ForeColor = System.Drawing.Color.White;
            this.roundButton3.ImageEnter = null;
            this.roundButton3.ImageNormal = null;
            this.roundButton3.Location = new System.Drawing.Point(791, 3);
            this.roundButton3.Name = "roundButton3";
            this.roundButton3.Radius = 390;
            this.roundButton3.Size = new System.Drawing.Size(390, 390);
            this.roundButton3.TabIndex = 2;
            this.roundButton3.Text = "退出系统";
            this.roundButton3.UseVisualStyleBackColor = false;
            this.roundButton3.Click += new System.EventHandler(this.roundButton3_Click);
            // 
            // roundButton2
            // 
            this.roundButton2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.roundButton2.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.roundButton2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundButton2.FlatAppearance.BorderSize = 0;
            this.roundButton2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.roundButton2.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.roundButton2.ForeColor = System.Drawing.Color.White;
            this.roundButton2.ImageEnter = null;
            this.roundButton2.ImageNormal = null;
            this.roundButton2.Location = new System.Drawing.Point(397, 4);
            this.roundButton2.Name = "roundButton2";
            this.roundButton2.Radius = 388;
            this.roundButton2.Size = new System.Drawing.Size(388, 388);
            this.roundButton2.TabIndex = 1;
            this.roundButton2.Text = "联动产线MES";
            this.roundButton2.UseVisualStyleBackColor = false;
            this.roundButton2.Click += new System.EventHandler(this.roundButton2_Click);
            // 
            // roundButton1
            // 
            this.roundButton1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.roundButton1.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.roundButton1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.roundButton1.FlatAppearance.BorderSize = 0;
            this.roundButton1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.roundButton1.Font = new System.Drawing.Font("微软雅黑", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.roundButton1.ForeColor = System.Drawing.Color.White;
            this.roundButton1.ImageEnter = null;
            this.roundButton1.ImageNormal = null;
            this.roundButton1.Location = new System.Drawing.Point(3, 4);
            this.roundButton1.Name = "roundButton1";
            this.roundButton1.Radius = 388;
            this.roundButton1.Size = new System.Drawing.Size(388, 388);
            this.roundButton1.TabIndex = 0;
            this.roundButton1.Text = "人社大赛MES";
            this.roundButton1.UseVisualStyleBackColor = false;
            this.roundButton1.Click += new System.EventHandler(this.roundButton1_Click);
            // 
            // ChoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.SteelBlue;
            this.ClientSize = new System.Drawing.Size(1184, 396);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "ChoiceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "系统选择";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private RoundButton roundButton1;
        private RoundButton roundButton3;
        private RoundButton roundButton2;

    }
}