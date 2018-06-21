using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Threading;

namespace Syncname
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }
        private FileStream currentFs;
        List<string> lines = new List<string>();
        private async void Create(string root)
        {
            button1.IsEnabled = false;
            var timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(0.1)
            };

            timer.Tick += Timer_Tick;
            timer.Start();
            var md5 = MD5.Create();
            var files = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            pb2.Maximum = files.Length;
            pb1.Maximum = 1;
            await Task.Run(() =>
            {
                for (int i = 0; i < files.Length; i++)
                {
                    var f = files[i];
                    using (var fs = File.OpenRead(f))
                    {
                        currentFs = fs;
                        var bytes = md5.ComputeHash(fs);
                        var hash = string.Concat(bytes.Select(b => b.ToString("X2")));
                        var line = $"{ hash}|{f.Substring(root.Length).TrimStart('\\')}";
                        lines.Add(line);
                        currentFs = null;
                    }
                    Dispatcher.Invoke(() => pb2.Value = i + 1);
                }
            });
            button2.IsEnabled = true;
            button3.IsEnabled = true;
            pb1.Value = 1;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (currentFs != null)
            {
                var v = currentFs.Position / (double)currentFs.Length;
                pb1.Value = v;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                TextBlock1.Text = foldPath;
            }
            if (TextBlock1.Text != "")
            {
                Create(TextBlock1.Text);
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Visibility = Visibility.Visible;//显示父窗体
            base.OnClosing(e);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TextBox1.Clear();
            string a = "";
            foreach (string txt in lines)
            {
                a = a + txt + "/r/n/t/t";
                TextBox1.Text = a;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "sync文件(*.sync)|*.sync|TXT文件|*.txt";
            sfd.ShowDialog();
            File.WriteAllLines(System.IO.Path.Combine(sfd.FileName), lines);
        }
    }
}
