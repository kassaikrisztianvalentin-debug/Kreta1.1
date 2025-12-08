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
            string[] temp = File.ReadAllLines(Filepath);

            foreach (var item in temp)
            {
                string[] d = item.Split(';');
                string role = d[3];
                if (role == "Tanulo")
                {
                    string name = d[0];
                    string osztaly = d[1];
                    string password = d[2];
                    string username = d[4];
                    osztalyok.Add(osztaly);
                    tanuloList.Add(new Tanulo(username, password, name, osztaly));
                    userList.Add(new Tanulo(username, password, name, osztaly));
                }
                else if (role == "Tanar")
                {
                    string name = d[1];
                    string username = d[1].Trim().ToLower();
                    string password = d[0];
                    string tantargy = d[2];

                    tantagyak.Add(tantargy);
                    tanarList.Add(new Tanar(username, password, name, tantargy));
                    userList.Add(new Tanar(username, password, name, tantargy));
                }
                else if (role == "Admin")
                {
                    string name = d[1];
                    string username = d[1].Trim().ToLower();
                    string password = d[0];

                    adminList.Add(new Admin(username, password, name));
                    userList.Add(new Admin(username, password, name));
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
