using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interfaces;
using PersonalFinance.Services;

namespace PersonalFinance.Controllers
{
    [Route("finances/statistic")]
    public class StatisticController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly IAuthService _authService;

        public StatisticController(ITransactionService transactionService, IAuthService authService)
        {
            _transactionService = transactionService;
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _authService.GetLoggedInUser(User);
            var histogram = await _transactionService.GetTransactionHistogram(user);

            return View(histogram.ToLookup(x => x.Year));
        }
    }
}
