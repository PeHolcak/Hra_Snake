using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static snake.game;
using System.Configuration;
using System.Windows;

namespace snake
{
    class herniLogika
    {
        public event TimerHandler EventTimer;
        public event KonecHryHandler KonecHry;
        private System.Timers.Timer aTimer;
        //Čas hry
        static int cas;
        //Skore ve hře
        public int score { get; set; }
        Random rnd = new Random();

        //kolekce souřadnic
        public ArrayList had { get; set; }
        public ArrayList prekazky { get; set; }
        public ArrayList potrava { get; set; }
        public string obtiznost { get; set; }

        private int pocetSloupcu;
        private int pocetRadku;
        private bool muzeSePosunout;
        private string posledniPosun;
        private bool snedlZrni;
        private bool konecHry;
        private int rychlost;
        private int koeficient;


        /// <summary>
        /// Konstruktor s dvěma parametry, které udávají velkost pole.
        /// </summary>
        /// <param name="pocetSloupcu">Udává počet sloupců herního pole.</param>
        /// <param name="pocetRadku">Udává počet řádků herního pole.</param>
        /// <remarks>Nastavení velikosti pole zatím nebylo implementováno. Stejně tak je možné implementovat počet zrní.</remarks>
        public herniLogika(int pocetSloupcu, int pocetRadku)
        {

            rychlost = int.Parse(ConfigurationManager.AppSettings.Get("rychlost"));

            koeficient = UrciKoeficient();
            cas = 0;
            score = 0;
            this.pocetSloupcu = pocetSloupcu;
            this.pocetRadku = pocetRadku;
            snedlZrni = false;
            konecHry = false;
            muzeSePosunout = true;
            had = new ArrayList();

            //přidává souřadnice hada
            had.Add(new souradnice(10, 20));
            had.Add(new souradnice(10, 21));
            had.Add(new souradnice(10, 22));

            GenerujPrekazky();
            GenerujPotravu();
            ZjistiUroven();
        }

        /// <summary>
        /// Tato metoda udává rychlost hry
        /// </summary>
        /// <returns>Vrací počet milisekund (čas obnovy Timeru)</returns>
        private int UrciKoeficient()
        {
            switch (rychlost)
            {
                case 0:
                    return 1500;
                case 1:
                    return 1100;
                case 2:
                    return 700;
                case 3:
                    return 500;
                case 4:
                    return 350;
                case 5:
                    return 300;
                case 6:
                    return 250;
                case 7:
                    return 200;
                case 8:
                    return 150;
                case 9:
                    return 100;
                case 10:
                    return 50;
                default:
                    return 200;
            }
        }

