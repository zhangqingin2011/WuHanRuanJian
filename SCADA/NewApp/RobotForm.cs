using SCADA.SimensPLC;
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

namespace SCADA.NewApp
{
    public partial class RobotForm : Form
    {
        private RobotUnit robotUnit1;
        private RobotUnit robotUnit2;
        private RobotUnit robotUnit3;
        private RobotUnit robotUnit4;
        private RobotUnit robotUnit5;
        private RobotUnit robotUnit6;
        private RobotUnit robotUnit7;
        private RobotUnit robotUnit8;
        private RobotUnit robotUnit9;
        public RobotForm()
        {
            InitializeComponent();
            InitUI();
            Task.Run(() => AutoDoWork());
        }

        public void InitUI()
        {
            this.robotUnit1 = new SCADA.NewApp.RobotUnit();
            this.robotUnit2 = new SCADA.NewApp.RobotUnit();
            this.robotUnit3 = new SCADA.NewApp.RobotUnit();
            this.robotUnit4 = new SCADA.NewApp.RobotUnit();
            this.robotUnit5 = new SCADA.NewApp.RobotUnit();
            this.robotUnit6 = new SCADA.NewApp.RobotUnit();
            this.robotUnit7 = new SCADA.NewApp.RobotUnit();
            this.robotUnit8 = new SCADA.NewApp.RobotUnit();
            this.robotUnit9 = new SCADA.NewApp.RobotUnit();
            this.tableLayoutPanel1.Controls.Add(this.robotUnit1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.robotUnit2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.robotUnit3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.robotUnit4, 1, 1);
            this.tableLayoutPanel12.Controls.Add(this.robotUnit5, 0, 0);
            this.tableLayoutPanel12.Controls.Add(this.robotUnit6, 1, 0);
            this.tableLayoutPanel12.Controls.Add(this.robotUnit7, 0,0);
            this.tableLayoutPanel12.Controls.Add(this.robotUnit8, 1, 1);
            this.tableLayoutPanel12.Controls.Add(this.robotUnit9, 2, 0);

            // 
            // robotUnit1
            // 
            this.robotUnit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotUnit1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.robotUnit1.Location = new System.Drawing.Point(5, 5);
            this.robotUnit1.Margin = new System.Windows.Forms.Padding(5);
            this.robotUnit1.Name = "robotUnit1";
            this.robotUnit1.Size = new System.Drawing.Size(467, 365);
            this.robotUnit1.TabIndex = 0;

            this.robotUnit2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotUnit2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.robotUnit2.Location = new System.Drawing.Point(5, 5);
            this.robotUnit2.Margin = new System.Windows.Forms.Padding(5);
            this.robotUnit2.Name = "robotUnit2";
            this.robotUnit2.Size = new System.Drawing.Size(467, 365);
            this.robotUnit2.TabIndex = 0;

            this.robotUnit3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotUnit3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.robotUnit3.Location = new System.Drawing.Point(5, 5);
            this.robotUnit3.Margin = new System.Windows.Forms.Padding(5);
            this.robotUnit3.Name = "robotUnit3";
            this.robotUnit3.Size = new System.Drawing.Size(467, 365);
            this.robotUnit3.TabIndex = 0;

            this.robotUnit4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotUnit4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.robotUnit4.Location = new System.Drawing.Point(5, 5);
            this.robotUnit4.Margin = new System.Windows.Forms.Padding(5);
            this.robotUnit4.Name = "robotUnit4";
            this.robotUnit4.Size = new System.Drawing.Size(467, 365);
            this.robotUnit4.TabIndex = 0;

            this.robotUnit5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotUnit5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.robotUnit5.Location = new System.Drawing.Point(5, 5);
            this.robotUnit5.Margin = new System.Windows.Forms.Padding(5);
            this.robotUnit5.Name = "robotUnit5";
            this.robotUnit5.Size = new System.Drawing.Size(467, 365);
            this.robotUnit5.TabIndex = 0;

            this.robotUnit6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotUnit6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.robotUnit6.Location = new System.Drawing.Point(5, 5);
            this.robotUnit6.Margin = new System.Windows.Forms.Padding(5);
            this.robotUnit6.Name = "robotUnit6";
            this.robotUnit6.Size = new System.Drawing.Size(467, 365);
            this.robotUnit6.TabIndex = 0;

            this.robotUnit7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotUnit7.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.robotUnit7.Location = new System.Drawing.Point(5, 5);
            this.robotUnit7.Margin = new System.Windows.Forms.Padding(5);
            this.robotUnit7.Name = "robotUnit7";
            this.robotUnit7.Size = new System.Drawing.Size(467, 365);
            this.robotUnit7.TabIndex = 0;

            this.robotUnit8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotUnit8.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.robotUnit8.Location = new System.Drawing.Point(5, 5);
            this.robotUnit8.Margin = new System.Windows.Forms.Padding(5);
            this.robotUnit8.Name = "robotUnit8";
            this.robotUnit8.Size = new System.Drawing.Size(467, 365);
            this.robotUnit8.TabIndex = 0;

            this.robotUnit9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotUnit9.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.robotUnit9.Location = new System.Drawing.Point(5, 5);
            this.robotUnit9.Margin = new System.Windows.Forms.Padding(5);
            this.robotUnit9.Name = "robotUnit9";
            this.robotUnit9.Size = new System.Drawing.Size(467, 365);
            this.robotUnit9.TabIndex = 0;

            robotUnit1.Description.Text = "单元一机器人";
            robotUnit1.Picture.Image = Image.FromFile("..\\picture\\Robot.png");

            robotUnit2.Description.Text = "单元二机器人";
            robotUnit2.Picture.Image = Image.FromFile("..\\picture\\Robot.png");
            robotUnit3.Description.Text = "单元三机器人";
            robotUnit3.Picture.Image = Image.FromFile("..\\picture\\Robot.png");
            robotUnit4.Description.Text = "单元四机器人";
            robotUnit4.Picture.Image = Image.FromFile("..\\picture\\Robot.png");
            robotUnit5.Description.Text = "装配一机器人";
            robotUnit5.Picture.Image = Image.FromFile("..\\picture\\Robot.png");
            robotUnit6.Description.Text = "装配二机器人";
            robotUnit6.Picture.Image = Image.FromFile("..\\picture\\Robot.png");
            robotUnit7.Description.Text = "检测二机器人";
            robotUnit7.Picture.Image = Image.FromFile("..\\picture\\Robot.png");
            robotUnit8.Description.Text = "检测一机器人";
            robotUnit8.Picture.Image = Image.FromFile("..\\picture\\Robot.png");
            robotUnit9.Description.Text = "清洗机器人";
            robotUnit9.Picture.Image = Image.FromFile("..\\picture\\Robot.png");

           
        }

