using Application.UseCases.Medications.Dto;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ImportMappings
{
    public class MedicationDetailExcelImportMap : ClassMap<MedicationDetailsExcelDto>
    {
        public MedicationDetailExcelImportMap()
        {
            this.AutoMap(CultureInfo.InvariantCulture);

            this.Map(m => m.TradeName).Name("TradeName");
            this.Map(m => m.ScientificName).Name("ScientificName");
            this.Map(m => m.Usage).Name("Usage");
            this.Map(m => m.Company).Name("Company");
            this.Map(m => m.Agent).Name("Agent");
            this.Map(m => m.PharmaceuticalForm).Name("PharmaceuticalForm");
            this.Map(m => m.Price).Name("Price");
            this.Map(m => m.AgentsPhoneNumber).Name("AgentsPhoneNumber");
        }
    }
}
