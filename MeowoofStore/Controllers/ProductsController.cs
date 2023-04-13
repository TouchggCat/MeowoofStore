using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MeowoofStore.Data;
using MeowoofStore.Models;
using MeowoofStore.ViewModels;
using Microsoft.CodeAnalysis;
using MeowoofStore.Models.StringKeys;

namespace MeowoofStore.Controllers
{
    public class ProductsController : Controller
    {
        private IWebHostEnvironment _environment; //取路徑用
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            //return _context.Product != null ? 
            //            View(await _context.Product.ToListAsync()) :
            //            Problem("Entity set 'ApplicationDbContext.Product'  is null.");  //Razor View
            return View();
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
                return View("NullView");

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
                return View("NullView");

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Stock,Photo")] ProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel); //看驗證訊息
            
            if(viewModel.Photo!=null)
            {
                SavePhoto(viewModel, FolderPath._Images_ProductImages);
            }
            Product product = new Product
            {
                Name = viewModel.Name,
                Stock = viewModel.Stock,
                Price = viewModel.Price,
                Description = viewModel.Description,
                ImageString = viewModel.ImageString
            };
            _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }



        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
                return View("NullView");

            var product = await _context.Product.FindAsync(id);
            if (product == null)
                return View("NullView");

            ProductViewModel viewModel = new ProductViewModel()
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                ImageString = product.ImageString
            };
            return View(viewModel);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel viewModel)
        {
            if (id != viewModel.Id)
                return View("NullView");

            if (!ModelState.IsValid)
                return View(viewModel);

            Product? product = _context.Product.FirstOrDefault(n => n.Id == viewModel.Id);
            if (product == null)
                return View("NullView");

            product.Description = viewModel.Description;
            product.Price = viewModel.Price;
            product.Name = viewModel.Name;
            product.Stock = viewModel.Stock;
            product.ImageString = viewModel.ImageString;
            if(viewModel.Photo != null&& viewModel.ImageString!=null)
                DeletePhoto(FolderPath._Images_ProductImages, viewModel.ImageString);
            if (viewModel.Photo != null)
            {
                SavePhoto(viewModel, FolderPath._Images_ProductImages);
                product.ImageString = viewModel.ImageString;
            }
            _context.Update(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id, string imageString)
        {
            if (id == null || _context.Product == null)
                return View("NullView");

            var product = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
                return View("NullView");

            if(!String.IsNullOrEmpty(imageString))
                DeletePhoto(FolderPath._Images_ProductImages, imageString);
            _context.Product.Remove(product);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private void SavePhoto(ProductViewModel viewModel, string folderPath)
        {
            string photoName = Guid.NewGuid().ToString() + ".jpg";
            viewModel.ImageString = photoName;
            //抓路徑 IWebHostEnvironment(見下方)
            viewModel.Photo.CopyTo(new FileStream(_environment.WebRootPath + folderPath + photoName, FileMode.Create));
        }
        private void DeletePhoto(string folderPath, string photoName)
        {
            string filePath = Path.Combine(_environment.WebRootPath+ folderPath, photoName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
