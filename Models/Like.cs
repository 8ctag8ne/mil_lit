using System;
using System.Collections.Generic;

namespace MIL_LIT;

public partial class Like
{
    public int UserId { get; set; }

    public int BookId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
