using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Medications.Dto
{
    public class MedicationDetailsDto
    {
        public int Id { get; set; }

        public string TradeName { get; set; }

        public string ScientificName { get; set; }

        public string Usage { get; set; }

        public string Company { get; set; }

        public string Agent { get; set; }

        public string AgentsPhoneNumber { get; set; }

        public string PharmaceuticalForm { get; set; }

        public double Price { get; set; } 
    }
}
