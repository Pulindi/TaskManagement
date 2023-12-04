using Microsoft.EntityFrameworkCore;

namespace ToDoManagement.Models
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MainTask>()
                .HasKey(mt => mt.Id);

        }

        public DbSet<MainTask> MainTasks { get; set; }
    }
}
