using System;
using System.ComponentModel.DataAnnotations;
using StoreManagement.Models;

namespace StoreManagement.Areas.Sales.ViewModels;

public class OrderVM
{
  
            public int Id { get; set; }

            [Required(ErrorMessage = "Customer is required")]
            [Display(Name = "Customer")]
            public int CustomerId { get; set; }

            [Display(Name = "Order Number")]
            public string? OrderNumber { get; set; }

            [Required(ErrorMessage = "Order date is required")]
            [DataType(DataType.DateTime)]
            [Display(Name = "Order Date")]
            public DateTime OrderDate { get; set; } = DateTime.UtcNow;

            [Required(ErrorMessage = "Status is required")]
            [Display(Name = "Status")]
            public string Status { get; set; } = "Pending";

            [Display(Name = "Shipping Address")]
            [StringLength(500)]
            public string? ShippingAddress { get; set; }

            [Display(Name = "Billing Address")]
            [StringLength(500)]
            public string? BillingAddress { get; set; }

            [Display(Name = "Payment Method")]
            [StringLength(50)]
            public string? PaymentMethod { get; set; }

            [Display(Name = "Payment Status")]
            [StringLength(50)]
            public string PaymentStatus { get; set; } = "Pending";

            [Display(Name = "Shipping Cost")]
            [Range(0, double.MaxValue)]
            public decimal ShippingCost { get; set; }

            [Display(Name = "Tax Amount")]
            [Range(0, double.MaxValue)]
            public decimal TaxAmount { get; set; }

            [Display(Name = "Discount Amount")]
            [Range(0, double.MaxValue)]
            public decimal DiscountAmount { get; set; }

            [Display(Name = "Total Amount")]
            [Range(0, double.MaxValue)]
            public decimal TotalAmount { get; set; }

            [Display(Name = "Notes")]
            [StringLength(1000)]
            [DataType(DataType.MultilineText)]
            public string? Notes { get; set; }

            // For display only
            public string? CustomerName { get; set; }
            public List<OrderItemVM> OrderItems { get; set; } = [];

            // For dropdowns
            public List<Customer> Customers { get; set; } = [];
            public List<Product> Products { get; set; } = [];
      }


