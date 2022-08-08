using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using uhrenWelt.Data;
using uhrenWelt.Extensions;
using uhrenWelt.Interfaces;
using uhrenWelt.Models;
using uhrenWelt.Services;
using uhrenWelt.ViewModels.Order;
using Rotativa.AspNetCore;

namespace uhrenWelt.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IProductService _productService;
    private readonly ILogger<OrderController> _logger;
    private readonly ICartService _cartService;
    private readonly IVoucherService _voucherService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEmailService _emailService;

    public OrderController(ILogger<OrderController> logger, ICartService cartService, UserManager<AppUser> userManager, IProductService productService, IVoucherService voucherService, IServiceScopeFactory scopeFactory, IEmailService emailService)
    {
        _scopeFactory = scopeFactory;
        _voucherService = voucherService;
        _productService = productService;
        _userManager = userManager;
        _cartService = cartService;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmOrder(string voucher)
    {
        var appUser = await _userManager.FindByEmailAsync(User.Identity.Name);

        if (!String.IsNullOrWhiteSpace(voucher))
        {
            var checkVoucher = await _voucherService.CheckVoucher(voucher);
            if (!checkVoucher)
            {
                return RedirectToAction("ShowCart", "Cart", new { voucherCheck = true });
            }
            else
            {
                var cart = await _cartService.GetCustomerCart(appUser.Id);
                await _voucherService.CalculateVoucherAsync(voucher, cart.Id);
                ViewBag.Message = "VoucherSuccess";
            }
        }

        var customerCart = await _cartService.GetCustomerCart(appUser.Id);
        var orderViewModel = new OrderViewModel
        {
            FirstName = customerCart.FirstName,
            LastName = customerCart.LastName,
            Street = customerCart.Street,
            Zip = customerCart.Zip,
            City = customerCart.City,
            PriceTotal = customerCart.PriceTotal.ToPriceString("€"),
            OrderId = customerCart.Id,
            VoucherId = null,
            DateOrdered = DateTime.Now
        };

        foreach (var orderLine in customerCart.OrderLines)
        {
            var productPrice = CalculateService.GetGrossPrice(_productService.GetProductById(orderLine.ProductId).Result.NetUnitPrice, _productService.GetProductById(orderLine.ProductId).Result.Category.TaxRate);
            var product = await _productService.GetProductById(orderLine.ProductId);

            var orderLineViewModel = new OrderLineViewModel
            {
                ProductId = orderLine.ProductId,
                Name = product.ProductName,
                Manufacturer = product.Manufacturer.Name,
                Quantity = orderLine.Quantity,
                NetUnitPrice = productPrice.ToPriceString("€"),
                LinePrice = customerCart.OrderLines.Where(x => x.Id == orderLine.Id).Sum(ol => ol.Quantity * productPrice).ToPriceString("€"),
                TaxRate = orderLine.TaxRate
            };
            orderViewModel.OrderLines.Add(orderLineViewModel);
        }

        return View(orderViewModel);
    }

    public async Task<IActionResult> CompleteOrder()
    {
        var appUser = await _userManager.FindByEmailAsync(User.Identity.Name);
        var customerCart = await _cartService.GetCustomerCart(appUser.Id);

        var orderViewModel = new OrderViewModel
        {
            FirstName = customerCart.FirstName,
            LastName = customerCart.LastName,
            Street = customerCart.Street,
            Zip = customerCart.Zip,
            City = customerCart.City,
            PriceTotal = customerCart.PriceTotal.ToPriceString("€"),
            OrderId = customerCart.Id,
            VoucherId = null,
            DateOrdered = DateTime.Now
        };

        foreach (var orderLine in customerCart.OrderLines)
        {
            var productPrice = CalculateService.GetGrossPrice(_productService.GetProductById(orderLine.ProductId).Result.NetUnitPrice, _productService.GetProductById(orderLine.ProductId).Result.Category.TaxRate);
            var product = await _productService.GetProductById(orderLine.ProductId);

            var orderLineViewModel = new OrderLineViewModel
            {
                ProductId = orderLine.ProductId,
                Name = product.ProductName,
                Manufacturer = product.Manufacturer.Name,
                Quantity = orderLine.Quantity,
                NetUnitPrice = productPrice.ToPriceString("€"),
                LinePrice = customerCart.OrderLines.Where(x => x.Id == orderLine.Id).Sum(ol => ol.Quantity * productPrice).ToPriceString("€"),
                TaxRate = orderLine.TaxRate
            };
            orderViewModel.OrderLines.Add(orderLineViewModel);
        }

        var invoicePdf = new ViewAsPdf("CompleteOrder", orderViewModel);
        var pdfBytes = await invoicePdf.BuildFile(ControllerContext);
        var contentRootPath = (string)AppDomain.CurrentDomain.GetData("ContentRootPath");
        await System.IO.File.WriteAllBytesAsync(contentRootPath + $@"/Invoice/Invoice-{orderViewModel.OrderId}.pdf", pdfBytes);

        //await _emailService.SendEmail("lnedza.dev@icloud.com", appUser.Email, "Your invoice!", "Here is your invoice!", pdfBytes);

        using (var scope = _scopeFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();
            var getOrder = await db.Orders.Where(c => c.AppUserId == appUser.Id && c.DateOrdered == null).FirstOrDefaultAsync();

            getOrder.DateOrdered = DateTime.Now;
            db.Entry(getOrder).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }
        await _cartService.CreateCartAsync(appUser);

        return View("ThankYou");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
