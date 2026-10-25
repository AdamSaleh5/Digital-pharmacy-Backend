using Application.Models;
using Application.UseCases.Medications.Dto;
using Application.UseCases.Medications.Request;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMedicationDetailsService
    {
        public Task<PaginatedList<MedicationSelectDto>> GetAll(MedicationSelectRequest request);

        public Task<PaginatedList<MedicationDetailsDto>> GetAllMedication(int id, SearchType searchType, PaginationRequest request);

        public Task<PaginatedList<CompanySelectDto>> GetCompanySeletc(int? CountryId,PaginationRequest request);

        public Task<PaginatedList<MedicationDetailsDto>> GetMedicationDetailsByCompany(int id, int? CountryId, PaginationRequest request);

        public Task<PaginatedList<MedicationSelectDto>> GetAgentSeletc(int? CountryId, PaginationRequest request);

        public Task<PaginatedList<MedicationDetailsDto>> GetMedicationDetailsByAgent(int id, int? CountryId, PaginationRequest request);

        public Task<PaginatedList<MedicationDetailsDto>> GetPharmaceuticalFormById(int id, PaginationRequest request);

        public Task CreateMedicationDetails(MedicationDetailsRequest request);

        public Task UpdateMedicationDetails(int id, MedicationDetailsRequest request);

        public Task DeleteMedicationDetails(int id);

        public Task<ImportResponse> ImportMedicationDetailsAsync(IFormFile file, int CountryId);
    }
}

