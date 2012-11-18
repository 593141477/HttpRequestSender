using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace HttpRequestSender
{
    public partial class Form_Response : Form
    {
        private WebHeaderCollection headers;
        public Form_Response(WebHeaderCollection h)
        {
            InitializeComponent();
            headers = h;
        }

        private void Form_Response_Load(object sender, EventArgs e)
        {
            if (headers == null)
                return;
            foreach (string key in headers.AllKeys)
            {
                listBox1.Items.Add(key + ": " + headers[key]);
            }
        }
    }
}
