using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace BookstoreAdmin.Model
{
    class BookstoreDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<InventoryBalance> InventoryBalances { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<BookLanguage> BookLanguages { get; set; }
        public DbSet<PurchaseHistory> PurchaseHistories { get; set; }

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
