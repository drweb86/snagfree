using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using WixSharp;

namespace SnagFree.Setup
{
    class Program
    {
        public static string RootDirectory
        {
            get
            {
                string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(Path.GetDirectoryName(path));
            }
        }

        public static string ArtifactsDirectory => Path.Combine(RootDirectory, "artifacts");
        public static string WixSharpDirectory => Path.Combine(RootDirectory, @"src\third-party\WixSharp");
        public static string WixSdkDirectory => Path.Combine(RootDirectory, @"src\third-party\WixSharp\Wix_bin");
        public static string WixSdkBinDirectory => Path.Combine(RootDirectory, @"src\third-party\WixSharp\Wix_bin\bin");

        public static void Main()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += LoadFromSameFolder;

            DoJob();
        }

        private static void DoJob()
        {
            var msiFile = Path.Combine(ArtifactsDirectory, $"SnagFree-{DateTime.Now:yyyy-MM-dd}.msi");

            var project = new Project("SnagFree");

            Compiler.WixLocation = WixSdkBinDirectory;
            Compiler.BuildMsi(project, msiFile);
        }

        static System.Reflection.Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
            var searchAssemblyName = new AssemblyName(args.Name).Name + ".dll";

            var searchLibraries = new List<string>();
            searchLibraries.Add(Path.Combine(WixSharpDirectory, searchAssemblyName));
            searchLibraries.Add(Path.Combine(WixSdkBinDirectory, searchAssemblyName));

            var resultFile = searchLibraries.FirstOrDefault(System.IO.File.Exists);
            
            return resultFile != null ? System.Reflection.Assembly.LoadFile(resultFile) : null;
        }
    }
}
