using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace SCADA.NewApp
{
    class TongJiLei
    {
        //static MySQLOperate MysqlOperator = MySQLOperate.GetInstance();

        //public MySQLOperate GetMysqlOperator()
        //{
            //return MysqlOperator;
        //}
        public string[] PieceName = { "圆盘", "底座", "装配件" };
        public void InitColumn(Chart chart, string Title, int count1 = 0, int count2 = 0, int count3 = 0)
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
                c.Titles[0].Font = new Font("微软雅黑", 12f, FontStyle.Regular);
                c.Titles[0].Alignment = ContentAlignment.TopCenter;

                //控件背景
                c.BackColor = Color.LightGray;
                //图表区背景
                c.ChartAreas[0].BackColor = Color.LightGray;
                c.ChartAreas[0].BorderColor = Color.LightGray;

                //X坐标轴颜色
                c.ChartAreas[0].AxisX.LineColor = ColorTranslator.FromHtml("#38587a"); ;
                c.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
                c.ChartAreas[0].AxisX.LabelStyle.Font = new Font("微软雅黑", 10f, FontStyle.Regular);
                //X轴网络线条
                c.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                c.ChartAreas[0].AxisX.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");
                //Y坐标轴颜色
                c.ChartAreas[0].AxisY.LineColor = ColorTranslator.FromHtml("#38587a");
                c.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Black;
                c.ChartAreas[0].AxisY.LabelStyle.Font = new Font("微软雅黑", 10f, FontStyle.Regular);

                //Y轴网格线条
                c.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                c.ChartAreas[0].AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");


                c.ChartAreas[0].AxisY2.LineColor = Color.Transparent;
                c.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;


                //给系列上的点进行赋值，分别对应横坐标和纵坐标的值
                Count.Points.AddXY(PieceName[0], Convert.ToString(count1));
                Count.Points[0].Color = Color.Red;
                Count.Points.AddXY(PieceName[1], Convert.ToString(count2));
                Count.Points[1].Color = Color.Yellow;
                Count.Points.AddXY(PieceName[2], Convert.ToString(count3));
                Count.Points[2].Color = Color.Blue;
                //把series添加到chart上
                c.Series.Add(Count);
                c.Legends.Add(legend);
                c.Legends[0].Position.Auto = false;  //隐藏工件数量
            });
        }

        public void InitLine(Chart chart, string Title)
        {
            chart.InvokeEx(c =>
            {
                c.ChartAreas.Clear();
                ChartArea chartArea1 = new ChartArea("C1");
                c.ChartAreas.Add(chartArea1);
                //清除默认的series
                c.Series.Clear();

                Series Count1 = new Series(PieceName[0]);
                Series Count2 = new Series(PieceName[1]);
                Series Count3 = new Series(PieceName[2]);

                Count1.XValueType = ChartValueType.String;  //设置X轴上的值类型
                Count1.Label = "#VAL";                //设置显示X Y的值    
                Count1.LabelForeColor = Color.Black;
                Count1.ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
                Count1.ChartType = SeriesChartType.Line;    //图类型(折线)
                Count1.Color = Color.Red;
                Count1.IsValueShownAsLabel = true;
                Count1.CustomProperties = "DrawingStyle = Cylinder";

                Count2.XValueType = ChartValueType.String;  //设置X轴上的值类型
                Count2.Label = "#VAL";                //设置显示X Y的值    
                Count2.LabelForeColor = Color.Black;
                Count2.ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
                Count2.ChartType = SeriesChartType.Line;    //图类型(折线)
                Count2.Color = Color.Yellow;
                Count2.IsValueShownAsLabel = true;
                Count2.CustomProperties = "DrawingStyle = Cylinder";

                Count3.XValueType = ChartValueType.String;  //设置X轴上的值类型
                Count3.Label = "#VAL";                //设置显示X Y的值    
                Count3.LabelForeColor = Color.Black;
                Count3.ToolTip = "#VALX:#VAL";     //鼠标移动到对应点显示数值
                Count3.ChartType = SeriesChartType.Line;    //图类型(折线)
                Count3.Color = Color.Blue;
                Count3.IsValueShownAsLabel = true;
                Count3.CustomProperties = "DrawingStyle = Cylinder";

                //标题
                c.Titles.Add(Title);
                c.Titles[0].ForeColor = Color.Black;
                c.Titles[0].Font = new Font("微软雅黑", 12f, FontStyle.Regular);
                c.Titles[0].Alignment = ContentAlignment.TopCenter;

                //控件背景
                c.BackColor = Color.LightGray;
                //图表区背景
                c.ChartAreas[0].BackColor = Color.LightGray;
                c.ChartAreas[0].BorderColor = Color.LightGray;

                //X坐标轴颜色
                c.ChartAreas[0].AxisX.LineColor = ColorTranslator.FromHtml("#38587a");
                c.ChartAreas[0].AxisX.LabelStyle.ForeColor = Color.Black;
                c.ChartAreas[0].AxisX.LabelStyle.Font = new Font("微软雅黑", 10f, FontStyle.Regular);
                //X轴网络线条
                c.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
                c.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightCyan/*ColorTranslator.FromHtml("#2c4c6d")*/;
                //Y坐标轴颜色
                c.ChartAreas[0].AxisY.LineColor = ColorTranslator.FromHtml("#38587a");
                c.ChartAreas[0].AxisY.LabelStyle.ForeColor = Color.Black;
                c.ChartAreas[0].AxisY.LabelStyle.Font = new Font("微软雅黑", 10f, FontStyle.Regular);

                //Y轴网格线条
                c.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
                c.ChartAreas[0].AxisY.MajorGrid.LineColor = ColorTranslator.FromHtml("#2c4c6d");

                c.ChartAreas[0].AxisY2.LineColor = Color.Transparent;
                c.ChartAreas[0].BackGradientStyle = GradientStyle.TopBottom;

                Count1.Color = Color.Red;
                //Count1.Points.DataBindXY(dayth, demoCount1);
                Count2.Color = Color.Yellow;
                //Count2.Points.DataBindXY(dayth, demoCount2);
                Count3.Color = Color.Blue;
                //Count3.Points.DataBindXY(dayth, demoCount3);

                c.Series.Add(Count1);
                c.Series.Add(Count2);
                c.Series.Add(Count3);
            });
        }

        public void InitPie(Chart chart, string Title, int count1 = 0, int count2 = 0, int count3 = 0)
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
                c.Titles[0].Font = new Font("微软雅黑", 12f, FontStyle.Regular);
                c.Titles[0].Alignment = ContentAlignment.TopCenter;
                //控件背景
                c.BackColor = Color.LightGray;
                //图表区背景
                c.ChartAreas[0].BackColor = Color.LightGray;
                c.ChartAreas[0].BorderColor = Color.LightGray;

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
                legend2.TitleFont = new Font("微软雅黑", 10f, FontStyle.Regular);
                legend2.Font = new Font("微软雅黑", 8f, FontStyle.Regular);
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

                c.Series[0].Points.AddXY(PieceName[0], Convert.ToString(count1));
                c.Series[0].Points[0].Color = Color.Red;
                c.Series[0].Points.AddXY(PieceName[1], Convert.ToString(count2));
                c.Series[0].Points[1].Color = Color.Yellow;
                c.Series[0].Points.AddXY(PieceName[2], Convert.ToString(count3));
                c.Series[0].Points[2].Color = Color.Blue;
                //绑定颜色
                c.Series[0].Palette = ChartColorPalette.BrightPastel;
            });
        }

        public void UpdateChartColumnOrPie(Chart chart, int Count1, int Count2, int Count3)
        {
            chart.InvokeEx(c =>
            {
                c.Series[0].Points.AddXY(PieceName[0], Convert.ToString(Count1));
                c.Series[0].Points[0].Color = Color.Red;
                c.Series[0].Points.AddXY(PieceName[1], Convert.ToString(Count2));
                c.Series[0].Points[1].Color = Color.Yellow;
                c.Series[0].Points.AddXY(PieceName[2], Convert.ToString(Count3));
                c.Series[0].Points[2].Color = Color.Blue;
            });
        }

        public void UpdateChartLine(Chart chart, string[] name, int[] LinePoint1, int[] LinePoint2, int[] LinePoint3)
        {
            chart.InvokeEx(c =>
            {
                c.Series[0].Color = Color.Red;
                c.Series[0].Points.DataBindXY(name, LinePoint1);
                c.Series[1].Color = Color.Yellow;
                c.Series[1].Points.DataBindXY(name, LinePoint2);
                c.Series[2].Color = Color.Blue;
                c.Series[2].Points.DataBindXY(name, LinePoint3);
            });
        }
    }
}
