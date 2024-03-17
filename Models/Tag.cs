using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MIL_LIT;

public partial class Tag
{
    [Display(Name = "ID категорії")]
    public int TagId { get; set; }

    [Display(Name = "Назва категорії")]
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
}
