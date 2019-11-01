using System;
using InSimDotNet;
using InSimDotNet.Packets;
using InSimDotNet.Helpers;

namespace Derp_InSim
{
    public partial class Form1
    {
        private void MCI_CarUpdates(InSim insim, IS_MCI MCI)
        {
            try
            {
                foreach (CompCar car in MCI.Info)
                {
                    Connections conn = GetConnection(car.PLID);
                    {
                        int Sped = Convert.ToInt32(MathHelper.SpeedToKph(car.Speed));

                        decimal SpeedMS = (int)(((car.Speed / 32768f) * 100f) / 2);
                        decimal Speed = (int)((car.Speed * (100f / 32768f)) * 3.6f);

                        int kmh = car.Speed / 91;
                        int mph = car.Speed / 146;
                        var X = car.X;
                        var Y = car.Y;
                        var Z = car.Z;
                        var angle = car.AngVel / 30;
                        string anglenew = "";
                        int Angle = AbsoluteAngleDifference(car.Direction, car.Heading);

                        _players[car.PLID].kmh = kmh;
                        _players[car.PLID].mph = mph;

                        conn.TotalDistance += Convert.ToInt32(SpeedMS);
                        conn.BonusDistance += Convert.ToInt32(SpeedMS);

                        if (AskedPosition == true && AskedPosUCID == conn.UCID)
                        {
                            // SendRCMToUsername(CurrentConnection.UName, "Your Position is: " + (car.X / 65535) + ", " + (car.Y / 65535), 5000);//keep the message for 5seconds(5000ms)
                            insim.Send(AskedPosUCID, "^3X: ^7" + (car.X / 65535) + " ^3Y: ^7" + (car.Y / 65535));
                            insim.Send(AskedPosUCID, "^3Speed: ^1" + Speed + " ^3Angle: ^1" + Angle);

                            AskedPosition = false;
                            AskedPosUCID = 255;
                        }

                        // if (GetDistXY(car.X, car.Y, -80, 1002) < 100)
                        /*
                        if (conn.OnTrack == true)
                        {
                            conn.InShopDist = (GetDistXY(car.X, car.Y, -80, 1002));
                            if (conn.InShopDist < 10)
                            v
                            {
                                insim.Send(conn.UCID, "^8Welcome to the ^2" + "Mechanics");
                            }
                        }*/

                        string text = String.Format(
                        "^1X: {0:F2} ^2Y: {1:F2} ^3Z: {2:F2}",
                        car.X / 65536,
                        car.Y / 65536,
                        car.Z / 65536);

                        if (conn.UName == "kristofferandersen")
                        {
                            // mci
                            insim.Send(new IS_BTN
                            {
                                Text = "^7" + text,
                                UCID = conn.UCID,
                                ReqI = 17,
                                ClickID = 17,
                                BStyle = ButtonStyles.ISB_DARK,
                                H = 4,
                                W = 30,
                                T = 0,
                                L = 139,
                            });
                        }

                        #region ' WE2X '

                        switch (TrackName)
                        {
                            case "WE2X":
                                {
                                    // if (((car.X / 65536) >= -143) && (car.X / 65536 <= -967) && (car.Y / 65536 >= -151) && (car.Y / 65536 <= 966))
                                    // if (MathHelper.LengthToMeters(MathHelper.Distance(car.X, car.Y, -147, 971)) < 5)

                                    // if (((car.X / 65536) >= -258) && (car.X / 65536 <= -3) && (car.Y / 65536 >= 873) && (car.Y / 65536 <= 834))


                                    /*conn.InBankDist = GetDistXY(car.X, car.Y, -148, 971);
                                    if (conn.InBankDist < 2)
                                    {
                                        insim.Send(255, "you're there!");
                                    }*/
                                    break;
                                }
                        }

                        #endregion

                        UpdateGui(conn.UCID, false, true);

                        anglenew = angle.ToString().Replace("-", "");

                        
                    }
                }
            }
            catch (Exception e) { LogTextToFile("error", "[+++] BTC - Exception: " + e, false); }
        }
    }
}
