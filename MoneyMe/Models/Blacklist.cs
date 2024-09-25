using System;
using System.Collections.Generic;

namespace MoneyMe.Models;

public partial class Blacklist
{
    public string Value { get; set; } = null!;

    public bool? IsMobile { get; set; }

    public bool? IsDomain { get; set; }

    public DateTime? DateCreated { get; set; }

    public long BlacklistId { get; set; }
}
