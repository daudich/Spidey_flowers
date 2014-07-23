using System;
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

using System.Data.SqlClient;

namespace Spidey_flowers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                DatabaseConnection test = new DatabaseConnection();
                test.connectionString = Properties.Settings.Default.SflowersConnectionString;
                test.Sql = "SELECT * FROM customers;";
                Console.WriteLine(Properties.Settings.Default.SflowersConnectionString);
                test.connectDB();

                SqlDataReader reader = test.run();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("{0}\t{1}", reader.GetString(0),
                            reader.GetString(1));
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e + " What the hell?");
            }
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for the exit button on the main page.
        /// </summary>
        /// <param name="sender">The caller's object</param>
        /// <param name="e">Event arguements</param>
        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Leaving so soon?", "Exit", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
