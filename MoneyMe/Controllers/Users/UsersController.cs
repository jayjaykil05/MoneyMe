using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoneyMe.Models;
using MoneyMe.Services;
using MoneyMe.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoneyMe.Controllers.CustomerLoans
{
    [Route("Customer")]
    public class UsersController : Controller
    {
        private readonly MoneymeDbContext _context;
        private readonly UsersService _usersService;


        public UsersController(MoneymeDbContext context, UsersService usersService)
        {
            _context = context;
            _usersService = usersService;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Create
        [HttpGet("CreateLoanAccount")]
        public IActionResult Create()
        {
            return View();
        }

        // GET: Users/Edit/5
        [HttpGet("ViewAccount")]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vm = await _context.Loans
                                .Include(x => x.User)
                                .Where(x => x.UserId == id).OrderByDescending(x => x.DateCreated)
                                .Select(z => new CustomerAccountViewModel
                                {
                                    UserId = (long)z.UserId,
                                    FirstName = z.User.FirstName,
                                    LastName = z.User.LastName,
                                    DateOfBirth = (DateTime)z.User.DateOfBirth,
                                    Mobile = z.User.Mobile,
                                    Title = z.User.Title,
                                    Email = z.User.Email,
                                    Term = (decimal)(z.ProductId ==2?z.Term+2 :z.Term),
                                    AmountRequired = (decimal)z.FinanceAmount,
                                }).FirstOrDefaultAsync();

            ViewData["ProductList"] = new SelectList(await _context.Products.ToListAsync(), "ProductId", "ProductName");

            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        [HttpGet("Quotation")]
        public async Task<IActionResult> Quotation(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vm = await _context.Loans
                                .Include(x => x.User)
                                .Where(x => x.UserId == id && x.Status == "calculated").OrderByDescending(x => x.DateCreated)
                                .Select(z => new CustomerAccountViewModel
                                {
                                    UserId = (long)z.UserId,
                                    LoanId = z.LoanId,
                                    FirstName = z.User.FirstName,
                                    LastName = z.User.LastName,
                                    DateOfBirth = (DateTime)z.User.DateOfBirth,
                                    Mobile = z.User.Mobile,
                                    Title = z.User.Title,
                                    Email = z.User.Email,
                                    Term = (decimal)(z.ProductId == 2 ? z.Term + 2 : z.Term),
                                    AmountRequired = (decimal)z.FinanceAmount,
                                    MonthlyAmount = (decimal)z.MonthlyAmounn,
                                    TotalRepayments = (decimal)z.TotalRepayments,
                                    EstablishmentFee = (decimal)z.EstablishmentFee,
                                    Interest = (decimal)z.InterestFee
                                }).FirstOrDefaultAsync();

            ViewData["ProductList"] = new SelectList(await _context.Products.ToListAsync(), "ProductId", "ProductName", vm.ProductId);

            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);

        }

        [HttpGet("Congrats")]
        public IActionResult SuccessPage()
        {
            return View();
        }

        private bool UserExists(long id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }


        [HttpGet("IsBlackListed")]
        public async Task<IActionResult> IsBlackListed(string value, string type)
        {
            var predicate = PredicateBuilder.True<Blacklist>();
            try
            {
                if (type == "Mobile")
                {
                    predicate.And(x => x.IsMobile == true && x.Value == value);
                }
                else if (type == "Email") {
                    predicate.And(x => x.IsDomain == true && x.Value == value);
                }

                var blist = await _context.Blacklists.Where(x => x.Value == value).Where(predicate).ToListAsync();

                if (blist.Count == 0)
                {
                    return Ok(new { isSuccess = true, message = "Success!", IsBlackListed = false });

                }
                else
                {
                    return Ok(new { isSuccess = true, message = type + " domain is blacklisted!", IsBlackListed = true });
                }


            }
            catch (Exception ex)
            {

                return BadRequest(new { isSuccess = false, message = "Failed" });
            }

        }
        [HttpPost("SubmitUser")]
        public async Task<IActionResult> SubmitUser([FromBody] CustomerViewModel obj)
        {
            try
            {
                try
                {

                    var customerResp = await _usersService.InsertNewCustomer(obj);

                    return Ok(new { isSuccess = customerResp.IsSuccess, message = customerResp.Message, id = customerResp.Customer.UserId });

                }
                catch (Exception ex)
                {

                    return BadRequest(new { isSuccess = false, message = "Failed to Save!" });
                }


            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "null data!" });
            }

        }

        [HttpPost("UpdatetUser")]
        public async Task<IActionResult> UpdateUser([FromBody] CustomerViewModel obj)
        {
            try
            {
                try
                {

                    var customerResp = await _usersService.UpdateCustomer(obj);

                    return Ok(new { isSuccess = customerResp.IsSuccess, message = customerResp.Message, id = customerResp.Customer.UserId });

                }
                catch (Exception ex)
                {

                    return BadRequest(new { isSuccess = false, message = "Failed to Save!" });
                }


            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "null data!" });
            }

        }

        [HttpPost("UpdateLoan")]
        public async Task<IActionResult> UpdateLoan([FromBody] Loan obj)
        {
            try
            {
                try
                {

                    var loanResp = await _usersService.UpdateLoan(obj);

                    return Ok(new { isSuccess = loanResp.IsSuccess, message = loanResp.Message, id = loanResp.Loans.UserId });

                }
                catch (Exception ex)
                {

                    return BadRequest(new { isSuccess = false, message = "Failed to Save!" });
                }


            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "null data!" });
            }

        }

        [HttpPost("CalculateQuote")]
        public async Task<IActionResult> CalculateQuote([FromBody] CustomerViewModel obj)
        {
            try
            {
                try
                {
                    var customerResp = await _usersService.CalculateAndUpdateAccount(obj);

                    return Ok(new { isSuccess = customerResp.IsSuccess, message = customerResp.Message, id = customerResp.Loans.UserId });

                }
                catch (Exception ex)
                {

                    return BadRequest(new { isSuccess = false, message = "Failed to Save!" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "null data!" });
            }
        }


        [HttpPost("ApplyNow")]
        public async Task<IActionResult> ApplyNow([FromBody] CustomerAccountViewModel obj)
        {
            try
            {
                try
                {
                    var customerResp = await _usersService.ApplyNow(obj);

                    return Ok(new { isSuccess = customerResp.IsSuccess, message = customerResp.Message, id = customerResp.Loans.UserId });

                }
                catch (Exception ex)
                {

                    return BadRequest(new { isSuccess = false, message = "Failed to Save!" });
                }

            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "null data!" });
            }

        }
    }
}
