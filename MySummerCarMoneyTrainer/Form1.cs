using System;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Diagnostics;

namespace MySummerCarMoneyTrainer
{
    public partial class Form1 : Form
    {
        private int base_address;

        private int money_address;

        private static bool state = true;

        SpeechSynthesizer synth = new SpeechSynthesizer();

        public Form1()
        {
            InitializeComponent();
            synth.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(synth_SpeakCompleted);
        }

        static void synth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
        {
            state = true;
        }

        private void button_Set_Click(object sender, EventArgs e)
        {
            bool game_is_running = (Memory.GetProcessHandle() != IntPtr.Zero);
            if (!game_is_running && state) {
                state = false;
                synth.SpeakAsync("Game not found");
            } else if (button_Set.Text == "LOAD" && game_is_running) {
                base_address = Memory.GetBaseAddress("mysummercar", "mono.dll") + 0x001F20AC;
                money_address = Memory.GetPointerAddress(base_address, new int[] { 0x14, 0x10, 0x8, 0xAC, 0x11C });
                button_Set.Text = "SET";
                label1.Visible = true;
                label1.Text = "Money: " + Memory.ReadFloat(money_address).ToString();
                textBox1.ReadOnly = false;
            } else if (button_Set.Text == "SET" && game_is_running) { 
                float money;

                if (float.TryParse(textBox1.Text.Replace('.', ','), out money)) {
                    Memory.WriteFloat(money_address, money);
                    label1.Text = "Money: " + Memory.ReadFloat(money_address).ToString();
                } else {
                    MessageBox.Show("Money couldn't be written!", "Error", MessageBoxButtons.OK);
                }
            } else if (!game_is_running && button_Set.Text == "SET") {
                DialogResult = MessageBox.Show("Game is not running! Do you want to close trainer?", "Error", MessageBoxButtons.YesNo);
                if (DialogResult == DialogResult.Yes) Process.GetCurrentProcess().Close();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return) button_Set_Click(null, null);
            else if (e.KeyCode == Keys.F2) button_Set.Text = "LOAD";
        }
    }
}
