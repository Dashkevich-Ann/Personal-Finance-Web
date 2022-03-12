using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Models;
using PersonalFinance.Attributes;

namespace PersonalFinance.Models
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        public TransactionType Type { get; set; }

        [Required]
        [MaxLength(60)]
        public string Name { get; set; }

        [Range(0, Double.MaxValue)]
        public decimal? MonthLimit { get; set; }
    }
}
