using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 合并sql
{
    public partial class MergeForm : Form
    {
        string FileName = "";
        public MergeForm()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            comboBox.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    textBox.Text = dlg.SelectedPath;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            //设置文件类型 
            sfd.Filter = "SQL文件（*.sql）|*.sql|文本文件（*.txt）|*.txt";

            //设置默认文件类型显示顺序 
            sfd.FilterIndex = 1;

            //保存对话框是否记忆上次打开的目录 
            sfd.RestoreDirectory = true;

            //设置默认的文件名
            sfd.FileName = DateTime.Now.ToString("yyyyMM");// in wpf is  sfd.FileName = "YourFileName";

            //点了保存按钮进入 

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string localFilePath = sfd.FileName.ToString(); //获得文件路径 
                string fileNameExt = localFilePath.Substring(localFilePath.LastIndexOf("\\") + 1); //获取文件名，不带路径
                FileName = localFilePath;
            }
            else {
                return;
            }
            //string FileName = @"D:\Update" + DateTime.Now.ToString("yyyyMM") + ".sql";
            Thread waitT = new Thread(new ThreadStart(Progress));
            if (textBox.Text == "")
            {
                MessageBox.Show("请选择文件后再合并！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            waitT.IsBackground = true;
            waitT.Start();
        }

        public void Progress()
        {
            int dsa = comboBox.SelectedIndex;
            try
            {
                FileInfo[] myFileInfo = (new DirectoryInfo(textBox.Text)).GetFiles("*.sql");
                string NewFileName = "";
                //空文件夹提示
                if (myFileInfo.Length == 0) {
                    MessageBox.Show("此文件夹内未找到SQL文件！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                for (int i = 0; i < myFileInfo.Length; i++)
                {
                    string str = textBox.Text + "\\" + myFileInfo[i].Name;
                    StreamReader Strsw = new StreamReader(str, comboBox.SelectedIndex == 0 ? Encoding.UTF8 : Encoding.GetEncoding(54936));
                    NewFileName += Strsw.ReadToEnd() + "\r\n\r\n /*******************分割线***************/ \r\n\r\n";
                    Strsw.Close();
                }
                
                FileStream myFs = null;
                if (!File.Exists(FileName))
                {
                    myFs = new FileStream(FileName, FileMode.Create);
                }
                else
                {
                    myFs = new FileStream(FileName, FileMode.Open);
                    MessageBox.Show("文件已存在，将会在原有基础上添加。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    StreamReader Strsw = new StreamReader(myFs, comboBox.SelectedIndex == 0 ? Encoding.UTF8 : Encoding.GetEncoding(54936));
                    NewFileName = Strsw.ReadToEnd() + "\r\n\r\n /*******************分割线***************/ \r\n\r\n" + NewFileName;

                }
                StreamWriter sw = new StreamWriter(myFs, Encoding.UTF8);
                sw.Write(NewFileName);
                sw.Close();
                MessageBox.Show("合并完毕，文件长度" + NewFileName.Length.ToString("n").Split('.')[0] + "KB大小", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
