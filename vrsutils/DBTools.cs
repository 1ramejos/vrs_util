using MySqlConnector;
using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace vrsutils
{
    public class DBTools
    {
        [DebuggerNonUserCode]
        public DBTools()
        {
        }
   
        public long MySqlExecuteNonQuery(string sql, string con_str)
        {
            MySqlConnection mySqlConnection = new(con_str);
            MySqlCommand mySqlCommand = new(sql, mySqlConnection);
            mySqlConnection.Open();
            long result = mySqlCommand.ExecuteNonQuery();
            mySqlCommand.Dispose();
            mySqlConnection.Close();
            return result;
        }

        public MySqlDataReader MySqlFetchDataReader(string sql, string con_str)
        {
            MySqlConnection mySqlConnection = new(con_str);
            MySqlCommand mySqlCommand = new(sql, mySqlConnection);
            if (mySqlConnection.State == ConnectionState.Closed)
            {
                mySqlConnection.Open();
            }
            return mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public object MySqlFetchScalar(string sql, string con_str)
        {
            MySqlConnection mySqlConnection = new(con_str);
            MySqlCommand mySqlCommand = new(sql, mySqlConnection);
            mySqlConnection.Open();
            mySqlCommand.Dispose();
            mySqlConnection.Close();
            object objectValue = RuntimeHelpers.GetObjectValue(mySqlCommand.ExecuteScalar());
            return objectValue;
        }

        public DataTable MySqlFetchDataTable(string sql, string con_str)
        {
            DataTable dataTable = new();
            MySqlConnection mySqlConnection = new (con_str);
            MySqlDataAdapter mySqlDataAdapter = new ()
            {
                SelectCommand = new MySqlCommand(sql, mySqlConnection)
            };
            mySqlDataAdapter.Fill(dataTable);
            mySqlConnection.Close();
            return dataTable;
        }

        public DataSet MySqlFetchDataset(string sql, string con_str)
        {
            DataSet dataSet = new DataSet();
            MySqlConnection mySqlConnection = new (con_str);
            MySqlDataAdapter mySqlDataAdapter = new ()
            {
                SelectCommand = new MySqlCommand(sql, mySqlConnection)
            };
            mySqlDataAdapter.Fill(dataSet, "temp");
            mySqlConnection.Close();
            return dataSet;
        }

        public MySqlDataReader getReader(string sql, string con_str)
        {
            MySqlConnection mySqlConnection = new (con_str);
            MySqlCommand mySqlCommand = new (sql, mySqlConnection);
            if (mySqlConnection.State == ConnectionState.Closed)
            {
                mySqlConnection.Open();
            }
            MySqlDataReader result = mySqlCommand.ExecuteReader();
            mySqlCommand.Parameters.Clear();
            return result;
        }
    }
}