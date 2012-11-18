using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace HttpRequestSender
{
    public partial class Form1 : Form
    {
        private byte[] request_content = { };
        private byte[] response_content = { };
        private WebHeaderCollection response_headers;
        private const long MaxAllowedSize = 1024 * 1024 * 2;
        public const string ProjectLink = "http://github.com/593141477/HttpRequestSender";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.ValidateNames = true;
                dialog.CheckPathExists = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(dialog.FileName, FileMode.Create))
                    {
                        BinaryWriter bw = new BinaryWriter(fs);
                        bw.Write(response_content);
                        bw.Close();
                        fs.Close();
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Exception");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if ("GET".Equals(comboBox1.SelectedItem as string))
                    ProcessResponse(HttpHelper.HttpGet(textBox1.Text));
                else if ("POST".Equals(comboBox1.SelectedItem as string))
                {
                    if (textBox2.Modified)
                        ProcessResponse(HttpHelper.HttpPost(textBox1.Text, System.Text.Encoding.Default.GetBytes(textBox2.Text)));
                    else
                        ProcessResponse(HttpHelper.HttpPost(textBox1.Text, request_content));
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Exception");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label2.Text = "";
            textBox2.Text = "";
            request_content = null;
            request_content = new byte[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.ValidateNames = true;
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    long fileSize = (new FileInfo(dialog.FileName)).Length;
                    if (fileSize > MaxAllowedSize)
                        throw new Exception("File is too large");
                    using (FileStream fs = new FileStream(dialog.FileName, FileMode.Open))
                    {
                        BinaryReader br = new BinaryReader(fs);
                        request_content = br.ReadBytes((int)fileSize);
                        br.Close();
                        fs.Close();
                    }
                    label2.Text = dialog.SafeFileName;
                    textBox2.Text = System.Text.Encoding.Default.GetString(request_content);
                    textBox2.Modified = false;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Exception");
            }
        }

        private void ProcessResponse(HttpWebResponse result)
        {
            response_headers = result.Headers;

            byte[] buffer = new byte[16 * 1024];
            Stream stream = result.GetResponseStream();
            using (MemoryStream ms = new MemoryStream())
            {
                int byteRead;
                while ((byteRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    ms.Write(buffer, 0, byteRead);
                response_content = ms.ToArray();
            }
            textBox3.Text = System.Text.Encoding.Default.GetString(response_content);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(ProjectLink);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (Form_Response form = new Form_Response(response_headers))
            {
                form.ShowDialog();
            }
        }

    }
    public class HttpHelper
    {
        public static HttpWebResponse HttpGet(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.Timeout = 5000;
            return request.GetResponse() as HttpWebResponse;
        }
        public static HttpWebResponse HttpPost(string url, byte[] content)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.Timeout = 5000;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = content.Length;

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(content, 0, content.Length);
                stream.Close();
            }
            return request.GetResponse() as HttpWebResponse;
        }
    }
}
