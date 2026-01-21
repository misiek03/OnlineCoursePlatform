using System.ComponentModel.DataAnnotations;

namespace OnlineCoursePlatform.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa kategorii jest wymagana")]
    [Display(Name = "Nazwa Kategorii")]
    public string Name { get; set; }
    
    public virtual ICollection<Course>? Courses { get; set; }
}