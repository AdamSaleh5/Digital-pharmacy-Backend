using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Pharmacies.Dto
{
    public class PharmaciesDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string ManagerName { get; set; }

        public string PhoneNumber { get; set; }
    }
}
