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

        // GET: TournamentActivity
        public ActionResult Main(int id)
        {

            TournamentActivity tournamentActivity = new TournamentActivity();

            TournamentMain tournament = new TournamentMain();
            List<Player> players = new List<Player>();

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
                        tournament = tmpTournament;
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
                    players.Add(player);
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("TournamentActivityController.Main{0}Get Players Error:{1}", Environment.NewLine, ex.Message));
            }

            tournamentActivity.AllPlayers = players;
            tournamentActivity.TournamentMain = tournament;

            return View(tournamentActivity);
        }
    }
}