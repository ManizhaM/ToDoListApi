using System.Linq;
using ToDoList.Models;

namespace ToDoList.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Priorities.Any())
            {
                return; 
            }

            var priorities = new Priority[]
            {
                new Priority { Level = 1, Name = "Low" },
                new Priority { Level = 2, Name = "Medium" },
                new Priority { Level = 3, Name = "High" }
            };

            context.Priorities.AddRange(priorities);
            context.SaveChanges();
        }
    }
}
