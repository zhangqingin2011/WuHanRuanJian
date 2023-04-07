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
    public partial class QualityRecordForm : Form
    {
        static int recordpageindex = 1;
        static int Sum = 1;
        private List<WorkOrderData> MeterResultList = new List<WorkOrderData>();//测量结果单前页的展示的10个工件检测结果
        private List<WorkOrderData> MeterResultShowList = new List<WorkOrderData>();//测量结果单前页的展示的10个工件检测结果

        private List<meterrecorddetil> meterrecorddetillist = new List<meterrecorddetil>();
        private List<meterrecorddetil> meterrecorddetilapgelist = new List<meterrecorddetil>();
        private int curpageindex = 0;

        public QualityRecordForm()
        {
            InitializeComponent();
            InitConfig();
        }

        private void InitConfig()
        {
            this.button3.Click += new System.EventHandler(this.button3_Click);
            this.button2.Click += new System.EventHandler(this.button2_Click);
            this.dataGridViewdis1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewdis1_CellContentClick);
            this.buttonP1.Click += new System.EventHandler(this.buttonP1_Click);
            comboBoxtime.SelectedIndex = 0;
            comboBoxtype.SelectedIndex = 0;
            InitdataGridViewdis1();
        }

        private void InitdataGridViewdis1()
        {
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
            RenewdataGridViewdis1(start, now, true, PRODUCTTYPE.无, 0);
            textBoxpageindex.Text = "0";
            labelpagesum.Text = "0";
        }

        private void RenewdataGridViewdis1(DateTime starttime, DateTime endtime, bool IsALLtype, PRODUCTTYPE type, int pageindex)
        {
            meterrecorddetilapgelist.Clear();
            //获取当天的的检测数据
            GetMeterResultShowList(starttime, endtime, true, type);
            Getmeterrecorddetillist(pageindex);

            //更新页码
            int sum = 0;

            if (meterrecorddetillist.Count() == 0)
            {
                dataGridViewdis1.Rows.Clear();
                labelpagesum.Text = "0";
                textBoxpageindex.Text = "0";
                return;
            }

            if (meterrecorddetilapgelist.Count == 0)
            {
                dataGridViewdis1.Rows.Clear();
                return;
            }
            if (meterrecorddetillist.Count() % 10 == 0)
            {
                Sum = meterrecorddetillist.Count() / 10;
            }
            else
            {
                Sum = meterrecorddetillist.Count() / 10 + 1;
            }

            labelpagesum.Text = Sum.ToString();
            textBoxpageindex.Text = pageindex.ToString();
            dataGridViewdis1.Rows.Clear();
            //dataGridViewdis1.Rows.Add(meterrecorddetillist.Count());
            //更新第一页数据
            int index = 0;
            var detil = new meterrecorddetil();
            if (meterrecorddetilapgelist.Count > 0)
            {
                foreach (var temp in meterrecorddetilapgelist)
                {
                    if (index == 0)
                    {
                        detil = temp;
                    }
                    var order = Program.Repo.GetSingleDelet<WorkOrderData>(p => p.Id == temp.OrderId);
                    if(order !=null)
                    {
                        string type1 = "";

                        if (temp.workpieceplace == "1" && order.Product1 != PRODUCTTYPE.无)
                        {
                            type1 = order.Product1.ToString();
                        }
                        if (temp.workpieceplace == "2" && order.Product2 != PRODUCTTYPE.无)
                        {
                            type1 = order.Product2.ToString();
                        }
                        if (temp.workpieceplace == "3" && order.Product3 != PRODUCTTYPE.无)
                        {
                            type1 = order.Product3.ToString();
                        }
                        if (temp.workpieceplace == "4" && order.Product4 != PRODUCTTYPE.无)
                        {
                            type1 = order.Product4.ToString();
                        }
                        dataGridViewdis1.Rows.Add();
                        dataGridViewdis1.Rows[index].Cells[0].Value = index + 1;
                        dataGridViewdis1.Rows[index].Cells[1].Value = temp.orderNo;
                        dataGridViewdis1.Rows[index].Cells[2].Value = temp.workpieceplace;
                        dataGridViewdis1.Rows[index].Cells[3].Value = type1;
                        dataGridViewdis1.Rows[index].Cells[4].Value = temp.testtime;
                        dataGridViewdis1.Rows[index].Cells[5].Value = temp.testresult;
                        dataGridViewdis1.Rows[index].Cells[6].Value = "查看详情";
                        index++;
                    }
    
                }
                if(dataGridViewdis1.Rows.Count>0)
                {
                    dataGridViewdis1.Visible = true;
                    var aorder = Program.Repo.GetSingleDelet<WorkOrderData>(p => p.Id == detil.OrderId);

                    renewdetil(aorder, Convert.ToInt16(detil.workpieceplace));
                }
               
            }
        }

        private void GetMeterResultShowList(DateTime starttime, DateTime endtime, bool IsALLtype, PRODUCTTYPE type)
        {
            var completelist = Program.Repo.GetDelet<WorkOrderData>(p => (p.OrderState == ORDERSTATE.结束 && p.CreationTime > starttime && p.CreationTime < endtime));
            if (completelist.Count() == 0)
            {
                MeterResultShowList.Clear();
                return;
            }
            MeterResultList.Clear();

            //获取完成订单的检测结果
            if (IsALLtype == true)
            {
                foreach (var ordertemp in completelist)
                {
                    if(type == PRODUCTTYPE.无)//选取所有订单
                    {
                         if ((ordertemp.Product1 != PRODUCTTYPE.无 && ordertemp.P1Quality != PIECEQUALITY.待检测)
                     || (ordertemp.Product2 != PRODUCTTYPE.无 && ordertemp.P2Quality != PIECEQUALITY.待检测)
                     || (ordertemp.Product3 != PRODUCTTYPE.无 && ordertemp.P3Quality != PIECEQUALITY.待检测)
                     || (ordertemp.Product4 != PRODUCTTYPE.无 && ordertemp.P4Quality != PIECEQUALITY.待检测))
                        {
                            MeterResultList.Add(ordertemp);
                        }
                    }
                    else if((ordertemp.Product1 == type && (ordertemp.P1Quality == PIECEQUALITY.不合格|| ordertemp.P1Quality == PIECEQUALITY.合格))
                     || (ordertemp.Product2 == type && (ordertemp.P2Quality == PIECEQUALITY.不合格 || ordertemp.P2Quality == PIECEQUALITY.合格))
                     || (ordertemp.Product3 == type && (ordertemp.P3Quality == PIECEQUALITY.不合格 || ordertemp.P3Quality == PIECEQUALITY.合格))
                     || (ordertemp.Product4 == type && (ordertemp.P4Quality == PIECEQUALITY.不合格 || ordertemp.P4Quality == PIECEQUALITY.合格)))
                        {
                        MeterResultList.Add(ordertemp);
                    }

                }
            }
            else
            {
               
                foreach (var ordertemp in completelist)
                {
                    if ((ordertemp.Product1 != type && (ordertemp.P1Quality == PIECEQUALITY.不合格 || ordertemp.P1Quality == PIECEQUALITY.合格))
                      || (ordertemp.Product2 != type && (ordertemp.P2Quality == PIECEQUALITY.不合格 || ordertemp.P2Quality == PIECEQUALITY.合格))
                      || (ordertemp.Product3 != type && (ordertemp.P3Quality == PIECEQUALITY.不合格 || ordertemp.P3Quality == PIECEQUALITY.合格))
                      || (ordertemp.Product4 != type && (ordertemp.P4Quality == PIECEQUALITY.不合格 || ordertemp.P4Quality == PIECEQUALITY.合格))
                      )
                    {
                        MeterResultList.Add(ordertemp);
                    }
                }
            }

            //更新第一页数据
            MeterResultShowList.Clear();
            if(MeterResultList.Count > 0)
            {
                MeterResultShowList.AddRange(MeterResultList);
            }
              //  MeterResultShowList.Add(MeterResultList.ElementAt(0));
        }

        private void Getmeterrecorddetillist(int pageindex)
        {
            meterrecorddetillist.Clear();
            foreach (var temp in MeterResultShowList)
            {
                var ordertemp = temp;
                ICollection<MeterResult> meterresultlist = null;
                var temp1 = Program.Repo.GetSingleDelet<MeterResult>(p => p.OrderId == ordertemp.Id && p.WorkpieceNumber == 1 && p.Rank == 1);
                var temp2 = Program.Repo.GetSingleDelet<MeterResult>(p => p.OrderId == ordertemp.Id && p.WorkpieceNumber == 2 && p.Rank == 1);
                var temp3 = Program.Repo.GetSingleDelet<MeterResult>(p => p.OrderId == ordertemp.Id && p.WorkpieceNumber == 3 && p.Rank == 1);
                var temp4 = Program.Repo.GetSingleDelet<MeterResult>(p => p.OrderId == ordertemp.Id && p.WorkpieceNumber == 4 && p.Rank == 1);

             
                if (temp1 != null)
                {
                    meterrecorddetil adetil = new meterrecorddetil();
                    adetil.OrderId = ordertemp.Id;
                    adetil.orderNo = ordertemp.OrderNO.ToString();
                    adetil.testresult = ordertemp.P1Quality.ToString();
                    adetil.workpieceplace = "1";
                    adetil.testtime = ordertemp.CreationTime.ToString();
                    meterrecorddetillist.Add(adetil);
                }
                if (temp2 != null)
                {
                    meterrecorddetil adetil = new meterrecorddetil();
                    adetil.OrderId = ordertemp.Id;
                    adetil.orderNo = ordertemp.OrderNO.ToString();
                    adetil.testresult = ordertemp.P2Quality.ToString();
                    adetil.workpieceplace = "2";
                    adetil.testtime = ordertemp.CreationTime.ToString();
                    meterrecorddetillist.Add(adetil);
                }
                if (temp3 != null)
                {
                    meterrecorddetil adetil = new meterrecorddetil();
                    adetil.OrderId = ordertemp.Id;
                    adetil.orderNo = ordertemp.OrderNO.ToString();
                    adetil.testresult = ordertemp.P3Quality.ToString();
                    adetil.workpieceplace = "3";
                    adetil.testtime = ordertemp.CreationTime.ToString();
                    meterrecorddetillist.Add(adetil);
                }
                if (temp4 != null)
                {
                    meterrecorddetil adetil = new meterrecorddetil();
                    adetil.OrderId = ordertemp.Id;
                    adetil.orderNo = ordertemp.OrderNO.ToString();
                    adetil.testresult = ordertemp.P4Quality.ToString();
                    adetil.workpieceplace = "4";
                    adetil.testtime = ordertemp.CreationTime.ToString();
                    meterrecorddetillist.Add(adetil);
                }
            }
            if (pageindex > 0)
            {
                if (meterrecorddetillist.Count() > (pageindex - 1) * 10)
                {
                    for (int ii = (pageindex - 1) * 10; ii < pageindex * 10; ii++)
                    {
                        if (meterrecorddetillist.Count > ii)
                        {
                            var t = meterrecorddetillist.ElementAt(ii);
                            meterrecorddetilapgelist.Add(t);
                        }
                    }
                }
            }
            else
            {
                return;
            }
           
        }

        private void renewdetil(WorkOrderData aorder, Int16 place)
        {
            PRODUCTTYPE Product = PRODUCTTYPE.无;
            if (place == 1)
            {
                Product = aorder.Product1;
            }
            else if (place == 2)
            {
                Product = aorder.Product2;
            }
            else if (place == 3)
            {
                Product = aorder.Product3;
            }
            else if (place == 4)
            {
                Product = aorder.Product4;
            }
            else
            {
                return;
            }
            var alist = Program.Repo.Get<MeterResult>(p => p.OrderId == aorder.Id && p.WorkpieceNumber == place);
            if (alist.Count() == 0)
            {
                return;
            }
            dataGridViewdetail.Rows.Clear();
            dataGridViewdetail.Rows.Add(alist.Count);
            for (int index = 0; index < alist.Count; index++)
            {
                var arow = Program.Repo.GetSingle<MeterResult>(p => p.OrderId == aorder.Id && p.WorkpieceNumber == place && p.Rank == index + 1);
                var sizesub = arow.SizeValue - arow.SizeDataStandard;
                dataGridViewdetail.Rows[index].Cells[0].Value = index + 1;
                // dataGridVieworder.Rows[index].Cells[0].Value = dataGridVieworder.Rows.Count - 1;
                dataGridViewdetail.Rows[index].Cells[1].Value = aorder.OrderNO.ToString();
                dataGridViewdetail.Rows[index].Cells[2].Value = place.ToString();
                dataGridViewdetail.Rows[index].Cells[3].Value = arow.ModeSN.ToString();
                dataGridViewdetail.Rows[index].Cells[4].Value = arow.SizeDataStandard.ToString();
                dataGridViewdetail.Rows[index].Cells[5].Value = arow.SizeDataMax.ToString();
                dataGridViewdetail.Rows[index].Cells[6].Value = arow.SizeDataMin.ToString();
                dataGridViewdetail.Rows[index].Cells[7].Value = arow.SizeValue.ToString();
                dataGridViewdetail.Rows[index].Cells[8].Value = sizesub.ToString("F4");
                if (arow.IsCertificate == true)
                {
                    dataGridViewdetail.Rows[index].Cells[9].Value = "合格";
                }
                else
                {
                    dataGridViewdetail.Rows[index].Cells[9].Value = "不合格";
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (recordpageindex > 1)
            {
                recordpageindex--;
            }
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

            switch (comboBoxtime.SelectedIndex)
            {
                case 0://本天
                    start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    break;
                case 1://本周

                    DateTime dt = DateTime.Now;
                    int dayOfWeek = -1 * (int)dt.Date.DayOfWeek;
                    //Sunday = 0,Monday = 1,Tuesday = 2,Wednesday = 3,Thursday = 4,Friday = 5,Saturday = 6,

                    DateTime weekStartTime = dt.AddDays(dayOfWeek + 1);//取本周一
                    if (dayOfWeek == 0) //如果今天是周日，则开始时间是上周一
                    {
                        weekStartTime = weekStartTime.AddDays(-7);
                    }
                    start = weekStartTime.Date;
                    break;
                case 2://本月

                    DateTime dt1 = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    start = now.AddDays(-dt1.Day + 1);
                    break;
                case 3://本年

                    DateTime dt2= new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    start = now.AddDays(-dt2.DayOfYear + 1);
                    break;
            }
            var type = PRODUCTTYPE.无;
            switch (comboBoxtype.SelectedIndex)
            {
                case 0://
                    type = PRODUCTTYPE.A料;
                    break;
                case 1://
                    type = PRODUCTTYPE.B料;
                    break;
                case 2://
                    type = PRODUCTTYPE.无; 
                    break;
                default:
                    return;

            }
            if (type == PRODUCTTYPE.无)
            {
                RenewdataGridViewdis1(start, now, true, type, recordpageindex);
            }
            else RenewdataGridViewdis1(start, now, false, type, recordpageindex);
        }

        private void buttonP1_Click(object sender, EventArgs e)
        {
           recordpageindex = 1;
           
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

            switch (comboBoxtime.SelectedIndex)
            {
                case 0://本天
                    start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    break;
                case 1://本周

                    DateTime dt = DateTime.Now;
                    int dayOfWeek = -1 * (int)dt.Date.DayOfWeek;
                    //Sunday = 0,Monday = 1,Tuesday = 2,Wednesday = 3,Thursday = 4,Friday = 5,Saturday = 6,

                    DateTime weekStartTime = dt.AddDays(dayOfWeek + 1);//取本周一
                    if (dayOfWeek == 0) //如果今天是周日，则开始时间是上周一
                    {
                        weekStartTime = weekStartTime.AddDays(-7);
                    }
                    start = weekStartTime.Date;
                    break;
                case 2://本月
                    DateTime dt1 = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    //本月第一天时间      
                    start = dt1.AddDays(1 - (dt1.Day));
                    break;
                case 3://本年
                    DateTime dt2= new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    start = now.AddDays(1- (dt2 .DayOfYear));
                    break;
            }
            var type = PRODUCTTYPE.无;
            switch (comboBoxtype.SelectedIndex)
            {
                case 0://全部
                    type = PRODUCTTYPE.A料;
                    break;
                case 1://
                    type = PRODUCTTYPE.B料;
                    break;
                case 2://
                    type = PRODUCTTYPE.无;
                    break;
                default:
                    return;
            }

            RenewdataGridViewdis1(start, now, false, type, recordpageindex);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (recordpageindex >= 1 && recordpageindex < Sum)
            {
                recordpageindex++;
            }
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

            switch (comboBoxtime.SelectedIndex)
            {
                case 0://本天
                    start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    break;
                case 1://本周

                    DateTime dt = DateTime.Now;
                    int dayOfWeek = -1 * (int)dt.Date.DayOfWeek;
                    //Sunday = 0,Monday = 1,Tuesday = 2,Wednesday = 3,Thursday = 4,Friday = 5,Saturday = 6,

                    DateTime weekStartTime = dt.AddDays(dayOfWeek + 1);//取本周一
                    if (dayOfWeek == 0) //如果今天是周日，则开始时间是上周一
                    {
                        weekStartTime = weekStartTime.AddDays(-7);
                    }
                    start = weekStartTime.Date;
                    break;
                case 2://本月
                    DateTime dt1 = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    //本月第一天时间      
                    start = dt1.AddDays(1 - (dt1.Day));
                    break;
                case 3://本年
                    DateTime dt2 = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                    start = now.AddDays(1 - (dt2.DayOfYear));
                    break;
            }
            var type = PRODUCTTYPE.无;
            switch (comboBoxtype.SelectedIndex)
            {
                case 0://全部
                    type = PRODUCTTYPE.A料;
                    break;
                case 1://
                    type = PRODUCTTYPE.B料;
                    break;
                case 2://
                    type = PRODUCTTYPE.无;
                    break;
                default:
                    return;
            }

            RenewdataGridViewdis1(start, now, false, type, recordpageindex);
        }

        private void dataGridViewdis1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridViewdis1.Rows.Count)
            {
                DataGridViewColumn column = dataGridViewdis1.Columns[e.ColumnIndex];
                if (column is DataGridViewButtonColumn)
                {
                    //行列都是0开始计算，表头不算， MessageBox.Show("x: " + e.RowIndex.ToString() + ",y:" + e.ColumnIndex.ToString());
                    if (e.ColumnIndex == 6)//订单派发按钮按下//BackColor = System.Drawing.Color.LightGreen;
                    {
                        var detil = meterrecorddetilapgelist.ElementAt(e.RowIndex);
                        var aorder = Program.Repo.GetSingleDelet<WorkOrderData>(p => p.Id == detil.OrderId);
                        var place = Convert.ToInt16(detil.workpieceplace);

                        renewdetil(aorder, place);
                    }
                }
            }
        }

    
        private void textBoxpageindex_Leave(object sender, EventArgs e)
        {
            try
            {
                var index = Convert.ToInt32(textBoxpageindex.Text);
                if (index == null)
                {

                    textBoxpageindex.Text = recordpageindex.ToString();
                }
                else
                {
                    recordpageindex = index;
                    if (index >= 1 && index <=Sum)
                    {
                        recordpageindex=index;
                    }
                    else
                    {
                        textBoxpageindex.Text = recordpageindex.ToString();
                        return;
                    }
                    DateTime now = DateTime.Now;
                    DateTime start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

                    switch (comboBoxtime.SelectedIndex)
                    {
                        case 0://本天
                            start = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                            break;
                        case 1://本周

                            DateTime dt = DateTime.Now;
                            int dayOfWeek = -1 * (int)dt.Date.DayOfWeek;
                            //Sunday = 0,Monday = 1,Tuesday = 2,Wednesday = 3,Thursday = 4,Friday = 5,Saturday = 6,

                            DateTime weekStartTime = dt.AddDays(dayOfWeek + 1);//取本周一
                            if (dayOfWeek == 0) //如果今天是周日，则开始时间是上周一
                            {
                                weekStartTime = weekStartTime.AddDays(-7);
                            }
                            start = weekStartTime.Date;
                            break;
                        case 2://本月
                            DateTime dt1 = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                            //本月第一天时间      
                            start = dt1.AddDays(1 - (dt1.Day));
                            break;
                        case 3://本年
                            DateTime dt2 = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                            start = now.AddDays(1 - (dt2.DayOfYear));
                            break;
                    }
                    var type = PRODUCTTYPE.无;
                    switch (comboBoxtype.SelectedIndex)
                    {
                        case 0://全部
                            type = PRODUCTTYPE.A料;
                            break;
                        case 1://
                            type = PRODUCTTYPE.B料;
                            break;
                        case 2://
                            type = PRODUCTTYPE.无;
                            break;
                        default:
                            return;
                    }

                    RenewdataGridViewdis1(start, now, false, type, recordpageindex);
                }
            }
            catch
            {

                textBoxpageindex.Text = recordpageindex.ToString();

            }
        }

     
    }

    public class meterrecorddetil
    {
        public Guid OrderId;
        public string orderNo;
        public string workpieceplace;
        public string testtime;
        public string testresult;
    }
}
