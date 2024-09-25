using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MoneyMe.Models;
using MoneyMe.Controllers.CustomerLoans;
using MoneyMe.ViewModels;
using MoneyMe.Services;

namespace MoneyMe.API.CustomerLoansAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansAPIController : ControllerBase
    {
        private readonly MoneymeDbContext _context;
        private readonly UsersService _usersService;


        public LoansAPIController(MoneymeDbContext context, UsersService usersService)
        {
            _context = context;
            _usersService = usersService;
        }

        [HttpPost("CustomerApplication")]
        public async Task<IActionResult> CallInsertNewCustomer([FromBody] CustomerViewModel data)
        {
            try
            {

                var customerResp = await _usersService.InsertNewCustomer(data);

                return Ok(new { isSuccess = customerResp.IsSuccess, message = customerResp.Message, user_id = customerResp.Customer.UserId });
                
            }
            catch (Exception ex)
            {

                return BadRequest(new { isSuccess = false, message = "Failed to Save!" });
            }

        }
    }
}
