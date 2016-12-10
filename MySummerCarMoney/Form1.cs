using System;
using System.Windows.Forms;
using System.Diagnostics;

namespace MySummerCarMoney
{

    public partial class Form1 : Form
    {
        int signature_Health, moneyfound = 0;
        string title;
        public int offset;
        public int[] needs = new int[5];
        public Form1()
        {
            InitializeComponent();
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (moneyfound == 0) { MoneyScan.RunWorkerAsync(); button1.Text = "Scanning..."; }
            else
            {
                label1.Text = "Money: " + Memory.ReadFloat(offset);
                string moneyy = textBox1.Text.Replace('.', ',');
                float mony;
                if (float.TryParse(moneyy, out mony) && moneyy != "0" && signature_Health == 1)
                {
                    Memory.WriteFloat(offset, mony);
                    FindProcess_Tick(null, null);
                    textBox1.Clear();
                    textBox1.Focus();
                    label1.Text = "Money: " + Memory.ReadFloat(offset);
                }
            }
        }

        private void RunSigScan_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (needs[0] == 0 && signature_Health == 1) {
                button2.Text = "Scanning....";
                PatternScan ps = new PatternScan();
                //Urine
                needs[0] = ps.FindPattern(new byte[] { 0x68, 0xC6, 0xFF, 0x04, 0x68, 0x04, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF }, "xx?xxx??xxxx????").ToInt32() + 0xC;
                //Hunger
                needs[1] = ps.FindPattern(new byte[] { 0x00, 0xBC, 0xFF, 0xFF, 0x10, 0x05, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF }, "xx??xx??xxxx????").ToInt32() + 0xC;
                //Thirst
                needs[2] = ps.FindPattern(new byte[] { 0xA0, 0xBB, 0xFF, 0xFF, 0x80, 0x04, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF }, "xx??xx??xxxx????").ToInt32() + 0xC;
                //Fatigue
                needs[3] = ps.FindPattern(new byte[] { 0x30, 0xBC, 0xFF, 0xFF, 0x40, 0x05, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF }, "xx??xx??xxxx????").ToInt32() + 0xC;
                //Dirtiness
                needs[4] = ps.FindPattern(new byte[] { 0x60, 0xBC, 0xFF, 0xFF, 0x70, 0x05, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF }, "xx??xx??xxxx????").ToInt32() + 0xC;
                button2.Text = "DONE";
                button3.Enabled = true;
                button2.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RunSigScan.RunWorkerAsync();
        }

        private void MoneyScan_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            PatternScan ps = new PatternScan();
            offset = ps.FindPattern(new byte[] { 0x90, 0xC6, 0xFF, 0x04, 0xE, 0x04, 0xFF, 0xFF, 0x01, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF }, "xx?x?x??xxxx????").ToInt32() + 0xC;
        }

        private void MoneyScan_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            button1.Text = "Set";
            label1.Text = "Money: " + Memory.ReadFloat(offset);
            moneyfound = 1;
        }

        private void FindProcess_Tick(object sender, EventArgs e)
        {
            if (Process.GetProcessesByName("mysummercar").Length != 0)                
            {
                button1.Enabled = true;
                button2.Enabled = true;
                signature_Health = 1;
                Text = title + " : READY!";
            }
            else
            {
                button1.Enabled = false; 
                button2.Enabled = false;
                signature_Health = 0;
                label1.Text = "Money: N/A";
                Array.Clear(needs, 0, needs.Length);
                moneyfound = 0;
                Text = title + " : GAME NOT FOUND";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Needs nd = new Needs(needs);
            nd.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            title = Text;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && button1.Enabled)
            {
                button1_Click(null,null);
            }
        }
    }
}
