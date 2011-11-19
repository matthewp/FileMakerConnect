using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMakerConnect
{
    public class FileMakerDelete : FileMakerCommand
    {
        #region Properties
        public string DeleteClause { get; set; }
        #endregion

        #region Constructors
        public FileMakerDelete(string connectionString) : base(connectionString) { }

        public FileMakerDelete(FileMakerConnect connect) : base(connect) { }

        public FileMakerDelete(string connectionString, string tableName) : base(connectionString)
        {
            DeleteClause = String.Format("DELETE FROM \"{0}\"", tableName);
        }

        public FileMakerDelete(FileMakerConnect connect, string tableName) : base(connect)
        {
            DeleteClause = String.Format("DELETE FROM \"{0}\"", tableName);
        }
        #endregion

        #region Methods
        public void DeleteFrom(string tableName)
        {
            DeleteClause = String.Format("DELETE FROM \"{0}\"", tableName);
        }

        public override string BuildOutputString()
        {
            if (String.IsNullOrEmpty(WhereClause))
                throw new InvalidDeleteException("Unable to delete without a WHERE clause");

            if (String.IsNullOrEmpty(DeleteClause))
                throw new InvalidDeleteException("Unable to delete without DELETE FROM clause");

            string result = DeleteClause + " ";
            if (!String.IsNullOrEmpty(FromClause))
                result += FromClause + " ";

            result += WhereClause;

            return result;
        }

        public override void Clear()
        {
            DeleteClause = null;
            FromClause = null;
            WhereClause = null;
        }
        #endregion
    }

    public class InvalidDeleteException : Exception
    {
        public InvalidDeleteException() { }

        public InvalidDeleteException(string message) : base(message) { }
    }
}
