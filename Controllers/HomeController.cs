using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using uhrenWelt.Extensions;
using uhrenWelt.Interfaces;
using uhrenWelt.Models;
using uhrenWelt.Services;
using uhrenWelt.ViewModels.Products;

namespace uhrenWelt.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProductService _productService;


    public HomeController(ILogger<HomeController> logger, IProductService productService)
    {
        _productService = productService;
        _logger = logger;
    }
    public async Task<IActionResult> Index()
    {
        var dbProducts = await _productService.GetTopProducts();
        var top5Products = new List<Top5ViewModel>();
        

        foreach (var dbProduct in dbProducts)
        {
            var prodVm = new Top5ViewModel
            {
                Id = dbProduct.Id,
                ProductName = dbProduct.ProductName,
                ImagePath = dbProduct.ImagePath,
                ManufacturerName = dbProduct.ManufacturerName,
                UnitPrice = dbProduct.UnitPrice
            };
            top5Products.Add(prodVm);
            
        }

        return View(top5Products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
