using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
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
using System.Collections;

namespace HelloWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string gfinpath, gfoutpath;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            gfinpath = this.Src.Text;
            gfoutpath=this.Dest.Text;
            if (!Directory.Exists(gfinpath) || !Directory.Exists(gfinpath))
            {
                print("Invalid Dir");
            }
            else
            {
                parseall(gfinpath);
                print("genete all");
            }
            this.Src.Text = "";
            this.Dest.Text = "";
            gfinpath = "";
            gfoutpath = "";
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            
        }

        static private Dictionary<(byte, byte), string> fileheadmap = new Dictionary<(byte, byte), string>()
        {
            { (0x42,0x4D),"bmp"},
            { (0x47,0x49) ,"gif"},
            {(0x89,0x50),"png"},
            {(0xFF,0xD8),"jpg"},
            { (0x49,0x49),"tif"},
            { (0x00,0x00),"ico"}
        };

        (string, string) getinfo(string filepath)
        {
            string[] item = filepath.Split(new char[] { '.' });
            if (item.Length != 2) return ("", "");
            return (item[0], item[1]);
        }

        (byte, string) getfiletype((byte, byte) h)
        {
            foreach (var kv in fileheadmap)
            {
                var k = kv.Key;
                var v = kv.Value;
                if ((k.Item1 ^ h.Item1) == (k.Item2 ^ h.Item2))
                {
                    return (Convert.ToByte(k.Item1 ^ h.Item1), v);
                }
            }
            return (0, "");
        }

        bool parse(string filepath, string finpath)
        {
            var info = getinfo(filepath);
            if (info.Item2 == "" || info.Item2 != "dat")
            {
                print("not a dat file");
                return false;
            }
            FileStream fin = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            int n = (int)fin.Length;
            byte[] buffer = new byte[n];
            fin.Read(buffer, 0, n);
            fin.Close();
            var filetype = getfiletype((buffer[0], buffer[1]));
            if (filetype.Item2 == "")
            {
                print("unknown file type");
                return false;
            }
            var key = filetype.Item1;
            for (int i = 0; i < n; ++i)
            {
                buffer[i] ^= key;
            }
            string fileoutpath = info.Item1 + "." + filetype.Item2;
            fileoutpath = fileoutpath.Replace(finpath, gfoutpath);
            FileStream fout = new FileStream(fileoutpath, FileMode.Create, FileAccess.Write);
            fout.Write(buffer,0,buffer.Length);
            fout.Close();
            return true;
        }


        private void parseall(string filepath)
        {
            DirectoryInfo thefolder = new DirectoryInfo(filepath);
            foreach (FileInfo file in thefolder.GetFiles())
            {
                parse(file.FullName, filepath);
            }
            foreach (DirectoryInfo dir in thefolder.GetDirectories())
            {
                parseall(dir.FullName);
            }
        }


        private void print(string data)
        {
            this.Info.Text=data;
        }
    }
}
