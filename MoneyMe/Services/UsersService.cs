using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MoneyMe.Models;
using MoneyMe.ViewModels;

namespace MoneyMe.Services
{
    public class UsersService
    {
        private readonly MoneymeDbContext _context;


        public UsersService(MoneymeDbContext context)
        {
            _context = context;
        }

        public async Task<CustomerResponseViewModel> InsertNewCustomer(CustomerViewModel data)
        {
            var customerResponse = new CustomerResponseViewModel();

            try
            {

                var duplicateCustInfo = await _context.Users
                                .Include(x => x.Loans)
                                .Where(x => x.FirstName.ToLower() == data.FirstName.ToLower()
                                        && x.LastName.ToLower() == data.LastName.ToLower()
                                        && ((DateTime)x.DateOfBirth).Equals(data.DateOfBirth) == true)
                                .FirstOrDefaultAsync();

                if (duplicateCustInfo == null)
                {
                    var newCust = new User()
                    {
                        FirstName = data.FirstName,
                        LastName = data.LastName,
                        Email = data.Email,
                        DateOfBirth = data.DateOfBirth,
                        Title = data.Title,
                        Mobile = data.Mobile,
                        DateCreated = DateTime.Now,

                    };

                    var loans = new Loan()
                    {
                        FinanceAmount = data.AmountRequired,
                        Term = data.Term,
                        MonthlyAmounn = 0,
                        DateCreated = DateTime.Now,
                        Status = "created"

                    };

                    newCust.Loans.Add(loans);

                    _context.Add(newCust);
                    await _context.SaveChangesAsync();

                    var respNewCust = new CustomerAccountViewModel()
                    {
                        AmountRequired = (decimal)newCust.Loans.First().FinanceAmount,
                        Term = (decimal)newCust.Loans.First().Term,
                        Title = newCust.Title,
                        FirstName = newCust.FirstName,
                        LastName = newCust.LastName,
                        DateOfBirth = (DateTime)newCust.DateOfBirth,
                        Mobile = newCust.Mobile,
                        Email = newCust.Email,
                        DateCreated = DateTime.Now,
                        UserId = newCust.UserId
                    };

                    customerResponse.Message = "Successfully Created Loan Account";
                    customerResponse.IsSuccess = true;
                    customerResponse.Customer = respNewCust;


                    return customerResponse;
                }
                else
                {
                    var existingCust = new CustomerAccountViewModel()
                    {
                        AmountRequired = (decimal)duplicateCustInfo.Loans.First().FinanceAmount,
                        Term = (decimal)duplicateCustInfo.Loans.First().Term,
                        Title = duplicateCustInfo.Title,
                        FirstName = duplicateCustInfo.FirstName,
                        LastName = duplicateCustInfo.LastName,
                        DateOfBirth = (DateTime)duplicateCustInfo.DateOfBirth,
                        Mobile = duplicateCustInfo.Mobile,
                        Email = duplicateCustInfo.Email,
                        DateCreated = DateTime.Now,
                        UserId = duplicateCustInfo.UserId
                    };

                    duplicateCustInfo.Title = data.Title;
                    duplicateCustInfo.Email = data.Email;
                    duplicateCustInfo.Mobile = data.Mobile;
                    duplicateCustInfo.DateUpdated = DateTime.Now;

                    _context.Update(duplicateCustInfo);
                    await _context.SaveChangesAsync();


                    customerResponse.Message = "Retrieved Existing";
                    customerResponse.IsSuccess = true;
                    customerResponse.Customer = existingCust;


                    return customerResponse;
                }


            }
            catch (Exception ex)
            {
                customerResponse.Message = "Transaction Failed";
                customerResponse.IsSuccess = false;
                customerResponse.Customer = null;

                return customerResponse;
            }


        }
        public async Task<LoanCalculationResponseViewModel> CalculateAndUpdateAccount(CustomerViewModel data)
        {
            var loanCalculationResponse = new LoanCalculationResponseViewModel();
            data.Term = data.Product == 2 ? data.Term - 2 : data.Term;

            try
            {

                var existingCust = new CustomerAccountViewModel();
                var monthlyPayment = PmtPayment(data.Term, data.AmountRequired, data.Product);
                var totalRepayment = TotalRepayment(data.Term, monthlyPayment);


                var customerLoan = await _context.Loans
                                .Where(x => x.User.UserId == data.UserId && x.Status == "created").FirstOrDefaultAsync();
                var customerAccount = await _context.Users
                                .Where(x => x.UserId == data.UserId).FirstOrDefaultAsync();


                customerAccount.FirstName = data.FirstName;
                customerAccount.LastName = data.LastName;
                customerAccount.Email = data.Email;
                customerAccount.Mobile = data.Mobile;
                customerAccount.DateUpdated = DateTime.Now;
                customerAccount.Title = data.Title;
                customerAccount.DateOfBirth = data.DateOfBirth;

                _context.Update(customerAccount);

                customerLoan.FinanceAmount = data.AmountRequired ;
                customerLoan.Term = data.Term;
                customerLoan.MonthlyAmounn = (decimal)Math.Round(monthlyPayment, 2); ;/////Calculate Later.
                customerLoan.TotalRepayments = (decimal)Math.Round(totalRepayment, 2);
                customerLoan.EstablishmentFee = Constant.establishmentFee;
                customerLoan.InterestFee = (decimal)Math.Round(ComputeInterest(totalRepayment, (double)data.AmountRequired), 2);
                customerLoan.DateUpdated = DateTime.Now;
                customerLoan.ProductId = data.Product;
                customerLoan.UserId = data.UserId;
                customerLoan.Status = "calculated";


                _context.Update(customerLoan);
                await _context.SaveChangesAsync();


                loanCalculationResponse.Message = "Proceeding to the Quotation Page";
                loanCalculationResponse.IsSuccess = true;
                loanCalculationResponse.Loans = customerLoan;

                return loanCalculationResponse;

            }
            catch (Exception ex)
            {
                loanCalculationResponse.Message = "Transaction Failed";
                loanCalculationResponse.IsSuccess = false;
                loanCalculationResponse.Loans = null;


                return loanCalculationResponse;
            }
        }

