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
using SCADA.SimensPLC;

namespace SCADA.NewApp
{
    public partial class PLCForm : Form
    {
        public PLCForm()
        {
            InitializeComponent();
            InitUI();
        }

        DataTable WMSPLCAlarmDataSource = new DataTable();
        DataTable ControlPLCAlarmDataSource = new DataTable();

        private void InitUI()
        {
            ControlPLC.REGINDEX e0 = new ControlPLC.REGINDEX();
            string[] regnames = Enum.GetNames(e0.GetType());
            var regindexs = Enum.GetValues(e0.GetType());
            for (int i = 0; i < regnames.Length; i++)
            {
                int m = dataGridView1.Rows.Add();
                dataGridView1.Rows[m].Cells[0].Value = (int)regindexs.GetValue(i);
                dataGridView1.Rows[m].Cells[1].Value = regnames[i];
            }

            UnitPLC.REGINDEX e1 = new UnitPLC.REGINDEX();
            string[] regnames1 = Enum.GetNames(e1.GetType());
            var regindexs1 = Enum.GetValues(e1.GetType());
            for (int i = 0; i < regindexs1.Length; i++)
            {
                int m = dataGridView5.Rows.Add();
                dataGridView6.Rows.Add();
                dataGridView7.Rows.Add();
                dataGridView8.Rows.Add();
                dataGridView5.Rows[m].Cells[0].Value = (int)regindexs1.GetValue(i);
                dataGridView5.Rows[m].Cells[1].Value = regnames1[i];
                dataGridView6.Rows[m].Cells[0].Value = (int)regindexs1.GetValue(i);
                dataGridView6.Rows[m].Cells[1].Value = regnames1[i];
                dataGridView7.Rows[m].Cells[0].Value = (int)regindexs1.GetValue(i);
                dataGridView7.Rows[m].Cells[1].Value = regnames1[i];
                dataGridView8.Rows[m].Cells[0].Value = (int)regindexs1.GetValue(i);
                dataGridView8.Rows[m].Cells[1].Value = regnames1[i];

            }

            WMSPLC.DATAINDEX e2 = new WMSPLC.DATAINDEX();
            WMSPLC.DATAMEANS e3 = new WMSPLC.DATAMEANS();

           // var datanames = Enum.GetNames(e2.GetType());
            var datanames = Enum.GetValues(typeof(WMSPLC.DATAINDEX));
            var datameans = Enum.GetNames(e3.GetType());

            for (int i = 0; i < datanames.Length; i++)
            {
                int m = dataGridView3.Rows.Add();
                dataGridView3.Rows[m].Cells[0].Value =(int) datanames.GetValue(i);
                dataGridView3.Rows[m].Cells[1].Value = datameans[i];
            }


            dataGridView1.ClearSelection();
            dataGridView3.ClearSelection();
            dataGridView5.ClearSelection();
            dataGridView6.ClearSelection();
            dataGridView7.ClearSelection();
        }

        private void Timersignal_Tick(object sender, EventArgs e)
        {

            UpdateControlPLCSingnal();
            UpdateWMSPLCSingnal();
            UpdateUnitPLCSingnal();
        }


