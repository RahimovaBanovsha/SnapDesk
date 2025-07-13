using SnapDesk.Interfaces;
using SnapDesk.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace SnapDesk.Services;
public class MacScreenshotService : IScreenshotService
{

    private readonly string _imagesPath;

    public MacScreenshotService()
    {
        _imagesPath = FileHelper.GetImagesFolderPath();
        FileHelper.EnsureDirectoryExists(_imagesPath);

    }

    public void CaptureAndSave()
    {
        string FileName = $"screenshot_{DateTime.Now:MMM-dd-yyyy_HH.mm.ss}.png";
        string FullPath = Path.Combine(_imagesPath, FileName);

        try
        {
            // MacOS terminalinda screencapture command ise salinir.
            // Bu command ekranin sekilini cekib gosterilen path-de saxlayir:
            Process.Start("screencapture", $"\"{FullPath}\"");

            Console.WriteLine($"Saved: {FileName}");
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while taking the screenshot: {ex.Message}");
        }
    }

    public void CaptureRegionAndSave(Rectangle region)
    {

        string FileName = $"screenshot_{DateTime.Now:MMM-dd-yyyy_HH.mm.ss}.png";
        string FullPath = Path.Combine(_imagesPath, FileName);
        // CommandArgs Screencapture commanda hansi regionun cekileceyini gosterir:
        string CommandArgs= $"-R{region.X},{region.Y},{region.Width},{region.Height} \"{FullPath}\"";
        // -R-->bu flag region ucun istifade olunur ve ardindan
        // 4 deyer gelir: X--> sahenin basladigi ufuqi koordinati
        // Y--> sahenin basladigi saquli koordinati
        // Width--> sahenin eni, Height--> sahenin hundurluyu

        Process.Start("screencapture", CommandArgs);
        Console.WriteLine($"Region Saved: {FileName}");

    }

    public void PrintSavedImageNames()
    {

        Console.WriteLine("\nSaved Screenshots: ");
        // Yalniz ".png" uzantili fayllari tapir ve onlari array-e yukleyir.
        // Directory.GetFiles(...) methodu netice olaraq string[],
        // yeni string array qaytarir!
        var files = Directory.GetFiles(_imagesPath, "*.png");
        if (files.Length == 0)
        {
            Console.WriteLine("(No screenshots found)");
            return;

        }

        foreach(var file in files)
        {
            Console.WriteLine($"   → {Path.GetFileName(file)}");
        }
    }

}
