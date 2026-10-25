using Application.Models;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Pharmacies.Request
{
    public class PharmaciesSelectRequest : PaginationRequest
    {
        public int CountryId { get; set; }

        public PharmacieSearchType? SearchType { get; set; }

        public string? StringSerch { get; set; }
    }
}

