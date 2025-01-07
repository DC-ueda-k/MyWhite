using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MyWhite
{
    public partial class Form1 : Form
    {
        private DrawingManager drawingManager;
        private UndoRedoManager undoRedoManager;
        private SettingsManager settingsManager;
        private Label pathLabel;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            drawingManager = new DrawingManager(this.ClientSize.Width, this.ClientSize.Height);
            undoRedoManager = new UndoRedoManager();
            settingsManager = new SettingsManager();

            this.Paint += new PaintEventHandler(Form1_Paint);
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            this.MouseUp += new MouseEventHandler(Form1_MouseUp);
            this.Resize += new EventHandler(Form1_Resize);

            // Enable key preview to capture key events at the form level  
            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);

            // Add MenuStrip  
            MenuStrip menuStrip = new MenuStrip();
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
            ToolStripMenuItem smallFontSizeMenuItem = AddFontSizeOption(fontSizeMenu, "小", 12);
            ToolStripMenuItem mediumFontSizeMenuItem = AddFontSizeOption(fontSizeMenu, "中", 16);
            ToolStripMenuItem largeFontSizeMenuItem = AddFontSizeOption(fontSizeMenu, "大", 20);

            // Set default font size menu item checked  
            smallFontSizeMenuItem.Checked = true;

            // Add Font Color menu  
            ToolStripMenuItem fontColorMenu = new ToolStripMenuItem("色");
            fontSettingsMenu.DropDownItems.Add(fontColorMenu);

            // Add font color options  
            ToolStripMenuItem blackColorMenuItem = AddFontColorOption(fontColorMenu, "黒", Color.Black);
            ToolStripMenuItem redColorMenuItem = AddFontColorOption(fontColorMenu, "赤", Color.Red);
            ToolStripMenuItem whiteColorMenuItem = AddFontColorOption(fontColorMenu, "白", Color.White);

            // Set default font color menu item checked  
            blackColorMenuItem.Checked = true;

            // Add Pen Settings menu  
            ToolStripMenuItem penSettingsMenu = new ToolStripMenuItem("ペン");
            menuStrip.Items.Add(penSettingsMenu);

            // Add Pen Width menu  
            ToolStripMenuItem penWidthMenu = new ToolStripMenuItem("太さ");
            penSettingsMenu.DropDownItems.Add(penWidthMenu);

            // Add pen width options  
            ToolStripMenuItem thinPenWidthMenuItem = AddPenWidthOption(penWidthMenu, "細", 2);
            ToolStripMenuItem mediumPenWidthMenuItem = AddPenWidthOption(penWidthMenu, "中", 4);
            ToolStripMenuItem thickPenWidthMenuItem = AddPenWidthOption(penWidthMenu, "太", 6);

            // Set default pen width menu item checked  
            thinPenWidthMenuItem.Checked = true;

            // Add Pen Color menu  
            ToolStripMenuItem penColorMenu = new ToolStripMenuItem("色");
            penSettingsMenu.DropDownItems.Add(penColorMenu);

            // Add pen color options  
            ToolStripMenuItem blackPenColorMenuItem = AddPenColorOption(penColorMenu, "黒", Color.Black);
            ToolStripMenuItem redPenColorMenuItem = AddPenColorOption(penColorMenu, "赤", Color.Red);
            ToolStripMenuItem whitePenColorMenuItem = AddPenColorOption(penColorMenu, "白", Color.White);

            // Set default pen color menu item checked  
            blackPenColorMenuItem.Checked = true;

            // Add path label  
            pathLabel = new Label();
            pathLabel.Location = new Point(10, menuStrip.Bottom + 10);
            pathLabel.AutoSize = true;
            this.Controls.Add(pathLabel);

            // Load saved settings  
            settingsManager.LoadSettings();
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
                drawingManager.SelectedFontSize = (int)menuItem.Tag;
                // Uncheck all font size menu items  
                foreach (ToolStripMenuItem item in menuItem.GetCurrentParent().Items)
                {
                    item.Checked = false;
                }
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
                drawingManager.SelectedFontColor = (Color)menuItem.Tag;
                // Uncheck all font color menu items  
                foreach (ToolStripMenuItem item in menuItem.GetCurrentParent().Items)
                {
                    item.Checked = false;
                }
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
                drawingManager.SelectedPenWidth = (int)menuItem.Tag;
                // Uncheck all pen width menu items  
                foreach (ToolStripMenuItem item in menuItem.GetCurrentParent().Items)
                {
                    item.Checked = false;
                }
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
                drawingManager.SelectedPenColor = (Color)menuItem.Tag;
                // Uncheck all pen color menu items  
                foreach (ToolStripMenuItem item in menuItem.GetCurrentParent().Items)
                {
                    item.Checked = false;
                }
                // Check the selected pen color menu item  
                menuItem.Checked = true;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(drawingManager.DrawingBitmap, Point.Empty);
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drawingManager.StartDrawing(e.Location);
                undoRedoManager.SaveState(drawingManager.DrawingBitmap);
            }
            else if (e.Button == MouseButtons.Right)
            {
                undoRedoManager.SaveState(drawingManager.DrawingBitmap);
                string text = Microsoft.VisualBasic.Interaction.InputBox("挿入する文字:", "文字挿入", "", e.X, e.Y);
                if (!string.IsNullOrEmpty(text))
                {
                    drawingManager.DrawText(text, e.Location);
                    this.Invalidate();
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawingManager.IsDrawing && e.Button == MouseButtons.Left)
            {
                drawingManager.DrawLine(e.Location);
                this.Invalidate();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drawingManager.StopDrawing();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Z)
            {
                drawingManager.DrawingBitmap = undoRedoManager.Undo(drawingManager.DrawingBitmap);
                this.Invalidate();
                e.SuppressKeyPress = true; // キーイベントを処理済みにする  
            }
            else if (e.Control && e.KeyCode == Keys.Y)
            {
                drawingManager.DrawingBitmap = undoRedoManager.Redo(drawingManager.DrawingBitmap);
                this.Invalidate();
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
            drawingManager.Resize(this.ClientSize.Width, this.ClientSize.Height);
            this.Invalidate();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    settingsManager.ScreenshotSavePath = dialog.SelectedPath;
                    settingsManager.SaveSettings();
                    UpdatePathLabel(); // Update label when path changes  
                }
            }
        }

        private void UpdatePathLabel()
        {
            pathLabel.Text = string.IsNullOrEmpty(settingsManager.ScreenshotSavePath) ? "保存先が未設定です。設定メニューから保存先を設定してください。" : "";
        }

        private void SaveScreenshot()
        {
            if (string.IsNullOrEmpty(settingsManager.ScreenshotSavePath))
            {
                MessageBox.Show("スクリーンショットの保存先を設定してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string filename = $"MyWhite_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string filepath = Path.Combine(settingsManager.ScreenshotSavePath, filename);
            drawingManager.DrawingBitmap.Save(filepath, System.Drawing.Imaging.ImageFormat.Png);
            MessageBox.Show($"スクリーンショットが保存されました: {filepath}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}
