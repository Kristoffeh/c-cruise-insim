using System;
using System.Windows.Forms;
using InSimDotNet;
using InSimDotNet.Packets;
using InSimDotNet.Helpers;

namespace Derp_InSim
{
    public partial class Form1
    {
        private void MessageReceived(InSim insim, IS_MSO mso)
        {
            try
            {

                const string TimeFormat = "HH:mm";//ex: 23/03/2003
                {
                    chatbox.AppendText(StringHelper.StripColors(mso.Msg.ToString()) + " \r\n");

                    if (mso.UserType == UserType.MSO_PREFIX)
                    {
                        string Text = mso.Msg.Substring(mso.TextStart, (mso.Msg.Length - mso.TextStart));
                        string[] command = Text.Split(' ');
                        command[0] = command[0].ToLower();

                        switch (command[0])
                        {
                            case "!test":

                                insim.Send(255, _connections[mso.UCID].PName + " ^8is a faggot.");

                                break;

                            case "!help":
                                _connections[mso.UCID].serverTime = true;

                                if (_connections[mso.UCID].DisplaysOpen == false)
                                {
                                    HomeScreen(mso.UCID);
                                    _connections[mso.UCID].DisplaysOpen = true;
                                }
                                else
                                {
                                    insim.Send(255, DisplaysOpenedMsg);
                                }
                                break;

                            case "!send":

                                if (command.Length > 1)
                                {
                                    if (Text.Contains("-") || Text.Contains("+"))
                                    {
                                        insim.Send(mso.UCID, "^1Invalid format. ^7Usage: ^3!send <amount>");
                                    }
                                    else
                                    {
                                        byte ButtonID1 = 150;
                                        byte LocationY = 15;

                                        _connections[mso.UCID].SendAmount = Convert.ToInt32(command[1]);
                                        insim.Send(mso.UCID, "^2€" + _connections[mso.UCID].SendAmount);
                                        
                                        foreach (var c in _connections.Values)
                                        {
                                            if (c.UName != "")
                                            {
                                                insim.Send(new IS_BTN { UCID = mso.UCID, ReqI = ButtonID1, ClickID = ButtonID1, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 20, T = LocationY, L = 69, Text = "^7" + c.PName + " ^3(" + c.UName + ") " + c.UCID });
                                            }

                                            LocationY += 4;
                                            ButtonID1++;
                                        }
                                    }
                                }
                                else
                                {
                                    insim.Send(mso.UCID, "^1Invalid command, ^3" + Text + " ^1did not work.");
                                }

                                break;

                            case "!ac":
                                {//Admin chat
                                    if (mso.UCID == _connections[mso.UCID].UCID)
                                    {
                                        if (!IsConnAdmin(_connections[mso.UCID]))
                                        {
                                            insim.Send(mso.UCID, 0, "You are not an admin");
                                            break;
                                        }
                                        if (command.Length == 1)
                                        {
                                            insim.Send(mso.UCID, 0, "^1Invalid command format. ^2Usage: ^7!ac <text>");
                                            break;
                                        }

                                        string atext = Text.Remove(0, command[0].Length + 1);

                                        foreach (var Conn in _connections.Values)
                                        {
                                            {
                                                if (IsConnAdmin(Conn) && Conn.UName != "")
                                                {
                                                    insim.Send(Conn.UCID, 0, "^3Admin chat: ^7" + _connections[mso.UCID].PName + " ^8(" + _connections[mso.UCID].UName + "):");
                                                    insim.Send(Conn.UCID, 0, "^7" + atext);
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                            case "!pos":

                                if (_connections[mso.UCID].IsAdmin == false)
                                {
                                    insim.Send(mso.UCID, "^1Error: ^7You are not an Admin!");
                                    break;
                                }
                                if (AskedPosition == true)
                                {
                                    insim.Send(mso.UCID, "^1Error: ^7Someone else already wants their position, please try again.");
                                    break;
                                }

                                //position @ MCI Packet
                                AskedPosUCID = mso.UCID;
                                AskedPosition = true;
                                break;

                            case "!teamspeak":
                            case "!ts":

                                insim.Send(mso.UCID, "^7Teamspeak 3 Server: ^3" + "ts.eugaming.org");

                                break;

                            case "!p":
                            case "!pen":
                            case "!penalty":

                                insim.Send("/p_clear " + _connections[mso.UCID].UName);

                                break;

                            case "!ban":
                                var k = _connections[mso.UCID];
                                insim.Send(255, "^1Added ^7" + k.PName + " ^1to the ban list!");
                                SqlInfo.AddtoBanlist(k.UName, StringHelper.StripColors(k.PName), "14.01.2016", "no longer welcome");
                                break;

                            case "!show":
                            case "!showoff":
                                foreach (var conn in _connections.Values)
                                {
                                    if (conn.UCID == mso.UCID)
                                    {
                                        if (conn.PName.EndsWith("s"))
                                        {
                                            insim.Send(255, conn.PName + "^8' cars: ^2" + _connections[mso.UCID].cars);
                                        }
                                        else
                                        {
                                            insim.Send(255, conn.PName + "^8's cars: ^2" + _connections[mso.UCID].cars);
                                        }

                                        insim.Send(255, conn.PName + " ^8has driven: ^2" + string.Format("{0:n0}", conn.TotalDistance / 1000) + " kms ^8- ^2" + string.Format("{0:n0}", conn.TotalDistance / 1609) + " mi");
                                        insim.Send(255, conn.PName + " ^8registered: ^2" + conn.regdate);

                                        TimeSpan span = TimeSpan.FromSeconds(conn.TotalConnectionTime);
                                        string hrs = span.ToString(@"hh\:mm\:ss");

                                        insim.Send(255, conn.PName + " ^8has played: ^2" + hrs);


                                        // insim.Send(255, "^6Showoff for ^7" + _connections[mso.UCID].PName + " ^6(^7" + _connections[mso.UCID].UName + "^6)");
                                        // insim.Send(255, "^6Cash: ^2€" + string.Format("{0:n0}", conn.cash));
                                        // insim.Send(255, "^6Bankbalance: ^2€" + string.Format("{0:n0}", conn.bankbalance));
                                        // insim.Send(255, "^6Total distance driven: ^7" + string.Format("{0:n0}", conn.TotalDistance / 1000) + " kms^6/^7" + string.Format("{0:n0}", conn.TotalDistance / 1609) + " mi");



                                        // insim.Send(255, "^6Jobs completed: ^7" + string.Format("{0:n0}", _connections[mso.UCID].totaljobsdone));
                                        // insim.Send(255, "^6Cash earned from jobs: ^2€" + string.Format("{0:n0}", _connections[mso.UCID].totalearnedfromjobs));
                                        // insim.Send(255, "^6Last seen: ^7" + _connections[mso.UCID].lastseen);
                                        // insim.Send(255, "^6Registration date: ^7" + _connections[mso.UCID].regdate);
                                    }
                                }
                                break;

                            case "!gmt":

                                var connn = _connections[mso.UCID];
                                if (command.Length == 1)
                                {
                                    insim.Send(mso.UCID, "^1Invalid command. ^7Usage: ^3!gmt <timezone>");
                                }
                                else if (command.Length == 2)
                                {
                                    #region ' Command '
                                    if (command[1] == "-12" || command[1] == "-12:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -12;
                                    }
                                    else if (command[1] == "-11" || command[1] == "-11:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -11;
                                    }
                                    else if (command[1] == "-10" || command[1] == "-10:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -10;
                                    }
                                    else if (command[1] == "-9" || command[1] == "-9:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -9;
                                    }
                                    else if (command[1] == "-8" || command[1] == "-8:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -8;
                                    }
                                    else if (command[1] == "-7" || command[1] == "-7:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -7;
                                    }
                                    else if (command[1] == "-6" || command[1] == "-6:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -6;
                                    }
                                    else if (command[1] == "-5" || command[1] == "-5:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -5;
                                    }
                                    else if (command[1] == "-4" || command[1] == "-4:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -4;
                                    }
                                    else if (command[1] == "-3" || command[1] == "-3:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -3;
                                    }
                                    else if (command[1] == "-2" || command[1] == "-2:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -2;
                                    }
                                    else if (command[1] == "-1" || command[1] == "-1:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -1;
                                    }
                                    else if (command[1] == "0" || command[1] == "0:00" || command[1] == "+0" || command[1] == "+0:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "default (GMT 0:00)");
                                        connn.Timezone = 0;
                                    }
                                    else if (command[1] == "1" || command[1] == "1:00" || command[1] == "+1" || command[1] == "+1:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 1;
                                    }
                                    else if (command[1] == "2" || command[1] == "2:00" || command[1] == "+2" || command[1] == "+2:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 2;
                                    }
                                    else if (command[1] == "3" || command[1] == "3:00" || command[1] == "+3" || command[1] == "+3:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 3;
                                    }
                                    else if (command[1] == "4" || command[1] == "4:00" || command[1] == "+4" || command[1] == "+4:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 4;
                                    }
                                    else if (command[1] == "5" || command[1] == "5:00" || command[1] == "+5" || command[1] == "+5:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 5;
                                    }
                                    else if (command[1] == "6" || command[1] == "6:00" || command[1] == "+6" || command[1] == "+6:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 6;
                                    }
                                    else if (command[1] == "7" || command[1] == "7:00" || command[1] == "+7" || command[1] == "+7:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 7;
                                    }
                                    else if (command[1] == "8" || command[1] == "8:00" || command[1] == "+8" || command[1] == "+8:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 8;
                                    }
                                    else if (command[1] == "9" || command[1] == "9:00" || command[1] == "+9" || command[1] == "+9:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 9;
                                    }
                                    else if (command[1] == "10" || command[1] == "10:00" || command[1] == "+10" || command[1] == "+10:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 10;
                                    }
                                    else if (command[1] == "11" || command[1] == "11:00" || command[1] == "+11" || command[1] == "+11:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 11;
                                    }
                                    else if (command[1] == "12" || command[1] == "12:00" || command[1] == "+12" || command[1] == "+12:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 12;
                                    }
                                    #endregion

                                }
                                break;

                            default:
                                insim.Send(mso.UCID, 0, "^8Invalid command, type {0} to see available commands", "^2!help^8");
                                break;
                        }
                    }
                }
            }
            catch (Exception e) { LogTextToFile("commands", "[" + mso.UCID + "] " + StringHelper.StripColors(_connections[mso.UCID].PName) + "(" + GetConnection(mso.PLID).UName + ") NPL - Exception: " + e, false); }
        }
    }

}
