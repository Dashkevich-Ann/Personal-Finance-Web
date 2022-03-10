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
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Cost> _costRepository;
        private readonly IRepository<Income> _incomeRepository;
        private readonly DbContext _dbContext;

        public TransactionService(IRepository<Cost> costRepository, IRepository<Income> incomeRepository, DbContext dbContext)
        {
            _costRepository = costRepository;
            _incomeRepository = incomeRepository;
            _dbContext = dbContext;
        }

        public async Task<ServiceResult> CreateTransaction(TransactionDTO transactionDTO)
        {
            if (transactionDTO.Category.Type == TransactionType.Cost)
                return await CreateCost(transactionDTO);
            else
                return await CreateIncome(transactionDTO);               
        }

        private async Task<ServiceResult> CreateIncome(TransactionDTO transactionDTO)
        {
            var income = transactionDTO.MapIncomeFromDTO();

            _incomeRepository.Create(income);
            await _dbContext.SaveChangesAsync();

            return ServiceResult.Success();
        }

        private async Task<ServiceResult> CreateCost(TransactionDTO transactionDTO)
        {
            var cost = transactionDTO.MapCostFromDTO();

            _costRepository.Create(cost);
            await _dbContext.SaveChangesAsync();

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteTransaction(int transactionId, int userId, TransactionType transactionType)
        {
            if(transactionType == TransactionType.Cost)
            {
                var dbCost = await _costRepository.Query()
                    .FirstOrDefaultAsync(x => x.CostId == transactionId && x.UserId == userId);

                if(dbCost == null)
                {
                    return ServiceResult.Error($"Cost transaction with id {transactionId} is not found");
                }

                _costRepository.Delete(dbCost);
                
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var dbIncome = await _incomeRepository.Query()
                    .FirstOrDefaultAsync(x => x.IncomeId == transactionId && x.UserId == userId);

                if (dbIncome == null)
                {
                    return ServiceResult.Error($"Income transaction with id {transactionId} is not found");
                }

                _incomeRepository.Delete(dbIncome);

                await _dbContext.SaveChangesAsync();
            }

            return ServiceResult.Success();
        }

        public async Task<IEnumerable<TransactionDTO>> GetAllTransactionList(int userId)
        {
            var costDTOs = (await _costRepository.Query()
                .Include(x => x.CostCategory)
                .Where(u => u.UserId == userId).ToListAsync())
                .Select(x => TransactionMapper.MapToDTO(x));

            var incomeDTOs = (await _incomeRepository.Query()
                .Include(x => x.IncomeCategory)
                .Where(u => u.UserId == userId).ToListAsync())
                .Select(x => TransactionMapper.MapToDTO(x));

            return incomeDTOs.Union(costDTOs);
        }

        public async Task<IEnumerable<TransactionDTO>> GetTransactionList(int userId, TransactionType transactionType)
        {
            if (transactionType == TransactionType.Cost)
            {
                var DTOs = (await _costRepository.Query()
                .Where(u => u.UserId == userId).ToListAsync())
                .Select(x => TransactionMapper.MapToDTO(x));
                return DTOs;
            }
            else
            {
                var DTOs = (await _incomeRepository.Query()
                .Where(u => u.UserId == userId).ToListAsync())
                .Select(x => TransactionMapper.MapToDTO(x));
                return DTOs;
            } 
        }

        public async Task<ServiceResult> UpdateTransaction(TransactionDTO transactionDTO, int userId, TransactionType transactionType)
        {
            if (transactionType == TransactionType.Cost)
                return await UpdateCost(transactionDTO, userId);
            else
                return await UpdateIncome(transactionDTO, userId);
        }

        private async Task<ServiceResult> UpdateCost(TransactionDTO transactionDTO, int userId)
        {
            var dbCost = _costRepository.Find(transactionDTO.TransactionId);

            if(dbCost == null)
            {
                return ServiceResult.Error($"Cost with id {transactionDTO.TransactionId} is not found"); 
            }
            if(dbCost.UserId != userId)
            {
                return ServiceResult.Error($"Current user does not own cost with Id {transactionDTO.TransactionId}");
            }

            transactionDTO.MapCostFromDTO(dbCost);

            await _dbContext.SaveChangesAsync();

            return ServiceResult.Success();
        }

        private async Task<ServiceResult> UpdateIncome(TransactionDTO transactionDTO, int userId)
        {
            var dbIncome = _incomeRepository.Find(transactionDTO.TransactionId);

            if(dbIncome == null)
            {
                return ServiceResult.Error($"Income with id {transactionDTO.TransactionId} is not found");
            }

            if (dbIncome.UserId != userId)
            {
                return ServiceResult.Error($"Current user does not own income with Id {transactionDTO.TransactionId}");
            }

            transactionDTO.MapIncomeFromDTO(dbIncome);

            await _dbContext.SaveChangesAsync();

            return ServiceResult.Success();
        }

        public async Task<ServiceResult<TransactionDTO>> GetTransaction(int userId, int transactionId, TransactionType transactionType)
        {
            if(transactionType == TransactionType.Cost)
            {
                var cost = await _costRepository.Query()
                    .Include(c => c.CostCategory)
                    .FirstOrDefaultAsync(c => c.CostId == transactionId && c.UserId == userId);

                if (cost == null)
                    return ServiceResult<TransactionDTO>.Error($"Cost with ID {transactionId} was not found");

                return ServiceResult.Success(cost.MapToDTO());
            }
            else
            {
                var cost = await _incomeRepository.Query()
                        .Include(c => c.IncomeCategory)
                        .FirstOrDefaultAsync(c => c.IncomeCategoryId == transactionId && c.UserId == userId);

                if (cost == null)
                    return ServiceResult<TransactionDTO>.Error($"Income with ID {transactionId} was not found");

                return ServiceResult.Success(cost.MapToDTO());

            }
        }
    }
}
