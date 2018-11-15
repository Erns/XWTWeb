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

        #region Main
      
        // GET: TournamentActivity
        public ActionResult TournamentActivityMain(int id)
        {
            tournamentId = id;

            objTournActivity = new TournamentActivity();            
            lstPlayersAll = new List<Player>();

            List<Player> lstPlayersNextRd = new List<Player>();
            List<int> lstActivePlayers = new List<int>();

            //Get Tournament Info
            try
            {
                if (tournamentId > 0)
                {
                    var request = new RestRequest("Tournaments/{userid}/{id}", Method.GET);
                    request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
                    request.AddUrlSegment("id", tournamentId);

                    // execute the request
                    IRestResponse response = client.Execute(request);
                    var content = response.Content;

                    List<TournamentMain> result = JsonConvert.DeserializeObject<List<TournamentMain>>(JsonConvert.DeserializeObject(content).ToString());
                    foreach (TournamentMain tmpTournament in result)
                    {
                        objTournMain = tmpTournament;
                        break;
                    }

                    //Didn't actually get a result (such as trying to access a tournament not "owned" by user).  Kick back to Main page
                    if (result.Count == 0)
                        return RedirectToAction("TournamentMain", "Tournament");
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("TournamentActivityController.TournamentActivityMain{0}Get Tournament Error:{1}", Environment.NewLine, ex.Message));
                return RedirectToAction("TournamentMain", "Tournament");
            }

            lstActivePlayers = objTournMain.ActivePlayersList();

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
                    if (lstActivePlayers.Contains(player.Id))
                    {
                        lstPlayersNextRd.Add(player);
                    }
                    else
                    {
                        lstPlayersAll.Add(player);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("TournamentActivityController.TournamentActivityMain{0}Get Players Error:{1}", Environment.NewLine, ex.Message));
            }

            //Set standings when loading page
            //Utilities.CalculatePlayerScores(ref objTournMain);

            //Set initial info
            objTournActivity.AllPlayers = lstPlayersAll;
            objTournActivity.NextRoundPlayers = lstPlayersNextRd;
            //objTournActivity.Standings = objTournMain.Players.OrderBy(obj => obj.Rank).ToList();
            objTournActivity.TournamentMain = objTournMain;

            if (objTournMain.Rounds.Count > 0)
            {
                objTournActivity.SwissMode = objTournMain.Rounds[objTournMain.Rounds.Count - 1].Swiss;
                objTournActivity.CurrentTableCount = objTournMain.Rounds[objTournMain.Rounds.Count - 1].Tables.Count;
            }


            return View(objTournActivity);
        }

        #endregion

        #region Setup Round

        public ActionResult AddNewRound(bool swiss, int topCut, string activePlayers)
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
                        currentPlayer.Bye = resultPlayer.Bye;
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
                                Active = true,
                                Bye = resultPlayer.Bye
                            };

                            objTournActivity.TournamentMain.Players.Add(newNextRoundPlayer);
                        }
                    }
                }
            }

            var request = new RestRequest("TournamentsPlayers/{userid}/{id}", Method.PUT);
            request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
            request.AddUrlSegment("id", tournamentId);
            request.AddJsonBody(JsonConvert.SerializeObject(objTournActivity.TournamentMain));

            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content;

            //Start up the next round
            StartRound(swiss, topCut);

            return View(objTournActivity);
            
        }

        private void StartRound(bool blnSwiss, int intTopCut)
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
                SetupSingleEliminationPlayers(ref lstActiveTournamentPlayers, intTopCut);

            //Create each table, pair 'em up
            TournamentMainRoundTable roundTable = new TournamentMainRoundTable();
            foreach (TournamentMainPlayer player in lstActiveTournamentPlayers)
            {
                if (roundTable.Player1Id != 0 && roundTable.Player2Id != 0)
                {
                    //setRoundTableNames(ref roundTable);
                    roundTable.TableName = string.Format("{0} vs {1}", roundTable.Player1Name, roundTable.Player2Name);
                    round.Tables.Add(roundTable);
                    roundTable = new TournamentMainRoundTable();
                }

                if (roundTable.Player1Id == 0)
                {
                    roundTable.Number = round.Tables.Count + 1;
                    roundTable.Player1Id = player.PlayerId;
                    roundTable.Player1Name = player.PlayerName;
                }
                else if (roundTable.Player2Id == 0)
                {
                    roundTable.Player2Id = player.PlayerId;
                    roundTable.Player2Name = player.PlayerName;
                }
            }

            //If the last table of non-manual byes is an odd-man, set table/player as a bye
            if (roundTable.Player2Id == 0)
            {
                roundTable.Bye = true;
                roundTable.Player1Score = objTournMain.MaxPoints / 2;
                roundTable.Player1Winner = true;
            }

            roundTable.TableName = string.Format("{0} vs {1}", roundTable.Player1Name, roundTable.Player2Name);
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
                    roundTable.Player1Name = player.PlayerName;
                    roundTable.Bye = true;
                    roundTable.Player1Score = objTournMain.MaxPoints / 2;
                    roundTable.Player1Winner = true;
                    roundTable.TableName = string.Format("{0} vs {1}", roundTable.Player1Name, "N/A");
                    round.Tables.Add(roundTable);
                }
            }

            objTournMain.Rounds.Add(round);


            //Create round and associated tables on database
            var request = new RestRequest("TournamentsRounds/{userid}/{id}", Method.POST);
            request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
            request.AddUrlSegment("id", round.Id);
            request.AddJsonBody(JsonConvert.SerializeObject(round));

            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content;
        }

        #endregion

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
                    roundPlayer.PlayerName = player.PlayerName;
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

        private void SetupSingleEliminationPlayers(ref List<TournamentMainPlayer> lstActiveTournamentPlayers, int intTopCut)
        {

            //If we're not passing in the top cut, we need the rankings of players for the next set (So if we've wrapped up the "top 4" with 2 tables, we just so happen to need to the "top 2")
            if (intTopCut == 0 && objTournMain.Rounds.Count > 0)
            {
                intTopCut = objTournMain.Rounds[objTournMain.Rounds.Count - 1].Tables.Count;
            }

            //Recalculate the latest scores, grab the top number of players that qualify for the number of tables
            Utilities.CalculatePlayerScores(ref objTournMain);
            List<TournamentMainPlayer> lstTmpPlayers = new List<TournamentMainPlayer>();

            lstTmpPlayers = objTournMain.Players.OrderBy(obj => obj.Rank).AsQueryable().Where(obj => obj.Rank <= (intTopCut)).ToList();

            while (lstTmpPlayers.Count > 0)
            {
                lstActiveTournamentPlayers.Add(lstTmpPlayers[0]);
                lstActiveTournamentPlayers.Add(lstTmpPlayers[lstTmpPlayers.Count - 1]);
                lstTmpPlayers.RemoveAt(0);
                lstTmpPlayers.RemoveAt(lstTmpPlayers.Count - 1);
            }
        }

        #endregion

        #region Update Table Info

        public string UpdateTableData(string table)
        {
            TournamentMainRoundTable result = JsonConvert.DeserializeObject<TournamentMainRoundTable>(table);

            try
            {

                //Update database
                var request = new RestRequest("TournamentsRounds/{userid}/{id}", Method.PUT);
                request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
                request.AddUrlSegment("id", result.RoundId);
                result.Player1Name = "";
                result.Player2Name = "";
                request.AddJsonBody(JsonConvert.SerializeObject(result));

                // execute the request
                IRestResponse response = client.Execute(request);
                var content = response.Content; // raw content as string


                if (content.ToUpper().Contains("PUT: SUCCESS"))
                {
                    foreach (TournamentMainRound round in objTournMain.Rounds)
                    {
                        if (round.Id == result.RoundId)
                        {
                            foreach (TournamentMainRoundTable rdTable in round.Tables)
                            {
                                if (rdTable.Id == result.Id)
                                {
                                    rdTable.Player1Score = result.Player1Score;
                                    rdTable.Player1Winner = result.Player1Winner;
                                    rdTable.Player2Score = result.Player2Score;
                                    rdTable.Player2Winner = result.Player2Winner;
                                    rdTable.ScoreTied = result.ScoreTied;
                                }
                            }
                            break;
                        }
                    }
                }

                return objTournActivity.GetStandings();
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("TournamentActivityController.UpdateTableData{0}Error:{1}", Environment.NewLine, ex.Message));
            }


            return "";

        }

        public string SwapTablePlayers(int roundId, int tableId, int originalPlayerId, int newPlayerId)
        {
            try
            {
                //Grab the tables we're going to need
                TournamentMainRoundTable table1 = null;
                TournamentMainRoundTable table2 = null;

                foreach (TournamentMainRound round in objTournMain.Rounds)
                {
                    if (round.Id == roundId)
                    {
                        foreach(TournamentMainRoundTable table in round.Tables)
                        {
                            if (table.Id == tableId)
                            {
                                table1 = table;
                            }
                            else
                            {
                                if (table.Player1Id == newPlayerId || table.Player2Id == newPlayerId)
                                {
                                    table2 = table;
                                }
                            }
                        }
                        break;
                    }
                }

                //If we're able to find valid tables, proceed with swapping players
                //table1 = starting table (from the table we're swapping said player)
                //table2 = new table (to the table we're moving said player)
                if (table1 != null && table2 != null)
                {
                    string strOriginalPlayerName = "";
                    string strNewPlayerName = "";

                    //Set variables to help facilitate the swap
                    if (table1.Player1Id == originalPlayerId)
                        strOriginalPlayerName = table1.Player1Name;
                    else
                        strOriginalPlayerName = table1.Player2Name;

                    if (table2.Player1Id == newPlayerId)
                        strNewPlayerName = table2.Player1Name;
                    else
                        strNewPlayerName = table2.Player2Name;


                    //Make the swap
                    if (table1.Player1Id == originalPlayerId)
                    {
                        table1.Player1Id = newPlayerId;
                        table1.Player1Name = strNewPlayerName;
                    }
                    else
                    {
                        table1.Player2Id = newPlayerId;
                        table1.Player2Name = strNewPlayerName;
                    }

                    if (table2.Player1Id == newPlayerId)
                    {
                        table2.Player1Id = originalPlayerId;
                        table2.Player1Name = strOriginalPlayerName;
                    }
                    else
                    {
                        table2.Player2Id = originalPlayerId;
                        table2.Player2Name = strOriginalPlayerName;
                    }

                    //Set table names
                    table1.TableName = string.Format("{0} vs {1}", table1.Player1Name, table1.Player2Name);
                    table2.TableName = string.Format("{0} vs {1}", table2.Player1Name, table2.Player2Name);


                    //Update database, update each table accordingly
                    foreach (TournamentMainRoundTable table in new List<TournamentMainRoundTable> { table1, table2 }){
                        var request = new RestRequest("TournamentsRounds/{userid}/{id}", Method.PUT);
                        request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
                        request.AddUrlSegment("id", table.RoundId);
                        request.AddJsonBody(JsonConvert.SerializeObject(table));

                        // execute the request
                        IRestResponse response = client.Execute(request);
                        var content = response.Content; // raw content as string
                    }

                    return "Player Swap SUCCESS";

                }                               
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("TournamentActivityController.SwapTablePlayers{0}Error:{1}", Environment.NewLine, ex.Message));
            }

            return "";
        }

        #endregion

        #region Delete Last Round

        public ActionResult DeleteRound(int roundId)
        {

            //Delete round
            try
            {
                if (tournamentId > 0)
                {
                    var request = new RestRequest("TournamentsRounds/{userid}/{id}", Method.DELETE);
                    request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
                    request.AddUrlSegment("id", roundId);

                    // execute the request
                    IRestResponse response = client.Execute(request);
                    var content = response.Content;
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("TournamentActivityController.DeleteRound{0}Delete Round Error:{1}", Environment.NewLine, ex.Message));
            }

            //Pull the tournament information to reload it correctly
            try
            {
                if (tournamentId > 0)
                {
                    var request = new RestRequest("Tournaments/{userid}/{id}", Method.GET);
                    request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
                    request.AddUrlSegment("id", tournamentId);

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
                Console.Write(string.Format("TournamentActivityController.DeleteRound{0}Get Tournament Error:{1}", Environment.NewLine, ex.Message));
            }

            objTournActivity.TournamentMain = objTournMain;

            return View(objTournActivity);
        }

        #endregion
    }
}