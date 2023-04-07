using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace SCADA.NewApp
{
    public partial class SetForm2 : Form
    {
        public SetForm2()
        {
            InitializeComponent();
            InitConfig();
        }

        public static string LogUser = "游客";
        private List<Binding> m_bindingList = new List<Binding>();
        public delegate void OpenTestButton(object sender);
        public static OpenTestButton OpenTest;

        private void OnOpenTestButton()
        {
            if (OpenTest != null)
            {
                OpenTest(this);
            }
        }

        private void InitConfig()
        {
            Binding m_Binding = new Binding("Text", dataGridViewcnc, "RowCount", false);
            m_Binding.Parse += new ConvertEventHandler(OnCountryFromParse);
            textBoxcnccount.DataBindings.Add(m_Binding);
            m_bindingList.Add(m_Binding);
            RefreshDgvDataChangeF(dataGridViewcnc, 0);
            m_Binding = new Binding("Text", dataGridViewPLC, "RowCount", false);
            m_Binding.Parse += new ConvertEventHandler(OnCountryFromParse);
            textBoxplccount.DataBindings.Add(m_Binding);
            m_bindingList.Add(m_Binding);
            RefreshDgvDataChangeF(dataGridViewPLC, 0);
            m_Binding = new Binding("Text", dataGridViewrfid, "RowCount", false);
            m_Binding.Parse += new ConvertEventHandler(OnCountryFromParse);
            textBoxrfidcount.DataBindings.Add(m_Binding);
            m_bindingList.Add(m_Binding);
            RefreshDgvDataChangeF(dataGridViewrfid, 0);

            InitdgvSet(ref dataGridViewcnc, m_xmlDociment.Default_CNC_STR);
            dataGridViewcnc.Columns[0].ReadOnly = true;
            dataGridViewcnc.Columns[1].ReadOnly = true;
            dataGridViewcnc.Columns[2].Visible = false;
            dataGridViewcnc.Columns[3].Visible = false;
            dataGridViewcnc.Columns[4].Visible = false;
            ChangeGridViewColumns(dataGridViewcnc);
            InitdgvSet(ref dataGridViewPLC, m_xmlDociment.Default_PLC_STR);
            dataGridViewPLC.Columns[0].ReadOnly = true;
            dataGridViewPLC.Columns[1].ReadOnly = true;
            dataGridViewPLC.Columns[2].Visible = false;
            dataGridViewPLC.Columns[3].Visible = false;
            dataGridViewPLC.Columns[4].Visible = false;
            dataGridViewPLC.Columns[8].Visible = false;
            ChangeGridViewColumns(dataGridViewPLC);
            InitdgvSet(ref dataGridViewrfid, m_xmlDociment.Default_RFID_STR);
            dataGridViewrfid.Columns[0].ReadOnly = true;
            dataGridViewrfid.Columns[1].ReadOnly = true;
            dataGridViewrfid.Columns[2].Visible = false;
            dataGridViewrfid.Columns[3].Visible = false;
            dataGridViewrfid.Columns[4].Visible = false;
            dataGridViewrfid.Columns[5].Visible = false;
            dataGridViewrfid.Columns[6].Visible = false;
            dataGridViewrfid.Columns[7].Visible = false;
            dataGridViewrfid.Columns[8].Visible = false;
            dataGridViewrfid.Columns[9].Visible = false;
            dataGridViewrfid.Columns[10].Visible = false;
            dataGridViewrfid.Columns[11].Visible = false;
            dataGridViewrfid.Columns[12].Visible = false;
            ChangeGridViewColumns(dataGridViewrfid);
        }

        private void RefreshDgvDataChangeF(DataGridView dgv, int flag)
        {
            dgv.TabIndex = flag;
        }

        private void OnCountryFromParse(object sender, ConvertEventArgs e)
        {
            int ii = 0;
            try
            {
                DataGridView DGV = (DataGridView)(((Binding)sender).DataSource);
                ii = int.Parse(e.Value.ToString());
                if (ii >= 0)
                {
                    if (DGV.RowCount != ii)//数据被改变
                    {
                        RefreshDgvDataChangeF(DGV, 1);
                    }
                    e.Value = ii.ToString();
                }
            }
            catch (Exception)
            {
            }
        }

        private void refreshDgvData2Xml(DataGridView DGV, string Pathstr)
        {
            if (!LineMainForm.m_xml.CheckNodeExist(Pathstr))
            {
                LineMainForm.m_xml.MakeXmlPLCDevicePath(Pathstr);
            }
            int ITemSUM = int.Parse(LineMainForm.m_xml.m_Read(Pathstr, -1, m_xmlDociment.Default_Attributes_str1[0]));//SUM
            if (DGV.Rows.Count > ITemSUM)//插入
            {
                for (int kk = 0; kk < (DGV.Rows.Count - ITemSUM); kk++)
                {
                    LineMainForm.m_xml.InserNode(Pathstr);
                }
            }
            else if (DGV.Rows.Count < ITemSUM)
            {
                for (int kk = 0; kk < (ITemSUM - DGV.Rows.Count); kk++)
                {
                    LineMainForm.m_xml.DeleNode(Pathstr, int.Parse(LineMainForm.m_xml.m_Read(Pathstr, -1, m_xmlDociment.Default_Attributes_str1[0])) - 1);
                }
            }
            for (int jj = 0; jj < DGV.Rows.Count; jj++)
            {
                string Itempas = Pathstr + "/" + m_xmlDociment.Default_Path_str[5] + jj.ToString();
                DataGridViewRow r = DGV.Rows[jj];
                LineMainForm.m_xml.GridViewRow2XmlAttributes(Itempas, ref r);
            }
        }

        private void refreshXmlData2DGV(string Pathstr, DataGridView DGV)
        {
            DGV.Rows.Clear();
            if (LineMainForm.m_xml.CheckNodeExist(Pathstr))
            {
                int ITSUM = int.Parse(LineMainForm.m_xml.m_Read(Pathstr, -1, m_xmlDociment.Default_Attributes_str1[0]));//SUM
                for (int ii = 0; ii < ITSUM; ii++)
                {
                    DataGridViewRow InserRows = new DataGridViewRow();
                    InserRows.CreateCells(DGV);
                    string inserPath = Pathstr + "/" + m_xmlDociment.Default_Path_str[5] + ii.ToString();
                    LineMainForm.m_xml.Attributes2GridViewRow(inserPath, ref InserRows);
                    DGV.Rows.Add(InserRows);
                }
            }
            DGV.ClearSelection();
        }

        private void InitdgvSet(ref DataGridView dgv, String[] dgvColumnAttributes_str)
        {
            dgv.ColumnCount = dgvColumnAttributes_str.Length;
            for (int ii = 0; ii < dgv.ColumnCount; ii++)
            {
                dgv.Columns[ii].HeaderText = dgvColumnAttributes_str[ii];
            }
        }

        private void SetForm2_Load(object sender, EventArgs e)
        {
            LoadSetData();
            dataGridViewcnc.ClearSelection();
            dataGridViewPLC.ClearSelection();
            dataGridViewrfid.ClearSelection();
            //label6.Text = "";
        }

        private void ChangeGridViewColumns(DataGridView dgv)
        {
            int c = 0;
            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                if (dgv.Columns[i].Visible)
                    c++;
            }
            int with = dgv.Width / c;
            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                if (dgv.Columns[i].Visible)
                {
                    dgv.Columns[i].Width = with - 1;
                    dgv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
        }

        private void LoadSetData()
        {
            textBox2.Text = LineMainForm.m_xml.m_Read(m_xmlDociment.Path_linetype, -1, m_xmlDociment.Default_Attributes_linetype[0]);
            //textBox5.Text = LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNCLocalIp, -1, m_xmlDociment.Default_Attributes_CNCLocalIp[0]);
            refreshXmlData2DGV(m_xmlDociment.PathRoot_CNC, dataGridViewcnc);            
            refreshXmlData2DGV(m_xmlDociment.PathRoot_PLC, dataGridViewPLC);
            refreshXmlData2DGV(m_xmlDociment.PathRoot_RFID, dataGridViewrfid);            
        }

        private void dataGridViewcnc_SizeChanged(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            ChangeGridViewColumns(dgv);
            dgv.ClearSelection();
        }

        private void dataGridViewPLC_SizeChanged(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            ChangeGridViewColumns(dgv);
            dgv.ClearSelection();
        }

        private void dataGridViewrfid_SizeChanged(object sender, EventArgs e)
        {
            DataGridView dgv = sender as DataGridView;
            ChangeGridViewColumns(dgv);
            dgv.ClearSelection();
        }

        private string dataGridViewcnc_CellOld;
        private void dataGridViewcnc_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                dataGridViewcnc_CellOld = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            else
            {
                dataGridViewcnc_CellOld = "";
            }
        }

        private string dataGridViewPLC_CellOld;
        private void dataGridViewPLC_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                dataGridViewPLC_CellOld = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            else
            {
                dataGridViewPLC_CellOld = "";
            }
        }

        private string dataGridViewrfid_CellOld;
        private void dataGridViewrfid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                dataGridViewrfid_CellOld = ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            }
            else
            {
                dataGridViewrfid_CellOld = "";
            }
        }

        private void WriteLog(int lodeindex, int level, string eventid, string keyword, string eventdata)
        {
            LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
            SendParm.Node1NameIndex = lodeindex;
            SendParm.LevelIndex = level;
            //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
            SendParm.EventID = eventid;
            SendParm.Keywords = keyword;
            SendParm.EventData = eventdata;
            LineMainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(LineMainForm.m_Log.AddLogMsgHandlerFinished), "");
        }

        /*private void buttonCCD_Click(object sender, EventArgs e)
        {
            System.Net.IPAddress Ip;            
            if(!System.Net.IPAddress.TryParse(textBox5.Text, out Ip))
            {
                MessageBox.Show("CCD系统IP格式不对！", " 提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox5.Text = LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNCLocalIp, -1, m_xmlDociment.Default_Attributes_CNCLocalIp[0]);
                return;
            }

            if (textBox5.Text != LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNCLocalIp, -1, m_xmlDociment.Default_Attributes_CNCLocalIp[0]))
            {
                DialogResult select = MessageBox.Show("CCD系统IP改变，是否保存？", " 提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (select != DialogResult.OK)
                {
                    textBox5.Text = LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNCLocalIp, -1, m_xmlDociment.Default_Attributes_CNCLocalIp[0]);
                }
                else
                {
                    LineMainForm.m_xml.m_UpdateAttribute(m_xmlDociment.PathRoot_CNCLocalIp, -1, m_xmlDociment.Default_Attributes_CNCLocalIp[0], textBox5.Text);
                    LineMainForm.m_xml.SaveXml2File(LineMainForm.XMLSavePath);
                    WriteLog((int)LogData.Node1Name.System_security, (int)LogData.Node2Level.AUDIT, ((int)LogData.Node2Level.AUDIT).ToString(), "CCD配置", string.Format("{0}修改了CCD系统的IP配置.", LogUser));
                }
            }
        }*/

        private void buttonsetlinename_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                MessageBox.Show("产线名称不能为空！", " 提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Text = LineMainForm.m_xml.m_Read(m_xmlDociment.Path_linetype, -1, m_xmlDociment.Default_Attributes_linetype[0]);
                return;
            }

            if (textBox2.Text != LineMainForm.m_xml.m_Read(m_xmlDociment.Path_linetype, -1, m_xmlDociment.Default_Attributes_linetype[0]))
            {
                DialogResult select = MessageBox.Show("产线名称改变，是否保存？", " 提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (select != DialogResult.OK)
                {
                    textBox2.Text = LineMainForm.m_xml.m_Read(m_xmlDociment.Path_linetype, -1, m_xmlDociment.Default_Attributes_linetype[0]);
                }
                else
                {
                    LineMainForm.m_xml.m_UpdateAttribute(m_xmlDociment.Path_linetype, -1, m_xmlDociment.Default_Attributes_linetype[0], textBox2.Text);
                    LineMainForm.m_xml.SaveXml2File(LineMainForm.XMLSavePath);
                    WriteLog((int)LogData.Node1Name.System_security, (int)LogData.Node2Level.AUDIT, ((int)LogData.Node2Level.AUDIT).ToString(), "产线名称", string.Format("{0}修改了产线名称.", LogUser));
                }
            }
        }

        private void buttonsetcnc_Click(object sender, EventArgs e)
        {
            int value;
            if (!int.TryParse(textBoxcnccount.Text, out value))
            {
                MessageBox.Show("数量无法识别！");
            }

            if (dataGridViewcnc.TabIndex == 1)
            {
                DialogResult select;
                select = MessageBox.Show("CNC配置数据改变，是否保存？", " 提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (select != DialogResult.OK)
                {
                    refreshXmlData2DGV(m_xmlDociment.PathRoot_CNC, dataGridViewcnc);
                    BindingLisReadValue(dataGridViewcnc);
                }
                else
                {
                    refreshDgvData2Xml(dataGridViewcnc, m_xmlDociment.PathRoot_CNC);
                    LineMainForm.m_xml.SaveXml2File(LineMainForm.XMLSavePath);
                    refreshXmlData2DGV(m_xmlDociment.PathRoot_CNC, dataGridViewcnc);
                    BindingLisReadValue(dataGridViewcnc);
                    WriteLog((int)LogData.Node1Name.System_security, (int)LogData.Node2Level.AUDIT, ((int)LogData.Node2Level.AUDIT).ToString(), "CNC配置", string.Format("{0}修改了CNC配置.", LogUser));
                }
                RefreshDgvDataChangeF(dataGridViewcnc, 0);
            }
        }


        private void BindingLisReadValue(DataGridView dgv)
        {
            foreach (Binding m_b in m_bindingList)
            {
                if (((DataGridView)(m_b.DataSource)) == dgv)
                {
                    m_b.ReadValue();
                }
            }
        }

        private void dataGridViewcnc_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridViewcnc_CellOld;
                return;
            }

            if (e.ColumnIndex == (int)m_xmlDociment.Attributes_str1.ip)//IP合法性检查
            {
                IPAddress ip;
                if (!IPAddress.TryParse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out ip))
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridViewcnc_CellOld;
                    return;
                }
            }
            else if (e.ColumnIndex == (int)m_xmlDociment.Attributes_str1.port)//Port合法性检查
            {
                int value;
                string cellvalue = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                if (!int.TryParse(cellvalue, out value) || value <= 0 || value > 65535)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridViewcnc_CellOld;
                    return;
                }
            }

            if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null && !string.Equals(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value, dataGridViewcnc_CellOld))
            {
                RefreshDgvDataChangeF(dgv, 1);
            }
            dgv.ClearSelection();
        }

        private void dataGridViewPLC_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridViewPLC_CellOld;
                return;
            }

            if (e.ColumnIndex == (int)m_xmlDociment.Attributes_str1.ip)//IP合法性检查
            {
                IPAddress ip;
                if (!IPAddress.TryParse(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out ip))
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridViewPLC_CellOld;
                    return;
                }
            }
            else if (e.ColumnIndex == (int)m_xmlDociment.Attributes_str1.port)//Port合法性检查
            {
                int value;
                string cellvalue = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                if (!int.TryParse(cellvalue, out value) || value <= 0 || value > 65535)
                {
                    dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridViewPLC_CellOld;
                    return;
                }
            }

            if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null && !string.Equals(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value, dataGridViewPLC_CellOld))
            {
                RefreshDgvDataChangeF(dgv, 1);
            }
            dgv.ClearSelection();
        }

        private void buttonsetplc_Click(object sender, EventArgs e)
        {
            int value;
            if (!int.TryParse(textBoxplccount.Text, out value))
            {
                MessageBox.Show("数量无法识别！");
            }

            if (dataGridViewPLC.TabIndex == 1)
            {
                DialogResult select;
                select = MessageBox.Show("PLC配置数据改变，是否保存？", " 提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (select != DialogResult.OK)
                {
                    refreshXmlData2DGV(m_xmlDociment.PathRoot_PLC, dataGridViewPLC);
                    BindingLisReadValue(dataGridViewPLC);
                }
                else
                {
                    refreshDgvData2Xml(dataGridViewPLC, m_xmlDociment.PathRoot_PLC);
                    LineMainForm.m_xml.SaveXml2File(LineMainForm.XMLSavePath);
                    refreshXmlData2DGV(m_xmlDociment.PathRoot_PLC, dataGridViewPLC);
                    BindingLisReadValue(dataGridViewPLC);
                    WriteLog((int)LogData.Node1Name.System_security, (int)LogData.Node2Level.AUDIT, ((int)LogData.Node2Level.AUDIT).ToString(), "PLC配置", string.Format("{0}修改了PLC配置.", LogUser));
                }
                RefreshDgvDataChangeF(dataGridViewPLC, 0);
            }
        }

        private void dataGridViewrfid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
            {
                dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = dataGridViewrfid_CellOld;
                return;
            }

            if (dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null && !string.Equals(dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value, dataGridViewrfid_CellOld))
            {
                RefreshDgvDataChangeF(dgv, 1);
            }
            dgv.ClearSelection();
        }

        private void buttonsetrfid_Click(object sender, EventArgs e)
        {
            int value;
            if (!int.TryParse(textBoxrfidcount.Text, out value))
            {
                MessageBox.Show("数量无法识别！");
            }

            if (dataGridViewrfid.TabIndex == 1)
            {
                DialogResult select;
                select = MessageBox.Show("RFID配置数据改变，是否保存？", " 提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (select != DialogResult.OK)
                {
                    refreshXmlData2DGV(m_xmlDociment.PathRoot_RFID, dataGridViewrfid);
                    BindingLisReadValue(dataGridViewrfid);
                }
                else
                {
                    refreshDgvData2Xml(dataGridViewrfid, m_xmlDociment.PathRoot_RFID);
                    LineMainForm.m_xml.SaveXml2File(LineMainForm.XMLSavePath);
                    refreshXmlData2DGV(m_xmlDociment.PathRoot_RFID, dataGridViewrfid);
                    BindingLisReadValue(dataGridViewrfid);
                    WriteLog((int)LogData.Node1Name.System_security, (int)LogData.Node2Level.AUDIT, ((int)LogData.Node2Level.AUDIT).ToString(), "RFID配置", string.Format("{0}修改了RFID配置.", LogUser));
                }
                RefreshDgvDataChangeF(dataGridViewrfid, 0);
            }
        }


        public static bool PingTest(string ip, int timeout)
        {
            bool connect = false;
            Ping pingtest = new Ping();
            try
            {
                PingReply reply = pingtest.Send(ip, timeout);
                if (reply.Status == IPStatus.Success)
                    connect = true;
            }
            catch
            {
            }
            return connect;
        }

        /*private void buttontest_Click(object sender, EventArgs e)
        {
            IPAddress ip;
            if (!IPAddress.TryParse(textBox5.Text, out ip))
            {
                MessageBox.Show("IP地址格式不对！");
                return;
            }
            buttontest.Enabled = false;
            Task.Run(() => PingTips(textBox5.Text));
        }

        void PingTips(string Ip)
        {
            label6.InvokeEx(c => {
                c.Text = "开始连接...";
            });
            bool res = PingTest(Ip, 300);
            if (res)
            {
                label6.InvokeEx(c =>
                {
                    c.Text = "通讯成功!";
                });
            }
            else
            {
                label6.InvokeEx(c =>
                {
                    c.Text = "通讯失败!";
                });
            }
            Thread.Sleep(800);
            label6.InvokeEx(c =>
            {
                c.Text = "";
            });
            buttontest.InvokeEx(c => {
                c.Enabled = true;
            });
        }*/

      

        private bool FindUserName(string user, out int index)
        {
            index = 0;
            bool res = false;
            int value = 0;
            string get_str = LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_User, -1, m_xmlDociment.Default_Attributes_str1[0]);
            if (int.TryParse(get_str, out value))
            {
                if (value <= 0)
                {
                    return res;
                }
                else
                {
                    for(int i=0; i< value; i++)
                    {
                        if (user == LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_User, i, m_xmlDociment.Default_Attributes_User[0]))
                        {
                            res = true;
                            index = i;
                            return res;
                        }
                    }
                }
            }
            return res;
        }

        private void buttonzhuce_Click(object sender, EventArgs e)
        {
            if (textBox4.Text == "")
            {
                MessageBox.Show("注册的用户名不能为空！");
                return;
            }

            if (textBox3.Text == "")
            {
                MessageBox.Show("注册的密码不能为空！");
                return;
            }

            if (textBox3.Text != textBox1.Text)
            {
                MessageBox.Show("两次输入的密码不一致！");
                return;
            }
            int index = 0;
            bool res = FindUserName(textBox4.Text, out index);
            if (res)
            {
                MessageBox.Show("注册的用户名已存在！");
            }
            else
            {
                LineMainForm.m_xml.InserNode(m_xmlDociment.PathRoot_User);
                LineMainForm.m_xml.m_UpdateAttribute(m_xmlDociment.PathRoot_User, -2, m_xmlDociment.Default_Attributes_User[0], textBox4.Text);
                LineMainForm.m_xml.m_UpdateAttribute(m_xmlDociment.PathRoot_User, -2, m_xmlDociment.Default_Attributes_User[1], textBox1.Text);
                LineMainForm.m_xml.SaveXml2File(LineMainForm.XMLSavePath);
                MessageBox.Show("注册成功！");
            }
        }

        private void buttonlogin_Click(object sender, EventArgs e)
        {
            if (textBoxname.Text == "")
            {
                MessageBox.Show("登录的用户名不能为空！");
                return;
            }

            if (textBoxkey.Text == "")
            {
                MessageBox.Show("登录的密码不能为空！");
                return;
            }
            int index = 0;
            if (textBoxname.Text == "admin")
            {
                if (textBoxkey.Text == "123456")
                {
                    LogUser = textBoxname.Text;
                    labelusername.Text = textBoxname.Text;
                    if (LogUser == "admin")
                    {
                        OnOpenTestButton();
                    }
                }
                else
                {
                    MessageBox.Show("登录的密码不正确！");
                }
                return;
            }
            bool res = FindUserName(textBoxname.Text, out index);
            if (res)
            {
                if (textBoxkey.Text == LineMainForm.m_xml.m_Read(m_xmlDociment.PathRoot_User, index, m_xmlDociment.Default_Attributes_User[1]))
                {
                    MessageBox.Show("登录成功！");
                    LogUser = textBoxname.Text;
                    labelusername.Text = textBoxname.Text;
                    if (LogUser == "admin")
                    {
                        OnOpenTestButton();
                    }
                }
                else
                {
                    MessageBox.Show("登录的密码不正确！");
                }
            }
            else
            {
                MessageBox.Show("登录的用户名不存在！");
            }
        }



        //NVRForm nVRForm;

        //private void Button1_Click(object sender, EventArgs e)
        //{
        //    if(nVRForm != null)
        //        nVRForm.Dispose();
        //    nVRForm = new NVRForm();
        //    nVRForm.Show();
        //}
    }
}
