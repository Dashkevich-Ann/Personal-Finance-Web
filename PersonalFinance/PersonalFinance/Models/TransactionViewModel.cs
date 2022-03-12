using BusinessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinance.Models
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }

        [Required]
        [BindProperty, DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }

        [Required]
        [Range(0.0, Double.MaxValue)]
        public decimal Amount { get; set; }

        public string Comment { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        [RequireForConditional(Values = new[] { nameof(TransactionType.Income) }, PropertyName = nameof(TransactionType))]

        public int? IncomeCategoryId { get; set; }

        [RequireForConditional(Values = new[] { nameof(TransactionType.Cost) }, PropertyName = nameof(TransactionType))]
        public int? CostCategoryId { get; set; }

    }
}
