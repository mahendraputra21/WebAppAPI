using System.ComponentModel.DataAnnotations;

namespace CQRSWebApplication.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Display(Name = "ProductName")]
        public string Name { get; set; }
        public string Barcode { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
        public string Description { get; set; }
        public decimal Rate { get; set; }

        [Display(Name = "Price($)")]
        public decimal BuyingPrice { get; set; }
        public string ConfidentialData { get; set; }
    }
}

