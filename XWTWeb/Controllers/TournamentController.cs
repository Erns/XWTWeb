using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XWTWeb.Classes;
using XWTWeb.Models;

namespace XWTWeb.Controllers
{
    [RequireHttps]
    [Authorize]
    public class TournamentController : Controller
    {

        RestClient client = Utilities.InitializeRestClient();

        // GET: Tournament
        public ActionResult Main()
        {
            List<TournamentMain> tournaments = new List<TournamentMain>();

            try
            {

                var request = new RestRequest("Tournaments/{userid}", Method.GET);
                request.AddUrlSegment("userid", Utilities.CurrentUser.Id);

                // execute the request
                IRestResponse response = client.Execute(request);
                var content = response.Content;

                List<TournamentMain> result = JsonConvert.DeserializeObject<List<TournamentMain>>(JsonConvert.DeserializeObject(content).ToString());
                foreach (TournamentMain tournament in result)
                {
                    tournaments.Add(tournament);
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("PlayerController.Main{0}Error:{1}", Environment.NewLine, ex.Message));
            }

            return View(tournaments.ToList());
        }


        public ActionResult AddEdit(int id)
        {

            TournamentMain tournament = new TournamentMain(id, "", null, 200, 75);

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
            catch(Exception ex)
            {
                Console.Write(string.Format("PlayerController.Main{0}AddEdit:{1}", Environment.NewLine, ex.Message));
            }

            return View(tournament);
        }

        [HttpPost]
        public ActionResult AddEdit(TournamentMain tournament)
        {

            try
            {
                //TODO:  Correct userid/id shit
                var request = new RestRequest("Tournaments/{userid}/{id}", Method.PUT);
                request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
                request.AddUrlSegment("id", tournament.Id);
                request.AddJsonBody(JsonConvert.SerializeObject(tournament));

                // execute the request
                IRestResponse response = client.Execute(request);
                var content = response.Content;

            }
            catch (Exception ex)
            {
                Console.Write(string.Format("TournamentController.AddEdit{0}Error:{1}", Environment.NewLine, ex.Message));
            }

            return RedirectToAction("Main", "Tournament");
        }
    }
}