using System;
using InSimDotNet.Helpers;
using InSimDotNet.Packets;

namespace Derp_InSim
{
    public partial class Form1
    {
        private void BTC_ClientClickedButton(IS_BTC BTC)
        {
            var conn = _connections[BTC.UCID];

            try
            {
                // DELETE a button:

                // deleteBtn(Ucid, Reqi, true, ClickID);

                // deleteBtn(BTC.UCID, BTC.ReqI, true, 6);
                {

                    switch (BTC.ClickID)
                    {
                        case 32:
                            HomeScreen(BTC.UCID);
                            break;

                        case 44:

                            #region ' content '
                            if (conn.DisplaysOpen == true)
                            {
                                conn.serverTime = false;
                                conn.DisplaysOpen = false;
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 30);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 31);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 32);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 33);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 34);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 35);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 36);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 37);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 38);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 39);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 40);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 41);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 42);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 43);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 44);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 45);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 46);

                            }
                            #endregion
                            break;

                        // Server Stats
                        case 33:

                            #region ' content '
                            // DARK
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 30, ClickID = 30, BStyle = ButtonStyles.ISB_DARK, H = 44, W = 65, T = 38, L = 66, Text = "" }); // H = 52

                            // Title
                            // string text = "^3European Cruise";
                            // insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 31, ClickID = 31, BStyle = ButtonStyles.ISB_LIGHT, H = 7, W = 61, T = 40, L = 68, Text = text });

                            #region ' Sections '
                            // Home
                            string text2 = "^8Home";
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 32, ClickID = 32, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 51, L = 69, Text = text2 });

                            // Server Stats
                            string text3 = "^2Server Stats";
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 33, ClickID = 33, BStyle = ButtonStyles.ISB_LIGHT, H = 4, W = 13, T = 55, L = 69, Text = text3 });

                            // Your Stats
                            string text4 = "^8Your Stats";
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 34, ClickID = 34, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 59, L = 69, Text = text4 });

                            // Commands
                            string text5 = "^8Commands";
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 35, ClickID = 35, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 63, L = 69, Text = text5 });

                            // About
                            string text6 = "^8About";
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 36, ClickID = 36, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 67, L = 69, Text = text6 });
                            #endregion

                            #region ' Lines '
                            // Lines
                            string line1 = "^7Current Track : ^3" + TrackHelper.GetFullTrackName(TrackName) + " ^7(" + TrackName + ")";
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 37, ClickID = 37, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 51, L = 83, Text = line1 });

                            string line2 = "^7Connections : ^3" + (_connections.Count - 1);
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 38, ClickID = 38, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 55, L = 83, Text = line2 });

                            string line3 = "^7Players Spectating : ^3" + (_connections.Count - _players.Count - 1);
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 39, ClickID = 39, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 59, L = 83, Text = line3 });

                            string line4 = "^7Players Driving : ^3" + (_players.Count);
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 40, ClickID = 40, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 63, L = 83, Text = line4 });

                            string line5 = "^7Registered Users : ^3" + dbCount;
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 41, ClickID = 41, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 67, L = 83, Text = line5 });

                            string line6 = "^7Permanently Banned Users : ^3" + dbBans;
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 42, ClickID = 42, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 71, L = 83, Text = line6 });

                            TimeSpan span = TimeSpan.FromSeconds(TotalUptime);
                            string hrs = span.ToString(@"hh\:mm\:ss");

                            string line7 = "^7InSim Uptime : ^3" + hrs;
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 43, ClickID = 43, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 75, L = 83, Text = line7 });

                            string line8 = "";
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 46, ClickID = 46, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 79, L = 83, Text = line8 });

                            string line9 = "";
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 45, ClickID = 45, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 83, L = 83, Text = line9 });
                            #endregion

                            #endregion

                            break;
                        // Your Stats
                        case 34:

                            #region ' content '
                            // DARK
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 30, ClickID = 30, BStyle = ButtonStyles.ISB_DARK, H = 48, W = 65, T = 38, L = 66, Text = "" }); // H = 52

                            // Title
                            // string text = "^3European Cruise";
                            // insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 31, ClickID = 31, BStyle = ButtonStyles.ISB_LIGHT, H = 7, W = 61, T = 40, L = 68, Text = text });

                            #region ' Sections '
                            // Home
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 32, ClickID = 32, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 51, L = 69, Text = "^8Home" });

                            // Server Stats
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 33, ClickID = 33, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 55, L = 69, Text = "^8Server Stats" });

                            // Your Stats
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 34, ClickID = 34, BStyle = ButtonStyles.ISB_LIGHT, H = 4, W = 13, T = 59, L = 69, Text = "^2Your Stats" });

                            // Commands
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 35, ClickID = 35, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 63, L = 69, Text = "^8Commands" });

                            // About
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 36, ClickID = 36, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 67, L = 69, Text = "^8About" });
                            #endregion

                            #region ' Lines '
                            // Lines
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 37, ClickID = 37, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 51, L = 83, Text = "^7User : " + conn.PName + " ^7(" + conn.UName + ")" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 38, ClickID = 38, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 55, L = 83, Text = "^7Cash : ^3€" + string.Format("{0:n0}", conn.cash) });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 39, ClickID = 39, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 59, L = 83, Text = "^7Bankbalance : ^3€" + string.Format("{0:n0}", conn.bankbalance) });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 40, ClickID = 40, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 63, L = 83, Text = "^7Driven Distance : ^3" + string.Format("{0:0.0}", conn.TotalDistance / 1000) + " km/" + string.Format("{0:0.0}", conn.TotalDistance / 1609) + " mi" });

                            TimeSpan span1 = TimeSpan.FromSeconds(conn.TotalConnectionTime);
                            string hrs1 = span1.ToString(@"hh\:mm\:ss");
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 41, ClickID = 41, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 67, L = 83, Text = "^7Time Spent : ^3" + hrs1 });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 42, ClickID = 42, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 71, L = 83, Text = "^7Jobs Completed : ^3" + string.Format("{0:n0}", conn.totaljobsdone) });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 43, ClickID = 43, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 75, L = 83, Text = "^7Current Bonus : ^3" + string.Format("{0:0.0}", conn.BonusDistance / 1000) + " km/" + string.Format("{0:0.0}", conn.BonusDistance / 1609) + " mi" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 46, ClickID = 46, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 79, L = 83, Text = "^7Registered : ^3" + conn.regdate });
                            
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 45, ClickID = 45, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 83, L = 83, Text = "" });
                            #endregion

                            #endregion

                            break;
                        
                        // Commands
                        case 35:

                            #region ' content '
                            // DARK
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 30, ClickID = 30, BStyle = ButtonStyles.ISB_DARK, H = 37, W = 65, T = 38, L = 66, Text = "" }); // H = 52

                            // Title
                            // string text = "^3European Cruise";
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 31, ClickID = 31, BStyle = ButtonStyles.ISB_LIGHT, H = 7, W = 61, T = 40, L = 68, Text = "^3InSim Commands" });

                            #region ' Sections '
                            // Home
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 32, ClickID = 32, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 51, L = 69, Text = "^8Home" });

                            // Server Stats
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 33, ClickID = 33, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 55, L = 69, Text = "^8Server Stats" });

                            // Your Stats
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 34, ClickID = 34, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 59, L = 69, Text = "^8Your Stats" });

                            // Commands
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 35, ClickID = 35, BStyle = ButtonStyles.ISB_LIGHT, H = 4, W = 13, T = 63, L = 69, Text = "^2Commands" });

                            // About
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 36, ClickID = 36, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 67, L = 69, Text = "^8About" });
                            #endregion

                            #region ' Lines '
                            // Lines
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 37, ClickID = 37, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 51, L = 83, Text = "^7!help -^2 Will show you this menu." });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 38, ClickID = 38, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 55, L = 83, Text = "^7!info -^2 Will show you a bit of info about the server." });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 39, ClickID = 39, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 59, L = 83, Text = "^7!pen (!p) -^2 Will show your stats to everyone." });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 40, ClickID = 40, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 63, L = 83, Text = "^7!gmt <-12 to +12> -^2 Will select your current timezone." });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 41, ClickID = 41, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 67, L = 83, Text = "" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 42, ClickID = 42, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 71, L = 83, Text = "" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 43, ClickID = 43, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 75, L = 83, Text = "" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 46, ClickID = 46, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 79, L = 83, Text = "" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 45, ClickID = 45, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 83, L = 83, Text = "" });
                            #endregion

                            #endregion

                            break;
                        // About
                        case 36:

                            #region ' content '
                            // DARK
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 30, ClickID = 30, BStyle = ButtonStyles.ISB_DARK, H = 37, W = 65, T = 38, L = 66, Text = "" }); // H = 52

                            // Title
                            // string text = "^3European Cruise";
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 31, ClickID = 31, BStyle = ButtonStyles.ISB_LIGHT, H = 7, W = 61, T = 40, L = 68, Text = "^3About Us" });

                            #region ' Sections '
                            // Home
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 32, ClickID = 32, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 51, L = 69, Text = "^8Home" });

                            // Server Stats
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 33, ClickID = 33, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 55, L = 69, Text = "^8Server Stats" });

                            // Your Stats
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 34, ClickID = 34, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 59, L = 69, Text = "^8Your Stats" });

                            // Commands
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 35, ClickID = 35, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 63, L = 69, Text = "^8Commands" });

                            // About
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 36, ClickID = 36, BStyle = ButtonStyles.ISB_LIGHT, H = 4, W = 13, T = 67, L = 69, Text = "^2About" });
                            #endregion

                            #region ' Lines '
                            // Lines
                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 37, ClickID = 37, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 51, L = 83, Text = "^7InSim Author : Kriss ^1N^7O^4R ^7(kristofferandersen)" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 38, ClickID = 38, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 55, L = 83, Text = "^7InSim Version : ^5EUC^7™ InSim V" + InSimVersion });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 39, ClickID = 39, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 59, L = 83, Text = "^7Layout Creator : ^1EMPTY" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 40, ClickID = 40, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 63, L = 83, Text = "^7Layout Version : ^1EMPTY" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 41, ClickID = 41, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 67, L = 83, Text = "^7Our Teamspeak : ^3ts.eugaming.org" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 42, ClickID = 42, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 71, L = 83, Text = "" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 43, ClickID = 43, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 75, L = 83, Text = "" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 46, ClickID = 46, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 79, L = 83, Text = "" });

                            insim.Send(new IS_BTN { UCID = BTC.UCID, ReqI = 45, ClickID = 45, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 83, L = 83, Text = "" });
                            #endregion

                            #endregion

                            break;

                        case 100:
                            
                            foreach (var c in _connections.Values)
                            {
                                // Sender
                                if (BTC.UCID == c.UCID)
                                {
                                    var Sender = BTC.UCID;

                                    _connections[Sender].cash -= _connections[Sender].SendAmount;
                                    insim.Send(255, "^8You have sent ^2€" + _connections[BTC.UCID].SendAmount + " ^8to " + _connections[_connections[BTC.UCID].SenderUCID].PName);

                                }

                                // Receiver
                                if (c.UCID == 9)
                                {
                                    var Receiver = c.ReceiverUCID1;

                                    _connections[Receiver].cash += _connections[Receiver].SendAmount;
                                    insim.Send(255, "^8You have received ^2€" + _connections[BTC.UCID].SendAmount + " ^8from " + _connections[BTC.UCID].PName);

                                }

                                _connections[BTC.UCID].SendAmount = 0;
                            }

                            break;

                        case 101:



                            break;
                    }
                }
            }
            catch (Exception e) { LogTextToFile("error", "[" + BTC.UCID + "] " + StringHelper.StripColors(_connections[BTC.UCID].PName) + "(" + _connections[BTC.UCID].UName + ") - BTC - Exception: " + e, false); }
        }
    }
}
