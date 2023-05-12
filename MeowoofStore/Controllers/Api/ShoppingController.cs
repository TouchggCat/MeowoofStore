using AutoMapper;
using Azure.Core;
using MeowoofStore.Data;
using MeowoofStore.Dtos;
using MeowoofStore.Models;
using MeowoofStore.Models.StringKeys;
using MeowoofStore.Models.Utilities;
using MeowoofStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;

namespace MeowoofStore.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private ApplicationDbContext _context { get; set; }
        private readonly IMapper _lMapper;
        public ShoppingController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _lMapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetShoppingList()
        {
            var products = await _context.Product.ToListAsync();

            if (products == null)
                return NotFound("No product !");

            return products;
        }

        [HttpGet("/api/check-stock")]
        public ActionResult CheckStock(int itemId, int quantity)
        {
            var item = _context.Product.Find(itemId);
            if (item == null)
                return NotFound();

            if (item.Stock >= quantity)
                return Ok(new { IsAvailable = true });
 
            return Ok(new { IsAvailable = false });
        }

        [HttpGet("/api/Shopping/LoadCartItem/{id}")]
        public ActionResult LoadCartItem(int id)
        {
            int defaultQuantity = 1;
            var product = _context.Product?.SingleOrDefault(n => n.Id == id);
            if (product != null)
            {
                var dto = _lMapper.Map<AddToCartDto>(product);
                dto.Quantity = defaultQuantity;

                return Ok(dto);
            }

            return NotFound("沒有此產品");
        }

        [HttpPost("/api/Shopping/AddToCart/")]
        public ActionResult AddToCart(AddToCartDto postedDto)
        {
            var product = _context.Product?.SingleOrDefault(n => n.Id == postedDto.id);
            if (product == null)
                return NotFound();

            List<Dtos.ShoppingCartDto>? shoppingCartDtoList = GetShoppingCartListFromSessionOrNewCart();

            CheckSameItemAlreadyExistInCart(postedDto, product, shoppingCartDtoList);

            SaveShoppingCartListToSession(shoppingCartDtoList);

            return Ok("SuccessAddToCart");
        }

        

        [HttpGet("/api/Shopping/GetCartData")]
        public ActionResult<IEnumerable<ShoppingCartDto>> GetCartData()
        {
            if (HttpContext.Session.Keys.Contains(ShoppingCartSessionKey.ShoppingCartListKey))
            {
                List<ShoppingCartDto>? shoppingCartDtoList = GetShoppingCartItemsFromSession();
                return shoppingCartDtoList;
            }

            return BadRequest();
        }

        [HttpDelete("/api/Shopping/DeleteProductsById/{id}")] 
        public async Task<ActionResult> DeleteProductsById(int id)
        {
            var products = await _context.Product.SingleOrDefaultAsync(m => m.Id == id);

            if (products == null)
                return NotFound();

            List<ShoppingCartDto>? shoppingCartDtoList = GetShoppingCartItemsFromSession();
            shoppingCartDtoList.RemoveAll(p => p.Product.Id == id);

            if (shoppingCartDtoList.Count == 0)
                RemoveSession(ShoppingCartSessionKey.ShoppingCartListKey);
            else
            SaveShoppingCartListToSession(shoppingCartDtoList);

            return NoContent();
        }

        private void SaveShoppingCartListToSession(List<ShoppingCartDto> shoppingCartDtoList)
        {
            var serializedJsonString = JsonSerializer.Serialize(shoppingCartDtoList);
            HttpContext.Session.SetString(ShoppingCartSessionKey.ShoppingCartListKey, serializedJsonString);
        }

        private List<ShoppingCartDto>? GetShoppingCartItemsFromSession()
        {
            string? jsonString = HttpContext.Session.GetString(ShoppingCartSessionKey.ShoppingCartListKey);
            List<ShoppingCartDto>? shoppingCartDtoList = JsonSerializer.Deserialize<List<ShoppingCartDto>>(jsonString);
            return shoppingCartDtoList;
        }

        private List<ShoppingCartDto> GetShoppingCartListFromSessionOrNewCart()
        {
            if (!HttpContext.Session.Keys.Contains(ShoppingCartSessionKey.ShoppingCartListKey))
                return new List<ShoppingCartDto>();

            var jsonString = HttpContext.Session.GetString(ShoppingCartSessionKey.ShoppingCartListKey);
            return JsonSerializer.Deserialize<List<ShoppingCartDto>>(jsonString);
        }

        private ShoppingCartDto MappingToShoppingCartDto(Product product, AddToCartDto addToCartDto)
        {
            var shoppingCartDto = _lMapper.Map<ShoppingCartDto>(addToCartDto);
            shoppingCartDto.Product = product;
            return shoppingCartDto;
        }

        private void CheckSameItemAlreadyExistInCart(AddToCartDto postedDto, Product? product, List<ShoppingCartDto> shoppingCartDtoList)
        {
            var shoppingCartDto = shoppingCartDtoList.FirstOrDefault(n => n.Id == postedDto.id);

            if (shoppingCartDto == null)
            {
                shoppingCartDto = MappingToShoppingCartDto(product, postedDto);

                shoppingCartDtoList.Add(shoppingCartDto);
            }
            else    //已有相同商品，增加數量
            {
                shoppingCartDto.Quantity += postedDto.Quantity;
            }
        }

        private void RemoveSession(string SessionKey)
        {
            if (HttpContext.Session.Keys.Contains(SessionKey))
                HttpContext.Session.Remove(SessionKey);
        }
    }
}
