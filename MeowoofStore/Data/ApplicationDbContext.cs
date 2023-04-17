﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MeowoofStore.Models;

namespace MeowoofStore.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MeowoofStore.Models.Product>? Product { get; set; }
        public DbSet<MeowoofStore.Models.ShoppingCartItem>? ShoppingCartItem { get; set; }
    }
}