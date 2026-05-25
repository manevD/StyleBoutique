using BoutiqueQuantity.Data;
using BoutiqueQuantity.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueQuantity.Controllers
{
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SaleController(
            ApplicationDbContext context)
        {
            _context = context;
        }


        // =========================
        // SALE GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> Sale(int id)
        {
            var product = await _context.Products
                .Include(x => x.Category)
                .Include(x => x.Variants)
                .ThenInclude(x => x.Inventories)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // =========================
        // SALE POST
        // =========================

        [HttpPost]
        public async Task<IActionResult> Sale(
            int inventoryId,
            int quantity,
            decimal price,
            string? paymentMethod,
            string? customerName,
            string? phoneNumber,
            string? note)
        {
            var inventory = await _context.Inventories
                .Include(x => x.ProductVariant)
                .ThenInclude(x => x.Product)
                .ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == inventoryId);

            if (inventory == null)
            {
                return NotFound();
            }

            if (quantity <= 0)
            {
                TempData["Error"] =
                "Количината мора да биде поголема од 0.";

                return RedirectToAction(
                    "Sale",
                    new
                    {
                        id =
                        inventory.ProductVariant.ProductId
                    });
            }

            if (inventory.Quantity < quantity)
            {
                TempData["Error"] =
                "Нема доволно залиха.";

                return RedirectToAction(
                    "Sale",
                    new
                    {
                        id =
                        inventory.ProductVariant.ProductId
                    });
            }

            var product =
            inventory.ProductVariant.Product;


            var sale =
            new Sale
            {
                ProductId =
                product.Id,

                ProductVariantId =
                inventory.ProductVariantId,

                InventoryId =
                inventory.Id,

                ProductName =
                product.Name,

                ProductCode =
                product.ProductCode,

                CategoryName =
                product.Category.Name,

                Color =
                inventory.ProductVariant.Color,

                Size =
                inventory.Size,

                Quantity =
                quantity,

                ReturnedQuantity =
                0,

                PurchasePrice =
                product.PurchasePrice,

                SalePrice =
                price,

                Total =
                price * quantity,

                Profit =
                (price -
                product.PurchasePrice)
                * quantity,

                SoldByUserId =
                User?.Identity?.Name,

                CustomerName =
                customerName,

                PhoneNumber =
                phoneNumber,

                PaymentMethod =
                paymentMethod,

                Note =
                note,

                CreatedAt =
                DateTime.UtcNow
            };


            var strategy =
            _context.Database
            .CreateExecutionStrategy();


            await strategy.ExecuteAsync(
            async () =>
            {
                await using var transaction =
                await _context.Database
                .BeginTransactionAsync();

                try
                {
                    inventory.Quantity -=
                    quantity;

                    _context.Sales
                    .Add(sale);

                    await _context
                    .SaveChangesAsync();

                    await transaction
                    .CommitAsync();
                }
                catch
                {
                    await transaction
                    .RollbackAsync();

                    throw;
                }
            });


            TempData["Success"] =
            "Продажбата е успешна.";


            return RedirectToAction(
                "Index");
        }


        // =========================
        // SALES HISTORY
        // =========================

        [HttpGet]
        public async Task<IActionResult> Index(
            string? search,
            DateTime? from,
            DateTime? to)
        {
            var sales =
            _context.Sales
            .AsQueryable();


            // SEARCH

            if (!string.IsNullOrWhiteSpace(search))
            {
                search =
                search.ToLower();

                sales =
                sales.Where(x =>

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
                sales =
                sales.Where(x =>
                x.CreatedAt.Date >=
                from.Value.Date);
            }


            // TO

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


            return View(data);
        }


        // =========================
        // DETAILS
        // =========================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var sale =
            await _context.Sales
            .FirstOrDefaultAsync(x =>
            x.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }


        // =========================
        // RETURN GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> Return(int id)
        {
            var sale =
            await _context.Sales
            .FirstOrDefaultAsync(x =>
            x.Id == id);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }


        // =========================
        // RETURN POST
        // =========================

        [HttpPost]
        public async Task<IActionResult> Return(
            int saleId,
            int quantity,
            string? reason)
        {
            var sale =
            await _context.Sales
            .Include(x => x.Inventory)
            .FirstOrDefaultAsync(x =>
            x.Id == saleId);

            if (sale == null)
            {
                return NotFound();
            }

            if (sale.Inventory == null)
            {
                TempData["Error"] =
                "Inventory не постои.";

                return RedirectToAction(
                    "Index");
            }

            var availableToReturn =
            sale.RealQuantity;

            if (quantity <= 0)
            {
                TempData["Error"] =
                "Невалидна количина.";

                return RedirectToAction(
                    "Return",
                    new
                    {
                        id =
                        sale.Id
                    });
            }

            if (quantity > availableToReturn)
            {
                TempData["Error"] =
                "Немате толку за враќање.";

                return RedirectToAction(
                    "Return",
                    new
                    {
                        id =
                        sale.Id
                    });
            }


            var strategy =
            _context.Database
            .CreateExecutionStrategy();


            await strategy.ExecuteAsync(
            async () =>
            {
                await using var transaction =
                await _context.Database
                .BeginTransactionAsync();

                try
                {
                    // STOCK BACK

                    sale.Inventory.Quantity +=
                    quantity;


                    // RETURN UPDATE

                    sale.ReturnedQuantity +=
                    quantity;

                    sale.ReturnedAt =
                    DateTime.UtcNow;

                    sale.ReturnReason =
                    reason;


                    await _context
                    .SaveChangesAsync();

                    await transaction
                    .CommitAsync();
                }
                catch
                {
                    await transaction
                    .RollbackAsync();

                    throw;
                }
            });


            TempData["Success"] =
            "Повратот е успешен.";


            return RedirectToAction(
                "Index");
        }
    }
}