using uhrenWelt.Models;
using uhrenWelt.ViewModels.Products;

namespace uhrenWelt.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProductById(int productId);
        Task<List<Product>> GetAllProducts();
        Task<List<Product>> GetAllProducts(string search, int? manufacturerId, int? categoryId);
        Task<List<Top5ViewModel>> GetTopProducts();
    }
}