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
using System.Windows.Forms.DataVisualization.Charting;

namespace SCADA.NewApp
{
    public partial class StatisticsForm : Form
    {
        public StatisticsForm()
        {
            InitializeComponent();
            InitUI();
            Task.Run(() => AutoDoWork());
        }

        private string[] unitStates = { "空仓位", "全毛坯", "全合格", "有不合格" };
        Color[] unitColors = { Color.White, Color.Silver, Color.GreenYellow, Color.LightCoral };

        private string[] pieceTypes = { "毛坯A", "毛坯B", "工件A", "工件B" };
        Color[] pieceColors = { Color.LightBlue, Color.DarkBlue, Color.Beige, Color.Olive };

        private string[] productTypes = { "工件A", "工件B" };

        Color[] productColors = { Color.Beige, Color.Olive };

        private void InitUI()
        {
            int[] counts1 = { 35, 0, 0, 0 };
            InitPie(chart1, "当前料库仓位状态统计饼图", unitStates, unitColors, counts1);
            int[] counts2 = { 0, 0, 0, 0};
            InitColumn(chart2, "当前料库物料统计条形图", pieceTypes, pieceColors, counts2);
            int[] counts3 = { 0, 0,};
            InitColumn(chart3, "当日产线加工产品统计条形图", productTypes, productColors, counts3);
        }

        private void InitPie(Chart chart, string Title, string[] names, Color[] colors, int[] counts)
        {
            chart.InvokeEx(c =>
            {
                c.ChartAreas.Clear();
                ChartArea chartArea1 = new ChartArea("C1");
                c.ChartAreas.Add(chartArea1);
                //清除默认的series
                c.Series.Clear();

                Series Count = new Series("工件数量");
                c.Series.Add(Count);

                //标题
                c.Titles.Add(Title);
                c.Titles[0].ForeColor = Color.Black;
                c.Titles[0].Font = new Font("微软雅黑", 14f, FontStyle.Regular);
                c.Titles[0].Alignment = ContentAlignment.TopCenter;
                //控件背景
                c.BackColor = Color.LightGray;
                //图表区背景
                c.ChartAreas[0].BackColor = Color.Wheat;
                c.ChartAreas[0].BorderColor = Color.LightGray;
                //开启三维模式的原因是为了避免标签重叠
                c.ChartAreas[0].Area3DStyle.Enable3D = true;//开启三维模式;PointDepth:厚度BorderWidth:边框宽
                c.ChartAreas[0].Area3DStyle.Rotation = 15;//起始角度
                c.ChartAreas[0].Area3DStyle.Inclination = 45;//倾斜度(0～90)
                c.ChartAreas[0].Area3DStyle.LightStyle = LightStyle.Realistic;//表面光泽度

                //X轴标签间距
                c.ChartAreas[0].AxisX.Interval = 1;
                c.ChartAreas[0].AxisX.LabelStyle.IsStaggered = true;
                c.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                c.ChartAreas[0].AxisX.TitleFont = new Font("微软雅黑", 14f, FontStyle.Regular);
                c.ChartAreas[0].AxisX.TitleForeColor = Color.Black;
                //X坐标轴颜色
                c.ChartAreas[0].AxisX.LineColor = ColorTranslator.FromHtml("#38587a"); ;
                c.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
                c.ChartAreas[0].AxisX.LabelStyle.Font = new Font("微软雅黑", 10f, FontStyle.Regular);
                //X坐标轴标题
                c.ChartAreas[0].AxisX.Title = "数量";
                c.ChartAreas[0].AxisX.TitleFont = new Font("微软雅黑", 10f, FontStyle.Regular);
                c.ChartAreas[0].AxisX.TitleForeColor = Color.Black;
                c.ChartAreas[0].AxisX.TextOrientation = TextOrientation.Horizontal;
                c.ChartAreas[0].AxisX.ToolTip = "数量";
                //X轴网络线条
                c.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                c.ChartAreas[0].AxisX.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");

                //Y坐标轴颜色
                c.ChartAreas[0].AxisY.LineColor = ColorTranslator.FromHtml("#38587a");
                c.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Black;
                c.ChartAreas[0].AxisY.LabelStyle.Font = new Font("微软雅黑", 10f, FontStyle.Regular);
                //Y坐标轴标题
                c.ChartAreas[0].AxisY.Title = "数量";
                c.ChartAreas[0].AxisY.TitleFont = new Font("微软雅黑", 10f, FontStyle.Regular);
                c.ChartAreas[0].AxisY.TitleForeColor = Color.Black;
                c.ChartAreas[0].AxisY.TextOrientation = TextOrientation.Rotated270;
                c.ChartAreas[0].AxisY.ToolTip = "数量";
                //Y轴网格线条
                c.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                c.ChartAreas[0].AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");

                c.ChartAreas[0].AxisY2.LineColor = Color.Transparent;

                //背景渐变
                c.ChartAreas[0].BackGradientStyle = GradientStyle.None;
                //图例样式
                Legend legend2 = new Legend("#VALX");
                legend2.Title = "图例";
                legend2.TitleBackColor = Color.Transparent;
                legend2.BackColor = Color.Transparent;
                legend2.TitleForeColor = Color.Black;
                legend2.TitleFont = new Font("微软雅黑", 14f, FontStyle.Regular);
                legend2.Font = new Font("微软雅黑", 12f, FontStyle.Regular);
                legend2.ForeColor = Color.Black;

                c.Series[0].XValueType = ChartValueType.String;  //设置X轴上的值类型
                c.Series[0].Label = "#VAL";                //设置显示X Y的值    
                c.Series[0].LabelForeColor = Color.Black;
                c.Series[0].ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
                c.Series[0].ChartType = SeriesChartType.Pie;    //图类型(饼图)

                c.Series[0].Color = Color.Lime;
                c.Series[0].LegendText = legend2.Name;
                c.Series[0].IsValueShownAsLabel = true;
                c.Series[0].LabelForeColor = Color.Black;
                c.Series[0].CustomProperties = "DrawingStyle = Cylinder";
                c.Series[0].CustomProperties = "PieLabelStyle = Outside";
                c.Legends.Add(legend2);
                c.Legends[0].Position.Auto = true;
                c.Series[0].IsValueShownAsLabel = true;
                //是否显示图例
                c.Series[0].IsVisibleInLegend = true;
                c.Series[0].ShadowOffset = 0;

                //饼图折线
                c.Series[0]["PieLineColor"] = "Black";
                //绑定数据

                c.Series[0].Points.DataBindXY(names, counts);
                for (int i = 0; i < names.Length; i++)
                {
                    //c.Series[0].Points.AddXY(names[i], Convert.ToString(counts[i]));
                    c.Series[0].Points[i].Color = colors[i];
                }
                //绑定颜色
                c.Series[0].Palette = ChartColorPalette.BrightPastel;
            });
        }

