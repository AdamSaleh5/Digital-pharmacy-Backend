using Application.Constants;
using Application.Exceptions;
using Application.ImportMappings;
using Application.Interfaces;
using Application.Models;
using Application.UseCases.Pharmacies.Dto;
using Application.UseCases.Pharmacies.Request;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Pharmacies
{
    public class PharmaciesService : IPharmaciesService
    {
        private readonly IGenericRepository genericRepository;
        private readonly IPharmaciesRepository pharmaciesRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IExcelImportProvider<PharmaciesExcelImportMap, PharmaciesExcelDto> excelImportProvider;

        public PharmaciesService(
            IGenericRepository genericRepository,
            IPharmaciesRepository pharmaciesRepository,
            ICurrentUserService currentUserService,
            IExcelImportProvider<PharmaciesExcelImportMap, PharmaciesExcelDto> excelImportProvider)
        {
            this.genericRepository = genericRepository;
            this.pharmaciesRepository = pharmaciesRepository;
            this.currentUserService = currentUserService;
            this.excelImportProvider = excelImportProvider;
        }

        public async Task<PaginatedList<PharmaciesDto>> GetAll(PharmaciesSelectRequest request)
        {
            try
            {

                var currentUser = currentUserService.GetCurrentUser();
                var pharmacies = pharmaciesRepository.GetAllPharmacies(request.StringSerch, request.SearchType);

                if (currentUser.Role == UserRole.Admin)
                {
                    pharmacies = pharmacies.Where(m => m.CountryId == currentUser.CountryId.Value);
                }
                else
                {
                    pharmacies = pharmacies.Where(m => m.CountryId == request.CountryId);
                }

                
                var pharmaciesDto = pharmacies.Select(m => new PharmaciesDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Address = m.Address,
                    ManagerName = m.ManagerName,
                    PhoneNumber = m.PhoneNumber
                });

                return await PaginatedList<PharmaciesDto>.CreateAsync(pharmaciesDto, request.PageNumber, request.PageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task CreatePharmacie(PharmaciesRequest request)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();

                var pharmacie = new Pharmacie();

                pharmacie.Name = request.Name;
                pharmacie.Address = request.Address;
                pharmacie.ManagerName = request.ManagerName;
                pharmacie.PhoneNumber = request.PhoneNumber;
                
                if (currentUser.Role == UserRole.Admin)
                {
                    pharmacie.CountryId = currentUser.CountryId.Value;
                }
                else if (currentUser.Role == UserRole.Administrator)
                {
                    pharmacie.CountryId = request.CountryId;
                }
                else
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }

                genericRepository.Create(pharmacie);

                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public Task UpdatePharmacie(int id, PharmaciesRequest request)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();
                var pharmacies = genericRepository.GetById<Pharmacie>(id);
                if (pharmacies is null)
                {
                    throw new KeyNotFoundException(AppMessages.PharmacieNotFound);
                }

                var pharmacie = new Pharmacie();

                pharmacies.Name = request.Name;
                pharmacies.Address = request.Address;
                pharmacies.ManagerName = request.ManagerName;
                pharmacies.PhoneNumber = request.PhoneNumber;
                if (currentUser.Role == UserRole.Admin)
                {
                    pharmacie.CountryId = currentUser.CountryId.Value;
                }
                else if (currentUser.Role == UserRole.Administrator)
                {
                    pharmacie.CountryId = request.CountryId;
                }
                else
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }

                genericRepository.Update(pharmacies);

                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task DeletePharmacie(int id)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();
                var pharmacie= genericRepository.GetById<Pharmacie>(id);
                if (pharmacie is null)
                {
                    throw new KeyNotFoundException(AppMessages.MedicationNotFound);
                }

                if (currentUser.Role == UserRole.Admin)
                {
                    if (pharmacie.CountryId == currentUser.CountryId)
                    {
                        this.genericRepository.Delete<Pharmacie>(pharmacie.Id);
                    }
                    else
                    {
                        throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                    }
                }
                else if (currentUser.Role == UserRole.Administrator)
                {
                    this.genericRepository.Delete<Pharmacie>(pharmacie.Id);
                }
                else
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }

                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ImportResponse> ImportPharmaciesAsync(IFormFile file, int countryId)
        {
            if (file is null || file.Length == 0)
            {
                return FailedImport(new ImportError { Message = "Please upload a non-empty Excel file." });
            }

            var currentUser = currentUserService.GetCurrentUser();
            var resolvedCountryId = currentUser.Role == UserRole.Admin
                ? currentUser.CountryId
                : countryId;
            if (!resolvedCountryId.HasValue || resolvedCountryId.Value <= 0)
            {
                return FailedImport(new ImportError
                {
                    Column = "CountryId",
                    Value = resolvedCountryId?.ToString(),
                    Message = "A valid CountryId is required."
                });
            }

            List<PharmaciesExcelDto> records;
            try
            {
                records = (await excelImportProvider.ReadExcelFileAsync(file)).ToList();
            }
            catch (Exception exception)
            {
                return FailedImport(new ImportError { Message = exception.Message });
            }

            var errors = new List<ImportError>();
            foreach (var record in records)
            {
                AddRequiredError(errors, record.RowNumber, "Name", record.Name);
            }

            if (records.Count == 0)
                errors.Add(new ImportError { Message = "The Excel file does not contain any pharmacy rows." });
            if (errors.Count > 0)
                return new ImportResponse { Status = false, Errors = errors };

            var pharmacies = records.Select(item => new Pharmacie
            {
                Name = item.Name!.Trim(),
                Address = item.Address?.Trim() ?? string.Empty,
                ManagerName = item.ManagerName?.Trim() ?? string.Empty,
                PhoneNumber = item.PhoneNumber?.Trim() ?? string.Empty,
                CountryId = resolvedCountryId.Value
            }).ToList();

            try
            {
                genericRepository.BeginTransaction();
                genericRepository.CreateRangeDataEntity(pharmacies);
                genericRepository.CommitTransaction();
                return new ImportResponse { ImportedCount = pharmacies.Count };
            }
            catch
            {
                genericRepository.RollbackTransaction();
                throw;
            }
        }

        private static void AddRequiredError(List<ImportError> errors, int row, string column, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value)) return;

            errors.Add(new ImportError
            {
                Row = row,
                Column = column,
                Value = "(empty)",
                Message = $"Empty value found at Excel row {row}, column '{column}'. This field is required."
            });
        }

        private static ImportResponse FailedImport(ImportError error) => new()
        {
            Status = false,
            Errors = new List<ImportError> { error }
        };

    }
}
