using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace MIL_LIT;

public partial class Book : IValidatableObject
{
    [Display(Name = "Назва")]
    [Required(ErrorMessage = "Назва не може бути порожньою.")]
    public string Name {get; set;} = null!;

    public int BookId { get; set; }

    [Display(Name = "Створено користувачем")]
    public int? CreatedBy { get; set; }

    [Display(Name = "Уподобали")]
    public int Likes { get; set; }

    [Display(Name = "Зберегли")]
    public int Saves { get; set; }

    [Display(Name = "Дата створення")]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Джерело")]
    [Required(ErrorMessage = "Посилання на джерело не може бути порожнім.")]
    public string SourceLink { get; set; } = null!;

    public string? Filepath { get; set; }

    [Display(Name = "Опис")]
    public string? GeneralInfo { get; set; }

    [Display(Name = "Автор")]
    public string? Author { get; set; }

    [Display(Name = "Обкладинка")]
    public string? CoverLink { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual User? CreatedByNavigation { get; set; }

    [NotMapped]
    public List<int> TagIds { get; set; } = new List<int>();

    [NotMapped]
    public IFormFile? CoverFile { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(Author?.Length > 50)
        {
            yield return new ValidationResult("Ім'я автора не має містити більш, ніж 50 символів.", new []{nameof(Author)});
        }

        String NameCharacters="+-!@#$%^&*?()_, .";

        foreach(var ch in Name)
        {
            if(!char.IsLetter(ch) && !char.IsDigit(ch) && !NameCharacters.Contains(ch))
            {
                yield return new ValidationResult("Назва книги може містити тільки літери цифри та розділові знаки", new []{nameof(Name)});
            }
        }
    }
}
