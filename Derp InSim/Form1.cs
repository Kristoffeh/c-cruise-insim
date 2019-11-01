using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InSimDotNet;
using InSimDotNet.Packets;
using System.Globalization;
using System.Threading;
using InSimDotNet.Helpers;
using System.IO;

namespace Derp_InSim
{
    public partial class Form1 : Form
    {
        InSim insim = new InSim();
        
        // Global Vars
        //public const string Tag = "^5EC^0™";
        public const string InSimVersion = "0.4d";
        public string website = "www.eugaming.org";
        const string AVAILABLE_CARS = "UF1+XFG+XRG+LX4+LX6+RB4+FXO+XRT+RAC+FZ5+UFR+XFR+FXR+XRR+FZR+MRT+FBM+FOX+FO8+BF1";


        public string TrackName = "None";
        public string HostName = "host";
        public string LayoutName = "None";
        public int dbCount = 0;
        public int dbBans = 0;

        public int TotalUptime = 0;

        public string DisplaysOpenedMsg = "^1Other InSim windows are already open, close them first and try again!";

        // MySQL Variables
        public SQLInfo SqlInfo = new SQLInfo();
        public bool ConnectedToSQL = false;
        public int SQLRetries = 0;


        // MySQL Connect
        string SQLIPAddress = "127.0.0.1"; // 93.190.143.115
        string SQLDatabase = "lfs";
        string SQLUsername = "root";
        string SQLPassword = "1997andre";

        public bool AskedPosition = false;
        public byte AskedPosUCID = 255;

        class Connections
        {
            // NCN fields
            public byte UCID;
            public string UName;
            public string PName;
            public bool IsAdmin;

            // Custom Fields
            public bool IsSuperAdmin;
            
            public bool OnTrack;

            // public byte Interface;
            public bool DisplaysOpen;
            public int InShopDist;
            public bool InShop;

            public int InBankDist;
            public bool InBank;
            public int BankX;
            public int BankY;

            public bool serverTime;

            public int cash;
            public int bankbalance;
            public string regdate;
            public string lastseen;
            public int totaljobsdone;
            public int totalearnedfromjobs;
            public string cars;

            public string bandate;
            public string banreason;

            public string Date;
            public string DateTime;
            public int Timezone;

            // Cash Sender
            public byte SenderUCID;

            // Cash Receivers
            public byte ReceiverUCID1;
            public byte ReceiverUCID2;
            public byte ReceiverUCID3;
            public byte ReceiverUCID4;
            public byte ReceiverUCID5;
            public byte ReceiverUCID6;
            public byte ReceiverUCID7;
            public byte ReceiverUCID8;
            public byte ReceiverUCID9;
            public byte ReceiverUCID10;
            public byte ReceiverUCID11;
            public byte ReceiverUCID12;
            public byte ReceiverUCID13;
            public byte ReceiverUCID14;
            public byte ReceiverUCID15;
            public byte ReceiverUCID16;
            public byte ReceiverUCID17;
            public byte ReceiverUCID18;
            public byte ReceiverUCID19;
            public byte ReceiverUCID20;

            // Cash Amount
            public int SendAmount;

            public int KMHorMPH;
            public decimal BonusDistance;
            public int TotalBonusDone;

            public decimal TotalDistance;
            public decimal Trip;
            public int _todayscash;
            public int _initialcash;

            public int TotalConnectionTime;

            public InSimDotNet.Packets.CompCar CompCar;

            public string CurrentCar = "None";

            public int TodaysCash
            {
                get { return _todayscash; }
                set { _todayscash = value; }
            }

            public int InitialCash
            {
                get { return _initialcash; }
                set { _initialcash = value; }
            }

        }
        class Players
        {
            public byte UCID;
            public byte PLID;
            public string PName;
            public string CName;

            public int kmh;
            public int mph;
            public string Plate;
        }

        private Dictionary<byte, Connections> _connections = new Dictionary<byte, Connections>();
        private Dictionary<byte, Players> _players = new Dictionary<byte, Players>();

        public Form1()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            InitializeComponent();
            RunInSim();
        }

