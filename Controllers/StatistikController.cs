using BoutiqueQuantity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueQuantity.Controllers
{
    public class StatistikController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatistikController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================
        // GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> Index(
            DateTime? from,
            DateTime? to)
        {
            var sales =
            _context.Sales
            .AsQueryable();

            // =========================
            // STOCK VALUE
            // =========================

            var inventories =
            await _context.Inventories
            .Include(x => x.ProductVariant)
            .ThenInclude(x => x.Product)
            .ToListAsync();


            ViewBag.TotalPurchaseStockValue =
            inventories.Sum(x =>

                x.Quantity *

                x.ProductVariant
                .Product
                .PurchasePrice);


            ViewBag.TotalSaleStockValue =
            inventories.Sum(x =>

                x.Quantity *

                x.ProductVariant
                .Product
                .SalePrice);
            // =========================
            // FILTER
            // =========================

            if (from.HasValue)
            {
                sales =
                sales.Where(x =>
                x.CreatedAt.Date >=
                from.Value.Date);
            }

            if (to.HasValue)
            {
                sales =
                sales.Where(x =>
                x.CreatedAt.Date <=
                to.Value.Date);
            }


            var data =
            await sales
            .OrderByDescending(x =>
            x.CreatedAt)
            .ToListAsync();


            // =========================
            // DATES
            // =========================

            var today =
            DateTime.UtcNow.Date;

            var week =
            today.AddDays(-7);

            var month =
            new DateTime(
                today.Year,
                today.Month,
                1);


            // =========================
            // TODAY
            // =========================

            ViewBag.TodayRevenue =
            data.Where(x =>
            x.CreatedAt.Date == today)
            .Sum(x =>
            x.RealTotal);

            ViewBag.TodayProfit =
            data.Where(x =>
            x.CreatedAt.Date == today)
            .Sum(x =>
            x.RealProfit);

            ViewBag.TodayCount =
            data.Where(x =>
            x.CreatedAt.Date == today)
            .Sum(x =>
            x.RealQuantity);


            // =========================
            // WEEK
            // =========================

            ViewBag.WeekRevenue =
            data.Where(x =>
            x.CreatedAt >= week)
            .Sum(x =>
            x.RealTotal);

            ViewBag.WeekProfit =
            data.Where(x =>
            x.CreatedAt >= week)
            .Sum(x =>
            x.RealProfit);

            ViewBag.WeekCount =
            data.Where(x =>
            x.CreatedAt >= week)
            .Sum(x =>
            x.RealQuantity);


            // =========================
            // MONTH
            // =========================

            ViewBag.MonthRevenue =
            data.Where(x =>
            x.CreatedAt >= month)
            .Sum(x =>
            x.RealTotal);

            ViewBag.MonthProfit =
            data.Where(x =>
            x.CreatedAt >= month)
            .Sum(x =>
            x.RealProfit);

            ViewBag.MonthCount =
            data.Where(x =>
            x.CreatedAt >= month)
            .Sum(x =>
            x.RealQuantity);


            // =========================
            // TOTAL
            // =========================

            ViewBag.TotalRevenue =
            data.Sum(x =>
            x.RealTotal);

            ViewBag.TotalProfit =
            data.Sum(x =>
            x.RealProfit);

            ViewBag.TotalSales =
            data.Sum(x =>
            x.RealQuantity);

            ViewBag.TotalReturns =
            data.Sum(x =>
            x.ReturnedQuantity);


            // =========================
            // TOP PRODUCTS
            // =========================

            ViewBag.TopProducts =
            data.GroupBy(x =>
            x.ProductName)
            .Select(x =>
            new
            {
                Name =
                x.Key,

                Quantity =
                x.Sum(y =>
                y.RealQuantity),

                Revenue =
                x.Sum(y =>
                y.RealTotal),

                Profit =
                x.Sum(y =>
                y.RealProfit)
            })
            .OrderByDescending(x =>
            x.Quantity)
            .Take(10)
            .ToList();


            // =========================
            // TOP CATEGORIES
            // =========================

            ViewBag.TopCategories =
            data.GroupBy(x =>
            x.CategoryName)
            .Select(x =>
            new
            {
                Name =
                x.Key,

                Quantity =
                x.Sum(y =>
                y.RealQuantity),

                Revenue =
                x.Sum(y =>
                y.RealTotal),

                Profit =
                x.Sum(y =>
                y.RealProfit)
            })
            .OrderByDescending(x =>
            x.Quantity)
            .Take(10)
            .ToList();


            // =========================
            // LAST SALES
            // =========================

            ViewBag.LastSales =
            data.Take(20)
            .ToList();


            return View();
        }


        // =========================
        // POST FILTER
        // =========================

        [HttpPost]
        public IActionResult Index(
            DateTime? from,
            DateTime? to,
            string? filter)
        {
            return RedirectToAction(
                "Index",
                new
                {
                    from,
                    to
                });
        }


        // =========================
        // RETURNS
        // =========================

        // =========================
        // RETURNS GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> Returns(
            string? search,
            DateTime? from,
            DateTime? to)
        {
            var returns =_context.Sales.Where(x =>x.ReturnedQuantity > 0).AsQueryable().Take(100);

            // SEARCH

            if (!string.IsNullOrWhiteSpace(search))
            {
                search =
                search.ToLower();

                returns =
                returns.Where(x =>

                    x.ProductName
                    .ToLower()
                    .Contains(search)

                    ||

                    x.ProductCode
                    .ToLower()
                    .Contains(search)

                    ||

                    x.Color
                    .ToLower()
                    .Contains(search)

                    ||

                    x.Size
                    .ToLower()
                    .Contains(search));
            }


            // FROM

            if (from.HasValue)
            {
                returns =
                returns.Where(x =>
                x.ReturnedAt >=
                from.Value.Date);
            }


            // TO

            if (to.HasValue)
            {
                var endDate =
                to.Value.Date
                .AddDays(1);

                returns =
                returns.Where(x =>
                x.ReturnedAt < endDate);
            }


            var data =
            await returns
            .OrderByDescending(x =>
            x.ReturnedAt)
            .ToListAsync();


            return View(data);
        }



        // =========================
        // RETURNS POST FILTER
        // =========================

        [HttpPost]
        public IActionResult Returns(string? search, DateTime? from,DateTime? to,string? filter)
        {
            return RedirectToAction(
                "Returns",
                new
                {
                    search,
                    from,
                    to
                });
        }
    }
}