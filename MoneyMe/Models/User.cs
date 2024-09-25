using System;
using System.Collections.Generic;

namespace MoneyMe.Models;

public partial class User
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public string? Title { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public long UserId { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}
