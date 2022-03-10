using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PersonalFinance.Models;
using PersonalFinance.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinance.Controllers
{
    [Route("finances")]
    [Authorize(Roles = "User")]
    public class MyFinanceController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly ITransactionCategoryService _transactionCategoryService;
        private readonly IAuthService _authService;

        public MyFinanceController(ITransactionService transactionService, IAuthService authService, ITransactionCategoryService transactionCategoryService)
        {
            _transactionService = transactionService;
            _authService = authService;
            _transactionCategoryService = transactionCategoryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("transaction/{transactionId:int}/type/{transactionType}/delete")]
        public async Task<IActionResult> DeleteTransactionView(int transactionId, TransactionType transactionType)
        {
            var user = await _authService.GetLoggedInUser(User);

            var transactionResult = await _transactionService.GetTransaction(user.UserId, transactionId, transactionType);

            if (!transactionResult.IsSuccess)
                return NotFound();

            return PartialView("_DeleteTransaction", transactionResult.Result);
        }


        [HttpGet]
        [Route("transaction/create")]

        public async Task<ActionResult> CreateView()
        {
            var user = await _authService.GetLoggedInUser(User);
            var categories = await _transactionCategoryService.GetAllCategoriesList(user.UserId);

            var costCategories = categories.Where(x => x.Type == TransactionType.Cost);
            var incomeCategories = categories.Where(x => x.Type == TransactionType.Income);

            var model = new TransactionViewModel();
            ViewBag.CostTypes = costCategories;
            ViewBag.IncomeTypes = incomeCategories;

            return PartialView("_CreateNew", model);
        }
    }
}
