using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreta1._0
{
    internal class Timetable
    {
        public string Osztaly { get; set; }
        public string DayOfWeek { get; set; }
        public string Subject { get; set; }
        public int Terem { get; set; }
        public int HourOfDay { get; set; }
        public string Teacher { get; set; }
        public Timetable(string osztaly, string dayOfWeek, string subject, int terem, int hourofday, string teacher)
        {
            Osztaly = osztaly;
            DayOfWeek = dayOfWeek;
            Subject = subject;
            Terem = terem;
            HourOfDay = hourofday;
            Teacher = teacher;
        }

        public static List<Timetable> timetable = new List<Timetable>();
        public static void CreateTimetable()
        {
            Random rnd = new Random();
            foreach (var osztaly in Authorization.osztalyok)
            {
                foreach (var day in new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" })
                {
                    for (int hour = 1; hour <= 8; hour++)
                    {
                        int terem = 100;
                        terem++;
                        Tanar randomTeacher = Authorization.tanarList[rnd.Next(0, Authorization.tanarList.Count)];
                        string subject = randomTeacher.tantargy;
                        string teacher = randomTeacher.Name;
                        while (timetable.Any(x => x.DayOfWeek == day && x.HourOfDay == hour && (x.Teacher == teacher || x.Terem == terem)))
                        {
                            terem++;
                            randomTeacher = Authorization.tanarList[rnd.Next(0, Authorization.tanarList.Count)];
                            subject = randomTeacher.tantargy;
                            teacher = randomTeacher.Name;
                        }
                        timetable.Add(new Timetable(osztaly, day, subject, terem, hour, teacher));
                    }
                }
            }
        }
    }
}
