using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Store.Domain.Dtos;
using Store.Domain.Products;
using Store.Web.Models;

namespace Store.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryStorer _categoryStorer;

        public CategoryController(CategoryStorer categoryStorer) {
            _categoryStorer = categoryStorer;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateOrEdit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateOrEdit(CategoryDto dto)
        {
            _categoryStorer.Store(dto);
            return View();
        }
    }
}
