# .NET CORE SDK
To understand how this great framework works, I did a simple example to show you how to make a cross-platform project.

To this project was used:
- [Elementary Os (ubuntu)](https://elementary.io)
- [Sql Server for Linux (Express edition)](https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-setup)
- [.NET Core SDK 2.1.4](https://www.microsoft.com/net/download)
- [Visual Studio Code](https://code.visualstudio.com/)

> Visual Studio Code Extensions
_:Launch VS Code Quick Open (Ctrl+P), paste the following command, and press enter._
* ext install ms-vscode.csharp
* ext install jchannon.csharpextensions
* ext install ms-vscode.cpptools
* ext install DavidAnson.vscode-markdownlint

*ps: This example will same work for MAC | Win | Linux*

# Sections
- [Initial](#initial)
- [Domain](#domain)
- [MVC-Category](#mvc-category)
- [Entity-Framework](#entity-framework)
- [Migration](#migration)
- [Settings-DI](#settings-di)
- [Saving-Category](#saving-category)
- [Form Validation](#form-validation)
- [Domain Exception](#domain-exception)
- [Category Crud Finish](#category-crud-finish)
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
namespace Store.Domain.Products
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
namespace Store.Domain.Products
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
```c
namespace Store.Domain
{  
    public interface IRepository<TEntity>
    {
        TEntity GetById(int id);
        void Save(TEntity entity);
    }
}
```
8. Go to src/Store.Domain/Products and create a file called: **ProductStorer.cs**

```c
using Store.Domain.Dtos;

namespace Store.Domain.Products
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
```c
namespace Store.Domain.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
```

Go to src/Store.Domain/Products and create a file called: **CategoryStorer.cs**
```c
using Store.Domain.Dtos;

namespace Store.Domain.Products
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

## MVC-Category
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

```html
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

```c
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
```html
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
```html
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
## Entity-Framework
> **Commit** : [18d69b8](https://github.com/uraquitanfilho/dotnetcore_store/tree/18d69b85e352d1f2582276a90c9207397cf0e35e#domain)
> ## SQL Server Data Persistence using Entity Framework ## 

* Goto Store/src/ to create the Data Project
```
dotnet new classlib --name Store.Data
```
* remove **Classe1.cs** at /Store/src/Store.Data/

* add references to Store.Data access Store.Domain
```
dotnet add Store.Data/Store.Data.csproj reference Store.Domain/Store.Domain.csproj
```
* go to /Store folder to references Data to Project
```
dotnet sln add src/Store.Data/Store.Data.csproj
dotnet restore
dotnet build
```
* Go to /Store/src/Store.Data to create the context class called: **ApplicationDbContext.cs**

* Let's ADD Entity Framework References:
```
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```
* Add on the file: /Core/app/Core.Data/Core.Data.csproj
```xml
  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.1" />
  </ItemGroup>
```

* Let's create a new project to dependence injector
```
dotnet new classlib --name Store.DI

dotnet restore
dotnet build
```
* Go to Store folder to add new project to solution
```
dotnet sln add src/Store.DI/Store.DI.csproj
```
* Rename the class Store/src/Store.DI/Class1.cs to Bootstrap.cs
```c
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Store.Data;
namespace Store.DI
{
    public class Bootstrap
    {
        public static void Configure(IServiceCollection services, string connection) 
        {
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(connection));
        }
    }
}
```

* go to folder: /Store/src/Store.DI/ to add a new package:

```
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.SqlServer.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design

dotnet add Store.DI.csproj reference ../Store.Data/Store.Data.csproj

dotnet restore
dotnet build
```
* Now go to folder: /Stpre/src/Store.Web to references the DI to Web Project.
```
dotnet add Store.Web.csproj reference ../Store.DI/Store.DI.csproj
dotnet restore
dotnet build
```
* Go to Store/src/Store.Web/Startup.cs 
> On the ConfigureServices Method:

```c
        public void ConfigureServices(IServiceCollection services)
        {
            Bootstrap.Configure(services, configuration.GetConnectionString("DefaultConnection"));    
            services.AddMvc();
        }
```
## Migration
> **Commit** : [0203527](https://github.com/uraquitanfilho/dotnetcore_store/tree/0203527fc6642016816c3125e91efd3ae280b90e)
> ## Let's start the migration configuration ##

* Go to **Store/src/Store.Web/appsetings.json** to add the Connection String. _for this example we are using SQL SERVER 2017 express for linux_

```
{
  "ConnectionStrings":{
    "DefaultConnection": "Server=localhost;Database=storedb;User Id=your_login;Password=your_password;"
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```
_ps: change your_login and your_password_

* As we can see, the Store.Web has the connection String but any Entity Framework references. Than we need the bellow command to add migrations:
  - go to **Store** folder to add the Store.Web project to application:
  ```
  dotnet sln add ./src/Store.Web/Store.Web.csproj
  ``` 
* Go to Store/src/Store.Web/Startup.cs and add the references:
```c
using Store.DI;
```
* Changes ConfigureServies method to.
_ps: has an error: UpperCase the "C" Configuration.GetConection..._ 
```c
        public void ConfigureServices(IServiceCollection services)
        {
            Bootstrap.Configure(services, Configuration.GetConnectionString("DefaultConnection"));

            services.AddMvc();
        }
```
* Lets do restore build
```
dotnet restore
dotnet build
```
- Now we can go at **Store/src/Store.Data** folder and type the command to create the migration for Category
```
dotnet ef --startup-project ../Store.Web/Store.Web.csproj --project ./Store.Data.csproj migrations add AddCategory
```
* Now we need create the table on the SQL Server

```
dotnet ef --startup-project ../Store.Web/Store.Web.csproj --project ./Store.Data.csproj database update

```
* Great :) now the database **storedb** was created at SQL Server with a table called **categories**

## Settings-DI
> **Commit** : [1a35c46](https://github.com/uraquitanfilho/dotnetcore_store/tree/1a35c46365039e31867d323153efd0526dfa4dda)
> ## Now it's the time to adjust the Direct Injection ## 

* Lets Edit the file: Store/src/Store.Web/Store.Web.csproj
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>netcoreapp2.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.All" Version="2.0.5" />
  </ItemGroup>

  <ItemGroup>
    <DotNetCliToolReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Tools" Version="2.0.2" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Store.DI\Store.DI.csproj" />
    <ProjectReference Include="..\Store.Domain\Store.Domain.csproj" />
  </ItemGroup>

</Project>
```
* Let's do a restore /build on the project
```
dotnet restore
dotnet build
```
* Edit the class: **Store/src/Core.Web/Controllers/CategoryController.cs**
```c
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Store.Domain.Dtos;
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
        public IActionResult CreateOrEdit(CategoryDto dto)
        {
            return View();
        }
    }
}
```
* Go to **Store/src/Store.Domain** and let's create a class called: **Entity.cs**
```c
namespace Store.Domain
{
    public class Entity
    {
        public int Id {get; protected set;}
    }
}
```
* Let's edit the class **Store/src/Store.Domain/Category.cs**
```c
namespace Store.Domain.Products
{
    public class Category : Entity
    {
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
* Go to **Store/src/Store.Data** and let's create a class called: **Repository.cs**
```c
using Store.Domain;
using System.Linq;
namespace Store.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity: Entity
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context) {
            _context = context;
        }
        public TEntity GetById(int id) {
           return _context.Set<TEntity>().SingleOrDefault(e => e.Id == id);
        }
        public void Save(TEntity entity) {
           _context.Set<TEntity>().Add(entity);
        }
    }
}
```
* Edit the class: **Store/src/Store.DI/Bootstrap.cs**
> We need now make the injection
```c
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Domain;
using Store.Domain.Products;

namespace Store.DI
{
    public class Bootstrap
    {
        public static void Configure(IServiceCollection services, string connection) 
        {
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(connection));
            //Generic Injection
            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));  
            services.AddSingleton(typeof(CategoryStorer));
        }
    }
}
```
*Finally for this section edit the class **Store/src/Store.Web/Controllers/CategoryController.cs**
```c
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


```
* Now you can build the project 
```
dotnet build
```
## Saving-Category
> **Commit** : [f296f7e](https://github.com/uraquitanfilho/dotnetcore_store/tree/f296f7ed6998c6d7bbd142a90340599970de64f9)
> ## In this section we will save on the database the Category ## 

_ps: To debug on visual studio code_
* edit the file **.vscode/lanuch.json**
> On the section "configurations" edit:
```javascript
"program": "${workspaceRoot}/src/Store.Web/bin/Debug/netcoreapp2.0/Store.Web.dll",
"cwd": "${workspaceRoot}/src/Store.Web",
```
###
* Edit **src/Store.DI/Bootstrap.cs**
```c
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Domain;
using Store.Domain.Products;

namespace Store.DI
{
    public class Bootstrap
    {
        public static void Configure(IServiceCollection services, string connection) 
        {
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(connection));
            //Generic Injection
            services.AddSingleton(typeof(IRepository<>), typeof(Repository<>));  
            services.AddSingleton(typeof(CategoryStorer));
            services.AddSingleton(typeof(IUnitOfWork), typeof(UnitOfWork));
        }
    }
}
```
* Edit **src/Store.Data/Repository.cs**

```c
using System.Linq;
using Store.Domain;

namespace Store.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity: Entity
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context) {
            _context = context;
        }
        public TEntity GetById(int id) {
           var query = _context.Set<TEntity>().Where(e => e.Id == id);
           if(query.Any())
             return query.First();
           return null;  
        }
        public void Save(TEntity entity) {
           _context.Set<TEntity>().Add(entity);
        }
    }
}
```
* Create an interface called **src/Store.Domain/IUnitOfWork.cs**
```c
using System.Threading.Tasks;

namespace Store.Domain
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}
```

* Create a class called **src/Store.Data/UnitOfWork.cs**
```c
using System.Threading.Tasks;
using Store.Domain;

namespace Store.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context) {
            _context = context;
        }

        public async Task Commit() {
            await _context.SaveChangesAsync();
        }
    }
}
```
* Edit the class **src/Store.Domain/DomainException.cs**
```c
using System;

namespace Store.Domain
{
    public class DomainException : Exception
    {
        public DomainException(string error): base(error) {

        }

        public static void When(bool hasError, string error) {
            if(hasError)
              throw new DomainException(error);
        }
        
    }
}
```
* edit the class **src/Store.Web/Controllers/CategoryController.cs**
```c
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
```
* Edit the class **src/Store.Web/Startup.cs**
```c
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Store.DI;
using Store.Domain;

namespace Store.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Bootstrap.Configure(services, Configuration.GetConnectionString("DefaultConnection"));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.Use(async(context, next) => 
            {
              await next.Invoke();
              var unitOfWork = (IUnitOfWork)context.RequestServices.GetService(typeof(IUnitOfWork));
              await unitOfWork.Commit();
            });            
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
```
* Edit the html file **src/Store.Web/Views/Category/CreateOrEdit.cshtml**
```html
@model Store.Domain.Products.Category

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
                    <input class="form-control" asp-for="Name">
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
## Form Validation
> **Commit** : []()
> ## Let's resolve the CustomException ##

* Edit **Store/src/Core.Domain/Category.cs** 
```c
namespace Store.Domain.Products
{
    public class Category : Entity
    {
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
            DomainException.When(name.Length < 3, "Invalid Category");
            Name = name;
        }
    }
}
```
* Cut folder **Store/src/Core.Domain/Dtos** and paste in the **Core.Web** folder
* Now, Remove Store/src/Core.Web/Dtos/CategoryDto.cs
* Create a new folder inside Store.Web called **ViewsModels**
* create a class called **CategoryViewModel.cs**
```c
using System.ComponentModel.DataAnnotations;

namespace Store.Web.ViewsModels
{
    public class CategoryViewModel
    {
            public int Id {get; set;}
            [Required]
            public string Name {get; set;}
    }
}
```
* Edit **/Store.Domain/Products/CategoryStorer.cs**
```
namespace Store.Domain.Products
{
    public class CategoryStorer
    {
        private readonly IRepository<Category> _categoryRepository;

        public CategoryStorer(IRepository<Category> categoryRepository)
        {
           _categoryRepository = categoryRepository;
        }

        public void Store(int id, string name) {
            var category = _categoryRepository.GetById(id);

            if(category == null) {
                category = new Category(name);
                _categoryRepository.Save(category);
            }
            else 
              category.Update(name);
        }  
    }
}
```

* Edit **/Store.Domain/Products/ProductStorer.cs**

```
namespace Store.Domain.Products
{
    public class ProductStorer
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        public ProductStorer(IRepository<Product> productRepository, IRepository<Category> categoryRepository) {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public void Store(int id, int categoryId, string name, decimal price, int stockQuantity) {
            var category = _categoryRepository.GetById(categoryId);
            DomainException.When(category == null, "Invalid Category");

            var product = _productRepository.GetById(id);
            if(product == null)
            {
                product = new Product(name, category, price, stockQuantity);
                _productRepository.Save(product);
            }
            else {
                product.Update(name, category, price, stockQuantity);
            }
        }
    }
}
```
* Edit **Store.Web/Views/Category/CreateOrEdit.cshtml**
```c
@model Store.Web.ViewsModels.CategoryViewModel

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
                    <input class="form-control" asp-for="Name">
                    <span asp-validation-for="Name" class="text-danger"></span>
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

@section scripts {
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.validate.unobtrusive.js"></script>
}
```
## Domain Exception
> **Commit** : [7ba3156](https://github.com/uraquitanfilho/dotnetcore_store/tree/7ba31560d372fd9de8643162962ee698adc8d336)

* Create a new folder in the **Store.Web** called **Filters**
* Create a class called: **Store.Web/Filters/CustomExceptionFilter.cs**

```c
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Store.Domain;

namespace Store.Web.Filters
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
    public override void OnException(ExceptionContext context) {
            bool isAjaxCall = context.HttpContext.Request.Headers["x-requested-with"] == "XMLHttpRequest";

            if(isAjaxCall) {
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = 500;
                var message = context.Exception is DomainException? context.Exception.Message : "An error occored";
                context.Result = new JsonResult(message);
                context.ExceptionHandled = true;
            }

            base.OnException(context);
        }  
    }
}
```
* Edit **Store.Web/Startup.cs**


```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Store.DI;
using Store.Domain;
using Store.Web.Filters;

namespace Store.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Bootstrap.Configure(services, Configuration.GetConnectionString("DefaultConnection"));

            services.AddMvc(config => {
                config.Filters.Add(typeof(CustomExceptionFilter));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.Use(async(context, next) => 
            {
              await next.Invoke();
              var unitOfWork = (IUnitOfWork)context.RequestServices.GetService(typeof(IUnitOfWork));
              await unitOfWork.Commit();
            });            
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

```
* Edit **Store.Web/Views/Category/CreateOrEdit.cshtml**
```c
@model Store.Web.ViewsModels.CategoryViewModel

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
        <form id="form" class="form-horizontal" asp-action="CreateOrEdit" asp-controller="Category" asp-anti-forgery=""
        data-ajax="true" data-ajax-method="POST" data-ajax-failure="formOnFail" data-ajax-success="window.location = '/Category'">
            <input type="hidden" asp-for="Id">
            <div class="form-group">
                <label class="col-md-2 control-label">Name</label>
                <div class="col-md-8">
                    <input class="form-control" asp-for="Name">
                    <span asp-validation-for="Name" class="text-danger"></span>
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

@section scripts {
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.validate.unobtrusive.js"></script>
     <script src="/js/jquery.unobtrusive-ajax.min.js"></script>
}
```
* Edit **Store.Web/Views/Shared/_Layout.cshtml**
```c
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>@ViewData["Title"] - Store.Web</title>
    <link rel="stylesheet" href="~/css/site.min.css" />
    <link rel="stylesheet" href="//cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.css" />
     
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
        <script src="~/js/jquery.min.js"></script>
        <script src="~/js/bootstrap.min.js"></script>
        <script src="~/js/site.js" asp-append-version="true"></script> 
        <script src="https://ajax.aspnetcdn.com/ajax/jquery/jquery-2.2.0.min.js"
                asp-fallback-src="~/js/jquery.min.js"
                asp-fallback-test="window.jQuery"
                crossorigin="anonymous"
                integrity="sha384-K+ctZQ+LL8q6tP7I94W+qzQsfRV2a+AfHIi9k8z8l9ggpc8X+Ytst4yBo/hH+8Fk">
        </script>
        <script src="https://ajax.aspnetcdn.com/ajax/bootstrap/3.3.7/bootstrap.min.js"
                asp-fallback-src="~/js/bootstrap.min.js"
                asp-fallback-test="window.jQuery && window.jQuery.fn && window.jQuery.fn.modal"
                crossorigin="anonymous"
                integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa">
        </script>
        <script src="~/js/site.js" asp-append-version="true"></script>
    </environment>
    <script src="//cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js"></script>
    @RenderSection("Scripts", required: false)
</body>
</html>
```
Edit **wwwroot/js/site.js**
```javascript
function formOnFail(error){
    if(error && error.status === 500) {
        toastr.error("error.responseText");
    }
}
```
* use yarn to install the jquey-unobtrusive-ajax
```javascript
yarn add jquery-ajax-unobtrusive
```
* copy from node_modules and paste on the folder **wwwroot/js/jquery.unobtrusive-ajax.min.js**

## Category Crud Finish
> **Commit** : []()

* Edit **Controllers/CategoryController.cs**

```c
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Store.Domain;
using Store.Domain.Products;

using Store.Web.ViewsModels;

namespace Store.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CategoryStorer _categoryStorer;
        private readonly IRepository<Category> _categoryRepository;
        public CategoryController(CategoryStorer categoryStorer,
                IRepository<Category> categoryRepository) 
        {
            _categoryStorer = categoryStorer;
            _categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            var categories = _categoryRepository.All();
            var viewsModels = categories.Select( c => new CategoryViewModel{ Id = c.Id, Name = c.Name });

            return View(viewsModels);
        }

        public IActionResult CreateOrEdit(int id)
        {
            if(id > 0) {
                var category = _categoryRepository.GetById(id);
                var categoryViewModel = new CategoryViewModel{Id = category.Id, Name = category.Name};
                return View(categoryViewModel);
            } else return View();
        }

        [HttpPost]
        public IActionResult CreateOrEdit(CategoryViewModel viewModel)
        {
            _categoryStorer.Store(viewModel.Id, viewModel.Name);
            return RedirectToAction("Index");
        }
    }
}
```

* Edit **Views/Category/Index.cshtml**
```c
@model IEnumerable<Store.Web.ViewsModels.CategoryViewModel>
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
               @foreach(var viewModel in @Model) {
                    <tr>
                        <td>
                            <a class="name">@viewModel.Name</a>
                        </td>
                        <td>
                            <a href="/Category/CreateOrEdit/@viewModel.Id" class="btn">Edit</a>
                        </td>
                    </tr>
               }
            </tbody>
        </table>
    </div>
</div>
```
* Edit **/Store.Data/Repository.cs**

```c
using System.Collections.Generic;
using System.Linq;
using Store.Domain;

namespace Store.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity: Entity
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context) {
            _context = context;
        }

        public IEnumerable<TEntity> All() {
            return _context.Set<TEntity>().AsEnumerable();
        }

        public TEntity GetById(int id) {
           var query = _context.Set<TEntity>().Where(e => e.Id == id);
           if(query.Any())
             return query.First();
           return null;  
        }
        public void Save(TEntity entity) {
           _context.Set<TEntity>().Add(entity);
        }
    }
}
```
Edit **/Store.Domain/IRepository.cs**
```c
using System.Collections.Generic;

namespace Store.Domain
{  
    public interface IRepository<TEntity>
    {
        TEntity GetById(int id);
       IEnumerable<TEntity> All();
        void Save(TEntity entity);
    }
}
```


