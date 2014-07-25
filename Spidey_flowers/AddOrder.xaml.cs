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
using System.Data;

namespace Spidey_flowers
{
    /// <summary>
    /// Interaction logic for AddOrder.xaml
    /// </summary>
    public partial class AddOrder : Window
    {
        private DatabaseConnection _dbConnect = null;
        private int _orderID = 0;
        private string _customerID = null;
        private string _date = null;
        private int _quantity = 0;
        private string _note = null;

        public AddOrder(DatabaseConnection dbConnect)
        {
            _dbConnect = dbConnect;

            InitializeComponent();

            try
            {
                populateCustomers();
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not retrieve customer IDs from the databse, please see the logs for more info.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine("Populate customers :: " + e);
                this.Close();
            }

        }

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
            catch (Exception e)
            {
                throw;
            }

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

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

        private bool checkInput()
        {

            if (orderIdBox.Text.Trim().Length == 0)// if order id is empty
            {
                MessageBox.Show("Order ID cannot be blank.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                try
                {
                    _orderID = Convert.ToInt32(orderIdBox.Text);
                }
                catch (Exception e)
                {
                    MessageBox.Show("Order ID must be an integer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            try //if the customer name is empty
            {
                _customerID = custIdBox.SelectedValue.ToString();
            }
            catch (Exception e)
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
                catch (Exception e)
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

        private int insertDBRecord()
        {
            string query = "INSERT INTO Orders VALUES (@ORDERID, @CUSTID, @DATE, @QTY, @NOTE);";

            SqlCommand command = new SqlCommand(query);

            command.Parameters.Add(new SqlParameter("ORDERID", _orderID));
            command.Parameters.Add(new SqlParameter("CUSTID", _customerID));
            command.Parameters.Add(new SqlParameter("DATE", _date));
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
