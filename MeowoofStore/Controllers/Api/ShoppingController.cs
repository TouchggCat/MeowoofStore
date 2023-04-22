﻿using MeowoofStore.Data;
using MeowoofStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MeowoofStore.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private ApplicationDbContext _context { get; set; }
        public ShoppingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetShoppingList()
        {
            var products = await _context.Product.ToListAsync();

            if (products == null)
                return NotFound("No product !");

            return products;
        }
    }
}
