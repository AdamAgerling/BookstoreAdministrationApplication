using System.Diagnostics;
using System.Net.Http;

namespace BookstoreAdmin.Model
{
    class BokusBookImageScraper
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly string _bokusUrl = "http://image.bokus.com/images/";
        private readonly string _isbn;

        public BokusBookImageScraper(string isbn)
        {
            _isbn = isbn;
        }

        public async Task SaveImageToDataBaseAsync()
        {
            var fullImgPath = $"{_bokusUrl}{_isbn}";

            try
            {
                var isImageAvailable = await CheckIfImageExistsAsync(fullImgPath);

                if (!isImageAvailable)
                {
                    Debug.WriteLine("No image found for the provided ISBN.");
                    return;
                }

                using (var db = new BookstoreDbContext())
                {
                    if (db.Database.CanConnect())
                    {
                        var existingImage = await db.Images.FindAsync(_isbn);
                        if (existingImage != null)
                        {
                            Debug.WriteLine("Image already exists in the database");
                            return;
                        }
                    }

                    var newImage = new Image
                    {
                        Id = _isbn,
                        ImageUrl = fullImgPath // Spara URL istället för binär data
                    };

                    db.Images.Add(newImage);
                    await db.SaveChangesAsync();
                    Debug.WriteLine("Image URL saved successfully");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error has occurred: {ex.Message}");
            }
        }

        private async Task<bool> CheckIfImageExistsAsync(string url)
        {
            try
            {
                var response = await _client.GetAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred while checking the image URL: {ex.Message}");
                return false;
            }
        }
    }
}