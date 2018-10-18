using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XWTWeb.Classes;
using XWTWeb.Models;

namespace XWTWeb.Controllers
{
    [RequireHttps]
    [Authorize]
    public class PlayerController : Controller
    {
        RestClient client = Utilities.InitializeRestClient();

        public ActionResult PlayerMain()
        {
            List<Player> players = new List<Player>();

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
                Console.Write(string.Format("PlayerController.Main{0}Error:{1}", Environment.NewLine, ex.Message));
            }

            return View(players.ToList());
        }


        public void UpdatePlayerData(string players)
        {
            List<Player> result = JsonConvert.DeserializeObject<List<Player>>(players);

            try
            {

                // client.Authenticator = new HttpBasicAuthenticator(username, password);

                var request = new RestRequest("Players/{userid}", Method.PUT);
                request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
                request.AddJsonBody(JsonConvert.SerializeObject(result));

                // execute the request
                IRestResponse response = client.Execute(request);
                var content = response.Content; // raw content as string

            }
            catch (Exception ex)
            {
                Console.Write(string.Format("PlayerController.UpdatePlayerData{0}Error:{1}", Environment.NewLine, ex.Message));
            }


            return;
            
        }

    }
}