using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Globalization;

namespace Kreta1._0
{
    internal class Menu
    {
        // simple in-memory records for timetable events (keeps changes local; add persistence later if needed)
        private class TimetableRecord
        {
            public string Osztaly { get; set; }
            public string DayOfWeek { get; set; }
            public int Hour { get; set; }
            public string Type { get; set; } // "Mulasztas" or "Elmaradas"
            public string Student { get; set; }
            public string Note { get; set; }
            public DateTime RecordedAt { get; set; }
        }

        private static readonly List<TimetableRecord> timetableRecords = new List<TimetableRecord>();

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
                int oraIndex = item.HourOfDay - 1;

                if (napIndex >= 0 && napIndex < days && oraIndex >= 0 && oraIndex < hours)
                {
                    orakMatrix[napIndex, oraIndex] = item;

                    var captured = item;
                    orakActionMatrix[napIndex, oraIndex] = () =>
                    {
                        ShowTimetableSlotOptions(captured);
                    };
                }
            }

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
            }

            int selectedDay = 0;
            int selectedHour = 0;

            if (FindFirstOccupied(out var fd, out var fh))
            {
                selectedDay = fd;
                selectedHour = fh;
            }

            bool running = true;
            while (running)
            {
                Console.Clear();

                for (int d = 0; d < days; d++)
                {
                    Console.Write($"{napokHu[d],-25}");
                }
                Console.WriteLine();
                Console.WriteLine(new string('-', 25 * days));

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
                            // show markers if there are records for this slot
                            int mCount = timetableRecords.FindAll(r => r.Osztaly == cell.Osztaly && r.DayOfWeek == cell.DayOfWeek && r.Hour == cell.HourOfDay && r.Type == "Mulasztas").Count;
                            int eCount = timetableRecords.FindAll(r => r.Osztaly == cell.Osztaly && r.DayOfWeek == cell.DayOfWeek && r.Hour == cell.HourOfDay && r.Type == "Elmaradas").Count;
                            string markers = "";
                            if (mCount > 0) markers += $" M:{mCount}";
                            if (eCount > 0) markers += $" E:{eCount}";

                            cellText = $"{cell.Subject}, {cell.Teacher}, R{cell.Terem}{markers}";
                        }

                        if (cellText.Length > 24) cellText = cellText.Substring(0, 24);
                        Console.Write($"{cellText,-25}");

                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();
                Console.WriteLine("Enter = részletek   ESC = vissza");

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

        private static void ShowTimetableSlotOptions(Timetable slot)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Óra részletei:\n");
                Console.WriteLine($"Osztály : {slot.Osztaly}");
                Console.WriteLine($"Nap     : {slot.DayOfWeek}");
                Console.WriteLine($"Óra     : {slot.HourOfDay}");
                Console.WriteLine($"Tantárgy: {slot.Subject}");
                Console.WriteLine($"Taná r  : {slot.Teacher}");
                Console.WriteLine($"Terem   : {slot.Terem}");
                Console.WriteLine();

                // show existing records for this slot
                var records = timetableRecords.FindAll(r => r.Osztaly == slot.Osztaly && r.DayOfWeek == slot.DayOfWeek && r.Hour == slot.HourOfDay);
                if (records.Count == 0)
                {
                    Console.WriteLine("Nincs rögzített mulasztás/elmaradás erre az órára.");
                }
                else
                {
                    Console.WriteLine("Rögzített bejegyzések:");
                    int i = 1;
                    foreach (var rec in records)
                    {
                        Console.WriteLine($"{i++}. [{rec.Type}] {rec.Student} - {rec.Note} ({rec.RecordedAt:g})");
                    }
                }

                Console.WriteLine("\n1) Mulasztás beírása");
                Console.WriteLine("2) Elmaradás beírása");
                Console.WriteLine("3) Vissza");

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
                {
                    AddTimetableRecord(slot, "Mulasztas");
                }
                else if (key == ConsoleKey.D2 || key == ConsoleKey.NumPad2)
                {
                    AddTimetableRecord(slot, "Elmaradas");
                }
                else if (key == ConsoleKey.D3 || key == ConsoleKey.NumPad3 || key == ConsoleKey.Escape)
                {
                    return;
                }
            }
        }

        private static void AddTimetableRecord(Timetable slot, string type)
        {
            Console.Clear();
            Console.WriteLine($"{(type == "Mulasztas" ? "Mulasztás" : "Elmaradás")} beírása");

            // validate student name exists in Authorization.tanuloList (ask until valid or cancel)
            string studentInput;
            while (true)
            {
                Console.Write("Tanuló neve (pontosan vagy felhasználónév, üres = vissza): ");
                studentInput = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(studentInput))
                {
                    Console.WriteLine("Mégse. Visszatérés...");
                    return;
                }

                string normInput = NormalizeName(studentInput);

                // match by normalized display name OR normalized username
                var match = Authorization.tanuloList.FirstOrDefault(t =>
                    string.Equals(NormalizeName(t.Name), normInput, StringComparison.Ordinal) ||
                    string.Equals(NormalizeName(t.Username), normInput, StringComparison.Ordinal));

                if (match != null)
                {
                    // use exact stored name for consistency
                    studentInput = match.Name;
                    break;
                }

                Console.WriteLine("Nincs ilyen tanuló. Kérlek próbáld újra (vagy üresen lépj vissza).");
            }

            Console.Write("Megjegyzés (opcionális): ");
            string note = Console.ReadLine();

            var rec = new TimetableRecord
            {
                Osztaly = slot.Osztaly,
                DayOfWeek = slot.DayOfWeek,
                Hour = slot.HourOfDay,
                Type = type,
                Student = studentInput,
                Note = note ?? "",
                RecordedAt = DateTime.Now
            };

            timetableRecords.Add(rec);

            // Also add an Into entry so the student sees the absence in their beírások list
            try
            {
                // Fokozat reuses the type text (Hungarian)
                string fokozat = type == "Mulasztas" ? "Mulasztás" : "Elmaradás";
                string szoveg = string.IsNullOrWhiteSpace(note) ? fokozat : note;

                // Use the teacher name from the timetable slot as TanarNeve
                Tanulo.Intok.Add(new Into(slot.Teacher, studentInput, DateTime.Now, szoveg, fokozat));

                // Persist the intok immediately so it is available after restart and for other views
                Save.mentes(Tanulo.Intok);
            }
            catch
            {
                // non-fatal: if Into constructor or list isn't available, we already keep timetableRecords
            }

            Console.WriteLine("\nSikeres rögzítés!");
            Console.WriteLine("Nyomj meg egy gombot a visszatéréshez...");
            Console.ReadKey(true);
        }

        // Normalize names: trim, lowercase, remove diacritics and collapse spaces
        private static string NormalizeName(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var trimmed = s.Trim().ToLowerInvariant();

            // remove diacritics
            var normalized = trimmed.Normalize(System.Text.NormalizationForm.FormKD);
            var sb = new StringBuilder();
            foreach (var ch in normalized)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }

            // collapse multiple spaces
            var collapsed = System.Text.RegularExpressions.Regex.Replace(sb.ToString(), @"\s+", " ").Trim();
            return collapsed;
        }
    }
}
