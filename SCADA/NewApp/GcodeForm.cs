using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScadaHncData;

namespace SCADA.NewApp
{
    public partial class GcodeForm : Form
    {
        public GcodeForm()
        {
            InitializeComponent();
            dataGridView_CNCGCode.AllowUserToAddRows = false;
            dataGridView_CNCGCode.ReadOnly = false;
            dataGridView_CNCGCode.RowHeadersVisible = false;

            dataGridView_GCodeSele.AllowUserToAddRows = false;
            dataGridView_GCodeSele.ReadOnly = false;
            dataGridView_GCodeSele.RowHeadersVisible = false;

            DataGridViewCheckBoxColumn dtCheck = new DataGridViewCheckBoxColumn();
            dtCheck.HeaderText = "选择";
            dtCheck.ReadOnly = false;
            dtCheck.Selected = false;

            dtCheck = new DataGridViewCheckBoxColumn();
            dtCheck.HeaderText = "选择";
            dtCheck.ReadOnly = false;
            dtCheck.Selected = false;
            dataGridView_CNCGCode.Columns.Insert(0, dtCheck);
            dataGridView_CNCGCode.Columns[0].Frozen = true;
            this.dataGridView_CNCGCode.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//设置自动换行
            for (int ii = 0; ii < STR_dataGridView_CNCGCode_Columns.Length; ii++)
            {
                dataGridView_CNCGCodeDb.Columns.Add(STR_dataGridView_CNCGCode_Columns[ii]);
            }
            dataGridView_CNCGCode.DataSource = dataGridView_CNCGCodeDb;

            dtCheck = new DataGridViewCheckBoxColumn();
            dtCheck.HeaderText = "选择";
            dtCheck.ReadOnly = false;
            dtCheck.Selected = false;
            dataGridView_GCodeSele.Columns.Insert(0, dtCheck);
            dataGridView_GCodeSele.Columns[0].Frozen = true;
            this.dataGridView_GCodeSele.DefaultCellStyle.WrapMode = DataGridViewTriState.True;//设置自动换行
            this.dataGridView_GCodeSele.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells; //设置自动调整高度
            for (int ii = 0; ii < STR_dataGridView_GCodeSele_Columns.Length; ii++)
            {
                dataGridView_GCodeSeleDb.Columns.Add(STR_dataGridView_GCodeSele_Columns[ii]);
            }
            dataGridView_GCodeSele.DataSource = dataGridView_GCodeSeleDb;
        }

        //public ShareData TaskDataObj; //2015.10.24

        const int resendNum = 3; //文件发送失败后的重发次数
        const int maxFileSize = 2; //发送单个文件大小最大默认为2M

        //2015.11.22   生产订单和机台编号 对换位置
        //string[] STR_WORKORDER = {"编号", "机台编号", "派工单号", "物料编码", "生产数量", "计划开始时间", "计划完成时间", "NC编号", "NC版本号", "作业指导号", "作业指导版本号", "线体", "工序编码", "生产订单", "机台组", "操作标示", "序号", "时间戳", "日期", "NC文件路径", "NC作业指导书" };
        string[] STR_dataGridView_CNCGCode_Columns = { "CNC机台号", "下载情况", "备注" };
        string[] STR_dataGridView_GCodeSele_Columns = { "G代码路径", "备注" };
        string[] MessageboxStr_登录 = { "你的操作权限不够，请先登录", "提示" };
        private Color sendGcodeBackColor = Color.Chocolate;
        private Color frezenBackColor = Color.BurlyWood;

        System.Data.DataTable dataGridView_CNCGCodeDb = new DataTable();
        System.Data.DataTable dataGridView_GCodeSeleDb = new DataTable();

