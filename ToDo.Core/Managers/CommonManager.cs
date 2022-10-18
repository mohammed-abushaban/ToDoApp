using AutoMapper;
using System.Linq;
using ToDo.Common.Extceptions;
using ToDo.Core.Managers.Interfaces;
using ToDo.DbModel.Data;
using ToDo.ModelView.ModelView;

namespace ToDo.Core.Managers
{
    public class CommonManager : ICommonManager
    {
        private readonly ToDoContext _toDoContext;
        private readonly IMapper _mapper;

        public CommonManager(ToDoContext toDoContext, IMapper mapper)
        {
            _toDoContext = toDoContext;
            _mapper = mapper;
        }

        public UserModelView GetUserRole(UserModelView user)
        {
            var dbUser = _toDoContext.User.FirstOrDefault(x => x.Id == user.Id) ??
                    throw new ServiceValidationException("Invalid user id");

            return _mapper.Map<UserModelView>(dbUser);
        }
    }
}
