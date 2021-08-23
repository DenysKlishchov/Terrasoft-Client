using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace ASPControlPC
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openFileBtn_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*";
            if (openFile.ShowDialog() == DialogResult.Cancel)
                return;
            
            textBox1.Text = openFile.FileName;
        }

        private async void analysisBtn_Click(object sender, EventArgs e)
        {
            string JSONData = await Task.Factory.StartNew(() => JsonConvert.SerializeObject(textBox1.Text));
            WebRequest request = WebRequest.Create("https://localhost:44317/Home/GetMetricsAsync");
            request.Method = "POST";

            string query = $"path={JSONData}";

            byte[] byteMass = Encoding.UTF8.GetBytes(query);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteMass.Length;

            using (Stream stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(byteMass, 0, byteMass.Length);
            }

            WebResponse response = await request.GetResponseAsync();

            string answer = null;

            using (Stream s = response.GetResponseStream())
            {
                using (StreamReader sR = new StreamReader(s)) 
                {
                    answer = await sR.ReadToEndAsync();
                }
            }
            response.Close();

            MessageBox.Show(answer);

        }
    }
}
