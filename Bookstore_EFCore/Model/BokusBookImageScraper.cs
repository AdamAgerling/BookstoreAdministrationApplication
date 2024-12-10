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
                    return;
                }

                using (var db = new BookstoreDbContext())
                {
                    if (db.Database.CanConnect())
                    {
                        var existingImage = await db.Images.FindAsync(_isbn);

                        if (existingImage == null)
                        {
                            var newImage = new Image
                            {
                                ImageId = _isbn,
                                ImageUrl = fullImgPath
                            };

                            db.Images.Add(newImage);
                            await db.SaveChangesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred during SaveImageToDataBaseAsync: {ex.Message}");
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
                Debug.WriteLine($"Error during image existence check: {ex.Message}");
                return false;
            }
        }
    }
}
