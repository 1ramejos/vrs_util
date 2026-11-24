
using MySqlConnector;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using static vrsutils.AppUtils;
using static vrsutils.CertificateAction;
using static vrsutils.WSQ;


namespace vrsutils
{
    public partial class Form1 : Form
    {
        private const string scriptGeneratorSecKey = AppConfig.scriptGeneratorSecKey;

        private readonly byte[] RijindaelKey;

        private readonly byte[] RijindaelIV;

        private readonly Utility utils;

        private readonly TimeSpan newtime;

        private readonly DateTime newdate;

        private readonly string scrGenSecKey = scriptGeneratorSecKey;

        private readonly string privKeyFile = AppConfig.privateKeyFile;

        private string pcode;

        private bool nocert;

        private bool noinput;

        private bool isconnected = false;

        public Form1()
        {
            RijindaelKey =
          [
                121, 29, 20, 246, 104, 31, 170, 150, 191, 147,
                184, 156, 164, 41, 71, 160, 194, 142, 18, 199,
                188, 151, 8, 51, 191, 150, 91, 154, 158, 241,
                128, 98
          ];
            RijindaelIV =
            [
                170, 225, 110, 154, 230, 126, 103, 53, 207, 142,
                120, 93, 153, 54, 182, 46
            ];
            GetConnection();
            utils = new Utility();
            InitializeComponent();
        }

        private void cmb_mun_KeyPress(object sender, KeyPressEventArgs e)
        {
            Dbconn.GetKeyPress(cmb_mun, e);
        }

        private void cmb_mun_TextChanged(object sender, EventArgs e)
        {
            // cmb_textchanged(cmb_mun, e);
        }

