using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Spidey_flowers
{
    class DatabaseConnection
    {
        private string _sqlQuery = null;
        private string _connectionStr = null;
        private SqlConnection _dbConnection = null;
        // Track whether Dispose has been called. 
        private bool disposed = false;

        /// <summary>
        /// The SQL property is the query that is going to be run.
        /// </summary>
        /// <value>Setting the SQL query variable.</value>
        public string Sql
        {
            set { _sqlQuery = value; }
        }

        /// <summary>
        /// Setting the connection string to the DB.
        /// </summary>
        public string connectionString
        {
            set { _connectionStr = value; }
        }

        /// <summary>
        /// Connecting to the DB after the connection string 
        /// </summary>
        /// <exception cref="System.NullReferenceException">Thrown when the connection string has not been set.</exception>
        /// <remarks>There are other exceptions being thrown by Open from SqlConnection, but not listed here.</remarks>
        public void connectDB()
        {
            if (_connectionStr == null)
            {
                throw new System.NullReferenceException("The SQL Connection string is not set. Nothing to connect to.");
            }
            else
            {
                _dbConnection = new SqlConnection(_connectionStr);
                try
                {
                    _dbConnection.Open();
                }
                catch
                {
                    throw; /* re-throw the exception, so that the stack trace is preserved */
                }
            }
        }

        /// <summary>
        /// Running the SQL query and returning the resultant SqlDataReader object.
        /// </summary>
        /// <returns>SqlDataReader object with the returned rows.</returns>
        /// <remarks>The method caller is responsible for closing the reader object.</remarks>
        public SqlDataReader run()
        {
            if (_dbConnection == null)
            {
                throw new InvalidOperationException("DB connection is not set.");
            }
            else if(_dbConnection.State != ConnectionState.Open){
                throw new InvalidOperationException("DB connection is not open.");
            }
            else if (_sqlQuery == null)
            {
                throw new InvalidOperationException("SQL query is not defined.");
            }
            else
            {
                SqlCommand cmd = new SqlCommand(_sqlQuery, _dbConnection);

                SqlDataReader reader = cmd.ExecuteReader();

                return reader;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Closing the db connection before the object is destroyed.
        /// </summary>
        /// <remarks>There are exceptions being thrown by Close from SqlConnection, but not listed here.</remarks>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    try
                    {
                        _dbConnection.Close();
                    }
                    catch
                    {
                        throw; /* re-throw the exception, so that the stack trace is preserved */
                    }
                }

                // Note disposing has been done.
                disposed = true;

            }
        }

        ~DatabaseConnection()
        {
            Dispose(false);
        }

    }
}
