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
    public partial class CncNewForm : Form
    {
        private CncUnit cncUnit8;
        private CncUnit cncUnit7;
        private CncUnit cncUnit6;
        private CncUnit cncUnit5;
        private CncUnit cncUnit4;
        private CncUnit cncUnit3;
        private CncUnit cncUnit2;
        private CncUnit cncUnit1;

        public CncNewForm()
        {
            InitializeComponent();
            InitUI();
            Task.Run(() => AutoDoWork());
        }

        public void InitUI()
        {
            this.cncUnit1 = new SCADA.NewApp.CncUnit();
            this.cncUnit2 = new SCADA.NewApp.CncUnit();
            this.cncUnit3 = new SCADA.NewApp.CncUnit();
            this.cncUnit4 = new SCADA.NewApp.CncUnit();
            this.cncUnit5 = new SCADA.NewApp.CncUnit();
            this.cncUnit6 = new SCADA.NewApp.CncUnit();
            this.cncUnit7 = new SCADA.NewApp.CncUnit();
            this.cncUnit8 = new SCADA.NewApp.CncUnit();
            this.tableLayoutPanel1.Controls.Add(this.cncUnit8, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.cncUnit7, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.cncUnit6, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.cncUnit5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cncUnit4, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.cncUnit3, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.cncUnit2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cncUnit1, 0, 0);
            // 
            // cncUnit1
            // 
            this.cncUnit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cncUnit1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cncUnit1.Location = new System.Drawing.Point(5, 5);
            this.cncUnit1.Margin = new System.Windows.Forms.Padding(5);
            this.cncUnit1.Name = "cncUnit1";
            this.cncUnit1.Size = new System.Drawing.Size(656, 384);
            this.cncUnit1.TabIndex = 0;
            // 
            // cncUnit2
            // 
            this.cncUnit2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cncUnit2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cncUnit2.Location = new System.Drawing.Point(671, 5);
            this.cncUnit2.Margin = new System.Windows.Forms.Padding(5);
            this.cncUnit2.Name = "cncUnit2";
            this.cncUnit2.Size = new System.Drawing.Size(657, 384);
            this.cncUnit2.TabIndex = 1;
            // 
            // cncUnit3
            // 
            this.cncUnit3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cncUnit3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cncUnit3.Location = new System.Drawing.Point(5, 399);
            this.cncUnit3.Margin = new System.Windows.Forms.Padding(5);
            this.cncUnit3.Name = "cncUnit3";
            this.cncUnit3.Size = new System.Drawing.Size(656, 384);
            this.cncUnit3.TabIndex = 2;
            // 
            // cncUnit4
            // 
            this.cncUnit4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cncUnit4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cncUnit4.Location = new System.Drawing.Point(671, 399);
            this.cncUnit4.Margin = new System.Windows.Forms.Padding(5);
            this.cncUnit4.Name = "cncUnit4";
            this.cncUnit4.Size = new System.Drawing.Size(657, 384);
            this.cncUnit4.TabIndex = 3;

            // 
            // cncUnit5
            // 
            this.cncUnit5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cncUnit5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cncUnit5.Location = new System.Drawing.Point(5, 5);
            this.cncUnit5.Margin = new System.Windows.Forms.Padding(5);
            this.cncUnit5.Name = "cncUnit5";
            this.cncUnit5.Size = new System.Drawing.Size(656, 384);
            this.cncUnit5.TabIndex = 0;
            // 
            // cncUnit6
            // 
            this.cncUnit6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cncUnit6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cncUnit6.Location = new System.Drawing.Point(671, 5);
            this.cncUnit6.Margin = new System.Windows.Forms.Padding(5);
            this.cncUnit6.Name = "cncUnit6";
            this.cncUnit6.Size = new System.Drawing.Size(657, 384);
            this.cncUnit6.TabIndex = 1;
            // 
            // cncUnit3
            // 
            this.cncUnit7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cncUnit7.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cncUnit7.Location = new System.Drawing.Point(5, 399);
            this.cncUnit7.Margin = new System.Windows.Forms.Padding(5);
            this.cncUnit7.Name = "cncUnit7";
            this.cncUnit7.Size = new System.Drawing.Size(656, 384);
            this.cncUnit7.TabIndex = 2;
            // 
            // cncUnit4
            // 
            this.cncUnit8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cncUnit8.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cncUnit8.Location = new System.Drawing.Point(671, 399);
            this.cncUnit8.Margin = new System.Windows.Forms.Padding(5);
            this.cncUnit8.Name = "cncUnit8";
            this.cncUnit8.Size = new System.Drawing.Size(657, 384);
            this.cncUnit8.TabIndex = 3;

            cncUnit1.Description.Text = "单元一车床";
            cncUnit1.Picture.Image = Image.FromFile("..\\picture\\chechuang3.png");
            cncUnit2.Description.Text = "单元一加工中心";
            cncUnit2.Picture.Image = Image.FromFile("..\\picture\\xichuang3.png");
            cncUnit3.Description.Text = "单元二车床";
            cncUnit3.Picture.Image = Image.FromFile("..\\picture\\chechuang3.png");
            cncUnit4.Description.Text = "单元二加工中心";
            cncUnit4.Picture.Image = Image.FromFile("..\\picture\\xichuang3.png");
            cncUnit5.Description.Text = "单元三车床";
            cncUnit5.Picture.Image = Image.FromFile("..\\picture\\chechuang3.png");
            cncUnit6.Description.Text = "单元三加工中心";
            cncUnit6.Picture.Image = Image.FromFile("..\\picture\\xichuang3.png");
            cncUnit7.Description.Text = "单元四车床";
            cncUnit7.Picture.Image = Image.FromFile("..\\picture\\chechuang3.png");
            cncUnit8.Description.Text = "单元四加工中心";
            cncUnit8.Picture.Image = Image.FromFile("..\\picture\\xichuang3.png");
        }

        private void AutoDoWork()
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                UpdateCNCDataShow();
            }
        }

        private void UpdateCNCDataShow()
        {
            if(LineMainForm.unitplc1.GetOnlineState())
            {
                int[] values = new int[4];
                LineMainForm.unitplc1.ReadMutiRegisters((int)UnitPLC.REGINDEX.车床状态1, values.Length, out values);
                GetUnitData(values[0], values[1], values[2], values[3], cncUnit1, cncUnit2);
            }
            else
            {
                CleanCNCIO(cncUnit1);
                CleanCNCIO(cncUnit2);
            }

            if (LineMainForm.unitplc2.GetOnlineState())
            {
                int[] values = new int[4];
                LineMainForm.unitplc2.ReadMutiRegisters((int)UnitPLC.REGINDEX.车床状态1, values.Length, out values);
                GetUnitData(values[0], values[1], values[2], values[3], cncUnit3, cncUnit4);
            }
            else
            {
                CleanCNCIO(cncUnit3);
                CleanCNCIO(cncUnit4);
            }

            if (LineMainForm.unitplc3.GetOnlineState())
            {
                int[] values = new int[4];
                LineMainForm.unitplc3.ReadMutiRegisters((int)UnitPLC.REGINDEX.车床状态1, values.Length, out values);
                GetUnitData(values[0], values[1], values[2], values[3], cncUnit5, cncUnit6);
            }
            else
            {
                CleanCNCIO(cncUnit5);
                CleanCNCIO(cncUnit6);
            }

            if (LineMainForm.unitplc4.GetOnlineState())
            {
                int[] values = new int[4];
                LineMainForm.unitplc4.ReadMutiRegisters((int)UnitPLC.REGINDEX.车床状态1, values.Length, out values);
                GetUnitData(values[0], values[1], values[2], values[3], cncUnit7, cncUnit8);
            }
            else
            {
                CleanCNCIO(cncUnit7);
                CleanCNCIO(cncUnit8);
            }
        }

        private void GetUnitData(int value1, int value2, int value3, int value4, CncUnit Unit1, CncUnit Unit2)
        {
            if (UnitPLC.GetBoolValue(value1, (int)UnitPLC.CNCSTATEBit1.Onlin))
                Unit1.OnlineIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                Unit1.OnlineIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });

            if (UnitPLC.GetBoolValue(value1, (int)UnitPLC.CNCSTATEBit1.RUN))
                Unit1.WorkIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                Unit1.WorkIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });

            if (UnitPLC.GetBoolValue(value1, (int)UnitPLC.CNCSTATEBit1.Alarm))
                Unit1.AlarmIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                Unit1.AlarmIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });

            if (UnitPLC.GetBoolValue(value1, (int)UnitPLC.CNCSTATEBit1.ClampOpen))
                Unit1.ChuckIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                Unit1.ChuckIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });

            if (UnitPLC.GetBoolValue(value2, (int)UnitPLC.CNCSTATEBit2.DoorOpen))
                Unit1.DoorIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                Unit1.DoorIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });
            
            if (UnitPLC.GetBoolValue(value3, (int)UnitPLC.CNCSTATEBit1.Onlin))
                Unit2.OnlineIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                Unit2.OnlineIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });

            if (UnitPLC.GetBoolValue(value3, (int)UnitPLC.CNCSTATEBit1.RUN))
                Unit2.WorkIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                Unit2.WorkIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });

            if (UnitPLC.GetBoolValue(value3, (int)UnitPLC.CNCSTATEBit1.Alarm))
                Unit2.AlarmIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                Unit2.AlarmIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });

            if (UnitPLC.GetBoolValue(value3, (int)UnitPLC.CNCSTATEBit1.ClampOpen))
                Unit2.ChuckIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                Unit2.ChuckIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });

            if (UnitPLC.GetBoolValue(value4, (int)UnitPLC.CNCSTATEBit2.DoorOpen))
                Unit2.DoorIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_green.png"); });
            else
                Unit2.DoorIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });
        }

        private void CleanCNCIO(CncUnit cncUnit)
        {
            cncUnit.OnlineIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });
            cncUnit.DoorIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });
            cncUnit.ChuckIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });
            cncUnit.WorkIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });
            cncUnit.AlarmIO.InvokeEx(c => { c.Picture.Image = Image.FromFile("top_bar_black.png"); });
        }
    }
}
