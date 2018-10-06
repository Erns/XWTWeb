using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XWTWeb.Classes;
using XWTWeb.Models;

namespace XWTWeb.Controllers
{
    [RequireHttps]
    [Authorize]
    public class TournamentActivityController : Controller
    {
        RestClient client = Utilities.InitializeRestClient();

        static TournamentActivity objTournActivity = new TournamentActivity();
        static TournamentMain objTournMain = new TournamentMain();
        static List<Player> lstPlayersAll = new List<Player>();

        static int tournamentId = 0;


        // GET: TournamentActivity
        public ActionResult Main(int id)
        {
            tournamentId = id;

            objTournActivity = new TournamentActivity();
            objTournMain = new TournamentMain();
            lstPlayersAll = new List<Player>();

            List<Player> playersNextRound = new List<Player>();

            //Get Tournament Info
            try
            {
                if (id > 0)
                {
                    var request = new RestRequest("Tournaments/{userid}/{id}", Method.GET);
                    request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
                    request.AddUrlSegment("id", id);

                    // execute the request
                    IRestResponse response = client.Execute(request);
                    var content = response.Content;

                    List<TournamentMain> result = JsonConvert.DeserializeObject<List<TournamentMain>>(JsonConvert.DeserializeObject(content).ToString());
                    foreach (TournamentMain tmpTournament in result)
                    {
                        objTournMain = tmpTournament;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("TournamentActivityController.Main{0}Get Tournament Error:{1}", Environment.NewLine, ex.Message));
            }

            //Get Players
            try
            {
                var request = new RestRequest("Players/{userid}", Method.GET);
                request.AddUrlSegment("userid", Utilities.CurrentUser.Id);

                // execute the request
                IRestResponse response = client.Execute(request);
                var content = response.Content;

                List<Player> result = JsonConvert.DeserializeObject<List<Player>>(JsonConvert.DeserializeObject(content).ToString());
                foreach (Player player in result)
                {
                    lstPlayersAll.Add(player);
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("TournamentActivityController.Main{0}Get Players Error:{1}", Environment.NewLine, ex.Message));
            }


            objTournActivity.AllPlayers = lstPlayersAll;
            objTournActivity.NextRoundPlayers = playersNextRound;
            objTournActivity.TournamentMain = objTournMain;

            return View(objTournActivity);
        }

        public ActionResult AddNewRound(string activePlayers)
        {

            List<TournamentMainPlayer> result = JsonConvert.DeserializeObject<List<TournamentMainPlayer>>(activePlayers);

            //Flag each current player as inactive
            foreach (TournamentMainPlayer currentPlayer in objTournMain.Players)
            {
                currentPlayer.Active = false;
            }

            //Then determine which, if any, should be active.
            foreach (TournamentMainPlayer resultPlayer in result)
            {
                foreach (TournamentMainPlayer currentPlayer in objTournMain.Players)
                {
                    if (currentPlayer.PlayerId == resultPlayer.PlayerId)
                    {
                        resultPlayer.Id = currentPlayer.Id;
                        currentPlayer.Active = true;
                        break;
                    }
                }
            }

            //Determine if there are any new players to be added
            foreach (TournamentMainPlayer resultPlayer in result)
            {
                if (resultPlayer.Id == 0)
                {
                    foreach(Player player in lstPlayersAll)
                    {
                        if (player.Id == resultPlayer.PlayerId)
                        {

                            TournamentMainPlayer newNextRoundPlayer = new TournamentMainPlayer
                            {
                                Id = 0,
                                TournamentId = tournamentId,
                                PlayerId = player.Id,
                                PlayerName = player.Name,
                                Active = true
                            };

                            objTournActivity.TournamentMain.Players.Add(newNextRoundPlayer);
                        }
                    }
                }
            }

            var request = new RestRequest("Tournaments/{userid}/{id}", Method.PUT);
            request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
            request.AddUrlSegment("id", tournamentId);
            request.AddJsonBody(JsonConvert.SerializeObject(objTournActivity.TournamentMain));

            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content;

            //TournamentMainRound round = new TournamentMainRound();
            //round.Number = 1;

            //result.TournamentMain.Rounds.Add(round);


            //objTournActivity = result;
            //UpdateModel<TournamentActivity>(result);

            //StartRound(true, 0);

            return View(objTournActivity);
            
        }

        private void StartRound(bool blnSwiss, int intTableCount)
        {
            //Create a new round
            TournamentMainRound round = new TournamentMainRound();
            round.TournamentId = tournamentId;
            round.Number = objTournActivity.TournamentMain.Rounds.Count + 1;
            round.Swiss = blnSwiss;

            List<TournamentMainPlayer> lstActiveTournamentPlayers = new List<TournamentMainPlayer>();
            List<TournamentMainPlayer> lstActiveTournamentPlayers_Byes = new List<TournamentMainPlayer>();

            if (blnSwiss)
                SetupSwissPlayers(ref lstActiveTournamentPlayers, ref lstActiveTournamentPlayers_Byes);
            else
                SetupSingleEliminationPlayers(ref lstActiveTournamentPlayers, intTableCount);

            //Create each table, pair 'em up
            TournamentMainRoundTable roundTable = new TournamentMainRoundTable();
            foreach (TournamentMainPlayer player in lstActiveTournamentPlayers)
            {
                if (roundTable.Player1Id != 0 && roundTable.Player2Id != 0)
                {
                    setRoundTableNames(ref roundTable);
                    round.Tables.Add(roundTable);
                    roundTable = new TournamentMainRoundTable();
                }

                if (roundTable.Player1Id == 0)
                {
                    roundTable.Number = round.Tables.Count + 1;
                    roundTable.Player1Id = player.PlayerId;
                }
                else if (roundTable.Player2Id == 0)
                {
                    roundTable.Player2Id = player.PlayerId;
                }
            }

            //If the last table of non-manual byes is an odd-man, set table/player as a bye
            if (roundTable.Player2Id == 0)
            {
                roundTable.Bye = true;
                roundTable.Player1Score = objTournMain.MaxPoints / 2;
                roundTable.Player1Winner = true;
            }

            setRoundTableNames(ref roundTable);
            round.Tables.Add(roundTable);

            //If a manual bye (such as first-round byes at a tournament), add these players now
            if (lstActiveTournamentPlayers_Byes.Count > 0)
            {
                lstActiveTournamentPlayers_Byes.Shuffle();
                foreach (TournamentMainPlayer player in lstActiveTournamentPlayers_Byes)
                {
                    roundTable = new TournamentMainRoundTable();
                    roundTable.Number = round.Tables.Count + 1;
                    roundTable.Player1Id = player.PlayerId;
                    roundTable.Bye = true;
                    roundTable.Player1Score = objTournMain.MaxPoints / 2;
                    roundTable.Player1Winner = true;
                    setRoundTableNames(ref roundTable);
                    round.Tables.Add(roundTable);
                }
            }

            ////Add/Save the round
            //using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DB_PATH))
            //{
            //    try
            //    {
            //        conn.Update(objTournMain); //Update any other information that was saved such as Bye counts and such
            //        conn.InsertWithChildren(round, true);
            //    }
            //    catch (Exception ex)
            //    {
            //        DisplayAlert("Warning!", "Error adding round to tournament! " + ex.Message, "OK");
            //    }

            //    OnAppearing();
            //}

        }



        #region Setup Player Methods

        private void SetupSwissPlayers(ref List<TournamentMainPlayer> lstActiveTournamentPlayers, ref List<TournamentMainPlayer> lstActiveTournamentPlayers_Byes, int intAttempts = 0)
        {
            //Grab list of currently active players in the tournament
            Dictionary<int, List<TournamentMainPlayer>> dctActiveTournamentPlayerScores = new Dictionary<int, List<TournamentMainPlayer>>();

            foreach (TournamentMainPlayer player in objTournMain.Players)
            {
                if (player.Active)
                {
                    TournamentMainPlayer roundPlayer = new TournamentMainPlayer();
                    roundPlayer.PlayerId = player.PlayerId;
                    roundPlayer.Score = player.Score;
                    roundPlayer.OpponentIds = player.OpponentIds;

                    if (!player.Bye)
                        lstActiveTournamentPlayers.Add(roundPlayer);
                    else
                    {
                        lstActiveTournamentPlayers_Byes.Add(roundPlayer);
                        player.Bye = false;  //No longer has a Bye for the next round
                        player.ByeCount++;
                    }
                }
            }


            if (objTournMain.Rounds.Count == 0)
            {
                //First round, completely random player pairings
                lstActiveTournamentPlayers.Shuffle();
            }
            else
            {
                //Subsequent rounds, group up players with same win count as much as possible and randomize 
                dctActiveTournamentPlayerScores = new Dictionary<int, List<TournamentMainPlayer>>();
                for (int i = objTournMain.Rounds.Count; i >= 0; i--)
                {
                    dctActiveTournamentPlayerScores.Add(i, new List<TournamentMainPlayer>());
                    foreach (TournamentMainPlayer activePlayer in lstActiveTournamentPlayers)
                    {
                        if (i == activePlayer.Score)
                        {
                            dctActiveTournamentPlayerScores[i].Add(activePlayer);
                        }
                    }

                    dctActiveTournamentPlayerScores[i].Shuffle(); //Shuffle all the players in each win bracket
                }

                //Clear out the active list, then go down the list and re-add them back in.
                lstActiveTournamentPlayers.Clear();
                for (int i = objTournMain.Rounds.Count; i >= 0; i--)
                {
                    if (dctActiveTournamentPlayerScores.ContainsKey(i))
                    {
                        foreach (TournamentMainPlayer activePlayer in dctActiveTournamentPlayerScores[i])
                        {
                            lstActiveTournamentPlayers.Add(activePlayer);
                        }
                    }
                }

                //If odd number of players, the last in the list will get a Bye
                //Get the lowest ranked player that hasn't had a bye already
                if (lstActiveTournamentPlayers.Count % 2 != 0)
                {
                    foreach (TournamentMainPlayer player in objTournMain.Players.OrderByDescending(obj => obj.Rank).ToList())
                    {
                        if (player.ByeCount == 0)
                        {
                            for (int i = lstActiveTournamentPlayers.Count - 1; i > 0; i--)
                            {
                                if (lstActiveTournamentPlayers[i].PlayerId == player.PlayerId)
                                {
                                    TournamentMainPlayer roundPlayer = lstActiveTournamentPlayers[i];
                                    lstActiveTournamentPlayers.RemoveAt(i);
                                    lstActiveTournamentPlayers.Add(roundPlayer);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }

                //Triple check to make sure no one is playing someone that they have already.
                //Reshuffle as necessary, multiple times (perhaps the tournament decides to keep going for whatever reason)
                if (intAttempts < 100)
                {
                    bool bFailMatchup = false;
                    int intCount = 1;
                    TournamentMainPlayer tmpPlayer1 = new TournamentMainPlayer();
                    foreach (TournamentMainPlayer player in lstActiveTournamentPlayers)
                    {
                        //Check every even player to see if they have already been paired up with the player before them (as they will be the ones paired up to went forwarded to the round table)
                        if (intCount % 2 == 0)
                        {
                            if (player.OpponentIds.Contains(tmpPlayer1.Id))
                            {
                                bFailMatchup = true;
                                break;
                            }
                        }
                        else
                        {
                            tmpPlayer1 = player;
                        }
                        intCount++;
                    }

                    if (bFailMatchup)
                    {
                        lstActiveTournamentPlayers = new List<TournamentMainPlayer>();
                        lstActiveTournamentPlayers_Byes = new List<TournamentMainPlayer>();
                        SetupSwissPlayers(ref lstActiveTournamentPlayers, ref lstActiveTournamentPlayers_Byes, intAttempts++);
                    }
                }
            }
        }

        private void SetupSingleEliminationPlayers(ref List<TournamentMainPlayer> lstActiveTournamentPlayers, int intTableCount)
        {
            if (intTableCount == 0 && objTournMain.Rounds.Count > 0)
            {
                intTableCount = objTournMain.Rounds[objTournMain.Rounds.Count - 1].Tables.Count;
            }

            //Recalculate the latest scores, grab the top number of players that qualify for the number of tables
            Utilities.CalculatePlayerScores(ref objTournMain);
            List<TournamentMainPlayer> lstTmpPlayers = new List<TournamentMainPlayer>();

            lstTmpPlayers = objTournMain.Players.OrderBy(obj => obj.Rank).AsQueryable().Where(obj => obj.Rank <= (intTableCount)).ToList();

            while (lstTmpPlayers.Count > 0)
            {
                lstActiveTournamentPlayers.Add(lstTmpPlayers[0]);
                lstActiveTournamentPlayers.Add(lstTmpPlayers[lstTmpPlayers.Count - 1]);
                lstTmpPlayers.RemoveAt(0);
                lstTmpPlayers.RemoveAt(lstTmpPlayers.Count - 1);
            }
        }

        #endregion

        #region Page-Specific Utilities

        private void setRoundTableNames(ref TournamentMainRoundTable roundTable)
        {
            //using (SQLite.SQLiteConnection conn = new SQLite.SQLiteConnection(App.DB_PATH))
            //{

            //    Player player;

            //    string strPlayer1Name = "N/A";
            //    string strPlayer2Name = "N/A";

            //    if (roundTable.Player1Id > 0)
            //    {
            //        player = conn.Get<Player>(roundTable.Player1Id);
            //        strPlayer1Name = player.Name;
            //    }

            //    if (roundTable.Player2Id > 0)
            //    {
            //        player = conn.Get<Player>(roundTable.Player2Id);
            //        strPlayer2Name = player.Name;
            //    }

            //    roundTable.Player1Name = strPlayer1Name;
            //    roundTable.Player2Name = strPlayer2Name;
            //    roundTable.TableName = string.Format("{0} vs {1}", strPlayer1Name, strPlayer2Name);
            //}
        }
        #endregion
    }
}