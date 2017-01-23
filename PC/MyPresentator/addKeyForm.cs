using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsInput.Native;

namespace MyPresentator
{
    public partial class addKeyForm : Form
    {
        DataGridView dataGrid = null;
        String irCode = null;

        public addKeyForm()
        {
            InitializeComponent();
        }
        public addKeyForm(DataGridView d, String code)
        {
            InitializeComponent();
            dataGrid = d;
            irCode = code;
        }

        private void addKeyForm_Load(object sender, EventArgs e)
        {
            labelCode.Text = irCode;
            foreach (VirtualKeyCode volume in Enum.GetValues(typeof(VirtualKeyCode)))
            {
                comboBox1.Items.Add(volume);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                VirtualKeyCode vkc = ((VirtualKeyCode)comboBox1.SelectedItem);
                dataGrid.Rows.Add(new Object[] { irCode, vkc });
                Close();
            }
            catch (Exception) { }
        }
    }
}
