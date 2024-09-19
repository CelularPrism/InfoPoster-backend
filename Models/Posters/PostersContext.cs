﻿using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Models.Posters
{
    public class PostersContext : DbContext
    {
        public PostersContext(DbContextOptions<PostersContext> options) : base(options)
        {
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                var exc = ex;
            }
        }

        public DbSet<PosterModel> Posters { get; set; }
        public DbSet<PosterMultilangModel> PostersMultilang { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<CategoryMultilangModel> CategoriesMultilang { get; set; }
        public DbSet<SubcategoryModel> Subcategories { get; set; }
        public DbSet<SubcategoryMultilangModel> SubcategoriesMultilang { get; set; }
        public DbSet<PosterSubcategoryModel> PosterSubcategory { get; set; }
    }
}
