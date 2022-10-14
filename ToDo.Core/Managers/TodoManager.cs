using AutoMapper;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using ToDo.Common.Extceptions;
using ToDo.Common.Extensions;
using ToDo.Common.Helper;
using ToDo.Core.Managers.Interfaces;
using ToDo.DbModel.Data;
using ToDo.DbModel.Models;
using ToDo.ModelView.ModelView;
using ToDo.ModelView.Request;

namespace ToDo.Core.Managers
{
    public class TodoManager : ITodoManager
    {
        private readonly ToDoContext _toDoContext;
        private IMapper _mapper;

        public TodoManager(ToDoContext toDoContext, IMapper mapper)
        {
            _toDoContext = toDoContext;
            _mapper = mapper;
        }
        public TodoModelView GetTodo(int id)
        {
            var todo = _toDoContext.ToDo.Include("Creator").FirstOrDefault(a => a.Id == id);
            if(todo == null)
                 throw new ServiceValidationException("Invalid todo id received");
            return _mapper.Map<TodoModelView>(todo);
        }

        public TodoResponseView GetTodos(int page = 1, int pageSize = 10, string sortColumn = "", string sortDirection = "ascending", string searchText = "")
        {
            var queryRes = _toDoContext.ToDo.Where(a => string.IsNullOrWhiteSpace(searchText)
                || (a.Title.Contains(searchText)
                || a.Content.Contains(searchText)));

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortDirection.Equals("ascending", StringComparison.InvariantCultureIgnoreCase))
            {
                queryRes = queryRes.OrderBy(sortColumn);
            }
            else if (!string.IsNullOrWhiteSpace(sortColumn) && sortDirection.Equals("descending", StringComparison.InvariantCultureIgnoreCase))
            {
                queryRes = queryRes.OrderByDescending(sortColumn);
            }

            var res = queryRes.GetPaged(page, pageSize);

            var userIds = res.Data.Select(a => a.CreatorId).Distinct().ToList();

            var users = _toDoContext.User.Where(a => userIds.Contains(a.Id))
                .ToDictionary(a => a.Id, x => _mapper.Map<UserResultView>(x));

            var data = new TodoResponseView()
            {
                Todo = _mapper.Map<PagedResult<TodoModelView>>(res),
                User = users
            };

            data.Todo.Sortable.Add("Title", "Title");
            data.Todo.Sortable.Add("CreatedDate", "Created Date");

            return data;
        }

        public TodoResponseView GetIsRead(int page = 1, int pageSize = 10, string sortColumn = "", string sortDirection = "ascending", string searchText = "")
        {
            _toDoContext.IgnoreIsRead = true;

            var isRead = _toDoContext.ToDo.Where(a => a.IsRead == true);
            var queryRes = isRead.Where(a => string.IsNullOrWhiteSpace(searchText)
                || (a.Title.Contains(searchText)
                || a.Content.Contains(searchText)));

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortDirection.Equals("ascending", StringComparison.InvariantCultureIgnoreCase))
            {
                queryRes = queryRes.OrderBy(sortColumn);
            }
            else if (!string.IsNullOrWhiteSpace(sortColumn) && sortDirection.Equals("descending", StringComparison.InvariantCultureIgnoreCase))
            {
                queryRes = queryRes.OrderByDescending(sortColumn);
            }

            var res = queryRes.GetPaged(page, pageSize);

            var userIds = res.Data.Select(a => a.CreatorId).Distinct().ToList();

            var users = _toDoContext.User.Where(a => userIds.Contains(a.Id))
                .ToDictionary(a => a.Id, x => _mapper.Map<UserResultView>(x));

            var data = new TodoResponseView()
            {
                Todo = _mapper.Map<PagedResult<TodoModelView>>(res),
                User = users
            };

            data.Todo.Sortable.Add("Title", "Title");
            data.Todo.Sortable.Add("CreatedDate", "Created Date");

            return data;
        }

        public TodoModelView CreateTodo(UserModelView currentUser, TodoRequest todoRequest)
        {
            var todo = _toDoContext.ToDo.Add(new DbModel.Models.ToDo
            {
                Title = todoRequest.Title,
                Image = todoRequest.Image,
                Content = todoRequest.Content,
                CreatorId = currentUser.Id,
                AssignedId = currentUser.Id,
            }).Entity;

            _toDoContext.SaveChanges();
            return _mapper.Map<TodoModelView>(todo);
        }

        public TodoModelView PutTodo(UserModelView currentUser, TodoRequest todoRequest)
        {
            var assignedId = _toDoContext.ToDo.FirstOrDefault(a=>a.Id==todoRequest.Id);

            if (!currentUser.IsAdmin && assignedId.AssignedId != currentUser.Id)
            {
                throw new ServiceValidationException("You dont have permission to edit this todo");
            }

            var todo = _toDoContext.ToDo.FirstOrDefault(a => a.Id == todoRequest.Id);
               if(todo == null)
                throw new ServiceValidationException("Invalid todo id received");
            var url = "";

            if (!string.IsNullOrWhiteSpace(todoRequest.ImageString))
            {
                url = SaveFiles.SaveImage(todoRequest.ImageString, "Images/TodoImages");
            }
            todo.Title = todoRequest.Title;
            todo.Content = todoRequest.Content;
            if (!string.IsNullOrWhiteSpace(url))
            {
                var baseUrl = "https://localhost:44377";
                todo.Image = $@"{baseUrl}/api/v1/user/fileretrive/todopic?filename={url}";
            }
            _toDoContext.SaveChanges();
            return _mapper.Map<TodoModelView>(todo);
        }

        public TodoModelView AssignTodo(UserModelView currentUser, TodoAssign todoAssign)
        {
            _toDoContext.IgnoreFilter = true;
            var todo = _toDoContext.ToDo.FirstOrDefault(a => a.Id == todoAssign.Id);
            if (todo == null)
                throw new ServiceValidationException("Invalid todo id received");
            todo.AssignedId = todoAssign.AssignedId;

            _toDoContext.SaveChanges();
            return _mapper.Map<TodoModelView>(todo);
        }

        public void ChangeIsRead(UserModelView currentUser, int id)
        {
            var assignedId = _toDoContext.ToDo.FirstOrDefault(a => a.Id == id);
            if (!currentUser.IsAdmin && assignedId.AssignedId != currentUser.Id)
            {
                throw new ServiceValidationException("You dont have permission to edit this todo");
            }
            var data = _toDoContext.ToDo.FirstOrDefault(a => a.Id == id);
            if (data == null)
                throw new ServiceValidationException("Invalid todo id received");
            data.IsRead = true;
            _toDoContext.SaveChanges();
        }

        public void ArchiveTodo(UserModelView currentUser, int id)
        {
            _toDoContext.IgnoreFilter = true;
            var data = _toDoContext.ToDo.FirstOrDefault(a => a.Id == id);
             if (data == null)
                throw new ServiceValidationException("Invalid todo id received");
            data.IsArchived = true;
            _toDoContext.SaveChanges();
        }
    }
}
