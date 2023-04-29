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
using AutoMapper;
using MeowoofStore.Models.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace MeowoofStore.Controllers
{
    [Authorize(Roles =nameof(RoleName.Administrator))]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _lMapper;
        private readonly PhotoProcess _photoProcess;          //需在Program.cs 註冊 PhotoProcess 變成服務才能注入

        public ProductsController(ApplicationDbContext context,IMapper mapper, PhotoProcess photoProcess)
        {
            _context = context;
            _lMapper = mapper;
            _photoProcess = photoProcess;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View();
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
                return View(ViewName.EmptyProduct);

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
                return View(ViewName.EmptyProduct);

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
                await _photoProcess.CreatePhoto<ProductViewModel>(viewModel, FolderPath._Images_ProductImages,
                nameof(viewModel.Photo), nameof(viewModel.ImageString));
            }

            var product =_lMapper.Map<Product>(viewModel);
            _context.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
                return View(ViewName.EmptyProduct);

            var product = await _context.Product.FindAsync(id);
            if (product == null)
                return View(ViewName.EmptyProduct);

            var viewModel = _lMapper.Map<ProductViewModel>(product);

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
                return View(ViewName.EmptyProduct);

            if (!ModelState.IsValid)
                return View(viewModel);

            Product? product = _context.Product.FirstOrDefault(n => n.Id == viewModel.Id);
            if (product == null)
                return View(ViewName.EmptyProduct);

            if (viewModel.Photo != null&& viewModel.ImageString!=null)
                await _photoProcess.DeletePhoto(FolderPath._Images_ProductImages, viewModel.ImageString);

            if (viewModel.Photo != null)
            {
                await _photoProcess.CreatePhoto<ProductViewModel>(viewModel, FolderPath._Images_ProductImages, 
                nameof(viewModel.Photo), nameof(viewModel.ImageString));
            }
            var productMapper = _lMapper.Map(viewModel,product);

            _context.Update(productMapper);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id, string imageString)
        {
            if (id == null || _context.Product == null)
                return View(ViewName.EmptyProduct);

            var product = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
                return View(ViewName.EmptyProduct);

            if(!String.IsNullOrEmpty(imageString))
                await _photoProcess.DeletePhoto(FolderPath._Images_ProductImages, imageString);

            _context.Product.Remove(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