        private void InitColumn(Chart chart, string Title, string[] names, Color[] colors, int[] counts)
        {
            chart.InvokeEx(c =>
            {
                c.ChartAreas.Clear();
                ChartArea chartArea1 = new ChartArea("C1");
                c.ChartAreas.Add(chartArea1);
                //清除默认的series
                c.Series.Clear();

                Legend legend = new Legend("legend");
                legend.Title = "legendTitle";

                Series Count = new Series("工件数量");

                Count.XValueType = ChartValueType.String;  //设置X轴上的值类型
                Count.Label = "#VAL";                //设置显示X Y的值    
                Count.LabelForeColor = Color.Black;
                Count.ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
                Count.ChartType = SeriesChartType.Column;    //图类型(柱状)
                Count.Color = Color.Lime;
                //Count.LegendText = legend.Name;
                Count.IsValueShownAsLabel = true;
                Count.CustomProperties = "DrawingStyle = Cylinder";
                Count.Palette = ChartColorPalette.Bright;

                //标题
                c.Titles.Add(Title);
                c.Titles[0].ForeColor = Color.Black;
                c.Titles[0].Font = new Font("微软雅黑", 14f, FontStyle.Regular);
                c.Titles[0].Alignment = ContentAlignment.TopCenter;

                //控件背景
                c.BackColor = Color.LightGray;
                //图表区背景
                c.ChartAreas[0].BackColor = Color.LightGray;
                c.ChartAreas[0].BorderColor = Color.LightGray;

                //X坐标轴颜色
                c.ChartAreas[0].AxisX.LineColor = ColorTranslator.FromHtml("#38587a"); ;
                c.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
                c.ChartAreas[0].AxisX.LabelStyle.Font = new Font("微软雅黑", 12f, FontStyle.Regular);
                //X轴网络线条
                c.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                c.ChartAreas[0].AxisX.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");
                //Y坐标轴颜色
                c.ChartAreas[0].AxisY.LineColor = ColorTranslator.FromHtml("#38587a");
                c.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Black;
                c.ChartAreas[0].AxisY.LabelStyle.Font = new Font("微软雅黑", 12f, FontStyle.Regular);

                //Y轴网格线条
                c.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                c.ChartAreas[0].AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");

                c.ChartAreas[0].AxisY2.LineColor = Color.Transparent;
                c.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;

                //开启三维模式的原因是为了避免标签重叠
                c.ChartAreas[0].Area3DStyle.Enable3D = true;//开启三维模式;PointDepth:厚度BorderWidth:边框宽
                c.ChartAreas[0].Area3DStyle.Rotation = 15;//起始角度
                c.ChartAreas[0].Area3DStyle.Inclination = 30;//倾斜度(0～90)
                c.ChartAreas[0].Area3DStyle.LightStyle = LightStyle.Realistic;//表面光泽度
                c.ChartAreas[0].AxisX.Interval = 1; //决定x轴显示文本的间隔，1为强制每个柱状体都显示，3则间隔3个显示

                Count.Points.DataBindXY(names, counts);
                for (int i = 0; i < names.Length; i++)
                {
                    Count.Points[i].Color = colors[i];
                }

                //把series添加到chart上
                c.Series.Add(Count);
                c.Legends.Add(legend);
                c.Legends[0].Position.Auto = false;  //隐藏工件数量
            });
        }