        private void UpdateUnitPLCSingnal()
        {
            UnitPLC.REGINDEX e0 = new UnitPLC.REGINDEX();
            var regindexs = Enum.GetValues(e0.GetType());

            //更新单元1
            if (LineMainForm.unitplc1.GetOnlineState())
            {
                int ModbusRegNum = (int)UnitPLC.REGINDEX.复位完成反馈 + 1;
              
                    int[] values = new int[ModbusRegNum];
                    bool res1 = LineMainForm.unitplc1.ReadMutiRegisters(0, values.Length, out values);
                    for (int i = 0; i < regindexs.Length; i++)
                    {
                        dataGridView5.Rows[i].Cells[2].Value = values[(int)regindexs.GetValue(i)];
                    }
               
            }
            else
            {
                for (int i = 0; i < regindexs.Length; i++)
                {
                    dataGridView5.Rows[i].Cells[2].Value = "";
                }
            }


            //更新单元2
            if (LineMainForm.unitplc2.GetOnlineState())
            {
                int ModbusRegNum = (int)UnitPLC.REGINDEX.复位完成反馈 + 1;

                int[] values = new int[ModbusRegNum];
                bool res1 = LineMainForm.unitplc2.ReadMutiRegisters(0, values.Length, out values);
                for (int i = 0; i < regindexs.Length; i++)
                {
                    dataGridView6.Rows[i].Cells[2].Value = values[(int)regindexs.GetValue(i)];
                }

            }
            else
            {
                for (int i = 0; i < regindexs.Length; i++)
                {
                    dataGridView6.Rows[i].Cells[2].Value = "";
                }
            }


            //更新单元3
            if (LineMainForm.unitplc3.GetOnlineState())
            {
                int ModbusRegNum = (int)UnitPLC.REGINDEX.复位完成反馈 + 1;

                int[] values = new int[ModbusRegNum];
                bool res1 = LineMainForm.unitplc3.ReadMutiRegisters(0, values.Length, out values);
                for (int i = 0; i < regindexs.Length; i++)
                {
                    dataGridView7.Rows[i].Cells[2].Value = values[(int)regindexs.GetValue(i)];
                }
            }

            else
            {
                for (int i = 0; i < regindexs.Length; i++)
                {
                    dataGridView7.Rows[i].Cells[2].Value = "";
                }
            }


            //更新单元4
            if (LineMainForm.unitplc4.GetOnlineState())
            {
                int ModbusRegNum = (int)UnitPLC.REGINDEX.复位完成反馈 + 1;
                if (ModbusRegNum > 120 && ModbusRegNum < 240)
                {
                    int[] values1 = new int[ModbusRegNum];
                    bool res1 = LineMainForm.unitplc4.ReadMutiRegisters(0, values1.Length, out values1);
                    for (int i = 0; i < regindexs.Length; i++)
                    {

                        dataGridView8.Rows[i].Cells[2].Value = values1[(int)regindexs.GetValue(i) - 120];
                        //Console.WriteLine("{0}, {1}", i, (int)regindexs.GetValue(i) - 120 );

                    }
                }
                else
                {
                    int[] values = new int[ModbusRegNum];
                    bool res1 = LineMainForm.unitplc4.ReadMutiRegisters(0, values.Length, out values);
                    for (int i = 0; i < regindexs.Length; i++)
                    {
                        dataGridView8.Rows[i].Cells[2].Value = values[(int)regindexs.GetValue(i)];
                    }
                }
            }
            else
            {
                for (int i = 0; i < regindexs.Length; i++)
                {
                    dataGridView8.Rows[i].Cells[2].Value = "";
                }
            }

        }
        private void UpdateControlPLCSingnal()
        {
            ControlPLC.REGINDEX e0 = new ControlPLC.REGINDEX();
            var regindexs = Enum.GetValues(e0.GetType());

            if (LineMainForm.controlplc.GetOnlineState())
            {
                int ModbusRegNum = (int)ControlPLC.REGINDEX.MES复位 + 1;




                if (ModbusRegNum > 360 && ModbusRegNum < 480)
                {
                    int[] values1 = new int[120];
                    int[] values2 = new int[120];
                    int[] values3 = new int[120];
                    int[] values4 = new int[ModbusRegNum - 360];
                    bool res1 = LineMainForm.controlplc.ReadMutiRegisters(0, values1.Length, out values1);
                    bool res2 = LineMainForm.controlplc.ReadMutiRegisters(120, values2.Length, out values2);
                    bool res3 = LineMainForm.controlplc.ReadMutiRegisters(240, values3.Length, out values3);
                    bool res4 = LineMainForm.controlplc.ReadMutiRegisters(360, values4.Length, out values4);
                    for (int i = 0; i < regindexs.Length; i++)
                    {
                        var k = (int)regindexs.GetValue(i);
                        if (k < 120)
                        {
                            dataGridView1.Rows[i].Cells[2].Value = values1[k];
                            //Console.WriteLine("{0}, {1}", i, (int)regindexs.GetValue(i));
                        }
                        else if (k < 240)
                        {
                            dataGridView1.Rows[i].Cells[2].Value = values2[k - 120];
                            //Console.WriteLine("{0}, {1}", i, (int)regindexs.GetValue(i) - 120 );
                        }
                        else if (k < 360)
                        {
                            dataGridView1.Rows[i].Cells[2].Value = values3[k - 240];
                            //Console.WriteLine("{0}, {1}", i, (int)regindexs.GetValue(i) - 120 );
                        }
                        else if (k < 480)
                        {
                            dataGridView1.Rows[i].Cells[2].Value = values4[k - 360];
                            //Console.WriteLine("{0}, {1}", i, (int)regindexs.GetValue(i) - 120 );
                        }

                    }
                }
                else if (ModbusRegNum > 240 && ModbusRegNum < 360)
                {
                    int[] values1 = new int[120];
                    int[] values2 = new int[120];
                    int[] values3 = new int[ModbusRegNum - 240];
                    bool res1 = LineMainForm.controlplc.ReadMutiRegisters(0, values1.Length, out values1);
                    bool res2 = LineMainForm.controlplc.ReadMutiRegisters(120, values2.Length, out values2);
                    bool res3 = LineMainForm.controlplc.ReadMutiRegisters(120, values3.Length, out values3);
                    for (int i = 0; i < regindexs.Length; i++)
                    {
                        var k = (int)regindexs.GetValue(i);
                        if (k < 120)
                        {
                            dataGridView1.Rows[i].Cells[2].Value = values1[k];
                            //Console.WriteLine("{0}, {1}", i, (int)regindexs.GetValue(i));
                        }
                        else if (k < 240)
                        {
                            dataGridView1.Rows[i].Cells[2].Value = values2[k - 120];
                            //Console.WriteLine("{0}, {1}", i, (int)regindexs.GetValue(i) - 120 );
                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells[2].Value = values3[k - 240];
                            //Console.WriteLine("{0}, {1}", i, (int)regindexs.GetValue(i) - 120 );
                        }
                    }
                }
                else if (ModbusRegNum > 120 && ModbusRegNum < 240)
                {
                    int[] values1 = new int[120];
                    int[] values2 = new int[ModbusRegNum - 120];
                    bool res1 = LineMainForm.controlplc.ReadMutiRegisters(0, values1.Length, out values1);
                    bool res2 = LineMainForm.controlplc.ReadMutiRegisters(120, values2.Length, out values2);
                    for (int i = 0; i < regindexs.Length; i++)
                    {
                        var k = (int)regindexs.GetValue(i);
                        if (k < 120)
                        {
                            dataGridView1.Rows[i].Cells[2].Value = values1[k];
                            //Console.WriteLine("{0}, {1}", i, (int)regindexs.GetValue(i));
                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells[2].Value = values2[k - 120];
                            //Console.WriteLine("{0}, {1}", i, (int)regindexs.GetValue(i) - 120 );
                        }
                    }
                }
                else
                {
                    int[] values = new int[ModbusRegNum];
                    bool res1 = LineMainForm.controlplc.ReadMutiRegisters(0, values.Length, out values);
                    for (int i = 0; i < regindexs.Length; i++)
                    {
                        dataGridView1.Rows[i].Cells[2].Value = values[(int)regindexs.GetValue(i)];
                    }
                }
            }
            else
            {
                for (int i = 0; i < regindexs.Length; i++)
                {
                    dataGridView1.Rows[i].Cells[2].Value = "";
                }
            }
        }

