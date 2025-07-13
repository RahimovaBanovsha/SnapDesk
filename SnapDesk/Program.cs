using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SnapDesk;
class Program
{
    // Bu Windows-un daxili funksiyasidir. Bezi ekranlarda Windows her seyi
    // avtomatik boyudur(zoom in). Eger proqramda bu miqyaslama nezere alinmasa,
    // goruntuler yanlis olcude gorune biler.
    // DPI‐aware olmaq ucun:
    [DllImport("user32.dll")]
    private static extern bool SetProcessDPIAware();
    // Single Threaded Apartment
    // Windows Forms duzgun islesin deye proqramin tek thread ile basladigi gosterilir:
    [STAThread]
    static void Main(string[] args)
    {
        // 1) Proqramı DPI‐aware edirik
        SetProcessDPIAware();

        // Buttonlar ve diger controllerler Windows 10/11 style olsun deye:
        Application.EnableVisualStyles();
        // Modern rendering method active olunur:
        Application.SetCompatibleTextRenderingDefault(false);

        try
        {
            var runner = new Runner();
            runner.Run();
        }
        catch (PlatformNotSupportedException)
        {
            Console.WriteLine("This operating system is not supported.");
        }
    }

}
