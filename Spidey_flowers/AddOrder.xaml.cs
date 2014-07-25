using System;
using System.Windows;
using System.Data.SqlClient;
using System.Data;

namespace Spidey_flowers
{
    /// <summary>
    /// This form is used to gather information about a new order and add to the Orders table.
    /// </summary>
    public partial class AddOrder : Window
    {
        private DatabaseConnection _dbConnect = null;
        private int _orderID = 0;
        private string _customerID = null;
        private string _date = null;
        private int _quantity = 0;
        private string _note = null;

        /// <summary>
        /// Constructor for adding customers sets the DB connection to the active connection being passed in.
        /// It then calls the populateCustomers function to populate the customer name dropdown.
        /// </summary>
        /// <param name="dbConnect">The currently used DB connection owned by MainWindow</param>
        public AddOrder(DatabaseConnection dbConnect)
        {
            _dbConnect = dbConnect;

            InitializeComponent();

            try
            {
                populateCustomers();
                getOrderID();
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not retrieve customer IDs nor Order IDs from the databse, please see the logs for more info.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine("Add Order :: " + e);
                this.Close();
            }

        }

        /// <summary>
        /// This method queries the Orders table for the max orderID value to predict the next one.
        /// This value is set to the Order ID field on the form.
        /// </summary>
        private void getOrderID()
        {
            string query = "SELECT IDENT_CURRENT('Orders');";

            SqlCommand command = new SqlCommand(query);

            try
            {
                _dbConnect.Sql = command;

                SqlDataReader reader = _dbConnect.run();

                reader.Read();

                _orderID = Convert.ToInt32(reader.GetDecimal(0)) + 1;
                orderIdBox.Text = _orderID.ToString();

                reader.Close(); //Need to close the reader as the DatabaseConnection.run method returns a reader.

            }
            catch
            {
                throw;
            }

        }

        /// <summary>
        /// This method queries the Customers table for all the customers and then populates the 'customer name' dropdown.
        /// It sets the CustomerID as the Key.
        /// </summary>
        private void populateCustomers()
        {
            string query = "SELECT CustomerID, CompanyName FROM Customers;";

            SqlCommand command = new SqlCommand(query);

            try
            {
                _dbConnect.Sql = command;

                SqlDataReader reader = _dbConnect.run();

                DataTable customers = new DataTable();
                customers.Columns.Add("Customer ID", typeof(string));
                customers.Columns.Add("Customer Name", typeof(string));
                customers.Load(reader);

                custIdBox.SelectedValuePath = "CustomerID";
                custIdBox.DisplayMemberPath = "CompanyName";
                custIdBox.ItemsSource = customers.DefaultView;

                reader.Close(); //Need to close the reader as the DatabaseConnection.run method returns a reader.

            }
            catch
            {
                throw;
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

        /// <summary>
        /// This is the active listener for the 'Add Order' button. It first calls the checkInput function to check for the correct input entries.
        /// If everything is okay, then it call on the insertDBRecord funtion to commit the changes to the DB. It then closes the window.
        /// In case of a primary key violation an appropriate error message is displayed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (checkInput() == false)
            {
                return;
            }

            int insertCheck = insertDBRecord();

            if (insertCheck == -1) //primary key violation
            {
                MessageBox.Show("DB Error: That OrderID already exsists. Please choose a different OrderID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (insertCheck == -2) //some other error
            {
                MessageBox.Show("Fatal Error: Could not add the new record to the database. Please check the logs for more information.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.Close();
        }

        /// <summary>
        /// This method checks if all the input fields are filled and then populates the correct variables.
        /// </summary>
        /// <returns>Returns true iff all input fields are correctly filled.</returns>
        private bool checkInput()
        {
            try //if the customer name is empty
            {
                _customerID = custIdBox.SelectedValue.ToString();
            }
            catch
            {
                MessageBox.Show("Customer name cannot be blank.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }


            if (dateBox.Text.Trim().Length == 0)// if the date has not been set
            {
                MessageBox.Show("You must select a delivery date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                _date = dateBox.Text;
            }

            if (qtyBox.Text.Trim().Length == 0)// if quantity is not set
            {
                MessageBox.Show("Quantity field cannot be blank.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                try
                {
                    _quantity = Convert.ToInt32(qtyBox.Text);
                }
                catch
                {
                    MessageBox.Show("Quantity must be an integer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            if (noteBox.Text.Trim().Length == 0)// if note is not set
            {
                _note = "-N-A-";
            }
            else
            {
                _note = noteBox.Text;
            }

            return true;
        }

        /// <summary>
        /// Inserting a new record in the Orders table. The query is parameterized for security purposes.
        /// </summary>
        /// <returns>0 if all OK, -1 for Primary Key Violation, -2 for all other errors</returns><summary>
        private int insertDBRecord()
        {
            string query = "INSERT INTO Orders VALUES (@CUSTID, @DATE, @QTY, @NOTE);";

            SqlCommand command = new SqlCommand(query);

            command.Parameters.Add(new SqlParameter("CUSTID", _customerID));
            command.Parameters.Add(new SqlParameter("DATE", DateTime.Parse(_date)));
            command.Parameters.Add(new SqlParameter("QTY", _quantity));
            command.Parameters.Add(new SqlParameter("NOTE", _note));

            try
            {
                _dbConnect.Sql = command;

                SqlDataReader reader = _dbConnect.run();
                reader.Close(); //Need to close the reader as the DatabaseConnection.run method returns a reader.

                return 0;
            }
            catch (SqlException e)
            {
                Console.WriteLine("Add Order :: SQL Error :: " + e);

                if (e.Number == 2627)//primary key violation
                {
                    return -1;
                }
                else
                {
                    return -2;
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine("Add Order :: DB Write :: " + e);
                return -2;
            }
        }

    }

}
