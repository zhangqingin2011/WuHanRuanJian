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
    public partial class TableForm : Form
    {
        private TableUnit tableunit1 = new TableUnit();
        private TableUnit tableunit2 = new TableUnit();
        private TableUnit tableunit3 = new TableUnit();
        private TableUnit tableunit4 = new TableUnit();
        private TableUnit tableunit5 = new TableUnit();
        private TableUnit tableunit6 = new TableUnit();
        private TableUnit tableunit7 = new TableUnit();
        private TableUnit tableunit8 = new TableUnit();
        private TableUnit tableunit9 = new TableUnit();
        public TableForm()
        {
            InitializeComponent();
            InitUI();
            Task.Run(() => AutoDoWork());
        }

        public void InitUI()
        {
            this.tableunit1 = new SCADA.NewApp.TableUnit();
            this.tableunit2 = new SCADA.NewApp.TableUnit();
            this.tableunit3 = new SCADA.NewApp.TableUnit();
            this.tableunit4 = new SCADA.NewApp.TableUnit();
            this.tableunit5 = new SCADA.NewApp.TableUnit();
            this.tableunit6 = new SCADA.NewApp.TableUnit();
            this.tableunit7 = new SCADA.NewApp.TableUnit();
            this.tableunit8 = new SCADA.NewApp.TableUnit();
            this.tableunit9 = new SCADA.NewApp.TableUnit();
            this.tableLayoutPanel1.Controls.Add(this.tableunit1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableunit2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableunit3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableunit4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableunit5, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableunit6, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableunit7, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableunit8, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableunit9, 2, 3);

            // 
            // tableunit1
            // 
            this.tableunit1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableunit1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableunit1.Location = new System.Drawing.Point(5, 5);
            this.tableunit1.Margin = new System.Windows.Forms.Padding(5);
            this.tableunit1.Name = "tableunit1";
            this.tableunit1.Size = new System.Drawing.Size(450, 230);
            this.tableunit1.TabIndex = 0;

            this.tableunit2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableunit2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableunit2.Location = new System.Drawing.Point(5, 5);
            this.tableunit2.Margin = new System.Windows.Forms.Padding(5);
            this.tableunit2.Name = "tableunit2";
            this.tableunit2.Size = new System.Drawing.Size(450, 230);
            this.tableunit2.TabIndex = 0;

            this.tableunit3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableunit3.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableunit3.Location = new System.Drawing.Point(5, 5);
            this.tableunit3.Margin = new System.Windows.Forms.Padding(5);
            this.tableunit3.Name = "tableunit3";
            this.tableunit3.Size = new System.Drawing.Size(450, 230);
            this.tableunit3.TabIndex = 0;

            this.tableunit4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableunit4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableunit4.Location = new System.Drawing.Point(5, 5);
            this.tableunit4.Margin = new System.Windows.Forms.Padding(5);
            this.tableunit4.Name = "tableunit4";
            this.tableunit4.Size = new System.Drawing.Size(450, 230);
            this.tableunit4.TabIndex = 0;

            this.tableunit5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableunit5.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableunit5.Location = new System.Drawing.Point(5, 5);
            this.tableunit5.Margin = new System.Windows.Forms.Padding(5);
            this.tableunit5.Name = "tableunit5";
            this.tableunit5.Size = new System.Drawing.Size(450, 230);
            this.tableunit5.TabIndex = 0;

            this.tableunit6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableunit6.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableunit6.Location = new System.Drawing.Point(5, 5);
            this.tableunit6.Margin = new System.Windows.Forms.Padding(5);
            this.tableunit6.Name = "tableunit6";
            this.tableunit6.Size = new System.Drawing.Size(450, 230);
            this.tableunit6.TabIndex = 0;

            this.tableunit7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableunit7.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableunit7.Location = new System.Drawing.Point(5, 5);
            this.tableunit7.Margin = new System.Windows.Forms.Padding(5);
            this.tableunit7.Name = "tableunit7";
            this.tableunit7.Size = new System.Drawing.Size(450, 230);
            this.tableunit7.TabIndex = 0;

            this.tableunit8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableunit8.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableunit8.Location = new System.Drawing.Point(5, 5);
            this.tableunit8.Margin = new System.Windows.Forms.Padding(5);
            this.tableunit8.Name = "tableunit8";
            this.tableunit8.Size = new System.Drawing.Size(450, 230);
            this.tableunit8.TabIndex = 0;

            this.tableunit9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableunit9.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tableunit9.Location = new System.Drawing.Point(5, 5);
            this.tableunit9.Margin = new System.Windows.Forms.Padding(5);
            this.tableunit9.Name = "tableunit9";
            this.tableunit9.Size = new System.Drawing.Size(450, 230);
            this.tableunit9.TabIndex = 0;

            tableunit1.Description.Text = "单元一定位台";
            tableunit2.Description.Text = "单元二定位台";
            tableunit3.Description.Text = "装配一定位台";
            tableunit4.Description.Text = "装配一定位台";
            tableunit5.Description.Text = "清洗定位台";
            tableunit6.Description.Text = "单元三定位台";
            tableunit7.Description.Text = "单元四定位台";
            tableunit8.Description.Text = "检测一定位台";
            tableunit9.Description.Text = "检测二定位台";
        }
        private void AutoDoWork()
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                UpdateTableDataShow();
            }
        }
        private void UpdateTableDataShow()
        {
            if (LineMainForm.controlplc.GetOnlineState())
            {

                var tableR = Program.Repo.GetSingle<TableUnitData>(p=>p.NO == 5 && p.PNO==1);
                var tableL= Program.Repo.GetSingle<TableUnitData>(p => p.NO == 5 && p.PNO == 2);
                GetTableData(tableR, tableL, tableunit5);


                 tableR = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 6 && p.PNO == 1);
                 tableL = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 6 && p.PNO == 2);
                GetTableData(tableR, tableL, tableunit8);

                tableR = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 9 && p.PNO == 1);
                tableL = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 9 && p.PNO == 2);
                GetTableData(tableR, tableL, tableunit9);

                tableR = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 3 && p.PNO == 1);
                tableL = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 3 && p.PNO == 2);
                GetTableData(tableR, tableL, tableunit3);

                tableR = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 4 && p.PNO == 1);
                tableL = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 4 && p.PNO == 2);
                GetTableData(tableR, tableL, tableunit4);


                tableR = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 1 && p.PNO == 1);
                tableL = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 1 && p.PNO == 2);
                GetTableData(tableR, tableL, tableunit1);

                tableR = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 2 && p.PNO == 1);
                tableL = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 2 && p.PNO == 2);
                GetTableData(tableR, tableL, tableunit2);

                tableR = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 6 && p.PNO == 1);
                tableL = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 6 && p.PNO == 2);
                GetTableData(tableR, tableL, tableunit6);

                tableR = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 7 && p.PNO == 1);
                tableL = Program.Repo.GetSingle<TableUnitData>(p => p.NO == 7 && p.PNO == 2);
                GetTableData(tableR, tableL, tableunit7);
            }
            else
            {
                CleanTableData(tableunit1);
                CleanTableData(tableunit2);
                CleanTableData(tableunit3);
                CleanTableData(tableunit4);
                CleanTableData(tableunit5);
                CleanTableData(tableunit6);
                CleanTableData(tableunit7);
                CleanTableData(tableunit8);
                CleanTableData(tableunit9);
            }
        }
        private void GetTableData(TableUnitData tabler, TableUnitData tablel, TableUnit tableunit)
        {
            tableunit.ShowID1.InvokeEx(c => { c.Text = tabler.RfidID.ToString(); });

            tableunit.ShowIP1.InvokeEx(c => { c.Text = tabler.Tray.ToString(); });

            tableunit.ShowS11.InvokeEx(c => { c.Text = tabler.Piece1State.ToString(); });
            tableunit.ShowS12.InvokeEx(c => { c.Text = tabler.Piece2State.ToString(); });
            tableunit.ShowS13.InvokeEx(c => { c.Text = tabler.Piece3State.ToString(); });
            tableunit.ShowS14.InvokeEx(c => { c.Text = tabler.Piece4State.ToString(); });


            tableunit.ShowID2.InvokeEx(c => { c.Text = tablel.RfidID.ToString(); });

            tableunit.ShowIP2.InvokeEx(c => { c.Text = tablel.Tray.ToString(); });

            tableunit.ShowS21.InvokeEx(c => { c.Text = tablel.Piece1State.ToString(); });
            tableunit.ShowS22.InvokeEx(c => { c.Text = tablel.Piece2State.ToString(); });
            tableunit.ShowS23.InvokeEx(c => { c.Text = tablel.Piece3State.ToString(); });
            tableunit.ShowS24.InvokeEx(c => { c.Text = tablel.Piece4State.ToString(); });
        }


        private void CleanTableData(TableUnit tableunit)
        {
            tableunit.ShowID1.InvokeEx(c => { c.Text = ""; });

            tableunit.ShowIP1.InvokeEx(c => { c.Text = ""; });

            tableunit.ShowS11.InvokeEx(c => { c.Text = ""; });
            tableunit.ShowS12.InvokeEx(c => { c.Text = ""; });
            tableunit.ShowS13.InvokeEx(c => { c.Text = ""; });
            tableunit.ShowS14.InvokeEx(c => { c.Text = ""; });


            tableunit.ShowID2.InvokeEx(c => { c.Text = ""; });

            tableunit.ShowIP2.InvokeEx(c => { c.Text = ""; });

            tableunit.ShowS21.InvokeEx(c => { c.Text = ""; });
            tableunit.ShowS22.InvokeEx(c => { c.Text = ""; });
            tableunit.ShowS23.InvokeEx(c => { c.Text = ""; });
            tableunit.ShowS24.InvokeEx(c => { c.Text = ""; });

        }

    }
}
