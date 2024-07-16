using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        #region Add new user to db
        // method POST - api/Users/AddNewUser
        [HttpPost("AddNewUser")]
        public async Task<JsonResult> CreateNewUser(User user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                    );
                Response.StatusCode = 400;
                return new JsonResult(new
                {
                    Message = "One or more validation errors occurred.",
                    Status = false,
                    errors
                });
            }

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber);
            if (existingUser != null)
            {
                Response.StatusCode = 400;
                return new JsonResult(
                       new
                       {
                           Message = "User with this phone number already exists",
                           Status = false
                       });
            }

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                Response.StatusCode = 200;
                return new JsonResult(new { Message = "User created successfully", UserId = user.Id });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return new JsonResult( new
                       {
                           Message = $"Failed to add new user: {ex.Message}",
                           Status = false
                       });
            }
        }
        #endregion

        #region Change user status to "not active"
        // PUT: api/Users/ChangeUserStatus/user=5/status=true
        [HttpPut("ChangeUserStatus/user={userId}/status={status}")]
        public async Task<JsonResult> ChangeUserStatus(int userId, bool status)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                Response.StatusCode = 404;
                return new JsonResult(
                       new
                       {
                           Message = "User not found",
                           Status = false
                       });
            }

            try
            {
                user.IsActive = status;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                Response.StatusCode = 200;
                return new JsonResult(
                       new
                       {
                           Message = "User successfully updated",
                           Status = true
                       });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return new JsonResult(new
                {
                    Message = $"Failed to update user: {ex.Message}",
                    Status = false
                });
            }
        }
        #endregion

    }
}
