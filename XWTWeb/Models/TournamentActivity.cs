using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XWTWeb.Classes;

namespace XWTWeb.Models
{
    public class TournamentActivity
    {
        public List<Player> AllPlayers { get; set; }
        public List<Player> NextRoundPlayers { get; set; }
        public TournamentMain TournamentMain { get; set; }

        public int GetPlayerTotal()
        {
            return AllPlayers.Count + NextRoundPlayers.Count;
        }

        public string GetStandings()
        {
            TournamentMain objTournMain = new TournamentMain();
            objTournMain = TournamentMain;

            Utilities.CalculatePlayerScores(ref objTournMain);

            string strHTML = "";
            foreach (TournamentMainPlayer player in objTournMain.Players.OrderBy(obj => obj.Rank).ToList())
            {
                strHTML += "<tr>";
                strHTML += string.Format("<td>{0}</td>", player.Rank);
                strHTML += string.Format("<td>{0}</td>", player.PlayerName);
                strHTML += string.Format("<td>{0}</td>", player.Score);
                strHTML += string.Format("<td>{0}</td>", player.MOV);
                strHTML += string.Format("<td>{0}</td>", player.SOS);
                strHTML += "</tr>";
            }
            return strHTML;
        }
    }
}