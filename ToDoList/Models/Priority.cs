using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class Priority
    {
        public int Level { get; set; }

        [Required]
        public string Name { get; set; }
        public ICollection<ToDoItem> ToDoItems { get; set; }
    }

}
