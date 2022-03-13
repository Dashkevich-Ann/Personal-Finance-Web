using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface ITransactionService
    {
        Task<IEnumerable<TransactionDTO>> GetAllTransactionList(int userId);
        Task<IEnumerable<TransactionDTO>> GetTransactionList(int userId, TransactionType transactionType);
        Task<ServiceResult<TransactionDTO>> GetTransaction(int userId, int transactionId, TransactionType transactionType);
        Task<ServiceResult> CreateTransaction(TransactionDTO transactionDTO);
        Task<ServiceResult> UpdateTransaction(TransactionDTO transactionDTO, int userId, TransactionType transactionType);
        Task<ServiceResult> DeleteTransaction(int transactionId, int userId, TransactionType transactionType);
        Task<IEnumerable<DateTime>> GetTransactionHistogram(UserDTO user);
    }
}
