using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMakerConnect
{
    public class FileMakerUpdate : FileMakerCommand
    {
        #region Private variables
        public string UpdateClause;
        public string SetClause;
        #endregion

        #region Constructors
        public FileMakerUpdate(string connectionString) : base(connectionString) { }

        public FileMakerUpdate(FileMakerConnect connect) : base(connect) { }

        public FileMakerUpdate(string connectionString, string tableToUpdate) : base(connectionString)
        {
            UpdateClause = "UPDATE " + tableToUpdate;
        }

        public FileMakerUpdate(FileMakerConnect connect, string tableToUpdate) : base(connect)
        {
            UpdateClause = "UPDATE " + tableToUpdate;
        }
        #endregion

        #region Methods
        public void Update(string tableToUpdate)
        {
            UpdateClause = "UPDATE " + tableToUpdate;
        }

        public void Set(string field, string value)
        {
            string format = null;
            if (String.IsNullOrEmpty(SetClause))
            {
                SetClause = "SET ";
                format = "\"{0}\" = \"{1}\"";
            }
            else
                format = ", \"{0}\" = \"{1}\"";

            SetClause += String.Format(format, field, value);
        }

        public void Set(string field, int value)
        {
            Set(field, value.ToString());
        }

        public void Set(string field, DateTime value)
        {
            Set(field, value.ToString("MM/dd/yy"));
        }

        public void Set(string field, Double value)
        {
            Set(field, value.ToString());
        }

        public void Set(string field, Decimal value)
        {
            Set(field, value.ToString());
        }

        public override string BuildOutputString()
        {
            if (String.IsNullOrEmpty(WhereClause))
                throw new InvalidUpdateException("Missing WHERE class");

            string result = null;
            result = UpdateClause + " " + SetClause;

            if (!String.IsNullOrEmpty(FromClause))
                result += " " + FromClause;

            result += " " + WhereClause;

            return result;
        }

        public override void Clear()
        {
            UpdateClause = null;
            SetClause = null;
            FromClause = null;
            WhereClause = null;
        }

        public void Execute()
        {
            _connect.ExecuteNotQuery(BuildOutputString());
        }
        #endregion
    }

    public class InvalidUpdateException : Exception
    {
        public InvalidUpdateException() { }

        public InvalidUpdateException(string message) : base(message) { }
    }
}
