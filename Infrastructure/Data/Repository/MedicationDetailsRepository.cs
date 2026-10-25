using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Infrastructure.Data.Repository
{
    public class MedicationDetailsRepository : IMedicationDetailsRepository
    {
        private readonly AppDbContext dbContext;

        public MedicationDetailsRepository(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IQueryable<MedicationDetails> GetMedicationDetails(string? stringSerch, SearchType searchType)
        {
            var query = this.dbContext.MedicationDetails.AsNoTracking();

            if(!string.IsNullOrEmpty(stringSerch))
            {
                var normalizedSearchString = stringSerch.Trim().ToLower();
                query = query.Where(m =>
                   (searchType == SearchType.TradeName && m.TradeName.Trim().ToLower().Contains(normalizedSearchString))
                    || (searchType == SearchType.ScientificName && m.ScientificName.Trim().ToLower().Contains(normalizedSearchString)));
            }

            return query;
        }

        public IQueryable<MedicationDetails> GetMedicationDetailsByTradeName(string tradeName)
        {
            var normalizedSearchString = tradeName.Trim().ToLower();

            return dbContext.MedicationDetails
                .AsNoTracking()
                .Where(m => m.TradeName.Trim().ToLower() == normalizedSearchString);
        }

        public IQueryable<MedicationDetails> GetMedicationDetailsByScientificName(string scientificName)
        {
            var scientificNames = scientificName
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(name => name.Trim().ToLower())
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct()
                .ToList();

            if (scientificNames.Count == 0)
            {
                return dbContext.MedicationDetails
                    .AsNoTracking()
                    .Where(_ => false);
            }

            return dbContext.MedicationDetails
                .AsNoTracking()
                .Where(m => scientificNames.Contains(m.ScientificName.Trim().ToLower()));
        }

        public IQueryable<MedicationDetails> GetMedicationDetailsByCompany(string Company)
        {
            var normalizedSearchString = Company.Trim().ToLower();
            return dbContext.MedicationDetails
                .AsNoTracking()
                .Where(m => m.Company.Trim().ToLower() == normalizedSearchString);
        }

        public IQueryable<MedicationDetails> GetMedicationDetailsByAgent(string agent)
        {
            var normalizedSearchString = agent.Trim().ToLower();
            return dbContext.MedicationDetails
                .AsNoTracking()
                .Where(m => m.Agent.Trim().ToLower() == normalizedSearchString);
        }

        public IQueryable<MedicationDetails> GetPharmaceuticalForm(string pharmaceuticalForm)
        {
            var normalizedSearchString = pharmaceuticalForm.Trim().ToLower();
            return dbContext.MedicationDetails
                .AsNoTracking()
                .Where(m => m.TradeName.Trim().ToLower() == normalizedSearchString);
        }
    }
}
