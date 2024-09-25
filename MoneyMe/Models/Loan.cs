using System;
using System.Collections.Generic;

namespace MoneyMe.Models;

public partial class Loan
{
    public long LoanId { get; set; }

    public decimal? FinanceAmount { get; set; }

    public decimal? Term { get; set; }

    public decimal? MonthlyAmounn { get; set; }

    public DateTime? DateCreated { get; set; }

    public DateTime? DateUpdated { get; set; }

    public long? ProductId { get; set; }

    public long? UserId { get; set; }

    public string Status { get; set; } = null!;

    public decimal? EstablishmentFee { get; set; }

    public decimal? InterestFee { get; set; }

    public decimal? TotalRepayments { get; set; }

    public virtual User? User { get; set; }
}
