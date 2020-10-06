using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace snake
{
    /// <summary>
    /// Interakční logika pro game.xaml
    /// </summary>
    public partial class game : Window
    {
        herniLogika hl;
        public delegate void TimerHandler(string s);
        public delegate void KonecHryHandler(string s);
        ArrayList had;
        ArrayList hadReferences;
        ArrayList prekazky;
        ArrayList potrava;
        internal int radky;
        internal int sloupce;
        public game()
        {
            InitializeComponent();

            //nastaceví velikosti hry
            radky = 30;
            sloupce = 30;

            //inicializace kolekcí
            had = new ArrayList();
            prekazky = new ArrayList();
            potrava = new ArrayList();
            hadReferences = new ArrayList();




            //Generování prázdného hracího pole
            pole.Background = Brushes.Black;
            for (int i = 0; i < sloupce; i++)
            {
                GridLength height = new GridLength(1, GridUnitType.Star);
                pole.RowDefinitions.Add(new RowDefinition()
                {
                    Height = height
                });
            }

            for (int i = 0; i < radky; i++)
            {
                GridLength width = new GridLength(1, GridUnitType.Star);
                pole.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = width
                });
            }
            //vytvoření instance herní logiky
            hl = new herniLogika(sloupce, radky);

            //získání dat
            prekazky = (ArrayList)hl.prekazky.Clone();
            potrava = (ArrayList)hl.potrava.Clone();
            had = (ArrayList)hl.had.Clone();
            hl.EventTimer += new TimerHandler(prekresly);
            hl.KonecHry += new KonecHryHandler(UkoncitHru);
            Zobraz();


            //Zobrazení informačního informačního panelu

            nadpis.Text = "Vítejte ve hře";
            this.zprava.Text = "Hru začnete stisknutím šipky. Pokud hra nemá řešení, nebo již nechcete pokračovat ukončete hru stisknutím tlačítka zpět";
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);

            //vypíše obtížnost
            obtiznost.Text = hl.obtiznost;

        }

        /// <summary>
        /// Tuto metodu spustí event, pokud dojde k ukončení hry.
        /// </summary>
        /// <param name="zprava">Text obsahují důvod ukončení.</param>
        public void UkoncitHru(string zprava)
        {
            Dispatcher.Invoke((Action)(() => nadpis.Text = "Konec hry"));
            Dispatcher.Invoke((Action)(() => this.zprava.Text = zprava));
            Dispatcher.Invoke((Action)(() => hratZnovu.Visibility = Visibility.Visible));
        }

        /// <summary>
        /// Zaznamenává stisknuté klávesy(šipky) a následně spouští metody v instanci "hernilogika"
        /// </summary>
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            string stisknutaKlavesa = e.Key.ToString();
            switch (stisknutaKlavesa)
            {
                case "Up":
                    hl.ZatocNahoru();
                    break;
                case "Down":
                    hl.ZatocDolu();
                    break;
                case "Left":
                    hl.ZatocDoLeva();
                    break;
                case "Right":
                    hl.ZatocDoPrava();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Tato metoda se spustí, pokud se sepne časevač herni logiky.
        /// Vykreslí každý snímek hry.
        /// </summary>
        /// <param name="s"> Informace o tom, jak dlouho hra trvá.</param>
        public void prekresly(string s)
        {
            Dispatcher.Invoke((Action)(() => cas.Text = s));
            Dispatcher.Invoke((Action)(() => score.Text = "Score: " + hl.score.ToString()));
            Dispatcher.Invoke((Action)(() => had = null));
            Dispatcher.Invoke((Action)(() => had = (ArrayList)hl.had.Clone()));
            Dispatcher.Invoke((Action)(() => potrava = null));
            Dispatcher.Invoke((Action)(() => potrava = (ArrayList)hl.potrava.Clone()));
            Dispatcher.Invoke((Action)(() => Zobraz()));
        }


        /// <summary>
        /// Zobrazuje jednotlivé čtverce v herním poli
        /// </summary>
        public void Zobraz()
        {
            pole.Children.Clear();

            foreach (souradnice s in prekazky)
            {
                VytvorCtverec(1, s.radky, s.sloupce);
            }

            foreach (souradnice s in potrava)
            {
                VytvorCtverec(0, s.radky, s.sloupce);
            }
            bool prvni = true;



            foreach (souradnice s in had)
            {
                if (prvni)
                {
                    VytvorCtverec(3, s.radky, s.sloupce);
                    prvni = false;
                }
                else
                {
                    VytvorCtverec(2, s.radky, s.sloupce);
                }
            }
        }


        /// <summary>
        /// Vytvoří čtverec v herním poli.
        /// </summary>
        /// <param name="typ">Určuje zda se jedná o překážku, potravu, nebo část hada</param>
        /// <param name="radek">Souřadnice y</param>
        /// <param name="sloupec">Souřadnice X</param>
        public void VytvorCtverec(short typ, int radek, int sloupec)
        {
            Rectangle r = new Rectangle();
            r.Width = 30;
            r.Height = 30;
            switch (typ)
            {
                case 0:
                    r.Fill = new SolidColorBrush(System.Windows.Media.Colors.Blue);
                    hadReferences.Add(r);
                    break;
                case 1:
                    hadReferences.Add(r);
                    r.Fill = new SolidColorBrush(System.Windows.Media.Colors.Red);
                    break;
                case 3:
                    r.Fill = new SolidColorBrush(System.Windows.Media.Colors.Green);
                    break;
                default:
                    r.Fill = new SolidColorBrush(System.Windows.Media.Colors.AliceBlue);
                    break;
            }
            Grid.SetRow(r, radek);
            Grid.SetColumn(r, sloupec);
            pole.Children.Add(r);
        }

        /// <summary>
        /// Vrátí uživatele zpět do menu.
        /// </summary>
        private void zpet(object sender, RoutedEventArgs e)
        {
            MainWindow menu = new MainWindow();
            menu.Show();
            this.Close();

        }

        /// <summary>
        /// ZAčnw novou hru.
        /// </summary>
        private void NovaHra_Cllick(object sender, RoutedEventArgs e)
        {
            game novaHra = new game();
            novaHra.Show();
            this.Close();
        }
    }
}
