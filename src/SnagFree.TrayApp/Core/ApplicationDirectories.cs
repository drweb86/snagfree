﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnagFree.TrayApp.Core
{
    static class ApplicationDirectories
    {
        public static string SnagFreeLibrary { get; private set; }

        static ApplicationDirectories()
        {
            SnagFreeLibrary = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                ApplicationInformation.Name);

            if (!Directory.Exists(SnagFreeLibrary))
                Directory.CreateDirectory(SnagFreeLibrary);
        }

        public static string GenerateOutputScreenshotFileName()
        {
            string fileName;

            do
            {
                fileName = Path.Combine(SnagFreeLibrary, $"{DateTime.Now:yyyy-MM-dd HH-mm-ss}.bmp");
            }
            while (File.Exists(fileName));

            return fileName;
        }
    }
}
