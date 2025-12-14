using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kreta1._0
{
    internal class Authorization
    {

        public static List<User> userList = new List<User>();
        public static HashSet<string> osztalyok = new HashSet<string>();
        public static List<Tanulo> tanuloList = new List<Tanulo>();

        public static HashSet<string> tantagyak = new HashSet<string>();

        public static List<Tanar> tanarList = new List<Tanar>();
        public static List<Admin> adminList = new List<Admin>();
        public static void fileRead(string Filepath)
        {
            if (!File.Exists(Filepath)) return;

            string[] temp = File.ReadAllLines(Filepath);

            foreach (var item in temp)
            {
                if (string.IsNullOrWhiteSpace(item)) continue;
                string[] d = item.Split(';');
                if (d.Length < 4) continue;

                string role = d[3].Trim();
                switch (role.ToLowerInvariant())
                {
                    case "tanulo":
                    case "tanuló":
                        {
                            string name = d[0];
                            string osztaly = d[1];
                            string password = d[2];
                            string username = d.Length > 4 ? d[4] : name.Trim().ToLower();
                            osztalyok.Add(osztaly);
                            var t = new Tanulo(username, password, name, osztaly);
                            tanuloList.Add(t);
                            userList.Add(t);
                        }
                        break;

                    case "tanar":
                    case "tanár":
                        {
                            string name = d[1];
                            string username = d[1].Trim().ToLower();
                            string password = d[0];
                            string tantargy = d.Length > 2 ? d[2] : "";
                            tantagyak.Add(tantargy);
                            var t = new Tanar(username, password, name, tantargy);
                            tanarList.Add(t);
                            userList.Add(t);
                        }
                        break;

                    case "admin":
                        {
                            string name = d[1];
                            string username = d[1].Trim().ToLower();
                            string password = d[0];
                            var a = new Admin(username, password, name);
                            adminList.Add(a);
                            userList.Add(a);
                        }
                        break;

                    default:
                        continue;
                }
            }
        }

        public static User LogIn()
        {
            Console.Clear();

            Console.Write("Felhasználónév: ");
            string fnev = Console.ReadLine();
            Console.Write("Jelszó: ");
            string jelszo = Console.ReadLine();

            foreach (var item in userList)
            {
                if (item.Username == fnev && item.Password == jelszo)
                {
                    Console.WriteLine("Sikeres bejelentkezés!");
                    return item;
                }
            }

            Console.WriteLine("Sikertelen bejelentkezés!");
            Thread.Sleep(1000);
            return null;
        }
    }
}
