using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XWTWeb.Models;

namespace XWTWeb.Controllers
{
    public class PlayerController : Controller
    {

        public ActionResult Main()
        {
            List<Player> players = new List<Player>();

            try
            {
                //using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                //{
                //    using (SqlCommand sqlCmd = new SqlCommand("dbo.spPlayers_GET", sqlConn))
                //    {
                //        sqlConn.Open();
                //        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                //        sqlCmd.Parameters.AddWithValue("@UserAccountId", 0);
                //        using (SqlDataReader sqlReader = sqlCmd.ExecuteReader())
                //        {
                //            while (sqlReader.Read())
                //            {
                //                players.Add(new Player(
                //                    sqlReader.GetInt32(sqlReader.GetOrdinal("Id"))
                //                    , sqlReader.GetString(sqlReader.GetOrdinal("Name"))
                //                    , sqlReader.GetString(sqlReader.GetOrdinal("Email"))));
                //            }
                //        }
                //    }
                //}

                players.Add(new Player(1, "Test 1", ""));
                players.Add(new Player(2, "test 2", "asdf@asdf.com"));
                players.Add(new Player(3, "test 3","", "Sparta"));

            }
            catch (Exception ex)
            {
                Console.Write(string.Format("PlayerController.Main{0}Error:{1}", Environment.NewLine, ex.Message));
            }



            ViewBag.Whatever = "Your player home page, whatever.";

            return View(players.ToList());
        }


        public void UpdatePlayerData(string players)
        {
            List<Player> result = JsonConvert.DeserializeObject<List<Player>>(players);

            //var model = new JavaScriptSerializer().Deserialize<Player>(players);

            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Group", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("Active", typeof(bool));

            foreach (Player player in result)
            {
                dt.Rows.Add(player.Id, player.Name, player.Group, player.Email, player.Active);
            }


            try
            {
                //using (SqlConnection sqlConn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["XWTWebConnectionString"].ToString()))
                //{
                //    using (SqlCommand sqlCmd = new SqlCommand("dbo.spPlayers_UPDATEINSERT_ALL", sqlConn))
                //    {
                //        sqlConn.Open();
                //        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                //        sqlCmd.Parameters.AddWithValue("@UserAccountId", 0);

                //        //or-- sqlCmd.Parameters.Add("@dt", SqlDbType.Structured).Value = dt;
                //        sqlCmd.ExecuteNonQuery();

                //    }
                //}
            }
            catch (Exception ex)
            {
                Console.Write(string.Format("PlayerController.UpdatePlayerData{0}Error:{1}", Environment.NewLine, ex.Message));
            }


            var test = "";
            return;
            
        }

    }
}