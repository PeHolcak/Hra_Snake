using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Shapes;

namespace snake
{
    /// <summary>
    /// Interakční logika pro nastaveni.xaml
    /// </summary>
    public partial class nastaveni : Window
    {
        public nastaveni()
        {
            InitializeComponent();
        }

        private void ulozitNastaveni_Click(object sender, RoutedEventArgs e)
        {
            //OpenFileDialog
            Configuration konfigurace = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            konfigurace.AppSettings.Settings["rychlost"].Value = rychlostHadaSlider.Value.ToString();
            ConfigurationManager.RefreshSection("appSettings");
            konfigurace.Save(ConfigurationSaveMode.Modified);
            this.Close();
        }
    }
}
