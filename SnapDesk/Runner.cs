using SnapDesk.Interfaces;
using SnapDesk.Services;
using SnapDesk.Utilities;
using SnapDesk.WindowsFormsHelpers;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace SnapDesk;
public class Runner
{
    private readonly IScreenshotService _screenshotService;

    public Runner()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            _screenshotService = new WindowsScreenshotService();

        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            _screenshotService = new MacScreenshotService();

        else
            throw new PlatformNotSupportedException();
    }

    public void Run()
    {
        while (true)
        {

            Console.WriteLine("\nSnapDesk - Screenshot Tool");
            Console.WriteLine("1. Full screenshot");
            Console.WriteLine("2. Region (coordinates)");
            Console.WriteLine("3. List saved");
            Console.WriteLine("4. Press Enter to screenshot");
            Console.WriteLine("5. Select region with mouse");
            Console.WriteLine("0. Exit");

            string opt = ConsoleHelper.PromptOption("Choice: ", new[] { "1", "2", "3", "4", "5", "0" });

            bool exit = opt switch
            {
                "1" => ConsoleHelper.Execute(() => _screenshotService.CaptureAndSave()),
                "2" => ConsoleHelper.Execute(() =>
                {
                    var r = AskCoords();
                    _screenshotService.CaptureRegionAndSave(r);
                }),
                "3" => ConsoleHelper.Execute(() => _screenshotService.PrintSavedImageNames()),
                "4" => ConsoleHelper.Execute(() =>
                {
                    Console.WriteLine("Press ENTER...");
                    while (Console.ReadKey(true).Key != ConsoleKey.Enter) ;
                    _screenshotService.CaptureAndSave();
                }),
                "5" => ConsoleHelper.Execute(() =>
                {
                    var r = MouseRegionSelector.SelectRegion();
                    if (r.Width > 0 && r.Height > 0)
                        _screenshotService.CaptureRegionAndSave(r);
                }),
                "0" => true,
                _ => ConsoleHelper.ShowInvalid()
            };

            if (exit) break;

        }
    }

    private static Rectangle AskCoords()
    {
        int x = ConsoleHelper.PromptInt("X: ");
        int y = ConsoleHelper.PromptInt("Y: ");
        int w = ConsoleHelper.PromptInt("Width: ");
        int h = ConsoleHelper.PromptInt("Height: ");

        return new Rectangle(x, y, w, h);
    }

}