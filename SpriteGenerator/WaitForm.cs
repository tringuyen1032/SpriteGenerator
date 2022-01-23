using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpriteGenerator
{
    public partial class WaitForm : Form
    {
        private static WaitForm form = null;
        public WaitForm()
        {
            InitializeComponent();
            form = this;
        }

        public void SetStep(int length)
        {
            form.progressBarGenerating.Maximum = length;

        }

        public void GeneratingPercent(int step)
        {
            form.progressBarGenerating.Value = step;
        }

        public int GetValue()
        {
            return form.progressBarGenerating.Value;
        }
    }
}
