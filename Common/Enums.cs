namespace StoreManagement.Common
{
      public enum UserRole
      {
            Admin = 1,
            Manager = 2,
            Staff = 3,
            Customer = 4
      }

      public enum OrderStatus
      {
            Pending = 1,
            Processing = 2,
            Shipped = 3,
            Delivered = 4,
            Cancelled = 5,
      }

      public enum ProductStatus
      {
            InStock = 1,
            OutOfStock = 2,
            Discontinued = 3
      }

      public enum PaymentStatus
      {
            Pending = 1,
            Paid = 2,
            Failed = 3,
            Refunded = 4
      }

      public enum StockTransactionType
      {
            Purchase = 1,
            Sale = 2,
            Return = 3
      }

};

