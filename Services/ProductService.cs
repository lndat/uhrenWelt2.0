using Microsoft.EntityFrameworkCore;
using uhrenWelt.Data;
using uhrenWelt.Extensions;
using uhrenWelt.Interfaces;
using uhrenWelt.Models;
using uhrenWelt.ViewModels.Products;

namespace uhrenWelt.Services
{
    public class ProductService : IProductService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ProductService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<Product> GetProductById(int productId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                return await db.Products
                    .Include(c => c.Category)
                    .Include(m => m.Manufacturer)
                    .FirstOrDefaultAsync(p => p.Id == productId);
            }
        }

        public async Task<List<Product>> GetAllProducts()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                return await db.Products
                                .Include(c => c.Category)
                                .Include(m => m.Manufacturer)
                                .ToListAsync();
            }
        }
        public async Task<List<Top5ViewModel>> GetTopProducts()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                var orders = await db.OrderLines.ToListAsync();

                var topFive = orders.GroupBy(f => f.ProductId)
                   .Select(g => new Top5ViewModel
                   {
                       Id = g.Key,
                       Quantity = g.Sum(f => f.Quantity),
                       UnitPrice = CalculateService.GetGrossPrice(GetProductById(g.Key).Result.NetUnitPrice, GetProductById(g.Key).Result.Category.TaxRate).ToPriceString("€"),
                       NetUnitPrice = GetProductById(g.Key).Result.NetUnitPrice,
                       ProductName = GetProductById(g.Key).Result.ProductName,
                       ManufacturerName = GetProductById(g.Key).Result.Manufacturer.Name,
                       ImagePath = GetProductById(g.Key).Result.ImagePath
                   })
                   .OrderByDescending(x => x.Quantity).Take(5)
                   .ToList();

                return topFive;
            }
        }

        public async Task<List<Product>> GetAllProducts(string search, int? manufacturerId, int? categoryId)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();

                if (string.IsNullOrWhiteSpace(search) && manufacturerId is null && categoryId is null)
                    return await GetAllProducts();

                if (!string.IsNullOrWhiteSpace(search) && categoryId is null && manufacturerId is null)
                    return await db.Products.Where(s =>
                        s.ProductName.ToLower().Contains(search.ToLower())
                        || s.Description.ToLower().Contains(search.ToLower())
                        || s.Manufacturer.Name.ToLower().Contains(search.ToLower())
                        || s.Category.Name.ToLower().Contains(search.ToLower()))
                        .Include(c => c.Category)
                        .Include(m => m.Manufacturer)
                        .ToListAsync();

                if (!string.IsNullOrWhiteSpace(search) && !(categoryId is null) && manufacturerId is null)
                    return await db.Products.Where(s =>
                        s.ProductName.ToLower().Contains(search.ToLower())
                        || s.Description.ToLower().Contains(search.ToLower())
                        || s.Manufacturer.Name.ToLower().Contains(search.ToLower())
                        || s.Category.Name.ToLower().Contains(search.ToLower()))
                        .Where(c => c.CategoryId == categoryId)
                        .Include(c => c.Category)
                        .Include(m => m.Manufacturer)
                        .ToListAsync();

                if (!string.IsNullOrWhiteSpace(search) && categoryId is null && !(manufacturerId is null))
                    return await db.Products.Where(s =>
                        s.ProductName.ToLower().Contains(search.ToLower())
                        || s.Description.ToLower().Contains(search.ToLower())
                        || s.Manufacturer.Name.ToLower().Contains(search.ToLower())
                        || s.Category.Name.ToLower().Contains(search.ToLower()))
                        .Where(c => c.ManufacturerId == manufacturerId)
                        .Include(c => c.Category)
                        .Include(m => m.Manufacturer)
                        .ToListAsync();

                if (!string.IsNullOrWhiteSpace(search) && !(categoryId is null) && !(manufacturerId is null))
                    return await db.Products.Where(s =>
                        s.ProductName.ToLower().Contains(search.ToLower())
                        || s.Description.ToLower().Contains(search.ToLower())
                        || s.Manufacturer.Name.ToLower().Contains(search.ToLower())
                        || s.Category.Name.ToLower().Contains(search.ToLower()))
                        .Where(c => c.ManufacturerId == manufacturerId && c.CategoryId == categoryId)
                        .Include(c => c.Category)
                        .Include(m => m.Manufacturer)
                        .ToListAsync();

                if (!(categoryId is null) && manufacturerId is null)
                    return await db.Products.Where(pf => pf.CategoryId == categoryId.Value)
                        .Include(c => c.Category)
                        .Include(m => m.Manufacturer)
                        .ToListAsync();

                if (!(manufacturerId is null) && categoryId is null)
                    return await db.Products.Where(pf => pf.ManufacturerId == manufacturerId.Value)
                        .Include(c => c.Category)
                        .Include(m => m.Manufacturer)
                        .ToListAsync();

                if (!(manufacturerId is null) && !(categoryId is null))
                {
                    return await db.Products.Where(pf =>
                        pf.ManufacturerId == manufacturerId.Value && pf.CategoryId == categoryId.Value)
                        .Include(c => c.Category)
                        .Include(m => m.Manufacturer)
                        .ToListAsync();
                }

                return await GetAllProducts();
            }
        }
    }
}