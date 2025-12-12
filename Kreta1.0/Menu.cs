using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kreta1._0
{
    internal class Menu
    {
        public static void menu(User current, List<string> menutext, List<Action> parancs, int hossz)
        {
            int index = 0;
            List<string> menuT = menutext;
            List<Action> menuP = parancs;

            void menukiiras()
            {
                Console.WriteLine($"\tÜdv {current.Name}!\n");
                for (int i = 0; i < menuT.Count; i++)
                {
                    if (index == i)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("\t[X]" + menuT[i]);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("\t[ ]" + menuT[i]);
                    }

                }
                Console.ForegroundColor = ConsoleColor.White;
            }

            bool beker = true;
            while (beker)
            {
                Console.Clear();
                menukiiras();

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        do
                        {
                            index--;
                            if (index < 0) index = hossz - 1;
                        } while(menuP[index] == null);
                        break;

                    case ConsoleKey.DownArrow:
                        do
                        {
                           index++;
                            if (index >= hossz) index = 0;
                        } while (menuP[index] == null);
                        break;

                    case ConsoleKey.Enter:
                        if (menuP[index] != null)
                        {
                            menuP[index].Invoke();
                            beker = false;
                        }
                        break;
                    default:
                        continue;
                }
            }
        }

        public static void TimetableMenu(string osztaly)
        {
            string[] napokHu = new[] { "Hétfő", "Kedd", "Szerda", "Csütörtök", "Péntek" };
            string[] napokEng = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

            const int days = 5;
            const int hours = 8;

            Timetable[,] orakMatrix = new Timetable[days, hours];
            Action[,] orakActionMatrix = new Action[days, hours];

            foreach (var item in Timetable.timetable)
            {
                if (item == null) continue;
                if (item.Osztaly != osztaly) continue;

                int napIndex = Array.IndexOf(napokEng, item.DayOfWeek);
                int oraIndex = item.HourOfDay - 1; // HourOfDay is 1-based in the model

                if (napIndex >= 0 && napIndex < days && oraIndex >= 0 && oraIndex < hours)
                {
                    // store timetable entry
                    orakMatrix[napIndex, oraIndex] = item;

                    // create a simple action that shows details for this cell
                    // capture the item in a local variable to avoid closure issues
                    var captured = item;
                    orakActionMatrix[napIndex, oraIndex] = () =>
                    {
                        Console.Clear();
                        Console.WriteLine("Óra részletei:\n");
                        Console.WriteLine($"Osztály : {captured.Osztaly}");
                        Console.WriteLine($"Nap     : {captured.DayOfWeek}");
                        Console.WriteLine($"Óra     : {captured.HourOfDay}");
                        Console.WriteLine($"Tantárgy: {captured.Subject}");
                        Console.WriteLine($"Taná r  : {captured.Teacher}");
                        Console.WriteLine($"Terem   : {captured.Terem}");
                        Console.WriteLine("\nNyomj meg egy gombot a visszatéréshez...");
                        Console.ReadKey(true);
                    };
                }
            }

            // Helper: find first occupied cell (returns true if found)
            bool FindFirstOccupied(out int day, out int hour)
            {
                for (int h = 0; h < hours; h++)
                {
                    for (int d = 0; d < days; d++)
                    {
                        if (orakMatrix[d, h] != null)
                        {
                            day = d;
                            hour = h;
                            return true;
                        }
                    }
                }
                day = 0;
                hour = 0;
                return false;
            }

            // Helper: move to next occupied cell given delta (wraps) — does not change selection if none found
            void MoveToNextOccupied(ref int curDay, ref int curHour, int deltaDay, int deltaHour)
            {
                int total = days * hours;
                int d = curDay;
                int h = curHour;
                for (int i = 0; i < total; i++)
                {
                    d = (d + deltaDay + days) % days;
                    h = (h + deltaHour + hours) % hours;
                    if (orakMatrix[d, h] != null)
                    {
                        curDay = d;
                        curHour = h;
                        return;
                    }
                }
                // leave curDay/curHour unchanged if no occupied cell exists
            }

            int selectedDay = 0;   // 0..4
            int selectedHour = 0;  // 0..7

            // Start on first occupied cell if any
            if (FindFirstOccupied(out var fd, out var fh))
            {
                selectedDay = fd;
                selectedHour = fh;
            }

            bool running = true;
            while (running)
            {
                Console.Clear();

                // Header: day names
                for (int d = 0; d < days; d++)
                {
                    Console.Write($"{napokHu[d],-25}");
                }
                Console.WriteLine();
                Console.WriteLine(new string('-', 25 * days));

                // Rows: hours
                for (int h = 0; h < hours; h++)
                {
                    for (int d = 0; d < days; d++)
                    {
                        var cell = orakMatrix[d, h];
                        bool isSelected = (d == selectedDay && h == selectedHour);

                        Console.ForegroundColor = isSelected ? ConsoleColor.Blue : ConsoleColor.White;

                        string cellText;
                        if (cell == null)
                        {
                            cellText = "(üres)";
                        }
                        else
                        {
                            // Build compact text: Subject, Teacher, Room
                            cellText = $"{cell.Subject}, {cell.Teacher}, R{cell.Terem}";
                        }

                        // Ensure fixed column width
                        if (cellText.Length > 24) cellText = cellText.Substring(0, 24);
                        Console.Write($"{cellText,-25}");

                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine();
                }

                // Controls hint
                Console.WriteLine();
                Console.WriteLine("Navigáció: ← → ↑ ↓    Enter = részletek    Esc = vissza");

                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.RightArrow:
                        MoveToNextOccupied(ref selectedDay, ref selectedHour, 1, 0);
                        break;
                    case ConsoleKey.LeftArrow:
                        MoveToNextOccupied(ref selectedDay, ref selectedHour, -1, 0);
                        break;
                    case ConsoleKey.DownArrow:
                        MoveToNextOccupied(ref selectedDay, ref selectedHour, 0, 1);
                        break;
                    case ConsoleKey.UpArrow:
                        MoveToNextOccupied(ref selectedDay, ref selectedHour, 0, -1);
                        break;
                    case ConsoleKey.Enter:
                        var action = orakActionMatrix[selectedDay, selectedHour];
                        if (action != null)
                        {
                            action.Invoke();
                        }
                        else
                        {
                            // no action for empty cell — show a brief message
                            Console.Clear();
                            Console.WriteLine("Ez az időpont üres.\nNyomj meg egy gombot a visszatéréshez...");
                            Console.ReadKey(true);
                        }
                        break;
                    case ConsoleKey.Escape:
                        running = false;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
