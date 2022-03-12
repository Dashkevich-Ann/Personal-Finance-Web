using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PersonalFinance.Services;
using BusinessLayer.Interfaces;
using PersonalFinance.Models;

namespace PersonalFinance.Controllers
{
    [Route("finances/categories")]
    [Authorize(Roles = "User")]
    public class TransactionCategoriesController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITransactionCategoryService _transactionCategoryService;

        public TransactionCategoriesController(IAuthService authService, ITransactionCategoryService transactionCategoryService)
        {
            _authService = authService;
            _transactionCategoryService = transactionCategoryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("{categoryId:int}/type/{transactionType}/delete")]
        public async Task<IActionResult> DeleteCategoryView(int categoryId, TransactionType transactionType)
        {
            var user = await _authService.GetLoggedInUser(User);

            var categoryDto =
                await _transactionCategoryService.GetCategory(user.UserId, categoryId, transactionType);

            if (categoryDto == null)
                return NotFound();

            return PartialView("_Delete", categoryDto);
        }

        [HttpDelete]
        [Route("{categoryId:int}/type/{transactionType}/delete")]
        public async Task<IActionResult> Delete(int categoryId, TransactionType transactionType)
        {
            var user = await _authService.GetLoggedInUser(User);

            var categoryDto =
                await _transactionCategoryService.GetCategory(user.UserId, categoryId, transactionType);

            if (categoryDto == null)
                return NotFound();

            var deleteResult = await _transactionCategoryService.DeleteCategory(categoryDto, user.UserId, transactionType);

            if (!deleteResult.IsSuccess)
                return BadRequest();

            return Ok("Success");
        }

        [HttpGet]
        [Route("create")]
        public async Task<IActionResult> Create()
        {
            return PartialView("_Create", new CategoryViewModel());
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if(!ModelState.IsValid)
                return PartialView("_Create", model);

            var user = await _authService.GetLoggedInUser(User);

            var dto = new TransactionCategoryDTO
            {
                UserId = user.UserId,
                Type = model.Type,
                Name = model.Name,
                MonthLimit = model.Type == TransactionType.Cost ? model.MonthLimit : (decimal?)null
            };

            var result = await _transactionCategoryService.CreateCategory(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Errors.First());
                return PartialView("_Create", model);
            }

            return Ok("Success");
        }

        [HttpGet]
        [Route("{categoryId:int}/type/{transactionType}/edit")]
        public async Task<IActionResult> EditCategoryView(int categoryId, TransactionType transactionType)
        {
            var user = await _authService.GetLoggedInUser(User);

            var categoryDto =
                await _transactionCategoryService.GetCategory(user.UserId, categoryId, transactionType);

            if (categoryDto == null)
                return NotFound();

            var model = new CategoryViewModel
            {
                CategoryId = categoryDto.CategoryId,
                Type = categoryDto.Type,
                Name = categoryDto.Name,
                MonthLimit = categoryDto.MonthLimit
            };

            return PartialView("_Edit", model);
        }

        [HttpPost]
        [Route("{categoryId:int}/type/{transactionType}/edit")]
        public async Task<IActionResult> Edit(int categoryId, TransactionType transactionType, CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_Edit", model);

            var user = await _authService.GetLoggedInUser(User);

            var categoryDto =
                await _transactionCategoryService.GetCategory(user.UserId, categoryId, transactionType);

            if (categoryDto == null || categoryDto.Type != model.Type)
                return NotFound();

            categoryDto.Name = model.Name;
            categoryDto.MonthLimit = model.MonthLimit;

            var result = await _transactionCategoryService.UpdateCategory(categoryDto, user.UserId, transactionType);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Errors.First());
                return PartialView("_Edit", model);
            }


            return Ok("Success");
        }



    }
}
