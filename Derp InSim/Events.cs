using System;
using System.Threading;
using InSimDotNet.Packets;
using System.Globalization;
using System.Windows.Forms;
using InSimDotNet.Helpers;

namespace Derp_InSim
{
    public partial class Form1
    {
        System.Timers.Timer SQLReconnectTimer = new System.Timers.Timer();
        System.Timers.Timer SaveTimer = new System.Timers.Timer();

        public void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

                #region ' Timer '
                System.Timers.Timer Payout = new System.Timers.Timer();
                Payout.Elapsed += new System.Timers.ElapsedEventHandler(Payout_Timer);
                Payout.Interval = 3000;
                Payout.Enabled = true;

                // SQL timer
                SQLReconnectTimer.Interval = 10000;
                SQLReconnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(SQLReconnectTimer_Elapsed);

                // Save timer
                SaveTimer.Interval = 2000;
                SaveTimer.Elapsed += new System.Timers.ElapsedEventHandler(Savetimer_Elapsed);
                SaveTimer.Enabled = true;

                System.Timers.Timer SecondTimer = new System.Timers.Timer();
                SecondTimer.Elapsed += new System.Timers.ElapsedEventHandler(SecondTimer_Timer);
                SecondTimer.Interval = 1000;
                SecondTimer.Enabled = true;
                #endregion
            }
            catch (Exception error)
            {

                {
                    MessageBox.Show("" + error.Message, "AN ERROR OCCURED");
                }
            }
        }

        private void SecondTimer_Timer(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                foreach (var Conn in _connections.Values)
                {
                    {
                        Conn.TotalConnectionTime += 1;
                        TotalUptime += 1;


                        #region ' Bonus Distance '

                        int prize1 = 450;
                        int prize2 = 750;
                        int prize3 = 1000;
                        int prize4 = 1250;
                        int prize5 = 1750;
                        int prize6 = 2000;
                        int prize7 = 3000;
                        int prize8 = 4000;
                        int prize9 = 5700;
                        int prize10 = 6000;
                        int prize11 = 7200;

                        int dist1 = 1000;
                        int dist2 = 2000;
                        int dist3 = 3000;
                        int dist4 = 4000;
                        int dist5 = 5000;
                        int dist6 = 6000;
                        int dist7 = 7000;
                        int dist8 = 8000;
                        int dist9 = 9000;
                        int dist10 = 10000;
                        int dist11 = 11000;

                        if (Conn.BonusDistance == 100 && Conn.TotalBonusDone == 0)
                        {
                            // insim.Send(255, Conn.PName + " ^3reached ^7" + string.Format("{0:0.0}", Conn.BonusDistance / 1000) + " km/" + string.Format("{0:0.0}", Conn.BonusDistance / 1601) + " mi ^3distance bonus!");
                            insim.Send(255, Conn.PName + " ^3reached ^70.1 km ^3distance bonus!");
                            insim.Send(255, Conn.PName + " ^3earned ^2€" + prize1);
                            Conn.TotalBonusDone += 1;
                        }
                        else if (Conn.BonusDistance == 200 && Conn.TotalBonusDone == 0)
                        {
                            // insim.Send(255, Conn.PName + " ^3reached ^7" + string.Format("{0:0.0}", Conn.BonusDistance / 1000) + " km/" + string.Format("{0:0.0}", Conn.BonusDistance / 1601) + " mi ^3distance bonus!");
                            insim.Send(255, Conn.PName + " ^3reached ^70.1 km ^3distance bonus!");
                            insim.Send(255, Conn.PName + " ^3earned ^2€" + prize2);
                            Conn.TotalBonusDone += 1;
                        }


                        #endregion
                    }
                }
            }
            catch (Exception error)
            {

                {
                    MessageBox.Show("" + error.Message, "AN ERROR OCCURED");
                }
            }
        }

        private void Payout_Timer(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                {
                    foreach (var Conn in _players.Values)
                    {
                        var CurrentConnection = GetConnection(Conn.PLID);

                        if ((Conn.PName == HostName && Conn.UCID == 0) == false)
                        {
                            if (Conn.CName == "UF1" || Conn.CName == "XFG" || Conn.CName == "XRG" || Conn.CName == "MRT")
                            {
                                if (Conn.kmh >= 30 && Conn.kmh <= 99)
                                {
                                    CurrentConnection.cash += 1;
                                }
                                else if (Conn.kmh >= 100 && Conn.kmh <= 149)
                                {
                                    CurrentConnection.cash += 2;
                                }
                                else if (Conn.kmh > 150)
                                {
                                    CurrentConnection.cash += 3;
                                }
                            }
                            else if (Conn.CName == "LX4" || Conn.CName == "LX6" || Conn.CName == "RB4" || Conn.CName == "FXO" || Conn.CName == "XRT" || Conn.CName == "RAC" || Conn.CName == "FZ5")
                            {
                                if (Conn.kmh >= 30 && Conn.kmh <= 89)
                                {
                                    CurrentConnection.cash += 1;
                                }
                                else if (Conn.kmh >= 90 && Conn.kmh <= 179)
                                {
                                    CurrentConnection.cash += 2;
                                }
                                else if (Conn.kmh > 180)
                                {
                                    CurrentConnection.cash += 3;
                                }
                            }
                            else if (Conn.CName == "UFR" || Conn.CName == "XFR" || Conn.CName == "FXR" || Conn.CName == "XRR" || Conn.CName == "FZR" || Conn.CName == "MRT" || Conn.CName == "FBM" || Conn.CName == "FOX")
                            {
                                if (Conn.kmh >= 30 && Conn.kmh <= 89)
                                {
                                    CurrentConnection.cash += 1;
                                }
                                else if (Conn.kmh >= 90 && Conn.kmh <= 179)
                                {
                                    CurrentConnection.cash += 2;
                                }
                                else if (Conn.kmh > 180)
                                {
                                    CurrentConnection.cash += 3;
                                }
                            }

                            UpdateGui(Conn.UCID, true, false);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                
                {
                    MessageBox.Show("" + error.Message, "AN ERROR OCCURED");
                }
            }
        }

        private void SQLReconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            {
                SQLRetries++;
                ConnectedToSQL = SqlInfo.StartUp(SQLIPAddress, SQLDatabase, SQLUsername, SQLPassword);
                if (!ConnectedToSQL)
                {
                    SQL_label.Text = "MySQL : NOT CONNECTED!";
                    MessageToAdmins("SQL connect attempt failed! Attempting to reconnect in ^310 ^8seconds!");
                }
                else
                {
                    SQL_label.Text = "MySQL : CONNECTED!";
                    MessageToAdmins("SQL connected after ^3" + SQLRetries + " ^8times!");
                    SQLRetries = 0;
                    SQLReconnectTimer.Stop();
                }
            }
        }

        private void Savetimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                foreach (var conn in _connections.Values)
                {
                    if (ConnectedToSQL)
                    {
                        try { SqlInfo.UpdateUser(_connections[conn.UCID].UName, StringHelper.StripColors(SqlInfo.RemoveStupidCharacters(_connections[conn.UCID].PName)), false, _connections[conn.UCID].cash, _connections[conn.UCID].bankbalance, _connections[conn.UCID].TotalDistance, _connections[conn.UCID].cars, _connections[conn.UCID].totaljobsdone, _connections[conn.UCID].totalearnedfromjobs, _connections[conn.UCID].Timezone, _connections[conn.UCID].KMHorMPH, _connections[conn.UCID].TotalConnectionTime); }
                        catch (Exception EX)
                        {
                            if (!SqlInfo.IsConnectionStillAlive())
                            {
                                SQL_label.Text = "MySQL : NOT CONNECTED!";
                                ConnectedToSQL = false;
                                SQLReconnectTimer.Start();
                            }
                            LogTextToFile("sqlerror", "[" + conn.UCID + "] " + StringHelper.StripColors(_connections[conn.UCID].PName) + "(" + _connections[conn.UCID].UName + ") conn - Exception: " + EX.Message, false);
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show("" + f.Message, "AN ERROR OCCURED");
            }
        }
    }
}
