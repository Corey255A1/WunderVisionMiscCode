using System;
using GZipMultiLib;
using System.IO;
using System.Collections.Generic;
namespace GZipMulti
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            List<FileInfo> files = new List<FileInfo>();
            foreach(string file in Directory.EnumerateFiles("C:\\Temp\\InputFolder\\"))
            {
                files.Add(new FileInfo(file));
            }

            GZipFiles.Compress("C:\\Temp\\Output.package", files);
            GZipFiles.Decompress("C:\\Temp\\Output.package", "C:\\Temp\\OutputFolder\\");

            Console.WriteLine("DONE");


        }
    }
}
