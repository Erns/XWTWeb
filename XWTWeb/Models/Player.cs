using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XWTWeb.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Group { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; } = true;
        public Nullable<DateTime> DateDeleted { get; set; } = null;
        public bool CanEdit { get; set; } = false;

        public string OriginalName { get; set; }
        public string OriginalEmail { get; set; }
        public string OriginalGroup { get; set; }

        public Player(int Id, string Name, string Email = "", string Group = "")
        {
            this.Id = Id;
            this.Name = Name;
            this.Email = Email;
            this.Group = Group;

            this.OriginalName = Name;
            this.OriginalEmail = Email;
            this.OriginalGroup = Group;
        }
    }
}
