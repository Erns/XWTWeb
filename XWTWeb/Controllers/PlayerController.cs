using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XWTWeb.Models;

namespace XWTWeb.Controllers
{
    public class PlayerController : Controller
    {

        // GET: Player
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Main()
        {
            List<Player> players = new List<Player>();
            
            using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
            {
                using (SqlCommand sqlCmd = new SqlCommand("dbo.spPlayers_GET", sqlConn))
                {
                    sqlConn.Open();
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("@UserAccountId", 0);
                    using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            players.Add(new Player(
                                sqlReader.GetInt32(sqlReader.GetOrdinal("Id"))
                                , sqlReader.GetString(sqlReader.GetOrdinal("Name"))
                                , sqlReader.GetString(sqlReader.GetOrdinal("Email"))));
                        }
                    }
                }
            }

            ViewBag.Whatever = "Your player home page, whatever.";

            return View(players.ToList());
        }


        [HttpPost]
        public ActionResult Main(List<Player> players)
        {


            return View(players);
        }

    }
}