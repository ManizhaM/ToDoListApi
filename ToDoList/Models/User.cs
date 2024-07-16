using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class User
    {
        public int Id { get; set; }


        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Name must contain only letters")]
        public string Name { get; set; }


        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits")]
        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<ToDoItem> ? ToDoItems { get; set; }
    }

}
