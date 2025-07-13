using SnapDesk.Interfaces;
using SnapDesk.Utilities;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace SnapDesk.Services
{
    // Bu class Windows classina aid funksiyalari cagirmaq ucundur.
    // Yalniz bu assembly-ye aid olsun deye INTERNAL!
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll")]
        // Bu method hal-hazirda isleyen pencerenin handle deyerini,
        // yeni ID ve ya unvanini verir.
        public static extern IntPtr GetConsoleWindow();
        // extern--> methodun bodysi olmadigini gosterir.
        // Yeni bu method .NET icinde yazilmayib, haradasa xarici
        // bir DLL-de movcuddur. Ona gore de DLL ile birlikde 
        // DLLImport atributu istifade olunur.
        [DllImport("user32.dll")]
        // Bu method verilmis bir pencerenin handle-na esasen,
        // hemin pencereni gizledir ve ya gosterir.
        // 2ci parameter bu davranisi teyin edir: 0->gizlet, 5->goster.
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        // Asagidaki constantlar ShowWindow methoduna hansi rejimde islemeli
        // oldugunu bildirir. 
        // Yeni eger ShowWindow(handle,0) desek, hemin pencere ekrandan silinir,
        // amma proqram islemeye davam edir.
        public const int SW_HIDE = 0;
        // Bu constant pencereni yeniden gorunen veziyyete getirmek ucun
        // istifade olunur.
        public const int SW_SHOW = 5;
        // Niye mehz 5? - Bu kodlara nCmdShow parameterleri deyilir,
        // Microsoftun resmi senedlerinde bu sekilde teyin olunub.
        // Meselen: 0-> SW_HIDE, 3-> SW_MAXIMIZE, 9-> SW_RESTORE(pencere minimize olunubsa, evvelki veziyyetine qaytarir.) 
    }

    public class WindowsScreenshotService : IScreenshotService
    {
        private readonly string _imagesPath;

        // Bu constructor screenshotlarin yazilacagi qovlugun unvanini 
        // teyin edir, eger qovluq movcud deyilse, avtomatik yaradilir:
        public WindowsScreenshotService()
        {
            _imagesPath = FileHelper.GetImagesFolderPath();
            FileHelper.EnsureDirectoryExists(_imagesPath);
        }

        public void CaptureAndSave()
        {
            // Console penceresinin unvanini aliriq:
            var h = NativeMethods.GetConsoleWindow();
            // Console penceresini gizledirik:
            NativeMethods.ShowWindow(h, NativeMethods.SW_HIDE);
            // 300 ms gozleyirik ki, console gizlensin
            Thread.Sleep(300);
            // Ekranin sekilini ceken Bitmap obyektini yaradiriq:
            // using istifade etmekle obyektin isimiz biten kimi 
            // RAMdan avtomatik silinmesini temin edirik:
            using var bmp = CaptureScreen();
            SaveImage(bmp);
            // Console penceresini geri qaytaririq:
            NativeMethods.ShowWindow(h, NativeMethods.SW_SHOW);

            Console.WriteLine("Full screenshot taken and saved.");

        }

        public void CaptureRegionAndSave(Rectangle region)
        {
            try
            {
                // Verilen olcude bos sekil(bitmap) yaradiriq:
                using var bmp = new Bitmap(region.Width, region.Height, PixelFormat.Format32bppArgb);
                // Bu bitmapin uzerinde ceke bilmek ucun Graphics obyektini aliriq:
                using var g = Graphics.FromImage(bmp);
                // Ekranin secilmis hissesini bitmap obyektine copy edirik:
                // region.Location → ekranda harada baslasin
                // Point.Empty → bitmap-de 0dan baslasin(0,0)
                // region.Size → ne qeder sahe kopyalansin(eni & hundurluyu)
                // CopyPixelOperation.SourceCopy → pikseli oldugu kimi kopyalasin
                g.CopyFromScreen(region.Location, Point.Empty, region.Size, CopyPixelOperation.SourceCopy);
                // Sekili fayla qeyd edirik:
                SaveImage(bmp);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error taking screenshot! {ex.Message}");
            }
        }

        public void PrintSavedImageNames()
        {
            Console.WriteLine("\nSaved Screenshots:");

            var files = Directory.GetFiles(_imagesPath, "*.png");

            if (files.Length == 0)
                Console.WriteLine("(No screenshots found)");

            foreach (var f in files)
                Console.WriteLine($"   → {Path.GetFileName(f)}");

        }

        private Bitmap CaptureScreen()
        {
            // App.manifest faylinda <dpiAware>true</dpiAware> elave 
            // olunduguna gore proqram DPI-aware olur, yeni
            // ekranin zoom seviyyesi(meselen: 125%, 150%) nezere alinir.
            // Eger elave olunmasaydi, zoom 150% olanda screenshotlar yarimciq alinardi.
            // Proyekt DPI-aware oldugunu gore duzgun olcunu qaytaracaq:
            Rectangle bounds = Screen.PrimaryScreen!.Bounds;

            // Debug meqsedile olculer konsola yazilir:
            Console.WriteLine($"[DEBUG] Screen Bounds: {bounds}");

            var bmp = new Bitmap(bounds.Width, bounds.Height);
            using var g = Graphics.FromImage(bmp);
            g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);

            return bmp;

        }

        private void SaveImage(Bitmap bmp)
        {

            string name = $"screenshot_{DateTime.Now:yyyy-MM-dd_HH.mm.ss}.png";
            string path = Path.Combine(_imagesPath, name);
            // Bitmap sekili gonderilen path-de PNG formatinda file kimi save edir:
            bmp.Save(path, ImageFormat.Png);

            Console.WriteLine($"Saved: {name}");

        }
    }
}