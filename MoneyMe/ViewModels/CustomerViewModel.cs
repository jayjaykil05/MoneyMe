using MoneyMe.Models;

namespace MoneyMe.ViewModels
{
    public class CustomerViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public int Product { get; set; }
        public int UserId { get; set; }
        public decimal Term { get; set; }
        public decimal AmountRequired { get; set; }
    }
    public class CustomerResponseViewModel
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public CustomerAccountViewModel Customer { get; set; }
    }
    public class UpdateCustomerResponseViewModel
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public User Customer { get; set; }
    }

    public class LoanCalculationResponseViewModel
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public Loan Loans { get; set; }
    }

    public class CustomerAccountViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public decimal Term { get; set; }
        public decimal AmountRequired { get; set; }
        public decimal MonthlyAmount { get; set; }
        public decimal TotalRepayments { get; set; }
        public decimal EstablishmentFee { get; set; }
        public decimal Interest { get; set; }
        public DateTime DateCreated { get; set; }
        public int ProductId { get; set; }
        public long UserId { get; set; }
        public long LoanId { get; set; }
        public List<Product> Product { get; set; }

    }
}

    