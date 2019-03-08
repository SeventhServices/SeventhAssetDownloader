using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace T7s_Sig_Counter
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            //MessageBox.Show(Counter.GetParam("63313430623432652c653630632e36316061293e6266352c3767323565653f353434673f"));
            textBox1.Text = Counter.GetParam("63313430623432652c653630632e36316061293e6266352c3767323565653f353434673f");
        }

    }
}
