namespace Gymbokning.Models
{
    internal class GymClassViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
        public bool IsBooked { get; set; }
    }
}