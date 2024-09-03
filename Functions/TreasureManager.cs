using Functions.Models;
using Functions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Functions
{
    public class TreasureManager
    {
        private readonly ILogger<TreasureManager> _logger;
        private readonly BlobStorageService _blobService;
        private readonly TableStorageService _tableStorageService;

        public TreasureManager(ILogger<TreasureManager> logger)
        {
            _logger = logger;
            _blobService = new BlobStorageService(Environment.GetEnvironmentVariable("AzureConnection"));
            _tableStorageService = new TableStorageService(Environment.GetEnvironmentVariable("AzureConnection"));
        }

        [Function("submit")]
        public async Task<IActionResult> submitAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            string body = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject<dynamic>(body);

            string itemName = string.IsNullOrEmpty(data["name"]) ? null : data["name"];
            string foundBy = string.IsNullOrEmpty(data["location"]) ? null : data["location"];
            string image = string.IsNullOrEmpty(data["image"]) ? null : data["image"];

            Find newFind = new Find(itemName, foundBy);

            //if image is not null we read the base64 and upload it as a blob

            if (!string.IsNullOrEmpty(image))
            {
                //Ref: https://stackoverflow.com/questions/31524343/how-to-convert-base64-value-from-a-database-to-a-stream-with-c-sharp
                byte[] imageBytes = Convert.FromBase64String(image);
                MemoryStream imageStream = new MemoryStream(imageBytes);
                image = await _blobService.UploadAsync(imageStream, Guid.NewGuid().ToString());
            }

            newFind.ImageURI = image;
            await _tableStorageService.AddFindAsync(newFind);

            return new OkObjectResult("Find has been logged");
        }

        [Function("leaderboard")]
        public async Task<IActionResult> leaderboardAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            List<Find> finds = await _tableStorageService.GetAllFindsAsync();

            Dictionary<string, int> findCounts = new Dictionary<string, int>();

            foreach (Find find in finds)
            {
                if (findCounts.ContainsKey(find.FinderName))
                {
                    findCounts[find.FindName] += 1;
                }
                else
                {
                    findCounts.Add(find.FinderName, 1);
                }
            }

            // Ref sean
            // https://stackoverflow.com/users/82371/sean
            // https://stackoverflow.com/questions/289/how-do-you-sort-a-dictionary-by-value
            var orderedFindCounts = findCounts.OrderByDescending(x => x.Value);

            var jsonData = JsonConvert.SerializeObject(orderedFindCounts);

            return new JsonResult(jsonData);
        }

        [Function("hint")]
        public async Task<IActionResult> hintAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            List<Hint> hints = await _tableStorageService.GetAllHintsAsync();

            int randomIndex = new Random().Next(hints.Count);
            Hint randomHint = hints[randomIndex];

            string jsonData = JsonConvert.SerializeObject(randomHint);

            return new JsonResult(jsonData);
        }


    }
}