        public async Task<LoanCalculationResponseViewModel> ApplyNow(CustomerAccountViewModel data)
        {
            var loanCalculationResponse = new LoanCalculationResponseViewModel();
            data.Term = data.ProductId == 2 ? data.Term - 2 : data.Term;

            try
            {

                var existingCust = new CustomerAccountViewModel();
                var monthlyPayment = PmtPayment(data.Term, data.AmountRequired, data.ProductId);
                var totalRepayment = TotalRepayment(data.Term, monthlyPayment);


                var customerLoan = await _context.Loans
                                .Where(x => x.User.UserId == data.UserId && x.Status == "calculated").FirstOrDefaultAsync();
                var customerAccount = await _context.Users
                                .Where(x => x.UserId == data.UserId).FirstOrDefaultAsync();


                customerAccount.FirstName = data.FirstName;
                customerAccount.LastName = data.LastName;
                customerAccount.Email = data.Email;
                customerAccount.Mobile = data.Mobile;
                customerAccount.DateUpdated = DateTime.Now;
                customerAccount.Title = data.Title;
                customerAccount.DateOfBirth = data.DateOfBirth;

                _context.Update(customerAccount);

                customerLoan.FinanceAmount = data.AmountRequired;
                customerLoan.Term = data.Term;
                customerLoan.MonthlyAmounn = (decimal)Math.Round(monthlyPayment, 2); ;/////Calculate Later.
                customerLoan.TotalRepayments = (decimal)Math.Round(totalRepayment, 2);
                customerLoan.EstablishmentFee = Constant.establishmentFee;
                customerLoan.InterestFee = (decimal)Math.Round(ComputeInterest(totalRepayment, (double)data.AmountRequired), 2);
                customerLoan.DateUpdated = DateTime.Now;
                customerLoan.ProductId = data.ProductId;
                customerLoan.UserId = data.UserId;
                customerLoan.Status = "applied";


                _context.Update(customerLoan);
                await _context.SaveChangesAsync();


                loanCalculationResponse.Message = "";
                loanCalculationResponse.IsSuccess = true;
                loanCalculationResponse.Loans = customerLoan;

                return loanCalculationResponse;

            }
            catch (Exception ex)
            {
                loanCalculationResponse.Message = "Transaction Failed";
                loanCalculationResponse.IsSuccess = false;
                loanCalculationResponse.Loans = null;


                return loanCalculationResponse;
            }
        }

