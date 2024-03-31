using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations.Schema;

namespace MIL_LIT;

public partial class User : IValidatableObject
{
    public int UserId { get; set; }

    [Display(Name = "Ім'я користувача")]
    [Required(ErrorMessage = "Ім'я користувача не може бути порожнім.")]
    public string Login { get; set; } = null!;

    [Display(Name = "Хеш паролю")]
    [Required(ErrorMessage = "Пароль не може бути порожнім.")]
    public string PasswordHash { get; set; } = null!;

    [Display(Name = "Є адміністратором")]
    public bool IsAdmin { get; set; }

    [Display(Name = "Дата створення")]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Зображення профілю")]
    public string? ProfilePicture { get; set; }
    
    [NotMapped]
    public IFormFile? CoverFile {get; set;}
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(Login.Length > 50)
        {
            yield return new ValidationResult("Довжина імені не має перевищувати 50 символів.", new []{nameof(Login)});
        }

        if(Login.IsNullOrEmpty())
        {
            yield return new ValidationResult("Ім'я користувача не може бути порожнім.", new []{nameof(Login)});
        }

        if(PasswordHash.Length > 50)
        {
            yield return new ValidationResult("Довжина паролю не має перевищувати 50 символів.", new []{nameof(PasswordHash)});
        }

        if(PasswordHash.Length < 8)
        {
            yield return new ValidationResult("Пароль має бути не коротшим від 8 символів.", new []{nameof(PasswordHash)});
        }

        if(PasswordHash.IsNullOrEmpty())
        {
            yield return new ValidationResult("Пароль не може бути порожнім.", new []{nameof(PasswordHash)});
        }

        string SpecialCharacters=@"!#'$%&()*+,-./:;<=>?@[\]^_`{|}~";
        foreach(var ch in PasswordHash)
        {
            if(!(ch<='z' && ch>='a') && !(ch<='Z' && ch>='A') && !char.IsDigit(ch) && !SpecialCharacters.Contains(ch))
            {
                yield return new ValidationResult("Пароль може складатися тільки з символів англійського алфавіту, цифр та спеціальних символів на кшталт !#$%&()*+-", new []{nameof(PasswordHash)});
            }
        }

        String LoginCharacters="+-!@#$%^&*()_, .";

        foreach(var ch in Login)
        {
            if(!char.IsLetter(ch) && !char.IsDigit(ch) && !LoginCharacters.Contains(ch))
            {
                yield return new ValidationResult("Ім'я користувача може містити тільки літери цифри та знаки +-!@#$%^&*()_, .", new []{nameof(Login)});
            }
        }
    }
}
