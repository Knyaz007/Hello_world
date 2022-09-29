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
    
    public partial class Form1 : Form
    {
        public static Image Imagebuffer;
        public Form1()
        {
            InitializeComponent();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form AboutDialog = new About();
            AboutDialog.Show();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OpenImageDialog = new OpenFileDialog();
            OpenImageDialog.Filter = "Изображения (*.jpg)|*.jpg";
            if (OpenImageDialog.ShowDialog() == DialogResult.OK)
            {
                Bitmap bit = new Bitmap(OpenImageDialog.FileName);
                WorkPicture.Image = bit;
                WorkPicture.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }
        // Добавил код сюда 
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveImageDialog = new SaveFileDialog();
            SaveImageDialog.Filter = "Изображения (*.jpg)|*.jpg)";
            if (SaveImageDialog.ShowDialog() == DialogResult.OK)
            {
                WorkPicture.Image.Save(SaveImageDialog.FileName);
            }
           
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void ChangeImage(Image img)
        {
            WorkPicture.Image = img;
        }

        private void контрастностьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Imagebuffer = WorkPicture.Image;
            ContrastEditing ContrastForm = new ContrastEditing();
            ContrastForm.ShowDialog();
            WorkPicture.Image = ContrastForm.Result;
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChangeImage(Imagebuffer);
        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void WorkPicture_Click(object sender, EventArgs e)
        {

        }
    }
}
