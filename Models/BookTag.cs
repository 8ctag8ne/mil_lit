using System;
using System.Collections.Generic;

namespace MIL_LIT;

public partial class BookTag
{
    public int BookId { get; set; }

    public int TagId { get; set; }

    public virtual Book Book { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
