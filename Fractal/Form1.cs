using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fractal
{
    public partial class Fractal : Form
    {

        private const int MAX = 256;      // max iterations
        private const double SX = -2.025; // start value real
        private const double SY = -1.125; // start value imaginary
        private const double EX = 0.6;    // end value real
        private const double EY = 1.125;  // end value imaginary
        private static int x1, y1, xs, ys, xe, ye;
        private static double xstart, ystart, xende, yende, xzoom, yzoom;
        private static bool action, rectangle, finished;
        private static float xy;
        private bool mouseDown = false;
        private Bitmap picture;
        private Graphics g1;
        private Cursor c1, c2;
        private HSB HSBcol = new HSB();

        private void Fractal_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();

            savefile.Filter = "JPEG | *.jpg";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                picture.Save(savefile.FileName);
            }
        }

        private void infoToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
        

        private void cloneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Fractal fb = new Fractal();
            fb.Show();
        }

        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            start();
            mandelbrot();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Image = null;
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mandelbrot();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //for starting mandelbort again
            start();
            mandelbrot();
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(pictureBox1.Image, 0, 0);
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrintDocument printDocument1 = new PrintDocument();
            printDocument1.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
            printDocument1.Print();
        }

        public Fractal()
        {
            InitializeComponent();

            HSBcol = new HSB();
            this.pictureBox1.Size = new System.Drawing.Size(640, 480); // equivalent of setSize in java code
            finished = false;
            Cursor c1 = Cursors.WaitCursor;
            Cursor c2 = Cursors.Cross;
            x1 = pictureBox1.Width;
            y1 = pictureBox1.Height;
            xy = (float)x1 / (float)y1;

            picture = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g1 = Graphics.FromImage(picture);
            finished = true;
            start();
        }


        public void destroy() // delete all instances 
        {
            if (finished)
            {
                this.pictureBox1.MouseDown -= this.pictureBox1_MouseDown;
                this.pictureBox1.MouseMove -= this.pictureBox1_MouseMove;
                this.pictureBox1.MouseUp -= this.pictureBox1_MouseUp;

                picture = null;
                g1 = null;
                c1 = null;
                c2 = null;
                System.GC.Collect(); // garbage collection
            }
        }

        public void start()
        {
            action = false;
            rectangle = false;
            initvalues();
            xzoom = (xende - xstart) / (double)x1;
            yzoom = (yende - ystart) / (double)y1;
            mandelbrot();
        }


        public void stop()
        {
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

            //Console.WriteLine("sdfdsf");
            update();
        }

        public void update()
        {
            
            Image tempPic = Image.FromHbitmap(picture.GetHbitmap());
            Graphics g = Graphics.FromImage(tempPic);

            if (rectangle)
            {
                Pen pen = new Pen(Color.White);

                Rectangle rect;

                if (xs < xe)
                {
                    if (ys < ye)
                    {
                        rect = new Rectangle(xs, ys, (xe - xs), (ye - ys));
                    }
                    else
                    {
                        rect = new Rectangle(xs, ye, (xe - xs), (ys - ye));
                    }
                }
                else
                {
                    if (ys < ye)
                    {
                        rect = new Rectangle(xe, ys, (xs - xe), (ye - ys));
                    }
                    else
                    {
                        rect = new Rectangle(xe, ye, (xs - xe), (ys - ye));
                    }
                }

                g.DrawRectangle(pen, rect);
                pictureBox1.Image = tempPic;

            }
        }

        private void mandelbrot() // calculate all points
        {
            int x, y;
            float h, b, alt = 0.0f;
            Pen pen = new Pen(Color.White);

            action = false;
            this.Cursor = c1; // in java setCursor(c1)

           //showStatus("Mandelbrot-Set will be produced - please wait...");
            for (x = 0; x < x1; x += 2)
            {
                for (y = 0; y < y1; y++)
                {
                    h = pointcolour(xstart + xzoom * (double)x, ystart + yzoom * (double)y); // hue value

                    if (h != alt)
                    {
                        b = 1.0f - h * h; // brightness

                        HSBcol.fromHSB(h, 0.8f, b); //convert hsb to rgb then make a Java Color
                        Color col = Color.FromArgb(Convert.ToByte(HSBcol.rChan), Convert.ToByte(HSBcol.gChan), Convert.ToByte(HSBcol.bChan));

                        pen = new Pen(col);

                        //djm end
                        //djm added to convert to RGB from HSB

                        alt = h;
                    }
                    g1.DrawLine(pen, new Point(x, y), new Point(x + 1, y)); // drawing pixel
                }
                //showStatus("Mandelbrot-Set ready - please select zoom area with pressed mouse.");
                this.Cursor = c2;
                action = true;
            }

            pictureBox1.Image = picture;
        }

        private float pointcolour(double xwert, double ywert) // color value from 0.0 to 1.0 by iterations
        {
            double r = 0.0, i = 0.0, m = 0.0;// real, imaginary, absolute value or distance
            int j = 0;

            while ((j < MAX) && (m < 4.0))
            {
                j++;
                m = r * r - i * i; // x^2 - y^2
                i = 2.0 * r * i + ywert; // 2xy + c
                r = m + xwert;
            }
            return (float)j / (float)MAX;
        }

        private void initvalues() // reset start values
        {
            xstart = SX;
            ystart = SY;
            xende = EX;
            yende = EY;
            if ((float)((xende - xstart) / (yende - ystart)) != xy)
                xstart = xende - (yende - ystart) * (double)xy;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int z, w;

           
            if (action)
            {
                xe = e.X;
                ye = e.Y;
                if (xs > xe)
                {
                    z = xs;
                    xs = xe;
                    xe = z;
                }
                if (ys > ye)
                {
                    z = ys;
                    ys = ye;
                    ye = z;
                }
                w = (xe - xs);
                z = (ye - ys);
                if ((w < 2) && (z < 2)) initvalues();
                else
                {
                    if (((float)w > (float)z * xy)) ye = (int)((float)ys + (float)w / xy);
                    else xe = (int)((float)xs + (float)z * xy);
                    xende = xstart + xzoom * (double)xe;
                    yende = ystart + yzoom * (double)ye;
                    xstart += xzoom * (double)xs;
                    ystart += yzoom * (double)ys;
                }
                xzoom = (xende - xstart) / (double)x1;
                yzoom = (yende - ystart) / (double)y1;
                mandelbrot();
                rectangle = false;
                pictureBox1.Refresh();
                mouseDown = false;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (action)
            {
                mouseDown = true;
                xs = e.X;
                ys = e.Y;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            

            if (action && mouseDown)
            {
                xe = e.X;
                ye = e.Y;
                rectangle = true;
                pictureBox1.Refresh();
            }
        }

        class HSB
        {
            public float rChan, gChan, bChan;
            public HSB()
            {
                rChan = gChan = bChan = 0;
            }
            public void fromHSB(float h, float s, float b)
            {
                if (s == 0)
                {
                    rChan = gChan = bChan = (int)(b * 255.0f + 0.5f);
                }
                else
                {
                    h = (h - (float)Math.Floor(h)) * 6.0f;
                    float f = h - (float)Math.Floor(h);
                    float p = b * (1.0f - s);
                    float q = b * (1.0f - s * f);
                    float t = b * (1.0f - (s * (1.0f - f)));
                    switch ((int)h)
                    {
                        case 0:
                            rChan = (int)(b * 255.0f + 0.5f);
                            gChan = (int)(t * 255.0f + 0.5f);
                            bChan = (int)(p * 255.0f + 0.5f);
                            break;
                        case 1:
                            rChan = (int)(q * 255.0f + 0.5f);
                            gChan = (int)(b * 255.0f + 0.5f);
                            bChan = (int)(p * 255.0f + 0.5f);
                            break;
                        case 2:
                            rChan = (int)(p * 255.0f + 0.5f);
                            gChan = (int)(b * 255.0f + 0.5f);
                            bChan = (int)(t * 255.0f + 0.5f);
                            break;
                        case 3:
                            rChan = (int)(p * 255.0f + 0.5f);
                            gChan = (int)(q * 255.0f + 0.5f);
                            bChan = (int)(b * 255.0f + 0.5f);
                            break;
                        case 4:
                            rChan = (int)(t * 255.0f + 0.5f);
                            gChan = (int)(p * 255.0f + 0.5f);
                            bChan = (int)(b * 255.0f + 0.5f);
                            break;
                        case 5:
                            rChan = (int)(b * 255.0f + 0.5f);
                            gChan = (int)(p * 255.0f + 0.5f);
                            bChan = (int)(q * 255.0f + 0.5f);
                            break;
                    }
                }
            }
        }



    }
}
