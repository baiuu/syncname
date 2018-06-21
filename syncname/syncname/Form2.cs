using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace syncname
{
    public partial class Form2 : Form
    {

        public Form2()
        {
            InitializeComponent();
        }
        void Sync(string root)
        {
            var dict = File.ReadAllLines(textBox3.Text)
                 .Select(l => l.Split('|'))
                 .ToDictionary(v => v[0], v => v[1]);
            var md5 = MD5.Create();
            var files = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                string hash;
                using (var fs = File.OpenRead(f))
                {
                    var bytes = md5.ComputeHash(fs);
                    hash = string.Concat(bytes.Select(b => b.ToString("X2")));
                }
                if (dict.TryGetValue(hash,out var path))
                {
                    var dest = Path.Combine(root, path);
                    if (dest != f)
                    {
                        string x="";
                        var dir = Path.GetDirectoryName(dest);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        File.Move(f, dest);
                        x = x + "MV:" + f + "=>" + dest + "/r/n/t/t";
                        textBox2.Text = x;
                    }
                }
                else
                    MessageBox.Show("异常");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                textBox1.Text = foldPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            Sync(textBox1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "sync文件(*.txt;*.sync)|*.txt;*.sync|所有文件|*.*";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;

            if (ofd.ShowDialog() == DialogResult.OK)

            {
                textBox3.Text = ofd.FileName;
                //其他代码
            }
        }
    }
}
