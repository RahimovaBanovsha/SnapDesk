using System;
using System.Linq;

namespace SnapDesk.Utilities;
public static class ConsoleHelper
{
    // Execute() her zaman false qaytarir ki, exit deyiseni true olmasin ve
    // proqram davam etsin. Yeni gonderilen funksiyani icra edir ve false 
    // qaytarir ki, switch blokunda exit yalniz "0" secilende olsun:
    public static bool Execute(Action action)
    {
        action();
        return false;

    }

    public static bool ShowInvalid()
    {

        Console.WriteLine("Invalid option. Please try again.");
        return false;

    }

    // Region koordinatlarini daxil etmek ucun istifade olunur:
    // Runner -> AskCoords methodu daxilinde: 
    public static int PromptInt(string message)
    {

        int value;
        Console.Write(message);

        while(!int.TryParse(Console.ReadLine(),out value) || value < 0)
        {

            Console.Write("Invalid input. Try again: ");

        }
        return value;

    }

    // Menuda secim ucun istifade olunur (Runner -> switch daxilinde):
    public static string PromptOption(string message, string[] validOptions)
    {

        Console.Write(message);
        string? input = Console.ReadLine();

        while (!validOptions.Contains(input))
        {

            Console.Write("Invalid option. Try again: ");
            input = Console.ReadLine();

        }
        return input!;
    }

}