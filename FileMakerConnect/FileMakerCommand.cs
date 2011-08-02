using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileMakerConnect
{
    public abstract class FileMakerCommand : IDisposable
    {
        #region Private variables
        private bool disposed = false;
        private bool disposeConnect = false;
        protected FileMakerConnect _connect;
        protected string FromClause { get; set; }
        protected string WhereClause { get; set; }
        #endregion

        #region Constructors
        public FileMakerCommand(string connectionString)
        {
            _connect = new FileMakerConnect(connectionString);
            disposeConnect = true;
        }

        public FileMakerCommand(FileMakerConnect connect)
        {
            _connect = connect;
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
        public void Where(string field, Condition condition, string value)
        {
            if (String.IsNullOrEmpty(WhereClause))
                Where(field, condition, value, Operator.None);
            else
                Where(field, condition, value, Operator.And);
        }

        /// <summary>
        /// Add a predicate to the WHERE clause of this command. This method can be called multiple times
        /// and the predicates will be added to the tail.
        /// </summary>
        /// <param name="field">The field you wish to match against.</param>
        /// <param name="condition">What condition you want to check.</param>
        /// <param name="value">The value you wish to check for.</param>
        /// <param name="oper">If this is a subsequent predicate, whether this is an AND or OR addition.</param>
        public void Where(string field, Condition condition, string value, Operator oper)
        {
            if (!String.IsNullOrEmpty(WhereClause))
            {
                if (oper != Operator.None)
                    WhereClause += oper == Operator.And ? " AND" : " OR";
            }
            else
                WhereClause += "WHERE";

            string format = null;
            if (condition == Condition.IsNull || condition == Condition.IsNotNull)
                format = " \"{0}\" {1} {2}";
            else
                format = " \"{0}\" {1} \"{2}\"";

            string conStr = GetConditionString(condition);

            WhereClause += String.Format(format, field, conStr, value);
        }

        public void Where(string field, Condition condition, int value)
        {
            Where(field, condition, value.ToString());
        }

        public void Where(string field, Condition condition, DateTime value)
        {
            Where(field, condition, value.ToString("MM/dd/yy"));
        }

        public void Where(string field, Condition condition, Double value)
        {
            Where(field, condition, value.ToString());
        }

        public void Where(string field, Condition condition, Decimal value)
        {
            Where(field, condition, value.ToString());
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
