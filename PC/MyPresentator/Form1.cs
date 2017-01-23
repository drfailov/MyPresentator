using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;
using System.IO.Ports;

namespace MyPresentator
{
    public partial class Form1 : Form
    {
        private SerialPort port = null;
        String clearLogText = "--------";
        InputSimulator inputSimulator = new InputSimulator();

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            refreshCOMs();
            dataGridView1.Columns.Add("ircode", "IR code");
            dataGridView1.Columns.Add("keycode", "Key code");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Thread.Sleep(5000);
            InputSimulator inp = new InputSimulator();
            inp.Keyboard.KeyPress(VirtualKeyCode.F5);
        }
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            refreshCOMs();
        }
        private void refreshCOMs()
        {
            comboBoxComPorts.Items.Clear();
            string[] COMs = SerialPort.GetPortNames();
            int sel = 0;
            for (int i = 0; i < COMs.Length; i++)
            {
                String com = COMs[i];
                if (!com.Equals("COM1"))
                    sel = i;
                comboBoxComPorts.Items.Add(com);
            }
            if(COMs.Length > 0)
                comboBoxComPorts.SelectedIndex = sel;
            writeLog("COM port list loaded.");
        }
        private void writeLog(String text)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { writeLog(text); });
                return;
            }
            // this code will run on main (UI) thread 
            labelCode.Text = (text);
            Application.DoEvents();
            if (!text.Equals(clearLogText))
                delayedClear();
        }
        private void clearLog()
        {
            Thread.Sleep(1000);
            writeLog(clearLogText);
        }
        private void delayedClear()
        {
            new Thread(clearLog).Start();
        }
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            new Thread(connect).Start();
        }
        private void connect()
        {
            if (port != null)
                writeLog("Уже подключено.");
            else
            {
                writeLog("Подключение...");
                try
                {
                    labelStatus.BackColor = Color.FromArgb(255, 255, 195);
                    port = new SerialPort(getSelectedCom(), 9600);
                    port.Open();
                    Thread.Sleep(500);
                    makeJobGreen();
                    beginListening();
                }
                catch (Exception)
                {
                    port.Close();
                    port = null;
                    writeLog("Ошибка подключения.");
                    makeJobRed();
                }
            }
        }
        private String getSelectedCom()
        {
            String text = null;
            this.Invoke((MethodInvoker)delegate()
            {
                text = comboBoxComPorts.Text;
            });
            while (text == null) ;
            return text;
        }
        private bool test()
        {
            sendString("test");
            return receiveString().Equals("OK.");
        }
        private void sendString(String text)
        {
            if (port != null)
            {
                writeLog("|#| <-- " + text);
                port.WriteLine(text);
            }
            else
            {
                writeLog("|X| <-- " + text);
            }
        }
        private String receiveString()
        {
            //todo ДОБАВИТЬ КЭШ С ВОЗМОЖНОСТЬЮ УЧЕТА РАЗМЕРА ОЧЕРЕДИ
            if (port != null)
            {
                try
                {
                    String text = port.ReadLine();
                    writeLog("|#| --> " + text);
                    return text;
                }
                catch (Exception e)
                {
                    return "";
                }
            }
            return "";
        }
        void beginListening()
        {
            new Thread(listen).Start();
        }
        void listen()
        {
            while (port != null)
            {
                String recv = receiveString();
                if (recv != null && !recv.Equals(""))
                {
                    Thread t = new Thread(() => processCode(recv));
                    t.Start();
                    writeLog(recv);
                }
            }
        }
        private void makeJobGreen()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { makeJobGreen(); });
                return;
            }
            // this code will run on main (UI) thread 
            labelStatus.Text = ("Подключено.");
            labelStatus.BackColor = Color.FromArgb(195, 255, 195);
            Application.DoEvents();
        }
        private void makeJobRed()
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { makeJobRed(); });
                return;
            }
            // this code will run on main (UI) thread 
            labelStatus.Text = ("Отключено.");
            labelStatus.BackColor = Color.FromArgb(255, 195, 195);
            Application.DoEvents();
        }
        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            new Thread(disconnect).Start();
        }
        private void disconnect()
        {
            if (port != null)
            {
                writeLog("Отключение...");
                try
                {
                    port.Close();
                    port = null;
                    writeLog("Отключено.");
                    makeJobRed();
                }
                catch (Exception e)
                {
                    writeLog("Ошибка отключения");
                    makeJobRed();
                }
            }
        }
        private void processCode(String code)
        {
            VirtualKeyCode keyCode = getKeyCode(code);
            if (keyCode != VirtualKeyCode.NONAME)
            {
                inputSimulator.Keyboard.KeyPress(keyCode);
            }
            else
            {
                if(!checkBoxLock.Checked)
                    openAddKeyForm(dataGridView1, code);
            }
        }
        private VirtualKeyCode getKeyCode(String code)
        {
            try
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    String ircode = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    if (ircode.Equals(code))
                    {
                        VirtualKeyCode keycode = (VirtualKeyCode)dataGridView1.Rows[i].Cells[1].Value;
                        return keycode;
                    }
                }
            }
            catch (Exception) { }
            return VirtualKeyCode.NONAME;
        }
        private void openAddKeyForm(DataGridView d, String code)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { openAddKeyForm(d, code); });
                return;
            }
            // this code will run on main (UI) thread 
            new addKeyForm(dataGridView1, code).ShowDialog();
            Application.DoEvents();
        }
    }
}
