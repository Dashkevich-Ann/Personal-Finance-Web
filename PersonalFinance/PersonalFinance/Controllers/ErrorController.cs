using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinance.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet("denied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
