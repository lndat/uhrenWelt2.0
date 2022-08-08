using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using uhrenWelt.Extensions;
using uhrenWelt.Interfaces;
using uhrenWelt.Models;
using uhrenWelt.Services;
using uhrenWelt.ViewModels.Cart;

namespace uhrenWelt.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly IProductService _productService;
    private readonly ILogger<CartController> _logger;
    private readonly ICartService _cartService;
    private readonly UserManager<AppUser> _userManager;

    public CartController(ILogger<CartController> logger, ICartService cartService, UserManager<AppUser> userManager, IProductService productService)
    {
        _productService = productService;
        _userManager = userManager;
        _cartService = cartService;
        _logger = logger;
    }
    
    public async Task<IActionResult> ShowCart(bool voucherCheck)
    {
        var appUser = await _userManager.FindByEmailAsync(User.Identity.Name);
        var cart = await _cartService.GetCustomerCart(appUser.Id);

        if (voucherCheck == true)
        {
            ViewBag.Message = "VoucherFail";
        }

        var vm = new IndexCartViewModel
        {
            Cart = new List<CartViewModel>()
        };

        foreach (var item in cart.OrderLines)
        {
            var getPrice = CalculateService.GetGrossPrice(_productService.GetProductById(item.ProductId).Result.NetUnitPrice, _productService.GetProductById(item.ProductId).Result.Category.TaxRate);
            var cartVm = new CartViewModel
            {
                OrderLineId = item.Id,
                ProductId = item.ProductId,
                ImagePath = _productService.GetProductById(item.ProductId).Result.ImagePath,
                ProductName = _productService.GetProductById(item.ProductId).Result.ProductName,
                ManufacturerName = _productService.GetProductById(item.ProductId).Result.Manufacturer.Name,
                NetUnitPrice = getPrice,
                LinePrice = cart.OrderLines.Where(x => x.Id == item.Id).Sum(ol => ol.Quantity * getPrice).ToPriceString("€"),
                PriceTotal = cart.PriceTotal.ToPriceString("€"),
                Quantity = item.Quantity,
                TaxRate = _productService.GetProductById(item.ProductId).Result.Category.TaxRate
            };
            vm.PriceTotal = cart.PriceTotal.ToPriceString("€");
            vm.QuantityTotal = cart.OrderLines.Sum(tp => tp.Quantity);
            vm.Cart.Add(cartVm);
        }

        return View(vm);
    }

    public async Task<IActionResult> AddProductToCart(int productId)
    {
        var appUser = await _userManager.FindByEmailAsync(User.Identity.Name);
        await _cartService.AddProductToCart(appUser.Id, productId, 1);

        return RedirectToAction("ShowCart", "Cart");
    }

    public async Task<IActionResult> DeleteProductFromCart(int productId, int orderLineId)
    {
        var appUser = await _userManager.FindByEmailAsync(User.Identity.Name);
        await _cartService.RemoveProductFromCart(appUser.Id, orderLineId);

        return RedirectToAction("ShowCart", "Cart");
    }

    public async Task<IActionResult> ChangeProductQuantity(int quantity, int orderLineId)
    {
        var appUser = await _userManager.FindByEmailAsync(User.Identity.Name);
        await _cartService.ChangeCartQuantity(appUser.Id, quantity, orderLineId);

        return RedirectToAction("ShowCart", "Cart");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
