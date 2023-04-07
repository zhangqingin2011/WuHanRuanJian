using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCADA.NewApp
{
    public partial class QualityShowForm : Form
    {
        private int  orderNo = 0; 
        private int pieceNo = 0;
        public QualityShowForm()
        {
            InitializeComponent();
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //MeasureForm.MeasureOrderNO = workOrderTasks[i].OrderNO;
            //MeasureForm.MeasurePicecNo = 2;

            if (MeasureForm.MeasureOrderNO == 0 ||MeasureForm.MeasurePicecNo ==0 )
            {
                return;
            }
            var t = Program.Repo.GetSingle<WorkOrderData>(p => p.OrderNO == MeasureForm.MeasureOrderNO && p.IsDeleted == false);
            if (t == null)
            {
                return;
            }
            if (orderNo == MeasureForm.MeasureOrderNO && pieceNo == MeasureForm.MeasurePicecNo)
            {
                return;
            }
            else
            {
                dataGridView1.Rows.Clear();
                orderNo = MeasureForm.MeasureOrderNO;
                int count = 0;
                if (t.Product1!= PRODUCTTYPE.无&& t.P1Quality != PIECEQUALITY.待检测)
                {
                    var list1 = Program.Repo.Get<MeterResult>(p => p.OrderId == t.Id && p.WorkpieceNumber == 1);
                    if (list1.Count > 0)
                    {
                        pieceNo = 1;
                        dataGridView1.Rows.Add(list1.Count);
                        foreach (var temp in list1)
                        {
                            int xuhao = count + 1;
                            dataGridView1.Rows[count].Cells[0].Value = xuhao.ToString();
                            dataGridView1.Rows[count].Cells[1].Value = t.OrderNO.ToString();
                            dataGridView1.Rows[count].Cells[2].Value = 1;
                            dataGridView1.Rows[count].Cells[3].Value = t.Product1.ToString();
                            dataGridView1.Rows[count].Cells[4].Value = temp.SizeDataStandard.ToString();
                            dataGridView1.Rows[count].Cells[5].Value = temp.SizeDataMax.ToString();
                            dataGridView1.Rows[count].Cells[6].Value = temp.SizeDataMin.ToString();
                            dataGridView1.Rows[count].Cells[7].Value = temp.SizeValue.ToString();
                            dataGridView1.Rows[count].Cells[8].Value = temp.IsCertificate.ToString();
                            if (temp.IsCertificate == true)
                            {
                                dataGridView1.Rows[count].Cells[8].Value = "合格";
                            }
                            else
                            {
                                dataGridView1.Rows[count].Cells[8].Value = "不合格";
                            }
                            count++;
                        }
                    }
                }
                if (t.Product2 != PRODUCTTYPE.无&&t.P2Quality != PIECEQUALITY.待检测)
                {

                    pieceNo = 2;
                    var list1 = Program.Repo.Get<MeterResult>(p => p.OrderId == t.Id && p.WorkpieceNumber == 2);
                    if (list1.Count > 0)
                    {
                        dataGridView1.Rows.Add(list1.Count);
                        foreach (var temp in list1)
                        {
                            int xuhao = count + 1;
                            dataGridView1.Rows[count].Cells[0].Value = xuhao.ToString();
                            dataGridView1.Rows[count].Cells[1].Value = t.OrderNO.ToString();
                            dataGridView1.Rows[count].Cells[2].Value = 2;
                            dataGridView1.Rows[count].Cells[3].Value = t.Product2.ToString();
                            dataGridView1.Rows[count].Cells[4].Value = temp.SizeDataStandard.ToString();
                            dataGridView1.Rows[count].Cells[5].Value = temp.SizeDataMax.ToString();
                            dataGridView1.Rows[count].Cells[6].Value = temp.SizeDataMin.ToString();
                            dataGridView1.Rows[count].Cells[7].Value = temp.SizeValue.ToString();
                            if (temp.IsCertificate == true)
                            {
                                dataGridView1.Rows[count].Cells[8].Value = "合格";
                            }
                            else
                            {
                                dataGridView1.Rows[count].Cells[8].Value = "不合格";
                            }
                            count++;
                        }
                    }
                }
                if (t.Product3 != PRODUCTTYPE.无&&t.P3Quality != PIECEQUALITY.待检测)
                {

                    pieceNo = 3;
                    var list1 = Program.Repo.Get<MeterResult>(p => p.OrderId == t.Id && p.WorkpieceNumber == 3);
                    if (list1.Count > 0)
                    {
                        dataGridView1.Rows.Add(list1.Count);
                        foreach (var temp in list1)
                        {
                            int xuhao = count + 1;
                            dataGridView1.Rows[count].Cells[0].Value = xuhao.ToString();
                            dataGridView1.Rows[count].Cells[1].Value = t.OrderNO.ToString();
                            dataGridView1.Rows[count].Cells[2].Value = 3;
                            dataGridView1.Rows[count].Cells[3].Value = t.Product3.ToString();
                            dataGridView1.Rows[count].Cells[4].Value = temp.SizeDataStandard.ToString();
                            dataGridView1.Rows[count].Cells[5].Value = temp.SizeDataMax.ToString();
                            dataGridView1.Rows[count].Cells[6].Value = temp.SizeDataMin.ToString();
                            dataGridView1.Rows[count].Cells[7].Value = temp.SizeValue.ToString();
                            if (temp.IsCertificate == true)
                            {
                                dataGridView1.Rows[count].Cells[8].Value = "合格";
                            }
                            else
                            {
                                dataGridView1.Rows[count].Cells[8].Value = "不合格";
                            }
                            count++;
                        }
                    }
                }
                if (t.Product4 != PRODUCTTYPE.无&&t.P4Quality != PIECEQUALITY.待检测)
                {

                    pieceNo = 4;
                    var list1 = Program.Repo.Get<MeterResult>(p => p.OrderId == t.Id && p.WorkpieceNumber == 4);
                    if (list1.Count > 0)
                    {
                        dataGridView1.Rows.Add(list1.Count);
                        foreach (var temp in list1)
                        {
                            int xuhao = count + 1;
                            dataGridView1.Rows[count].Cells[0].Value = xuhao.ToString();
                            dataGridView1.Rows[count].Cells[1].Value = t.OrderNO.ToString();
                            dataGridView1.Rows[count].Cells[2].Value = 4;
                            dataGridView1.Rows[count].Cells[3].Value = t.Product4.ToString();
                            dataGridView1.Rows[count].Cells[4].Value = temp.SizeDataStandard.ToString();
                            dataGridView1.Rows[count].Cells[5].Value = temp.SizeDataMax.ToString();
                            dataGridView1.Rows[count].Cells[6].Value = temp.SizeDataMin.ToString();
                            dataGridView1.Rows[count].Cells[7].Value = temp.SizeValue.ToString();
                            if (temp.IsCertificate == true)
                            {
                                dataGridView1.Rows[count].Cells[8].Value = "合格";
                            }
                            else
                            {
                                dataGridView1.Rows[count].Cells[8].Value = "不合格";
                            }
                            count++;
                        }
                    }
                }
            }
        }

  
    }
}
