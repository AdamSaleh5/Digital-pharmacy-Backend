using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPharmaciesRepository
    {
        public IQueryable<Pharmacie> GetAllPharmacies(string? stringSerch, PharmacieSearchType? pharmacieSearchType);
    }
}
