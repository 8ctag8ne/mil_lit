using System;
using System.Collections.Generic;

namespace MIL_LIT;

public partial class Comment
{
    public int CommentId { get; set; }

    public string Text { get; set; } = null!;

    public int UserId { get; set; }

    public int BookId { get; set; }

    public byte[] PostedAt { get; set; } = null!;

    public int ParentCommentId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Comment ParentComment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
