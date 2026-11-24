using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace vrsutils
{
    public class Utility
    {
    //    private static string regCode;

        public static string[] DTREncryptedFields = new string[8] { "IN", "OUT_NOON", "IN_NOON", "OUT", "HOUR", "MIN", "UTIME_HOURS", "UTIME_MIN" };

        public static string[] LogEncryptedFields = new string[2] { "MESSAGE", "LDATE" };

        public static string Rkey = "JVhxSW8sv8BSXlwg";

        public static string Riv = "jmhHRaxl";

        public static string DtrRKey = "~!b3rtAyQ1+gOw8Qn2E8f7/UOJQg!01@";

        public static string dtrRIV = "k@7nI$$3v34de3ñ";

        public static int nistQuality;

        [DebuggerNonUserCode]
        public Utility()
        {
        }
  
        /*    public string registrationCode(string requestcode, byte[] rijKey, byte[] rijIV)
            {
                Crypto crypto = new Crypto();
                string text = "bAttOuSaI_0521";
                string empty = crypto.Encrypt(requestcode + text, rijKey, rijIV).ToUpper();
                empty = empty.Replace("/", "R");
                empty = empty.Replace("+", "O");
                empty = empty.Replace("=", "A");
                return empty.Substring(0,5) + '-' + empty.Substring(5, 5) + '-' + empty.Substring(10, 5) + '-' + empty.Substring(15, 5) + '-' + empty.Substring(20, 5);
            }

            public string GetMACAddress()
            {
                ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection instances = managementClass.GetInstances();
                string text = string.Empty;
                foreach (ManagementObject item in instances)
                {
                    if (string.Compare(text, string.Empty, false) == 0 && Convert.ToBoolean(RuntimeHelpers.GetObjectValue(item["IPEnabled"])))
                    {
                        text = item["MacAddress"].ToString();
                    }
                }
                return text.Replace(":", "");
            }*/

        public void checkConfigTime(string configFilename)
        {
            Crypto crypto = new();
            var now = DateTime.Now;
            byte[] Rkeybytes = Encoding.Unicode.GetBytes(Rkey);
            byte[] Rivbytes = Encoding.Unicode.GetBytes(Riv);
            string tpresent = now.ToString("MM/dd/yyyy hh:mm:ss tt");
           // var encdaynow = crypto.Encrypt(tpresent, Rkeybytes, Rivbytes);
            DataSet dataSet = new DataSet();
            try
            {
                dataSet.ReadXml(configFilename);
                nistQuality = int.Parse(dataSet.Tables[0].Rows[0]["FPQuality"].ToString());
          /*      DateTime last = Convert.ToDateTime(crypto.Decrypt(dataSet.Tables[0].Rows[0]["Tsetting"].ToString(), Rkeybytes, Rivbytes), CultureInfo.CurrentCulture.DateTimeFormat);
                if (DateTime.Compare(Convert.ToDateTime(tpresent, CultureInfo.CurrentCulture.DateTimeFormat), last) < 0)
                {
                    MessageBox.Show("System Time is incorrect.Please set the time correctly", "Biometric Login", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Application.Exit();
                }
                else
                {
                //    dataSet.Tables[0].Rows[0]["Tsetting"] = encdaynow;
                    dataSet.WriteXml(configFilename);
                }*/
                dataSet.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("System Configuration Has Changed!.Program is unusable", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Application.Exit();
            }
        }

  /*      public void EncrypRecord(ref DataSet data, string[] fieldsToEncrypt)
        {
            Crypto crypto = new Crypto();
            try
            {
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    foreach (string columnName in fieldsToEncrypt)
                    {
                        if (!Mod_rao.IsDBNull(RuntimeHelpers.GetObjectValue(row[columnName])))
                        {
                            row[columnName] = crypto.CreateEncryptedString((string)row[columnName], DtrRKey, dtrRIV);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                MessageBox.Show(ex2.ToString(), "Biometric Login", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }*/

        public void EncryptLog(ref DataSet data, string[] fieldsToEncrypt, string primaryKey)
        {
            Crypto crypto = new Crypto();
            try
            {
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    if (!Dbconn.IsDBNull(RuntimeHelpers.GetObjectValue(row[primaryKey])))
                    {
                        continue;
                    }
                    foreach (string columnName in fieldsToEncrypt)
                    {
                        if (!Dbconn.IsDBNull(RuntimeHelpers.GetObjectValue(row[columnName])))
                        {
                           // row[columnName] = crypto.CreateEncryptedString((string)row[columnName], DtrRKey, dtrRIV);
                        }
                    }
                }
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.ToString(), "Biometric Login", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void DecrypRecord(ref DataSet data, string[] fieldsToDecrypt)
        {
            Crypto crypto = new Crypto();
            try
            {
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    foreach (string columnName in fieldsToDecrypt)
                    {
                        if (row[columnName].ToString().Length > 0)
                        {
                           // row[columnName] = crypto.DecryptString((string)row[columnName], DtrRKey, dtrRIV);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Biometric Login", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
    }
}