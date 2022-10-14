using System.Collections.Generic;
using ToDo.Common.Extensions;

namespace ToDo.ModelView.ModelView
{
    public class TodoResponseView
    {
        public PagedResult<TodoModelView> Todo { get; set; }
        public Dictionary<int, UserResultView> User { get; set; }
    }
}
