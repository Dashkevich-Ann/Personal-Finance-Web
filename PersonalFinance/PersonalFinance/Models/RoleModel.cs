using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinance.Models
{
    public class RoleModel : IChecked
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }

    }
}
