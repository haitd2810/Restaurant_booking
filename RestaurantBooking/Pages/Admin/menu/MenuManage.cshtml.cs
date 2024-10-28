using DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Drawing;
using System.Numerics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

namespace RestaurantBooking.Pages.Admin.Member
{
    public class MenuManageModel : PageModel
    {
        private readonly IConfiguration Configuration;
        public MenuManageModel( IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public string Search { get; set; }
        public int? MinPrice { get; set; } = 0;
        public int? MaxPrice { get; set; } = 100000000;
        public int? Category { get; set; }
        public List<Menu> menu { get; set; } = new List<Menu>();
        public List<Category> categories { get; set; } = new List<Category>();

        public void OnGet(string search, string category, string min, string max, int pageIndex = 1, int pageSize = 10)
        {
            Search = search;
            CurrentPage = pageIndex;

            MinPrice = int.TryParse(min, out var parsedMin) ? parsedMin : 0;
            MaxPrice = int.TryParse(max, out var parsedMax) ? parsedMax : 100000000;

            Category = int.TryParse(category, out var parsedCategory) && parsedCategory != 0 ? parsedCategory : (int?)null;

            var query = RestaurantContext.Ins.Menus.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }

            if (Category.HasValue)
            {
                query = query.Where(x => x.CateId == Category.Value);
            }

            if (MinPrice.HasValue && MaxPrice.HasValue)
            {
                query = query.Where(x => x.Price >= MinPrice && x.Price <= MaxPrice);
            }

            categories = RestaurantContext.Ins.Categories.ToList();
            int totalItems = query.Count();
            TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            menu = query
                        .OrderByDescending(i => i.Id)
                        .Skip((pageIndex - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
        }


        public IActionResult OnPostAdd(IFormFile image)
        {
            try
            {
                categories = RestaurantContext.Ins.Categories.ToList();

                string name = Request.Form["name"];
                string description = Request.Form["description"];
                float price = float.Parse(Request.Form["price"]); 
                int cateId = int.Parse(Request.Form["cate"]);
                var existingMenu = RestaurantContext.Ins.Menus.FirstOrDefault(x => x.Name.Equals(name));
                if (existingMenu != null)
                {   
                    ViewData["error"] = "Name is exist";
                    return RedirectToPage("/Admin/menu/MenuManage");
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets","img");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = null;

                if (image != null && image.Length > 0)
                {   
                    var path= Path.GetExtension(image.FileName).ToLower();
                    if(path != ".png" || path != ".jpg")
                    {   
                        return RedirectToPage("/Admin/menu/MenuManage");
                    }
                    fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        image.CopyTo(stream); 
                    }
                }

                Menu mnu = new Menu
                {
                    Name = name,
                    Detail = description,
                    Price = price,
                    Img = fileName, 
                    IsSell = false,
                    CateId = cateId
                };

                RestaurantContext.Ins.Menus.Add(mnu);
                RestaurantContext.Ins.SaveChanges();

                return RedirectToPage("/Admin/menu/MenuManage");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while adding the menu item.");
                return Page();
            }
        }


        public IActionResult OnPostDelete()
        {
            string id = Request.Form["itemId"];
            var me = RestaurantContext.Ins.Menus.Find(int.Parse(id));
            if (me != null)
            {
                me.IsSell = false;
            }
            RestaurantContext.Ins.Menus.Update(me);
            RestaurantContext.Ins.SaveChanges();
            return RedirectToPage("/Admin/menu/MenuManage");
        }

        public IActionResult OnPostActive()
        {
            string id = Request.Form["itemId"];
            var me = RestaurantContext.Ins.Menus.Find(int.Parse(id));
            if (me != null)
            {
                me.IsSell = true;
            }
            RestaurantContext.Ins.Menus.Update(me);
            RestaurantContext.Ins.SaveChanges();
            return RedirectToPage("/Admin/menu/MenuManage");
        }

        public IActionResult OnPostUpdate()
        {
            string id = Request.Form["itemId"];
            string name = Request.Form["name"];
            string description = Request.Form["description"];
            string price = Request.Form["price"];
            string image = Request.Form["image"];
            string cate = Request.Form["cate"];
            var me = RestaurantContext.Ins.Menus.Find(int.Parse(id));
            if (me != null)
            {
                me.Name = name;
                me.Detail = description;
                me.Price = float.Parse(price);
                me.CateId = int.Parse(cate);
                me.Img = image;
                RestaurantContext.Ins.Menus.Update(me);
                RestaurantContext.Ins.SaveChanges();
            }
            return RedirectToPage("/Admin/menu/MenuManage");
        }

        //public IActionResult OnPostFilter()
        //{
        //    int cate = int.Parse(Request.Form["category"]);
        //    int minprice = int.Parse(Request.Form["minprice"]);
        //    int maxprice = int.Parse(Request.Form["maxprice"]);
        //    categories = RestaurantContext.Ins.Categories.ToList();
        //    if (cate == 0 )
        //    {
        //        menu = RestaurantContext.Ins.Menus.Where(x => minprice <= x.Price && x.Price <= maxprice).OrderByDescending(x=>x.Id).ToList();
        //    }
        //    else if (cate != 0)
        //    {
        //        menu = RestaurantContext.Ins.Menus.Where(x => x.CateId == cate && minprice <= x.Price && x.Price <= maxprice).ToList();
        //    }
        //    return Page();
        //}

        //public IActionResult OnPostSearch()
        //{
        //    string search = Request.Form["search"];
        //    categories = RestaurantContext.Ins.Categories.ToList();
        //    if (search == null)
        //    {
        //        menu = RestaurantContext.Ins.Menus.ToList();
        //    }
        //    else
        //    {
        //        menu = RestaurantContext.Ins.Menus.Where(x => x.Name.Contains(search)).ToList();
        //    }
        //    return Page();
        //}

        //public IActionResult OnGetDownloadExcel()
        //{
        //    var excelFileName = "Product.xlsx";
        //    var memoryStream = new MemoryStream();

        //    using (var package = new ExcelPackage(memoryStream))
        //    {
        //        var worksheet = package.Workbook.Worksheets.Add("Sheet1");

        //        worksheet.Cells[1, 1].Value = "Name";
        //        worksheet.Cells[1, 2].Value = "Detail";
        //        worksheet.Cells[1, 3].Value = "Price";
        //        worksheet.Cells[1, 4].Value = "Image";
        //        worksheet.Cells[1, 5].Value = "Category";

        //        package.Save();
        //    }

        //    memoryStream.Position = 0;

        //    return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
        //}

        public IActionResult OnGetDownloadExcel()
        {
            var excelFileName = "Product.xlsx";
            var memoryStream = new MemoryStream();

            using (var package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");

                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Detail";
                worksheet.Cells[1, 3].Value = "Price";
                worksheet.Cells[1, 4].Value = "Image";
                worksheet.Cells[1, 5].Value = "Category";


                var categories = RestaurantContext.Ins.Categories.ToList();
                for (int i = 0; i < categories.Count(); i++)
                {
                    worksheet.Cells[i + 1, 7].Value = categories[i].Name; 
                }
                int x = categories.Count();
                var categoryRange = worksheet.Cells["G1:G"+x];
                worksheet.Names.Add("CategoryList", categoryRange);

                var validation = worksheet.DataValidations.AddListValidation("E2:E100"); 
                validation.Formula.ExcelFormula = "CategoryList";
                validation.ShowErrorMessage = true;
                validation.ErrorTitle = "Invalid Category";
                validation.Error = "Please select a category from the list.";

                package.Save();
            }

            memoryStream.Position = 0;

            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelFileName);
        }

        public IActionResult OnPostUploadExcel(IFormFile excel)
        {
            if (excel == null || excel.Length == 0)
            {
                ModelState.AddModelError("ExcelFile", "Please upload a valid Excel file.");
                return Page();
            }

            using (var stream = new MemoryStream())
            {
                excel.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var Name = worksheet.Cells[row, 1].Value?.ToString();
                        var detail = worksheet.Cells[row, 2].Value?.ToString();

                        var price = worksheet.Cells[row, 3].Value != null ? Convert.ToDouble(worksheet.Cells[row, 3].Value) : (double?)null;
                        var img = worksheet.Cells[row, 4].Value?.ToString();
                        var cate = worksheet.Cells[row, 5].Value?.ToString();

                        if (string.IsNullOrWhiteSpace(Name) || price == null)
                        {
                            continue; 
                        }

                        var category = RestaurantContext.Ins.Categories.FirstOrDefault(c => c.Name == cate);
                        if (category == null)
                        {   
                            using(var range = worksheet.Cells[row+1,1,row+1,4])
                            {
                                range.Style.Font.Color.SetColor(Color.Red);
                                range.Style.Font.Bold = true;

                            }
                            continue; 
                        }

                        var m = RestaurantContext.Ins.Menus.Where(x => x.Name.Equals(Name)).FirstOrDefault();
                        if(m != null)
                        {
                            using (var range = worksheet.Cells[row + 1, 1, row + 1, 4])
                            {
                                range.Style.Font.Color.SetColor(Color.Red);
                                range.Style.Font.Bold = true;

                            }
                            continue;
                        }

                        var product = new Menu
                        {
                            Name = Name,
                            Detail = detail,
                            Price = (float)price,
                            Img = img,
                            CateId = category.Id,
                            IsSell = false
                        };

                        RestaurantContext.Ins.Menus.Add(product);
                    }
                    RestaurantContext.Ins.SaveChanges();
                }
            }
            TempData["Message"] = "Products added to the database successfully.";
            return RedirectToPage("/Admin/menu/MenuManage");
        }

    }

}