        /// <summary>
        /// Spuštění Timeru
        /// </summary>
        private void SetTimer()
        {
            aTimer = new System.Timers.Timer(koeficient);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        /// <summary>
        /// Vypnutí Timeru
        /// </summary>
        private void StoppTimer()
        {
            aTimer.Dispose();
        }

        /// <summary>
        /// Udává co se stanu pokud Timer doběhne do konce.
        /// </summary>
        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {

            /*Ošetření, aby se had nemohl posunout o 2 políčka, při několikanásobném rychlém stisknutí šipky.
             * A současně zamezuje pohybu hada, pokud je konec hry.
             */
            if (muzeSePosunout && (!konecHry))
            {
          /* Zajišťuje, že při stisknutí šipky, had přímočaře cestuje 
          * (pokud by nebylo, tak by had poskočil pouze o jedno políčko) .
          * 
          */
                switch (posledniPosun)
                {
                    case "Up":
                        ZatocNahoru();
                        break;
                    case "Down":
                        ZatocDolu();
                        break;
                    case "Left":
                        ZatocDoLeva();
                        break;
                    case "Right":
                        ZatocDoPrava();
                        break;
                    default:
                        break;
                }
            }

            /* Pokud had sní zrní, tak poslední blok jeho ocasu zůstává na místě (tím se zvětší).
             * Pokud nic nesní, tak se poslední blok odebere (tím, že se had současně posune dopředu, zůstane jeho délka stejná).
             */
            if (snedlZrni)
            {
                score++;
                snedlZrni = false;
            }
            else
            {
                had.RemoveAt(had.Count - 1);
            }


            //Implementování časovače
            cas++;
            int sekundy = cas / (1000 / koeficient) ;
            string sekundyBezHod = (sekundy % 60).ToString();
            if (sekundyBezHod.Length == 1)
            {
                sekundyBezHod = "0" + sekundyBezHod;
            }
            EventTimer((sekundy / 60).ToString() + ":" + sekundyBezHod);

            //Nyní bude had znovu reagovat, pokud hráč stiskne šipku
            muzeSePosunout = true;
        }

        /// <summary>
        /// Pokud Časovač neběží, tak zavolá metodu, která ho spustí.
        /// </summary>
        public void CheckTimer()
        {
            if (aTimer is null)
            {
                SetTimer();
            }
        }

        /// <summary>
        /// Zjistí, zda had při zatočení doprava nenarazí na konec herní plochy (pokud ano, tak ukončí hru).
        /// Zavola metodu - "OverStav", která zjistí, zda had nenarazil na překážku, sám sebe nebo potravu.
        /// Přidá do kolekce "had" nový první blok,
        /// nastaví poslední posun,
        /// a nastaví proměnou "muzeSePosunout" na false.
        /// Pokud hra neběží, tak zavolá metodu "CheckTimer", ktreá spustí časovač 
        /// </summary>
        public void ZatocDoPrava()
        {
            if (muzeSePosunout && posledniPosun != "Left")
            {
                souradnice hlavaHada = (souradnice)had[0];
                if (hlavaHada.sloupce != (pocetSloupcu - 1))
                {
                    had.Insert(0, new souradnice(hlavaHada.sloupce + 1, hlavaHada.radky));
                    posledniPosun = "Right";
                    muzeSePosunout = false;
                    CheckTimer();
                    OverStav();
                }
                else
                {
                    StoppTimer();
                    konecHry = true;
                    KonecHry("Narazil jsi na okraj.");
                }
            }
        }

        /// <summary>
        /// Zjistí, zda had při zatočení nahoru nenarazí na konec herní plochy (pokud ano, tak ukončí hru).
        /// Zavola metodu - "OverStav", která zjistí, zda had nenarazil na překážku, sám sebe nebo potravu.
        /// Přidá do kolekce "had" nový první blok,
        /// nastaví poslední posun,
        /// a nastaví proměnou "muzeSePosunout" na false.
        /// Pokud hra neběží, tak zavolá metodu "CheckTimer", ktreá spustí časovač 
        /// </summary>
        public void ZatocNahoru()
        {
            if (muzeSePosunout && posledniPosun != "Down")
            {
                souradnice hlavaHada = (souradnice)had[0];
                if (hlavaHada.radky != 0)
                {
                    had.Insert(0, new souradnice(hlavaHada.sloupce, hlavaHada.radky - 1));
                    posledniPosun = "Up";
                    muzeSePosunout = false;
                    OverStav();
                    CheckTimer();
                }
                else
                {
                    StoppTimer();
                    konecHry = true;
                    KonecHry("Narazil jsi na okraj.");
                }
            }
        }


        /// <summary>
        /// Zjistí, zda had při zatočení dolu nenarazí na konec herní plochy (pokud ano, tak ukončí hru).
        /// Zavola metodu - "OverStav", která zjistí, zda had nenarazil na překážku, sám sebe nebo potravu.
        /// Přidá do kolekce "had" nový první blok,
        /// nastaví poslední posun,
        /// a nastaví proměnou "muzeSePosunout" na false.
        /// Pokud hra neběží, tak zavolá metodu "CheckTimer", ktreá spustí časovač 
        /// </summary>
        public void ZatocDolu()
        {
            if (muzeSePosunout && posledniPosun != "Up")
            {
                souradnice hlavaHada = (souradnice)had[0];
                if (hlavaHada.radky != (pocetRadku - 1))
                {
                    had.Insert(0, new souradnice(hlavaHada.sloupce, hlavaHada.radky + 1));
                    posledniPosun = "Down";
                    muzeSePosunout = false;
                    CheckTimer();
                    OverStav();
                }
                else
                {
                    StoppTimer();
                    konecHry = true;
                    KonecHry("Narazil jsi na okraj.");
                }
            }
        }


        /// <summary>
        /// Zjistí, zda had při zatočení doleva nenarazí na konec herní plochy (pokud ano, tak ukončí hru).
        /// Zavola metodu - "OverStav", která zjistí, zda had nenarazil na překážku, sám sebe nebo potravu.
        /// Přidá do kolekce "had" nový první blok,
        /// nastaví poslední posun,
        /// a nastaví proměnou "muzeSePosunout" na false.
        /// Pokud hra neběží, tak zavolá metodu "CheckTimer", ktreá spustí časovač 
        /// </summary>
        public void ZatocDoLeva()
        {
            if (muzeSePosunout && posledniPosun != "Right")
            {
                souradnice hlavaHada = (souradnice)had[0];
                if (hlavaHada.sloupce != 0)
                {
                    had.Insert(0, new souradnice(hlavaHada.sloupce - 1, hlavaHada.radky));
                    posledniPosun = "Left";
                    muzeSePosunout = false;
                    CheckTimer();
                    OverStav();
                }
                else
                {
                    StoppTimer();
                    konecHry = true;
                    KonecHry("Narazil jsi na okraj.");
                }
            }
        }


        /// <summary>
        /// Zjistí, zda had nenarazil na zrní, sám sebe nebo překážku.
        /// </summary>
        public void OverStav()
        {
            //deklaruje novou proměnou a inicializuje ji na souřadnici prvního bloku hada.
            souradnice hlavaHada = (souradnice)had[0];

            //Zjistí, zda had nenarazil na zrní.
            foreach (souradnice zrni in potrava)
            {
                if (hlavaHada.radky == zrni.radky && hlavaHada.sloupce == zrni.sloupce)
                {
                    snedlZrni = true;
                    potrava = null;
                    GenerujPotravu();
                }
            }

            //Zjistí, zda had nenarazil na překážku.
            foreach (souradnice prekazka in prekazky)
            {
                if (hlavaHada.radky == prekazka.radky && hlavaHada.sloupce == prekazka.sloupce)
                {
                    StoppTimer();
                    konecHry = true;
                    KonecHry("Narazil jsi na překážku.");
                }
            }

            //Zjistí, zda had nenarazil na sám sebe.
            for (int i = 0; (i < (had.Count - 1)); i++)
            {
                if (i == 0)
                {
                    continue;
                }
                else
                {
                    if (hlavaHada.radky == ((souradnice)had[i]).radky && hlavaHada.sloupce == ((souradnice)had[i]).sloupce)
                    {
                        StoppTimer();
                        konecHry = true;
                        KonecHry("Snědl jsi sám sebe.");
                    }
                }
            }
        }

        /// <summary>
        /// Tato metoda vygeneruje souřadnice potravy.
        /// </summary>
        public void GenerujPotravu()
        {
            //Smazání původních souřadnic potravy
            potrava = null;
            potrava = new ArrayList();

            //Generování nových souřadnic
            int sloupec = rnd.Next(pocetSloupcu);
            int radek = rnd.Next(pocetRadku);
            souradnice moznaSouradnice = new souradnice(sloupec, radek);

            /*zavolá metodu je volné, která zjistí, zda je daná souřadnice obsazená překážkou. Pokud ne, tak souřadnici přidá do kolekce potravy.
             * Pokud ano, pokusí se překážku posunou. V případě, že stále kryje překážku, tak zavolá metoda rekurzivním voláním sama sebe a proces opakuje.
             */
            if (JeVolne(moznaSouradnice))
            {
                potrava.Add(moznaSouradnice);
            }
            else
            {
                moznaSouradnice = posunSouradnice(moznaSouradnice);
                if (JeVolne(moznaSouradnice))
                {
                    potrava.Add(moznaSouradnice);
                }
                else
                {
                    GenerujPotravu();
                }

            }


        }

        /// <summary>
        /// Zjistí, zda není souřadnice v parametru zabraná nějakou překážkou (lze rozšířit i na hada).
        /// </summary>
        /// <param name="s">Souřadnice u které se ověřuje dostupnost.</param>
        /// <returns></returns>
        public bool JeVolne(souradnice s)
        {
            foreach (souradnice prekazka in prekazky)
            {
                if (prekazka.sloupce == s.sloupce)
                {
                    if (prekazka.radky == s.radky)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>
        /// Generuje překážky a ty následně ukládá do kolekce "prekazky".
        /// </summary>
        public void GenerujPrekazky()
        {
            prekazky = new ArrayList();

            //Každá hra má má náhodný počet překážek
            //Ošetřuje, aby se překážky do hry vešly
            int pocetPrekazek = (rnd.Next((int)Math.Sqrt(Convert.ToDouble(pocetSloupcu))) + 1) * 2;

            //každá překážka má o jeden blok více než je počet překážek.
            int pocetBloku = pocetPrekazek + 1;
            int sloupce;
            int radky;

            //generuje první blok překážky a následně i další bloky pomocí posouvání ("metoda posun souřadnic").
            for (int i = 0; i < pocetPrekazek; i++)
            {
                sloupce = rnd.Next(pocetSloupcu);
                radky = rnd.Next(pocetRadku);
                prekazky.Add(new souradnice(sloupce, radky));
                int j = 0;
                while (j < pocetBloku)
                {
                    souradnice s = posunSouradnice(new souradnice(sloupce, radky));
                    sloupce = s.sloupce;
                    radky = s.radky;
                    prekazky.Add(new souradnice(sloupce, radky));
                    j++;
                }
            }
        }

        /// <summary>
        /// Generuje souřadnice,které se nacházejí vedle souřadnice v parametru.
        /// </summary>
        /// <param name="s">Souradnice, která má být posunuta.</param>
        /// <returns></returns>
        public souradnice posunSouradnice(souradnice s)
        {
            int sloupce = s.sloupce;
            int radky = s.radky;
            souradnice vysledek = null;

            //Generuje náhodné číslo, které určuje na kterou stranu se má souřadnice posunout.
            switch (rnd.Next(3))
            {
                case 0:
                    if (sloupce != 0)
                    {
                        sloupce--;
                        vysledek = new souradnice(sloupce, radky);
                    }
                    break;
                case 1:
                    if (sloupce < pocetSloupcu)
                    {
                        sloupce++;
                        vysledek = new souradnice(sloupce, radky);
                    }
                    break;
                case 2:
                    if (radky != 0)
                    {
                        radky--;
                        vysledek = new souradnice(sloupce, radky);
                    }
                    break;
                default:
                    if (radky < pocetRadku)
                    {
                        radky++;
                        vysledek = new souradnice(sloupce, radky);
                    }
                    break;
            }

            /*Pokud není žádný výsledek, znamená to že posunutá souřadnice, by zasahovala za hranice hracího pole.
             * Proto musí být vygenerována nová souřadnice.
             * Pokud souřadnice není mimo pole, tak vrátí danou souřadnici.
             */
            if (vysledek is null)
            {
                return posunSouradnice(s);
            }
            else
            {
                return vysledek;
            }
        }

        /// <summary>
        /// Podle počtu bloků ve hře, vypíše složitost hry.
        /// </summary>
        public void ZjistiUroven()
        {
            switch (prekazky.Count)
            {
                case int n when (n < 20):
                    obtiznost = "velmi lehká";
                    break;
                case int n when (n >= 20 && n < 40):
                    obtiznost = "lehká";
                    break;
                case int n when (n >= 40 && n < 100):
                    obtiznost = "střední";
                    break;
                case int n when (n >= 100):
                    obtiznost = "těžká";
                    break;
            }
        }
    }
}
