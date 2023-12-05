using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;
using st_genetics_v1.Models;

namespace st_genetics_v1.Services
{
    public class AnimalService
    {
        private List<Animal> _animals;
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _httpClient;
        private readonly string _filePath;

        public AnimalService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            _httpClient = httpClient;
            _filePath = Path.Combine(AppContext.BaseDirectory, "sample-data/animaldata.json");
        }

        public List<Animal> GetAnimals()
        {
            return _animals;
        }

        public async Task LoadAnimalsAsync()
        {
            try
            {
                var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "animals");
                _animals = string.IsNullOrEmpty(json)
                    ? await _httpClient.GetFromJsonAsync<List<Animal>>(_filePath)
                    : JsonSerializer.Deserialize<List<Animal>>(json);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading animals: {ex.Message}");
            }
        }

        public async Task AddAnimalAsync(Animal newAnimal)
        {
            // Find the maximum existing AnimalId
            int maxAnimalId = _animals.Any() ? _animals.Max(a => a.AnimalId) : 0;

            // Assign a unique ID to the new animal
            newAnimal.AnimalId = maxAnimalId + 1;

            _animals.Add(newAnimal);
            await SaveChangesToFileAsync();
        }


        public async Task UpdateAnimalAsync(Animal updatedAnimal)
        {
            // Update the animal in the list
            int index = _animals.FindIndex(a => a.AnimalId == updatedAnimal.AnimalId);
            if (index != -1)
            {
                _animals[index] = updatedAnimal;
                await SaveChangesToFileAsync();
            }
        }

        public async Task UpdateAnimalPropertyAsync(int animalId, string propertyName, string propertyValue)
        {
            // Update a specific property of the animal
            var animalToUpdate = _animals.Find(a => a.AnimalId == animalId);
            if (animalToUpdate != null)
            {
                // Use reflection to set the property value dynamically
                var propertyInfo = typeof(Animal).GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(animalToUpdate, Convert.ChangeType(propertyValue, propertyInfo.PropertyType));
                    await SaveChangesToFileAsync();
                }
            }
        }

        public async Task DeleteAnimalAsync(int animalId)
        {
            _animals.RemoveAll(a => a.AnimalId == animalId);
            await SaveChangesToFileAsync();
        }

        private async Task SaveChangesToFileAsync()
        {
            try
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "animals", JsonSerializer.Serialize(_animals));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving changes to localStorage: {ex.Message}");
            }
        }
    }
}
