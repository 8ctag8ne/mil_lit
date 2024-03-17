using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MIL_LIT;

public partial class User
{
    public int UserId { get; set; }

    [Display(Name = "Ім'я користувача")]
    public string Login { get; set; } = null!;

    [Required(ErrorMessage = "Поле не має бути порожнім")]
    [Display(Name = "Хеш паролю")]
    public string PasswordHash { get; set; } = null!;

    [Display(Name = "Чи є адміністратором")]
    public bool IsAdmin { get; set; }

    [Display(Name = "Дата створення")]
    public DateTime? CreatedAt { get; set; }

    [Display(Name = "Посилання на зображення профілю")]
    public string? ProfilePicture { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
