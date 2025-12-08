using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreta1._0
{
    public class Jegy
    {
        public string Tantargy { get; set; }
        public int Ertek { get; set; }
        public DateTime Datum { get; set; }
        public string TanarNeve { get; set; }
        public string TanuloNeve { get; set; }

        public Jegy(string tantargy, int ertek, DateTime datum, string tanarNeve, string tanuloNeve)
        {
            this.Tantargy = tantargy;
            this.Ertek = ertek;
            this.Datum = datum;
            this.TanarNeve = tanarNeve;
            this.TanuloNeve = tanuloNeve;
        }
    }
    public class Into
    {
        public string TanarNeve { get; set; }
        public string TanuloNeve { get; set; }
        public DateTime Datum { get; set; }
        public string Szoveg { get; set; }
        public string Fokozat { get;set; }

        public Into(string tanarNeve, string tanuloNeve, DateTime datum, string szoveg, string fokozat)
        {
            TanarNeve = tanarNeve;
            TanuloNeve = tanuloNeve;
            Datum = datum;
            this.Szoveg = szoveg;
            Fokozat = fokozat;
        }
    }
}