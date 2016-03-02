using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DAL
{
    public class CustomProcess
    {
        public static DataSet ExecuteProc(string name, Object[] parameterValues)
        {
            return SQLHelper.ExecuteDataset(GetConnectionString(),name,parameterValues);
        }

        public static int ExecuteProcNonQuery(string name, Object[] parameterValues)
        {
            return SQLHelper.ExecuteNonQuery(GetConnectionString(),name, parameterValues);
        }

        public static string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];

        }
    }
}
