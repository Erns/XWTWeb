using System;
using System.Collections.Generic;
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
            ViewBag.Whatever = "Your player home page, whatever.";

            List<Player> players = new List<Player>();
            players.Add(new Player(1, "Test 1"));
            players.Add(new Player(2, "test @"));
            players.Add(new Player(3, "Test 3", "Eamail@email.com"));

            return View(players);
        }


    }
}