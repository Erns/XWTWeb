using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XWTWeb.Models
{
    public class TournamentActivity
    {
        public List<Player> AllPlayers { get; set; }
        public TournamentMain TournamentMain { get; set; }
    }
}