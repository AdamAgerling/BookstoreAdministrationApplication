using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace BookstoreAdmin.Model
{
    public class BookstoreDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<InventoryBalance> InventoryBalances { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<BookLanguage> BookLanguages { get; set; }
        public DbSet<PurchaseHistory> PurchaseHistories { get; set; }
        public DbSet<Image> Images { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Language)
                .WithMany()
                .HasForeignKey(b => b.LanguageId);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Image)
                .WithOne()
                .HasForeignKey<Book>(b => b.ImageId);

            modelBuilder.Entity<InventoryBalance>()
               .HasOne(ib => ib.Book)
               .WithMany(b => b.InventoryBalances)
               .HasForeignKey(ib => ib.ISBN13);

            modelBuilder.Entity<InventoryBalance>()
               .HasOne(ib => ib.Store)
               .WithMany(s => s.InventoryBalances)
               .HasForeignKey(ib => ib.StoreId);

        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = new SqlConnectionStringBuilder()
            {
                ServerSPN = "localhost",
                InitialCatalog = "BookstoreAdministration",
                TrustServerCertificate = true,
                IntegratedSecurity = true
            }.ToString();


            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
