using BusinessLayer.Interfaces;
using BusinessLayer.Mappers;
using BusinessLayer.Models;
using DataLayer.Entities;
using DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class TransactionCategoryService : ITransactionCategoryService
    {
        private readonly IRepository<CostCategory> _costCategoryRepository;
        private readonly IRepository<IncomeCategory> _incomeCategoryRepository;
        private readonly DbContext _dbContext;

        public TransactionCategoryService(IRepository<CostCategory> costCategoryRepository, IRepository<IncomeCategory> incomeCategoryrepository, DbContext dbContext)
        {
            _costCategoryRepository = costCategoryRepository;
            _incomeCategoryRepository = incomeCategoryrepository;
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

        public async Task<IEnumerable<TransactionCategoryDTO>> GetCategoriesList(int userId, TransactionType transactionType)
        {
            if (transactionType == TransactionType.Cost)
            {
                var CategoryDTOs = (await _costCategoryRepository.Query()
                .Where(u => u.UserId == userId).ToListAsync())
                .Select(x => TransactionMapper.MapToDTO(x));
                return CategoryDTOs;
            }
            else
            {
                var CategoryDTOs = (await _incomeCategoryRepository.Query()
                .Where(u => u.UserId == userId).ToListAsync())
                .Select(x => TransactionMapper.MapToDTO(x));
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
    }
}
