using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;

namespace FileMakerConnect
{
    public class FileMakerConnect : IDisposable
    {
        #region Private variables
        private OdbcConnection _connection;
        private OdbcCommand _command;
        private OdbcDataAdapter _adapter;
        #endregion

        #region Properties
        /// <summary>
        /// The driver connection string used to connect to FileMaker via ODBC.
        /// </summary>
        public string ConnectionString { get; set; }
        #endregion

        #region Constructors
        public FileMakerConnect()
        {
            _connection = new OdbcConnection();
            _command = new OdbcCommand();
            _command.Connection = _connection;
            _adapter = new OdbcDataAdapter(_command);
        }

        public FileMakerConnect(string connectionString) : this()
        {
            ConnectionString = connectionString;
        }
        #endregion

        #region Methods
        public DataTable ExecuteDataTable(string sql)
        {
            DataTable table = new DataTable();

            _command.CommandText = sql;
            _adapter.Fill(table);

            return table;
        }

        public void ExecuteNotQuery(string sql)
        {
            _command.CommandText = sql;
            _command.ExecuteNonQuery();
        }

        public object ExecuteScalar(string sql)
        {
            _command.CommandText = sql;
            return _command.ExecuteScalar();
        }

        public T ExecuteScalar<T>(string sql)
        {
            _command.CommandText = sql;
            return (T)_command.ExecuteScalar();
        }
        #endregion

        #region Interface implementations
        public void Dispose()
        {
            try
            {
                if (_connection != null && _connection.State != ConnectionState.Closed)
                {
                    _connection.Close();
                }

                if (_connection != null)
                {
                    _connection.Dispose();
                    _connection = null;
                }

                if (_command != null)
                {
                    _command.Dispose();
                    _command = null;
                }

                if (_adapter != null)
                {
                    _adapter.Dispose();
                    _adapter = null;
                }
            }
            catch { }
        }
        #endregion
    }
}
