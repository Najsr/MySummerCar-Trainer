using System;
using System.Windows.Forms;

namespace MySummerCarMoney
{
    public partial class Needs : Form
    {
        int[] addresses;

        public Needs(int[] ad)
        {
            InitializeComponent();
            addresses = ad;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            trackBar1.Value = (int)Memory.ReadFloat(addresses[comboBox1.SelectedIndex]);
            label1.Text = trackBar1.Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Memory.WriteFloat(addresses[comboBox1.SelectedIndex], trackBar1.Value);
            label1.Text = trackBar1.Value.ToString();
        }

        private void Needs_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;

            trackBar1.Value = (int)Memory.ReadFloat(addresses[comboBox1.SelectedIndex]);
            label1.Text = trackBar1.Value.ToString();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(trackBar1, trackBar1.Value.ToString());
        }
    }
}
