using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ToDoList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ToDoItemsController(AppDbContext context)
        {
            _context = context;
        }

        #region Get ToDoItems by filter
        // method Get - api/ToDoItems/GetToDoItemsByFilter
        [HttpGet("GetToDoItemsByFilter")]
        public async Task<JsonResult> GetToDoItems([FromQuery] FilterItems filter)
        {
            var query = _context.ToDoItems.AsQueryable();

            if (filter.isCompleted != null)
            {
                query = query.Where(t => t.IsCompleted == filter.isCompleted);
            }

            if (filter.priorityId != 0)
            {
                query = query.Where(t => t.PriorityId == filter.priorityId);
            }

            if(filter.userId != 0)
            {
                query = query.Where(t => t.UserId == filter.userId);
            }

            if (filter.fromDate != null)
            {
                query = query.Where(t => t.DueDate >= filter.fromDate);
            }

            if (filter.toDate != null)
            {
                query = query.Where(t => t.DueDate <= filter.toDate);
            }
            var toDoItemsList = await query.Include(t => t.Priority).Include(t => t.User).ToListAsync();

            var countToDoItems = toDoItemsList.Count();

            var totalPages = countToDoItems / filter.pageSize;

            var productsList = toDoItemsList.Skip((filter.pageNum - 1) * filter.pageSize)
                            .Take(filter.pageSize)
                            .ToList();

            Response.StatusCode = 200;

            ItemsPerPage<ToDoItem> products = new()
            {
                TotalPages = totalPages,
                CurrentPage = filter.pageNum,
                NextPage = filter.pageNum >= totalPages ? totalPages : filter.pageNum + 1,
                PreviosPage = filter.pageNum > 1 ? filter.pageNum - 1 : 1,
                TotalItems = countToDoItems,
                Items = productsList
            };

            return new JsonResult(products);
        }

        #endregion

        #region Get ToDoItem by Id
        // method GET - api/ToDoItems/5
        [HttpGet("GetToDoItemById/itemId={itemId}")]
        public async Task<JsonResult> GetToDoItem(int itemId)
        {
            try
            {
                var toDoItem = await _context.ToDoItems.FirstOrDefaultAsync(t => t.Id == itemId);

                if (toDoItem == null)
                {
                    Response.StatusCode = 404;
                    return new JsonResult(new
                    {
                        Message = "ToDoItem is not found",
                        Status = false,
                    });
                }
                else
                {
                    Response.StatusCode = 200;
                    return new JsonResult(toDoItem);
                }
            }
            catch (Exception ex) {
                Response.StatusCode = 500;
                return new JsonResult(new
                {
                    Message = $"Failed to get ToDoItemById: {ex.Message}",
                    Status = false
                });
            }
}
        #endregion

        #region Create a new ToDoItem
        // method POST - api/ToDoItems/AddNewToDoItem
        [HttpPost("AddNewToDoItem")]
        public async Task<JsonResult> AddToDoItem(ToDoItem toDoItem)
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

            try
            {
                var priority = await _context.Priorities.FindAsync(toDoItem.PriorityId);
                if (priority == null)
                {
                    Response.StatusCode = 404;
                    return new JsonResult(
                        new
                        {
                            Message = "Priority not found",
                            Status = false
                        });
                }
                toDoItem.Priority = priority;

                var user = await _context.Users.FindAsync(toDoItem.UserId);
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
                toDoItem.Priority = priority;

                // DueDate to UTC format
                toDoItem.DueDate = DateTime.SpecifyKind(toDoItem.DueDate, DateTimeKind.Utc);

                _context.ToDoItems.Add(toDoItem);
                await _context.SaveChangesAsync();

                Response.StatusCode = 200;
                return new JsonResult(
                    new {
                        Message = "ToDoItem created successfully",
                        Status = true,
                        toDoItemId = toDoItem.Id 
                    });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return new JsonResult(new
                {
                    Message = $"Failed to add new to do item: {ex.Message}",
                    Status = false
                });
            }
        }
        #endregion

        #region Edit ToDoItem set another User, priority and other parameters
        //method PUT - api/ToDoItems/UpdateToDoItem
        [HttpPut("UpdateToDoItem")]
        public async Task<JsonResult> PutToDoItem(ToDoItem toDoItem)
        {
            try
            {
                var getItem = await _context.ToDoItems.FirstOrDefaultAsync(t => t.Id == toDoItem.Id);
                if (getItem == null)
                {
                    Response.StatusCode = 404;
                    return new JsonResult(
                        new
                        {
                            Message = "ToDoItem not found",
                            Status = false
                        });
                }
                else
                {
                    if (toDoItem.UserId != 0 && getItem.UserId != toDoItem.UserId)
                    {
                        var getUser = await _context.Users
                                            .FirstOrDefaultAsync(u => u.Id == toDoItem.UserId);
                        if (getUser == null)
                        {
                            Response.StatusCode = 404;
                            return new JsonResult(
                                new
                                {
                                    Message = "User not found",
                                    Status = false
                                });
                        }
                        else
                        {
                            getItem.UserId = getUser.Id;
                            getItem.User = getUser;
                        }
                    }
                    if (toDoItem.PriorityId != 0 && getItem.PriorityId != toDoItem.PriorityId)
                    {
                        var getPriority = await _context.Priorities.FirstOrDefaultAsync(p => p.Level ==  toDoItem.PriorityId);
                        if(getPriority == null)
                        {
                            Response.StatusCode = 404;
                            return new JsonResult(
                                new
                                {
                                    Message = "Priority not found",
                                    Status = false
                                });
                        }
                        else
                        {
                            getItem.PriorityId = getPriority.Level;
                            getItem.Priority = getPriority;
                        }
                    }
                    getItem.DueDate = toDoItem.DueDate != DateTime.MinValue ? toDoItem.DueDate : getItem.DueDate;
                    getItem.Title = toDoItem.Title ?? getItem.Title;
                    getItem.Description = toDoItem.Description ?? getItem.Description;
                    getItem.IsCompleted = toDoItem.IsCompleted != null ? toDoItem.IsCompleted : getItem.IsCompleted;

                    try
                    {
                        _context.Entry(getItem).State = EntityState.Modified;
                        await _context.SaveChangesAsync();

                        Response.StatusCode = 200;
                        return new JsonResult(
                            new
                            {
                                Message = "ToDoItem updates successfully",
                                Status = true,
                                toDoItemId = toDoItem.Id
                            });
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_context.ToDoItems.Any(e => e.Id == toDoItem.Id))
                        {
                            Response.StatusCode = 404;
                            return new JsonResult(
                                new
                                {
                                    Message = "ToDoItem not found",
                                    Status = true
                                });
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return new JsonResult(new
                {
                    Message = $"Failed to add new user: {ex.Message}",
                    Status = false
                });
            }
        }

        #endregion

        #region Delete a ToDoItem
        // method DELETE - api/ToDoItems/DeleteToDoItem/itemId=itemId
        [HttpDelete("DeleteToDoItem/itemId={itemId}")]
        public async Task<JsonResult> DeleteToDoItem(int itemId)
        {
            try
            {
                var toDoItem = await _context.ToDoItems.FindAsync(itemId);
                if (toDoItem == null)
                {
                    Response.StatusCode = 404;
                    return new JsonResult(
                        new
                        {
                            Message = "ToDoItem not found",
                            Status = false
                        });
                }

                _context.ToDoItems.Remove(toDoItem);
                await _context.SaveChangesAsync();

                Response.StatusCode = 200;
                return new JsonResult(
                    new
                    {
                        Message = "ToDoItem successfully deleted",
                        Status = true
                    });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return new JsonResult(new
                {
                    Message = $"Failed to delete ToDoItem: {ex.Message}",
                    Status = false
                });
            }
        }
        #endregion
    }

}
