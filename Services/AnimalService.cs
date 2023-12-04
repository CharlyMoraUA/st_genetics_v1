
using System;
using System.Collections.Generic;
using st_genetics_v1.Models;

namespace st_genetics_v1.Services
{

    public class AnimalService
    {
        private List<Animal> _animals;

        public AnimalService()
        {
            // Initialize the list with 20 fictitious animals
            _animals = GenerateFictitiousData();
        }

        public List<Animal> GetAnimals()
        {
            return _animals;
        }

        // Other CRUD operations go here...

        private List<Animal> GenerateFictitiousData()
        {
            var animals = new List<Animal>();
            var random = new Random();

            for (int i = 1; i <= 20; i++)
            {
                var animal = new Animal
                {
                    AnimalId = i,
                    Name = $"Animal {i}",
                    Breed = $"Breed {random.Next(1, 5)}",
                    BirthDate = DateTime.Now.AddYears(-random.Next(1, 5)),
                    Sex = (i % 2 == 0) ? "Male" : "Female",
                    Price = random.Next(100, 1000),
                    Status = (i % 3 == 0) ? "Inactive" : "Active"
                };

                animals.Add(animal);
            }

            return animals;
        }
    }

}
