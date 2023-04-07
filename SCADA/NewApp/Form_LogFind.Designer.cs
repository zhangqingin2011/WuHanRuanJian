
namespace SCADA.NewApp
{
    partial class Form_LogFind
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
            this.button_Cans = new System.Windows.Forms.Button();
            this.button_FindUp = new System.Windows.Forms.Button();
            this.button_FindNext = new System.Windows.Forms.Button();
            this.textBox_FindStr = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_Cans
            // 
            this.button_Cans.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.button_Cans.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_Cans.ForeColor = System.Drawing.Color.White;
            this.button_Cans.Location = new System.Drawing.Point(329, 105);
            this.button_Cans.Name = "button_Cans";
            this.button_Cans.Size = new System.Drawing.Size(95, 42);
            this.button_Cans.TabIndex = 8;
            this.button_Cans.Text = "取消";
            this.button_Cans.UseVisualStyleBackColor = false;
            // 
            // button_FindUp
            // 
            this.button_FindUp.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.button_FindUp.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_FindUp.ForeColor = System.Drawing.Color.White;
            this.button_FindUp.Location = new System.Drawing.Point(182, 105);
            this.button_FindUp.Name = "button_FindUp";
            this.button_FindUp.Size = new System.Drawing.Size(109, 42);
            this.button_FindUp.TabIndex = 6;
            this.button_FindUp.Text = "查找上一个";
            this.button_FindUp.UseVisualStyleBackColor = false;
            // 
            // button_FindNext
            // 
            this.button_FindNext.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.button_FindNext.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_FindNext.ForeColor = System.Drawing.Color.White;
            this.button_FindNext.Location = new System.Drawing.Point(35, 105);
            this.button_FindNext.Name = "button_FindNext";
            this.button_FindNext.Size = new System.Drawing.Size(104, 42);
            this.button_FindNext.TabIndex = 7;
            this.button_FindNext.Text = "查找下一个";
            this.button_FindNext.UseVisualStyleBackColor = false;
            // 
            // textBox_FindStr
            // 
            this.textBox_FindStr.Location = new System.Drawing.Point(127, 48);
            this.textBox_FindStr.Name = "textBox_FindStr";
            this.textBox_FindStr.Size = new System.Drawing.Size(189, 29);
            this.textBox_FindStr.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 21);
            this.label1.TabIndex = 4;
            this.label1.Text = "查找内容：";
            // 
            // Form_LogFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 180);
            this.ControlBox = false;
            this.Controls.Add(this.button_Cans);
            this.Controls.Add(this.button_FindUp);
            this.Controls.Add(this.button_FindNext);
            this.Controls.Add(this.textBox_FindStr);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "Form_LogFind";
            this.Text = "查找";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Cans;
        private System.Windows.Forms.Button button_FindUp;
        private System.Windows.Forms.Button button_FindNext;
        private System.Windows.Forms.TextBox textBox_FindStr;
        private System.Windows.Forms.Label label1;
    }
}