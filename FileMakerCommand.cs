using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;

namespace FileMakerConnect
{
    public abstract class FileMakerCommand : IDisposable
    {
        #region Private variables
        private bool disposed = false;
        private bool disposeConnect = false;
        private List<OdbcParameter> _parameters;
        protected FileMakerConnect _connect;
        protected string FromClause { get; set; }
        protected string WhereClause { get; set; }
        #endregion

        #region Constructors
        public FileMakerCommand(string connectionString)
        {
            _connect = new FileMakerConnect(connectionString);
            disposeConnect = true;
            _parameters = new List<OdbcParameter>();
        }

        public FileMakerCommand(FileMakerConnect connect)
        {
            _connect = connect;
            _parameters = new List<OdbcParameter>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Add the FROM clause to this command.
        /// </summary>
        /// <param name="table">Name of the table you want to select FROM.</param>
        public void From(string table)
        {
            FromClause = "FROM \"" + table + "\"";
        }

        /// <summary>
        /// Add a predicate to the WHERE clause of this command. This method can be called multiple times
        /// and the predicates will be added to the tail.
        /// </summary>
        /// <param name="field">The field you wish to match against.</param>
        /// <param name="condition">What condition you want to check.</param>
        /// <param name="value">The value you wish to check for.</param>
        public void Where(string field, Condition condition, object value)
        {
            bool needsQuotes = NeedsQuotes(value.GetType());

            if (String.IsNullOrEmpty(WhereClause))
                Where(field, condition, value, needsQuotes, Operator.None);
            else
                Where(field, condition, value, needsQuotes, Operator.And);
        }

        /// <summary>
        /// Add a predicate to the WHERE clause of this command. This method can be called multiple times
        /// and the predicates will be added to the tail.
        /// </summary>
        /// <param name="field">The field you wish to match against.</param>
        /// <param name="condition">What condition you want to check. Must be either IS NULL or IS NOT NULL</param>
        public void Where(string field, Condition condition)
        {
            if (condition != Condition.IsNull && condition != Condition.IsNotNull)
                throw new ArgumentOutOfRangeException("Only conditions IsNull or IsNotNull are value without a value");

            if (String.IsNullOrEmpty(WhereClause))
                Where(field, condition, "NULL", false, Operator.None);
            else
                Where(field, condition, "NULL", false, Operator.And);
        }

        /// <summary>
        /// Add a predicate to the WHERE clause of this command. This method can be called multiple times
        /// and the predicates will be added to the tail.
        /// </summary>
        /// <param name="field">The field you wish to match against.</param>
        /// <param name="condition">What condition you want to check.</param>
        /// <param name="value">The value you wish to check for.</param>
        /// <param name="oper">If this is a subsequent predicate, whether this is an AND or OR addition.</param>
        public void Where(string field, Condition condition, object value, bool quoted, Operator oper)
        {
            if (!String.IsNullOrEmpty(WhereClause))
            {
                if (oper != Operator.None)
                    WhereClause += oper == Operator.And ? " AND" : " OR";
            }
            else
                WhereClause += "WHERE";

            string conStr = GetConditionString(condition);

            if (condition == Condition.WithoutValue)
                WhereClause += String.Format(" \"{0}\" {1} {2}", field, conStr, value);
            else
            {
                string format = " \"{0}\" {1} '{2}'";
                if (!quoted) format = format.Replace("'", "");

                //string paramName = "@p" + (_parameters.Count + 1).ToString();
                WhereClause += String.Format(format, field, conStr, value);
                //AddParameter(paramName, value);
            }
        }

        public void Where(string field, Condition condition, int value)
        {
            Where(field, condition, (object)value);
        }

        public void Where(string field, Condition condition, DateTime value)
        {
            Where(field, condition, value.ToString("M/d/yy"));
        }

        public void Where(string field, Condition condition, Double value)
        {
            Where(field, condition, (object)value);
        }

        public void Where(string field, Condition condition, Decimal value)
        {
            Where(field, condition, (object)value);
        }

        /// <summary>
        /// Create the string that will be sent to FileMaker as a query.
        /// </summary>
        /// <returns>The complete SQL string.</returns>
        public abstract string BuildOutputString();

        /// <summary>
        /// Clear out the command so that it can be reused.
        /// </summary>
        public abstract void Clear();
        #endregion

        #region Private methods
        private string GetConditionString(Condition condition)
        {
            switch(condition)
            {
                case Condition.Equals:
                    return "=";
                case Condition.NotEquals:
                    return "<>";
                case Condition.GreaterThan:
                    return ">";
                case Condition.GreaterThanOrEqualTo:
                    return ">=";
                case Condition.LessThan:
                    return "<";
                case Condition.LessThanOrEqualTo:
                    return "<=";
                case Condition.IsNull:
                    return "IS";
                case Condition.IsNotNull:
                    return "IS NOT";
                default:
                    throw new ArgumentOutOfRangeException("condition", 
                        String.Format("Condition {0} is not supported.", condition));
            }
        }

        private bool NeedsQuotes(Type type)
        {
            // Only these types do not need to be quoted.
            return type != typeof(int) && type != typeof(decimal) && type != typeof(double);
        }

        protected void AddParameter(string name, object value)
        {
            OdbcParameter param = new OdbcParameter(name, value);
            _parameters.Add(param);
        }

        protected OdbcParameter[] GetParameters()
        {
            return _parameters.ToArray();
        }
        #endregion

        #region Interface implementations
        public void Dispose()
        {
            if (!disposed)
            {
                Clear();

                if (disposeConnect)
                {
                    if (_connect != null)
                    {
                        _connect.Dispose();
                        _connect = null;
                    }
                }

                disposed = true;
            }
        }
        #endregion
    }
}
