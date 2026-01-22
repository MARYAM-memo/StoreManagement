using System;
using System.Globalization;

namespace StoreManagement.Common
{
      public static class DateHelper
      {
            public static string? ToDisplayDate(this DateTime date)
            {
                  if (date == DateTime.MinValue)
                        return "-";

                  return date.ToString("dd MMM yyyy - HH:mm", CultureInfo.InvariantCulture);
            }
      }

      public static class PriceHelper
      {
            public static string ToCurrency(this decimal amount)
            {
                  return string.Format("{0:C}", amount);
            }
      }

      public static class StringHelper
      {
            public static string Truncate(string text, int maxLength)
            {
                  if (string.IsNullOrEmpty(text)) return text;
                  return text.Length <= maxLength ? text : string.Concat(text.AsSpan(0, maxLength), "...");
            }
      }

      public static class EnumHelper
      {
            public static string ToDisplayName<T>(this T enumValue) where T : Enum
            {
                  return enumValue.ToString();
            }
      }
}