        private void AutoDoWork()
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));          
                UpdateRobotDataShow();
            }
        }

        private void UpdateRobotDataShow()
        {
            if (LineMainForm.controlplc.GetOnlineState())
            {
                int[] values = new int[22];

                LineMainForm.controlplc.ReadMutiRegisters((int)ControlPLC.REGINDEX.检测机器人1J12实时坐标值, values.Length, out values);
                GetRobotData(values, robotUnit7);

                LineMainForm.controlplc.ReadMutiRegisters((int)ControlPLC.REGINDEX.检测机器人2J1实时坐标值, values.Length, out values);
                GetRobotData(values, robotUnit8);

                LineMainForm.controlplc.ReadMutiRegisters((int)ControlPLC.REGINDEX.装配机器人1J1实时坐标值, values.Length, out values);
                GetRobotData(values, robotUnit5);

                LineMainForm.controlplc.ReadMutiRegisters((int)ControlPLC.REGINDEX.装配机器人2J1实时坐标值, values.Length, out values);
                GetRobotData(values, robotUnit6);

                LineMainForm.controlplc.ReadMutiRegisters((int)ControlPLC.REGINDEX.清洗机器人1J1实时坐标值, values.Length, out values);
                GetRobotData(values, robotUnit9);
            }
            else
            {
                CleanRobotData(robotUnit5);
                CleanRobotData(robotUnit6);
                CleanRobotData(robotUnit7);
                CleanRobotData(robotUnit8);
                CleanRobotData(robotUnit9);
            }
            if(LineMainForm.unitplc1.GetOnlineState())
            {
                int[] values = new int[20];
                LineMainForm.unitplc1.ReadMutiRegisters((int)UnitPLC.REGINDEX.加工机器人J1实时坐标值, values.Length, out values);
                GetRobotData(values, robotUnit1);
            }
            else
            {
                CleanRobotData(robotUnit1);
            }
            if (LineMainForm.unitplc2.GetOnlineState())
            {
                int[] values = new int[20];
                LineMainForm.unitplc2.ReadMutiRegisters((int)UnitPLC.REGINDEX.加工机器人J1实时坐标值, values.Length, out values);
                GetRobotData(values, robotUnit2);
            }
            else
            {
                CleanRobotData(robotUnit2);
            }
            if (LineMainForm.unitplc3.GetOnlineState())
            {
                int[] values = new int[20];
                LineMainForm.unitplc3.ReadMutiRegisters((int)UnitPLC.REGINDEX.加工机器人J1实时坐标值, values.Length, out values);
                GetRobotData(values, robotUnit3);
            }
            else
            {
                CleanRobotData(robotUnit3);
            }
            if (LineMainForm.unitplc4.GetOnlineState())
            {
                int[] values = new int[20];
                LineMainForm.unitplc4.ReadMutiRegisters((int)UnitPLC.REGINDEX.加工机器人J1实时坐标值, values.Length, out values);
                GetRobotData(values, robotUnit4);
            }
            else
            {
                CleanRobotData(robotUnit4);
            }
            
        }
        private void GetRobotData(int[] values , RobotUnit robotunit)
        {
            robotunit.ShowJ1.Description.Text = (values[0] *256+values[1]).ToString(); 
            robotunit.ShowJ2.Description.Text = (values[2] * 256 + values[3]).ToString();
            robotunit.ShowJ3.Description.Text = (values[4] * 256 + values[5]).ToString();
            robotunit.ShowJ4.Description.Text = (values[6] * 256 + values[7]).ToString();
            robotunit.ShowJ5.Description.Text = (values[8] * 256 + values[9]).ToString();
            robotunit.ShowJ6.Description.Text = (values[10] * 256 + values[11]).ToString();
            robotunit.ShowJ7.Description.Text = (values[12] * 256 + values[13]).ToString();
     

            if (values[14]==0)
                robotunit.AlarmIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                robotunit.AlarmIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });

            if (values[16] == 1)
                robotunit.HomeIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                robotunit.HomeIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });

            if (values[20] == 1)
                robotunit.BusyIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                robotunit.BusyIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });

            if (values[18] == 2)
                robotunit.AotoIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                robotunit.AotoIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });
        }


        private void CleanRobotData(RobotUnit robotunit)
        {
            robotunit.ShowJ1.InvokeEx(c => { c.Text = ""; });
            robotunit.ShowJ2.InvokeEx(c => { c.Text = ""; });
            robotunit.ShowJ3.InvokeEx(c => { c.Text = ""; });
            robotunit.ShowJ4.InvokeEx(c => { c.Text = ""; });
            robotunit.ShowJ5.InvokeEx(c => { c.Text = ""; });
            robotunit.ShowJ6.InvokeEx(c => { c.Text = ""; });
            robotunit.ShowJ7.InvokeEx(c => { c.Text = ""; });
            robotunit.HomeIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });
            robotunit.AlarmIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });
            robotunit.BusyIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });
            robotunit.AotoIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });

          

        }
    }
}
