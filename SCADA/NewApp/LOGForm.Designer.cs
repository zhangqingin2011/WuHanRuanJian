namespace SCADA.NewApp
{
    partial class LOGForm
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
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("安全");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("运行");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("系统日志", new System.Windows.Forms.TreeNode[] {
            treeNode10,
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("CNC");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("ROBOT");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("PLC");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("RFID");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("设备日志", new System.Windows.Forms.TreeNode[] {
            treeNode13,
            treeNode14,
            treeNode15,
            treeNode16});
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("网络日志");
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView_LogTree = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dataGridView_ShowLogData = new System.Windows.Forms.DataGridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button_OpenOldLogFile = new System.Windows.Forms.Button();
            this.button_CurentLog = new System.Windows.Forms.Button();
            this.button_LogFind = new System.Windows.Forms.Button();
            this.button_ReFshLog = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ShowLogData)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView_LogTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1284, 850);
            this.splitContainer1.SplitterDistance = 139;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView_LogTree
            // 
            this.treeView_LogTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView_LogTree.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.treeView_LogTree.Location = new System.Drawing.Point(0, 0);
            this.treeView_LogTree.Name = "treeView_LogTree";
            treeNode10.Name = "Node_system_security";
            treeNode10.Text = "安全";
            treeNode11.Name = "Node_system_runing";
            treeNode11.Text = "运行";
            treeNode12.Name = "Node_system";
            treeNode12.Text = "系统日志";
            treeNode13.Name = "Node_equipment_CNC";
            treeNode13.Text = "CNC";
            treeNode14.Name = "Node_equipment_ROBOT";
            treeNode14.Text = "ROBOT";
            treeNode15.Name = "Node_equipment_PLC";
            treeNode15.Text = "PLC";
            treeNode16.Name = "Node_equipment_RFID";
            treeNode16.Text = "RFID";
            treeNode17.Name = "Node_equipment";
            treeNode17.Text = "设备日志";
            treeNode18.Name = "Node_net";
            treeNode18.Text = "网络日志";
            this.treeView_LogTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode12,
            treeNode17,
            treeNode18});
            this.treeView_LogTree.Size = new System.Drawing.Size(139, 850);
            this.treeView_LogTree.TabIndex = 0;
            this.treeView_LogTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_LogTree_AfterSelect);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dataGridView_ShowLogData);
            this.splitContainer2.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer2.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer2.Size = new System.Drawing.Size(1141, 850);
            this.splitContainer2.SplitterDistance = 780;
            this.splitContainer2.TabIndex = 0;
            // 
            // dataGridView_ShowLogData
            // 
            this.dataGridView_ShowLogData.AllowUserToAddRows = false;
            this.dataGridView_ShowLogData.AllowUserToDeleteRows = false;
            this.dataGridView_ShowLogData.AllowUserToOrderColumns = true;
            this.dataGridView_ShowLogData.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView_ShowLogData.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.dataGridView_ShowLogData.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView_ShowLogData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_ShowLogData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView_ShowLogData.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.dataGridView_ShowLogData.Location = new System.Drawing.Point(0, 0);
            this.dataGridView_ShowLogData.MultiSelect = false;
            this.dataGridView_ShowLogData.Name = "dataGridView_ShowLogData";
            this.dataGridView_ShowLogData.ReadOnly = true;
            this.dataGridView_ShowLogData.RowHeadersVisible = false;
            this.dataGridView_ShowLogData.RowTemplate.Height = 23;
            this.dataGridView_ShowLogData.Size = new System.Drawing.Size(1141, 780);
            this.dataGridView_ShowLogData.TabIndex = 6;
            this.dataGridView_ShowLogData.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView_ShowLogData_CellPainting);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 10;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.Controls.Add(this.button_OpenOldLogFile, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_CurentLog, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_LogFind, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.button_ReFshLog, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1141, 66);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // button_OpenOldLogFile
            // 
            this.button_OpenOldLogFile.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.button_OpenOldLogFile.FlatAppearance.BorderSize = 0;
            this.button_OpenOldLogFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_OpenOldLogFile.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_OpenOldLogFile.ForeColor = System.Drawing.Color.White;
            this.button_OpenOldLogFile.Location = new System.Drawing.Point(687, 3);
            this.button_OpenOldLogFile.Name = "button_OpenOldLogFile";
            this.button_OpenOldLogFile.Size = new System.Drawing.Size(108, 60);
            this.button_OpenOldLogFile.TabIndex = 0;
            this.button_OpenOldLogFile.Text = "查看历史日志";
            this.button_OpenOldLogFile.UseVisualStyleBackColor = false;
            this.button_OpenOldLogFile.Click += new System.EventHandler(this.button_OpenOldLogFile_Click);
            // 
            // button_CurentLog
            // 
            this.button_CurentLog.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.button_CurentLog.FlatAppearance.BorderSize = 0;
            this.button_CurentLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_CurentLog.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_CurentLog.ForeColor = System.Drawing.Color.White;
            this.button_CurentLog.Location = new System.Drawing.Point(573, 3);
            this.button_CurentLog.Name = "button_CurentLog";
            this.button_CurentLog.Size = new System.Drawing.Size(108, 60);
            this.button_CurentLog.TabIndex = 2;
            this.button_CurentLog.Text = "查看当前日志";
            this.button_CurentLog.UseVisualStyleBackColor = false;
            this.button_CurentLog.Click += new System.EventHandler(this.button_CurentLog_Click);
            // 
            // button_LogFind
            // 
            this.button_LogFind.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.button_LogFind.FlatAppearance.BorderSize = 0;
            this.button_LogFind.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_LogFind.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_LogFind.ForeColor = System.Drawing.Color.White;
            this.button_LogFind.Location = new System.Drawing.Point(459, 3);
            this.button_LogFind.Name = "button_LogFind";
            this.button_LogFind.Size = new System.Drawing.Size(108, 60);
            this.button_LogFind.TabIndex = 3;
            this.button_LogFind.Text = "查找";
            this.button_LogFind.UseVisualStyleBackColor = false;
            this.button_LogFind.Click += new System.EventHandler(this.button_LogFind_Click);
            // 
            // button_ReFshLog
            // 
            this.button_ReFshLog.BackColor = System.Drawing.Color.DeepSkyBlue;
            this.button_ReFshLog.FlatAppearance.BorderSize = 0;
            this.button_ReFshLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button_ReFshLog.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_ReFshLog.ForeColor = System.Drawing.Color.White;
            this.button_ReFshLog.Location = new System.Drawing.Point(345, 3);
            this.button_ReFshLog.Name = "button_ReFshLog";
            this.button_ReFshLog.Size = new System.Drawing.Size(108, 60);
            this.button_ReFshLog.TabIndex = 1;
            this.button_ReFshLog.Text = "刷新";
            this.button_ReFshLog.UseVisualStyleBackColor = false;
            this.button_ReFshLog.Click += new System.EventHandler(this.button_ReFshLog_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.panel1.Size = new System.Drawing.Size(1284, 850);
            this.panel1.TabIndex = 5;
            // 
            // LOGForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 850);
            this.Controls.Add(this.panel1);
            this.Name = "LOGForm";
            this.Text = "LOGForm";
            this.Load += new System.EventHandler(this.LOGForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_ShowLogData)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView_LogTree;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView dataGridView_ShowLogData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button_OpenOldLogFile;
        private System.Windows.Forms.Button button_CurentLog;
        private System.Windows.Forms.Button button_LogFind;
        private System.Windows.Forms.Button button_ReFshLog;
        private System.Windows.Forms.Panel panel1;

    }
}