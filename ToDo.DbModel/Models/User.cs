
using System;
using System.Collections.Generic;

namespace ToDo.DbModel.Models
{
    public partial class User
    {
        public User()
        {
            ToDo = new HashSet<ToDo>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<ToDo> ToDo { get; set; }
    }
}