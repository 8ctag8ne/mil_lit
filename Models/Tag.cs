using System;
using System.Collections.Generic;

namespace MIL_LIT;

public partial class Tag
{
    public int TagId { get; set; }

    public string Name { get; set; } = null!;

    public int CreatedBy { get; set; }

    public byte[]? CoverImage { get; set; }

    public byte[] CreatedAt { get; set; } = null!;

    public int ParentTagId { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Tag> InverseParentTag { get; set; } = new List<Tag>();

    public virtual Tag ParentTag { get; set; } = null!;
}
