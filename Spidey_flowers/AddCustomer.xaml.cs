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
using System.Windows.Shapes;
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

        public AddCustomer(DatabaseConnection dbConnect)
        {
            _dbConnect = dbConnect;

            InitializeComponent();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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
        /// Inserting a new record in the Customers table.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
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
            catch
            {
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
