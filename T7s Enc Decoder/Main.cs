using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;


namespace T7s_Enc_Decoder
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }


        private void ChoosePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "选择要打开的文件",
                Multiselect = true,
                Filter = "加密文件|*.json|加密文件|*.enc|加密文件|*.*",
                RestoreDirectory = true
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {

                foreach (var filePath in ofd.FileNames)
                {
                    DecryptFiles.DecryptFile(filePath);
                    //DecryptFiles.EncryptFile(filePath);
                }



            }
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }
    }


}
