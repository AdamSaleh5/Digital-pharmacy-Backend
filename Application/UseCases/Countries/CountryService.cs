using Application.Constants;
using Application.Interfaces;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Countries
{
    public class CountryService : ICountryService
    {
        private readonly IGenericRepository genericRepository;
        private readonly ICurrentUserService currentUserService;

        public CountryService(IGenericRepository genericRepository, ICurrentUserService currentUserService) 
        {
            this.genericRepository = genericRepository;
            this.currentUserService = currentUserService;
        }
        public IQueryable<Country> GetAllCountries()
        {
            var currentUser = this.currentUserService.GetCurrentUser();

            if (currentUser.Role != UserRole.Administrator)
            {
                throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
            }

           var Countries =  this.genericRepository.GetAll<Country>();

            return Countries;
        }

        public Task CreateCountry(Country request)
        {
            var currentUser = this.currentUserService.GetCurrentUser();

            if (currentUser.Role != UserRole.Administrator)
            {
                throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
            }

            var Countries = this.genericRepository.GetAll<Country>().Where(c => c.Name.Trim().ToLower() == request.Name.Trim().ToLower());
            if (Countries.Any())
            {
                throw new InvalidOperationException($"Country with name '{request.Name}' already exists.");
            }

            this.genericRepository.Create< Country>(request);
            return Task.CompletedTask;
        }

        public Task UpdateCountry(int id, Country request)
        {
            var currentUser = this.currentUserService.GetCurrentUser();

            if (currentUser.Role != UserRole.Administrator)
            {
                throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
            }
            var contry = this.genericRepository.GetById<Country>(id);
            var Countries = this.genericRepository.GetAll<Country>().Where(c => c.Name.Trim().ToLower() == request.Name.Trim().ToLower() && c.Id != id);
            if (Countries.Any())
            {
                throw new InvalidOperationException($"Country with name '{request.Name}' already exists.");
            }
            contry.Name = request.Name;
            this.genericRepository.Update<Country>(contry);
            return Task.CompletedTask;
        }

        public Task DeleteCountry(int id)
        {
            var currentUser = this.currentUserService.GetCurrentUser();

            if (currentUser.Role != UserRole.Administrator)
            {
                throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
            }

            var Country = this.genericRepository.GetById<Country>(id);
            if (Country == null)
            {
                throw new InvalidOperationException(AppMessages.CountryNotFound);
            }

            this.genericRepository.Delete<Country>(id);
            return Task.CompletedTask;
        }
    }
}
