using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace OnlineCoursePlatform.Models;

public class Enrollment
{
    public int Id { get; set; }

    public int CourseId { get; set; }
    [ForeignKey("CourseId")]
    public virtual Course? Course { get; set; }
    
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual IdentityUser? User { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Data zapisu")]
    public DateTime EnrollmentDate { get; set; } = DateTime.Now;
}