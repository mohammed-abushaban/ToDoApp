using System.Linq;
using ToDo.Core.Managers.Interfaces;
using ToDo.DbModel.Data;
using ToDo.ModelView.ModelView;

namespace ToDo.Core.Managers
{
    public class RoleManager : IRoleManager
    {
        private readonly ToDoContext _toDoContext;

        public RoleManager(ToDoContext toDoContext)
        {
            _toDoContext = toDoContext;
        }

        public bool CheckAccess(UserModelView userModel)
        {
            return _toDoContext.User.Any(x => x.Id == userModel.Id && x.IsAdmin);
        }
    }
}
