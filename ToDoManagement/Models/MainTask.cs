namespace ToDoManagement.Models
{
    public class MainTask
    {
        public Guid Id { get; set; }
        public string TaskLabel { get; set; }
        public string? TaskDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? TotalTask { get; set; }
        public bool IsDone { get; set; } = false;
        public bool IsPinned { get; set; } = false;
    }
}