        void RunInSim()
        {

            // Bind packet events.
            insim.Bind<IS_NCN>(NewConnection);
            insim.Bind<IS_NPL>(NewPlayer);
            insim.Bind<IS_MSO>(MessageReceived);
            insim.Bind<IS_MCI>(MultiCarInfo);
            insim.Bind<IS_CNL>(ConnectionLeave);
            insim.Bind<IS_CPR>(ClientRenames);
            insim.Bind<IS_PLL>(PlayerLeave);
            insim.Bind<IS_STA>(OnStateChange);
            insim.Bind<IS_BTC>(ButtonClicked);
            insim.Bind<IS_BFN>(ClearButtons);
            insim.Bind<IS_VTN>(VoteNotify);
            insim.Bind<IS_AXI>(OnAutocrossInformation);
            insim.Bind<IS_TINY>(OnTinyReceived);
            insim.Bind<IS_CON>(CarCOntact);
            insim.Bind<IS_PLP>(PLayerPit);

            // Initialize InSim
            insim.Initialize(new InSimSettings
            {
                Host = "127.0.0.1", // 93.190.143.115
                Port = 29999,
                Admin = "2910",
                Prefix = '!',
                Flags = InSimFlags.ISF_MCI | InSimFlags.ISF_MSO_COLS | InSimFlags.ISF_CON,

                Interval = 1000
            });

            insim.Send(new[]
            {
                new IS_TINY { SubT = TinyType.TINY_NCN, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_NPL, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_ISM, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_SST, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_MCI, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_NCI, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_AXI, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_SST, ReqI = 255 },
                });

            insim.Send("/cars " + AVAILABLE_CARS);                                    // Disabled until I fix shops
            // insim.Send(255, 0, "^8InSim connected with version ^2" + InSimVersion);

            ConnectedToSQL = SqlInfo.StartUp(SQLIPAddress, SQLDatabase, SQLUsername, SQLPassword);
            if (!ConnectedToSQL)
            {
                SQL_label.Text = "MySQL : NOT CONNECTED!";
                insim.Send(255, "SQL connect attempt failed! Attempting to reconnect in ^310 ^8seconds!");
                SQLReconnectTimer.Start();
                SaveTimer.Start();
                
            }
            else
            {
                SQL_label.Text = "MySQL : CONNECTED!";
                MessageToAdmins("Connected to MySQL database!");
            }
        }

        #region ' Misc '

        bool TryParseCommand(IS_MSO mso, out string[] args)
        {
            if (mso.UserType == UserType.MSO_PREFIX)
            {
                var message = mso.Msg.Substring(mso.TextStart);
                args = message.Split();
                return args.Length > 0;
            }

            args = null;
            return false;
        }

        void MssO(InSim insim, IS_MSO mso)
        {
            try
            {
                chatbox.Text += mso.Msg.ToString();
            }
            catch (Exception e) { LogTextToFile("error", "[" + mso.UCID + "] " + " MSO - Exception: " + e.Message, false); }
        }

        /// <summary>Returns true if method needs invoking due to threading</summary>
        private bool DoInvoke()
        {
            foreach (Control c in this.Controls)
            {
                if (c.InvokeRequired) return true;
                break;	// 1 control is enough
            }
            return false;
        }
        #endregion

