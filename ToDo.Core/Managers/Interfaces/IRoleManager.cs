using ToDo.ModelView.ModelView;

namespace ToDo.Core.Managers.Interfaces
{
    public interface IRoleManager : IManager
    {
        bool CheckAccess(UserModelView userModelView);
    }
}
