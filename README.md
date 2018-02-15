# .NET CORE SDK
To understand how this great framework works, I did a simple example to show you how to make a cross-platform project.

To this project was used:
- [Elementary Os (ubuntu)](https://elementary.io)
- [Sql Server for Linux (Express edition)](https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-setup)
- [.NET Core SDK 2.1.4](https://www.microsoft.com/net/download)
- [Visual Studio Code](https://code.visualstudio.com/)

*ps: This example will same work for MAC | Win | Linux*

# Sections
- [Initial](#initial)
- [Domain](#domain)
- [MVC - Category](#mvc - category)

## Initial
> **Commit** : [31dc559](https://github.com/uraquitanfilho/dotnetcore_store/tree/31dc5599ee52d4e30f9959538079dca983e1682a)
> ## Let's create the project ## 

_Project Name_: _**Store**_
___
1. Create the folder:
``` 
  mkdir Store\src
```  
2. Go to new folder: 
```
cd Store\src
```
3. Create the project: 
```
dotnet new classlib --name Store.Domain
```
4. back to Store folder: 
```
cd ..
```
5. Lets create the sln project
```
dotnet new sln --name Store
```
6. We need add the .csproj 
```
dotnet sln add src/Store.Domain/Store.Domain.csproj
```
7. Finaly we can do the restore project and Build to check if all is fine.
```
dotnet restore
dotnet build
```

## Domain
> **Commit** : [fbe2d55](https://github.com/uraquitanfilho/dotnetcore_store/tree/fbe2d550725843f19a36e4ef8ee04dcff9017d66)
> ## Let's create the Domains ## 

1. Go to Project Folder **Store**:
```
cd src/Store.Domain
```
2. Create a new Folder to Product Domain
```
  mkdir Products
```
_src/Store.Domain/Products_
  
3. Go to src/Store.Domain folder and create a file called: DomainException.cs
> _this class was created to prevent user error. Active Class. Always will put correct information._
```c
using System;

namespace Store.Domain
{
    public class DomainException : Exception
    {
        public DomainException(string error): base(error) {

        }

        public static void When(bool valid, string error) {
            if(!valid)
              throw new DomainException(error);
        }
    }
}
```

4. On the folder src/Store.Domain/Products create a class called: **Category.cs**
```c
namespace Store.Domain.Projects
{
    public class Category
    {
        public int Id { get; private set; }
        
        public string Name { get; private set; }

        public Category(string name)
        {
            ValidateAndSetName(name);
        }

        public void Update(string name) 
        {
            ValidateAndSetName(name);
        }
        private void ValidateAndSetName(string name)
        {
            DomainException.When(string.IsNullOrEmpty(name), "Name is required");
            Name = name;
        }
    }
}
```
5. On the folder src/Store.Domain/Products create a class called: **Product.cs**
```c
namespace Store.Domain.Projects
{
    public class Product
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public Category Category { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }

        public Product(string name,
                       Category category,
                       decimal price,
                       int stockQuantity)
        {
            ValidateValues(name, category, stockQuantity);
            SetProperties(name, category, price, stockQuantity);
        }

        public void Update(string name,
                       Category category,
                       decimal price,
                       int stockQuantity)
        {
            ValidateValues(name, category, stockQuantity);
            SetProperties(name, category, price, stockQuantity);
        } 
        private void SetProperties(string name, Category category, decimal price, int stockQuantity)
        {
            Name = name;
            Category = category;
            Price = price;
            StockQuantity = stockQuantity;
        }

        private void ValidateValues(string name, Category category, int stockQuantity)
        {
            DomainException.When(string.IsNullOrEmpty(name), "Name is required");
            DomainException.When(category == null, "Category is required");
            DomainException.When(Price < 0, "Price is required");
            DomainException.When(stockQuantity < 0, "Stock Quantity is required");
        }
    }
}
```
6. Create a folder called Dtos: **src/Store.Domain/Dtos**
> Lets to control the data persistence.
- Create a class called: **ProductDto.cs**
```c
namespace Store.Domain.Dtos
{
    public class ProductDto
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int CategoryId { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }

    }
}
```
7. Goto /src/Store.Domain and create an interface called: **IRepository.cs**
```
namespace Store.Domain
{  
    public interface IRepository<TEntity>
    {
        TEntity GetById(int id);
        void Save(TEntity entity);
    }
}
```
8. Go to src/Store.Domain/Projects and create a file called: **ProductStorer.cs**

```
using Store.Domain.Dtos;

namespace Store.Domain.Projects
{
    public class ProductStorer
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        public ProductStorer(IRepository<Product> productRepository, IRepository<Category> categoryRepository) {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public void Store(ProductDto dto) {
            var category = _categoryRepository.GetById(dto.CategoryId);
            DomainException.When(category == null, "Invalid Category");

            var product = _productRepository.GetById(dto.Id);
            if(product == null)
            {
                product = new Product(dto.Name, category, dto.Price, dto.StockQuantity);
                _productRepository.Save(product);
            }
            else {
                product.Update(dto.Name, category, dto.Price, dto.StockQuantity);
            }
        }
    }
}
```

8. Lets do same to Category. 
Go to src/Store.Domain/Dtos and create a class called: CategoryDto.cs
```
namespace Store.Domain.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
```

Go to src/Store.Domain/Projects and create a file called: **CategoryStorer.cs**
```
using Store.Domain.Dtos;

namespace Store.Domain.Projects
{
    public class CategoryStorer
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryStorer(IRepository<Category> categoryRepository)
        {
           _categoryRepository = categoryRepository;
        }

        public void Store(CategoryDto dto) {
            var category = _categoryRepository.GetById(dto.Id);

            if(category == null) {
                category = new Category(dto.Name);
                _categoryRepository.Save(category);
            }
            else 
              category.Update(dto.Name);
        }  
    }
}
```

## MVC - Category
> **Commit** : [915318e](https://github.com/uraquitanfilho/dotnetcore_store/tree/915318eed36801d3c43bea8669ad6681728dca90)
> ## Let's create the MVC Project to WEB Interface. And will implement Category View ## 

1. Go to folder: Store/src and lets create the MVC Project:
```
dotnet new mvc --name Store.Web
```
* delete the folder **lib** located at Store/src/Store.Web/wwwroot/lib

* delete the file: Store/src/Store.Web/bundleconfig.json 
_case your project comes with bower files, remove all_

* Install the Yarn 
_[What is Yarn?](https://code.facebook.com/posts/1840075619545360)_
```
npm install -g yarn
```
* Go to the folder **Store/src/Store.Web/** to create the package.json with command:
```
  yarn init
```
* Now, lets install some packages 
```
yarn add bootstrap@3.3.7 jquery jquery-validation jquery-validation-unobtrusive
```
_[What is GULP?](https://gulpjs.com/)_
```
yarn add gulp gulp-concat gulp-cssmin gulp-uncss browser-sync
```
* On the folder Store/src/Store.Web create the file **gulpfile.js**
```javascript
var gulp = require('gulp');
var concat = require('gulp-concat');
var cssmin = require('gulp-cssmin');
var uncss = require('gulp-uncss');
var browserSync = require('browser-sync').create();

gulp.task('browser-sync', () => {
  browserSync.init({
      proxy: 'localhost:5000'
  });

  gulp.watch('./styles/**/*.css',['css']);
  gulp.watch('./js/**/*.js',['js']);
});

gulp.task('js', () => {
   
    return gulp.src([
          './node_modules/bootstrap/dist/js/bootstrap.min.js',
          './node_modules/jquery/dist/jquery.min.js',
          './node_modules/jquery-validation/dist/jquery.validate.min.js',
          './node_modules/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js',
          './js/site.js'
        ])
        .pipe(gulp.dest('wwwroot/js/'))
        .pipe(browserSync.stream());
});

gulp.task('css', () => {

    return gulp.src([
        './styles/site.css',
        './node_modules/bootstrap/dist/css/bootstrap.css',
    ])
    .pipe(concat('site.min.css'))
    .pipe(cssmin())
    .pipe(uncss({html: ["Views/**/*.cshtml"]}))
    .pipe(gulp.dest('wwwroot/css'))
    .pipe(browserSync.stream());
});
```
* create 2 new folders on the path: **Store/src/Store.Web**
```
mkdir styles
mkdir js
```
_Store/src/Store.Web/styles_
_Store/src/Store.Web/js_

* Move the file **site.css**: Store/src/Store.Web/wwwroot/css/site.css to Store/src/Store.Web/styles/site.css

* Move the file **site.js**: Store/src/Store.Web/wwwroot/js/site.css to Store/src/Store.Web/js/site.js

* Go to /Store/src/Store.Web and lets type the prompt command to generate the css and js
```
 yarn gulp css
 yarn gulp js
```
* Now lets update the file: **Store/src/Store.Web/Views/Shared/_Layout.cshtml**

```
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Store.Web</title>
    <link rel="stylesheet" href="~/css/site.min.css" />

</head>
<body>
    <nav class="navbar navbar-inverse navbar-fixed-top">
        <div class="container">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <a asp-area="" asp-controller="Home" asp-action="Index" class="navbar-brand">Store.Web</a>
            </div>
            <div class="navbar-collapse collapse">
                <ul class="nav navbar-nav">
                    <li><a asp-area="" asp-controller="Home" asp-action="Index">Home</a></li>
                    <li><a asp-area="" asp-controller="Home" asp-action="About">About</a></li>
                    <li><a asp-area="" asp-controller="Home" asp-action="Contact">Contact</a></li>
                </ul>
            </div>
        </div>
    </nav>
    <div class="container body-content">
        @RenderBody()
        <hr />
        <footer>
            <p>&copy; 2018 - Store.Web</p>
        </footer>
    </div>

    <environment include="Development">
        <script src="~/js/jquery.min.js"></script>
        <script src="~/js/bootstrap.min.js"></script>
        <script src="~/js/site.js" asp-append-version="true"></script>
    </environment>
    <environment exclude="Development">
        <script src="https://ajax.aspnetcdn.com/ajax/jquery/jquery-2.2.0.min.js"
                asp-fallback-src="~/lib/jquery/dist/jquery.min.js"
                asp-fallback-test="window.jQuery"
                crossorigin="anonymous"
                integrity="sha384-K+ctZQ+LL8q6tP7I94W+qzQsfRV2a+AfHIi9k8z8l9ggpc8X+Ytst4yBo/hH+8Fk">
        </script>
        <script src="https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/bootstrap.min.js"
                asp-fallback-src="~/lib/bootstrap/dist/js/bootstrap.min.js"
                asp-fallback-test="window.jQuery && window.jQuery.fn && window.jQuery.fn.modal"
                crossorigin="anonymous"
                integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa">
        </script>
        <script src="~/js/site.js" asp-append-version="true"></script>
    </environment>

    @RenderSection("Scripts", required: false)
</body>
</html>
```
* Lets create the controller: **CategoryController.cs** at Store/src/Store.Web/Controllers

```
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Store.Web.Models;

namespace Store.Web.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateOrEdit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateOrEdit(int id)
        {
            return View();
        }
    }
}
```

* Create a folder called **Category** at: Store/src/Store.Web/Views
* On the folder **Category** Create 2 Files:
  - Index.cshtml
```
@{
    ViewData["Title"] = "Home Page";
}

<div class="row header">
    <div class="col-md-12">
        <h3>Categories</h3>
        <a href="/Category/CreateOrEdit" class="btn btn-primary">New</a>
    </div>
</div>
<div class="row">
    <div class="col-md-12">
        <table class="table table-hover">
            <tbody>
                <tr>
                    <td>
                        <a class="name">Category 1</a>
                    </td>
                    <td>
                        <a class="btn">Edit</a>
                    </td>
                </tr>
                <tr>
                    <td>
                        <a class="name">Category 2</a>
                    </td>
                    <td>
                        <a class="btn">Edit</a>
                    </td>
                </tr>
                <tr>
                    <td>
                        <a class="name">Category 3</a>
                    </td>
                    <td>
                        <a class="btn">Edit</a>
                    </td>
                </tr>                                
            </tbody>
        </table>
    </div>
</div>
```
_Static informations because we don't have database implementation **yet**_

 - CreateOrEdit.cshtml
```
@model Store.Domain.ViewModels.CategoryViewModel

@{
    ViewData["Title"] = "Home Page";
}

<div class="row header">
    <div class="co-md-12">
        <h3>Category</h3>
        <a href="/Category" class="btn btn-primary">Back</a>
    </div>
</div>
<div class="row form-wrapper">
    <div class="col-md-12">
        <form id="form" class="form-horizontal" asp-action="CreateOrEdit" asp-controller="Category" asp-anti-forgery="">
            <div class="form-group">
                <label class="col-md-2 control-label">Name</label>
                <div class="col-md-8">
                    input class="form-control" asp-for="Name">
                </div>
            </div>
            <div class="form-group">
                <div class="col-md-offset-2 col-md-8">
                    <button class="btn btn-success">Save</button>
                </div>
            </div>
        </form>
    </div>
</div> 
```