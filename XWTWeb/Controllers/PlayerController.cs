using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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

        [Authorize]
        public string GetPlayerData()
        {
            List<Player> players = new List<Player>();
            string content = "";

            try
            {

                var request = new RestRequest("Players/{userid}", Method.GET);
                request.AddUrlSegment("userid", Utilities.CurrentUser.Id);

                // execute the request
                IRestResponse response = client.Execute(request);
                content = response.Content;

                List<Player> result = JsonConvert.DeserializeObject<List<Player>>(JsonConvert.DeserializeObject(content).ToString());
                foreach (Player player in result)
                {
                    players.Add(player);
                }
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("PlayerController.GetPlayerData{0}Error:{1}", Environment.NewLine, ex.Message));
            }

            return JsonConvert.SerializeObject(players);
        }

        [Authorize]
        public void UpdatePlayerData()
        {

            try
            {

                using (Stream stream = Request.InputStream)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var players = reader.ReadToEnd();
                        List<Player> result = JsonConvert.DeserializeObject<List<Player>>(players);

                        foreach(Player player in result)
                        {
                            if (player.Id < 0)
                                player.Id = 0;

                        }

                        //client.Authenticator = new HttpBasicAuthenticator(username, password);

                        var request = new RestRequest("Players/{userid}", Method.PUT);
                        request.AddUrlSegment("userid", Utilities.CurrentUser.Id);
                        request.AddJsonBody(JsonConvert.SerializeObject(result));

                        // execute the request
                        IRestResponse response = client.Execute(request);
                        var content = response.Content; // raw content as string
                    }
                }



            }
            catch (Exception ex)
            {
                Console.Write(string.Format("PlayerController.UpdatePlayerData{0}Error:{1}", Environment.NewLine, ex.Message));
            }


            return;
            
        }

    }
}
