using System;

namespace StoreManagement.Common
{

      public static class StringExtensions
      {
            public static string Truncate(this string value, int maxLength, string truncationSuffix = "...")
            {
                  if (string.IsNullOrEmpty(value))
                        return value;

                  return value.Length <= maxLength ?
                      value :
                      value.Substring(0, maxLength) + truncationSuffix;
            }
      }
      public static class FileExtensions
      {
            public static string? ExtractImagePath(this IFormFile? imageFile, IWebHostEnvironment host)
            {
                  if (!(imageFile != null && imageFile.Length > 0)) return null;
                  // 1. توليد اسم فريد للصورة لتجنب overwrite لو في صورة بنفس الاسم موجودة
                  string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile!.FileName);

                  // 2. تحديد مكان حفظ الصورة في wwwroot/images/products
                  string uploadPath = Path.Combine(host.WebRootPath, "images/products");

                  // 3. التأكد إن الفولدر موجود
                  if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                  // 4. إنشاء المسار الكامل عشان نقدر نعمل FileStream ونحفظ الصورة
                  string filePath = Path.Combine(uploadPath, fileName);

                  // 5. حفظ الملف فعليًا
                  using (var fileStream = new FileStream(filePath, FileMode.Create))
                  {
                        imageFile.CopyTo(fileStream);
                  }

                  // 6. حفظ المسار النسبي في الداتابيز
                  return "/images/products/" + fileName;
            }
      }
}