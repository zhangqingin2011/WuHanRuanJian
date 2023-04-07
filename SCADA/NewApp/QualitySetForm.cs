using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCADA.NewApp
{
    public partial class QualitySetForm : Form
    {
        private TreeNode treeNode;
        private List<string> modenamelist = new List<string>();

        public QualitySetForm()
        {
            InitializeComponent();
            InitConfig();
        }

        private void InitConfig()
        {
            this.buttonsave.Click += new System.EventHandler(this.Buttonsave_Click);
            //this.groupBox3.SizeChanged += new System.EventHandler(this.GroupBox3_SizeChanged);
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            this.buttoncreatemode.Click += new System.EventHandler(this.Buttonmadeorder_Click);
            //this.SizeChanged += new System.EventHandler(this.MeterModeSet_SizeChanged);
            this.treeView1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView1_NodeMouseClick);
            treeNode = new TreeNode("模型列表");
            initmodename();
            renewtree();
        }

        private void initmodename()
        {
            var count = Program.Repo.Get<MeterMode>(p => p.Rank == 1);
            if (count == null || count.Count < 1)
            {
                return;
            }
            modenamelist = new List<string>();
            foreach (var temp in count)
            {
                modenamelist.Add(temp.ModeSN);
            }
        }

        private void Buttonmadeorder_Click(object sender, EventArgs e)
        {
            var modename = comboBoxModetype.Text;
            var num = textBoxmodenum.Text;
            if (modename == "")
            {
                MessageBox.Show("模型名称为空");
                return;
            }

            var count = Program.Repo.Count<MeterMode>(p => p.ModeSN == modename);
            if (count >= 1)
            {
                if (MessageBox.Show("是否清除原有检测模板，生成新模板？", "提示", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    this.Close();
                    return;
                }
            }

            var temp = Regex.IsMatch(num, @"([1-9])|([1-2]+[0-9])");
            if (temp == false)
            {
                MessageBox.Show("数据量不合法");
                return;
            }

            int number = Convert.ToInt32(num);
            if (count == 0)
            {
                for (int i = 1; i < number + 1; i++)
                {
                    var data = new MeterMode
                    {
                        ModeSN = modename,
                        NUM = number,
                        Rank = i,
                        SizeDataMin = 0.000,
                        SizeDataMax = 0.000,
                        SizeDataStandard = 0.000,
                        SizeDataUnit = "mm",
                    };
                    Program.Repo.Insert<MeterMode>(data);
                }
            }
            else if (count > 1)
            {
                var metermodelist = Program.Repo.Get<MeterMode>(p => p.ModeSN == modename);
                foreach (var t in metermodelist)
                {
                    Program.Repo.Delete(t);
                }
                for (int i = 1; i < number + 1; i++)
                {
                    var data = new MeterMode
                    {
                        ModeSN = modename,
                        NUM = number,
                        Rank = i,
                        SizeDataMin = 0.000,
                        SizeDataMax = 0.000,
                        SizeDataStandard = 0.000,
                        SizeDataUnit = "mm",
                    };
                    Program.Repo.Insert<MeterMode>(data);
                }
            }
            renewtree();
        }

        private void renewtree()
        {
            treeNode.Nodes.Clear();
            treeView1.Nodes.Clear();
            initmodename();
            if (modenamelist.Count < 1)
            {
                return;
            }

            foreach (var temp in modenamelist)
            {
                treeNode.Nodes.Add(temp);
            }
            treeView1.Nodes.Add(treeNode);
            treeView1.ExpandAll();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            renewtree();
        }

        private void TreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var name = e.Node.Text.ToString();
            textBoxCurMode.Text = name;
            var metermode = Program.Repo.Get<MeterMode>(p => p.ModeSN == name);
            if (metermode.Count < 1)
            {
                return;
            }
            var metermodefist = metermode.ElementAt<MeterMode>(0);

            dataGridViewDistence.Rows.Clear();
            int rownumber = metermode.Count();
            dataGridViewDistence.Rows.Add(rownumber);
            foreach (var temp in metermode)
            {
                int rank = temp.Rank;
                dataGridViewDistence.Rows[rank - 1].Cells[0].Value = rank.ToString();
                dataGridViewDistence.Rows[rank - 1].Cells[1].Value = temp.SizeDataStandard.ToString("f4");
                dataGridViewDistence.Rows[rank - 1].Cells[2].Value = temp.SizeDataMax.ToString("f4");
                dataGridViewDistence.Rows[rank - 1].Cells[3].Value = temp.SizeDataMin.ToString("f4");
            }

        }

        private void Buttonsave_Click(object sender, EventArgs e)
        {
            string name = textBoxCurMode.Text;
            var metermode = Program.Repo.Get<MeterMode>(p => p.ModeSN == name);
            if (metermode.Count < 1)
            {
                return;
            }
            var metermodelist = metermode.ElementAt<MeterMode>(0);
            try
            {
                foreach (var temp in metermode)
                {
                    int rank = temp.Rank;

                    string str = dataGridViewDistence.Rows[rank - 1].Cells[1].Value.ToString();
                    string str3 = Convert.ToDouble(str).ToString("0.0000");
                    dataGridViewDistence.Rows[rank - 1].Cells[1].Value = str3;
                    double d = Convert.ToDouble(str3);
                    temp.SizeDataStandard = d;

                    str = dataGridViewDistence.Rows[rank - 1].Cells[2].Value.ToString();
                    str3 = Convert.ToDouble(str).ToString("0.0000");
                    d = Convert.ToDouble(str3);
                    dataGridViewDistence.Rows[rank - 1].Cells[2].Value = str3;
                    temp.SizeDataMax = d;

                    str = dataGridViewDistence.Rows[rank - 1].Cells[3].Value.ToString();
                    str3 = Convert.ToDouble(str).ToString("0.0000");
                    d = Convert.ToDouble(str3);
                    dataGridViewDistence.Rows[rank - 1].Cells[3].Value = str3;
                    temp.SizeDataMin = d;
                    Program.Repo.Update(temp);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入数据不合法!");
            }
        }

     
    }
}
