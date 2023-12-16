using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            
        }
        //this generates a table called Villas
        public DbSet<Villa> Villas { get; set; }
        public DbSet<VillaNumber> VillaNumbers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa()
                {
                    Id = 1,
                    Name = "Royal Villa",
                    Details = "lorem ipsum",
                    ImageUrl = "https://picsum.photos/200/300",
                    Occupancy = 5,
                    Rate = 200,
                    SqFt = 5,
                    Amenity = "",
                    CreatedDate = DateTime.Now
                },
                new Villa()
                {
                    Id = 2,
                    Name = "Trash Villa",
                    Details = "lorem ipsum",
                    ImageUrl = "https://picsum.photos/200/300",
                    Occupancy = 5,
                    Rate = 200,
                    SqFt = 5,
                    Amenity = "",
                    CreatedDate = DateTime.Now
                },
                new Villa()
                {
                    Id = 3,
                    Name = "Good Villa",
                    Details = "lorem ipsum",
                    ImageUrl = "https://picsum.photos/200/300",
                    Occupancy = 5,
                    Rate = 200,
                    SqFt = 5,
                    Amenity = "",
                    CreatedDate = DateTime.Now
                },
                new Villa()
                {
                    Id = 4,
                    Name = "Ok Villa",
                    Details = "lorem ipsum",
                    ImageUrl = "https://picsum.photos/200/300",
                    Occupancy = 5,
                    Rate = 200,
                    SqFt = 5,
                    Amenity = "",
                    CreatedDate = DateTime.Now
                },
                new Villa()
                {
                    Id = 5,
                    Name = "Normal Villa",
                    Details = "lorem ipsum",
                    ImageUrl = "https://picsum.photos/200/300",
                    Occupancy = 5,
                    Rate = 200,
                    SqFt = 5,
                    Amenity = "",
                    CreatedDate = DateTime.Now
                }
                );
        }
    }
}
