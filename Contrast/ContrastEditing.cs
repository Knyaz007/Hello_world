using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Contrast
{
    public partial class ContrastEditing : Form
    {
        Image buf = null;
        public Image Result = null;

        public ContrastEditing()
        {
            InitializeComponent();
        }

        private void ContrastEditing_VisibleChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = Form1.Imagebuffer;
            buf = Form1.Imagebuffer;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }



        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label4.Text = trackBar1.Value.ToString();
            Bitmap NewImage = (Bitmap)buf;
            Filter.ChangeContrast.ApplyContrast(trackBar1.Value, NewImage);
            pictureBox2.Image = NewImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Result = pictureBox1.Image;
            Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
