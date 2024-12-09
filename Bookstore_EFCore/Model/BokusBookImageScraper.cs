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
            Debug.WriteLine($"Attempting to fetch image from URL: {fullImgPath}");

            try
            {
                var isImageAvailable = await CheckIfImageExistsAsync(fullImgPath);
                Debug.WriteLine($"Image availability check completed: {isImageAvailable}");

                if (!isImageAvailable)
                {
                    Debug.WriteLine("No image found at the provided URL. Skipping image saving.");
                    return;
                }

                using (var db = new BookstoreDbContext())
                {
                    Debug.WriteLine("Attempting to connect to the database...");

                    if (db.Database.CanConnect())
                    {
                        Debug.WriteLine("Database connection successful.");

                        var existingImage = await db.Images.FindAsync(_isbn);

                        if (existingImage == null)
                        {
                            var newImage = new Image
                            {
                                ImageId = _isbn,
                                ImageUrl = fullImgPath
                            };

                            db.Images.Add(newImage);
                            Debug.WriteLine("Adding new image to database...");
                            await db.SaveChangesAsync();
                            Debug.WriteLine("Image saved successfully.");
                        }
                        else
                        {
                            Debug.WriteLine("Image already exists in the database. No action taken.");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Failed to connect to the database.");
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
            Debug.WriteLine($"Checking image existence for URL: {url}");
            try
            {
                var response = await _client.GetAsync(url);
                Debug.WriteLine($"Response for URL {url}: {response.StatusCode}");

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
