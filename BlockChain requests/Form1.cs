using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Net;

namespace BlockChain_requests
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Deploy")
            {
                label4.Enabled = false;
                textBox3.Enabled = false;
                label3.Enabled = true;
                textBox2.Enabled = true;
            } else if(comboBox1.Text == "Query" || comboBox1.Text == "Invoke")
            {
                label4.Enabled = true;
                textBox3.Enabled = true;
                label3.Enabled = false;
                textBox2.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dynamic main = new JObject();
            main.jsonrpc = "2.0";
            main.method = comboBox1.Text.ToLower();

            dynamic parameters = new JObject();
            parameters.type = 1;

            dynamic chaincode = new JObject();
            if (comboBox1.Text == "Deploy")
            {
                chaincode.path = textBox2.Text;
            } else
            {
                chaincode.name = textBox3.Text;
            }

            dynamic ctor = new JObject();
            ctor.function = textBox4.Text;
            

            JArray args = new JArray();
            string[] a = textBox5.Text.Split(',');
            int i = 0;
            foreach(string c in a)
            {
                args.Add(c);
                i++;
            }
            ctor.args = args;

            parameters.chaincodeID = chaincode;
            parameters.ctorMsg = ctor;
            parameters.secureContext = textBox7.Text;

            main.@params = parameters;
            main.id = 0;


            string uri = textBox1.Text + "/" + "chaincode";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {               
                streamWriter.Write(main.ToString());
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    textBox6.Text = streamReader.ReadToEnd();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}

