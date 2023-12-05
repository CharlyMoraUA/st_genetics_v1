using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
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
            // Assign a unique ID to the new animal here
            newAnimal.AnimalId = _animals.Count + 1;
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
