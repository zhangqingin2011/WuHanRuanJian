using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SCADA;

namespace SCADA.NewApp
{
    public partial class LOGForm : Form
    {
        public LOGForm()
        {
            InitializeComponent();
        }

        private System.Data.DataTable ShowDataTable;
        String treeView_LogTree_Pathstr = LogData.LogDataNode0Name[1];
        private LogData m_OldLog = new LogData();
        bool LogOldNew = false;

        private void treeView_LogTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((e.Node != null && e.Node.FirstNode != null) || e.Node.Text == "网络日志")
            {
                treeView_LogTree_Pathstr = LogData.LogDataNode0Name[e.Node.Index + 1];
            }
            else if (e.Node != null && e.Node.FirstNode == null)
            {
                treeView_LogTree_Pathstr = LogData.LogDataNode0Name[e.Node.Parent.Index + 1];
                if (e.Node.Parent.Index == 0)
                {
                    treeView_LogTree_Pathstr += "\\" + LogData.LogDataNode1Name[e.Node.Index];
                }
                else if (e.Node.Parent.Index == 1)
                {
                    treeView_LogTree_Pathstr += "\\" + LogData.LogDataNode1Name[e.Node.Index + 2];
                }
            }
            button_ReFshLog_Click(null, null);
        }

        private void button_ReFshLog_Click(object sender, EventArgs e)
        {
            if (LogOldNew)
            {
                ShowDataTable = m_OldLog.ReadFromXml(treeView_LogTree_Pathstr);
            }
            else
            {
                ShowDataTable = LineMainForm.m_Log.ReadFromXml(treeView_LogTree_Pathstr);
            }

            if (sender != null)
            {
                treeView_LogTree.Nodes.Clear();
                TreeNode treeNode1 = new System.Windows.Forms.TreeNode("安全");
                TreeNode treeNode2 = new System.Windows.Forms.TreeNode("运行");
                TreeNode treeNode3 = new System.Windows.Forms.TreeNode("系统日志", new System.Windows.Forms.TreeNode[] {
                treeNode1,
                treeNode2});
                TreeNode treeNode4 = new System.Windows.Forms.TreeNode("CNC");
                TreeNode treeNode5 = new System.Windows.Forms.TreeNode("ROBOT");
                TreeNode treeNode6 = new System.Windows.Forms.TreeNode("PLC");
                TreeNode treeNode7 = new System.Windows.Forms.TreeNode("RFID");
                TreeNode treeNode8 = new System.Windows.Forms.TreeNode("设备日志", new System.Windows.Forms.TreeNode[] {
                treeNode4,
                treeNode5,
                treeNode6,
                treeNode7});
                TreeNode treeNode9 = new System.Windows.Forms.TreeNode("网络日志");
                treeView_LogTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
                treeNode3,
                treeNode8,
                treeNode9});
                treeView_LogTree.TabIndex = 0;
            }

            dataGridView_ShowLogData.DataSource = null;
            dataGridView_ShowLogData.DataSource = ShowDataTable;
            dataGridView_ShowLogData.Columns[0].Width = dataGridView_ShowLogData.Width / 6 - 100;
            dataGridView_ShowLogData.Columns[1].Width = dataGridView_ShowLogData.Width / 6 - 40;
            dataGridView_ShowLogData.Columns[2].Width = dataGridView_ShowLogData.Width / 6;
            dataGridView_ShowLogData.Columns[3].Width = dataGridView_ShowLogData.Width / 6 - 100;
            dataGridView_ShowLogData.Columns[4].Width = dataGridView_ShowLogData.Width / 6;
            dataGridView_ShowLogData.Columns[5].Width = dataGridView_ShowLogData.Width / 6 + 220;
        }

        static string[] PictrueFile = { "..\\picture\\Cross.ico",
        "..\\picture\\Key.ico","..\\picture\\Warning.ico","..\\picture\\yanzhong.ico", "..\\picture\\Mesg.ico"};//状态图片路径

        private void dataGridView_ShowLogData_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                if (this.dataGridView_ShowLogData.Rows[e.RowIndex].Cells[LogData.DataDataTableShowColumnStr[(int)LogData.Node2Attributes.Level]].Value == DBNull.Value ||
                    this.dataGridView_ShowLogData.Rows[e.RowIndex].Cells[LogData.DataDataTableShowColumnStr[(int)LogData.Node2Attributes.Level]].Value == null)
                    return;

                string vulestr = this.dataGridView_ShowLogData.Rows[e.RowIndex].Cells[LogData.DataDataTableShowColumnStr[(int)LogData.Node2Attributes.Level]].Value.ToString();
                //Console.WriteLine(vulestr);
                Bitmap m_bitmap;//= new Bitmap(System.Drawing.Image.FromFile(PictrueFile[0]));
                if (vulestr == "消息" || vulestr == "Message")
                {
                    m_bitmap = new Bitmap(System.Drawing.Image.FromFile(PictrueFile[4]));
                }
                else if (vulestr == "警告" || vulestr == "Warning")
                {
                    m_bitmap = new Bitmap(System.Drawing.Image.FromFile(PictrueFile[2]));
                }
                else if (vulestr == "审核" || vulestr == "Audit")
                {
                    m_bitmap = new Bitmap(System.Drawing.Image.FromFile(PictrueFile[1]));
                }
                else // (vulestr == LogData.LogDataNode2Level[(int)LogData.Node2Level.严重])
                {
                    m_bitmap = new Bitmap(System.Drawing.Image.FromFile(PictrueFile[3]));
                }
                Rectangle newRect = new Rectangle(e.CellBounds.X + 3, e.CellBounds.Y + 3, e.CellBounds.Height - 5,
                    e.CellBounds.Height - 5);

                using (Brush gridBrush = new SolidBrush(this.dataGridView_ShowLogData.GridColor), backColorBrush = new SolidBrush(e.CellStyle.BackColor))
                {
                    using (Pen gridLinePen = new Pen(gridBrush, 2))
                    {
                        // Erase the cell.
                        e.Graphics.FillRectangle(backColorBrush, e.CellBounds);

                        //划线
                        Point p1 = new Point(e.CellBounds.Left + e.CellBounds.Width, e.CellBounds.Top);
                        Point p2 = new Point(e.CellBounds.Left + e.CellBounds.Width, e.CellBounds.Top + e.CellBounds.Height);
                        Point p3 = new Point(e.CellBounds.Left, e.CellBounds.Top + e.CellBounds.Height);
                        Point[] ps = new Point[] { p1, p2, p3 };
                        e.Graphics.DrawLines(gridLinePen, ps);

                        //画图标
                        e.Graphics.DrawImage(m_bitmap, newRect);
                        //画字符串
                        e.Graphics.DrawString(vulestr, e.CellStyle.Font, Brushes.Crimson,
                            e.CellBounds.Left + 20, e.CellBounds.Top + 5, StringFormat.GenericDefault);
                        e.Handled = true;
                    }
                }
            }
        }

        private String FindStr;
        Form_LogFind m_FindForm = new Form_LogFind();

        private void button_LogFind_Click(object sender, EventArgs e)
        {
            if (m_FindForm.FindEventHandler == null)
            {
                m_FindForm.FindEventHandler = new EventHandler<String>(FindEventHandlerFuc);
                m_FindForm.SettextBox_FindStr(FindStr);
                m_FindForm.Show();
            }
            else
            {
                m_FindForm.Visible = true;
            }
        }

        //查找
        int FindRowsIndex = -1;
        private void FindEventHandlerFuc(object ob, String FindStr)
        {
            String BtText = ((Button)ob).Text;

            if (this.FindStr != FindStr)
            {
                FindRowsIndex = -1;
            }
            this.FindStr = FindStr;
            if (FindStr.Length != 0)
            {
                //if (BtText == "查找下一个")
                if ((Button)ob == m_FindForm.GetFindNext())
                {
                    for (int ii = FindRowsIndex + 1; ii < dataGridView_ShowLogData.RowCount; ii++)
                    {
                        foreach (DataGridViewCell dgvc in dataGridView_ShowLogData.Rows[ii].Cells)//遍历行中的所有单元格
                        {
                            if (dgvc.Value.ToString().Contains(FindStr))//如果单元格中的值符合
                            {
                                dataGridView_ShowLogData.Rows[ii].Selected = true;//单元格被选中
                                dataGridView_ShowLogData.CurrentCell = dataGridView_ShowLogData.Rows[ii].Cells[0];
                                //                         dataGridView_ShowLogData.Rows[ii].DefaultCellStyle.ForeColor = Color.Red;//红色
                                FindRowsIndex = dataGridView_ShowLogData.Rows[ii].Index;
                                return;
                            }
                        }
                    }
                    for (int ii = 0; ii < FindRowsIndex; ii++)
                    {
                        foreach (DataGridViewCell dgvc in dataGridView_ShowLogData.Rows[ii].Cells)//遍历行中的所有单元格
                        {
                            if (dgvc.Value.ToString().Contains(FindStr))//如果单元格中的值符合
                            {
                                dataGridView_ShowLogData.Rows[ii].Selected = true;//单元格被选中
                                dataGridView_ShowLogData.CurrentCell = dataGridView_ShowLogData.Rows[ii].Cells[0];
                                FindRowsIndex = dataGridView_ShowLogData.Rows[ii].Index;
                                return;
                            }
                        }
                    }
                }
                //else if (BtText == "查找上一个")
                else if ((Button)ob == m_FindForm.GetFindUP())
                {
                    if (FindRowsIndex == -1)
                    {
                        for (int ii = dataGridView_ShowLogData.RowCount - 1; ii >= 0; ii--)
                        {
                            foreach (DataGridViewCell dgvc in dataGridView_ShowLogData.Rows[ii].Cells)//遍历行中的所有单元格
                            {
                                if (dgvc.Value.ToString().Contains(FindStr))//如果单元格中的值符合
                                {
                                    dataGridView_ShowLogData.Rows[ii].Selected = true;//单元格被选中
                                    dataGridView_ShowLogData.CurrentCell = dataGridView_ShowLogData.Rows[ii].Cells[0];
                                    FindRowsIndex = dataGridView_ShowLogData.Rows[ii].Index;
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int ii = FindRowsIndex - 1; ii >= 0; ii--)
                        {
                            foreach (DataGridViewCell dgvc in dataGridView_ShowLogData.Rows[ii].Cells)//遍历行中的所有单元格
                            {
                                if (dgvc.Value.ToString().Contains(FindStr))//如果单元格中的值符合
                                {
                                    dataGridView_ShowLogData.Rows[ii].Selected = true;//单元格被选中
                                    dataGridView_ShowLogData.CurrentCell = dataGridView_ShowLogData.Rows[ii].Cells[0];
                                    FindRowsIndex = dataGridView_ShowLogData.Rows[ii].Index;
                                    return;
                                }
                            }
                        }
                        for (int ii = dataGridView_ShowLogData.RowCount - 1; ii > FindRowsIndex; ii--)
                        {
                            foreach (DataGridViewCell dgvc in dataGridView_ShowLogData.Rows[ii].Cells)//遍历行中的所有单元格
                            {
                                if (dgvc.Value.ToString().Contains(FindStr))//如果单元格中的值符合
                                {
                                    dataGridView_ShowLogData.Rows[ii].Selected = true;//单元格被选中
                                    dataGridView_ShowLogData.CurrentCell = dataGridView_ShowLogData.Rows[ii].Cells[0];
                                    FindRowsIndex = dataGridView_ShowLogData.Rows[ii].Index;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void button_CurentLog_Click(object sender, EventArgs e)
        {
            LogOldNew = false;
            button_ReFshLog_Click(null, null);
        }

        private void button_OpenOldLogFile_Click(object sender, EventArgs e)
        {
            try
            {
                System.Windows.Forms.OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
                openFileDialog1.Filter = "(*.xml)|*.xml";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    m_OldLog.m_load(openFileDialog1.FileName, false);
                    LogOldNew = true;
                    button_ReFshLog_Click(null, null);
                }
            }
            catch
            {

            }
        }

        private void LOGForm_Load(object sender, EventArgs e)
        {
            splitContainer2.SplitterDistance = 790;
            button_ReFshLog_Click(null, null);
        }
    }
}
