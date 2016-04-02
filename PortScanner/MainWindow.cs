﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PortScanner
{
    public partial class MainWindow : Form
    {
        // Delegate to report back with one open port
        public delegate void ExecuteOnceCallback(int openPort);

        public delegate void ExecuteOnceAsyncCallback(int port, bool isOpen);

        // The manager instance
        ScannerManagerSingleton smc;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            // Get the ScannerManagerSingleton instance
            smc = ScannerManagerSingleton.Instance;

            // Add new line to log text box
            statusTextBox.Text += Environment.NewLine;

            // Populate the timeout times list box
            PopulateTimeoutListBox();
        }

        private void PopulateTimeoutListBox()
        {
            // Assign the list to the ComboBox's DataSource property
            timeoutComboBox.DataSource = TimeoutListItem.CreateTimeoutListItems();
            timeoutComboBox.DisplayMember = "DisplayMember";
            timeoutComboBox.ValueMember = "ValueMember";

            // Set default value
            timeoutComboBox.SelectedValue = 2000;
        }

        private void statusTextBox_TextChanged(object sender, EventArgs e)
        {
            // When new text is written, scroll the bar down
        }

        private void PortResult(int port, bool isOpen)
        {
            if (isOpen)
            {
                statusTextBox.AppendText(String.Format("{0}, port {1} is open.{2}", hostnameTextBox.Text, port, Environment.NewLine));
            }
            else
            {
                statusTextBox.AppendText(String.Format("{0}, port {1} is closed.{2}", hostnameTextBox.Text, port, Environment.NewLine));
            }

        }

        private void checkPortButton_Click(object sender, EventArgs e)
        {
            ToggleInputs(false);

            // Simple one port check
            if (!portRangeCheckBox.Enabled)
            {
                // Get inputs
                string hostname = hostnameTextBox.Text;
                int port = System.Int32.Parse(portTextBoxMin.Text);

                // Get desired mode TCP/UDP radio button
                ScannerManagerSingleton.ScanMode scanMode = ReadScanMode();

                // Get desired timeout
                int timeout = (int)timeoutComboBox.SelectedValue;

                // Set status box text
                statusTextBox.AppendText(String.Format("Connecting to {0}, port {1}...{2}", hostname, port, Environment.NewLine));

                // The callback for scan result
                var callback = new ExecuteOnceAsyncCallback(PortResult);

                // Send one check request
                smc.ExecuteOnceAsync(hostname, port, timeout, scanMode, callback);
            }
            // Port range check
            else
            {
                // var callback = new ExecuteOnceCallback(WriteOpenPort);

                string hostname = hostnameTextBox.Text;
                int portMin = Int32.Parse(portTextBoxMin.Text);
                int portMax = Int32.Parse(portTextBoxMax.Text);

                statusTextBox.Text = "Open: ";

                // TODO: sm.ExecuteRange(hostname, portMin, portMax, writeDelegate);
            }
            // Enable inputs
            ToggleInputs(true);
        }

        // Read scan mode radio button selection
        private ScannerManagerSingleton.ScanMode ReadScanMode()
        {
            if (tcpModeRadioButton.Checked)
                return ScannerManagerSingleton.ScanMode.TCP;
            else
                return ScannerManagerSingleton.ScanMode.UDP;
        }

        private void portRangeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (portRangeCheckBox.Checked)
            {
                portTextBoxMax.Enabled = true;
            }
            else
            {
                portTextBoxMax.Enabled = false;
            }
        }

        // Toggle all inputs
        private void ToggleInputs(bool setting)
        {
            hostnameTextBox.Enabled = setting;
            portTextBoxMin.Enabled = setting;
            checkPortButton.Enabled = setting;
            portTextBoxMax.Enabled = setting;
            portRangeCheckBox.Enabled = setting;

            // Re-disable the portMax text box
            if (!portRangeCheckBox.Checked)
            {
                portTextBoxMax.Enabled = false;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
