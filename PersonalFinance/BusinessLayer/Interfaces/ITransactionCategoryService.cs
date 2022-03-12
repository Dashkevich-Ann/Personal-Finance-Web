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
        Task<TransactionCategoryDTO> GetCategory(int userId, int transactionId, TransactionType transactionType);
        Task<ServiceResult> CreateCategory(TransactionCategoryDTO categoryDTO);
        Task<ServiceResult> UpdateCategory(TransactionCategoryDTO categoryDTO, int userId, TransactionType transactionType);
        Task<ServiceResult> DeleteCategory(TransactionCategoryDTO categoryDTO, int userId, TransactionType transactionType);
        Task<IEnumerable<TransactionCategoryHistory>> GetTransactionCategoryHistories(UserDTO user);

    }
}
