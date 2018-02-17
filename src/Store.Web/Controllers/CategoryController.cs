using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Store.Domain.Products;
using Store.Web.Models;
using Store.Web.ViewsModels;

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
        public IActionResult CreateOrEdit(CategoryViewModel viewModel)
        {
            _categoryStorer.Store(viewModel.Id, viewModel.Name);
            return View();
        }
    }
}
