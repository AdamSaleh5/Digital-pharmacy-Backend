using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMedicationDetailsRepository
    {
        public IQueryable<MedicationDetails> GetMedicationDetails(string? stringSerch, SearchType searchType);

        public IQueryable<MedicationDetails> GetMedicationDetailsByTradeName(string tradeName);

        public IQueryable<MedicationDetails> GetMedicationDetailsByScientificName(string scientificName);

        public IQueryable<MedicationDetails> GetMedicationDetailsByCompany(string Company);

        public IQueryable<MedicationDetails> GetMedicationDetailsByAgent(string agent);

        public IQueryable<MedicationDetails> GetPharmaceuticalForm(string pharmaceuticalForm);
    }
}
