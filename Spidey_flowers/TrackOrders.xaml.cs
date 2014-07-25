using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Spidey_flowers
{
    /// <summary>
    /// This form is used to track an order provided the ID.
    /// </summary>
    public partial class TrackOrders : Window
    {
        private DatabaseConnection _dbConnect = null;
        private string _orderID = null;

        /// <summary>
        /// Constructor for tracking orders sets the DB connection to the active connection being passed in.
        /// It then calls the populateOrdersID function to populate the orders id dropdown.
        /// </summary>
        /// <param name="dbConnect">The currently used DB connection owned by MainWindow</param>
        public TrackOrders(DatabaseConnection dbConnect)
        {
            _dbConnect = dbConnect;

            InitializeComponent();

            try
            {
                populateOrderID();
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not retrieve Order IDs from the databse, please see the logs for more info.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine("Track Order :: " + e);
                this.Close();
            }
        }

        /// <summary>
        /// This method queries the Orders table for all the order ids and then populates the 'order id' dropdown.
        /// </summary>
        private void populateOrderID()
        {
            string query = "SELECT OrderID FROM Orders;";

            SqlCommand command = new SqlCommand(query);

            try
            {
                _dbConnect.Sql = command;

                SqlDataReader reader = _dbConnect.run();

                DataTable orders = new DataTable();
                orders.Columns.Add("Order ID", typeof(string));
                orders.Load(reader);

                orderIdBox.SelectedValuePath = "OrderID";
                orderIdBox.DisplayMemberPath = "OrderID";
                orderIdBox.ItemsSource = orders.DefaultView;

                reader.Close(); //Need to close the reader as the DatabaseConnection.run method returns a reader.

            }
            catch
            {
                throw;
            }

        }

        /// <summary>
        /// This is the active listener for the 'Track Order' button. It first checks if the input is valid.
        /// If everything is okay, then it proceeds to run a search query accross the databse and retrieve all the records and set the appropriate fields
        /// in the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackButton_Click(object sender, RoutedEventArgs e)
        {
            try //if order id is empty
            {
                _orderID = orderIdBox.SelectedValue.ToString();
            }
            catch
            {
                MessageBox.Show("Order ID cannot be blank.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string query = "SELECT Orders.CustomerID, OrderDate, OrderQuantity, CustomerNote, CompanyName, CompanyAddress, Phone FROM Orders, Customers WHERE OrderID = " + _orderID + "AND Orders.CustomerID = Customers.CustomerID;";

            SqlCommand command = new SqlCommand(query);

            try
            {
                _dbConnect.Sql = command;

                SqlDataReader reader = _dbConnect.run();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        custNameBox.Text = reader.GetString(4);
                        custAddressBox.Text = reader.GetString(5);
                        custPhoneBox.Text = reader.GetString(6);
                        dateBox.Text = reader.GetDateTime(1).ToShortDateString();
                        qtyBox.Text = reader.GetInt32(2).ToString();
                        noteBox.Text = reader.GetString(3);
                    }
                }
                else
                {
                    Console.WriteLine("FATAL ERROR ::  Track Orders :: No rows found.");
                }

                reader.Close(); //Need to close the reader as the DatabaseConnection.run method returns a reader.

            }
            catch (SqlException excpt)
            {
                Console.WriteLine("Track Order :: SQL Error :: " + excpt);
            }
        }

        /// <summary>
        /// This method returns control back to the MainWindow if the cancel button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
