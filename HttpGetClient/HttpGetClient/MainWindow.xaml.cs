using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace HttpGetClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private static String key = "19821005";
        //private String urlBase = "https://127.0.0.1:8080/server/Server?value=";
        private String urlBase = "https://192.168.1.120/server/";
        private String commandReslut = null;
        private int unitSize = 1024;
        private System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
        private Random random = new Random();

        private DataModel dataModel = new DataModel();

        public MainWindow()
        {
            this.DataContext = dataModel;
            InitializeComponent();
            ReflashFileList();

            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

           

        }

        private bool UploadFile(string p)
        {
            //throw new NotImplementedException();
            String name=System.IO.Path.GetFileName(p);
            if (!SendCreateCommand(name))
            {
                return false;
            }

            if (!SendAppendCommand(p))
            {
                return false;
            }
            return true;
        }

        private bool SendAppendCommand(string p)
        {
            System.IO.FileStream fs = new System.IO.FileStream(p, System.IO.FileMode.Open);
            dataModel.FileSize = (int)fs.Length;
            byte[] buf = new byte[unitSize];
            int count = fs.Read(buf, 0, buf.Length);
            int start = 0;
            while (count > 0)
            {
                // append,name,content,start,size
                
                StringBuilder command = new StringBuilder();
                command.Append("append\n");
                String name = System.IO.Path.GetFileName(p);
                command.Append(name+"\n");
                string base64Content = Convert.ToBase64String(buf,0,count);
                command.Append(base64Content + "\n");
                command.Append(start + "\n");
                command.Append(count);
                start += count;

                String encodedCommand = EncodeCommand(command.ToString());
                if (!SendCommand(encodedCommand))
                {
                    fs.Close();
                    return false;
                }

                dataModel.UploadSize = start;

                count = fs.Read(buf, 0, buf.Length);
            }

            fs.Close();

            return true;
        }

        private String EncodeCommand(string command)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {

                des.Key = Encoding.UTF8.GetBytes(key);
                des.IV = Encoding.UTF8.GetBytes(key);
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                byte[] data = UTF8Encoding.UTF8.GetBytes(command.ToString());

                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }

                string base64Command = Convert.ToBase64String(ms.ToArray());
                ms.Close();

                return base64Command;
            }
        }



        public static bool ValidateServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        { 

            return true; 

        }


        private bool SendCommand(String base64Command)
        {
            try
            {
                int r=random.Next(100000);
                String url = urlBase + "Server?value=" + System.Web.HttpUtility.UrlEncode(base64Command)+"&r="+r;

                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                ServicePointManager.ServerCertificateValidationCallback =ValidateServerCertificate;

                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

                //client.
                WebClient client = new WebClient();
                client.Encoding = Encoding.UTF8;

                String resp = client.DownloadString(url);
                commandReslut = resp;
                string[] result = resp.Split('\n');
                if (result[0] != "t")
                {
                    MessageBox.Show(resp);
                }
                return result[0] == "t" ? true : false;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        private bool SendCreateCommand(string name)
        {
            StringBuilder command = new StringBuilder();
            command.Append("create");
            command.Append("\n");
            command.Append(name);

            String encodedCommand = EncodeCommand(command.ToString());
            return SendCommand(encodedCommand);
        }

        private bool SendListCommand()
        {
            StringBuilder command = new StringBuilder();
            command.Append("list");
            String encodedCommand = EncodeCommand(command.ToString());
            return SendCommand(encodedCommand);
        }

        private bool SendDeleteCommand(string name)
        {
            StringBuilder command = new StringBuilder();
            command.Append("delete");
            command.Append("\n");
            command.Append(name);
            String encodedCommand = EncodeCommand(command.ToString());
            return SendCommand(encodedCommand);
        }

        private void Window_Drop_1(object sender, DragEventArgs e)
        {
            String[] files =(String[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0)
            {
                return;
            }
            if (!worker.IsBusy)
            {
                dataModel.Visibility = "Visible";
                worker.RunWorkerAsync(files[0]);
            }

        }

        void worker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("恭喜，文件上传成功！");
            ReflashFileList();
            dataModel.Visibility = "Collapsed";
        }

        void worker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
             UploadFile(e.Argument as string);
        }

        private void buttonReflash_Click(object sender, RoutedEventArgs e)
        {
            ReflashFileList();
        }

        private void ReflashFileList()
        {
            if (SendListCommand())
            {
                string[] results = this.commandReslut.Split('\n');
                listBoxFiles.ItemsSource = results.Where(str=>str.ToUpper().EndsWith(".RAR")); 
            }
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxFiles.SelectedItem != null)
            {
                SendDeleteCommand(listBoxFiles.SelectedItem as string);
            }

            ReflashFileList();
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            this.urlBase = textBoxServer.Text;
        }

        private void textUnitSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.unitSize = Int32.Parse(textUnitSize.Text);
        }

        private void textUnitSize_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
