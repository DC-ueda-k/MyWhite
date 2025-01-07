using System;
using System.Collections.Generic;
using System.Drawing;

namespace MyWhite
{
    public class DrawingManager
    {
        private Bitmap drawingBitmap;
        private Graphics graphics;
        private bool isDrawing;
        private Point lastPoint;

        public Color SelectedPenColor { get; set; }
        public int SelectedPenWidth { get; set; }
        public Color SelectedFontColor { get; set; }
        public int SelectedFontSize { get; set; }

        public Bitmap DrawingBitmap
        {
            get { return drawingBitmap; }
            set
            {
                if (drawingBitmap != null)
                {
                    drawingBitmap.Dispose();
                }
                drawingBitmap = value;
                graphics = Graphics.FromImage(drawingBitmap);
            }
        }

        public bool IsDrawing
        {
            get { return isDrawing; }
        }

        public DrawingManager(int width, int height)
        {
            drawingBitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(drawingBitmap);
            SelectedPenColor = Color.Black;
            SelectedPenWidth = 2;
            SelectedFontColor = Color.Black;
            SelectedFontSize = 12;
        }

        public void StartDrawing(Point startPoint)
        {
            isDrawing = true;
            lastPoint = startPoint;
        }

        public void DrawLine(Point currentPoint)
        {
            if (isDrawing)
            {
                using (Pen pen = new Pen(SelectedPenColor, SelectedPenWidth))
                {
                    graphics.DrawLine(pen, lastPoint, currentPoint);
                }
                lastPoint = currentPoint;
            }
        }

        public void StopDrawing()
        {
            isDrawing = false;
        }

        public void DrawText(string text, Point location)
        {
            using (Font font = new Font("Arial", SelectedFontSize))
            using (Brush brush = new SolidBrush(SelectedFontColor))
            {
                graphics.DrawString(text, font, brush, location);
            }
        }

        public void Resize(int width, int height)
        {
            Bitmap newBitmap = new Bitmap(width, height);
            using (Graphics newGraphics = Graphics.FromImage(newBitmap))
            {
                newGraphics.DrawImageUnscaled(drawingBitmap, Point.Empty);
            }
            DrawingBitmap = newBitmap;
        }
    }
}
