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
using EncryptionTool.Model.CRYPTON;

namespace EncryptionTool.WinFormView
{
    public partial class MainForm : Form
    {
        private IEncryptionMethod _method;

        public MainForm()
        {
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

            if (string.IsNullOrEmpty(KeyTextBox.Text) || KeySizeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Key or/and its size is/are not defined");
                return;
            }

            var keySize = Int32.Parse(KeySizeComboBox.Items[KeySizeComboBox.SelectedIndex].ToString());
            var keyBytes = Encoding.UTF8.GetBytes(KeyTextBox.Text);

            if (keySize != keyBytes.Length * 8)
            {
                MessageBox.Show("Key has incorrect length");
                return;
            }

            _method = new CryptonMethod(keyBytes);
            
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

        private void GenerateKeyButton_Click(object sender, EventArgs e)
        {
            if (KeySizeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select key size first");
                return;
            }

            var keySize = Int32.Parse(KeySizeComboBox.Items[KeySizeComboBox.SelectedIndex].ToString());
            var key = new byte[keySize / 8];
            var random = new Random();
            
            for (int i = 0; i < key.Length; ++i)
            {
                key[i] = (byte)random.Next(1, 127);
            }

            KeyTextBox.Text = Encoding.UTF8.GetString(key);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Encription/Decripting file is not defined!");
                return;
            }

            if (string.IsNullOrEmpty(KeyTextBox.Text) || KeySizeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Key or/and its size is/are not defined");
                return;
            }

            var keySize = Int32.Parse(KeySizeComboBox.Items[KeySizeComboBox.SelectedIndex].ToString());
            var keyBytes = Encoding.UTF8.GetBytes(KeyTextBox.Text);

            if (keySize != keyBytes.Length * 8)
            {
                MessageBox.Show("Key has incorrect length");
                return;
            }

            _method = new CryptonMethod(keyBytes);

            var dlg = new SaveFileDialog();
            if (dlg.ShowDialog() == DialogResult.Cancel)
                return;

            var resultPath = dlg.FileName;
            var sourcePath = textBox1.Text;

            try
            {
                byte[] source = File.ReadAllBytes(sourcePath);

                byte[] result = _method.Decode(source);
                File.WriteAllBytes(resultPath, result);

                MessageBox.Show($"File {textBox1.Text} encoded/decoded. Good Luck.");
            }
            catch (IOException ex)
            {
                MessageBox.Show($"File {textBox1.Text} is not found or Access denied!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error has been occured during enconding/decoding file.\n{ ex.Message }");
            }
        }
    }
}
