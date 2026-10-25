using Application.Models;
using Application.UseCases.Pharmacies.Dto;
using Application.UseCases.Pharmacies.Request;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPharmaciesService
    {
        public Task<PaginatedList<PharmaciesDto>> GetAll(PharmaciesSelectRequest request);

        public Task CreatePharmacie(PharmaciesRequest request);

        public Task UpdatePharmacie(int id, PharmaciesRequest request);

        public Task DeletePharmacie(int id);

        Task<ImportResponse> ImportPharmaciesAsync(IFormFile file, int countryId);


    }
}
