using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace Image_Processing_1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap image1, image2;

        int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        private void addNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog1 = new OpenFileDialog();
            dialog1.Filter = "Image files|*.png;*.jpg;*.bmp|All filec(*.*)|*.*";
            if (dialog1.ShowDialog() == DialogResult.OK)
            {
                image1 = new Bitmap(dialog1.FileName);
                pictureBox1.Image = image1;
                pictureBox1.Refresh();
            }
            else { return; }

            var rnd = new Random();

            double R = 0;
            double G = 0;
            double B = 0;
            int sigma = 20, m = 128;
            double gaussianNoiseR = rnd.Next(sigma, m);
            double gaussianNoiseG = rnd.Next(sigma, m);
            double gaussianNoiseB = rnd.Next(sigma, m);

            image2 = new Bitmap(image1.Width, image1.Height);

            for (int i = 0; i < image1.Width; i++)
            {
                for (int j = 0; j < image1.Height; j++)
                {
                    R = image1.GetPixel(i, j).R + gaussianNoiseR * 0.5;
                    G = image1.GetPixel(i, j).G + gaussianNoiseG * 0.5;
                    B = image1.GetPixel(i, j).B + gaussianNoiseB * 0.5;
                    Color color = Color.FromArgb(
                        Clamp((int)R, 0, 255), 
                        Clamp((int)G, 0, 255), 
                        Clamp((int)B, 0, 255));
                    image2.SetPixel(i, j, color);
                }
            }
            pictureBox2.Image = image2;
            pictureBox2.Refresh();
        }

        private void removeNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            label1.Visible = false;
            label2.Visible = false;
            pictureBox1.Visible = false;
            pictureBox2.Visible = false;

            OpenFileDialog dialog1 = new OpenFileDialog();
            dialog1.Filter = "Image files|*.png;*.jpg;*.bmp|All filec(*.*)|*.*";
            if (dialog1.ShowDialog() == DialogResult.OK)
            {
                label1.Visible = true;
                label1.Text = "Processing...";
                image1 = new Bitmap(dialog1.FileName);
                pictureBox1.Image = image1;
                pictureBox1.Width = 380;
                pictureBox1.Visible = true;
                pictureBox1.Refresh();

            }
            else { return; }

            if (dialog1.ShowDialog() == DialogResult.OK)
            {
                label2.Visible = true;
                label2.Text = "Processing...";
                image2 = new Bitmap(dialog1.FileName);
                pictureBox2.Image = image2;
                pictureBox2.Width = 380;
                pictureBox2.Visible = true;
                pictureBox2.Refresh();

            }
            else { return; }

            //MyMedianFilter
            int size = 3;
            Color calculateNewPixelColor(Bitmap sourceImage, int x, int y)
            {
                int[] medianR = new int[size * size];
                int[] medianG = new int[size * size];
                int[] medianB = new int[size * size];

                int radius = size / 2;
                int i = 0;
                for (int l = -radius; l <= radius; l++)
                    for (int k = -radius; k <= radius; k++)
                    {
                        int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                        int idY = Clamp(y + l, 0, sourceImage.Height - 1);

                        medianR[i] = sourceImage.GetPixel(idX, idY).R;
                        medianG[i] = sourceImage.GetPixel(idX, idY).G;
                        medianB[i++] = sourceImage.GetPixel(idX, idY).B;
                    }
                Array.Sort(medianR);
                Array.Sort(medianG);
                Array.Sort(medianB);

                return Color.FromArgb(
                    Clamp(medianR[size * size / 2], 0, 255),
                    Clamp(medianG[size * size / 2], 0, 255),
                    Clamp(medianB[size * size / 2], 0, 255));
            }


            //Time start
            
            var watch1 = System.Diagnostics.Stopwatch.StartNew();
            //OpenCV Median filter
            Mat srcImage = Cv2.ImRead(dialog1.FileName);
            Mat filteredImage = new Mat();
            Cv2.MedianBlur(srcImage, filteredImage, 3);
            //Time end
            watch1.Stop();
            label2.Text = "OpenCV MedianFilter elapsed time: " + watch1.ElapsedMilliseconds.ToString() + "ms";

            image2 = filteredImage.ToBitmap();
            pictureBox2.Image = image2;
            pictureBox2.Show();
            pictureBox2.Refresh();

            Bitmap resultImage = new Bitmap(image1.Width, image1.Height);
            


            //Time start
            watch1 = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < image1.Width; i++)
            {
                for (int j = 0; j < image1.Height; j++)
                {
                    resultImage.SetPixel(i, j, calculateNewPixelColor(image1, i, j));
                }
            }
            watch1.Stop();
            //Time end
            label1.Text = "MyMedianFilter elapsed time: " + watch1.ElapsedMilliseconds.ToString() + "ms";


            pictureBox1.Image = resultImage;
            pictureBox1.Refresh();
            
        }

    }
    }
