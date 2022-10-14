using ToDo.ModelView.ModelView;

namespace ToDo.Core.Managers.Interfaces
{
    public interface ICommonManager : IManager
    {
        UserModelView GetUserRole(UserModelView user);
    }
}
