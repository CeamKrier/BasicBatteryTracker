﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleCounter
{
    public partial class Edit : Form
    {
        private Application parent;
        private static Regex rgxParameter = new Regex(@"^[[1-9]{1}[0-9]{0,1}]*$");
        public Edit(Application parent)
        {
            InitializeComponent();
            this.ControlBox = false;
            this.parent = parent;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (rgxParameter.IsMatch(txtInterval.Text))
            {
                this.parent.batteryLogInterval = Int32.Parse(txtInterval.Text);
                this.parent.updateIntervalLabel();
                this.Close();
            }
            else
            {
                MessageBox.Show("Please be sure to enter a number between 1-99", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtInterval.Text = "";
            }
        }
    }
}
