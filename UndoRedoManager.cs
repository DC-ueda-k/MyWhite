using System;
using System.Collections.Generic;
using System.Drawing;

namespace MyWhite
{
    public class UndoRedoManager
    {
        private Stack<Bitmap> undoStack;
        private Stack<Bitmap> redoStack;

        public UndoRedoManager()
        {
            undoStack = new Stack<Bitmap>();
            redoStack = new Stack<Bitmap>();
        }

        public void SaveState(Bitmap bitmap)
        {
            undoStack.Push((Bitmap)bitmap.Clone());
            redoStack.Clear();
        }

        public Bitmap Undo(Bitmap currentBitmap)
        {
            if (undoStack.Count > 0)
            {
                redoStack.Push((Bitmap)currentBitmap.Clone());
                return undoStack.Pop();
            }
            return currentBitmap;
        }

        public Bitmap Redo(Bitmap currentBitmap)
        {
            if (redoStack.Count > 0)
            {
                undoStack.Push((Bitmap)currentBitmap.Clone());
                return redoStack.Pop();
            }
            return currentBitmap;
        }
    }
}
