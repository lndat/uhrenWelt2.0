using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using uhrenWelt.Extensions;
using uhrenWelt.Interfaces;
using uhrenWelt.Models;
using uhrenWelt.Services;
using uhrenWelt.ViewModels.Products;

namespace uhrenWelt.Controllers;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;
    private readonly IProductService _productService;

    public ProductController(ILogger<ProductController> logger, IProductService productService)
    {
        _productService = productService;
        _logger = logger;
    }
    
    public async Task<IActionResult> Index()
    {
        var dbProducts = await _productService.GetAllProducts();

        var vm = new IndexViewModel
        {
            Products = new List<IndexProductViewModel>()
        };

        foreach (var dbProduct in dbProducts)
        {
            var prodVm = new IndexProductViewModel
            {
                Id = dbProduct.Id,
                ProductName = dbProduct.ProductName,
                ImagePath = dbProduct.ImagePath,
                ManufacturerName = dbProduct.Manufacturer.Name,
                Price = CalculateService.GetGrossPrice(dbProduct.NetUnitPrice, dbProduct.Category.TaxRate).ToPriceString("€")
            };
            vm.Products.Add(prodVm);
        }

        return View(vm);
    }

    public async Task<IActionResult> Search(string search, int? categoryId, int? manufacturerId)
    {
        var dbProducts = await _productService.GetAllProducts(search, manufacturerId, categoryId);
        var vm = new IndexViewModel
        {
            Products = new List<IndexProductViewModel>()
        };

        foreach (var dbProduct in dbProducts)
        {
            var prodVm = new IndexProductViewModel
            {
                Id = dbProduct.Id,
                CategoryId = dbProduct.CategoryId,
                ProductName = dbProduct.ProductName,
                ImagePath = dbProduct.ImagePath,
                ManufacturerName = dbProduct.Manufacturer.Name,
                ManufacturerId = dbProduct.Manufacturer.Id,
                Description = dbProduct.Description,
                Price = CalculateService.GetGrossPrice(dbProduct.NetUnitPrice, dbProduct.Category.TaxRate).ToPriceString("€")
            };
            vm.Products.Add(prodVm);
        }

        return View(vm);
    }

    public async Task<IActionResult> Detail(int productId)
    {
        var dbProduct = await _productService.GetProductById(productId);

        var vm = new DetailViewModel
        {
            Id = dbProduct.Id,
            Category = dbProduct.Category.Name,
            Description = dbProduct.Description,
            ImagePath = dbProduct.ImagePath,
            ManufacturerName = dbProduct.Manufacturer.Name,
            ProductName = dbProduct.ProductName,
            Price = CalculateService.GetGrossPrice(dbProduct.NetUnitPrice, dbProduct.Category.TaxRate).ToPriceString("€")
        };

        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
