using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MyWhite
{
    public partial class Form1 : Form
    {
        private bool isDrawing = false;
        private Point lastPoint = Point.Empty;
        private Bitmap drawingBitmap;
        private Graphics graphics;
        private Stack<Bitmap> undoStack = new Stack<Bitmap>();
        private Stack<Bitmap> redoStack = new Stack<Bitmap>();
        private string screenshotSavePath = "";
        private Label pathLabel;
        private MenuStrip menuStrip;
        private int selectedFontSize = 12; // デフォルトの文字サイズ  
        private ToolStripMenuItem smallFontSizeMenuItem;
        private ToolStripMenuItem mediumFontSizeMenuItem;
        private ToolStripMenuItem largeFontSizeMenuItem;
        private Color selectedFontColor = Color.Black; // デフォルトの文字色  
        private ToolStripMenuItem blackColorMenuItem;
        private ToolStripMenuItem redColorMenuItem;
        private ToolStripMenuItem whiteColorMenuItem;
        private int selectedPenWidth = 2; // デフォルトのペンの太さ  
        private ToolStripMenuItem thinPenWidthMenuItem;
        private ToolStripMenuItem mediumPenWidthMenuItem;
        private ToolStripMenuItem thickPenWidthMenuItem;
        private Color selectedPenColor = Color.Black; // デフォルトのペンの色  
        private ToolStripMenuItem blackPenColorMenuItem;
        private ToolStripMenuItem redPenColorMenuItem;
        private ToolStripMenuItem whitePenColorMenuItem;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.drawingBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
            this.graphics = Graphics.FromImage(this.drawingBitmap);
            this.graphics.Clear(Color.White);
            this.Paint += new PaintEventHandler(Form1_Paint);
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            this.MouseUp += new MouseEventHandler(Form1_MouseUp);
            this.Resize += new EventHandler(Form1_Resize);

            // Enable key preview to capture key events at the form level  
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            // Add MenuStrip  
            menuStrip = new MenuStrip();
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);

            // Add File menu  
            ToolStripMenuItem fileMenu = new ToolStripMenuItem("設定");
            menuStrip.Items.Add(fileMenu);

            // Add Settings menu item  
            ToolStripMenuItem settingsMenuItem = new ToolStripMenuItem("保存先の設定");
            settingsMenuItem.Click += new EventHandler(SettingsButton_Click);
            fileMenu.DropDownItems.Add(settingsMenuItem);

            // Add Font Settings menu  
            ToolStripMenuItem fontSettingsMenu = new ToolStripMenuItem("文字");
            menuStrip.Items.Add(fontSettingsMenu);

            // Add Font Size menu  
            ToolStripMenuItem fontSizeMenu = new ToolStripMenuItem("サイズ");
            fontSettingsMenu.DropDownItems.Add(fontSizeMenu);

            // Add font size options  
            smallFontSizeMenuItem = AddFontSizeOption(fontSizeMenu, "小", 12);
            mediumFontSizeMenuItem = AddFontSizeOption(fontSizeMenu, "中", 16);
            largeFontSizeMenuItem = AddFontSizeOption(fontSizeMenu, "大", 20);

            // Set default font size menu item checked  
            smallFontSizeMenuItem.Checked = true;

            // Add Font Color menu  
            ToolStripMenuItem fontColorMenu = new ToolStripMenuItem("色");
            fontSettingsMenu.DropDownItems.Add(fontColorMenu);

            // Add font color options  
            blackColorMenuItem = AddFontColorOption(fontColorMenu, "黒", Color.Black);
            redColorMenuItem = AddFontColorOption(fontColorMenu, "赤", Color.Red);
            whiteColorMenuItem = AddFontColorOption(fontColorMenu, "白", Color.White);

            // Set default font color menu item checked  
            blackColorMenuItem.Checked = true;

            // Add Pen Settings menu  
            ToolStripMenuItem penSettingsMenu = new ToolStripMenuItem("ペン");
            menuStrip.Items.Add(penSettingsMenu);

            // Add Pen Width menu  
            ToolStripMenuItem penWidthMenu = new ToolStripMenuItem("太さ");
            penSettingsMenu.DropDownItems.Add(penWidthMenu);

            // Add pen width options  
            thinPenWidthMenuItem = AddPenWidthOption(penWidthMenu, "細", 2);
            mediumPenWidthMenuItem = AddPenWidthOption(penWidthMenu, "中", 4);
            thickPenWidthMenuItem = AddPenWidthOption(penWidthMenu, "太", 6);

            // Set default pen width menu item checked  
            thinPenWidthMenuItem.Checked = true;

            // Add Pen Color menu  
            ToolStripMenuItem penColorMenu = new ToolStripMenuItem("色");
            penSettingsMenu.DropDownItems.Add(penColorMenu);

            // Add pen color options  
            blackPenColorMenuItem = AddPenColorOption(penColorMenu, "黒", Color.Black);
            redPenColorMenuItem = AddPenColorOption(penColorMenu, "赤", Color.Red);
            whitePenColorMenuItem = AddPenColorOption(penColorMenu, "白", Color.White);

            // Set default pen color menu item checked  
            blackPenColorMenuItem.Checked = true;

            // Add path label  
            pathLabel = new Label();
            pathLabel.Location = new Point(10, menuStrip.Bottom + 10);
            pathLabel.AutoSize = true;
            this.Controls.Add(pathLabel);

            // Load saved settings  
            LoadSettings();
            // Update path label with current save path  
            UpdatePathLabel();
        }

        private ToolStripMenuItem AddFontSizeOption(ToolStripMenuItem parentMenu, string label, int fontSize)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem(label);
            menuItem.Tag = fontSize;
            menuItem.Click += new EventHandler(FontSizeMenuItem_Click);
            parentMenu.DropDownItems.Add(menuItem);
            return menuItem;
        }

        private void FontSizeMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                selectedFontSize = (int)menuItem.Tag;
                // Uncheck all font size menu items  
                smallFontSizeMenuItem.Checked = false;
                mediumFontSizeMenuItem.Checked = false;
                largeFontSizeMenuItem.Checked = false;
                // Check the selected font size menu item  
                menuItem.Checked = true;
            }
        }

        private ToolStripMenuItem AddFontColorOption(ToolStripMenuItem parentMenu, string label, Color color)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem(label);
            menuItem.Tag = color;
            menuItem.Click += new EventHandler(FontColorMenuItem_Click);
            parentMenu.DropDownItems.Add(menuItem);
            return menuItem;
        }

        private void FontColorMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                selectedFontColor = (Color)menuItem.Tag;
                // Uncheck all font color menu items  
                blackColorMenuItem.Checked = false;
                redColorMenuItem.Checked = false;
                whiteColorMenuItem.Checked = false;
                // Check the selected font color menu item  
                menuItem.Checked = true;
            }
        }

        private ToolStripMenuItem AddPenWidthOption(ToolStripMenuItem parentMenu, string label, int width)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem(label);
            menuItem.Tag = width;
            menuItem.Click += new EventHandler(PenWidthMenuItem_Click);
            parentMenu.DropDownItems.Add(menuItem);
            return menuItem;
        }

        private void PenWidthMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                selectedPenWidth = (int)menuItem.Tag;
                // Uncheck all pen width menu items  
                thinPenWidthMenuItem.Checked = false;
                mediumPenWidthMenuItem.Checked = false;
                thickPenWidthMenuItem.Checked = false;
                // Check the selected pen width menu item  
                menuItem.Checked = true;
            }
        }

        private ToolStripMenuItem AddPenColorOption(ToolStripMenuItem parentMenu, string label, Color color)
        {
            ToolStripMenuItem menuItem = new ToolStripMenuItem(label);
            menuItem.Tag = color;
            menuItem.Click += new EventHandler(PenColorMenuItem_Click);
            parentMenu.DropDownItems.Add(menuItem);
            return menuItem;
        }

        private void PenColorMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                selectedPenColor = (Color)menuItem.Tag;
                // Uncheck all pen color menu items  
                blackPenColorMenuItem.Checked = false;
                redPenColorMenuItem.Checked = false;
                whitePenColorMenuItem.Checked = false;
                // Check the selected pen color menu item  
                menuItem.Checked = true;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(this.drawingBitmap, Point.Empty);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = true;
                lastPoint = e.Location;
                SaveState();
            }
            else if (e.Button == MouseButtons.Right)
            {
                SaveState();
                string text = Microsoft.VisualBasic.Interaction.InputBox("挿入する文字:", "文字挿入", "", e.X, e.Y);
                if (!string.IsNullOrEmpty(text))
                {
                    using (Font font = new Font("Arial", selectedFontSize))
                    using (Brush brush = new SolidBrush(selectedFontColor))
                    {
                        this.graphics.DrawString(text, font, brush, e.Location);
                    }
                    this.Invalidate();
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.Button == MouseButtons.Left)
            {
                using (Pen pen = new Pen(selectedPenColor, selectedPenWidth))
                {
                    this.graphics.DrawLine(pen, lastPoint, e.Location);
                }
                lastPoint = e.Location;
                this.Invalidate();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDrawing = false;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                Undo();
                e.SuppressKeyPress = true; // キーイベントを処理済みにする  
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                Redo();
                e.SuppressKeyPress = true; // キーイベントを処理済みにする  
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                SaveScreenshot();
                e.SuppressKeyPress = true; // キーイベントを処理済みにする  
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.ClientSize.Width > 0 && this.ClientSize.Height > 0)
            {
                Bitmap newBitmap = new Bitmap(this.ClientSize.Width, this.ClientSize.Height);
                Graphics newGraphics = Graphics.FromImage(newBitmap);
                newGraphics.Clear(Color.White);
                newGraphics.DrawImage(this.drawingBitmap, new Rectangle(Point.Empty, newBitmap.Size));
                this.drawingBitmap = newBitmap;
                this.graphics = newGraphics;
                this.Invalidate();
            }
        }

        private void SaveState()
        {
            Bitmap bitmapCopy = new Bitmap(this.drawingBitmap);
            undoStack.Push(bitmapCopy);
            redoStack.Clear();  // 新しい操作が行われたらRedoスタックをクリア  
        }

        private void Undo()
        {
            if (undoStack.Count > 0)
            {
                redoStack.Push(new Bitmap(this.drawingBitmap));
                this.drawingBitmap = undoStack.Pop();
                this.graphics = Graphics.FromImage(this.drawingBitmap);
                this.Invalidate();
            }
        }

        private void Redo()
        {
            if (redoStack.Count > 0)
            {
                undoStack.Push(new Bitmap(this.drawingBitmap));
                this.drawingBitmap = redoStack.Pop();
                this.graphics = Graphics.FromImage(this.drawingBitmap);
                this.Invalidate();
            }
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    screenshotSavePath = dialog.SelectedPath;
                    SaveSettings();
                    UpdatePathLabel(); // Update label when path changes  
                }
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.ScreenshotSavePath = screenshotSavePath;
            Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {
            screenshotSavePath = Properties.Settings.Default.ScreenshotSavePath;
        }

        private void UpdatePathLabel()
        {
            pathLabel.Text = string.IsNullOrEmpty(screenshotSavePath) ? "保存先が未設定です。設定メニューから保存先を設定してください。" : "";
        }

        private void SaveScreenshot()
        {
            if (string.IsNullOrEmpty(screenshotSavePath))
            {
                MessageBox.Show("スクリーンショットの保存先を設定してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string filename = $"MyWhite_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string filepath = Path.Combine(screenshotSavePath, filename);
            drawingBitmap.Save(filepath, System.Drawing.Imaging.ImageFormat.Png);
            MessageBox.Show($"スクリーンショットが保存されました: {filepath}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}