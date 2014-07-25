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
        protected DatabaseConnection _dbConnect = null;

        public MainWindow()
        {
            try
            {
                _dbConnect = new DatabaseConnection();
                _dbConnect.connectionString = Properties.Settings.Default.SflowersConnectionString;
                _dbConnect.connectDB();

                // REMOVE THIS BEFORE RELEASE
                _dbConnect.Sql = new SqlCommand("SELECT * FROM Customers;");

                SqlDataReader reader = _dbConnect.run();

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
                Console.WriteLine("DB Error in Main Window :: " + e); // REMOVE THIS BEFORE RELEASE
                MessageBox.Show("DB Error: Could not open the database. Please check the logs for more information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Application.Current.Shutdown();
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
            if (MessageBox.Show("Leaving so soon?", "Exit", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    _dbConnect.closeDB();
                }
                catch (Exception excpt)
                {
                    Console.WriteLine("Fatal Error: There was an error trying to close the DB connection : " + excpt);
                }

                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Event handler for adding a new customer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addNewCustButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            AddCustomer addCustomerWindow = new AddCustomer(_dbConnect);
            addCustomerWindow.ShowDialog();

            this.Show();
        }
    }
}
