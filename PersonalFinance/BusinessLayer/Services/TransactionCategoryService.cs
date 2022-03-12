using BusinessLayer.Interfaces;
using BusinessLayer.Mappers;
using BusinessLayer.Models;
using DataLayer.Entities;
using DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class TransactionCategoryService : ITransactionCategoryService
    {
        private readonly IRepository<CostCategory> _costCategoryRepository;
        private readonly IRepository<IncomeCategory> _incomeCategoryRepository;
        private readonly ITransactionService _transactionService;
        private readonly DbContext _dbContext;

        public TransactionCategoryService(IRepository<CostCategory> costCategoryRepository, IRepository<IncomeCategory> incomeCategoryrepository, DbContext dbContext, ITransactionService transactionService)
        {
            _costCategoryRepository = costCategoryRepository;
            _incomeCategoryRepository = incomeCategoryrepository;
            _transactionService = transactionService;
            _dbContext = dbContext;
        }

        public async Task<ServiceResult> CreateCategory(TransactionCategoryDTO categoryDTO)
        {
            if(categoryDTO.Type == TransactionType.Cost)
            {
                return await CreateCategoryCost(categoryDTO);
            }
            else
            {
                return await CreateCategoryIncome(categoryDTO);
            }
        }

        private async Task<ServiceResult> CreateCategoryIncome(TransactionCategoryDTO categoryDTO)
        {
            var incomeCategory = categoryDTO.MapDTOToIncomeCategory();

            _incomeCategoryRepository.Create(incomeCategory);
            await _dbContext.SaveChangesAsync();

            return ServiceResult.Success();
        }

        private async Task<ServiceResult> CreateCategoryCost(TransactionCategoryDTO categoryDTO)
        {
            var costCategory = categoryDTO.MapDTOToCostCategory();

            _costCategoryRepository.Create(costCategory);
            await _dbContext.SaveChangesAsync();

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteCategory(TransactionCategoryDTO categoryDTO, int userId, TransactionType transactionType)
        {
            if(transactionType == TransactionType.Cost)
            {
                var dbCostCategory = await _costCategoryRepository.Query()
                    .Include(c => c.Costs)
                    .FirstOrDefaultAsync(x => x.CostCategoryId == categoryDTO.CategoryId && x.UserId == userId);

                if (dbCostCategory == null)
                {
                    return ServiceResult.Error($"Cost category transaction with id {categoryDTO.CategoryId} is not found");
                }

                if (dbCostCategory.Costs.Any())
                {
                    return ServiceResult.Error($"There are some transaction(s) within this category. Please remove it at first.");
                }

                _costCategoryRepository.Delete(dbCostCategory);

                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var dbIncomeCategory = await _incomeCategoryRepository.Query()
                    .Include(c => c.Incomes)
                    .FirstOrDefaultAsync(x => x.IncomeCategoryId == categoryDTO.CategoryId && x.UserId == userId);

                if (dbIncomeCategory == null)
                {
                    return ServiceResult.Error($"Cost category transaction with id {categoryDTO.CategoryId} is not found");
                }

                if (dbIncomeCategory.Incomes.Any())
                {
                    return ServiceResult.Error($"There are some transaction(s) within this category. Please remove it at first.");
                }

                _incomeCategoryRepository.Delete(dbIncomeCategory);

                await _dbContext.SaveChangesAsync();
            }

            return ServiceResult.Success();
        }

        public async Task<IEnumerable<TransactionCategoryDTO>> GetAllCategoriesList(int userId)
        {
            var costCategoryDTOs = (await _costCategoryRepository.Query()
                .Where(u => u.UserId == userId).ToListAsync())
                .Select(x => TransactionMapper.MapToDTO(x));

            var incomeCategoryDTOs = (await _incomeCategoryRepository.Query()
                .Where(u => u.UserId == userId).ToListAsync())
                .Select(x => TransactionMapper.MapToDTO(x));

            return incomeCategoryDTOs.Union(costCategoryDTOs);
        }

        public async Task<TransactionCategoryDTO> GetCategory(int userId, int transactionId, TransactionType transactionType)
        {
            if (transactionType == TransactionType.Cost)
            {
                var CategoryDTOs = (await _costCategoryRepository.Query()
                        .FirstOrDefaultAsync(u => u.UserId == userId && u.CostCategoryId == transactionId))
                ?.MapToDTO();
                return CategoryDTOs;
            }
            else
            {
                var CategoryDTOs = (await _incomeCategoryRepository.Query()
                .FirstOrDefaultAsync(u => u.UserId == userId && u.IncomeCategoryId == transactionId))
                ?.MapToDTO();
                return CategoryDTOs;
            }
        }

        public async Task<ServiceResult> UpdateCategory(TransactionCategoryDTO categoryDTO, int userId, TransactionType transactionType)
        {
            if (transactionType == TransactionType.Cost)
                return await UpdateCostCategory(categoryDTO, userId);
            else
                return await UpdateIncomeCategory(categoryDTO, userId);
        }

        private async Task<ServiceResult> UpdateIncomeCategory(TransactionCategoryDTO categoryDTO, int userId)
        {
            var dbIncomeCategory = _incomeCategoryRepository.Find(categoryDTO.CategoryId);

            if (dbIncomeCategory == null)
            {
                return ServiceResult.Error($"Income category with id {categoryDTO.CategoryId} is not found");
            }
            if (dbIncomeCategory.UserId != userId)
            {
                return ServiceResult.Error($"Current user does not own income category with Id {categoryDTO.CategoryId}");
            }

            categoryDTO.MapDTOToIncomeCategory(dbIncomeCategory);

            await _dbContext.SaveChangesAsync();

            return ServiceResult.Success();
        }

        private async Task<ServiceResult> UpdateCostCategory(TransactionCategoryDTO categoryDTO, int userId)
        {
            var dbCostCategory = _costCategoryRepository.Find(categoryDTO.CategoryId);

            if (dbCostCategory == null)
            {
                return ServiceResult.Error($"Cost category with id {categoryDTO.CategoryId} is not found");
            }
            if (dbCostCategory.UserId != userId)
            {
                return ServiceResult.Error($"Current user does not own cost category with Id {categoryDTO.CategoryId}");
            }

            categoryDTO.MapDTOToCostCategory(dbCostCategory);

            await _dbContext.SaveChangesAsync();

            return ServiceResult.Success();
        }

        public async Task<IEnumerable<TransactionCategoryHistory>> GetTransactionCategoryHistories(UserDTO user)
        {
            var categoryList = (
                await GetAllCategoriesList(user.UserId)
                ).Select(x => new TransactionCategoryHistory(x))
                .ToList();

            var transactions = await _transactionService.GetAllTransactionList(user.UserId);

            return ProcessHistory(categoryList, transactions);
        }

        private IEnumerable<TransactionCategoryHistory> ProcessHistory(IEnumerable<TransactionCategoryHistory> categoryList, IEnumerable<TransactionDTO> transactions)
        {
            var now = DateTime.Now;
            var currentMonthRange = (start: FirstDayOfMonth(now), end: LastDayOfMonth(now));

            var range = Enumerable.Range(0, 3).Select(i =>
                (start: currentMonthRange.start.AddMonths(-i), end: currentMonthRange.end.AddMonths(-i))
            );

            foreach (var category in categoryList)
            {
                var history = new Collection<CategoryMonth>();
                foreach (var month in range)
                {
                    var monthTransactions = transactions
                        .Where(x => x.Category.Equals(category) && x.Date >= month.start && x.Date < month.end);

                    history.Add(new CategoryMonth
                    {
                        MonthYear = month.start,
                        MonthLimit = category.MonthLimit,
                        Amount = monthTransactions.Sum(x => x.Amount)
                    });
                }

                category.CategoryHistory = history.OrderBy(x => x.MonthYear);
            }

            return categoryList;
        }

        private DateTime FirstDayOfMonth(DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        private DateTime LastDayOfMonth(DateTime value)
        {
            return FirstDayOfMonth(value)
                .AddMonths(1)
                .AddMinutes(-1);
        }
    }
}
