using System;
using System.IO;

namespace SnapDesk.Utilities;
public static class FileHelper
{

    public static string GetImagesFolderPath()
    {

        string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        return Path.Combine(Desktop, "Images");

    }

    public static void EnsureDirectoryExists(string path)
    {

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

    }
    
    //public static string[] GetAllImageNames(string path)
    //{
    //    if (!Directory.Exists(path))
    //    {

    //        return Array.Empty<string>();

    //    }
    //    return Directory.GetFiles(path, "*.png");

    //}

}