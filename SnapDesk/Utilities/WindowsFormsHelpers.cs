using System;
using System.Drawing;
using System.Windows.Forms;

namespace SnapDesk.WindowsFormsHelpers;
public static class MouseRegionSelector
{
    public static Rectangle SelectRegion()
    {
        // selected deyiseni eni ve hundurluyu 0 olan Rectangle kimi 
        // teyin olunur. Daha sonra user mouse ile region secdikde 
        // bu deyisene real koordinatlar yazilir.
        Rectangle selected = Rectangle.Empty;
        var screen = SystemInformation.VirtualScreen;

        using var form = new Form
        {
            BackColor = Color.White,
            Opacity = 0.25, // 0-> tam seffaf, 1-> tam gorunen
            FormBorderStyle = FormBorderStyle.None,
            Bounds = screen,
            TopMost = true,
            StartPosition = FormStartPosition.Manual
        };

        Point start = Point.Empty, end = Point.Empty;
        bool dragging = false;

        // EVENT HANDLERS:
        // MouseDown, MouseMove, MouseUp, Paint

        // Mouse-un sol duymesi sixildiqda(secime baslayiriq):
        form.MouseDown += (s, e) =>
        {
            dragging = true;
            start = Cursor.Position; // Mouse ile ilk click olunan noqte

        };

        form.MouseMove += (s, e) =>
        {
            if (dragging)
            {
                end = Cursor.Position;
                form.Invalidate(); // Paint event trigger olunur
            }

        };

        // Mouse-un duymesi buraxildiqda(secimi bitiririk):
        form.MouseUp += (s, e) =>
        {
            dragging = false; // Artiq user sahe secimini bitirib
            end = Cursor.Position;
            selected = GetRect(start, end);
            form.Close();
        };

        form.Paint += (s, e) =>
        {
            if (dragging)
                // DrawRectangle(...) qirmizi frame cekir ki, user ekranda
                // hansi saheni cekdiyini vizual olaraq gore bilsin:
                e.Graphics.DrawRectangle(Pens.Red, GetRect(start, Cursor.Position));
        };
        
        // formun event loopunu ise salir:
        Application.Run(form);

        return selected;
    }

    // User mouse ile ekranin bir noqtesinden digerine qeder sahe secende
    // 2ci noqte birinciden solda ve ya yuxarida ola biler. Bu method hemin
    // 2 noqteye esaslanaraq duzgun Rectangle qaytarir. 

    // 1. Duzbucaqlinin baslangic noqtesi hemise son yuxari kunc olur-->Math.Min() ile teyin olunur
    // 2. Width ve Height hemise musbet olur-->Math.Abs() ile hesablanir
    private static Rectangle GetRect(Point a, Point b)
    {
        return new Rectangle(
            Math.Min(a.X, b.X),
            Math.Min(a.Y, b.Y),
            Math.Abs(a.X - b.X),
            Math.Abs(a.Y - b.Y)
        );
    }

}