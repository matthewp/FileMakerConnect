using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        public T ExtractObject<T>()
        {
            throw new NotImplementedException();
        }

        public T[] ExtractObjects<T>()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
