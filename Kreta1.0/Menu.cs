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
            int index = 0;
            int yndex = 0;
            string[] napokHu = new[] { "Hétfő", "Kedd", "Szerda", "Csütörtök", "Péntek" }; 
            string[] napokEng = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            List<Timetable> orak = new List<Timetable>();
            List<Action> orakAction = new List<Action>();
            Action[,] orakActionMatrix = new Action[5, 8];
            Timetable[,] orakMatrix = new Timetable[5, 8];
            foreach (var item in Timetable.timetable)
            {
                if (item.Osztaly == osztaly)
                {
                    int napIndex = Array.IndexOf(napokEng, item.DayOfWeek);
                    int oraIndex = item.HourOfDay - 1;
                    if (napIndex >= 0 && oraIndex >= 0 && oraIndex < 8)
                    {
                        orakMatrix[napIndex, oraIndex] = item;
                        orakActionMatrix[napIndex, oraIndex] = Tanar.;
                        orak.Add(item);
                    }
                }
            }
            Console.WriteLine(orak.Count);

            void Timetablekiirass()
            {
                foreach (var item in napokHu)
                {
                    Console.Write($"{item,-25}");
                }
                Console.WriteLine();
                for (int i = 0; i < 100; i++)
                {
                    Console.Write('-');
                }
                Console.WriteLine();

                for (int i = 0; i < orakMatrix.GetLength(1); i++)
                {
                    for (int j = 0; j < orakMatrix.GetLength(0); j++)
                    {
                        if (index == i)
                            Console.ForegroundColor = ConsoleColor.Blue;
                        else
                            Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($"{orakMatrix[j, i].Subject}, {orakMatrix[j, i].Teacher}:{orakMatrix[j, i].Terem, -20}");
                        Console.ForegroundColor = ConsoleColor.White;
                        if (orakMatrix[j, i].DayOfWeek == "Friday")
                        {
                            Console.WriteLine();
                        }
                    }
                }
                Console.ForegroundColor = ConsoleColor.White;

            }
        bool beker = true;
            while (beker)
            {
                Console.Clear();
                Timetablekiirass();

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        do
                        {
                            index--;
                        } while (orak[index] == null);
                        break;

                    case ConsoleKey.DownArrow:
                        do
                        {
                            index++;
                        } while (orak[index] == null);
                        break;
                        
                    case ConsoleKey.Enter:
                        if (orak[index] != null)
                        {
                            orakActionMatrix[index, yndex].Invoke();
                            beker = false;
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        yndex++;
                        if (yndex >= napokHu.Length) yndex = 0;
                        break;
                    case ConsoleKey.LeftArrow:
                        yndex--;
                        if (yndex < 0) yndex = napokHu.Length - 1;
                        break;
                    default:
                        continue;
                }
            }
        }
    }
}
