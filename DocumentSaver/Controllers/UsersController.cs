namespace DocumentSaver.Controllers;

using DocumentSaver.Authorization;
using DocumentSaver.Data.Entities;
using DocumentSaver.Models;
using DocumentSaver.Services;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController
{
    private IUserService _userService;
    private ILogService _logService;
    public UsersController(IUserService userService, ILogService logService)
    {
        _userService = userService;
        _logService = logService;
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<IActionResult> Authenticate(AuthenticateRequest model)
    {
        var result = new Result<AuthenticateResponse>();
        AuthenticateResponse response;
        try
        {

            response = _userService.Authenticate(model);
            result.Content = response;
        }
        catch(Exception ex)
        {
            if (ex.Message.StartsWith("Username"))
                result.Error = PopulateError(401, ex.Message, "Invalid Credentials");
                return StatusCode(401, result);
            result.Error = PopulateError(500, ex.Message, "Server Error");
            return StatusCode(500, result);
        }
        
        return Ok(result);
    }

    [Authorize(Role.Admin)]
    //[AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = new Result<List<User>>();
        List<User> response;
        try
        {
            var loggedInUser = GetAuthorizedUser();
            await _logService.AddLog(loggedInUser.Username, "Get Users");
            response = _userService.GetAll();
            result.Content = response;
        }
        catch (Exception ex)
        {
            result.Error = PopulateError(500, ex.Message, "Server Error");
        }

        return Ok(result);
    }

    [Authorize(Role.Admin)]
    //[AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<IActionResult> AddUser(RegisterRequest model)
    {
        var result = new Result<User>();
        User response;
        try
        {
            var loggedInUser = GetAuthorizedUser();
            await _logService.AddLog(loggedInUser.Username, "Add User");
            response = _userService.Register(model);
            result.Content = response;
        }
        catch (Exception ex)
        {
            result.Error = PopulateError(500, ex.Message, "Server Error");
        }

        return Ok(result);
    }

    [Authorize()]
    [HttpPost("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateRequest model)
    {
        var result = new Result<User>();
        User response;
        try
        {
            var user = GetAuthorizedUser();

            if (model.IsBlocked.HasValue && user.Role != Role.Admin)
            {
                result.Error = PopulateError(403, "You are not allowed to perform this action", "Forbidden");
            }

            if (model.Role.HasValue && user.Role != Role.Admin)
            {
                result.Error = PopulateError(403, "You are not allowed to perform this action", "Forbidden");
            }

            
            await _logService.AddLog(user.Username, "Update User");

            response = _userService.Update(id, model);
            result.Content = response;
        }
        catch (Exception ex)
        {
            result.Error = PopulateError(500, ex.Message, "Server Error");
        }

        return Ok(result);
    }

    [Authorize(Role.Admin)]
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        // only admins can access other user records
        var currentUser = (User)HttpContext.Items["User"];
        if (id != currentUser.Id && currentUser.Role != Role.Admin)
            return Unauthorized(new { message = "Unauthorized" });

        var result = new Result<User>();
        User response;
        try
        {
            response = _userService.GetById(id);
            result.Content = response;
        }
        catch (Exception ex)
        {
            result.Error = PopulateError(500, ex.Message, "Server Error");
        }

        return Ok(result);
    }

    private User GetAuthorizedUser()
    {
        var currentUser = (User)HttpContext.Items["User"];

        return currentUser;
    }
}