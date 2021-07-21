using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Zoo.Models.ApiModels;
using Zoo.Models.DbModels;

namespace Zoo.Services
{
    public interface IAnimalService
    {
        AnimalResponseModel GetAnimalById(int id);
        void AddAnimalToDb(AnimalRequestModel animal);
        SpeciesResponseModel GetSpeciesById(int id);
        void AddSpeciesToDb(SpeciesRequestModel animal);
        SpeciesDbModel GetDbModelSpeciesById(int id);
        List<AnimalResponseModel> Search(SearchRequestModel search);
        List<AnimalResponseModel> OrderResponse(List<AnimalResponseModel> response, SearchRequestModel search);
    }

    public class AnimalService : IAnimalService
    {
        private readonly ZooDbContext _context;

        public AnimalService(ZooDbContext context)
        {
            _context = context;
        }

        public AnimalResponseModel GetAnimalById(int id)
        {
            return new AnimalResponseModel(
                _context.Animal
                .Include(animal => animal.Species)
                .Single(animal => animal.Id == id)
            );
        }

        public void AddAnimalToDb(AnimalRequestModel animal)
        {
            _context.Animal.Add(new AnimalDbModel
            {
                Name = animal.Name,
                DateOfBirth = animal.DateOfBirth,
                DateOfArrival = animal.DateOfArrival,
                Sex = animal.Sex,
                Species = GetDbModelSpeciesById(animal.SpeciesId)
            });

            _context.SaveChanges();
        }

        public SpeciesResponseModel GetSpeciesById(int id) => new SpeciesResponseModel(GetDbModelSpeciesById(id));

        public SpeciesDbModel GetDbModelSpeciesById(int id)
        {
            return _context.Species
            .Single(species => species.Id == id);
        }

        public void AddSpeciesToDb(SpeciesRequestModel species)
        {
            _context.Species.Add(new SpeciesDbModel
            {
                Type = species.Type,
                Classification = species.Classification

            });

            _context.SaveChanges();
        }

        public List<AnimalResponseModel> Search(SearchRequestModel search)
        {
            var mostRecentBirthday = DateTime.Today.AddYears(-search.Age);
            var earliestBirthday = mostRecentBirthday.AddYears(-1);

            var unorderedResponse = _context.Animal
                .Include(a => a.Species)
                .Where(a => search.Classification == null || a.Species.Classification == search.Classification)
                .Where(a => search.Type == "all" || a.Species.Type == search.Type)
                .Where(a => search.Age == 0 || a.DateOfBirth > earliestBirthday && a.DateOfBirth <= mostRecentBirthday)
                .Where(a => search.Name == null || a.Name == search.Name)
                .Where(a => search.DateAcquired == default(DateTime) || a.DateOfArrival == search.DateAcquired)
                .Select(a => new AnimalResponseModel(a))
                .ToList();

            return OrderResponse(unorderedResponse, search)
                .Skip((search.Page - 1) * search.PageSize)
                .Take(search.PageSize)
                .ToList();
        }

        public List<AnimalResponseModel> OrderResponse(List<AnimalResponseModel> response, SearchRequestModel search)
        {
            switch (search.OrderBy)
            {                
                case (Models.Enums.OrderBy)0:
                    response = response.OrderBy(a => a.Species).ToList();
                    break;
                case (Models.Enums.OrderBy)1:
                    response = response.OrderBy(a => a.Classification).ToList();
                    break;
                case (Models.Enums.OrderBy)2:
                    response = response.OrderBy(a => a.Age).ToList();
                    break;
                case (Models.Enums.OrderBy)3:
                    response = response.OrderBy(a => a.Name).ToList();
                    break;
                case (Models.Enums.OrderBy)4:
                    response = response.OrderBy(a => a.DateOfArrival).ToList();
                    break;
                default:
                    response = response.OrderBy(a => a.Species).ToList();
                    break;
            }
            return response;
        }
    }
}
