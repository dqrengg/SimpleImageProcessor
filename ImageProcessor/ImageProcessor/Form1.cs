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
    public partial class Form1 : Form
    {
        #region members

        // icc list
        private List<ImageControlCollection> iccList = new List<ImageControlCollection>();
        // active icc
        private ImageControlCollection actICC = null;

        // for paste function file name
        private int fileNumber = 1;

        // for status s label
        private string[] zoomLabel = { "0%", "33.3%", "66.6%", "100%", "133.3%", "166.6%", "200%" };

        private static Color lightModeBackColor = SystemColors.Control;
        private static Color lightModeForeColor = SystemColors.ControlText;
        private static Color lightModeSeparatorBackColor = SystemColors.ControlLightLight;
        private static Color lightModeSeparatorForeColor = SystemColors.ControlDark;
        private static Color darkModeBackColor = SystemColors.ControlText;
        private static Color darkModeForeColor = SystemColors.Control;

        private Mode mode = Mode.Light;

        private enum Mode
        {
            Light = 0,
            Dark = 1,
        }

        public Color backColor = lightModeBackColor;
        public Color foreColor = lightModeForeColor;
        public Color separatorBackColor = lightModeSeparatorBackColor;
        public Color separatorForeColor = lightModeSeparatorForeColor;

        #endregion

        // form constructor
        public Form1()
        {
            InitializeComponent();
        }

        #region form handler

        // key down handler
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.C))
            {
                copyImage();
            }
            else if (e.KeyData == (Keys.Control | Keys.V))
            {
                pasteImage();
            }
            else if (e.KeyData == (Keys.Control | Keys.O))
            {
                openImage();
            }
            else if (e.KeyData == (Keys.Control | Keys.S))
            {
                saveImage();
            }
            else if (e.KeyData == (Keys.Control | Keys.X))
            {
                cutImage();
            }
            else if (e.KeyData == (Keys.Control | Keys.Y))
            {
                redoImage();
            }
            else if (e.KeyData == (Keys.Control | Keys.Z))
            {
                undoImage();
            }
        }

        #endregion

        #region menu strip click handler

        // file->open
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openImage();
        }

        // file->save new
        private void saveNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveNewImage();
        }

        // file->save
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveImage();
        }

        // file->close
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeImage();
        }

        // file->close all
        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeAllImage();
        }

        // file->exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            exit();
        }

        // edit->undo
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            undoImage();
        }

        // edit->redo
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            redoImage();
        }

        // edit->copy
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyImage();
        }

        // edit->cut
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cutImage();
        }

        // edit->paste
        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteImage();
        }

        // edit->rotate
        private void rotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rotateImage();
        }

        // view->theme->light
        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lightMode();
        }

        // view->theme->dark
        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            darkMode();
        }

        // view->statistics
        private void statisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.statisticsToolStripMenuItem.CheckState == CheckState.Checked)
            {
                this.statisticsToolStripMenuItem.Checked = false;
                this.statisticsToolStripMenuItem.CheckState = CheckState.Unchecked;
                this.splitContainer1.Panel2Collapsed = true;
            }
            else
            {
                this.statisticsToolStripMenuItem.Checked = true;
                this.statisticsToolStripMenuItem.CheckState = CheckState.Checked;
                this.histogramToolStripMenuItem.Checked = false;
                this.histogramToolStripMenuItem.CheckState = CheckState.Unchecked;
                this.label2.Text = "Statistics";
                setDataGrid();
                this.dataGridView1.BringToFront();
                this.splitContainer1.Panel2Collapsed = false;
                this.splitContainer1.SplitterDistance = 1206;
            }
        }

        // view->histogram
        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.histogramToolStripMenuItem.CheckState == CheckState.Checked)
            {
                this.histogramToolStripMenuItem.Checked = false;
                this.histogramToolStripMenuItem.CheckState = CheckState.Unchecked;
                this.splitContainer1.Panel2Collapsed = true;
            }
            else
            {
                this.histogramToolStripMenuItem.Checked = true;
                this.histogramToolStripMenuItem.CheckState = CheckState.Checked;
                this.statisticsToolStripMenuItem.Checked = false;
                this.statisticsToolStripMenuItem.CheckState = CheckState.Unchecked;
                this.splitContainer1.Panel2Collapsed = false;
                this.splitContainer1.SplitterDistance = 1206;
                this.label2.Text = "Histogram";
                this.comboBox2.SelectedIndex = -1;
                this.panel12.BringToFront();
            }
        }

        // option->red channel
        private void redChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.redChannel();
                setDataGrid();
                setHistogramStatisticsLabels();
            }
        }

        // option->grean channel
        private void greenChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.greenChannel();
                setDataGrid();
                setHistogramStatisticsLabels();
            }
        }

        // option->blue channel
        private void blueChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.blueChannel();
                setDataGrid();
                setHistogramStatisticsLabels();
            }
        }

        // option->gray scale
        private void grayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxRgrToGray;
                this.panel10.Visible = true;
            }
        }

        // option->blur
        private void blurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxBlur;
                this.panel10.Visible = true;
            }
        }

        // option->gaussian blur
        private void gaussianBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxGaussianBlur;
                this.panel10.Visible = true;
            }
        }

        // option->median blur
        private void medianBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxMedianBlur;
                this.panel10.Visible = true;
            }
        }

        // option->bilateral filter
        private void bilateralFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxBilateralFilter;
                this.panel10.Visible = true;
            }
        }

        // option->adaptive threshold
        private void adaptiveThresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxAdaptiveThreshold;
                this.panel10.Visible = true;
            }
        }

        // option->erode
        private void erodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxErode;
                this.panel10.Visible = true;
            }
        }

        // option->dilate
        private void dilateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxDilate;
                this.panel10.Visible = true;
            }
        }

        // option->close
        private void closeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxClose;
                this.panel10.Visible = true;
            }
        }

        // option->open
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxOpen;
                this.panel10.Visible = true;
            }
        }

        // option->gradient
        private void gradientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxGradient;
                this.panel10.Visible = true;
            }
        }

        // option->tophat
        private void tophatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxTophat;
                this.panel10.Visible = true;
            }
        }

        // option->blackhat
        private void blackHatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (actICC != null)
            {
                actICC.isModifying = true;
                this.comboBox1.SelectedIndex = (int)ImageProcess.Function.IdxBlackhat;
                this.panel10.Visible = true;
            }
        }

        #endregion

        #region sub menu buttons handler

        // sub menu -> undo
        private void button9_Click(object sender, EventArgs e)
        {
            undoImage();
        }

        // sub menu -> redo
        private void button8_Click(object sender, EventArgs e)
        {
            redoImage();
        }

        // sub menu -> open image
        private void button7_Click(object sender, EventArgs e)
        {
            openImage();
        }

        // sub menu -> save
        private void button3_Click(object sender, EventArgs e)
        {
            saveImage();
        }

        // sub menu -> copy
        private void button4_Click(object sender, EventArgs e)
        {
            copyImage();
        }

        // sub menu -> cut
        private void button5_Click(object sender, EventArgs e)
        {
            cutImage();
        }

        // sub menu -> Zoom In
        private void button1_Click(object sender, EventArgs e)
        {
            zoomInImage();
        }

        // sub menu -> Zoom Out
        private void button2_Click(object sender, EventArgs e)
        {
            zoomOutImage();
        }

        // sub menu -> zoom to original
        private void button6_Click(object sender, EventArgs e)
        {
            zoomImageOriginalSize();
        }

        // sub menu -> rotate
        private void button11_Click(object sender, EventArgs e)
        {
            rotateImage();
        }

        // sub menu -> edit
        private void button10_Click(object sender, EventArgs e)
        {
            var icc = getActiveICC();
            if (icc == null)
            {
                this.panel10.Visible = false;
                this.comboBox1.SelectedIndex = -1;
                return;
            }
            else if (icc.isModifying)
            {
                icc.isModifying = false;
                this.comboBox1.SelectedIndex = -1;
                this.panel10.Visible = false;
            }
            else
            {
                icc.isModifying = true;
                this.comboBox1.SelectedIndex = -1;
                this.panel10.Visible = true;
            }
        }

        // sub menu -> function select
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            modifyImage(this.comboBox1.SelectedIndex);
        }

        // sub menu -> ok
        private void button13_Click(object sender, EventArgs e)
        {
            var icc = getActiveICC();
            if (icc != null)
            {
                this.panel10.Visible = false;
                icc.modifyCheck();
                setUndoRedoEnable();
                setDataGrid();
                setHistogramStatisticsLabels();
            }
        }

        #endregion

        #region timer handler

        private void timer1_Tick(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString();
            this.toolStripStatusLabel1.Text = time; 
        }

        #endregion

        #region dockpanel panel handler

        private void panel2_Resize(object sender, EventArgs e)
        {
            int newWidth = this.panel2.Width;
            int newHeight = this.panel2.Height;

            foreach (Control c in this.panel2.Controls)
            {
                if (c == this.panel9) continue;

                (c as Panel).Width = newWidth;
                (c as Panel).Height = newHeight;
            }
        }

        #endregion

        #region dockpanel control

        private ImageControlCollection getActiveICC()
        {
            return actICC;
        }

        private void newPanel(Bitmap bmp, string fileName, string safeFileName, bool isPasted)
        {
            var icc = new ImageControlCollection(bmp, fileName, safeFileName, isPasted);
            iccList.Add(icc);
            this.splitContainer1.Panel1.Controls.Add(icc.panel);
            this.panel9.Controls.Add(icc.navButton);
            this.panel9.Controls.Add(icc.closeButton);
            icc.findForm();
            setActPanel(icc);
            refreshNavButtons();
            refreshMenuStrip();
            setDataGrid();
            setUndoRedoEnable();
        }

        public void delPanel(ImageControlCollection icc)
        {
            iccList.Remove(icc);
            this.splitContainer1.Panel1.Controls.Remove(icc.panel);
            this.panel9.Controls.Remove(icc.navButton);
            this.panel9.Controls.Remove(icc.closeButton);
            icc.Dispose();
            var _icc = (iccList.Count == 0) ? null : iccList.Last();
            setActPanel(_icc);
            refreshNavButtons();
            refreshMenuStrip();
            setDataGrid();
            setUndoRedoEnable();
        }

        public void setActPanel(ImageControlCollection icc)
        {
            if (actICC != null)
            {
                actICC.setNorBtnColor();
            }
            actICC = icc;
            if (actICC != null)
            {
                actICC.panel.BringToFront();
                actICC.setActBtnColor();
                this.panel10.Visible = icc.isModifying;
                this.comboBox1.SelectedIndex = icc.doingFun;
                setSizeStatusStripLabel(icc.cur.Size);
            }
            else
            {
                this.panel10.Visible = false;
                this.comboBox1.SelectedIndex = -1;
                resetStatusStripLabel();
            }
        }

        private void refreshNavButtons()
        {
            if (iccList.Count <= 0)
            {
                return;
            }

            int start = 0;
            foreach (var icc in iccList)
            {
                icc.navButton.Location = new Point(start, icc.navButton.Location.Y);
                start += icc.navButton.Width;
                icc.closeButton.Location = new Point(start, icc.closeButton.Location.Y);
                start += icc.closeButton.Width;
            }
        }

        private void refreshMenuStrip()
        {
            this.windowToolStripMenuItem.DropDownItems.Clear();
            foreach (var icc in iccList)
            {
                this.windowToolStripMenuItem.DropDownItems.Add(icc.toolStripMenuItem);
            }
        }

        #endregion

        #region status strip

        public void resetStatusStripLabel()
        {
            string sizeReset = "Size:";
            string zoomReset = "";
            string pxlReset = "Pixel: (-, -)";
            string rgbReset = "(R, G, B): (-, - ,-)";
            string hslReset = "(H, S, L): (-, - ,-)";
            string yccReset = "(Y, U, V): (-, - ,-)";

            this.toolStripStatusLabel4.Text = sizeReset;
            this.toolStripStatusLabel2.Text = zoomReset;
            this.toolStripStatusLabel5.Text = pxlReset;
            this.toolStripStatusLabel3.Text = rgbReset;
            this.toolStripStatusLabel6.Text = hslReset;
            this.toolStripStatusLabel7.Text = yccReset;
        }

        public void resetImageStatusStripLabel()
        {
            string pxlReset = "Pixel: (-, -)";
            string rgbReset = "(R, G, B): (-, - ,-)";
            string hslReset = "(H, S, L): (-, - ,-)";
            string yccReset = "(Y, U, V): (-, - ,-)";

            this.toolStripStatusLabel5.Text = pxlReset;
            this.toolStripStatusLabel3.Text = rgbReset;
            this.toolStripStatusLabel6.Text = hslReset;
            this.toolStripStatusLabel7.Text = yccReset;
        }

        public void setSizeStatusStripLabel(Size size)
        {
            string format = String.Format("Size: {0:D} x {1:D}", size.Width, size.Height);
            this.toolStripStatusLabel4.Text = format;
        }

        public void setZoomStatusStripLabel(int zoom)
        {
            string format = String.Format("{0}", zoomLabel[zoom]);
            this.toolStripStatusLabel2.Text = format;
        }

        public void setPixelStatusStripLabel(Point point)
        {
            string format = String.Format("Pixel: ({0:D}, {1:D})", point.X, point.Y);
            this.toolStripStatusLabel5.Text = format;
        }

        public void setRgbStatusStripLabel(Color color)
        {
            string format = String.Format("(R, G, B): ({0:D}, {1:D}, {2:D})", color.R, color.G, color.B);
            this.toolStripStatusLabel3.Text = format;
        }

        public void setHslStatusStripLabel(Color color)
        {
            string format = String.Format("(H, S, L): ({0:0.0}, {1:0.0}, {2:P1})", color.GetHue(), color.GetSaturation(), color.GetBrightness());
            this.toolStripStatusLabel6.Text = format;
        }

        public void setYCbCrStatusStripLabel(Color color)
        {
            float Y = 0.299F * color.R + 0.587F * color.G + 0.114F * color.B;
            float U = -0.1687F * color.R - 0.3313F * color.G + 0.5F * color.B + 128;
            float V = 0.5F * color.R - 0.4187F *color.G - 0.0813F * color.B + 128;

            string format = String.Format("(Y, Cb, Cr): ({0:0.0}, {1:0.0}, {2:0.0})", Y, U, V);
            this.toolStripStatusLabel7.Text = format;
        }

        #endregion

        #region light mode, dark mode control

        private void lightMode()
        {
            if (mode == Mode.Light)
            {
                return;
            }

            backColor = lightModeBackColor;
            foreColor = lightModeForeColor;
            separatorBackColor = lightModeBackColor;
            separatorBackColor = lightModeForeColor;

            this.lightToolStripMenuItem.Checked = true;
            this.lightToolStripMenuItem.CheckState = CheckState.Checked;
            this.darkToolStripMenuItem.Checked = false;
            this.darkToolStripMenuItem.CheckState = CheckState.Unchecked;

            this.menuStrip1.BackColor = lightModeBackColor;
            this.menuStrip1.ForeColor = lightModeForeColor;

            foreach (var c in this.menuStrip1.Items)
            {
                ToolStripMenuItem tsmi = c as ToolStripMenuItem;
                setDropDownItemColor(tsmi, Mode.Light);
            }
            //this.menuStrip1.Refresh();
            resetMenuStrip();

            //this.Refresh();
        }

        private void darkMode()
        {
            if (mode == Mode.Dark)
            {
                return;
            }

            backColor = darkModeBackColor;
            foreColor = darkModeForeColor;
            separatorBackColor = darkModeBackColor;
            separatorBackColor = darkModeForeColor;

            this.lightToolStripMenuItem.Checked = false;
            this.lightToolStripMenuItem.CheckState = CheckState.Unchecked;
            this.darkToolStripMenuItem.Checked = true;
            this.darkToolStripMenuItem.CheckState = CheckState.Checked;

            this.menuStrip1.BackColor = darkModeBackColor;
            this.menuStrip1.ForeColor = darkModeForeColor;

            foreach (var c in this.menuStrip1.Items)
            {
                ToolStripMenuItem tsmi = c as ToolStripMenuItem;
                setDropDownItemColor(tsmi, Mode.Dark);
            }
            //this.menuStrip1.Refresh();
            resetMenuStrip();

            //this.Refresh();
        }

        private void setDropDownItemColor(object o, Mode m)
        {
            Color bc = lightModeBackColor;
            Color fc = lightModeForeColor;

            if (m == Mode.Light)
            {
                bc = lightModeBackColor;
                fc = lightModeForeColor;
            }
            else if (m == Mode.Dark)
            {
                bc = darkModeBackColor;
                fc = darkModeForeColor;
            }
            else
            {
                return;
            }

            ToolStripMenuItem tsmi = o as ToolStripMenuItem;
            foreach (var item in tsmi.DropDownItems)
            {
                if (item is ToolStripItem)
                {
                    (item as ToolStripItem).BackColor = bc;
                    (item as ToolStripItem).ForeColor = fc;
                }
                else if (item is ToolStripMenuItem)
                {
                    (item as ToolStripMenuItem).BackColor = bc;
                    (item as ToolStripMenuItem).ForeColor = fc;
                    if ((item as ToolStripMenuItem).DropDownItems.Count != 0)
                    {
                        setDropDownItemColor(item, m);
                    }
                }
            }
        }

        private void resetMenuStrip()
        {
            this.fileToolStripMenuItem.DropDownItems.Clear();
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveNewToolStripMenuItem,
            this.toolStripSeparator10,
            this.closeToolStripMenuItem,
            this.closeAllToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.exitToolStripMenuItem.DropDownItems.Clear();
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator9,
            this.copyToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator11,
            this.rotateToolStripMenuItem});
            this.viewToolStripMenuItem.DropDownItems.Clear();
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statisticsToolStripMenuItem,
            this.histogramToolStripMenuItem,
            this.toolStripSeparator12,
            this.themeToolStripMenuItem});
            this.optionToolStripMenuItem.DropDownItems.Clear();
            this.optionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.redChannelToolStripMenuItem,
            this.greenChannelToolStripMenuItem,
            this.blueChannelToolStripMenuItem,
            this.toolStripMenuItem1,
            this.grayScaleToolStripMenuItem,
            this.toolStripMenuItem2,
            this.blurToolStripMenuItem,
            this.gaussianBlurToolStripMenuItem,
            this.medianBlurToolStripMenuItem,
            this.bilateralFilterToolStripMenuItem,
            this.toolStripMenuItem3,
            this.adaptiveThresholdToolStripMenuItem,
            this.toolStripMenuItem4,
            this.erodeToolStripMenuItem,
            this.dilateToolStripMenuItem,
            this.closeToolStripMenuItem1,
            this.openToolStripMenuItem1,
            this.toolStripSeparator14,
            this.gradientToolStripMenuItem,
            this.toolStripSeparator13,
            this.tophatToolStripMenuItem,
            this.blackHatToolStripMenuItem});
        }

        #endregion

        #region tool strip separator paint event

        // tool strip separator 1 paint event
        private void ToolStripSeparator1_Paint(object sender, PaintEventArgs e)
        {
            int width = this.toolStripSeparator1.Width;
            int height = this.toolStripSeparator1.Height;

            // Fill the background.
            e.Graphics.FillRectangle(new SolidBrush(separatorBackColor), 40, 0, width - 40, height);

            // Draw the line.
            e.Graphics.DrawLine(new Pen(separatorForeColor), 40, height / 2, width, height / 2);
        }

        // tool strip separator 9 paint event
        private void ToolStripSeparator9_Paint(object sender, PaintEventArgs e)
        {
            int width = this.toolStripSeparator9.Width;
            int height = this.toolStripSeparator9.Height;

            // Fill the background.
            e.Graphics.FillRectangle(new SolidBrush(separatorBackColor), 40, 0, width - 40, height);

            // Draw the line.
            e.Graphics.DrawLine(new Pen(separatorForeColor), 40, height / 2, width, height / 2);
        }

        // tool strip separator 10 paint event
        private void ToolStripSeparator10_Paint(object sender, PaintEventArgs e)
        {
            int width = this.toolStripSeparator10.Width;
            int height = this.toolStripSeparator10.Height;

            // Fill the background.
            e.Graphics.FillRectangle(new SolidBrush(separatorBackColor), 40, 0, width - 40, height);

            // Draw the line.
            e.Graphics.DrawLine(new Pen(separatorForeColor), 40, height / 2, width, height / 2);
        }

        // tool strip separator paint event
        private void toolStripSeparator_Paint(object sender, PaintEventArgs e)
        {
            ToolStripSeparator tss = sender as ToolStripSeparator;
            int width = tss.Width;
            int height = tss.Height;

            // Fill the background.
            e.Graphics.FillRectangle(new SolidBrush(separatorBackColor), 40, 0, width - 40, height);

            // Draw the line.
            e.Graphics.DrawLine(new Pen(separatorForeColor), 40, height / 2, width, height / 2);
        }

        #endregion

        #region self define functions

        private void openImage()
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Get the path of specified file
                string fileName = openFileDialog1.FileName;
                string safeFileName = openFileDialog1.SafeFileName;

                // create bitmap
                Bitmap bmp = new Bitmap(fileName);

                // add new panel
                newPanel(bmp, fileName, safeFileName, false);
            }
        }

        private void saveNewImage()
        {
            if (actICC == null)
            {
                return;
            }

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (saveFileDialog1.FileName != "")
                {
                    // Saves the Image in the appropriate ImageFormat based upon the
                    // File type selected in the dialog box.
                    // NOTE that the FilterIndex property is one-based.
                    switch (saveFileDialog1.FilterIndex)
                    {
                        case 1:
                            actICC.cur.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            break;

                        case 2:
                            actICC.cur.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            break;

                        case 3:
                            actICC.cur.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            break;
                    }
                }
            }
        }

        private void saveImage()
        {
            var icc = getActiveICC();
            if (icc != null)
            {
                icc.save();
            }
        }

        private void closeImage()
        {
            if (actICC != null)
            {
                delPanel(actICC);
            }
        }

        private void closeAllImage()
        {
            while (actICC != null)
            {
                delPanel(actICC);
            }
        }

        private void exit()
        {
            Application.Exit();
        }

        private void copyImage()
        {
            var icc = getActiveICC();
            if (icc != null)
            {
                Clipboard.SetDataObject(icc.cur, true);
            }
        }

        private void cutImage()
        {
            var icc = getActiveICC();
            if (icc != null)
            {
                Clipboard.SetDataObject(icc.cur, true);
                delPanel(icc);
            }
        }

        private void pasteImage()
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (iData.GetDataPresent(DataFormats.Bitmap))
            {
                var bmp = (Bitmap)iData.GetData(DataFormats.Bitmap);
                newPanel(bmp, "", "new image(" + fileNumber + ").jpg", true);
                fileNumber++;
            }
        }

        private void undoImage()
        {
            var icc = getActiveICC();
            if (icc != null)
            {
                icc.undo();
                setUndoRedoEnable();
                setDataGrid();
                setHistogramStatisticsLabels();
            }
        }

        private void redoImage()
        {
            var icc = getActiveICC();
            if (icc != null)
            {
                icc.redo();
                setUndoRedoEnable();
                setDataGrid();
                setHistogramStatisticsLabels();
            }
        }

        private void setUndoRedoEnable()
        {
            var icc = getActiveICC();
            if (icc != null)
            {
                this.button9.Enabled = icc.undoEnabled;
                this.button8.Enabled = icc.redoEnabled;
            }
            else
            {
                this.button9.Enabled = false;
                this.button8.Enabled = false;
            }
        }

        private void zoomInImage()
        {
            var icc = getActiveICC();
            if (icc != null)
            {
                icc.zoomIn();
                this.toolStripStatusLabel2.Text = zoomLabel[icc.zoomFac];
            }
        }

        private void zoomOutImage()
        {
            var icc = getActiveICC();
            if (icc != null)
            {
                icc.zoomOut();
                this.toolStripStatusLabel2.Text = zoomLabel[icc.zoomFac];
            }
        }

        private void zoomImageOriginalSize()
        {
            var icc = getActiveICC();
            if (icc != null)
            {
                icc.zoomOriginal();
                this.toolStripStatusLabel2.Text = zoomLabel[icc.zoomFac];
            }
        }

        private void rotateImage()
        {
            var icc = getActiveICC();
            if (icc != null)
            {
                icc.rotate90();
                setUndoRedoEnable();
            }
        }

        private void modifyImage(int function)
        {
            var icc = getActiveICC();
            if (icc != null)
            {
                icc.modifyImage(function);
            }
        }

        #endregion

        #region statistics and histogram

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel13.Refresh();
            setHistogramStatisticsLabels();
        }

        private void panel13_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen;

            int width = this.panel13.Width;
            int height = this.panel13.Height;

            g.FillRectangle(new SolidBrush(Color.LightGray), 0, 0, width, height);
            //g.DrawRectangle(new Pen(foreColor), 0, 0, width + 1, height + 1);

            if (actICC != null)
            {
                switch (this.comboBox2.SelectedIndex)
                {
                    case 0:
                        {
                            pen = new Pen(Color.Red);
                            double ratio = 0.9F / actICC.percentR[actICC.redMode];
                            for (int i = 0; i < 256; i++)
                            {
                                g.DrawLine(pen, new Point(i, height), new Point(i, (height - (int)((float)height * actICC.percentR[i] * ratio))));
                            }
                            break;
                        }
                    case 1:
                        {
                            pen = new Pen(Color.Green);
                            double ratio = 0.9D / actICC.percentG[actICC.greenMode];
                            for (int i = 0; i < 256; i++)
                            {
                                g.DrawLine(pen, new Point(i, height), new Point(i, (height - (int)((float)height * actICC.percentG[i] * ratio))));
                            }
                            break;
                        }
                    case 2:
                        {
                            pen = new Pen(Color.Blue);
                            double ratio = 0.9D / actICC.percentB[actICC.blueMode];
                            for (int i = 0; i < 256; i++)
                            {
                                g.DrawLine(pen, new Point(i, height), new Point(i, (height - (int)((float)height * actICC.percentB[i] * ratio))));
                            }
                            break;
                        }
                    default:
                        return;
                }
            }
        }

        private void panel13_MouseMove(object sender, MouseEventArgs e)
        {
            if (actICC == null)
            {
                setHistogramValueLabels(-1);
            }
            else
            {
                setHistogramValueLabels(e.X);
            }
        }

        private void setDataGrid()
        {
            var icc = actICC;
            this.dataGridView1.Rows.Clear();

            if (icc == null)
            {
                this.dataGridView1.Rows.Add(new string[] { "General", "" });
                this.dataGridView1.Rows.Add(new string[] { "Pixel Count", "-" });
                this.dataGridView1.Rows.Add(new string[] { "", "" });
                this.dataGridView1.Rows.Add(new string[] { "Red Channel", "" });
                this.dataGridView1.Rows.Add(new string[] { "Red Max", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Red Min", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Red Mean", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Red Median", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Red Mode", "-" });
                this.dataGridView1.Rows.Add(new string[] { "", "" });
                this.dataGridView1.Rows.Add(new string[] { "Green Channel", "" });
                this.dataGridView1.Rows.Add(new string[] { "Green Max", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Green Min", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Green Mean", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Green Median", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Green Mode", "-" });
                this.dataGridView1.Rows.Add(new string[] { "", "" });
                this.dataGridView1.Rows.Add(new string[] { "Blue Channel", "" });
                this.dataGridView1.Rows.Add(new string[] { "Blue Max", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Blue Min", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Blue Mean", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Blue Median", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Blue Mode", "-" });
                this.dataGridView1.Rows.Add(new string[] { "", "" });
                this.dataGridView1.Rows.Add(new string[] { "Hue", "" });
                this.dataGridView1.Rows.Add(new string[] { "Hue Max", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Hue Min", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Hue Mean", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Hue Median", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Hue Mode", "-" });
                this.dataGridView1.Rows.Add(new string[] { "", "" });
                this.dataGridView1.Rows.Add(new string[] { "Saturation", "" });
                this.dataGridView1.Rows.Add(new string[] { "Saturation Max", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Saturation Min", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Saturation Mean", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Saturation Median", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Saturation Mode", "-" }); 
                this.dataGridView1.Rows.Add(new string[] { "", "" });
                this.dataGridView1.Rows.Add(new string[] { "Brightness", "" });
                this.dataGridView1.Rows.Add(new string[] { "Brightness Max", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Brightness Min", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Brightness Mean", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Brightness Median", "-" });
                this.dataGridView1.Rows.Add(new string[] { "Brightness Mode", "-" });
            }
            else
            {
                this.dataGridView1.Rows.Add(new string[] { "General", "" });
                this.dataGridView1.Rows.Add(new string[] { "Pixel Count", icc.pixelCount.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "", "" });
                this.dataGridView1.Rows.Add(new string[] { "Red Channel", "" });
                this.dataGridView1.Rows.Add(new string[] { "Red Max", icc.redMax.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "Red Min", icc.redMin.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "Red Mean", icc.redMean.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "Red Median", icc.redMedian.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "Red Mode", icc.redMode.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "", "" });
                this.dataGridView1.Rows.Add(new string[] { "Green Channel", "" });
                this.dataGridView1.Rows.Add(new string[] { "Green Max", icc.greenMax.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "Green Min", icc.greenMin.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "Green Mean", icc.greenMean.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "Green Median", icc.greenMedian.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "Green Mode", icc.greenMode.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "", "" });
                this.dataGridView1.Rows.Add(new string[] { "Blue Channel", "" });
                this.dataGridView1.Rows.Add(new string[] { "Blue Max", icc.blueMax.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "Blue Min", icc.blueMin.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "Blue Mean", icc.blueMean.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "Blue Median", icc.blueMedian.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "Blue Mode", icc.blueMode.ToString() });
                this.dataGridView1.Rows.Add(new string[] { "", "" });
                this.dataGridView1.Rows.Add(new string[] { "Hue", "" });
                this.dataGridView1.Rows.Add(new string[] { "Hue Max", icc.hueMax.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "Hue Min", icc.hueMin.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "Hue Mean", icc.hueMean.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "Hue Median", icc.hueMedian.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "Hue Mode", icc.hueMode.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "", "" });
                this.dataGridView1.Rows.Add(new string[] { "Saturation", "" });
                this.dataGridView1.Rows.Add(new string[] { "Saturation Max", icc.saturationMax.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "Saturation Min", icc.saturationMin.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "Saturation Mean", icc.saturationMean.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "Saturation Median", icc.saturationMedian.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "Saturation Mode", icc.saturationMode.ToString("F2") });
                this.dataGridView1.Rows.Add(new string[] { "", "" });
                this.dataGridView1.Rows.Add(new string[] { "Brightness", "" });
                this.dataGridView1.Rows.Add(new string[] { "Brightness Max", icc.brightnessMax.ToString("P1") });
                this.dataGridView1.Rows.Add(new string[] { "Brightness Min", icc.brightnessMin.ToString("P1") });
                this.dataGridView1.Rows.Add(new string[] { "Brightness Mean", icc.brightnessMean.ToString("P1") });
                this.dataGridView1.Rows.Add(new string[] { "Brightness Median", icc.brightnessMedian.ToString("P1") });
                this.dataGridView1.Rows.Add(new string[] { "Brightness Mode", icc.brightnessMode.ToString("P1") });
            }
        }

        private void setHistogramValueLabels(int n)
        {
            if (n < 0 || n >= 256)
            {
                this.label8.Text = "Value:";
                this.label9.Text = "Count:";
                this.label10.Text = "Percentage:";
            }
            else
            {
                switch (comboBox2.SelectedIndex)
                {

                    case 0:
                        {
                            this.label8.Text = "Value: " + n.ToString();
                            this.label9.Text = "Count: " + actICC.R[n].ToString();
                            this.label10.Text = "Percentage: " + actICC.percentR[n].ToString("P1");
                            break;
                        }
                    case 1:
                        {
                            this.label8.Text = "Value: " + n.ToString();
                            this.label9.Text = "Count: " + actICC.B[n].ToString();
                            this.label10.Text = "Percentage: " + actICC.percentB[n].ToString("P1");
                            break;
                        }
                    case 2:
                        {
                            this.label8.Text = "Value: " + n.ToString();
                            this.label9.Text = "Count: " + actICC.B[n].ToString();
                            this.label10.Text = "Percentage: " + actICC.percentB[n].ToString("P1");
                            break;
                        }
                    default:
                        return;
                }
            }
        }

        private void setHistogramStatisticsLabels()
        {
            if (actICC != null)
            {
                switch (comboBox2.SelectedIndex)
                {
                    case 0:
                        {
                            this.label3.Text = "Max: " + actICC.redMax.ToString();
                            this.label4.Text = "Min: " + actICC.redMin.ToString();
                            this.label5.Text = "Mean: " + actICC.redMean.ToString();
                            this.label6.Text = "Median: " + actICC.redMedian.ToString();
                            this.label7.Text = "Mode: " + actICC.redMode.ToString();
                            break;
                        }
                    case 1:
                        {
                            this.label3.Text = "Max: " + actICC.greenMax.ToString();
                            this.label4.Text = "Min: " + actICC.greenMin.ToString();
                            this.label5.Text = "Mean: " + actICC.greenMean.ToString();
                            this.label6.Text = "Median: " + actICC.greenMedian.ToString();
                            this.label7.Text = "Mode: " + actICC.greenMode.ToString();
                            break;
                        }
                    case 2:
                        {
                            this.label3.Text = "Max: " + actICC.blueMax.ToString();
                            this.label4.Text = "Min: " + actICC.blueMin.ToString();
                            this.label5.Text = "Mean: " + actICC.blueMean.ToString();
                            this.label6.Text = "Median: " + actICC.blueMedian.ToString();
                            this.label7.Text = "Mode: " + actICC.blueMode.ToString();
                            break;
                        }
                    default:
                        {
                            return;
                        }
                }
            }
            else
            {
                this.label3.Text = "Max:";
                this.label4.Text = "Min:";
                this.label5.Text = "Mean:";
                this.label6.Text = "Median:";
                this.label7.Text = "Mode:";
            }
        }

        #endregion
    }
}
