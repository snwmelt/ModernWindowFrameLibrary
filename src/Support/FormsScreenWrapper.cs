using System;
using System.Windows.Forms;
using System.Drawing;

namespace ModernWindowFrameLibrary.Support
{
    internal static class FormsScreenWrapper
    {
        internal static double[] currentDisplayAvailableBounds()
        {
            Screen[] displays       = Screen.AllScreens;
            Screen   currentDisplay = null;
            double[] bounds         = new double[4];
            double   mouseXPos      = System.Windows.Forms.Control.MousePosition.X;
            double   mouseYPos      = System.Windows.Forms.Control.MousePosition.Y;

            for (int i = 0; i < displays.Length; i++)
            {
                if (displays[i].Bounds.Left <= mouseXPos && displays[i].Bounds.Right >= mouseXPos)
                {
                    if (displays[i].Bounds.Top <= mouseYPos && displays[i].Bounds.Bottom >= mouseYPos)
                    {
                        currentDisplay = displays[i];
                        break;
                    }
                }
            }

            if (currentDisplay == null) return null;

            bounds[0] = currentDisplay.WorkingArea.Y;
            bounds[1] = currentDisplay.WorkingArea.X;
            bounds[2] = currentDisplay.WorkingArea.Width;
            bounds[3] = currentDisplay.WorkingArea.Height;

            return bounds;
        }

        internal static double[] displayAvailableBounds(int x, int y)
        {
            Screen[] displays       = Screen.AllScreens;
            Screen   currentDisplay = null;
            double[] bounds         = new double[4];

            for (int i = 0; i < displays.Length; i++)
            {
                if (displays[i].Bounds.Left <= x && displays[i].Bounds.Right >= x)
                {
                    if (displays[i].Bounds.Top <= y && displays[i].Bounds.Bottom >= y)
                    {
                        currentDisplay = displays[i];
                        break;
                    }
                }
            }

            if (currentDisplay == null) return null;

            bounds[0] = currentDisplay.WorkingArea.Y;
            bounds[1] = currentDisplay.WorkingArea.X;
            bounds[2] = currentDisplay.WorkingArea.Width;
            bounds[3] = currentDisplay.WorkingArea.Height;
            
            return bounds;
        }

        internal static System.Windows.Point mouseLocation()
        {
            System.Windows.Point point = new System.Windows.Point();
            point.X = Control.MousePosition.X;
            point.Y = Control.MousePosition.Y;

            return point;
        }
    }
}
