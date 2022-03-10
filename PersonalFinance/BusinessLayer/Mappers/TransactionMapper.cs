using BusinessLayer.Models;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Mappers
{
    public static class TransactionMapper
    {
        public static TransactionDTO MapToDTO(this Cost cost)
        {
            var costDTO = new TransactionDTO
            {
                UserId = cost.UserId,
                TransactionId = cost.CostId,
                Date = cost.Date,
                Amount = cost.Amount,
                Comment = cost.Comment,
                Category = cost.CostCategory.MapToDTO()
            };
            return costDTO;
        }

        public static TransactionDTO MapToDTO(this Income income)
        {
            var incomeDTO = new TransactionDTO
            {
                UserId = income.UserId,
                TransactionId = income.IncomeId,
                Date = income.Date,
                Amount = income.Amount,
                Comment = income.Comment,
                Category = income.IncomeCategory.MapToDTO()
            };
            return incomeDTO;
        }

        public static TransactionCategoryDTO MapToDTO(this CostCategory costCategory)
        {
            var costCategoryDTO = new TransactionCategoryDTO
            {
                UserId = costCategory.UserId,
                CategoryId = costCategory.CostCategoryId,
                Name = costCategory.Name,
                MonthLimit = costCategory.CostLimit,
                Type = TransactionType.Cost
            };
            return costCategoryDTO;
        }

        public static TransactionCategoryDTO MapToDTO(this IncomeCategory incomeCategory)
        {
            var incomeCategoryDTO = new TransactionCategoryDTO
            {
                UserId = incomeCategory.UserId,
                CategoryId = incomeCategory.IncomeCategoryId,
                Name = incomeCategory.Name,
                Type = TransactionType.Income
            };
            return incomeCategoryDTO;
        }

        public static Cost MapCostFromDTO(this TransactionDTO transaction, Cost cost = null)
        {
            cost = cost ?? new Cost
            {
                UserId = transaction.UserId
            };

            cost.Date = transaction.Date;
            cost.CostCategoryId = transaction.Category.CategoryId;
            cost.Amount = transaction.Amount;
            cost.Comment = transaction.Comment;

            return cost;
        }

        public static Income MapIncomeFromDTO(this TransactionDTO transaction, Income income = null)
        {
            income = income ?? new Income
            {
                UserId = transaction.UserId
            };

            income.Date = transaction.Date;
            income.IncomeCategoryId = transaction.Category.CategoryId;
            income.Amount = transaction.Amount;
            income.Comment = transaction.Comment;

            return income;
        }

        public static IncomeCategory MapDTOToIncomeCategory(this TransactionCategoryDTO category, IncomeCategory incomeCategory = null)
        {
            incomeCategory = incomeCategory ?? new IncomeCategory
            {
                UserId = category.UserId
            };

            incomeCategory.Name = category.Name;

            return incomeCategory;
        }

        public static CostCategory MapDTOToCostCategory(this TransactionCategoryDTO category, CostCategory costCategory = null)
        {
            costCategory = costCategory ?? new CostCategory
            {
                UserId = category.UserId
            };

            costCategory.Name = category.Name;
            costCategory.CostLimit = category.MonthLimit ?? 0;

            return costCategory;
        }
    }
}
