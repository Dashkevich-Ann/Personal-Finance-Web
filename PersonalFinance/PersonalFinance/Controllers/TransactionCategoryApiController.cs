using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using PersonalFinance.Services;

namespace PersonalFinance.Controllers
{
    [Route("api/transaction/categories")]
    [ApiController]
    public class TransactionCategoryApiController : ControllerBase
    {
        private readonly ITransactionCategoryService _transactionCategoryService;
        private readonly IAuthService _authService;

        public TransactionCategoryApiController(ITransactionCategoryService transactionCategoryService, IAuthService authService)
        {
            _transactionCategoryService = transactionCategoryService;
            _authService = authService;
        }

        [HttpGet]
        [Route("histories")]
        public async Task<IEnumerable<TransactionCategoryHistory>> GetCategoryHistories()
        {
            var user = await _authService.GetLoggedInUser(User);

            return await _transactionCategoryService.GetTransactionCategoryHistories(user);
        }

    }
}
