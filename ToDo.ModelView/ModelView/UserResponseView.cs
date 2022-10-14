using System.Collections.Generic;
using ToDo.Common.Extensions;

namespace ToDo.ModelView.ModelView
{
    public class UserResponseView
    {
        public PagedResult<UserModelView> User { get; set; }
        public Dictionary<int, TodoResultView> Todo { get; set; }
    }
}