        private void cmb_prov_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmb_prov.SelectedIndex > -1)
                {
                    pcode = cmb_prov.SelectedValue.ToString();
                    Dbconn.sqlselmun = "Select MUN, AREANAME from ref.allmun where ID_CITY  like '" + pcode + "%' order by AREANAME";
                    Dbconn.Populate_List(Dbconn.sqlselmun, cmb_mun, "AREANAME", "MUN");
                    cmb_mun.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                //  Text += "  Error!  " + ex.Message;
            }
            if (cmb_mun.Items.Count == 0)
            {
                cmb_mun.Tag = false;
                cmb_mun.BackColor = SystemColors.Window;
                //  ValidateOK();
            }
        }


        private void cmb_prov_KeyPress(object sender, KeyPressEventArgs e)
        {
            Dbconn.GetKeyPress(cmb_prov, e);
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            this.MinimumSize = new Size(903, 301);
            this.Size = new Size(903, 301);
            tabControl1.SelectedIndex = 1;
            // for editing DT.txt
            /*    dbconn.SetCustomDate(DateTime.Now);
                dbconn.Dmonth = dbconn.curmonth;
                newtime = dbconn.ntime;// TimeOnly.Parse(Mod_rao.ntime.ToString("hh:mm:ss tt"));
                newdate = dbconn.rdatetime;//.ToString("MMMM dd, yyyy hh:mm tt"));*/
            //  ReaderWriterLock DT.txt 
            //  utils.checkConfigTime("Biometrics_Login.xml");

            nocert = true;
            noinput = true;

           if (isconnected == true)
            {
                Dbconn.Populate_List(Dbconn.sqlselprov, cmb_prov, "AREA", "PRV");
                cmb_prov.SelectedIndex = 50;
                ViewProfile();
            }
          

            // code to make table
            /*    dbconn.schema = "configuration_local";
                dbconn.CreateSchema(dbconn.schema);
                dbconn.CreateTable(dbconn.schema, dbconn.curmonth, "ID", "MyISAM");
                dbconn.CreateTableLog(dbconn.schema, dbconn.curmonth + "Log", "CODE", "MYISAM");*/


        }
        private void GetConnection()
        {
            Appinit();
            try
            {
                Dbconn.con_str = "server=localhost;uid=vrs;port=3306;database=personnel;pwd=" + GlobalVariables.DatabasePassword;
                isconnected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void ViewProfile()
        {
            try
            {
                listView1.Items.Clear();
                Crypto crypto = new();
                CertificateManager certman = new();
                string logquery = "SELECT id, uname, pword, sys_user_group_id, fname, lname, is_locked, is_delete, fingerprint FROM configuration_local.sys_users";
                DBTools dBTools = new();
                MySqlDataReader mySqlDataReader = dBTools.MySqlFetchDataReader(logquery, Dbconn.con_str);
                int fieldCount = mySqlDataReader.FieldCount;
                string[] array2 = new string[fieldCount + 1];
                int i = 0;
                string tmpCryp = "";
                int tmpnum;
                int q = 0;
                listView1.BeginUpdate();
                while (mySqlDataReader.Read())
                {
                    for (; i != fieldCount; i++)
                    {
                        string name = mySqlDataReader.GetName(i);
                        try
                        {
                            switch (name)

                            {
                                case "fname":
                                case "mname":
                                case "lname":
                                    tmpCryp = mySqlDataReader.GetString(mySqlDataReader.GetOrdinal(name));
                                    array2[i] = crypto.RijndaelDecrypt(tmpCryp, "v/P0f~VKh*IwFspPpxnqzSXbXixu*jFN",
                                    "ckJSkPY!lWDzC$eO*czH@sx?M8wlhpEB", 256);
                                    break;
                                case "pword":
                                    if (nocert == false)
                                    {
                                        tmpCryp = mySqlDataReader.GetString(mySqlDataReader.GetOrdinal(name));
                                        array2[i] = certman.Decrypt(tmpCryp, GlobalVariables.VRSPrivkey);
                                    }
                                    break;
                                case "sys_user_group_id":
                                    tmpnum = mySqlDataReader.GetInt32(mySqlDataReader.GetOrdinal(name));
                                    array2[i] = tmpnum == 9999 ? "Local admin" : "user";
                                    break;
                                case "id":
                                    array2[i] = mySqlDataReader.GetInt32(mySqlDataReader.GetOrdinal(name)).ToString();
                                    break;
                                case "uname":
                                    array2[i] = mySqlDataReader.GetString(mySqlDataReader.GetOrdinal(name)).ToString();
                                    break;
                                case "is_locked":
                                    tmpnum = mySqlDataReader.GetInt32(mySqlDataReader.GetOrdinal(name));
                                    array2[i] = tmpnum == 1 ? "Yes" : "";
                                    break;
                                case "is_delete":
                                    tmpnum = mySqlDataReader.GetInt32(mySqlDataReader.GetOrdinal(name));
                                    array2[i] = tmpnum == 1 ? "Yes" : "";
                                    break;
                                case "fingerprint":
                                    q++;
                                    byte[] arrayFP1 = new byte[(int)(mySqlDataReader.GetBytes(mySqlDataReader.GetOrdinal("fingerprint"), 0L, null, 0, int.MaxValue) - 1) + 1];
                                    mySqlDataReader.GetBytes(mySqlDataReader.GetOrdinal("fingerprint"), 0L, arrayFP1, 0, arrayFP1.Length);

                                    Bitmap fingerprintImage = WSQToBitmap(arrayFP1);
                                    //   
                                    fingerprintImage.Save(q + "-" + mod_Image.FingerPrintImageFilename);

                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }


                    ListViewItem value = new ListViewItem(array2);
                    listView1.Items.Add(value);
                    //       lblStatus.Value = lstLogView.Items.Count;
                    //      tslabel.Text = "loading " + lstLogView.Items.Count.ToString() + " of " + reccount;
                    i = 0;
                }
              //  mySqlDataReader.Close();
              //  mySqlDataReader.Dispose();
            }
            catch (Exception ex)
            {
             MessageBox.Show(ex.Message, "ambot");

            }

            if ((listView1.Items.Count - 1) > 100)
            {
                listView1.EnsureVisible(listView1.Items.Count - 1);
            }
            listView1.EndUpdate();
            GC.Collect();
        }


        private byte[] GetPicture()
        {
            string fileName = Dbconn.ExePath() + "SnapShot.bmp";
            FileInfo fileInfo = new FileInfo(fileName);
            checked
            {
                byte[] array = new byte[fileInfo.Length + 1];
                FileStream fileStream = fileInfo.OpenRead();
                fileStream.Read(array, 0, int.Parse(fileInfo.Length.ToString()));
                fileStream.Close();
                return array;
            }
        }

        public static string GenerateAccessCode(string rCode)
        {
            ArgumentNullException.ThrowIfNull(rCode);
            //  ArgumentNullException.ThrowIfNull(contextApp);

            _ = new GlobalVariables();
            string key = CipherTools.RijndaelCipher(rCode, GlobalVariables.GetAppSecureKey(),
                GlobalVariables.GetAppInitVector(), 256, true).ToUpperInvariant();

            return Regex.Replace(key, "[+/&=]", "");
        }

        static string InsertDashEvery5(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            StringBuilder sb = new();

            for (int i = 0; i < input.Length; i++)
            {
                sb.Append(input[i]);
                if ((i + 1) % 5 == 0 && i != input.Length - 1)
                {
                    sb.Append('-');
                }
            }

            return sb.ToString();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string tenmp = GenerateAccessCode(textBox1.Text);

            string t2 = tenmp.Substring(10, 20);

            textBox2.Text = InsertDashEvery5(t2);

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            string tenmp = GenerateAccessCode(textBox3.Text);

            string t2 = tenmp.Substring(15, 20);

            textBox4.Text = InsertDashEvery5(t2);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            string textback = "";
            // CryptoUtils cryptoUtils = new();
            BigInteger x = CryptoUtils.GetBigInteger(textBox1.Text);
            byte[] bytes = x.ToByteArray(isUnsigned: false, isBigEndian: true);
            string textBack = Encoding.UTF8.GetString(bytes);
            string textb = textBack;
            textBox2.Text = textb;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            string cvals = PromptForKeyAndcert.ShowDialog("Certificate passwords :",
                    "* keystore password", "* certificate password");
            string[] aval = cvals.Split([',']);
            ConfirmCert(aval[0], aval[1]);
            nocert = false;
            ViewProfile();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                ListViewItem item = listView1.SelectedItems[0];
                string uname = item.SubItems[1].Text;
                string urole = item.SubItems[3].Text;
                int selCode = int.Parse(item.Text);
                string sqlnonqry;
                // MessageBox.Show(selCode.ToString() +  " "+ urole);
                if (urole == "Local admin" && !uname.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
                {
                    sqlnonqry = "UPDATE configuration_local.sys_users SET sys_user_group_id = 3 WHERE id = " + selCode;
                }
                else
                {
                    sqlnonqry = "UPDATE configuration_local.sys_users SET sys_user_group_id = 9999 WHERE id = " + selCode;
                }
                DBTools dBTCount = new();
                dBTCount.MySqlExecuteNonQuery(sqlnonqry, Dbconn.con_str);
            }
            catch (Exception ex) { }
            ViewProfile();
        }


        public void DecScript()
        {
            //  if (scriptText is null)
            //  return "incompleteReq";


            string confilename = string.Empty;

            //    string fileContent = string.Empty;
            //    string filenme = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Text files (*.txt)|*.txt";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    confilename = openFileDialog.FileName;
                    textBox5.Text = confilename;
                }
                else { return; }
            }

            try
            {
                string[] lines = File.ReadAllLines(confilename);
                using var certificates = new CertificateManager();
                string scriptKey = certificates.Decrypt(
                    scrGenSecKey, privKeyFile);
                string scriptIv = certificates.Decrypt(
                    AppConfig.scriptGeneratorIniVec, privKeyFile);

                string decryptedScript = "";
                try
                {
                    decryptedScript = new Crypto().RijndaelDecrypt(
                        lines[0], scriptKey, scriptIv, 256);
                }
                catch (InvalidCipherTextException)
                {
                    MessageBox.Show("invalidfiletype");
                }

                string[] scriptLines = decryptedScript.TrimEnd('\r', '\n')
                    .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                textBox6.Text = string.Join(Environment.NewLine, scriptLines);
            }
            finally
            {
            }
        }

        public static void WriteToFile(string[] lines)
        {
            string outputPath = Path.Combine("C:\\tmp", "DecryptedScript.txt");

            using var writer = new StreamWriter(outputPath);
            for (int i = 0; i < lines.Length; i++)
            {
                writer.WriteLine($"Sql Query {i + 1} :  {lines[i]}");
                writer.WriteLine();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DecScript();

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    this.MinimumSize = new Size(1161, 593);//899, 306
                    this.Size = new Size(1161, 593);
                    this.FormBorderStyle = FormBorderStyle.Sizable;

                    break;
                case 1:
                    this.MinimumSize = new Size(903, 301);//903, 301
                    this.Size = new Size(903, 301);
                    this.FormBorderStyle = FormBorderStyle.Fixed3D;

                    break;
                case 2:
                  
                    Dbconn.Populate_List(Dbconn.sqlselprov, cmb_prov, "AREA", "PRV");
                    cmb_prov.SelectedIndex = 50;
                    this.MinimumSize = new Size(1161, 593);
                    this.Size = new Size(1161, 593);
                    this.FormBorderStyle = FormBorderStyle.Fixed3D;
                    break;
            }
        }

        private void textBox7_Enter(object sender, EventArgs e)
        {
            if (noinput == true)
                textBox7.Text = string.Empty;

        }

        private void button7_Click(object sender, EventArgs e) => CreateScript(textBox7.Text);


        public void CreateScript(string scriptContent)
        {
            if (scriptContent == null) return;
            using var certificates = new CertificateManager();
            string scriptKey = certificates.Decrypt(
                scrGenSecKey, privKeyFile);
            string scriptIv = certificates.Decrypt(
                AppConfig.scriptGeneratorIniVec, privKeyFile);

            string encryptedScript = new Crypto().RijndaelEncrypt(
                scriptContent, scriptKey, scriptIv, 256);

            string destination = "C:\\temp";
            Directory.CreateDirectory(destination);
            string scriptFile = Path.Combine(destination, "vrs-script.txt");

            try
            {
                File.WriteAllText(scriptFile, encryptedScript);
                MessageBox.Show("Script saved in c:\temp\vrs-script.txt");
            }
            catch (Exception)
            {
                // return "error";
            }

        }

        public static int GetDaysDiff(DateTime date1, DateTime date2)
        {
            return (int)Math.Ceiling(
                (date2 - date1).TotalMilliseconds / 86_400_000d);
        }

        public static void CreateExtendLocktimeScript(string province, string city, string erbDate, string endDate)
        {
            string prov = province;
            string mun = city;
            string provmun = prov + mun;
            //string d1 = erbDate;
            string d2 = endDate;
            // int diffDays = 0;

            DateTime date1 = DateTime.ParseExact(erbDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime date2 = DateTime.ParseExact(endDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            int diffDays = GetDaysDiff(date1, date2) + 1;

            string conditionprovmun = provmun.Equals("3908") ? "b.value='" + prov + "'"
                : "CONCAT(b.value,c.value) = '" + provmun + "'";
            string scriptdetails = "UPDATE configuration_local.config a CROSS JOIN (SELECT * from configuration_local.config where id ='1') b  CROSS JOIN (SELECT * from configuration_local.config where id ='2') c  SET a.VALUE='"
                    + 0.ToString() + "' WHERE a.id='39' and (" + conditionprovmun
                    + " or CONCAT(b.value,c.value) = '3900');";
            string destination = "C:\\tmp\\";

            DBTools dBTCount = new DBTools();

            string sql1 = "SELECT CONCAT(b.areaname,'-',a.area) FROM ref.allprov a LEFT JOIN ref.allmun b ON a.prv = b.prv WHERE b.id_city = " + provmun;
            string loc = dBTCount.MySqlFetchScalar(sql1, Dbconn.con_str).ToString();
            string filename = loc + "-LOCKDOWN_EXTENDER_UPTO-" + d2;
            string scriptFile = destination + filename + ".txt";

            if (File.Exists(scriptFile))
            {
                File.Delete(scriptFile);
            }
            using var certificates = new CertificateManager();

            string scriptKey = certificates.Decrypt(AppConfig.scriptGeneratorSecKey, AppConfig.privateKeyFile);
            string scriptIv = certificates.Decrypt(AppConfig.scriptGeneratorIniVec, AppConfig.privateKeyFile);

            string encryptedScript = new Crypto().RijndaelEncrypt(scriptdetails, scriptKey, scriptIv, 256);

            try
            {
                File.WriteAllText(scriptFile, encryptedScript);
            }
            catch (IOException)
            {
                //  return "error";
            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            CreateExtendLocktimeScript(cmb_prov.SelectedValue.ToString(), cmb_mun.SelectedValue.ToString(), textBox8.Text, textBox9.Text);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems == null)
            {
                MessageBox.Show("Please select a user to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ListViewItem item = listView1.SelectedItems[0];
            string uname = item.SubItems[1].Text;
            string urole = item.SubItems[3].Text;
            int selCode = int.Parse(item.Text);
            string sqlnonqry;
            // MessageBox.Show(selCode.ToString() +  " "+ urole);
            if (uname.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
            {
                DialogResult res = MessageBox.Show("Are you sure you want to delete admin account?", "Confirmation",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (res == DialogResult.OK)
                {
                    DBTools dBTCount = new();
                    sqlnonqry = "DELETE FROM configuration_local.sys_users WHERE id = " + selCode;
                    dBTCount.MySqlExecuteNonQuery(sqlnonqry, Dbconn.con_str);
                }
            }
            else
            {
                DialogResult res = MessageBox.Show("Are you sure you want to delete user?", "Confirmation",
                   MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (res == DialogResult.OK)
                {
                    DBTools dBTCount = new();
                    sqlnonqry = "DELETE FROM configuration_local.sys_users WHERE id = " + selCode;
                    dBTCount.MySqlExecuteNonQuery(sqlnonqry, Dbconn.con_str);
                }
            }


            ViewProfile();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedIndices.Count > 0)
                {
                    int selitem = listView1.SelectedItems[0].Index + 1;

                    pictureBox1.Image = Image.FromFile(selitem + "-" + mod_Image.FingerPrintImageFilename);
                }
            }
            catch
            {
            }
        }
        /*   private void getImages()
          {
              ms = new MemoryStream();
              string mySelectQuery = "select * from personnel.employees where emp_id is null";
              try
              {
                  PictureBox1.Image.Save(ms, ImageFormat.Bmp);
              }
              catch (Exception ex)
              {
                  MessageBox.Show(ex.Message.ToString(), "Null", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                  return;
              }
              try
              {
                  if (fp1.Image != null)
                  {
                      fpByte1 = File.ReadAllBytes("1_fingerPrintImage.wsq");
                  }
                  if (fp2.Image != null)
                  {
                      fpByte2 = File.ReadAllBytes("2_fingerPrintImage.wsq");
                  }
              }
              catch (Exception ex)
              {
                  MessageBox.Show(ex.Message.ToString());
                  return;
              }
              Mod_rao.captured = false;
              AddRecord(Mod_rao.rao_con_str, mySelectQuery, "table1");
          }*/


        /*     private DataSet AddRecord(string myConnection, string mySelectQuery, string myTableName)
             {
                 MySqlConnection mySqlConnection = new MySqlConnection(myConnection);
                 MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter()
                 {
                     SelectCommand = new MySqlCommand(mySelectQuery, mySqlConnection)
                 };
                 MySqlCommandBuilder mySqlCommandBuilder = new MySqlCommandBuilder(mySqlDataAdapter);
                 mySqlConnection.Open();
                 DataSet dataSet = new DataSet();
                 Encryption encryption = new Encryption();
                 DataSet result;
                 try
                 {
                     mySqlDataAdapter.Fill(dataSet, myTableName);
                     DataRow dataRow = dataSet.Tables[myTableName].NewRow();
                     dataRow["LASTNAME"] = encryption.CreateEncryptedString(tbx_lastname.Text.Replace("'", "''"));
                     dataRow["FIRSTNAME"] = encryption.CreateEncryptedString(tbx_firstname.Text.Replace("'", "''"));
                     dataRow["MATERNALNAME"] = encryption.CreateEncryptedString(tbx_maternalname.Text.Replace("'", "''"));
                     dataRow["EMP_ID"] = tbx_empid.Text.Replace("'", "''");
                     dataRow["DESIGNATION"] = RuntimeHelpers.GetObjectValue(cmb_desig.SelectedValue);
                     dataRow["DEPT_PROV"] = RuntimeHelpers.GetObjectValue(cmb_prov.SelectedValue);
                     dataRow["DIV_MUN"] = RuntimeHelpers.GetObjectValue(cmb_mun.SelectedValue);
                     dataRow["SALARY"] = Convert.ToDouble(tbx_salary.Text);
                     dataRow["SALGRADE"] = lbl_sg.Text;
                     dataRow["TAX"] = tbx_tax.Text;
                     //dataRow["SCHED_FROM"] = Strings.Left(cmb_sch_from.Text, checked(cmb_sch_from.Text.IndexOf("-") - 1));
                     //dataRow["SCHED_FROM"] = cmb_sch_from.Text.Substring(0, checked(cmb_sch_from.Text.IndexOf("-") - 1));
                     dataRow["SCHED_FROM"] = cmb_sch_from.Text.Substring(0, checked(cmb_sch_from.Text.IndexOf('-') - 1));
                     dataRow["SCHED_TO"] = RuntimeHelpers.GetObjectValue(cmb_sch_from.SelectedValue);
                     dataRow["MAX_OT"] = cmb_sch_OT.Text;
                     dataRow["FP1"] = fpByte1;
                     dataRow["FP2"] = fpByte2;
                     dataRow["PICTURE"] = encryption.EncryptByte(ms.ToArray());
                     dataSet.Tables["Table1"].Rows.Add(dataRow);
                     mySqlDataAdapter.Update(dataSet, myTableName);
                     mySqlDataAdapter.Dispose();
                     mySqlConnection.Close();
                     MessageBox.Show("Record Successfully Added", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     ClearInputs();
                     ms.Dispose();
                     ms.Close();
                     count = 0;
                     result = dataSet;
                 }
                 catch (Exception ex)
                 {
                     if (string.Compare(ex.Message.ToString().Substring(0, 6), "#23000", StringComparison.OrdinalIgnoreCase) == 0)
                     {
                         MessageBox.Show("Record Successfully Added", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                         ms.Dispose();
                         ms.Close();
                         mySqlDataAdapter.Dispose();
                         mySqlConnection.Close();
                         count = 0;
                     }
                     else
                     {
                         MessageBox.Show(ex.Message.ToString(), "Enroll", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                         ms.Dispose();
                         ms.Close();
                         mySqlDataAdapter.Dispose();
                         mySqlConnection.Close();
                     }
                     result = dataSet;
                 }
                 return result;
             }*/







    }


}
