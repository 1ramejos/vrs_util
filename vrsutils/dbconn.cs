
using MySqlConnector;
using System;
using System.Data;
using System.Diagnostics;
using System.Formats.Tar;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace vrsutils
{
    internal class Dbconn
    {
        public static MySqlConnection app_con = new MySqlConnection();

        public static string sqlselprov = "SELECT PRV, AREA FROM ref.allprov order by AREA";

        private static int intindex;

        public static string sqlselmun;

        private static string strentry;

        private static readonly bool mResetonClear = false;

        public static DateTime rdatetime;

        public static TimeSpan ntime;

        public static string Year;

        public static string Dmonth; // monthname for checking if month has changed to make new schema

        public static string dday; //weekday name

        public static int monthcode; //month num

        public static string schema;

        public static string con_str;

        public static string curmonth; // monthname

        public static string currentday;

        private static readonly byte[] key = new byte[24]
        {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
            11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
            21, 22, 23, 24
        };

        private static readonly byte[] iv = new byte[8] { 65, 110, 68, 26, 69, 178, 200, 219 };


        public static void Populate_List(string StrQuery, ComboBox DropDwnLst, string DispMember, string ValMember = null)
        {
            try
            {
                DBTools dBTools = new();
                DataTable dataSource = dBTools.MySqlFetchDataTable(StrQuery, con_str);
                DropDwnLst.DataSource = dataSource;
                DropDwnLst.DisplayMember = DispMember;
                DropDwnLst.ValueMember = ValMember;
                DropDwnLst.SelectedIndex = -1;
            }
            catch { }
        }


        public static void GetKeyPress(ComboBox combo, KeyPressEventArgs e)
        {
            checked
            {
                if (char.IsControl(e.KeyChar))
                {
                    if (combo.SelectionStart <= 1)
                    {
                        if (mResetonClear)
                        {
                            combo.SelectedIndex = 0;
                            combo.SelectAll();
                        }
                        else
                        {
                            combo.Text = string.Empty;
                            combo.SelectedIndex = -1;
                        }
                        e.Handled = true;
                        return;
                    }
                    if (combo.SelectionLength == 0)
                    {
                        strentry = combo.Text.Substring(0, combo.Text.Length - 1);
                    }
                    else
                    {
                        strentry = combo.Text.Substring(0, combo.SelectionStart - 1);
                    }
                }
                else
                {
                    if (!char.IsLetterOrDigit(e.KeyChar) & !char.IsWhiteSpace(e.KeyChar))
                    {
                        return;
                    }
                    if (combo.SelectionLength == 0)
                    {
                        strentry = (combo.Text + e.KeyChar).ToUpper();
                    }
                    else
                    {
                        strentry = combo.Text.Substring(0, combo.SelectionStart) + e.KeyChar;
                    }
                }
                intindex = combo.FindString(strentry);
                if (intindex != -1)
                {
                    combo.SelectedIndex = intindex;
                    combo.SelectionStart = strentry.Length;
                    combo.SelectionLength = combo.Text.Length - combo.SelectionStart;
                }
                e.Handled = true;
            }
        }


        private static int Asc(char c)
        {
            int converted = c;
            if (converted >= 0x80)
            {
                byte[] buffer = new byte[2];
                // if the resulting conversion is 1 byte in length, just use the value
                switch (Encoding.Default.GetBytes(new char[] { c }, 0, 1, buffer, 0))
                {
                    case 1:
                        converted = buffer[0];
                        break;

                    default:
                        // byte swap bytes 1 and 2;
                        converted = buffer[0] << 16 | buffer[1];
                        break;
                }
            }
            return converted;
        }
        public static void SetCustomDate(DateTime dt)//, TimeOnly ti)
        {
            // DateOnly dt = idt;
            //ntime = TimeOnly.FromDateTime(DateTime.Parse(dt.ToString("hh:mm:ss tt")));
            ntime = dt.TimeOfDay;
            rdatetime = DateTime.Parse(dt.ToString("yyyy-MM-dd hh:mm:ss tt"));
            Year = dt.Year.ToString();
            monthcode = dt.Month;
            currentday = dt.DayOfWeek.ToString(); // monday, tuesday ...
            dday = dt.DayOfWeek.ToString();
            curmonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dt.Month);
        }

        public static bool IsDBNull(object value)
        {
            if (value == DBNull.Value) return true;
            return value is IConvertible convertible ? convertible.GetTypeCode() == TypeCode.DBNull : false;
        }

        public static int CreateSchema(string SchemaName)
        {
            string sql = "create schema if not exists " + SchemaName + "";
            DBTools dBTools = new DBTools();
            int result = default;
            try
            {
                result = checked((int)dBTools.MySqlExecuteNonQuery(sql, con_str));
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Create Schema");
            }
            return result;
        }

        public static int CreateTable(string SchemaName, string Tablename, string PrimaryKey, string Engine)
        {
            string sql = "create table if not exists " + SchemaName + "." + Tablename + " like dtr.template";
            DBTools dBTools = new DBTools();
            int result = default;
            try
            {
                result = checked((int)dBTools.MySqlExecuteNonQuery(sql, con_str));
                return result;
            }
            catch //(Exception ex)
            {
                // Exception ex2 = ex;
            }
            return result;
        }

        public static string KeyGenerator()
        {
            RndmKeyGenerator rndmKeyGenerator = new RndmKeyGenerator()
            {
                KeyLetters = "BCD©T¤R¶I®S§H®A@M#A%Y&EMNO",
                KeyNumbers = "6101980521347",
                KeyChars = 10
            };
            return rndmKeyGenerator.Generate();
        }

        public static string Right(string original, int numberCharacters)
        {
            if (String.IsNullOrEmpty(original)) return string.Empty;
            return original.Length <= numberCharacters ? original : original.Substring(original.Length - numberCharacters);
            //   return original.Substring(original.Length - numberCharacters);
        }

        public static string ExePath()
        {
            //   s = s.Substring(s.LastIndexOf("\\", s.Length - 2) + 1);
            string path = Assembly.GetExecutingAssembly().Location;
            int length = path.LastIndexOf("\\", path.Length - 2) + 1;
            return path.Substring(0, length);
        }

        private static DataSet UpdateTable(string mySelectQuery, string myTableName, string hash)
        {
            MySqlConnection mySqlConnection = new MySqlConnection(con_str);
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter()
            {
                SelectCommand = new MySqlCommand(mySelectQuery, mySqlConnection)
            };
            MySqlCommandBuilder mySqlCommandBuilder = new MySqlCommandBuilder(mySqlDataAdapter);
            mySqlConnection.Open();
            DataSet dataSet = new DataSet();
            DataSet result;
            try
            {
                mySqlDataAdapter.Fill(dataSet, myTableName);
                dataSet.Tables[myTableName].PrimaryKey = new DataColumn[1] { dataSet.Tables[myTableName].Columns["day"] };
            /*    dataSet.Tables[myTableName].Rows[0]["DAYS"] = DayName;
                dataSet.Tables[myTableName].Rows[0]["IN_AM"] = dIN;
                dataSet.Tables[myTableName].Rows[0]["OUT_NOON"] = dOutNoon;
                dataSet.Tables[myTableName].Rows[0]["IN_NOON"] = dInNoon;
                dataSet.Tables[myTableName].Rows[0]["OUT_PM"] = dOut;
                dataSet.Tables[myTableName].Rows[0]["SALARY"] = salary;
                dataSet.Tables[myTableName].Rows[0]["SALGRADE"] = sgrade;
                dataSet.Tables[myTableName].Rows[0]["SCHED_FROM"] = schedfrom;
                dataSet.Tables[myTableName].Rows[0]["SCHED_TO"] = schedto;
                dataSet.Tables[myTableName].Rows[0]["LASTNAME"] = lastname;
                dataSet.Tables[myTableName].Rows[0]["FIRSTNAME"] = firstname;
                dataSet.Tables[myTableName].Rows[0]["MATERNALNAME"] = maternalname;
                dataSet.Tables[myTableName].Rows[0]["OT_HOURS"] = hour;
                dataSet.Tables[myTableName].Rows[0]["OT_MIN"] = min;
                dataSet.Tables[myTableName].Rows[0]["UTIME_HOURS"] = utime_hour;
                dataSet.Tables[myTableName].Rows[0]["UTIME_MIN"] = utime_min;
                dataSet.Tables[myTableName].Rows[0]["MONTH_NAME"] = month_name;
                dataSet.Tables[myTableName].Rows[0]["WORKING_HOURS"] = wrk_hour;
                dataSet.Tables[myTableName].Rows[0]["DESIGNATION"] = desig;
                dataSet.Tables[myTableName].Rows[0]["TAX"] = tax;
                dataSet.Tables[myTableName].Rows[0]["HASH"] = hash;*/
                mySqlDataAdapter.Update(dataSet, myTableName);
                mySqlDataAdapter.Dispose();
                mySqlConnection.Close();
                result = dataSet;
            }
            catch (Exception ex)
            {
                mySqlDataAdapter.Dispose();
                mySqlConnection.Close();
                EventLog(schema, myTableName, ex.ToString(), rdatetime, "error", "5");
                result = dataSet;
            }
            return result;
        }

        public static void ClearTable()
        {
            string sql = "update dtr.ot set DAYS = null,in_am = null,out_noon = null,ot_hours=null,ot_min=null,in_noon = null,out_pm = null,sched_from = null,sched_to=null,salary=null,salgrade = null,lastname=null,firstname=null,maternalname=null,tax=null,month_name = null,designation = null,utime_hours = null,utime_min = null,hash = null";
            DBTools dBTools = new DBTools();
            dBTools.MySqlExecuteNonQuery(sql, con_str);
        }

        public static void GetSelectedRecord(string empid, string monthname, int work_hours, string SelSchema, bool NoTax, string ReportType)
        {
            string sql = "select a.lastname,a.firstname,a.maternalname,a.sched_from,a.sched_to,a.salary,a.tax,a.designation,b.date,b.days,b.in,b.out_noon,b.in_noon,b.out_noon,b.out,b.hour,b.min,b.utime_hours,b.utime_min from personnel.employees a," + SelSchema + "." + monthname + " b where b.emp_id = '" + empid + "' and a.emp_id = b.emp_id order by date";
            DBTools dBTools = new DBTools();
            Encryption encryption = new Encryption();
            Crypto crypto = new Crypto();
  
            DataSet data = dBTools.MySqlFetchDataset(sql, con_str);
            Utility utility = new Utility();
            utility.DecrypRecord(ref data, Utility.DTREncryptedFields);
            foreach (DataRow row in data.Tables[0].Rows)
            {
       
                try
                {
             //       lastname = encryption.DecryptString(row["lastname"].ToString());
                }
                catch { }
                try
                {
              //      firstname = encryption.DecryptString(row["firstname"].ToString());
                }
                catch { }
                try
                {
              //      maternalname = encryption.DecryptString(row["maternalname"].ToString());
                }
                catch { }
         //       string mySelectQuery = "select * from dtr.ot where `date` = " + theDay + "";
                string text = KeyGenerator();
           //     string plainText = text + ReportType + monthname + Right(SelSchema, 4) + firstname + maternalname.Substring(0, 1) + lastname;
                EncryptClass encryptClass = new EncryptClass();
              //  string hash = Convert.ToBase64String(encryptClass.Encrypt(plainText));
            //    UpdateTable(mySelectQuery, "table1", hash);
            }
        }

        public static void EventLog(string schema, string table, string msg, DateTime ddate, string id, string logcode)
        {
            string cmdText = "select * from " + schema + "." + table + " where CODE is null";
            MySqlConnection mySqlConnection = new MySqlConnection(con_str);
            MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter()
            {
                SelectCommand = new MySqlCommand(cmdText, mySqlConnection)
            };
            MySqlCommandBuilder mySqlCommandBuilder = new MySqlCommandBuilder(mySqlDataAdapter);
            string text = "Table1";
            mySqlConnection.Open();
            DataSet data = new DataSet();
            string value = ddate.ToString();
            try
            {
                mySqlDataAdapter.Fill(data, text);
                DataRow dataRow = data.Tables[text].NewRow();
                dataRow["MESSAGE"] = msg;
                dataRow["LDATE"] = value;
                dataRow["ID"] = id;
                dataRow["LOGCODE"] = logcode;
                data.Tables["Table1"].Rows.Add(dataRow);
                Utility utility = new Utility();
                utility.EncryptLog(ref data, Utility.LogEncryptedFields, "CODE");
                mySqlDataAdapter.Update(data, text);
                mySqlDataAdapter.Dispose();
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Event Log Error");
            }
        }


    }
}