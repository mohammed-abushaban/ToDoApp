using System.ComponentModel;

namespace ToDo.ModelView.ModelView
{
    public class TodoResultView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [DefaultValue("")]
        public string Image { get; set; }
        public string Content { get; set; }
        public int CreatorId { get; set; }
        public int AssignedId { get; set; }
        public bool IsRead { get; set; }
    }
}
