using System;

namespace StoreManagement.Areas.Inventory.ViewModels;

public class StockFilterVM
{
      public string? SearchTerm { get; set; }
      public string? TransactionType { get; set; }
      public DateTime? StartDate { get; set; }
      public DateTime? EndDate { get; set; }
      public int? ProductId { get; set; }
      public int? SupplierId { get; set; }
}
