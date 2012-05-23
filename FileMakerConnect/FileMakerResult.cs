using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileMakerConnect
{
    public class FileMakerResult
    {
        #region Properties
        /// <summary>
        /// The DataTable for this result which holds the resultset, if applicable
        /// </summary>
        public DataTable ResultSet { get; set; }
        #endregion

        #region Methods
        public T ExtractObject<T>() where T : new()
        {
            try
            {
                if (ResultSet.Rows.Count > 0)
                {
                    T obj = new T();
                    DataRow row = ResultSet.Rows[0];
                    Type type = typeof(T);

                    PropertyInfo[] properties = type.GetProperties();

                    foreach (DataColumn column in ResultSet.Columns)
                    {
                        PropertyInfo property = properties.FirstOrDefault(a => a.Name == column.ColumnName);
                        if (property != null)
                            property.SetValue(obj, row[column], null);
                    }

                    return obj;
                }
                else { return default(T); }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T[] ExtractObjects<T>()
        {
            // TODO Implement
            throw new NotImplementedException();
        }
        #endregion
    }
}
