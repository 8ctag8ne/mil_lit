using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIL_LIT;

public partial class Tag : IValidatableObject
{
    [Display(Name = "ID категорії")]
    public int TagId { get; set; }

    [Display(Name = "Назва категорії")]
    [Required(ErrorMessage = "Назва не може бути порожньою.")]
    public string Name { get; set; } = null!;

    
    public int? CreatedBy { get; set; }

    [Display(Name = "Обкладинка")]
    public string? CoverImage { get; set; }

    [Display(Name = "Дата створення")]
    public DateTime? CreatedAt { get; set; }

    public int? ParentTagId { get; set; }

    [Display(Name = "Створено користувачем")]
    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Tag> InverseParentTag { get; set; } = new List<Tag>();

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    [Display(Name = "Є підкатегорією")]
    public virtual Tag? ParentTag { get; set; }

     [NotMapped]
    public IFormFile? CoverFile { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(Name.Length > 50)
        {
            yield return new ValidationResult("Довжина назви не має перевищувати 50 символів.", new []{nameof(Name)});
        }

        if(string.IsNullOrEmpty(Name))
        {
            yield return new ValidationResult("Ім'я користувача не може бути порожнім.", new []{nameof(Name)});
        }

        String NameCharacters="+-!@#$%^&*?()_, .";

        foreach(var ch in Name)
        {
            if(!char.IsLetter(ch) && !char.IsDigit(ch) && !NameCharacters.Contains(ch))
            {
                yield return new ValidationResult("Назва категорії може містити тільки літери цифри та розділові знаки", new []{nameof(Name)});
            }
        }
    }
}
