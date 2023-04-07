namespace SCADA.NewApp
{
    partial class GcodeForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.button_DowLoadGCode = new System.Windows.Forms.Button();
            this.button_DeletGCode = new System.Windows.Forms.Button();
            this.button_AddGCode = new System.Windows.Forms.Button();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dataGridView_CNCGCode = new System.Windows.Forms.DataGridView();
            this.dataGridView_GCodeSele = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_CNCGCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_GCodeSele)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel1.Controls.Add(this.button_DowLoadGCode);
            this.splitContainer1.Panel1.Controls.Add(this.button_DeletGCode);
            this.splitContainer1.Panel1.Controls.Add(this.button_AddGCode);
            this.splitContainer1.Panel1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel2);
            this.splitContainer1.Size = new System.Drawing.Size(1284, 761);
            this.splitContainer1.SplitterDistance = 230;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 3;
            // 
            // button_DowLoadGCode
            // 
            this.button_DowLoadGCode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button_DowLoadGCode.BackColor = System.Drawing.Color.DarkTurquoise;
            this.button_DowLoadGCode.FlatAppearance.BorderSize = 0;
            this.button_DowLoadGCode.ForeColor = System.Drawing.Color.White;
            this.button_DowLoadGCode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button_DowLoadGCode.Location = new System.Drawing.Point(43, 402);
            this.button_DowLoadGCode.Name = "button_DowLoadGCode";
            this.button_DowLoadGCode.Size = new System.Drawing.Size(150, 45);
            this.button_DowLoadGCode.TabIndex = 17;
            this.button_DowLoadGCode.Text = "G代码派发";
            this.button_DowLoadGCode.UseVisualStyleBackColor = false;
            this.button_DowLoadGCode.Click += new System.EventHandler(this.button_DowLoadGCode_Click);
            // 
            // button_DeletGCode
            // 
            this.button_DeletGCode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button_DeletGCode.BackColor = System.Drawing.Color.DarkTurquoise;
            this.button_DeletGCode.FlatAppearance.BorderSize = 0;
            this.button_DeletGCode.ForeColor = System.Drawing.Color.White;
            this.button_DeletGCode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button_DeletGCode.Location = new System.Drawing.Point(43, 258);
            this.button_DeletGCode.Name = "button_DeletGCode";
            this.button_DeletGCode.Size = new System.Drawing.Size(150, 45);
            this.button_DeletGCode.TabIndex = 16;
            this.button_DeletGCode.Text = "删除G代码";
            this.button_DeletGCode.UseVisualStyleBackColor = false;
            this.button_DeletGCode.Click += new System.EventHandler(this.button_DeletGCode_Click);
            // 
            // button_AddGCode
            // 
            this.button_AddGCode.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button_AddGCode.BackColor = System.Drawing.Color.DarkTurquoise;
            this.button_AddGCode.FlatAppearance.BorderSize = 0;
            this.button_AddGCode.ForeColor = System.Drawing.Color.White;
            this.button_AddGCode.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button_AddGCode.Location = new System.Drawing.Point(43, 108);
            this.button_AddGCode.Name = "button_AddGCode";
            this.button_AddGCode.Size = new System.Drawing.Size(150, 45);
            this.button_AddGCode.TabIndex = 15;
            this.button_AddGCode.Text = "添加G代码";
            this.button_AddGCode.UseVisualStyleBackColor = false;
            this.button_AddGCode.Click += new System.EventHandler(this.button_AddGCode_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.dataGridView_CNCGCode, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.dataGridView_GCodeSele, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1049, 761);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // dataGridView_CNCGCode
            // 
            this.dataGridView_CNCGCode.AllowUserToDeleteRows = false;
            this.dataGridView_CNCGCode.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_CNCGCode.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView_CNCGCode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_CNCGCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_CNCGCode.Location = new System.Drawing.Point(4, 5);
            this.dataGridView_CNCGCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView_CNCGCode.Name = "dataGridView_CNCGCode";
            this.dataGridView_CNCGCode.RowHeadersVisible = false;
            this.dataGridView_CNCGCode.RowHeadersWidth = 51;
            this.dataGridView_CNCGCode.RowTemplate.Height = 23;
            this.dataGridView_CNCGCode.Size = new System.Drawing.Size(1041, 370);
            this.dataGridView_CNCGCode.TabIndex = 0;
            // 
            // dataGridView_GCodeSele
            // 
            this.dataGridView_GCodeSele.AllowUserToDeleteRows = false;
            this.dataGridView_GCodeSele.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_GCodeSele.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView_GCodeSele.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_GCodeSele.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_GCodeSele.Location = new System.Drawing.Point(4, 385);
            this.dataGridView_GCodeSele.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView_GCodeSele.Name = "dataGridView_GCodeSele";
            this.dataGridView_GCodeSele.RowHeadersWidth = 51;
            this.dataGridView_GCodeSele.RowTemplate.Height = 23;
            this.dataGridView_GCodeSele.Size = new System.Drawing.Size(1041, 371);
            this.dataGridView_GCodeSele.TabIndex = 0;
            // 
            // GcodeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1284, 761);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "GcodeForm";
            this.Text = "GcodeForm";
            this.Load += new System.EventHandler(this.GcodeForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_CNCGCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_GCodeSele)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.DataGridView dataGridView_CNCGCode;
        private System.Windows.Forms.DataGridView dataGridView_GCodeSele;
        private System.Windows.Forms.Button button_AddGCode;
        private System.Windows.Forms.Button button_DowLoadGCode;
        private System.Windows.Forms.Button button_DeletGCode;
    }
}