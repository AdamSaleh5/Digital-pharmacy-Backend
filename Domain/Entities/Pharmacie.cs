using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Pharmacie : IDataEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string ManagerName { get; set; }

        public string PhoneNumber { get; set; }

        public int? CountryId { get; set; }

        public Country Country { get; set; }
    }
}
