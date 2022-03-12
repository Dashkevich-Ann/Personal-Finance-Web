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
        [Route("transaction/{transactionId:int}/type/{transactionType}/edit")]

        public async Task<ActionResult> Edit(int transactionId, TransactionType transactionType)
        {
            var user = await _authService.GetLoggedInUser(User);
            var dto = await _transactionService.GetTransaction(user.UserId, transactionId, transactionType);

            await SetViewBagCategories(user);

            if (!dto.IsSuccess)
                return NotFound();

            var model = new TransactionViewModel
            {
                TransactionId = dto.Result.TransactionId,
                Amount = dto.Result.Amount,
                Comment = dto.Result.Comment,
                Date = dto.Result.Date,
                TransactionType = dto.Result.Category.Type,
                IncomeCategoryId = dto.Result.Category.Type == TransactionType.Income ? dto.Result.Category.CategoryId : (int?)null,
                CostCategoryId = dto.Result.Category.Type == TransactionType.Cost ? dto.Result.Category.CategoryId : (int?)null
            };

            return PartialView("_EditCreate", model);
        }

        [HttpPost]
        [Route("transaction/edit")]

        public async Task<ActionResult> Edit(TransactionViewModel model)
        {
            var user = await _authService.GetLoggedInUser(User);
            var categories = await _transactionCategoryService.GetAllCategoriesList(user.UserId);
            await SetViewBagCategories(user);

            if (!ModelState.IsValid)
            {
                return PartialView("_EditCreate", model);
            }

            var dtoResult = await _transactionService.GetTransaction(user.UserId, model.TransactionId, model.TransactionType);

            if (!dtoResult.IsSuccess)
                return NotFound();

            var dto = new TransactionDTO
            {
                UserId = user.UserId,
                TransactionId = model.TransactionId,
                Amount = model.Amount,
                Date = model.Date.Value,
                Comment = model.Comment,
                Category = categories.FirstOrDefault(x => x.Type == model.TransactionType
                                                          && (x.CategoryId == model.CostCategoryId || x.CategoryId == model.IncomeCategoryId))
            };

            var updateResult =
                await _transactionService.UpdateTransaction(dto, user.UserId, dto.Category.Type);

            if (!updateResult.IsSuccess)
            {
                ModelState.AddModelError("", updateResult.Errors.First());
                return PartialView("_EditCreate", model);
            }

            return Ok("Success");
        }

        [HttpGet]
        [Route("transaction/create")]

        public async Task<ActionResult> CreateView()
        {
            var user = await _authService.GetLoggedInUser(User);
            await SetViewBagCategories(user);

            var model = new TransactionViewModel();

            return PartialView("_EditCreate", model);
        }

        private async Task SetViewBagCategories(UserDTO user)
        {
            var categories = await _transactionCategoryService.GetAllCategoriesList(user.UserId);

            var costCategories = categories.Where(x => x.Type == TransactionType.Cost);
            var incomeCategories = categories.Where(x => x.Type == TransactionType.Income);
            ViewBag.CostTypes = costCategories;
            ViewBag.IncomeTypes = incomeCategories;
        }

        [HttpPost]
        [Route("transaction/create")]

        public async Task<ActionResult> Create(TransactionViewModel model)
        {
            var user = await _authService.GetLoggedInUser(User);
            var categories = await _transactionCategoryService.GetAllCategoriesList(user.UserId);
            await SetViewBagCategories(user);

            if (!ModelState.IsValid)
            {
                return PartialView("_EditCreate", model);
            }

            var dto = new TransactionDTO
            {
                UserId = user.UserId,
                Amount = model.Amount,
                Date = model.Date.Value,
                Comment = model.Comment,
                Category = categories.FirstOrDefault(x => x.Type == model.TransactionType
                                                          && (x.CategoryId == model.CostCategoryId || x.CategoryId == model.IncomeCategoryId))
            };

            var result = await _transactionService.CreateTransaction(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Errors.First());
                return PartialView("_EditCreate", model);
            }

            return Ok("Success");
        }
    }
}
