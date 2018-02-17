using Store.Domain.Products;

namespace Store.Domain.Sales
{
    public class SaleItem : Entity
    {
        public Product Product { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

        public SaleItem(Product product, int quantity)
        {
            DomainException.When(product == null, "Product is required");
            DomainException.When(quantity < 1, "Quanatity incorrect");
                        
            Product = product;
            Price = Product.Price;
            Quantity = quantity;
            Total = Price * Quantity;
        }
    }
}