        // Player joins server
        void NewConnection(InSim insim, IS_NCN packet)
        {
            try
            {
                _connections.Add(packet.UCID, new Connections
                {
                    UCID = packet.UCID,
                    UName = packet.UName,
                    PName = packet.PName,
                    IsAdmin = packet.Admin,

                    IsSuperAdmin = GetUserAdmin(packet.UName),
                    OnTrack = false,
                    TotalDistance = 0,
                    cash = 17500,
                    InitialCash = 0,
                    bankbalance = 1000,
                    cars = "UF1 XFG XRG",
                    TodaysCash = 0,
                    DisplaysOpen = false,
                    InShop = false,
                    InShopDist = 0,
                    DateTime = "" + DateTime.UtcNow.Hour + DateTime.UtcNow.Minute,
                    Date = "" + DateTime.UtcNow.Hour + DateTime.UtcNow.Minute,
                    Timezone = 0,
                    bandate = "",
                    banreason = "",
                    serverTime = false,
                    KMHorMPH = 0,
                    TotalConnectionTime = 0,
                    TotalBonusDone = 0,
                    BonusDistance = 0,
                    SenderUCID = 0,
                    ReceiverUCID1 = 0,
                    SendAmount = 0
                });

                if (ConnectedToSQL && packet.UName != "")
                {
                    try
                    {
                        if (SqlInfo.UserExist(packet.UName))
                        {

                            SqlInfo.UpdateUser(packet.UName, packet.PName, true);//Updates the last joined time to the current one

                            string[] LoadedOptions = SqlInfo.LoadUserOptions(packet.UName);
                            _connections[packet.UCID].cash = Convert.ToInt32(LoadedOptions[0]);
                            _connections[packet.UCID].bankbalance = Convert.ToInt32(LoadedOptions[1]);
                            _connections[packet.UCID].TotalDistance = Convert.ToDecimal(LoadedOptions[2]);
                            _connections[packet.UCID].cars = LoadedOptions[3];
                            _connections[packet.UCID].regdate = LoadedOptions[4];
                            _connections[packet.UCID].lastseen = LoadedOptions[5];
                            _connections[packet.UCID].totaljobsdone = Convert.ToInt32(LoadedOptions[6]);
                            _connections[packet.UCID].totalearnedfromjobs = Convert.ToInt32(LoadedOptions[7]);
                            _connections[packet.UCID].Timezone = Convert.ToInt32(LoadedOptions[8]);
                            _connections[packet.UCID].KMHorMPH = Convert.ToInt32(LoadedOptions[9]);

                            _connections[packet.UCID].InShop = false;
                            _connections[packet.UCID].InShopDist = 0;

                            _connections[packet.UCID].InitialCash = _connections[packet.UCID].cash;
                            _connections[packet.UCID].Trip = _connections[packet.UCID].TotalDistance;

                            if (packet.PName != HostName && packet.UCID != 0)
                            {
                                insim.Send(255, "" + packet.PName + " ^8was last seen at ^3" + _connections[packet.UCID].lastseen);
                            }

                            _connections[packet.UCID].TotalConnectionTime = Convert.ToInt32(LoadedOptions[10]);
                        }
                        else
                        {
                            if (packet.PName != HostName && packet.UCID != 0)
                            {
                                insim.Send(255, packet.PName + " ^8(" + packet.UName + ") joined the server for the first time!");
                            }

                            _connections[packet.UCID].InitialCash = _connections[packet.UCID].cash;

                            SqlInfo.AddUser(packet.UName, SqlInfo.RemoveStupidCharacters(_connections[packet.UCID].PName), _connections[packet.UCID].cash, _connections[packet.UCID].bankbalance, _connections[packet.UCID].TotalDistance, _connections[packet.UCID].cars, _connections[packet.UCID].totaljobsdone, _connections[packet.UCID].totalearnedfromjobs, _connections[packet.UCID].Timezone, _connections[packet.UCID].KMHorMPH, _connections[packet.UCID].TotalConnectionTime);
                        }

                        if (_connections[packet.UCID].IsSuperAdmin == false && _connections[packet.UCID].IsAdmin == true)
                        {
                            insim.Send(255, "^7" + packet.PName + " ^1is not an admin!");
                            insim.Send("/kick " + packet.UName);
                        }

                        // Welcome messages
                        insim.Send(packet.UCID, "^8Welcome back, " + packet.PName);

                        CheckCars(packet.UCID);
                        UpdateGui(packet.UCID, true, true, true);

                        dbCount = SqlInfo.userCount();
                        dbBans = SqlInfo.bansCount();

                    }
                    catch (Exception EX)
                    {
                        if (!SqlInfo.IsConnectionStillAlive())
                        {
                            SQL_label.Text = "MySQL : NOT CONNECTED!";
                            ConnectedToSQL = false;
                            SQLReconnectTimer.Start();
                        }
                        LogTextToFile("sqlerror", "[" + packet.UCID + "] " + StringHelper.StripColors(packet.PName) + "(" + packet.UName + ") NCN - Exception: " + EX, false);
                    }
                }

                #region ' Retrieve HostName '
                if (packet.UCID == 0 && packet.UName == "")
                {
                    HostName = packet.PName;
                }
                #endregion
            }
            catch (Exception e) { LogTextToFile("error", "[" + packet.UCID + "] " + StringHelper.StripColors(packet.PName) + "(" + packet.UName + ") NCN - Exception: " + e.Message, false); }
        }


        // Player joins race or enter track
        void NewPlayer(InSim insim, IS_NPL packet)
        {
            try
            {
                var r = GetConnection(packet.PLID);

                if (_players.ContainsKey(packet.PLID))
                {
                    // Leaving pits, just update NPL object.
                    _players[packet.PLID].UCID = packet.UCID;
                    _players[packet.PLID].PLID = packet.PLID;
                    _players[packet.PLID].PName = packet.PName;
                    _players[packet.PLID].CName = packet.CName;
                    _players[packet.PLID].Plate = packet.Plate;
                }
                else
                {
                    // Add new player.
                    _players.Add(packet.PLID, new Players
                    {
                        UCID = packet.UCID,
                        PLID = packet.PLID,
                        PName = packet.PName,
                        CName = packet.CName,
                        Plate = packet.Plate,
                    });
                }

                Connections CurrentConnection = GetConnection(packet.PLID);
                CurrentConnection.TotalBonusDone = 0;

                bool StoleThisCar = CheckCars(CurrentConnection.UCID);
                if (StoleThisCar)
                {
                    insim.Send(255, CurrentConnection.PName + " ^8tried to steal ^1" + packet.CName);
                    insim.Send("/spec " + _connections[packet.UCID].UName);
                }
                else
                {
                    insim.Send(CurrentConnection.UCID, "^7Leave pits to the ^1LEFT^7, drive ^2SAFE!");
                    CurrentConnection.OnTrack = true;
                    CurrentConnection.CurrentCar = packet.CName;
                }

            }
            catch (Exception e) { LogTextToFile("error", "[" + packet.UCID + "] " + StringHelper.StripColors(packet.PName) + "(" + GetConnection(packet.PLID).UName + ") NPL - Exception: " + e, false); }
        }

