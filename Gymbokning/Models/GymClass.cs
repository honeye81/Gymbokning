using System.ComponentModel.DataAnnotations;

namespace Gymbokning.Models
{
    public class GymClass
    {

        public GymClass()
        {
            AttendingMembers = new List<ApplicationUserGymClass>();
        }
        
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        public DateTime EndTime { get { return StartTime + Duration; } }

        [StringLength(200)]
        public string Description { get; set; }

        // Navigation property for the many-to-many relationship
        public ICollection<ApplicationUserGymClass> AttendingMembers { get; set; }


    }

}

