﻿using System;
using System.Windows;

using System.Data.SqlClient;

namespace Spidey_flowers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected DatabaseConnection _dbConnect = null;

        /// <summary>
        /// Constructor for the MainWindow initiates a new DB connection then initialises the window. In case of failure, an error message is displayed
        /// before exiting.
        /// </summary>
        public MainWindow()
        {
            try
            {
                _dbConnect = new DatabaseConnection();
                _dbConnect.connectionString = Properties.Settings.Default.SflowersConnectionString;
                _dbConnect.connectDB();
            }
            catch
            {
                MessageBox.Show("DB Error: Could not open the database. Please check the logs for more information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                Application.Current.Shutdown();
            }

            InitializeComponent();
        }

        /// <summary>
        /// Event handler for the exit button on the main page. Trying to close the resource before exiting the system.
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
        /// Event handler for 'Add a new customer' button.
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

        /// <summary>
        /// Event handler for the 'Add a new order' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addNewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            AddOrder addOrderWindow = new AddOrder(_dbConnect);
            addOrderWindow.ShowDialog();

            this.Show();
        }

        /// <summary>
        /// This method is called before the window is closed. If the DB connection is open, it will be closed before exitting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                _dbConnect.closeDB();
            }
            catch (Exception excpt)
            {
                Console.WriteLine("Fatal Error: There was an error trying to close the DB connection : " + excpt);
            }
        }

        /// <summary>
        /// Event handler for the 'Track an order' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackOrderButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            TrackOrders trackOrderWindow = new TrackOrders(_dbConnect);
            trackOrderWindow.ShowDialog();

            this.Show();
        }

    }
}
