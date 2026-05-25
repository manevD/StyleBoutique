using BoutiqueQuantity.Data;
using BoutiqueQuantity.Entities;
using BoutiqueQuantity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BoutiqueQuantity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context) => _context = context;
       
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Inventories)
                .ToListAsync();
            return View(products);
        }
        // =========================
        // DELETE GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product =
            await _context.Products
            .Include(x => x.Category)
            .Include(x => x.Variants)
            .ThenInclude(x => x.Inventories)
            .FirstOrDefaultAsync(x =>
            x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }



        // =========================
        // DELETE POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product =
            await _context.Products
            .Include(x => x.Variants)
            .ThenInclude(x => x.Inventories)
            .FirstOrDefaultAsync(x =>
            x.Id == id);

            if (product == null)
            {
                return NotFound();
            }


            // =========================
            // CHECK SALES
            // =========================

            var hasSales =
            await _context.Sales
            .AnyAsync(x =>
            x.ProductId == product.Id);

            if (hasSales)
            {
                TempData["Error"] =
                "Производот има продажби и не може да се избрише.";

                return RedirectToAction(
                    "Details",
                    new
                    {
                        id = product.Id
                    });
            }


            // =========================
            // REMOVE INVENTORIES
            // =========================

            var inventories =
            product.Variants
            .SelectMany(x =>
            x.Inventories)
            .ToList();

            _context.Inventories
            .RemoveRange(inventories);


            // =========================
            // REMOVE VARIANTS
            // =========================

            _context.ProductVariants
            .RemoveRange(product.Variants);


            // =========================
            // REMOVE PRODUCT
            // =========================

            _context.Products
            .Remove(product);


            await _context
            .SaveChangesAsync();


            TempData["Success"] =
            "Производот е успешно избришан.";


            return RedirectToAction(
                "Index");
        }
        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Inventories)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        public async Task<IActionResult> Create()
        {
            var vm =
            new ProductEditViewModel
            {
                Categories =
                await _context.Categories
                .Select(x =>
                new SelectListItem
                {
                    Value =
                    x.Id.ToString(),

                    Text =
                    x.Name
                })
                .ToListAsync(),

                Variants =
                new List<VariantEditViewModel>
                {
            new VariantEditViewModel
            {
                Inventories =
                new List<InventoryEditViewModel>
                {
                    new InventoryEditViewModel()
                }
            }
                }
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductEditViewModel model)
        {
            model.Variants ??=
            new List<VariantEditViewModel>();


            var product =
            new Product
            {
                ProductCode =
                model.ProductCode,

                Name =
                model.Name,

                CategoryId =
                model.CategoryId,

                PurchasePrice =
                model.PurchasePrice,

                SalePrice =
                model.SalePrice,

                Variants =
                new List<ProductVariant>()
            };


            // =========================
            // VARIANTS
            // =========================

            foreach (var vModel in
            model.Variants.ToList())
            {
                // EMPTY COLOR
                if (string.IsNullOrWhiteSpace(vModel.Color))
                {
                    continue;
                }


                var variant =
                new ProductVariant
                {
                    Color =
                    vModel.Color,

                    Barcode =
                    Guid.NewGuid()
                    .ToString(),

                    Inventories =
                    new List<Inventory>()
                };


                vModel.Inventories ??=
                new List<InventoryEditViewModel>();


                // =========================
                // INVENTORIES
                // =========================

                foreach (var iModel in
                vModel.Inventories.ToList())
                {
                    // DELETE
                    if (iModel.Delete)
                    {
                        continue;
                    }

                    // EMPTY SIZE
                    if (string.IsNullOrWhiteSpace(iModel.Size))
                    {
                        continue;
                    }

                    variant.Inventories
                    .Add(
                    new Inventory
                    {
                        Size =
                        iModel.Size,

                        Quantity =
                        iModel.Quantity
                    });
                }


                product.Variants
                .Add(variant);
            }


            _context.Products
            .Add(product);

            await _context
            .SaveChangesAsync();


            return RedirectToAction(
                "Details",
                new
                {
                    id = product.Id
                });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Inventories)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            var vm = new ProductEditViewModel
            {
                Id = product.Id,
                Name = product.Name,
                ProductCode = product.ProductCode,
                CategoryId = product.CategoryId,
                PurchasePrice = product.PurchasePrice,
                SalePrice = product.SalePrice,
                Categories = await _context.Categories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToListAsync(),
                Variants = product.Variants.Select(v => new VariantEditViewModel
                {
                    Id = v.Id,
                    Color = v.Color,
                    Inventories = v.Inventories.Select(i => new InventoryEditViewModel
                    {
                        Id = i.Id,
                        Size = i.Size,
                        Quantity = i.Quantity,
                        Delete = false
                    }).ToList()
                }).ToList()
            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel model)
        {
            var product =
            await _context.Products
            .Include(x => x.Variants)
            .ThenInclude(x => x.Inventories)
            .FirstOrDefaultAsync(
                x => x.Id == model.Id);

            if (product == null)
            {
                return NotFound();
            }

            // =========================
            // PRODUCT
            // =========================

            product.Name =
            model.Name;

            product.ProductCode =
            model.ProductCode;

            product.CategoryId = model.CategoryId;

            product.PurchasePrice = model.PurchasePrice;

            product.SalePrice = model.SalePrice;

            model.Variants ??= new List<VariantEditViewModel>();

            // =========================
            // VARIANTS
            // =========================

            foreach (var vModel in model.Variants.ToList())
            {
                // =========================
                // DELETE VARIANT
                // =========================

                if (string.IsNullOrWhiteSpace(vModel.Color))
                {
                    var variantToDelete =
                    product.Variants
                    .FirstOrDefault(
                        x => x.Id == vModel.Id);

                    if (variantToDelete != null)
                    {
                        _context.Inventories
                        .RemoveRange(
                            variantToDelete
                            .Inventories
                            .ToList());

                        _context.ProductVariants
                        .Remove(
                            variantToDelete);
                    }

                    continue;
                }


                // =========================
                // NEW VARIANT
                // =========================

                if (vModel.Id == 0)
                {
                    var newVariant =
                    new ProductVariant
                    {
                        ProductId =
                        product.Id,

                        Color =
                        vModel.Color,

                        Barcode =
                        Guid.NewGuid()
                        .ToString(),

                        Inventories =
                        new List<Inventory>()
                    };

                    foreach (var iModel in
                    vModel.Inventories
                    ?? new List<InventoryEditViewModel>())
                    {
                        if (iModel.Delete)
                        {
                            continue;
                        }

                        newVariant.Inventories
                        .Add(
                        new Inventory
                        {
                            Size =
                            iModel.Size,

                            Quantity =
                            iModel.Quantity
                        });
                    }

                    _context.ProductVariants
                    .Add(
                        newVariant);

                    continue;
                }


                // =========================
                // EXISTING VARIANT
                // =========================

                var variant =
                product.Variants
                .FirstOrDefault(
                    x => x.Id == vModel.Id);

                if (variant == null)
                {
                    continue;
                }

                variant.Color =
                vModel.Color;


                vModel.Inventories ??=
                new List<InventoryEditViewModel>();


                foreach (var iModel in
                vModel.Inventories.ToList())
                {
                    // =========================
                    // DELETE INVENTORY
                    // =========================

                    if (iModel.Delete)
                    {
                        var inventoryToDelete =
                        variant.Inventories
                        .FirstOrDefault(
                            x => x.Id == iModel.Id);

                        if (inventoryToDelete != null)
                        {
                            _context.Inventories
                            .Remove(
                                inventoryToDelete);
                        }

                        continue;
                    }


                    // =========================
                    // NEW INVENTORY
                    // =========================

                    if (iModel.Id == 0)
                    {
                        variant.Inventories
                        .Add(
                        new Inventory
                        {
                            Size =
                            iModel.Size,

                            Quantity =
                            iModel.Quantity
                        });

                        continue;
                    }


                    // =========================
                    // UPDATE INVENTORY
                    // =========================

                    var inventory =
                    variant.Inventories
                    .FirstOrDefault(
                        x => x.Id == iModel.Id);

                    if (inventory != null)
                    {
                        inventory.Size =
                        iModel.Size;

                        inventory.Quantity =
                        iModel.Quantity;
                    }
                }
            }


            // =========================
            // REMOVE VARIANTS
            // =========================

            var variantIdsInModel =
            model.Variants
            .Where(x => x.Id != 0)
            .Select(x => x.Id)
            .ToList();

            var variantsToRemove =
            product.Variants
            .Where(x =>
            !variantIdsInModel
            .Contains(x.Id) && x.Id != 0)
            .ToList();

            foreach (var variant in variantsToRemove.ToList())
            {
                _context.Inventories.RemoveRange(variant.Inventories.ToList());

                _context.ProductVariants.Remove(variant);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = product.Id });
        }
    }
}