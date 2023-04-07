using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SCADA.SimensPLC;

namespace SCADA.NewApp
{
    public partial class AlarmForm : Form
    {
        public AlarmForm()
        {
            InitializeComponent();
            InitUI();
            tabControl1.TabPages.Remove(tabPage2);
        }
        private void InitUI()
        {
            ControlPLC.ALARMINDEX e0 = new ControlPLC.ALARMINDEX();
            string[] regnames1 = Enum.GetNames(e0.GetType());
            var regindexs1 = Enum.GetValues(e0.GetType());

            for (int i = 0; i < regnames1.Length; i++)
            {
                int m = dataGridViewLine.Rows.Add();
                dataGridViewLine.Rows[m].Cells[0].Value = (int)regindexs1.GetValue(i);
                dataGridViewLine.Rows[m].Cells[1].Value = regnames1[i];
            }
            WMSPLC.ALARMINDEX e1 = new WMSPLC.ALARMINDEX();
            string[] regnames2 = Enum.GetNames(e0.GetType());
            var regindexs2 = Enum.GetValues(e0.GetType());
            for (int i = 0; i < regnames2.Length; i++)
            {
                int m = dataGridViewWMS.Rows.Add();
                dataGridViewWMS.Rows[m].Cells[0].Value = (int)regindexs2.GetValue(i);
                dataGridViewWMS.Rows[m].Cells[1].Value = regnames2[i];
            }
            dataGridViewLine.ClearSelection();
            dataGridViewWMS.ClearSelection();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            Timer timer = sender as Timer;

            UpdateControlPLCAlarm();
            UpdateWMSPLCAlarm();

        }
        private void UpdateControlPLCAlarm()
        {
            ControlPLC.REGINDEX e0 = new ControlPLC.REGINDEX();
            var regindexs = Enum.GetValues(e0.GetType());
            int start = (int)ControlPLC.REGINDEX.定位台报警信息1 + 1;
            int end = (int)ControlPLC.REGINDEX.单元4报警 + 1;
            int k = end - start+1;
            int[] values1 = new int[k];
            if (LineMainForm.controlplc.GetOnlineState())
            {

                bool res1 = LineMainForm.controlplc.ReadMutiRegisters(start, k, out values1);
                if ((values1[1] & 0x01) == 0x01)
                {
                    dataGridViewLine.Rows[0].Cells[2].Value = "报警";
                   // dataGridViewLine.SelectedRows[0].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[0].Cells[2].Value = "未报警";
                    //dataGridViewLine.SelectedRows[0].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[1] & 0x02) == 0x02)
                {
                    dataGridViewLine.Rows[1].Cells[2].Value = "报警";
                    //dataGridViewLine.SelectedRows[1].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[1].Cells[2].Value = "未报警";
                    // dataGridViewLine.SelectedRows[1].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[1] & 0x04) == 0x04)
                {
                    dataGridViewLine.Rows[2].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[2].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[2].Cells[2].Value = "未报警";
                    //dataGridViewLine.SelectedRows[2].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[1] & 0x08) == 0x08)
                {
                    dataGridViewLine.Rows[3].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[3].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[3].Cells[2].Value = "未报警";
                    //dataGridViewLine.SelectedRows[3].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[1] & 0x10) == 0x10)
                {
                    dataGridViewLine.Rows[4].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[4].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[4].Cells[2].Value = "未报警";
                    //  dataGridViewLine.SelectedRows[4].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[1] & 0x20) == 0x20)
                {
                    dataGridViewLine.Rows[5].Cells[2].Value = "报警";
                    //  dataGridViewLine.SelectedRows[5].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[5].Cells[2].Value = "未报警";
                    // dataGridViewLine.SelectedRows[5].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[1] & 0x40) == 0x40)
                {
                    dataGridViewLine.Rows[6].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[6].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[6].Cells[2].Value = "未报警";
                    //dataGridViewLine.SelectedRows[6].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[1] & 0x80) == 0x80)
                {
                    dataGridViewLine.Rows[7].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[7].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[7].Cells[2].Value = "未报警";
                    //dataGridViewLine.SelectedRows[7].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[0] & 0x0001) == 0x0001)
                {
                    dataGridViewLine.Rows[8].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[8].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[8].Cells[2].Value = "未报警";
                    // dataGridViewLine.SelectedRows[8].DefaultCellStyle.BackColor = Color.White;
                }

                if ((values1[0] & 0x0002) == 0x0002)
                {
                    dataGridViewLine.Rows[9].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[9].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[9].Cells[2].Value = "未报警";
                    // dataGridViewLine.SelectedRows[9].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[0] & 0x0004) == 0x0004)
                {
                    dataGridViewLine.Rows[10].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[10].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[10].Cells[2].Value = "未报警";
                    // dataGridViewLine.SelectedRows[10].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[0] & 0x0008) == 0x0008)
                {
                    dataGridViewLine.Rows[11].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[11].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[11].Cells[2].Value = "未报警";
                    // dataGridViewLine.SelectedRows[11].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[0] & 0x0010) == 0x0010)
                {
                    dataGridViewLine.Rows[12].Cells[2].Value = "报警";
                    //  dataGridViewLine.SelectedRows[12].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[12].Cells[2].Value = "未报警";
                    //  dataGridViewLine.SelectedRows[12].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[0] & 0x0020) == 0x0020)
                {
                    dataGridViewLine.Rows[13].Cells[2].Value = "报警";
                    //  dataGridViewLine.SelectedRows[13].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[13].Cells[2].Value = "未报警";
                    // dataGridViewLine.SelectedRows[13].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[0] & 0x0040) == 0x0040)
                {
                    dataGridViewLine.Rows[14].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[14].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[14].Cells[2].Value = "未报警";
                    //dataGridViewLine.SelectedRows[14].DefaultCellStyle.BackColor = Color.White;
                }
                if ((values1[0] & 0x0080) == 0x0080)
                {
                    dataGridViewLine.Rows[15].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[15].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[15].Cells[2].Value = "未报警";
                    // dataGridViewLine.SelectedRows[15].DefaultCellStyle.BackColor = Color.White;
                }

                if (values1[4] != 0)
                {
                    dataGridViewLine.Rows[16].Cells[2].Value = "报警";
                    //  dataGridViewLine.SelectedRows[16].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[16].Cells[2].Value = "未报警";
                    //  dataGridViewLine.SelectedRows[16].DefaultCellStyle.BackColor = Color.White;
                }
                if (values1[6] != 0)
                {
                    dataGridViewLine.Rows[17].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[17].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[17].Cells[2].Value = "未报警";
                    // dataGridViewLine.SelectedRows[17].DefaultCellStyle.BackColor = Color.White;
                }
                if (values1[8] != 0)
                {
                    dataGridViewLine.Rows[18].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[18].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[18].Cells[2].Value = "未报警";
                    // dataGridViewLine.SelectedRows[18].DefaultCellStyle.BackColor = Color.White;
                }
                if (values1[10] != 0)
                {
                    dataGridViewLine.Rows[19].Cells[2].Value = "报警";
                    //  dataGridViewLine.SelectedRows[19].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[19].Cells[2].Value = "未报警";
                    //dataGridViewLine.SelectedRows[19].DefaultCellStyle.BackColor = Color.White;
                }
                if (values1[12] !=0)
                {
                    dataGridViewLine.Rows[20].Cells[2].Value = "报警";
                    // dataGridViewLine.SelectedRows[20].DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    dataGridViewLine.Rows[20].Cells[2].Value = "未报警";
                    // dataGridViewLine.SelectedRows[20].DefaultCellStyle.BackColor = Color.White;
                }
                //dataGridViewLine.Rows[0].Cells[2].Value = (values1[0] & 0x0100).ToString();
                //dataGridViewLine.Rows[1].Cells[2].Value = (values1[0] & 0x0200).ToString();
                //dataGridViewLine.Rows[2].Cells[2].Value = (values1[0] & 0x0400).ToString();
                //dataGridViewLine.Rows[3].Cells[2].Value = (values1[0] & 0x0800).ToString();
                //dataGridViewLine.Rows[4].Cells[2].Value = (values1[0] & 0x1000).ToString();
                //dataGridViewLine.Rows[5].Cells[2].Value = (values1[0] & 0x2000).ToString();
                //dataGridViewLine.Rows[6].Cells[2].Value = (values1[0] & 0x4000).ToString();
                //dataGridViewLine.Rows[7].Cells[2].Value = (values1[0] & 0x8000).ToString();
                //dataGridViewLine.Rows[8].Cells[2].Value = (values1[0] & 0x0001).ToString();
                //dataGridViewLine.Rows[9].Cells[2].Value = (values1[0] & 0x0002).ToString();
                //dataGridViewLine.Rows[10].Cells[2].Value = (values1[0] & 0x0004).ToString();
                //dataGridViewLine.Rows[11].Cells[2].Value = (values1[0] & 0x0008).ToString();
                //dataGridViewLine.Rows[12].Cells[2].Value = (values1[0] & 0x0010).ToString();
                //dataGridViewLine.Rows[13].Cells[2].Value = (values1[0] & 0x0020).ToString();
                //dataGridViewLine.Rows[14].Cells[2].Value = (values1[0] & 0x0040).ToString();
                //dataGridViewLine.Rows[15].Cells[2].Value = (values1[0] & 0x0080).ToString();

                //dataGridViewLine.Rows[16].Cells[2].Value = (values1[4] & 0x0100).ToString();
                //dataGridViewLine.Rows[17].Cells[2].Value = (values1[4] & 0x0200).ToString();
                //dataGridViewLine.Rows[18].Cells[2].Value = (values1[4] & 0x0400).ToString();
                //dataGridViewLine.Rows[19].Cells[2].Value = (values1[4] & 0x0800).ToString();
                //dataGridViewLine.Rows[20].Cells[2].Value = (values1[4] & 0x1000).ToString();
            }
            else
            {
                for (int i = 0; i < dataGridViewLine.Rows.Count; i++)
                {
                    dataGridViewLine.Rows[i].Cells[2].Value = "离线";
                }
            }
        }

        private void UpdateWMSPLCAlarm()//未完成，wms信号未定义
        {
            WMSPLC.DATAINDEX e0 = new WMSPLC.DATAINDEX();
            var regindexs = Enum.GetValues(e0.GetType());

            if (LineMainForm.controlplc.GetOnlineState())
            {
                int start = (int)WMSPLC.DATAINDEX.RackErrCode + 1;
                int end = start + 1;
                int[] values1 = new int[end - start];
                bool res1 = LineMainForm.wmsplc.ReadMutiRegisters(start, end, out values1);
                dataGridViewWMS.Rows[0].Cells[2].Value = (values1[0] & 0x0100).ToString();
            }
            else
            {
                for (int i = 0; i < dataGridViewWMS.Rows.Count; i++)
                {
                    dataGridViewWMS.Rows[i].Cells[2].Value = "离线";
                }
            }
        }



        void AlarmTick()
        {
            while (true)
            {
                System.Threading.Thread.Sleep(3500);

            }
        }

        private void dataGridViewLine_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }

        private void dataGridViewWMS_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }

        private void dataGridViewU1_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }

        private void dataGridViewU2_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }

        private void dataGridViewU3_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }

        private void dataGridViewU4_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }
    }
}
