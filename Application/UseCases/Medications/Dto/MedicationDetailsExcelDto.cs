using CsvHelper.Configuration.Attributes;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Medications.Dto
{
    public class MedicationDetailsExcelDto
    {
        [Name("TradeName")]
        public string? TradeName { get; set; }

        [Name("ScientificName")]
        public string? ScientificName { get; set; }

        [Name("Usage")]
        public string? Usage { get; set; }

        [Name("Company")]
        public string? Company { get; set; }

        [Name("Agent")]
        public string? Agent { get; set; }

        [Name("PharmaceuticalForm")]
        public string? PharmaceuticalForm { get; set; }

        [Name("Price")]
        public double Price { get; set; }

        [Name("AgentsPhoneNumber")]
        public string? AgentsPhoneNumber { get; set; }
    }
}
