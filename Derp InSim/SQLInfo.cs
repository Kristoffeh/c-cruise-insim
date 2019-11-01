using InSimDotNet.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace Derp_InSim
{
    public partial class SQLInfo
    {
        const string TimeFormat = "dd/MM/yyyy";//ex: 23/03/2003
        const string TimeFormatTwo = "dd/MM/yyyy HH:mm";//ex: 23/03/2003
        MySqlConnection SQL = new MySqlConnection();
        public SQLInfo() { }

        public bool IsConnectionStillAlive()
        {
            try
            {
                if (SQL.State == System.Data.ConnectionState.Open) return true;
                else return false;
            }
            catch { return false; }
        }
        
        // Load the database
        public bool StartUp(string server, string database, string username, string password)
        {
            try
            {
                if (IsConnectionStillAlive()) return true;

                SQL.ConnectionString = "Server=" + server +
                    ";Database=" + database +
                    ";Uid=" + username +
                    ";Pwd=" + password +
                    ";Connect Timeout=10;";
                SQL.Open();

                // Users table
                Query("CREATE TABLE IF NOT EXISTS users(PRIMARY KEY(username),username CHAR(25) NOT NULL,playername CHAR(30) NOT NULL,cash int(10),bankbalance int(10),totaldistance decimal(11),cars CHAR(90),regdate CHAR(16) NOT NULL,lastseen CHAR(16),totaljobsdone int(10), totalearnedfromjobs int(10), timezone int(5), kmhormph int(5), totalconnectiontime(16));");

                // Banlist table
                Query("CREATE TABLE IF NOT EXISTS banlist(PRIMARY KEY(username),username CHAR(25) NOT NULL, playername CHAR(40) NOT NULL,bandate CHAR(10), banreason CHAR(75));");
            }
            catch { return false; }
            return true;
        }

        // Load query
        public int Query(string str)
        {
            try
            {
                MySqlCommand query = new MySqlCommand();
                query.Connection = SQL;
                query.CommandText = str;
                query.Prepare();
                return query.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                if (System.IO.File.Exists("files/sqlerror.log") == false) { FileStream CurrentFile = System.IO.File.Create("files/sqlerror.log"); CurrentFile.Close(); }

                StreamReader TextTempData = new StreamReader("files/sqlerror.log");
                string TempText = TextTempData.ReadToEnd();
                TextTempData.Close();

                StreamWriter TextData = new StreamWriter("files/sqlerror.log");
                TextData.WriteLine(TempText + DateTime.Now + ": Query Error - Exception: " + e);
                TextData.Flush();
                TextData.Close();
                return 0;
            }
        }

        #region Player Saving Stuff
        // Exist in database
        public bool UserExist(string username, string table = "users")
        {
            MySqlCommand query = new MySqlCommand();
            query.Connection = SQL;
            query.CommandText = "SELECT username FROM " + table + " WHERE username='" + username + "' LIMIT 1;";
            query.Prepare();
            MySqlDataReader dr = query.ExecuteReader();

            bool found = false;

            if (dr.Read()) if (dr.GetString(0) != "") found = true;
            dr.Close();

            return found;
        }


        // Users db count
        public int userCount()
        {
            MySqlCommand query = new MySqlCommand();
            query.Connection = SQL;
            query.CommandText = "SELECT COUNT(*) FROM users";
            query.Prepare();
            MySqlDataReader dr = query.ExecuteReader();

            if (dr.Read())
                if (dr.GetString(0) != "")
                    dr.Close();

            return Convert.ToInt32(query.ExecuteScalar());
        }

        // Banned users count
        public int bansCount()
        {
            MySqlCommand query = new MySqlCommand();
            query.Connection = SQL;
            query.CommandText = "SELECT COUNT(*) FROM banlist";
            query.Prepare();
            MySqlDataReader dr = query.ExecuteReader();

            if (dr.Read())
                if (dr.GetString(0) != "")
                    dr.Close();

            return Convert.ToInt32(query.ExecuteScalar());
        }

        // Add user to database
        public void AddUser(string username, string playername, long cash, long bankbalance, decimal totaldistance, string cars, long totaljobsdone, long totalearnedfromjobs, int timezone, int KMHorMPH, int totalconnectiontime)
        {
            try
            {
                if (username == "") return;
                Query("INSERT INTO users VALUES ('" + username + "', '" + StringHelper.StripColors(RemoveStupidCharacters(playername)) +  "', " + cash + ", " + bankbalance + ", " + totaldistance + ", '" + cars + "', '" +
                    DateTime.UtcNow.ToString(TimeFormat) + "', '" + DateTime.UtcNow.ToString(TimeFormat) + "', " + totaljobsdone + ", " + totalearnedfromjobs + ", " + timezone + ", " + KMHorMPH + ", " + totalconnectiontime + ");");
            }
            catch
            {

            }
        }
        // Add user to banlist
        public void AddtoBanlist(string username, string playername, string bandate, string banreason)
        {
            if (username == "") return;
            Query("INSERT INTO banlist VALUES ('" + username + "', '" + RemoveStupidCharacters(playername) + "', '" + bandate + "', '" + banreason + "');");
        }

        public void UpdateUser(string username, string playername, bool updatejointime, int cash = 0, int bankbalance = 0, decimal totaldistance = 0, string cars = "UF1 XFG XRG", int totaljobsdone = 0, int totalearnedfromjobs = 0, int timezone = 0, int KMHorMPH = 0, int TotalConnectionTime = 0)
        {
            if (updatejointime) Query("UPDATE users SET lastseen='" + DateTime.UtcNow.ToString(TimeFormat) + "' WHERE username='" + username + "';");
            else Query("UPDATE users SET playername='" + StringHelper.StripColors(RemoveStupidCharacters(playername)) + "', cash=" + cash + ", bankbalance=" + bankbalance + ", totaldistance=" + totaldistance + ", cars='" + cars + "', lastseen='" + DateTime.UtcNow.ToString(TimeFormat) + "', totaljobsdone=" + totaljobsdone + ", totalearnedfromjobs=" + totalearnedfromjobs + ", timezone=" + timezone + ", kmhormph=" + KMHorMPH + ", totalconnectiontime=" + TotalConnectionTime + " WHERE username='" + username + "';");
        }

        // Load their options
        public string[] LoadUserOptions(string username)
        {
            string[] options = new string[11];

            MySqlCommand query = new MySqlCommand();
            query.Connection = SQL;
            query.CommandText = "SELECT cash, bankbalance, totaldistance, cars, regdate, lastseen, totaljobsdone, totalearnedfromjobs, timezone, kmhormph, totalconnectiontime FROM users WHERE username='" + username + "' LIMIT 1;";
            query.Prepare();
            MySqlDataReader dr = query.ExecuteReader();

            if (dr.Read())
                if (dr.GetString(0) != "")
                {
                    options[0] = dr.GetString(0);
                    options[1] = dr.GetString(1);
                    options[2] = dr.GetString(2);
                    options[3] = dr.GetString(3);
                    options[4] = dr.GetString(4);
                    options[5] = dr.GetString(5);
                    options[6] = dr.GetString(6);
                    options[7] = dr.GetString(7);
                    options[8] = dr.GetString(8);
                    options[9] = dr.GetString(9);
                    options[10] = dr.GetString(10);
                }
            dr.Close();

            return options;
        }
        public string RemoveStupidCharacters(string text)
        {
            if (text.Contains("'")) text = text.Replace('\'', '`');
            if (text.Contains("‘")) text = text.Replace('‘', '`');
            if (text.Contains("’")) text = text.Replace('’', '`');
            if (text.Contains("^h")) text = text.Replace("^h", "#");

            return text;
        }
        #endregion
    }
}
