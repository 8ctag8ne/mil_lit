using System;
using System.Collections.Generic;

namespace MIL_LIT;

public partial class Book
{
    public int BookId { get; set; }

    public int CreatedBy { get; set; }

    public int Likes { get; set; }

    public int Saves { get; set; }

    public byte[] CreatedAt { get; set; } = null!;

    public string Sourcelink { get; set; } = null!;

    public byte[]? CoverImage { get; set; }

    public string? Filepath { get; set; }

    public string? GeneralInfo { get; set; }

    public int TagId { get; set; }

    public string? Author { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
