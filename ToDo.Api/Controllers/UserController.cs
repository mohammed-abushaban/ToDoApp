﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using ToDo.Api.Attributes;
using ToDo.Core.Managers.Interfaces;
using ToDo.ModelView.ModelView;

namespace ToDo.Api.Controllers
{
    [ApiController]
    public class UserController : ApiBaseController
    {
        private IUserManager _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger, IUserManager userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [Route("api/user/getall")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ToDoAuthorize()]
        public IActionResult GetUsers(int page = 1, int pageSize = 10, string sortColumn = "", string sortDirection = "ascending", string searchText = "")
        {
            var result = _userManager.GetUsers(page, pageSize, sortColumn, sortDirection, searchText);
            return Ok(result);
        }

        [Route("api/user/get/{id}")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ToDoAuthorize()]
        public IActionResult GetUser(int id)
        {
            var result = _userManager.GetUser(id);
            return Ok(result);
        }

        [Route("api/user/register")]
        [HttpPost]
        [AllowAnonymous]
        public IActionResult SignUp([FromBody] UserRegisterView userReg)
        {
            var res = _userManager.SignUp(userReg);
            return Ok(res);
        }

        [Route("api/user/login")]
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody] UserLoginView userReg)
        {
            var res = _userManager.Login(userReg);
            return Ok(res);
        }

        [Route("api/user/fileretrieve/profilepic")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Retrive(string filename)
        {
            var folderPath = Directory.GetCurrentDirectory();
            folderPath = $@"{folderPath}\{filename}";
            var byteArray = System.IO.File.ReadAllBytes(folderPath);
            return File(byteArray, "image/jpeg", filename);
        }

        [Route("api/user/myaccount")]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult UpdateMyProfile(UserModelView request)
        {
            var user = _userManager.UpdateProfile(LoggedInUser, request);
            return Ok(user);
        }

        [Route("api/user/assignadmin/{id}")]
        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ToDoAuthorize()]
        public IActionResult AssignAdmin(int id)
        {
            _userManager.AssignAdmin(id);
            return Ok();
        }

        [Route("api/user/delete/{id}")]
        [HttpDelete]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [ToDoAuthorize()]
        public IActionResult Delete(int id)
        {
            _userManager.DeleteUser(LoggedInUser, id);
            return Ok();
        }
    }
}
