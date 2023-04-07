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
    public partial class Form_LogFind : Form
    {
        public EventHandler<string> FindEventHandler;
        public Form_LogFind()
        {
            InitializeComponent();
            this.Load += new System.EventHandler(this.Form_LogFind_Load);
            this.button_FindUp.Click += new System.EventHandler(this.button_FindNext_Click);
            this.button_Cans.Click += new System.EventHandler(this.button_Cans_Click);
            this.button_FindNext.Click += new System.EventHandler(this.button_FindNext_Click);
            this.textBox_FindStr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_FindStr_KeyDown);
        }

        public void SettextBox_FindStr(String str)
        {
            textBox_FindStr.Text = str;
        }

        private void button_FindNext_Click(object sender, EventArgs e)
        {
            if (FindEventHandler != null)
            {
                FindEventHandler.Invoke(sender, textBox_FindStr.Text);
            }
        }

        private void button_Cans_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            //Close();
        }

        private void textBox_FindStr_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (FindEventHandler != null)
                {
                    FindEventHandler.Invoke(this, textBox_FindStr.Text);
                }
            }
        }

        private void Form_LogFind_Load(object sender, EventArgs e)
        {
            ChangeLanguage.LoadLanguage(this);//zxl 4.19
        }

        public Button GetFindUP()
        {
            return button_FindUp;
        }

        public Button GetFindNext()
        {
            return button_FindNext;
        }
    }
}
