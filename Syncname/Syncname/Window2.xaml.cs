using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Syncname
{
    /// <summary>
    /// Window2.xaml 的交互逻辑
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }
        private FileStream currentFs;
        List<string[]> lines = new List<string[]>();
        private void Sync(string root)
        {
            BT2.IsEnabled = false;
            BT1.IsEnabled = false;
            var dict = File.ReadAllLines(TextBlock2.Text)
                 .Select(l => l.Split('|'))
                 .ToDictionary(v => v[0], v => v[1]);
            var files = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            string x = "";
            for (int i = 0; i < files.Length; i++)
            {

                if (dict.TryGetValue(lines[i][0], out var path))
                {
                    var dest1 = root + System.IO.Path.Combine(root, lines[i][1]);
                    var dest = System.IO.Path.Combine(root, path);
                    if (dest != lines[i][0])
                    {
                        var dir = System.IO.Path.GetDirectoryName(dest);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        File.Move(dest1, dest);
                        x = x + "MV:" + lines[i][1] + "=>" + dest + "/r/n/t/t";
                        TextBox1.Text = x;
                    }
                }

            }
            button1.IsEnabled = true;
            BT2.IsEnabled = true;
        }
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
                        string[] line = { hash,f.Substring(root.Length)};
                        lines.Add(line);
                        currentFs = null;
                    }
                    Dispatcher.Invoke(() => pb2.Value = i + 1);
                }
            });
            pb1.Value = 1;
            BT1.IsEnabled = true;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (currentFs != null)
            {
                var v = currentFs.Position / (double)currentFs.Length;
                pb1.Value = v;
            }
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Owner.Visibility = Visibility.Visible;//显示父窗体
            base.OnClosing(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "sync文件(*.txt;*.sync)|*.txt;*.sync|所有文件|*.*";
            ofd.ValidateNames = true;
            ofd.CheckPathExists = true;
            ofd.CheckFileExists = true;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)

            {
                TextBlock2.Text = ofd.FileName;
                //其他代码
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Sync(TextBlock1.Text);
        }
    }
}
