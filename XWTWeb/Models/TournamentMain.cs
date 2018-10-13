using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XWTWeb.Models
{
    public class TournamentMain
    {

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<DateTime> StartDate { get; set; } = null;
        public int MaxPoints { get; set; }
        public int RoundTimeLength { get; set; }
        public Nullable<DateTime> DateDeleted { get; set; } = null;

        public List<TournamentMainPlayer> Players { get; set; } = new List<TournamentMainPlayer>();

        public List<TournamentMainRound> Rounds { get; set; } = new List<TournamentMainRound>();

        public List<int> ActivePlayersList()
        {
            List<int> lstIDs = new List<int>();
            foreach (TournamentMainPlayer item in Players)
            {
                if (item.Active) lstIDs.Add(item.PlayerId);
            }

            //return String.Join(",", lstIDs.ToArray());
            return lstIDs;
        }

        public TournamentMain()
        {

        }

        public TournamentMain(int Id = 0, string Name = "", DateTime? StartDate = null, int MaxPoints = 0, int RoundTimeLength = 0)
        {
            this.Id = Id;
            this.Name = Name;
            this.StartDate = StartDate;
            this.MaxPoints = MaxPoints;
            this.RoundTimeLength = RoundTimeLength;
        }
    }

    public class TournamentMainPlayer
    {
        [Key]
        public int Id { get; set; }

        //FK
        public int TournamentId { get; set; }

        //FK
        public int PlayerId { get; set; }

        public List<int> OpponentIds { get; set; } = new List<int>();
        public string OpponentIdsBlobbed { get; set; }

        public string PlayerName { get; set; }
        public bool Active { get; set; } = true;
        public bool Bye { get; set; } = false;
        public int ByeCount { get; set; } = 0;

        public int RoundsPlayed { get; set; } = 0;
        public int Rank { get; set; } = 0;
        public int Score { get; set; } = 0;
        public int MOV { get; set; } = 0;
        public decimal SOS { get; set; } = 0;

    }

    public class TournamentMainRound
    {
        [Key]
        public int Id { get; set; }

        //FK
        public int TournamentId { get; set; }
        public int Number { get; set; }
        public bool Swiss { get; set; } = true;
        public Nullable<DateTime> RoundTimeEnd { get; set; } = null;

        public List<TournamentMainRoundTable> Tables { get; set; } = new List<TournamentMainRoundTable>();

    }
    
    public class TournamentMainRoundTable
    {
        [Key]
        public int Id { get; set; }

        //Table general info
        //FK
        public int RoundId { get; set; }
        public int Number { get; set; }
        public string TableName { get; set; }
        public bool ScoreTied { get; set; } = true;
        public bool Bye { get; set; }

        //Player 1 Info
        //FK
        public int Player1Id { get; set; } = 0;
        public string Player1Name { get; set; } = "N/A";
        public int Player1Score { get; set; }
        public bool Player1Winner { get; set; } = false;

        //Player 2 info
        //FK
        public int Player2Id { get; set; } = 0;
        public string Player2Name { get; set; } = "N/A";
        public int Player2Score { get; set; }
        public bool Player2Winner { get; set; } = false;

    }

}