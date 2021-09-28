using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using ABB.Robotics.Controllers.IOSystemDomain;

namespace Tara_Demo
{
    public partial class Form1 : Form
    {
        private NetworkScanner scanner = null;
        private Controller controller = null;
        private NetworkWatcher networkwatcher = null;

        public Form1()
        {
            InitializeComponent();
            this.scanner = new NetworkScanner();
            scanner.Scan();
            ControllerInfoCollection controllers = scanner.Controllers;
            this.networkwatcher = new NetworkWatcher(scanner.Controllers);
            this.networkwatcher.Found += new EventHandler<NetworkWatcherEventArgs>(HandleFoundEvent);

            this.networkwatcher.EnableRaisingEvents = true;



            foreach (ControllerInfo controllerInfo in controllers)
            {

                comboBox1.Items.Add(controllerInfo.SystemName);
                comboBox1.Tag = controllerInfo;

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {





        }
        void HandleFoundEvent(object sender, NetworkWatcherEventArgs e)
        {
            this.Invoke(new
            EventHandler<NetworkWatcherEventArgs>(AddControllerToListView),
            new Object[] { this, e });
        }
        private void AddControllerToListView(object sender, NetworkWatcherEventArgs e)
        {
            ControllerInfo controllerInfo = e.Controller;
            comboBox1.Items.Add(controllerInfo.ControllerName);
            comboBox1.Tag = controllerInfo;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.controller != null)
            {
                Signal signal1 = controller.IOSystem.GetSignal("diStart");
                DigitalSignal diSig = (DigitalSignal)signal1;
                if (button1.Text == "Start")
                {
                    diSig.Set();
                    button1.Text = "Stop";
                }
                else if (button1.Text == "Stop")
                {
                    diSig.Reset();
                    button1.Text = "Start";
                }
            }

        }

        private void button3_Click_1(object sender, EventArgs e)
        {

            if (comboBox1.SelectedText != null)
            {
                ControllerInfo controllerInfo = (ControllerInfo)comboBox1.Tag;
                if (controllerInfo.Availability == Availability.Available)
                {
                    if (this.controller != null)
                    {
                        this.controller.Logoff();
                        this.controller.Dispose();
                        this.controller = null;
                        button3.Text = "Connect";
                    }
                    else
                    {
                        this.controller = ControllerFactory.CreateFrom(controllerInfo);
                        this.controller.Logon(UserInfo.DefaultUser);

                        CheckSignal();
                        button3.Text = "Disconnect";
                    }

                }
                else
                {
                    MessageBox.Show("Selected controller not available.");
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void CheckSignal()
        {
            Signal signal1 = controller.IOSystem.GetSignal("diStart");
            DigitalSignal diSig = (DigitalSignal)signal1;
            if (diSig.Get() == 1)
            {
                button1.Text = "Stop";
            }
            else
            {
                button1.Text = "Start";
            }

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.controller != null)
            {
                RapidData rd1 = controller.Rapid.GetRapidData("T_ROB1", "Module1", "nCycleTime");
                if (rd1.Value is ABB.Robotics.Controllers.RapidDomain.Num)
                {
                    textBox1.Text = rd1.Value.ToString();
                }

                RapidData rd2 = controller.Rapid.GetRapidData("T_ROB1", "Module1", "nCounter");
                if (rd2.Value is ABB.Robotics.Controllers.RapidDomain.Num)
                {
                    textBox2.Text = rd2.Value.ToString();
                }
            }
        }

    }
}
