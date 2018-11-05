using EncryptionTool.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace EncryptionTool.WinFormView
{
    public partial class MainForm : Form
    {
        private IEncryptionMethod _method;

        public MainForm()
        {
            try
            {
                var a1 = Encoding.UTF8.GetBytes("123456789qwerqerlejwasdfasdffjфвыафываdjasfldjlasdkfj;adkakljasdlfasd");
                var a2 = Encoding.UTF8.GetBytes("123456789afsdfljdhlaasdfsdjkfjaasdfasdfkasdlk;fjasd;fjkasd;fkjasdfasd");
                var a3 = Encoding.UTF8.GetBytes("123456789asdfasdjhsdsdfgsdf g sdfg sdf gfd sg dsfg fdghdfghdfhgdf54 y4 y err f");

                _method = new PikeMethod(a1, a2, a3);
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error has been occured during program initialization.\n{ ex.Message }");
                throw;
            }
            
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();

            if (dlg.ShowDialog() == DialogResult.Cancel)
                return;

            string filename = dlg.FileName;
            textBox1.Text = filename;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Encription/Decripting file is not defined!");
                return;
            }

            var dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == DialogResult.Cancel)
                return;

            var resultPath = dlg.FileName;
            var sourcePath = textBox1.Text;

            try
            {
                byte[] source = File.ReadAllBytes(sourcePath);

                byte[] result = _method.Encode(source);
                File.WriteAllBytes(resultPath, result);

                MessageBox.Show($"File {textBox1.Text} encoded/decoded. Good Luck.");
            }
            catch(IOException ex)
            {
                MessageBox.Show($"File {textBox1.Text} is not found or Access denied!");
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Error has been occured during enconding/decoding file.\n{ ex.Message }");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
