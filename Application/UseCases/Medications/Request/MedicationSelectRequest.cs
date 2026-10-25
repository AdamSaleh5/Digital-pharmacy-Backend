using Application.Models;
using Domain.Enums;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Medications.Request
{
    public class MedicationSelectRequest : PaginationRequest
    {
        public int CountryId { get; set; }

        public SearchType SearchType { get; set; }

        public string? StringSerch { get; set; }
    }
}
