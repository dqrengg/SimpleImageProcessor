using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessor
{
    public class ImageControlCollection : IDisposable
    {
        #region members

        // for function Dispose(bool)
        private bool disposed = false;

        // winform control
        private Form1 form1;
        public Panel panel { get; }
        private SplitContainer splitContainer;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        public Button navButton;
        public Button closeButton;
        public ToolStripMenuItem toolStripMenuItem;

        // tool strip menu item width
        public int toolStripMenuItemWidth;

        // navigation buttons active color
        private readonly Color navBtnActBackColor = Color.RoyalBlue;
        private readonly Color navBtnActTextColor = Color.White;
        // navigation buttons normal color
        private readonly Color navBtnNorBackColor = SystemColors.Control;
        private readonly Color navBtnNorTextColor = Color.Black;

        // file name
        public string fileName;
        public string safeFileName;
        public string extension;

        // redo and undo control
        public bool redoEnabled = false;
        public bool undoEnabled = false;

        // use stack to implement undo
        private Stack<Bitmap> undoStack = new Stack<Bitmap>();
        // use stack to implement redo
        private Stack<Bitmap> redoStack = new Stack<Bitmap>();

        // current image in picture box 1
        public Bitmap cur = null;
        // modified image in picture box 2
        private Bitmap modified = null;
        // backup image
        public Bitmap backup = null;

        // check if is pasted
        public bool isPasted;

        // check if is modifying
        public bool isModifying = false;
        // remember what is doing
        public int doingFun = -1;

        // Factor for zoom the image
        private static readonly int zoomMin = 1;
        private static readonly int zoomNormal = 3;
        private static readonly int zoomMax = 6;
        public int zoomFac;

        // Value for moving the image in X direction
        private float translateX = 0;
        // Value for moving the image in Y direction
        private float translateY = 0;

        // Flag to set mouse down on the image
        private bool translate = false;

        // Set on the mouse down to know from where moving starts
        private float transStartX;
        private float transStartY;

        // Current Image position after moving 
        private float curImageX = 0;
        private float curImageY = 0;

        #region for statistics

        public int pixelCount = 0;

        public int[] R = new int[256];
        public int[] G = new int[256];
        public int[] B = new int[256];

        public double[] percentR = new double[256];
        public double[] percentG = new double[256];
        public double[] percentB = new double[256];

        public int redMax = 0;
        public double redMean = 0D;
        public int redMedian = 0;
        public int redMin = 0;
        public int redMode = 0;

        public int greenMax = 0;
        public double greenMean = 0D;
        public int greenMedian = 0;
        public int greenMin = 0;
        public int greenMode = 0;

        public int blueMax = 0;
        public double blueMean = 0D;
        public int blueMedian = 0;
        public int blueMin = 0;
        public int blueMode = 0;

        public double hueMax = 0D;
        public double hueMean = 0D;
        public double hueMedian = 0D;
        public double hueMin = 0D;
        public double hueMode = 0D;

        public double saturationMax = 0D;
        public double saturationMean = 0D;
        public double saturationMedian = 0D;
        public double saturationMin = 0D;
        public double saturationMode = 0D;

        public double brightnessMax = 0D;
        public double brightnessMean = 0D;
        public double brightnessMedian = 0D;
        public double brightnessMin = 0D;
        public double brightnessMode = 0D;

        #endregion

        #endregion

        #region constructor and destructor

        // constructor
        public ImageControlCollection(Bitmap bmp, string fn, string sfn, bool pasteFalg)
        {
            navButton = new Button();
            closeButton = new Button();
            panel = new Panel();
            splitContainer = new SplitContainer();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            toolStripMenuItem = new ToolStripMenuItem();
            navButton.SuspendLayout();
            closeButton.SuspendLayout();
            panel.SuspendLayout();
            splitContainer.SuspendLayout();
            pictureBox1.SuspendLayout();
            pictureBox2.SuspendLayout();

            this.navButton.FlatAppearance.BorderSize = 0;
            this.navButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.navButton.Location = new System.Drawing.Point(0, 0);
            this.navButton.Margin = new System.Windows.Forms.Padding(0);
            this.navButton.Name = "navigation_dymanic_button";
            this.navButton.TabStop = false;
            this.navButton.Text = sfn;
            using (Graphics cg = navButton.CreateGraphics())
            {
                SizeF size = cg.MeasureString(navButton.Text, navButton.Font);
                this.navButton.Size = new System.Drawing.Size((int)size.Width + 32, 32);
            }
            this.navButton.UseVisualStyleBackColor = true;
            this.navButton.Click += new EventHandler(this.navBtn_Click);

            this.closeButton.FlatAppearance.BorderSize = 0;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Location = new System.Drawing.Point(0, 0);
            this.closeButton.Margin = new System.Windows.Forms.Padding(0);
            this.closeButton.Name = "dymanic_close_button";
            this.closeButton.Size = new System.Drawing.Size(32, 32);
            this.closeButton.TabIndex = 0;
            this.closeButton.TabStop = false;
            this.closeButton.Text = "X";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.closeBtn_Click);

            this.pictureBox1.AutoSize = true;
            this.pictureBox1.BackColor = Color.White;
            this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(578, 604);
            this.pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            this.pictureBox1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseWheel);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);

            this.pictureBox2.AutoSize = true;
            this.pictureBox2.BackColor = Color.White;
            this.pictureBox2.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(578, 604);
            this.pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox2_Paint);
            this.pictureBox2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.pictureBox2.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            this.pictureBox2.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseWheel);
            this.pictureBox2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);

            this.panel.BackColor = Color.RoyalBlue;
            this.panel.Controls.Add(splitContainer);
            this.panel.Dock = DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(2, 32);
            this.panel.Margin = new System.Windows.Forms.Padding(0);
            this.panel.Padding = new System.Windows.Forms.Padding(2);
            this.panel.Name = "dynamic_panel";
            this.panel.Size = new System.Drawing.Size(1522, 806);

            this.splitContainer.AutoSize = true;
            this.splitContainer.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = FixedPanel.None;
            this.splitContainer.Location = new System.Drawing.Point(2, 2);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer.Name = "dymanic_splitContainer";
            this.splitContainer.Padding = new System.Windows.Forms.Padding(3);
            this.splitContainer.Panel1.Controls.Add(pictureBox1);
            this.splitContainer.Panel2.Controls.Add(pictureBox2);
            this.splitContainer.Panel2Collapsed = true;
            this.splitContainer.Size = new System.Drawing.Size(1168, 610);
            this.splitContainer.SplitterDistance = 614;
            this.splitContainer.SplitterWidth = 1;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.TabStop = false;

            this.toolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.toolStripMenuItem.Text = sfn;
            this.toolStripMenuItem.Click += new System.EventHandler(this.toolStripMenuItem_Click);

            this.navButton.ResumeLayout(false);
            this.closeButton.ResumeLayout(false);
            this.pictureBox1.ResumeLayout(false);
            this.pictureBox2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.panel.ResumeLayout(false);

            fileName = fn;
            safeFileName = sfn;
            isPasted = pasteFalg;
            extension = System.IO.Path.GetExtension(fn);

            this.cur = new Bitmap(bmp);
            this.backup = cur;
            this.undoStack.Push(cur);

            initImageArgs();
            statistics();
            refresh();

            this.splitContainer.Panel2Collapsed = !isModifying;
            //this.panel.Refresh();
        }

        // destructor
        ~ImageControlCollection()
        {
            // Simply call Dispose(false).
            Dispose(false);
        }

        #endregion

        #region implement IDisposable.Dispose()

        // Implement IDisposable.Dispose()
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Implement IDisposable.Dispose(bool)
        protected void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects)
                    this.panel.Controls.Remove(splitContainer);
                    this.splitContainer.Panel1.Controls.Remove(pictureBox1);
                    this.splitContainer.Panel2.Controls.Remove(pictureBox2);
                    this.navButton.Invalidate();
                    this.closeButton.Invalidate();
                    this.pictureBox1.Invalidate();
                    this.pictureBox2.Invalidate();
                    this.splitContainer.Invalidate();
                    this.panel.Invalidate();
                }

                // Free your own state (unmanaged objects)
                // Set large fields to null

                this.undoStack.Clear();
                this.redoStack.Clear();
                this.cur = null;
                this.modified = null;
                this.backup = null;

                disposed = true;
            }
        }

        #endregion

        #region button event handler

        protected void navBtn_Click(object sneder, EventArgs e)
        {
            form1.setActPanel(this);
        }

        protected void closeBtn_Click(object sneder, EventArgs e)
        {
            form1.delPanel(this);
        }

        #endregion

        #region picture box event handler

        // picture box 1 paint event
        protected void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (cur == null)
            {
                return;
            }

            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black);

            // move image to new position
            g.TranslateTransform(translateX, translateY);

            // Scale transform operation on the picture box device context
            float zoom = (float)zoomFac / (float)zoomNormal;
            
            int width = (int)(cur.Width * zoom);
            int height = (int)(cur.Height * zoom);

            int x = (int)((pictureBox1.Width - width) / 2);
            int y = (int)((pictureBox1.Height - height) / 2);

            // Drawback the bitmap to the transformed decive context
            // Apply double buffering (Draw to a bitmap first and then draw to picturebox)
            // if using large image and experience flickering
            g.DrawRectangle(pen , x - 1, y - 1, width + 1, height + 1);
            g.DrawImage(cur, x, y, width, height);

            pen.Dispose();
        }

        // picture box 2 paint event
        protected void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            if (modified == null || this.splitContainer.Panel2Collapsed)
            {
                return;
            }

            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black);

            // Scale transform operation on the picture box device context
            float zoom = (float)zoomFac / (float)zoomNormal;

            int width = (int)(modified.Width * zoom);
            int height = (int)(modified.Height * zoom);

            // move image to new position
            g.TranslateTransform(translateX, translateY);

            int x = (int)((pictureBox1.Width - width) / 2);
            int y = (int)((pictureBox1.Height - height) / 2);

            // Drawback the bitmap to the transformed decive context
            // Apply double buffering (Draw to a bitmap first and then draw to picturebox)
            // if using large image and experience flickering
            g.DrawRectangle(pen , x - 1, y - 1, width + 1, height + 1);
            g.DrawImage(modified, x, y, width, height);

            pen.Dispose();
        }

        // picture box mouse up event
        protected void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {            
            // If mouse down is true
            if (translate == true)
            {
                // calculate the total distance to move from 0,0
                // previous image position+ current moving distance
                float zoom = (float)zoomFac / (float)zoomNormal;
                translateX = curImageX + ((e.X - transStartX) / zoom);
                translateY = curImageY + ((e.Y - transStartY) / zoom);

                // call picturebox to update the image in the new position
                refresh();

                // set mouse down operation end
                translate = false;

                pictureBox1.Cursor = Cursors.Arrow;
                pictureBox2.Cursor = Cursors.Arrow;

                // set present position of the image after move.
                curImageX = translateX;
                curImageY = translateY;
            }
        }

        // picture box mouse down event
        protected void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            float zoom = (float)zoomFac / (float)zoomNormal;
            int width = (int)(cur.Width * zoom);
            int height = (int)(cur.Height * zoom);
            int x = (int)((pictureBox1.Width - width) / 2 + translateX);
            int y = (int)((pictureBox1.Height - height) / 2 + translateY);

            if (e.X >= x && e.X <= x + width && e.Y >= y && e.Y <= y + height)
            {
                pictureBox1.Cursor = Cursors.Hand;
                pictureBox2.Cursor = Cursors.Hand;
            
                // mouse down is true
                translate = true;
                // starting coordinates for move
                transStartX = e.X;
                transStartY = e.Y;
            } 
        }

        // picture box mouse wheel event
        protected void pictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            int numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
            
            if (numberOfTextLinesToMove >= 1)
            {
                zoomIn();
            }
            else if (numberOfTextLinesToMove <= -1)
            {
                zoomOut();
            }
        }

        // picture box mouse hover event
        protected void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            Form1 form1 = this.panel.FindForm() as Form1;

            float zoom = (float)zoomFac / (float)zoomNormal;
            int width = (int)(cur.Width * zoom);
            int height = (int)(cur.Height * zoom);
            int posX = (int)((pictureBox1.Width - width) / 2 + translateX);
            int posY = (int)((pictureBox1.Height - height) / 2 + translateY);
            if (e.X >= posX && e.X <= posX + width && e.Y >= posY && e.Y <= posY + height)
            {
                int pxlConvertX = (int)((e.X - posX) / zoom);
                int pxlConvertY = (int)((e.Y - posY) / zoom);
                if (pxlConvertX < 0) pxlConvertX = 0;
                else if (pxlConvertX >= cur.Width) pxlConvertX = cur.Width - 1;
                if (pxlConvertY < 0) pxlConvertY = 0;
                else if (pxlConvertY >= cur.Height) pxlConvertY = cur.Height - 1;

                Point point = new Point(pxlConvertX, pxlConvertY);
                Color rgb = cur.GetPixel(pxlConvertX, pxlConvertY);

                form1.setPixelStatusStripLabel(point);
                form1.setRgbStatusStripLabel(rgb);
                form1.setHslStatusStripLabel(rgb);
                form1.setYCbCrStatusStripLabel(rgb);    
            }
            else
            {
                form1.resetImageStatusStripLabel();
            }
        }

        #endregion

        #region tool strip menu item handler

        protected void toolStripMenuItem_Click(object sender, EventArgs e)
        {
            form1.setActPanel(this);
        }

        #endregion

        #region other functions

        // Init some arguments
        private void initImageArgs()
        {
            zoomFac = zoomNormal;

            translateX = 0;
            translateY = 0;

            translate = false;

            curImageX = 0;
            curImageY = 0;
        }

        // refresh picturebox
        private void refresh()
        {
            this.pictureBox1.Refresh();
            this.pictureBox2.Refresh();
        }

        // do image processing
        public void modifyImage(int function)
        {
            if (this.splitContainer.Panel2Collapsed)
            {
                this.splitContainer.Panel2Collapsed = false;
            }

            if (doingFun == function)
            {
                return;
            }

            doingFun = function;
            process(doingFun);
            refresh();
        }

        // process image
        private void process(int function)
        {
            switch (function)
            {
                case (int)ImageProcess.Function.IdxRgrToGray:
                    {
                        modified = ImageProcess.RgbToGray(cur);
                        break;
                    }
                case (int)ImageProcess.Function.IdxBlur:
                    {
                        modified = ImageProcess.Blur(cur, 3);
                        break;
                    }
                case (int)ImageProcess.Function.IdxGaussianBlur:
                    {
                        modified = ImageProcess.GaussianBlur(cur, 3, 3);
                        break;
                    }
                case (int)ImageProcess.Function.IdxMedianBlur:
                    {
                        modified = ImageProcess.MedianBlur(cur, 3);
                        break;
                    }
                case (int)ImageProcess.Function.IdxAdaptiveThreshold:
                    {
                        modified = ImageProcess.AdaptiveThreshold(cur, 7);
                        break;
                    }
                case (int)ImageProcess.Function.IdxBilateralFilter:
                    {
                        modified = ImageProcess.BilateralFilter(cur, 7);
                        break;
                    }
                case (int)ImageProcess.Function.IdxErode:
                    {
                        modified = ImageProcess.Erode(cur, 3);
                        break;
                    }
                case (int)ImageProcess.Function.IdxDilate:
                    {
                        modified = ImageProcess.Dilate(cur, 3);
                        break;
                    }
                case (int)ImageProcess.Function.IdxClose:
                    {
                        modified = ImageProcess.Close(cur, 3);
                        break;
                    }
                case (int)ImageProcess.Function.IdxOpen:
                    {
                        modified = ImageProcess.Open(cur, 3);
                        break;
                    }
                case (int)ImageProcess.Function.IdxGradient:
                    {
                        modified = ImageProcess.Gradient(cur, 3);
                        break;
                    }
                case (int)ImageProcess.Function.IdxTophat:
                    {
                        modified = ImageProcess.Tophat(cur, 3);
                        break;
                    }
                case (int)ImageProcess.Function.IdxBlackhat:
                    {
                        modified = ImageProcess.Blackhat(cur, 3);
                        break;
                    }
                default:
                    {
                        return;
                    }
            }
        }

        // modify current image
        public void modifyCheck()
        {
            if (modified != null)
            {
                cur = modified;
                undoStack.Push(cur);
                redoStack.Clear();

                checkUndoRedoEnable();

                modified = null;
                isModifying = false;

                this.splitContainer.Panel2Collapsed = true;

                doingFun = -1;

                statistics();
                refresh();
            }
        }

        // save image
        public void save()
        {
            cur.Save(fileName);
        }

        // zoom in
        public void zoomIn()
        {
            // larger
            if (zoomFac < zoomMax)
                zoomFac++;

            // call the picture box paint
            refresh();

            Form1 form1 = this.panel.FindForm() as Form1;
            form1.setZoomStatusStripLabel(zoomFac);
        }

        // zoom out
        public void zoomOut()
        {
            // smaller
            if (zoomFac > zoomMin)
                zoomFac--;

            // call the picture box paint
            refresh();

            Form1 form1 = this.panel.FindForm() as Form1;
            form1.setZoomStatusStripLabel(zoomFac);
        }

        // zoom original
        public void zoomOriginal()
        {
            zoomFac = zoomNormal;
            refresh();

            Form1 form1 = this.panel.FindForm() as Form1;
            form1.setZoomStatusStripLabel(zoomFac);
        }

        // undo
        public void undo()
        {
            redoStack.Push(undoStack.Pop());
            checkUndoRedoEnable();
            cur = undoStack.Peek();
            if (isModifying)
            {
                process(doingFun);
            }
            statistics();
            refresh();
        }

        // redo
        public void redo()
        {
            undoStack.Push(redoStack.Pop());
            checkUndoRedoEnable();
            cur = undoStack.Peek();
            if (isModifying)
            {
                process(doingFun);
            }
            statistics();
            refresh();
        }

        // check undo redo enable
        private void checkUndoRedoEnable()
        {
            undoEnabled = undoStack.Count > 1 ? true : false;
            redoEnabled = redoStack.Count >= 1 ? true : false;
        }

        // rotate 90 degree
        public void rotate90()
        {
            cur = ImageProcess.Rotate(cur);
            if (isModifying)
            {
                process(doingFun);
            }
            undoStack.Push(cur);
            redoStack.Clear();

            checkUndoRedoEnable();
            statistics();

            refresh();
        }

        // set this.form1
        public void findForm()
        {
            form1 = this.panel.FindForm() as Form1;
        }

        // set normal button color
        public void setNorBtnColor()
        {
            navButton.ForeColor = navBtnNorTextColor;
            navButton.BackColor = navBtnNorBackColor;
            closeButton.ForeColor = navBtnNorTextColor;
            closeButton.BackColor = navBtnNorBackColor;
        }

        // set active button color
        public void setActBtnColor()
        {
            navButton.ForeColor = navBtnActTextColor;
            navButton.BackColor = navBtnActBackColor;
            closeButton.ForeColor = navBtnActTextColor;
            closeButton.BackColor = navBtnActBackColor;
        }

        // statistics
        private void statistics()
        {
            pixelCount = cur.Width * cur.Height;

            Color pixel;
            List<int> rList = new List<int>();
            List<int> gList = new List<int>();
            List<int> bList = new List<int>();

            List<double> hList = new List<double>();
            List<double> sList = new List<double>();
            List<double> vList = new List<double>();

            for (int i = 0; i < cur.Width; i++)
            {
                for (int j = 0; j < cur.Height; j++)
                {
                    pixel = cur.GetPixel(i, j);
                    R[pixel.R]++;
                    G[pixel.G]++;
                    B[pixel.B]++;
                    rList.Add(pixel.R);
                    gList.Add(pixel.G);
                    bList.Add(pixel.B);
                    hList.Add(pixel.GetHue());
                    sList.Add(pixel.GetSaturation());
                    vList.Add(pixel.GetBrightness());
                }
            }

            for (int i = 0; i < 256; i++)
            {
                percentR[i] = (double)R[i] / (double)pixelCount;
                percentG[i] = (double)G[i] / (double)pixelCount;
                percentB[i] = (double)B[i] / (double)pixelCount;
            }

            redMean = rList.Average();
            greenMean = gList.Average();
            blueMean = bList.Average();
            hueMean = hList.Average();
            saturationMean = sList.Average();
            brightnessMean = vList.Average();

            redMedian = rList.OrderBy(n => n).ElementAt(pixelCount / 2);
            greenMedian = gList.OrderBy(n => n).ElementAt(pixelCount / 2);
            blueMedian = bList.OrderBy(n => n).ElementAt(pixelCount / 2);
            hueMedian = hList.OrderBy(n => n).ElementAt(pixelCount / 2);
            saturationMedian = sList.OrderBy(n => n).ElementAt(pixelCount / 2);
            brightnessMedian = vList.OrderBy(n => n).ElementAt(pixelCount / 2);

            redMode = rList.GroupBy(n => n).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();
            greenMode = gList.GroupBy(n => n).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();
            blueMode = bList.GroupBy(n => n).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();
            hueMode = hList.GroupBy(n => n).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();
            saturationMode = sList.GroupBy(n => n).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();
            brightnessMode = vList.GroupBy(n => n).OrderByDescending(g => g.Count()).Select(g => g.Key).FirstOrDefault();

            redMax = rList.OrderBy(n => n).ElementAt(pixelCount - 1);
            greenMax = gList.OrderBy(n => n).ElementAt(pixelCount - 1);
            blueMax = bList.OrderBy(n => n).ElementAt(pixelCount - 1);
            hueMax = hList.OrderBy(n => n).ElementAt(pixelCount - 1);
            saturationMax = sList.OrderBy(n => n).ElementAt(pixelCount - 1);
            brightnessMax = vList.OrderBy(n => n).ElementAt(pixelCount - 1);

            redMin = rList.OrderBy(n => n).ElementAt(0);
            greenMin = gList.OrderBy(n => n).ElementAt(0);
            blueMin = bList.OrderBy(n => n).ElementAt(0);
            hueMin = hList.OrderBy(n => n).ElementAt(0);
            saturationMin = sList.OrderBy(n => n).ElementAt(0);
            brightnessMin = vList.OrderBy(n => n).ElementAt(0);
        }

        // get red channel
        public void redChannel()
        {
            cur = ImageProcess.getRedChannel(cur);
            undoStack.Push(cur);
            redoStack.Clear();
            checkUndoRedoEnable();
            statistics();
            refresh();
        }

        // get green channel
        public void greenChannel()
        {
            cur = ImageProcess.getGreenChannel(cur);
            undoStack.Push(cur);
            redoStack.Clear();
            checkUndoRedoEnable();
            statistics();
            refresh();
        }

        // get blue channel
        public void blueChannel()
        {
            cur = ImageProcess.getBlueChannel(cur);
            undoStack.Push(cur);
            redoStack.Clear();
            checkUndoRedoEnable();
            statistics();
            refresh();
        }

        #endregion
    }
}
