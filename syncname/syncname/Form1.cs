using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace syncname
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<string> lines = new List<string>();
        void Create(string root)
        {
            var md5 = MD5.Create();
            var files = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                using (var fs = File.OpenRead(f))
                {
                    var bytes = md5.ComputeHash(fs);
                    var hash = string.Concat(bytes.Select(b => b.ToString("X2")));
                    var line =hash+ $"|{f.Substring(root.Length).TrimStart('\\')}";
                    lines.Add(line);
                }
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
            string a = "";
            Create(textBox1.Text);
            foreach (string txt in lines)
            {
                a = a + txt + "/r/n/t/t";
                textBox2.Text = a;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "sync文件(*.sync)|*.sync|TXT文件|*.txt";
            sfd.ShowDialog();
            File.WriteAllLines(Path.Combine(sfd.FileName), lines); 
            
        }
    }
}
