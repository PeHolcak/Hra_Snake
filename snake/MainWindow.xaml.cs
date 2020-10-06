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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace snake
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
          
        }

        /// <summary>
        /// Spustí hrací okno
        /// </summary>
        private void Hra_Click(object sender, RoutedEventArgs e)
        {
            game hra = new game();
            hra.Show();
            this.Close();
        }

        /// <summary>
        /// Zobrazí okno s nastavením
        /// </summary>
        private void Nastaveni_Click(object sender, RoutedEventArgs e)
        {
            nastaveni sett = new nastaveni();
            sett.Show();
          
        }

        /// <summary>
        /// Ukončí hru
        /// </summary>
        private void Konec_Click(object sender, RoutedEventArgs e)
        {
        this.Close();

        }
    }
}
