using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICountryService
    {
        public IQueryable<Country> GetAllCountries();

        public Task CreateCountry(Country request);

        public Task UpdateCountry(int id, Country request);

        public Task DeleteCountry(int id);
    }
}
