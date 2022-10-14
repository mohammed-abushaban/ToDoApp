using AutoMapper;
using ToDo.Common.Extensions;
using ToDo.DbModel.Models;
using ToDo.ModelView.ModelView;

namespace ToDo.Core.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModelView>().ReverseMap();
            CreateMap<User, LoginUserResponseView>().ReverseMap();
            CreateMap<User, UserResultView>().ReverseMap();
            CreateMap<DbModel.Models.ToDo, TodoResultView>().ReverseMap();
            CreateMap<DbModel.Models.ToDo, TodoModelView>().ReverseMap();
            CreateMap<PagedResult<TodoModelView>, PagedResult<DbModel.Models.ToDo>>().ReverseMap();
            CreateMap<PagedResult<UserModelView>, PagedResult<User>>().ReverseMap();
            CreateMap<TodoModelView, UserModelView>().ReverseMap();
        }
    }
}
