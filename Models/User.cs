using System;
using System.Collections.Generic;

namespace MIL_LIT;

public partial class User
{
    public int UserId { get; set; }

    public string PasswordHash { get; set; } = null!;

    public string Login { get; set; } = null!;

    public byte[] ProfilePicture { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public byte[] CreatedAt { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
