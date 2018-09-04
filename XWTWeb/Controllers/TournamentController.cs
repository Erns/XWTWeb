using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XWTWeb.Models;

namespace XWTWeb.Controllers
{
    public class TournamentController : Controller
    {
        // GET: Tournament
        public ActionResult Main()
        {
            List<TournamentMain> tournaments = new List<TournamentMain>();


            return View(tournaments.ToList());
        }
    }
}