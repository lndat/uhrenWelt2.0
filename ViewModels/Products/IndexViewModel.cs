using Microsoft.AspNetCore.Mvc.Rendering;

namespace uhrenWelt.ViewModels.Products
{
    public class IndexViewModel
    {
        public List<IndexProductViewModel> Products { get; set; }
        public string Search { get; set; }
        public int CategoryId { get; set; }
        public List<SelectListItem> Categories { get; } = new List<SelectListItem> 
        {
        new SelectListItem { Value = "1", Text = "Automatic" },
        new SelectListItem { Value = "2", Text = "Smart Watch" },
        new SelectListItem { Value = "3", Text = "Mechanical" },
        };  
        public int ManufacturerId { get; set; }
        public List<SelectListItem> Manufacturers { get; } = new List<SelectListItem> 
        {
        new SelectListItem { Value = "1", Text = "Rolex" },
        new SelectListItem { Value = "2", Text = "Breitling" },
        new SelectListItem { Value = "3", Text = "Tag Heuer" },
        new SelectListItem { Value = "4", Text = "Hublot" },
        new SelectListItem { Value = "5", Text = "IWC Schaffhausen" },
        new SelectListItem { Value = "6", Text = "Longines" },
        new SelectListItem { Value = "7", Text = "Tudor" },
        new SelectListItem { Value = "8", Text = "Panerai" },
        };  
    }
}