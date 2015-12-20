using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace SnagFree.TrayApp.Core
{
    static class ScreenshotTaker
    {
        public static Bitmap TakeScreenshot()
        {
            Bitmap screenshot = new Bitmap(
                SystemInformation.VirtualScreen.Width,
                SystemInformation.VirtualScreen.Height,
                PixelFormat.Format32bppArgb);

            Graphics screenGraph = Graphics.FromImage(screenshot);

            screenGraph.CopyFromScreen(
                SystemInformation.VirtualScreen.X,
                SystemInformation.VirtualScreen.Y,
                0,
                0,
                SystemInformation.VirtualScreen.Size,
                CopyPixelOperation.SourceCopy);

            return screenshot;
        }
    }
}
