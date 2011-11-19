using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            return ExtractObject<T>(0);
        }

        private T ExtractObject<T>(int rowNumber) where T : new()
        {
            try
            {
                if (ResultSet.Rows[rowNumber] != null)
                {
                    T obj = new T();
                    DataRow row = ResultSet.Rows[rowNumber];
                    Type type = typeof(T);

                    PropertyInfo[] properties = type.GetProperties();

                    foreach (PropertyInfo property in properties)
                    {
                        object[] atts = property.GetCustomAttributes(typeof(FileMakerFieldAttribute), true);
                        if (atts != null && atts.Length > 0)
                        {
                            FileMakerFieldAttribute attribute = atts[0] as FileMakerFieldAttribute;
                            DataColumn column = ResultSet.Columns.OfType<DataColumn>().FirstOrDefault(c => c.ColumnName == attribute.Name);
                            if (column != null)
                            {
                                object value = ChangeType(property.PropertyType, row[column]);
                                property.SetValue(obj, value, null);
                            }
                        }
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

        public T[] ExtractObjects<T>() where T : new()
        {
            List<T> ts = new List<T>();

            for (int i = 0; i < ResultSet.Rows.Count; i++)
            {
                T obj = ExtractObject<T>(i);
                ts.Add(obj);
            }

            return ts.ToArray();
        }

        public bool HasData()
        {
            return ResultSet != null && ResultSet.Rows.Count > 0;
        }
        #endregion

        #region Private methods
        private PropertyInfo GetPropertyFromFieldName(PropertyInfo[] properties, string name)
        {
            return properties.FirstOrDefault(p => (p.GetCustomAttributes(typeof(FileMakerFieldAttribute), true) != null
                && ((FileMakerFieldAttribute[])p.GetCustomAttributes(typeof(FileMakerFieldAttribute), true)).Any(a => a.Name == name))
                || p.Name == name);
        }

        private object ChangeType(Type outType, object value)
        {
            if (value == DBNull.Value)
                return null;

            try
            {
                Object outObj = null;

                Type underlying = Nullable.GetUnderlyingType(outType);
                if (underlying != null && underlying.IsValueType) // If this is a nullable type.
                {
                    Type nullable = typeof(Nullable<>).MakeGenericType(underlying);
                    object underObj = Convert.ChangeType(value, underlying);

                    TypeConverter conv = TypeDescriptor.GetConverter(nullable);
                    outObj = conv.ConvertFrom(underObj);
                }
                else
                    outObj = Convert.ChangeType(value, outType);

                return outObj;
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
