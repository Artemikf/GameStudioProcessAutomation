using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        [MaxLength(64)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        [MaxLength(64)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Position is required")]
        [MaxLength(64)]
        public string Position { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(96)]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [MaxLength(20)] // Увеличил для международных номеров
        public string Phone { get; set; }

        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; } = DateTime.Now;

        [Display(Name = "Salary")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be positive")]
        public decimal Salary { get; set; }

        // СВЯЗИ 
        public int? TeamId { get; set; }
        public virtual Team Team { get; set; }

        public virtual ICollection<Task> AssignedTasks { get; set; }
        public virtual ICollection<BugReport> CreatedBugReports { get; set; }
    }
}