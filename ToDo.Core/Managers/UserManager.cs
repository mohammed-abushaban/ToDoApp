using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using ToDo.Common.Extceptions;
using ToDo.Common.Extensions;
using ToDo.Common.Helper;
using ToDo.Core.Managers.Interfaces;
using ToDo.DbModel.Data;
using ToDo.DbModel.Models;
using ToDo.ModelView.ModelView;

namespace ToDo.Core.Managers
{
    public class UserManager : IUserManager
    {
        private readonly ToDoContext _toDoContext;
        private IMapper _mapper;
        public UserManager(ToDoContext toDoContext, IMapper mapper)
        {
            _toDoContext = toDoContext;
            _mapper = mapper;
        }

        public UserModelView GetUser(int id)
        {
            var user = _toDoContext.User.FirstOrDefault(a => a.Id == id);
            if(user == null)
                 throw new ServiceValidationException("Invalid user id received");
            return _mapper.Map<UserModelView>(user);
        }

        public UserResponseView GetUsers(int page = 1, int pageSize = 10, string sortColumn = "", string sortDirection = "ascending", string searchText = "")
        {
            var queryRes = _toDoContext.User.Where(a => string.IsNullOrWhiteSpace(searchText)
                 || (a.FirstName.Contains(searchText)
                 || a.LastName.Contains(searchText)));

            if (!string.IsNullOrWhiteSpace(sortColumn) && sortDirection.Equals("ascending", StringComparison.InvariantCultureIgnoreCase))
            {
                queryRes = queryRes.OrderBy(sortColumn);
            }
            else if (!string.IsNullOrWhiteSpace(sortColumn) && sortDirection.Equals("descending", StringComparison.InvariantCultureIgnoreCase))
            {
                queryRes = queryRes.OrderByDescending(sortColumn);
            }

            var res = queryRes.GetPaged(page, pageSize);

            var todoIds = res.Data.Select(a => a.Id).Distinct().ToList();

            var todos = _toDoContext.ToDo.Where(a => todoIds.Contains(a.AssignedId)).ToDictionary(a => a.Id, x => _mapper.Map<TodoResultView>(x));

            var data = new UserResponseView()
            {
                User = _mapper.Map<PagedResult<UserModelView>>(res),
                Todo = todos
            };

            data.User.Sortable.Add("Email", "Email");
            data.User.Sortable.Add("CreatedDate", "Created Date");

            return data;
        }

        public LoginUserResponseView Login(UserLoginView userReg)
        {
            var user = _toDoContext.User.FirstOrDefault(a => a.Email.Equals(userReg.Email, StringComparison.InvariantCultureIgnoreCase));

            if (user == null || !VerifyHashPassword(userReg.Password, user.Password))
            {
                throw new ServiceValidationException(300, "Invalid email or password");
            }

            var map = _mapper.Map<LoginUserResponseView>(user);
            map.Token = $"Bearer {GenerateJWTToken(user)}";
            return map;
        }

        public LoginUserResponseView SignUp(UserRegisterView userReg)
        {
            if (_toDoContext.User.Any(a => a.Email.Equals(userReg.Email,
                    StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ServiceValidationException("User Already Exist");
            }

            var hashedPassword = HashPassword(userReg.Password);

            var user = _toDoContext.User.Add(new User
            {
                FirstName = userReg.FirstName,
                LastName = userReg.LastName,
                Email = userReg.Email,
                Password = hashedPassword,
                Image = string.Empty
            }).Entity;

            _toDoContext.SaveChanges();

            var res = _mapper.Map<LoginUserResponseView>(user);
            res.Token = $"Bearer {GenerateJWTToken(user)}";
            return res;
        }

        public UserModelView UpdateProfile(UserModelView currentUser, UserModelView request)
        {
            var user = _toDoContext.User.FirstOrDefault(a => a.Id == currentUser.Id);
            if(user == null)
               throw new ServiceValidationException("User not found");
            var url = "";

            if (!string.IsNullOrWhiteSpace(request.ImageString))
            {
                url = SaveFiles.SaveImage(request.ImageString, "Images/UserImages");
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            if (!string.IsNullOrWhiteSpace(url))
            {
                var baseUrl = "https://localhost:44377";
                user.Image = $@"{baseUrl}/api/v1/user/fileretrive/profilepic?filename={url}";
            }
            _toDoContext.SaveChanges();
            return _mapper.Map<UserModelView>(user);
        }

        public void AssignAdmin(int id)
        {
            var user = _toDoContext.User.FirstOrDefault(a => a.Id == id);
            if(user == null)
                throw new ServiceValidationException("User not found");
            user.IsAdmin = true;
            _toDoContext.SaveChanges();
        }

        public void DeleteUser(UserModelView currentUser, int id)
        {
            if (currentUser.Id == id)
            {
                throw new ServiceValidationException("You have no access to delete yourself");
            }

            var user = _toDoContext.User.FirstOrDefault(a => a.Id == id);
            if(user == null)
                throw new ServiceValidationException("User not found");
            user.IsArchived = true;
            _toDoContext.SaveChanges();
        }

        private static string HashPassword(string password)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return hashedPassword;
        }

        private static bool VerifyHashPassword(string password, string HashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, HashedPassword);
        }

        private string GenerateJWTToken(User user)
        {
            var jwtKey = "#test.key*&^vanthis%$^&*()$%^@#$@!@#%$#^%&*%^*";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Id", user.Id.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("DateOfJoining", user.CreatedDate.ToString("yyyy-MM-dd")),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var issuer = "test.com";

            var token = new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.Now.AddDays(20),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}