        public async Task<UpdateCustomerResponseViewModel> UpdateCustomer(CustomerViewModel data)
        {
            var customerResponse = new UpdateCustomerResponseViewModel();

            try
            {

                var duplicateCustInfo = await _context.Users
                                .Where(x => x.UserId == data.UserId)
                                .FirstOrDefaultAsync();


                duplicateCustInfo.FirstName = data.FirstName;
                duplicateCustInfo.LastName = data.LastName;
                duplicateCustInfo.Title = data.Title;
                duplicateCustInfo.Email = data.Email;
                duplicateCustInfo.Mobile = data.Mobile;
                duplicateCustInfo.DateOfBirth = data.DateOfBirth;
                duplicateCustInfo.DateUpdated = DateTime.Now;

                _context.Update(duplicateCustInfo);
                await _context.SaveChangesAsync();


                customerResponse.Message = "Retrieved Existing";
                customerResponse.IsSuccess = true;
                customerResponse.Customer = duplicateCustInfo;


                return customerResponse;
            }
            catch (Exception ex)
            {
                customerResponse.Message = "Transaction Failed";
                customerResponse.IsSuccess = false;
                customerResponse.Customer = null;

                return customerResponse;
            }
        }
        public async Task<LoanCalculationResponseViewModel> UpdateLoan(Loan data)
        {
            var loanResponse = new LoanCalculationResponseViewModel();
            data.Term = data.ProductId == 2 ? data.Term - 2 : data.Term;

            try
            {
                var monthlyPayment = PmtPayment((decimal)data.Term, (decimal)(data.FinanceAmount), (int)data.ProductId);
                var totalRepayment = TotalRepayment((decimal)data.Term, monthlyPayment);


                var existingLoan = await _context.Loans
                                .Where(x => x.LoanId == data.LoanId)
                                .FirstOrDefaultAsync();

                existingLoan.FinanceAmount = data.FinanceAmount ;
                existingLoan.Term = data.Term;
                existingLoan.MonthlyAmounn = Math.Round((decimal)monthlyPayment, 2);
                existingLoan.TotalRepayments = Math.Round((decimal)totalRepayment, 2);
                existingLoan.EstablishmentFee = Math.Round(Constant.establishmentFee, 2);
                existingLoan.InterestFee = (decimal)ComputeInterest(totalRepayment, (double)data.FinanceAmount);
                existingLoan.DateUpdated = DateTime.Now;
                existingLoan.ProductId = data.ProductId;
                existingLoan.UserId = data.UserId;
                existingLoan.Status = "calculated";

                _context.Update(existingLoan);
                await _context.SaveChangesAsync();


                loanResponse.Message = "Successfully Updated";
                loanResponse.IsSuccess = true;
                loanResponse.Loans = existingLoan;


                return loanResponse;
            }
            catch (Exception ex)
            {
                loanResponse.Message = "Transaction Failed";
                loanResponse.IsSuccess = false;
                loanResponse.Loans = null;

                return loanResponse;
            }
        }

        public double PmtPayment(decimal terms, decimal loanAmount, int productId)
        {
            double PMT = 0;
            var principalAmt = (double)loanAmount;
            var monthlyInterest = (Constant.interestRate) / (12 * 100);/// Annual Interest
            var numOfPayments = terms;


            if (productId == 1)
            {
                PMT = principalAmt / (double)numOfPayments;
            }
            else
            {
                /// PMT = P*(r(1+r)^n)/
                ///       ((1+r)^n)-1


                ////value of (1+r)^n
                var onePlusRN = (Math.Pow(1 + monthlyInterest, Convert.ToDouble(numOfPayments)));
                ///PMT
                PMT = (principalAmt * (monthlyInterest * onePlusRN)) / (onePlusRN - 1);

            }

            return PMT;
        }
        public double TotalRepayment(decimal terms, double pmt)
        {

            return Convert.ToDouble(pmt * (double)terms);
        }
        public double ComputeInterest(double totalRepayment, double principalAmount)
        {
            return Convert.ToDouble(totalRepayment - principalAmount);
        }


    }
}