        private void UpdatePie(Chart chart, string[] names, Color[] colors, int[] counts)
        {
            chart.InvokeEx(c =>
            {
                c.Series[0].Points.DataBindXY(names, counts);
                for (int i = 0; i < names.Length; i++)
                {
                    c.Series[0].Points[i].Color = colors[i];
                }

            });
        }

        private void AutoDoWork()
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
                int[] rackcount = new int[unitStates.Length];//{ "0空仓位", "1全毛坯", "2全合格", "3有不合格"};
                int[] piececount = new int[pieceTypes.Length];// { "0毛坯A ", "1毛坯B ","2工件A", "3工件B"};
                for (int i = 0; i < StockBinForm.listRackData.Count; i++)
                {
                    var rackdata = StockBinForm.listRackData[i];
                    if (rackdata.Tray == TRAYTYPE.空)
                    {
                        rackcount[0]++;
                    }
                    else if (rackdata.Tray == TRAYTYPE.料盘A)
                    {
                        if (rackdata.Piece1Quality == PIECEQUALITY.待检测 && rackdata.Piece2Quality == PIECEQUALITY.待检测 && rackdata.Piece3Quality == PIECEQUALITY.待检测 && rackdata.Piece4Quality == PIECEQUALITY.待检测)
                        {
                            rackcount[1]++;

                        }
                        else if (rackdata.Piece1Quality == PIECEQUALITY.合格 && rackdata.Piece2Quality == PIECEQUALITY.合格 && rackdata.Piece3Quality == PIECEQUALITY.合格 && rackdata.Piece4Quality == PIECEQUALITY.合格)
                        {
                            rackcount[2]++;

                        }
                        else if (rackdata.Piece1Quality == PIECEQUALITY.不合格 || rackdata.Piece2Quality == PIECEQUALITY.不合格 || rackdata.Piece3Quality == PIECEQUALITY.不合格 || rackdata.Piece4Quality == PIECEQUALITY.不合格)

                        {
                            rackcount[3]++;
                        }
                        if (rackdata.Piece1 == PIECETYTPE.毛坯A)
                        {
                            piececount[0]++;
                        }
                        else if (rackdata.Piece1 == PIECETYTPE.工件A)
                        {

                            piececount[2]++;
                        }
                        if (rackdata.Piece2 == PIECETYTPE.毛坯A)
                        {
                            piececount[0]++;
                        }
                        else if (rackdata.Piece2 == PIECETYTPE.工件A)
                        {

                            piececount[2]++;
                        }
                        if (rackdata.Piece3 == PIECETYTPE.毛坯A)
                        {
                            piececount[0]++;
                        }
                        else if (rackdata.Piece3 == PIECETYTPE.工件A)
                        {

                            piececount[2]++;
                        }
                        if (rackdata.Piece4 == PIECETYTPE.毛坯A)
                        {
                            piececount[0]++;
                        }
                        else if (rackdata.Piece4 == PIECETYTPE.工件A)
                        {

                            piececount[2]++;
                        }

                    }
                    else if (rackdata.Tray == TRAYTYPE.料盘B)
                    {
                        if (rackdata.Piece1Quality == PIECEQUALITY.待检测 && rackdata.Piece2Quality == PIECEQUALITY.待检测 && rackdata.Piece3Quality == PIECEQUALITY.待检测 && rackdata.Piece4Quality == PIECEQUALITY.待检测)
                        {
                            rackcount[1]++;

                        }
                        else if (rackdata.Piece1Quality == PIECEQUALITY.合格 && rackdata.Piece2Quality == PIECEQUALITY.合格 && rackdata.Piece3Quality == PIECEQUALITY.合格 && rackdata.Piece4Quality == PIECEQUALITY.合格)
                        {
                            rackcount[2]++;

                        }
                        else if (rackdata.Piece1Quality == PIECEQUALITY.不合格 || rackdata.Piece2Quality == PIECEQUALITY.不合格 || rackdata.Piece3Quality == PIECEQUALITY.不合格 || rackdata.Piece4Quality == PIECEQUALITY.不合格)

                        {
                            rackcount[3]++;
                        }
                        if (rackdata.Piece1 == PIECETYTPE.毛坯B)
                        {
                            piececount[1]++;
                        }
                        else if (rackdata.Piece1 == PIECETYTPE.工件B)
                        {

                            piececount[3]++;
                        }
                        if (rackdata.Piece2 == PIECETYTPE.毛坯B)
                        {
                            piececount[1]++;
                        }
                        else if (rackdata.Piece2 == PIECETYTPE.工件B)
                        {
                            piececount[3]++;
                        }
                        if (rackdata.Piece3 == PIECETYTPE.毛坯B)
                        {
                            piececount[1]++;
                        }
                        else if (rackdata.Piece3 == PIECETYTPE.工件B)
                        {
                            piececount[3]++;
                        }
                        if (rackdata.Piece4 == PIECETYTPE.毛坯B)
                        {
                            piececount[1]++;
                        }
                        else if (rackdata.Piece4 == PIECETYTPE.工件B)
                        {
                            piececount[3]++;
                        }

                    }
                }

                UpdatePie(chart1, unitStates, unitColors, rackcount);
                UpdateChartColumn(chart2, pieceTypes, pieceColors, piececount);

                int[] productcount = new int[productTypes.Length];
                DateTime nowtime = DateTime.Now;
                DateTime start = new DateTime(nowtime.Year, nowtime.Month, nowtime.Day, 0, 0, 0);
                var completelist = Program.Repo.Get<StaticsData>(p => (p.LastModificationTime> start && p.LastModificationTime <= nowtime));
                if (completelist != null)
                {
                    foreach (var pdata in completelist)
                    {
                        switch (pdata.Product)
                        {
                            case PRODUCTTYPE.A料:
                                productcount[0]++;
                                break;
                            case PRODUCTTYPE.B料:
                                productcount[1]++;
                                break;
                        }
                    }
                }
                UpdateChartColumn(chart3, productTypes, productColors, productcount);
            }
        }

        private void UpdateChartColumn(Chart chart, string[] names, Color[] colors, int[] counts)
        {
            chart.InvokeEx(c =>
            {
                chart.Series[0].Points.DataBindXY(names, counts);
                for (int i = 0; i < names.Length; i++)
                {
                    chart.Series[0].Points[i].Color = colors[i];
                }
            });
        }
    }
}
