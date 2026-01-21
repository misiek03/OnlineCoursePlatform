using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineCoursePlatform.Models;

public class Course
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tytuł jest wymagany")]
    [StringLength(100, MinimumLength = 3)]
    [Display(Name = "Tytuł Kursu")]
    public string Title { get; set; }

    [Display(Name = "Opis")]
    public string? Description { get; set; }

    [Range(0, 10000)]
    [DataType(DataType.Currency)]
    [Display(Name = "Cena")]
    public decimal Price { get; set; }
    
    [Display(Name = "Kategoria")]
    public int CategoryId { get; set; }
    
    [Display(Name = "Kategoria")]
    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }
    
    public virtual ICollection<Enrollment>? Enrollments { get; set; }
    
    [Display(Name = "Link do YouTube")]
    [Url(ErrorMessage = "Wprowadź poprawny adres URL")]
    public string? YouTubeUrl { get; set; }
    
    public string? GetEmbedUrl()
    {
        if (string.IsNullOrEmpty(YouTubeUrl)) return null;

        try
        {
            var uri = new Uri(YouTubeUrl);
            
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            if (query.AllKeys.Contains("v"))
            {
                return $"https://www.youtube.com/embed/{query["v"]}";
            }
            
            if (uri.Host.Contains("youtu.be"))
            {
                return $"https://www.youtube.com/embed{uri.AbsolutePath}";
            }
        }
        catch 
        {
            return null;
        }

        return null;
    }
}