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
> **Commit** : []()
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

