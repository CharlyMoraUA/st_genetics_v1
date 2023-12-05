using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using st_genetics_v1.Models;

namespace st_genetics_v1.Services
{
    public class AnimalService
    {
        private List<Animal> _animals;
        private readonly string _filePath;
        private readonly HttpClient _httpClient;

        public AnimalService(HttpClient httpClient)
        {
            _filePath = Path.Combine(AppContext.BaseDirectory, "sample-data/animaldata.json");
            _httpClient = httpClient;
        }

        public List<Animal> GetAnimals()
        {
            return _animals;
        }


        public async Task LoadAnimalsAsync()
        {
            try
            {
                _animals = await _httpClient.GetFromJsonAsync<List<Animal>>(_filePath);
                Console.WriteLine("_animals");
                Console.WriteLine(_animals);
                Console.WriteLine("_animals");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading animals: {ex.Message}");
            }
        }


        private void WriteToJson(List<Animal> animals, string filePath)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(animals, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to JSON file: {ex.Message}");
            }
        }
    }
}
