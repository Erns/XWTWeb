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
        public ActionResult TournamentMain()
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
                Console.Write(string.Format("PlayerController.TournamentMain{0}Error:{1}", Environment.NewLine, ex.Message));
            }

            return View(tournaments.ToList());
        }

        public ActionResult DeleteTournament(int id)
        {


            try
            {
                if (id > 0)
                {
                    var request = new RestRequest("Tournaments/{userid}/{id}", Method.DELETE);
                    request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
                    request.AddUrlSegment("id", id);

                    // execute the request
                    IRestResponse response = client.Execute(request);
                    var content = response.Content;

                    //List<TournamentMain> result = JsonConvert.DeserializeObject<List<TournamentMain>>(JsonConvert.DeserializeObject(content).ToString());

                    ////Didn't actually get a result (such as trying to access a tournament not "owned" by user).  Kick back to Main page
                    //if (result.Count == 0)
                    //    return RedirectToAction("TournamentMain", "Tournament");
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("PlayerController.Main{0}DeleteTournament:{1}", Environment.NewLine, ex.Message));
                return RedirectToAction("TournamentMain", "Tournament");
            }

            return RedirectToAction("TournamentMain", "Tournament");
        }


        public ActionResult TournamentAddEdit(int id)
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

                    //Didn't actually get a result (such as trying to access a tournament not "owned" by user).  Kick back to Main page
                    if (result.Count == 0)
                        return RedirectToAction("TournamentMain", "Tournament");
                }
            }
            catch(Exception ex)
            {
                Console.Write(string.Format("PlayerController.Main{0}TournamentAddEdit:{1}", Environment.NewLine, ex.Message));
                return RedirectToAction("TournamentMain", "Tournament");
            }

            return View(tournament);
        }


        [HttpPost]
        public ActionResult TournamentAddEdit(TournamentMain tournament)
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
                Console.Write(string.Format("TournamentController.TournamentAddEdit{0}Error:{1}", Environment.NewLine, ex.Message));
            }

            return RedirectToAction("TournamentMain", "Tournament");
        }
    }
}