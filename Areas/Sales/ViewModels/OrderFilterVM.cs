using System;

namespace StoreManagement.Areas.Sales.ViewModels;

public class OrderFilterVM
{
      public string? SearchTerm { get; set; }
      public string? Status { get; set; }
      public string? PaymentStatus { get; set; }
      public DateTime? StartDate { get; set; }
      public DateTime? EndDate { get; set; }
      public int? CustomerId { get; set; }
}
