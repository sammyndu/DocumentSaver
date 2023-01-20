using DocumentSaver.Authorization;
using DocumentSaver.Data;
using DocumentSaver.Data.Entities;
using DocumentSaver.Helpers;
using DocumentSaver.Models;
using BCrypt.Net;
using Microsoft.Extensions.Options;

namespace DocumentSaver.Services;

    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        List<User> GetAll();
        User GetById(int id);
        User Register(RegisterRequest model);
        User Update(int id, UpdateRequest model);
        void Delete(int id);
    }

    public class UserService : IUserService
    {
        private AppDbContext _context;
        private IJwtUtils _jwtUtils;
        private readonly AppSettings _appSettings;

        public UserService(
            AppDbContext context,
            IJwtUtils jwtUtils,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _jwtUtils = jwtUtils;
            _appSettings = appSettings.Value;
        }


        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _context.Users.SingleOrDefault(x => x.Username == model.Username);



            // validate
            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                throw new AppException("Username or password is incorrect");

            // authentication successful so generate jwt token
            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken);
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }


    public User GetById(int id)
    {
        return getUser(id);
    }

    public User Register(RegisterRequest model)
    {
        // validate
        if (_context.Users.Any(x => x.Username == model.Username))
            throw new AppException("Username '" + model.Username + "' is already taken");

        var user = new User { Username = model.Username };

        // hash password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        user.Role = model.Role;

        user.IsBlocked = false;

        user.DateCreated = DateTime.Now;
        user.DateUpdated = DateTime.Now;

        // save user
        _context.Users.Add(user);
        _context.SaveChanges();

        return user;
    }

    public User Update(int id, UpdateRequest model)
    {
        var user = getUser(id);

        // validate
        //if (model?.Username != user.Username && _context.Users.Any(x => x.Username == model.Username))
        //    throw new AppException("Username '" + model.Username + "' is already taken");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        if (model.IsBlocked.HasValue && model.IsBlocked.Value)
            user.IsBlocked = !user.IsBlocked;

        if (model.Role.HasValue)
            user.Role = model.Role.Value;

        user.DateUpdated = DateTime.Now;

        _context.Users.Update(user);
        _context.SaveChanges();

        return user;
    }

    public void Delete(int id)
    {
        var user = getUser(id);
        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    // helper methods

    private User getUser(int id)
    {
        var user = _context.Users.Find(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }
}
