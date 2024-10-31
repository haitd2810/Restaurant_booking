using System;
using System.Collections.Generic;

namespace DataLibrary.Models
{
    public partial class Account
    {
        public Account()
        {
            Tokens = new HashSet<Token>();
        }

        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? DeleteAt { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Token> Tokens { get; set; }
    }
}
