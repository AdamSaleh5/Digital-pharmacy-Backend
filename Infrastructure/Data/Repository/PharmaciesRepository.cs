using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repository
{
    public class PharmaciesRepository : IPharmaciesRepository
    {
        private readonly AppDbContext dbContext;

        public PharmaciesRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<Pharmacie> GetAllPharmacies(string? stringSerch, PharmacieSearchType? pharmacieSearchType)
        {
            var query = this.dbContext.Pharmacies.AsNoTracking();

            if (!string.IsNullOrEmpty(stringSerch))
            {
                var normalizedSearchString = stringSerch.Trim().ToLower();
                query = query.Where(p =>
                   (pharmacieSearchType == PharmacieSearchType.Name && p.Name.Trim().ToLower().Contains(normalizedSearchString))
                    || (pharmacieSearchType == PharmacieSearchType.Address && p.Address.Trim().ToLower().Contains(normalizedSearchString))
                    || (pharmacieSearchType == PharmacieSearchType.ManagerName && p.ManagerName.Trim().ToLower().Contains(normalizedSearchString))
                    || (pharmacieSearchType == PharmacieSearchType.PhoneNumber && p.PhoneNumber.Trim().ToLower().Contains(normalizedSearchString)));
            }

            return query;
        }
    }
}
