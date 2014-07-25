using System;
using System.Windows;
using System.Data.SqlClient;

namespace Spidey_flowers
{
    /// <summary>
    /// This form is used to gather information and add a new customer to Customers table.
    /// </summary>
    public partial class AddCustomer : Window
    {
        private DatabaseConnection _dbConnect = null;
        private string _customerID = null;
        private string _customerName = null;
        private string _customerAddress = null;
        private string _customerPhone = null;

        /// <summary>
        /// Constructor for add customer sets the DB connection to the active connection being passed in.
        /// </summary>
        /// <param name="dbConnect">The currently used DB connection owned by MainWindow</param>
        public AddCustomer(DatabaseConnection dbConnect)
        {
            _dbConnect = dbConnect;

            InitializeComponent();
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

        /// <summary>
        /// This is the active listener for the 'Add Customer' button. It first calls the checkInput function to check for the correct input entries.
        /// If everything is okay, then it call on the insertDBRecord funtion to commit the changes to the DB. It then closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkInput() == false)
            {
                return;
            }

            if (!insertDBRecord())
            {
                MessageBox.Show("Fatal Error: Could not add the new record to the database. Please check the logs for more information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.Close();

        }

        /// <summary>
        /// Inserting a new record in the Customers table. The query is parameterized for security purposes.
        /// </summary>
        /// <returns>True if the query ran successfully otherwise false.</returns>
        private bool insertDBRecord()
        {
            string query = "INSERT INTO Customers VALUES (@CUSTID, @CUSTNAME, @CUSTADDRESS, @CUSTPHONE);";

            SqlCommand command = new SqlCommand(query);

            command.Parameters.Add(new SqlParameter("CUSTID", _customerID));
            command.Parameters.Add(new SqlParameter("CUSTNAME", _customerName));
            command.Parameters.Add(new SqlParameter("CUSTADDRESS", _customerAddress));
            command.Parameters.Add(new SqlParameter("CUSTPHONE", _customerPhone));

            try
            {
                _dbConnect.Sql = command;

                SqlDataReader reader = _dbConnect.run();
                reader.Close(); //Need to close the reader as the DatabaseConnection.run method returns a reader.

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("DB Error :: Add Customer :: " + e);
                return false;
            }

        }

        /// <summary>
        /// This method checks if all the input fields are filled and then populates the correct variables.
        /// </summary>
        /// <returns>Returns true iff all input fields are correctly filled.</returns>
        private bool checkInput()
        {
            if (custIdBox.Text.Trim().Length == 0)// if customer id is empty
            {
                MessageBox.Show("Customer ID cannot be blank.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                _customerID = custIdBox.Text;
            }

            if (custNameBox.Text.Trim().Length == 0)// if customer name is empty
            {
                MessageBox.Show("Customer Name cannot be blank.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                _customerName = custNameBox.Text;
            }

            if (CustAddressBox.Text.Trim().Length == 0)// if customer address is empty
            {
                MessageBox.Show("Customer Address cannot be blank.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                _customerAddress = CustAddressBox.Text;
            }

            if (custPhoneBox.Text.Trim().Length == 0)// if customer phone is empty
            {
                MessageBox.Show("Customer Phone cannot be blank.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                _customerPhone = custPhoneBox.Text;
            }

            return true;
        }
    }
}
