using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title length can't be more than 100")]
        public string Title { get; set; }

        public string? Description { get; set; }
        public bool ? IsCompleted { get; set; } = false;


        [Required(ErrorMessage = "DueDate is required")]
        public DateTime DueDate { get; set; }


        [Required(ErrorMessage = "PriorityId is required")]
        public int PriorityId { get; set; }

        public Priority? Priority { get; set; }


        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }

        public User? User { get; set; }
    }

    public class FilterItems
    {
        public int userId { get; set; }
        public bool? isCompleted { get; set; }
        public int priorityId { get; set; }
        public DateTime? fromDate {  get; set; }
        public DateTime? toDate { get; set; }
        public int pageNum {  get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }

    public class ItemsPerPage<T>
    {
        public int PreviosPage { get; set; }
        public int CurrentPage { get; set; }
        public int NextPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public List<T> Items { get; set; }
    }

}