        private void UpdateWMSPLCSingnal()
        {
            WMSPLC.DATAINDEX e0 = new WMSPLC.DATAINDEX();
            var dataindexs = Enum.GetValues(e0.GetType());
            if (LineMainForm.wmsplc.GetOnlineState())
            {
                for (int i = 0; i < dataindexs.Length; i++)
                {
                    dataGridView3.Rows[i].Cells[2].Value = GetWMSPLCData(i);
                }
            }
            else
            {
                for (int i = 0; i < dataindexs.Length; i++)
                {
                    dataGridView3.Rows[i].Cells[2].Value = "";
                }
            }
        }

        string GetWMSPLCData(int index)
        {
            string res = "";
            int dvalue;
            bool bvalue;
            LineMainForm.wmsplc.ReadsingleRegister(index, out dvalue);
            res = dvalue.ToString();                  
            return res;
        }






        private void DataGridView1_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }

        private void DataGridView2_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }

        private void DataGridView3_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }

        private void DataGridView4_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }

        private void PLCForm_Load(object sender, EventArgs e)
        {
            //Task.Run(() => AlarmTick());
        }

        private void dataGridView6_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }

        private void dataGridView5_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }

        private void dataGridView7_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }

        private void dataGridView8_SizeChanged(object sender, EventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            gridView.ClearSelection();
        }
    }
}
