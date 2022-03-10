using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ITransactionCategoryService
    {
        Task<IEnumerable<TransactionCategoryDTO>> GetAllCategoriesList(int userId);
        Task<IEnumerable<TransactionCategoryDTO>> GetCategoriesList(int userId, TransactionType transactionType);
        Task<ServiceResult> CreateCategory(TransactionCategoryDTO categoryDTO);
        Task<ServiceResult> UpdateCategory(TransactionCategoryDTO categoryDTO, int userId, TransactionType transactionType);
        Task<ServiceResult> DeleteCategory(TransactionCategoryDTO categoryDTO, int userId, TransactionType transactionType);
    }
}
