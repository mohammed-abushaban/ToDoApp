using System;
using System.Collections.Generic;

namespace ToDo.DbModel.Models
{
    public partial class ToDo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Content { get; set; }
        public int CreatorId { get; set; }
        public int AssignedId { get; set; }
        public bool IsRead { get; set; }
        public bool IsArchived { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual User Creator { get; set; }
    }
}