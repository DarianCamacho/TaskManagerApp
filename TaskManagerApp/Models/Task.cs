namespace TaskManagerApp.Models
{
    public class Task
    {
        public string? Id { get; set; }
        public string? TaskId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? DueDate { get; set; }
        public string? IsCompleted { get; set; }
        public string? Priority { get; set; }
    }

    public class TaskHandler
    {

    }
}