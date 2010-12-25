using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SetupWizard
{
    public partial class SetupWizardScreen1 : Form
    {
        public SetupWizardScreen1()
        {
            InitializeComponent();
        }

        private void nextTab(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex++;
        }

        private void previousTab(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex--;
        }
    }
}
