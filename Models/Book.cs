using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace MIL_LIT;

public partial class Book
{
    [Display(Name = "Назва")]
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
}
