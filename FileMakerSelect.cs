using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FileMakerConnect
{
    public class FileMakerSelect : FileMakerCommand
    {
        #region Private variables
        private List<string> SelectFields = new List<string>();
        #endregion

        #region Properties
        public string SelectClause { get; set; }
        public string OrderByClause { get; set; }
        #endregion

        #region Constructors
        public FileMakerSelect(string connectionString) : base(connectionString) { }

        public FileMakerSelect(FileMakerConnect connect) : base(connect) { }
        #endregion

        #region Methods
        /// <summary>
        /// Provide the fields you would like to retrieve from the select.
        /// </summary>
        /// <param name="field">A list of fields</param>
        public void Select(params string[] fields)
        {
            foreach (string field in fields)
                SelectFields.Add(field);
        }

        /// <summary>
        /// Add an OrderBy to this select command.
        /// </summary>
        /// <param name="field">The field you want to sort.</param>
        /// <param name="sortOrder">The Order you want to sort, either ascending or descending.</param>
        public void OrderBy(string field, SortOrder sortOrder)
        {
            string orderStr = sortOrder == SortOrder.Ascending ? "ASC" : "DESC";

            string format = null;
            if (String.IsNullOrEmpty(OrderByClause))
                format = "ORDER BY \"{0}\" {1}";
            else
                format = ", {0} {1}";

            OrderByClause += String.Format(format, field, orderStr);
        }

        public override string BuildOutputString()
        {
            string result = null;

            if (String.IsNullOrEmpty(SelectClause))
            {
                SelectClause = "SELECT";
                bool hasStarted = false;
                foreach (string field in SelectFields)
                {
                    SelectClause += hasStarted ? ", " : " ";
                    SelectClause += "\"" + field + "\"";

                    if (!hasStarted) hasStarted = true;
                }

                SelectClause.TrimEnd(' ');
            }

            result = SelectClause + " " + FromClause;

            if(!String.IsNullOrEmpty(WhereClause))
                result += " " + WhereClause;

            if (!String.IsNullOrEmpty(OrderByClause))
                result += " " + OrderByClause;

            return result;
        }

        public override void Clear()
        {
            SelectClause = null;
            SelectFields.Clear();
            FromClause = null;
            WhereClause = null;
            OrderByClause = null;
        }

        /// <summary>
        /// Execute the command and return results from FileMaker.
        /// </summary>
        /// <returns>A FileMakerResult object containing the result set.</returns>
        public FileMakerResult Execute()
        {
            DataTable table = _connect.ExecuteDataTable(BuildOutputString(), GetParameters());

            FileMakerResult result = new FileMakerResult();
            result.ResultSet = table;

            return result;
        }
        #endregion
    }
}
