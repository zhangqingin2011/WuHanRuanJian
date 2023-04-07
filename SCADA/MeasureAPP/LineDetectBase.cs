using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HNC_MacDataService;
using HNCAPI;


namespace SCADA.NewApp
{
    public partial class LineDetectBase<TspBase> : Form
        where TspBase : SerialPortCommBase
    {
        public TspBase gauge;

        private DataTable dtResult = new DataTable();

        public Label labelSpCommState = new Label() { Text = "串口状态" };

        Button buttonClose = new Button() { Text = "关闭" };

        /// <summary>
        /// 测量完成事件
        /// </summary>
        public event EventHandler MeasureCompleted;

        /// <summary>
        /// 触发MeasureCompleted事件
        /// </summary>
        protected virtual void OnMeasureCompleted()
        {
            if (MeasureCompleted != null)
            {
                MeasureCompleted(this, new EventArgs());
            }
        }

        /// <summary>
        /// 测量计数
        /// </summary>
        protected int MeasureCount { get; private set; }

        /// <summary>
        /// 是否启用刀补
        /// </summary>
        protected bool toolCompensationEnabled;

        public LineDetectBase()
        {
            InitializeComponent();
            InitControl();

        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        protected virtual void InitControl()
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = dtResult;
            dataGridViewResult.DataSource = bs;
            AddLabelState(labelSpCommState);
            gauge.SpCommStateChanged += gauge_SpCommStateChanged;
            AddColumn("时间", "time", typeof(DateTime));
            AddColumn("序号", "sn", typeof(int));
            AddButton(buttonClose);
        }

        void gauge_SpCommStateChanged(object sender, SpCommStateChangedEventArgs e)
        {
            labelSpCommState.InvokeEx(c =>
            {
                c.Text = "串口：" + e.StateText;
            });
            buttonOpen.InvokeEx(c =>
            {
                c.Enabled = e.State == SpCommStates.通信正常 ? false : true;
            });
            buttonClose.InvokeEx(c =>
            {
                c.Enabled = e.State == SpCommStates.通信正常 ? false : true;
            });
        }

        /// <summary>
        /// 添加数据列
        /// </summary>
        /// <param name="headerText">显示名</param>
        /// <param name="columnName">字段名</param>
        protected void AddColumn(string headerText, string columnName)
        {
            AddColumn(headerText, columnName, typeof(string));
        }

        /// <summary>
        /// 添加数据列
        /// </summary>
        /// <param name="headerText">显示名</param>
        /// <param name="columnName">字段名</param>
        /// <param name="type">字段类型</param>
        private void AddColumn(string headerText, string columnName, Type type)
        {
            if (!dtResult.Columns.Contains(columnName))
            {
                dtResult.Columns.Add(columnName, type);
                dataGridViewResult.Columns[columnName].HeaderText = headerText;
                dataGridViewResult.Columns[columnName].MinimumWidth = 220;
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="data">列名，值</param>
        protected void AddResultData(params Tuple<string, string>[] data)
        {
            MeasureCount++;
            DataRow dr = dtResult.NewRow();
            dr["time"] = DateTime.Now;
            dr["sn"] = MeasureCount;
            foreach (var d in data)
            {
                dr[d.Item1] = d.Item2;
            }
            dtResult.Rows.InsertAt(dr, 0);
            dataGridViewResult.InvokeEx(c =>
            {
                BindingSource bs = new BindingSource();
                bs.DataSource = dtResult;
                c.DataSource = bs;
                c.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            });
        }

        /// <summary>
        /// 在groupBoxButton中添加Button
        /// </summary>
        /// <param name="button"></param>
        protected void AddButton(Button button)
        {
            if (!groupBoxButton.Controls.Contains(button))
            {
                button.Height = 50;
                button.Dock = DockStyle.Top;
                groupBoxButton.Controls.Add(button);
                button.BringToFront();
            }
        }

        /// <summary>
        /// 在groupBoxState中添加Label
        /// </summary>
        /// <param name="label"></param>
        protected void AddLabelState(Label label)
        {
            if (!groupBoxState.Contains(label))
            {
                label.AutoSize = true;
                label.Dock = DockStyle.Left;
                label.TextAlign = ContentAlignment.MiddleCenter;
                groupBoxState.Controls.Add(label);
                label.BringToFront();
            }
        }

        /// <summary>
        /// 设置测量仪显示数据
        /// </summary>
        /// <param name="text">数据</param>
        protected void SetTextBoxMeasureResult(string text)
        {
            textBoxMeasureResult.InvokeEx(c => { c.Text = text; });
        }

        private void LineDetectBase_Shown(object sender, EventArgs e)
        {
            //buttonOpen.PerformClick();
        }

        private void LineOpen()
        {
            if (gauge != null && !gauge.IsOpen)
            {
                gauge.Open();
            }
          
        }

        private void LineClose()
        {
            if (gauge != null && gauge.IsOpen)
            {
                gauge.Close();
            }
          
        }

        private void LineDetectBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            //LineClose();
        }

        protected virtual void buttonOpen_Click(object sender, EventArgs e)
        {
            try
            {
                //LineOpen();
            }
            catch (Exception ex)
            {
                //LineClose();
            }
        }

        protected virtual void buttonReset_Click(object sender, EventArgs e)
        {
            //TODO复位，清除相关信号
        }

        protected virtual void buttonToolCompensation_Click(object sender, EventArgs e)
        {
            toolCompensationEnabled = !toolCompensationEnabled;
            buttonToolCompensation.Text = toolCompensationEnabled ? "关闭刀补" : "启动刀补";
        }


    }
}
