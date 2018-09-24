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
            

            return View(tournaments.ToList());
        }


        public ActionResult AddEdit()
        {

            TournamentMain tournament = new TournamentMain();
            tournament.MaxPoints = 200;
            tournament.RoundTimeLength = 75;

            return View(tournament);
        }

        [HttpPost]
        public ActionResult AddEdit(TournamentMain tournament)
        {

            try
            {

                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                //client.Authenticator = new HttpBasicAuthenticator(Utilities.CurrentUser.UserName, Utilities.CurrentUser.APIPassword);

                //var request = new RestRequest("Tournaments/{id}", Method.POST);
                //request.AddUrlSegment("id", Utilities.CurrentUser.Id);

                //// execute the request
                //IRestResponse response = client.Execute(request);
                //var content = response.Content;

                //List<Player> result = JsonConvert.DeserializeObject<List<Player>>(JsonConvert.DeserializeObject(content).ToString());
                //foreach (Player player in result)
                //{
                //    players.Add(new Player(player.Id, player.Name, player.Email, player.Group));
                //}
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("PlayerController.Main{0}Error:{1}", Environment.NewLine, ex.Message));
            }

            return RedirectToAction("Main", "Tournament");
        }
    }
}