        // Player left the server
        void ConnectionLeave(InSim insim, IS_CNL CNL)
        {
            try
            {
                LogTextToFile("connections", _connections[CNL.UCID].PName + " (" + _connections[CNL.UCID].UName + ") Disconnected", false);

                // Save values of user - CNL (on disconnect)

                if (ConnectedToSQL)
                {
                    try { SqlInfo.UpdateUser(_connections[CNL.UCID].UName, StringHelper.StripColors(SqlInfo.RemoveStupidCharacters(_connections[CNL.UCID].PName)), false, _connections[CNL.UCID].cash, _connections[CNL.UCID].bankbalance, _connections[CNL.UCID].TotalDistance, _connections[CNL.UCID].cars, _connections[CNL.UCID].totaljobsdone, _connections[CNL.UCID].totalearnedfromjobs, _connections[CNL.UCID].Timezone, _connections[CNL.UCID].KMHorMPH, _connections[CNL.UCID].TotalConnectionTime); }
                    catch (Exception EX)
                    {
                        if (!SqlInfo.IsConnectionStillAlive())
                        {
                            SQL_label.Text = "MySQL : NOT CONNECTED!";
                            ConnectedToSQL = false;
                            SQLReconnectTimer.Start();
                        }
                        LogTextToFile("sqlerror", "[" + CNL.UCID + "] " + StringHelper.StripColors(_connections[CNL.UCID].PName) + "(" + _connections[CNL.UCID].UName + ") CNL - Exception: " + EX.Message, false);
                    }
                }

                _connections.Remove(CNL.UCID);
            }
            catch (Exception e) {  LogTextToFile("error", "[" + CNL.UCID + "] " + StringHelper.StripColors(_connections[CNL.UCID].PName) + "(" + _connections[CNL.UCID].UName + ") CNL - Exception: " + e, false); }
        }

        // Button click (is_btn click ID's)
        void ButtonClicked(InSim insim, IS_BTC BTC)
        {
            try { BTC_ClientClickedButton(BTC); }
            catch (Exception e) { LogTextToFile("error", "[" + BTC.UCID + "] " + StringHelper.StripColors(_connections[BTC.UCID].PName) + "(" + _connections[BTC.UCID].UName + ") BTC - Exception: " + e, false); }
        }

        // BuTton FunctioN (IS_BFN, SHIFT + I)
        void ClearButtons(InSim insim, IS_BFN BFN)
        {
            try
            {
                insim.Send(BFN.UCID, "^8InSim buttons cleared ^7(SHIFT + I)");
                if (_connections[BFN.UCID].DisplaysOpen == true)
                {
                    _connections[BFN.UCID].DisplaysOpen = false;
                }
                _connections[BFN.UCID].serverTime = false; UpdateGui(BFN.UCID, true, true, true);
            }
            catch (Exception e)
            { LogTextToFile("error", "[" + BFN.UCID + "] " + StringHelper.StripColors(_connections[BFN.UCID].PName) + "(" + _connections[BFN.UCID].UName + ") BFN - Exception: " + e, false); }
        }

        // Autocross information
        private void OnAutocrossInformation(InSim insim, IS_AXI AXI)
        {
            try
            {
                if (AXI.NumO != 0)
                {
                    LayoutName = AXI.LName;
                    if (AXI.ReqI == 0) insim.Send(255, "Layout loaded");
                }
            }
            catch (Exception EX) { LogTextToFile("error", "AXI - " + EX.Message); }
        }

        // Vote notify (cancel votes)
        private void VoteNotify(InSim insim, IS_VTN VTN)
        {
            try
            {
                foreach (var conn in _connections.Values)
                {
                    if (conn.UCID == VTN.UCID)
                    {
                        if (VTN.Action == VoteAction.VOTE_END)
                        {
                            if (_connections[VTN.UCID].IsAdmin != true)
                            {
                                insim.Send("/cv");
                            }
                        }

                        if (VTN.Action == VoteAction.VOTE_RESTART)
                        {
                            if (_connections[VTN.UCID].IsAdmin != true)
                            {
                                insim.Send("/cv");
                            }
                        }


                    }
                }

            }
            catch (Exception e) { LogTextToFile("error", "[" + VTN.UCID + "] " + StringHelper.StripColors(_connections[VTN.UCID].PName) + "(" + _connections[VTN.UCID].UName + ") - VTN - Exception: " + e, false); }
        }

        // MCI - Multi Car Info
        void MultiCarInfo(InSim insim, IS_MCI mci)
        {
            try
            {
                MCI_CarUpdates(insim, mci);
            }
            catch (Exception e) { LogTextToFile("error", "MCI - Exception: " + e, false); }
        }

        void ClientRenames(InSim insim, IS_CPR CPR)
        {
            try
            {
                _connections[CPR.UCID].PName = CPR.PName;
                foreach (var CurrentPlayer in _players.Values) if (CurrentPlayer.UCID == CPR.UCID) CurrentPlayer.PName = CPR.PName;//make sure your code is AFTER this one
            }
            catch (Exception e) { LogTextToFile("error", "[" + CPR.UCID + "] " + StringHelper.StripColors(CPR.PName) + "(" + _connections[CPR.UCID].UName + ") - CPR - Exception: " + e, false); }
        }

