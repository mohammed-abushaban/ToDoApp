using ToDo.ModelView.ModelView;
using ToDo.ModelView.Request;

namespace ToDo.Core.Managers.Interfaces
{
    public interface ITodoManager
    {
        TodoResponseView GetTodos(int page = 1, int pageSize = 10, string sortColumn = "",
            string sortDirection = "ascending", string searchText = "");
        TodoResponseView GetIsRead(int page = 1, int pageSize = 10, string sortColumn = "", string sortDirection = "ascending", string searchText = "");

        TodoModelView GetTodo(int id);
        TodoModelView CreateTodo(UserModelView currentUser, TodoRequest todoRequest);
        TodoModelView PutTodo(UserModelView currentUser, TodoRequest todoRequest);
        TodoModelView AssignTodo(UserModelView currentUser, TodoAssign todoAssign);
        void ChangeIsRead(UserModelView currentUser, int id);
        void ArchiveTodo(UserModelView currentUser, int id);
    }
}
