using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinance.Controllers
{
    [Route("api/transaction")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITransactionService _transactionService;

        public TransactionController(IAuthService authService, ITransactionService transactionService)
        {
            _authService = authService;
            _transactionService = transactionService;
        }

        [HttpGet]
        [Route("")]
        public async Task<IEnumerable<TransactionDTO>> GetAll()
        {
            var user = await _authService.GetLoggedInUser(User);

            return await _transactionService.GetAllTransactionList(user.UserId);
        }
    }
}