        void OnStateChange(InSim insim, IS_STA STA)
        {
            try
            {
                if (TrackName != STA.Track)
                {
                    TrackName = STA.Track;
                    insim.Send(new IS_TINY { SubT = TinyType.TINY_AXI, ReqI = 255 });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("" + e, "AN ERROR OCCURED");
                insim.Send(255, "^8An error occured: ^1{0}", e);
            }
        }

        // Join spectators (SHIFT+S / SHIFT + S)
        void PlayerLeave(InSim insim, IS_PLL PLL)
        {
            try
            {
                Connections conn = GetConnection(PLL.PLID);

                _players.Remove(PLL.PLID);//make sure your code is BEFORE this one
                conn.CurrentCar = "None";
                conn.OnTrack = false;
                conn.TotalBonusDone = 0;

                insim.Send(conn.UCID, "^1Your distance bonus ended at ^7" + string.Format("{0:0.00}", conn.BonusDistance / 1000) + " km/" + string.Format("{0:0.00}", conn.BonusDistance / 1609) + " mi");
                conn.BonusDistance = 0;

                UpdateGui(conn.UCID, true, true, true);
            }
            catch (Exception e)
            {
                Connections conn = GetConnection(PLL.PLID);
                LogTextToFile("error", "[" + conn.UCID + "] " + StringHelper.StripColors(conn.PName) + "(" + conn.UName + ") - PLL - Exception: " + e, false);
            }
        }

        // Goes to Garage (SHIFT+P / SHIFT + P)
        void PLayerPit(InSim insim, IS_PLP PLP)
        {
            try
            {
                Connections conn = GetConnection(PLP.PLID);

                _players.Remove(PLP.PLID);//make sure your code is BEFORE this one
                conn.CurrentCar = "None";
                conn.OnTrack = false;

                conn.TotalBonusDone = 0;
                insim.Send(conn.UCID, "^1Your distance bonus ended at ^7" + string.Format("{0:0.00}", conn.BonusDistance / 1000) + " km/" + string.Format("{0:0.00}", conn.BonusDistance / 1609) + " mi");
                conn.BonusDistance = 0;

                UpdateGui(conn.UCID, true, true, true);
            }
            catch (Exception e)
            {
                Connections conn = GetConnection(PLP.PLID);
                LogTextToFile("error", "[" + conn.UCID + "] " + StringHelper.StripColors(conn.PName) + "(" + conn.UName + ") - PLL - Exception: " + e, false);
            }
        }



        #region ' Functions '
        void UpdateGui(byte UCID, bool money, bool km, bool main = false)
        {
            const string TimeFormat = "HH:mm";//ex: 23:23 PM
            const string TimeFormatTwo = "dd/MM/yy";//ex: 23:23 PM

            if (main)
            {
                // DARK
                insim.Send(new IS_BTN
                {
                    UCID = UCID,
                    ReqI = 1,
                    ClickID = 1,
                    BStyle = ButtonStyles.ISB_DARK,
                    H = 8,
                    W = 73,
                    T = 0,
                    L = 63,
                });
            }

            if (money)
            {
                // Cash label
                insim.Send(new IS_BTN
                {
                    Text = "^7Cash:",
                    UCID = UCID,
                    ReqI = 2,
                    ClickID = 2,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 7,
                    T = 0,
                    L = 64,
                });

                // Cash box
                insim.Send(new IS_BTN
                {
                    Text = "^2€" + string.Format("{0:n0}", _connections[UCID].cash),
                    UCID = UCID,
                    ReqI = 3,
                    ClickID = 3,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 10,
                    T = 0,
                    L = 70,
                });

                // Session Cash label
                insim.Send(new IS_BTN
                {
                    Text = "^7Today:",
                    UCID = UCID,
                    ReqI = 4,
                    ClickID = 4,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 7,
                    T = 4,
                    L = 64,
                });

                // Session Cash box
                insim.Send(new IS_BTN
                {
                    Text = "^2€" + string.Format("{0:n0}", (_connections[UCID].cash - _connections[UCID].InitialCash)),
                    UCID = UCID,
                    ReqI = 5,
                    ClickID = 5,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 10,
                    T = 4,
                    L = 70,
                });
            }

            if (km)
            {
                // label
                insim.Send(new IS_BTN
                {
                    Text = "^7Total distance:",
                    UCID = UCID,
                    ReqI = 6,
                    ClickID = 6,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 15,
                    T = 0,
                    L = 82,
                });

                // label 2
                insim.Send(new IS_BTN
                {
                    Text = "^7Distance today:",
                    UCID = UCID,
                    ReqI = 7,
                    ClickID = 7,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 15,
                    T = 4,
                    L = 82,
                });

                // km
                insim.Send(new IS_BTN
                {
                    Text = "^7" + string.Format("{0:0.0}", _connections[UCID].TotalDistance / 1000) + " km",
                    UCID = UCID,
                    ReqI = 8,
                    ClickID = 8,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 12,
                    T = 0,
                    L = 96,
                });

                // todays km
                insim.Send(new IS_BTN
                {
                    Text = "^7" + string.Format("{0:0.0}", (_connections[UCID].BonusDistance) / 1000) + " km",
                    UCID = UCID,
                    ReqI = 9,
                    ClickID = 9,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 12,
                    T = 4,
                    L = 96,
                });
            }

            // Vehicle label
            insim.Send(new IS_BTN
            {
                Text = "^7Vehicle (" + _connections[UCID].CurrentCar + "):",
                UCID = UCID,
                ReqI = 10,
                ClickID = 10,
                BStyle = ButtonStyles.ISB_LEFT,
                H = 4,
                W = 13,
                T = 4,
                L = 109,
            });

            // Vehicle value
            insim.Send(new IS_BTN
            {
                Text = "^7100%",
                UCID = UCID,
                ReqI = 14,
                ClickID = 14,
                BStyle = ButtonStyles.ISB_LEFT,
                H = 4,
                W = 8,
                T = 4,
                L = 122,
            });
            
            /*if (_connections[UCID].serverTime == true)
            {
                insim.Send(new IS_BTN
                {
                    Text = "^7" + DateTime.UtcNow.ToString(TimeFormat),
                    UCID = UCID,
                    ReqI = 46,
                    ClickID = 46,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_RIGHT,
                    H = 4,
                    W = 27,
                    T = 76,
                    L = 91,
                });
            }*/

            if (_connections[UCID].Timezone == -12)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-12).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(-12).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == -11)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-11).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(-11).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == -10)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-10).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(-10).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == -9)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-9).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(-9).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == -8)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-8).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(-8).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == -7)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-7).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(-7).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == -6)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-6).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(-6).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == -5)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-5).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(-5).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == -4)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-3).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(-4).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == -3)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-3).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(-3).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == -2)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-2).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(-2).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == -1)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-1).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(-1).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 0)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(0).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(0).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 1)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(1).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(1).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 2)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(2).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(2).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 3)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(3).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(3).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 4)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(4).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(4).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 5)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(5).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(5).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 6)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(6).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(6).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 7)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(7).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(7).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 8)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(8).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(8).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 9)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(9).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(9).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 10)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(10).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(10).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 11)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(11).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(11).ToString(TimeFormatTwo);
            }
            else if (_connections[UCID].Timezone == 12)
            {
                _connections[UCID].DateTime = DateTime.UtcNow.AddHours(12).ToString(TimeFormat);
                _connections[UCID].Date = DateTime.UtcNow.AddHours(12).ToString(TimeFormatTwo);
            }
            


            // Clock
            insim.Send(new IS_BTN
            {
                Text = "^7" + _connections[UCID].DateTime,
                UCID = UCID,
                ReqI = 11,
                ClickID = 11,
                BStyle = ButtonStyles.ISB_LEFT,
                H = 4,
                W = 13,
                T = 0,
                L = 122,
            });

            // Date
            insim.Send(new IS_BTN
            {
                Text = "^7v" + InSimVersion,
                UCID = UCID,
                ReqI = 13,
                ClickID = 13,
                BStyle = ButtonStyles.ISB_C4,
                H = 4,
                W = 9,
                T = 4,
                L = 126,
            });

            // Total Jobs label
            insim.Send(new IS_BTN
            {
                Text = "^7Jobs: " + string.Format("{0:n0}", _connections[UCID].totaljobsdone),
                UCID = UCID,
                ReqI = 12,
                ClickID = 12,
                BStyle = ButtonStyles.ISB_LEFT,
                H = 4,
                W = 12,
                T = 0,
                L = 109,
            });
        }

        private void OnTinyReceived(InSim insim, IS_TINY TINY)
        {
            if (TINY.SubT == TinyType.TINY_AXC)
            {
                try
                {
                    if (LayoutName != "None")
                    {
                        insim.Send(255, "^8Layout removed");

                    }
                    else
                    {
                        LayoutName = "None";
                    }
                }
                catch (Exception EX) { LogTextToFile("packetError", "AXC - " + EX.Message); }
            }
        }

        private void CarCOntact(InSim insim, IS_CON CON)
        {
            try
            {
                var one = GetConnection(CON.A.PLID);
                var two = GetConnection(CON.B.PLID);

                if (CON.A.Speed > CON.B.Speed)
                {
                    insim.Send(two.UCID, "^3Contact with ^7" + two.PName + " ^3- " + MathHelper.MpsToKph(CON.A.Speed) + " kmh/" + string.Format("{0:n0}", MathHelper.MpsToMph(CON.A.Speed)) + " mi");
                }
                else
                {
                    insim.Send(two.UCID, "^3Contact with ^7" + one.PName + " ^3- " + MathHelper.MpsToKph(CON.B.Speed) + " kmh/" + string.Format("{0:n0}", MathHelper.MpsToMph(CON.B.Speed)) + " mi");
                }
            }
            catch (Exception EX) { LogTextToFile("packetError", "CON - " + EX.Message); }
        }

        CarFlags CarToPLC(string CarName)
        {
            switch (CarName.ToUpper())
            {
                case "UF1": return CarFlags.UF1;
                case "XFG": return CarFlags.XFG;
                case "XRG": return CarFlags.XRG;
                case "LX4": return CarFlags.LX4;
                case "LX6": return CarFlags.LX6;
                case "RB4": return CarFlags.RB4;
                case "FXO": return CarFlags.FXO;
                case "XRT": return CarFlags.XRT;
                case "RAC": return CarFlags.RAC;
                case "FZ5": return CarFlags.FZ5;
                case "UFR": return CarFlags.UFR;
                case "XFR": return CarFlags.XFR;
                case "FXR": return CarFlags.FXR;
                case "XRR": return CarFlags.XRR;
                case "FZR": return CarFlags.FZR;
                case "MRT": return CarFlags.MRT;
                case "FBM": return CarFlags.FBM;
                case "FOX": return CarFlags.FOX;
                case "FO8": return CarFlags.FO8;
                case "BF1": return CarFlags.BF1;

            }
            return CarFlags.None;
        }

        bool CheckCars(byte UCID)
        {
            string[] AllCars = AVAILABLE_CARS.Split('+');
            CarFlags AvailableCars = CarFlags.None;
            _connections[UCID].CurrentCar = "None";
            bool DrivingNotOwnedCar = true;

            byte PlayerID = GetPlayer(UCID);
            if (PlayerID != 255) _connections[UCID].CurrentCar = _players[PlayerID].CName;

            foreach (var ThisCar in AllCars)
            {
                if (_connections[UCID].cars.Contains(ThisCar))
                {
                    if (ThisCar == _connections[UCID].CurrentCar) DrivingNotOwnedCar = false;
                    AvailableCars |= CarToPLC(ThisCar);
                }
            }

            insim.Send(new IS_PLC { UCID = UCID, Cars = AvailableCars });
            return DrivingNotOwnedCar;
        }

        void ClearPen(string Username) { insim.Send("/p_clear " + Username); }

        void KickID(string Username) { insim.Send("/kick " + Username); }

        private void btn(string text, byte height, byte width, byte top, byte length, ButtonStyles bstyle, byte clickid, byte ucid)
        {
            insim.Send(new IS_BTN
            {
                Text = text,
                UCID = ucid,
                ReqI = clickid,
                ClickID = clickid,
                BStyle = bstyle,
                H = height,
                W = width,
                T = top,
                L = length
            });
        }

        byte GetPlayer(byte UCID)
        {//Get Player from UCID
            byte PLID = 255;
            foreach (var CurrentPlayer in _players.Values) if (CurrentPlayer.UCID == UCID) PLID = CurrentPlayer.PLID;

            return PLID;
        }

        private Connections GetConnection(byte PLID)
        {//Get Connection from PLID
            Players NPL;
            if (_players.TryGetValue(PLID, out NPL)) return _connections[NPL.UCID];
            return null;
        }

        private bool IsConnAdmin(Connections Conn)
        {//general admin check, both Server and InSim
            if (Conn.IsAdmin == true || Conn.IsSuperAdmin == true) return true;
            return false;
        }

        private bool GetUserAdmin(string Username)
        {//reading admins.ini when connecting to server for InSim admin
            StreamReader CurrentFile = new StreamReader("files/admins.ini");

            string line = null;
            while ((line = CurrentFile.ReadLine()) != null)
            {
                if (line == Username)
                {
                    CurrentFile.Close();
                    return true;
                }
            }
            CurrentFile.Close();
            return false;
        }

        private void LogTextToFile(string file, string text, bool AdminMessage = true)
        {

            if (System.IO.File.Exists("files/" + file + ".log") == false) { FileStream CurrentFile = System.IO.File.Create("files/" + file + ".log"); CurrentFile.Close(); }

            StreamReader TextTempData = new StreamReader("files/" + file + ".log");
            string TempText = TextTempData.ReadToEnd();
            TextTempData.Close();

            StreamWriter TextData = new StreamWriter("files/" + file + ".log");
            TextData.WriteLine(TempText + DateTime.Now + ": " + text);
            TextData.Flush();
            TextData.Close();
        }

        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var cenn in _connections.Values)
            {
                if (ConnectedToSQL)
                {
                    try { SqlInfo.UpdateUser(_connections[cenn.UCID].UName, StringHelper.StripColors(SqlInfo.RemoveStupidCharacters(_connections[cenn.UCID].PName)), false, _connections[cenn.UCID].cash, _connections[cenn.UCID].bankbalance, _connections[cenn.UCID].TotalDistance, _connections[cenn.UCID].cars, _connections[cenn.UCID].totaljobsdone, _connections[cenn.UCID].totalearnedfromjobs, _connections[cenn.UCID].Timezone, _connections[cenn.UCID].KMHorMPH, _connections[cenn.UCID].TotalConnectionTime); }
                    catch (Exception EX)
                    {
                        if (!SqlInfo.IsConnectionStillAlive())
                        {
                            SQL_label.Text = "MySQL : NOT CONNECTED!";
                            ConnectedToSQL = false;
                            SQLReconnectTimer.Start();
                        }
                        LogTextToFile("sqlerror", "[" + cenn.UCID + "] " + StringHelper.StripColors(_connections[cenn.UCID].PName) + "(" + _connections[cenn.UCID].UName + ") CNL - Exception: " + EX.Message, false);
                    }
                }
            }
        }

        private void deleteBtn(byte ucid, byte reqi, bool sendbfn, byte clickid)
        {
            if (sendbfn == true)
            {
                IS_BFN bfn = new IS_BFN();
                bfn.ClickID = clickid;
                bfn.UCID = ucid;
                bfn.ReqI = reqi;

                insim.Send(bfn);
            }
        }

        private int AbsoluteAngleDifference(int d, int h)
        {
            d /= 180;
            h /= 180;
            int absdiff = Math.Abs(d - h);

            if (absdiff <= 180) return absdiff;

            if (d < 180)
            {
                h -= 360;
                return d - h;
            }
            else
            {
                d -= 360;
                return h - d;
            }
        }

        private void MessageToAdmins(string Message)
        {
            foreach (var conn in _connections.Values)
            {
                if (conn.IsAdmin == true && conn.IsSuperAdmin == true)
                {
                    if (conn.UName != "")
                    {
                        insim.Send(conn.UCID, "^3Admin Notice:");
                        insim.Send(conn.UCID, "^2");
                    }
                }
            }
        }

        public int GetDistXY(int X1, int Y1, int X2, int Y2)
        {
            //int X;			// X map (65536 = 1 metre)
            //int Y;			// Y map (65536 = 1 metre)
            //int Z;			// Z alt (65536 = 1 metre)
            return ((int)Math.Sqrt(Math.Pow(X1 - X2, 2) + Math.Pow(Y1 - Y2, 2)) / 65536);
        }

        private void typebox_TextChanged(object sender, EventArgs e)
        {
            if (typebox.Text.Length > 2)
            {
                sendmsgfromhost(typebox.Text);
            }
        }

        void sendmsgfromhost(string message)
        {
            insim.Send(255, "^7Host : ^2" + message);
        }

        private void typebox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (typebox.Text.Length != 0)
                {
                    sendmsgfromhost(typebox.Text);
                    typebox.Text = "";
                    e.Handled = e.SuppressKeyPress = true;
                }
            }
        }

        public void HomeScreen(byte UCID)
        {
            {
                // DARK
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 30, ClickID = 30, BStyle = ButtonStyles.ISB_DARK, H = 37, W = 65, T = 38, L = 66, Text = "" }); // H = 52

                // Title
                string text = "^3European Cruise";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 31, ClickID = 31, BStyle = ButtonStyles.ISB_LIGHT, H = 7, W = 61, T = 40, L = 68, Text = text });

                // Home
                string text2 = "^2Home";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 32, ClickID = 32, BStyle = ButtonStyles.ISB_LIGHT, H = 4, W = 13, T = 51, L = 69, Text = text2 });

                // Server Stats
                string text3 = "^8Server Stats";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 33, ClickID = 33, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 55, L = 69, Text = text3 });

                // Your Stats
                string text4 = "^8Your Stats";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 34, ClickID = 34, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 59, L = 69, Text = text4 });

                // Commands
                string text5 = "^8Commands";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 35, ClickID = 35, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 63, L = 69, Text = text5 });

                // About
                string text6 = "^8About";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 36, ClickID = 36, BStyle = ButtonStyles.ISB_DARK | ButtonStyles.ISB_CLICK, H = 4, W = 13, T = 67, L = 69, Text = text6 });

                // ---------------------------------------- //
                // ---------------------------------------- //

                // Lines
                string line1 = "^7Welcome to ^3European Cruise^7.";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 37, ClickID = 37, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 51, L = 83, Text = line1 });

                string line2 = "^7There aren't much going on, there aren't much you can do.";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 38, ClickID = 38, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 55, L = 83, Text = line2 });

                string line3 = "^7Jobs and distance bonuses are coming later.";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 39, ClickID = 39, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 59, L = 83, Text = line3 });

                string line4 = "^7For more info about stats and commands type ^3!help";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 40, ClickID = 40, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 63, L = 83, Text = line4 });

                string line5 = "^7Get started by jumping into a car and start driving!";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 41, ClickID = 41, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 67, L = 83, Text = line5 });

                string line6 = "";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 42, ClickID = 42, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 71, L = 83, Text = line6 });

                string line7 = "";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 43, ClickID = 43, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 75, L = 83, Text = line7 });

                string line8 = "";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 46, ClickID = 46, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 79, L = 83, Text = line8 });

                string line9 = "";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 45, ClickID = 45, BStyle = ButtonStyles.ISB_LEFT, H = 4, W = 46, T = 83, L = 83, Text = line9 });

                // Exit button
                string close = "^1^J‚w";
                insim.Send(new IS_BTN { UCID = UCID, ReqI = 44, ClickID = 44, BStyle = ButtonStyles.ISB_C4 | ButtonStyles.ISB_CLICK, H = 7, W = 7, T = 40, L = 122, Text = close });
            }
        }
    }
}
