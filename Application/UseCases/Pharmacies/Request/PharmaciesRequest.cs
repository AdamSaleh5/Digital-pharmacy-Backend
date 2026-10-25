using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Pharmacies.Request
{
    public class PharmaciesRequest
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string ManagerName { get; set; }

        public string PhoneNumber { get; set; }

        public int CountryId { get; set; }
    }
}