        private void button_AddGCode_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.RestoreDirectory = true; //记忆上次浏览路径
                fileDialog.Multiselect = true;
                fileDialog.Title = ChangeLanguage.GetString("SelectFileTitle");
                fileDialog.Filter = ChangeLanguage.GetString("SelectFileFilter");

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    dataGridView_GCodeSele.DataSource = null;
                    foreach (string filename in fileDialog.FileNames)
                    {
                        if (IsGridwiewHasFilePath(filename))
                        {
                            MessageBox.Show("列表中已存在该G代码路径！" + filename);
                        }
                        else
                        {
                            string[] array = new string[STR_dataGridView_GCodeSele_Columns.Length];
                            array[0] = filename;
                            array[1] = "";
                            dataGridView_GCodeSeleDb.Rows.Add(array);
                        }
                    }
                    dataGridView_GCodeSele.DataSource = null;
                    dataGridView_GCodeSele.DataSource = dataGridView_GCodeSeleDb;
                }
            }
            catch
            {

            }
        }

        private bool IsGridwiewHasFilePath(string filename)
        {
            bool result = false;
            for (int i = 0; i < dataGridView_GCodeSeleDb.Rows.Count; i++)
            {
                if (filename == dataGridView_GCodeSeleDb.Rows[i][0].ToString())
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        private void GcodeForm_Load(object sender, EventArgs e)
        {
            if (LineMainForm.cnclist != null && LineMainForm.cnclist.Count > 0)
            {
                for (int ii = 0; ii < LineMainForm.cnclist.Count; ii++)
                {
                    string[] array = new string[STR_dataGridView_CNCGCode_Columns.Length];
                    array[0] = LineMainForm.cnclist[ii].BujianID;
                    array[1] = "";
                    array[2] = "";
                    dataGridView_CNCGCodeDb.Rows.Add(array);
                }
            }
        }

        private void button_DeletGCode_Click(object sender, EventArgs e)
        {
            System.Data.DataTable Db = new DataTable();
            STR_dataGridView_GCodeSele_Columns[0] = ChangeLanguage.GetString("GcodeSeleColumn02");
            STR_dataGridView_GCodeSele_Columns[1] = ChangeLanguage.GetString("GcodeSeleColumn03");
            for (int ii = 0; ii < STR_dataGridView_GCodeSele_Columns.Length; ii++)
            {
                Db.Columns.Add(STR_dataGridView_GCodeSele_Columns[ii]);
            }
            for (int ii = 0; ii < dataGridView_GCodeSeleDb.Rows.Count; ii++)
            {
                if (!(bool)dataGridView_GCodeSele.Rows[ii].Cells[0].EditedFormattedValue)
                {
                    string[] array = new string[STR_dataGridView_GCodeSele_Columns.Length];
                    array[0] = dataGridView_GCodeSeleDb.Rows[ii][0].ToString();
                    array[1] = dataGridView_GCodeSeleDb.Rows[ii][1].ToString();
                    Db.Rows.Add(array);
                }
            }
            dataGridView_GCodeSeleDb = Db;
            dataGridView_GCodeSele.DataSource = null;
            dataGridView_GCodeSele.DataSource = dataGridView_GCodeSeleDb;
        }

        private void button_DowLoadGCode_Click(object sender, EventArgs e)
        {
            string str3 = "";
            for (int ii = 0; ii < dataGridView_CNCGCode.Rows.Count; ii++)
            {
                if ((bool)dataGridView_CNCGCode.Rows[ii].Cells[0].EditedFormattedValue)
                {
                    string str1 = "", str2 = "";
                    int selectcount = 0;
                    for (int jj = 0; jj < dataGridView_GCodeSele.Rows.Count; jj++)
                    {
                        if ((bool)dataGridView_GCodeSele.Rows[jj].Cells[0].EditedFormattedValue)
                        {
                            selectcount++;
                            string[] filename = dataGridView_GCodeSeleDb.Rows[jj][0].ToString().Split('\\');
                            if (LineMainForm.cnclist[ii].sendFile(dataGridView_GCodeSeleDb.Rows[jj][0].ToString(), "h/lnc8/prog/" + filename[filename.Length - 1], 0, true) == 0)
                            {
                                str1 += filename[filename.Length - 1] + "；";
                            }
                            else
                            {
                                str2 += filename[filename.Length - 1] + "；";
                            }
                        }
                    }
                    if (selectcount == 0)
                    {
                        MessageBox.Show("未选择下发程序！");
                        return;
                    }
                    if (str1.Length > 0)
                    {
                        str1 = "下发成功：" + str1;
                    }
                    if (str2.Length > 0)
                    {
                        str2 = "下发失败：" + str2;
                    }
                    dataGridView_CNCGCodeDb.Rows[ii][1] = str1 + "\r\n" + str2;
                    if (str2.Length > 0)
                    {
                        dataGridView_CNCGCodeDb.Rows[ii][2] = DateTime.Now.ToString() + ":下发失败!";
                        str3 += dataGridView_CNCGCodeDb.Rows[ii][0] + ":" + str2;
                    }
                    else
                    {
                        dataGridView_CNCGCodeDb.Rows[ii][2] = DateTime.Now.ToString() + ":下发成功!";
                    }
                }
            }
        }
    }
}
