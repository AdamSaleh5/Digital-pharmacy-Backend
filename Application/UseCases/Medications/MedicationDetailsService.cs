using Application.Interfaces;
using Application.Constants;
using Application.Exceptions;
using Application.Models;
using Application.UseCases.Medications.Dto;
using Application.UseCases.Medications.Request;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Application.ImportMappings;

namespace Application.UseCases.Medications
{
    public class MedicationDetailsService : IMedicationDetailsService
    {
        private readonly IGenericRepository genericRepository;
        private readonly IMedicationDetailsRepository medicationDetailsRepository;
        private readonly ICurrentUserService currentUserService;
        private readonly IExcelImportProvider<MedicationDetailExcelImportMap, MedicationDetailsExcelDto> excelImportProvider;
        private readonly IMedicationDetailsValidation medicationDetailsValidation;

        public MedicationDetailsService(
            IMedicationDetailsRepository medicationDetailsRepository,
            ICurrentUserService currentUserService,
            IGenericRepository genericRepository,
            IExcelImportProvider<MedicationDetailExcelImportMap, MedicationDetailsExcelDto> excelImportProvider,
            IMedicationDetailsValidation medicationDetailsValidation)
        {
            this.medicationDetailsRepository = medicationDetailsRepository;
            this.currentUserService = currentUserService;
            this.genericRepository = genericRepository;
            this.excelImportProvider = excelImportProvider;
            this.medicationDetailsValidation = medicationDetailsValidation;
        }

        public async Task<PaginatedList<MedicationSelectDto>> GetAll(MedicationSelectRequest request)
        {
            try
            {
            if (!Enum.IsDefined(request.SearchType))
            {
                throw new InvalidSearchTypeException();
            }

            var currentUser = currentUserService.GetCurrentUser();
            var medications = medicationDetailsRepository.GetMedicationDetails(request.StringSerch, request.SearchType);
            if (currentUser.Role == UserRole.Agent)
            {
                medications = medications.Where(m => m.CountryId == currentUser.CountryId.Value && m.Agent.Trim().ToLower().Contains(currentUser.AgentName.Trim().ToLower()));
            }
            else if (currentUser.Role == UserRole.Admin)
            {
                medications = medications.Where(m => m.CountryId == currentUser.CountryId.Value);
            }
            else
            {
                medications = medications.Where(m => m.CountryId == request.CountryId);
            }
            
            var medicationSelectDtos = medications.Select(m => new MedicationSelectDto
            {
                Id = m.Id,
                Value = request.SearchType == SearchType.TradeName ? m.TradeName : m.ScientificName
            });

            medicationSelectDtos = medicationSelectDtos.GroupBy(m => m.Value).Select(m => m.First()); 

            return await PaginatedList<MedicationSelectDto>.CreateAsync(medicationSelectDtos, request.PageNumber, request.PageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaginatedList<MedicationDetailsDto>>  GetAllMedication(int id, SearchType searchType, PaginationRequest request)
        {
            try
            {
            if (!Enum.IsDefined(searchType))
            {
                throw new InvalidSearchTypeException();
            }

            var currentUser = currentUserService.GetCurrentUser();

            var medication = genericRepository.GetById<MedicationDetails>(id);
            if (medication is null)
            {
                throw new KeyNotFoundException(AppMessages.MedicationNotFound);
            }

            var medicationDetails = searchType switch
            {
                SearchType.TradeName =>
                    medicationDetailsRepository.GetMedicationDetailsByTradeName(
                        medication.TradeName),

                SearchType.ScientificName =>
                    medicationDetailsRepository.GetMedicationDetailsByScientificName(
                        medication.ScientificName),

                _ => throw new ArgumentOutOfRangeException(nameof(searchType), searchType, null)
            };

            if (currentUser.Role == UserRole.Agent)
            {
                medicationDetails = medicationDetails.Where(m => m.CountryId == currentUser.CountryId.Value && m.Agent.Trim().ToLower().Contains(currentUser.AgentName.Trim().ToLower()));
            }
            else if (currentUser.Role == UserRole.Admin)
            {
                medicationDetails = medicationDetails.Where(m => m.CountryId == currentUser.CountryId.Value);
            }
            else
            {
                medicationDetails = medicationDetails.Where(m => m.CountryId == medication.CountryId);
            }

            var medicationDetailsDto = medicationDetails.Select(m => new MedicationDetailsDto
            {
                Id = m.Id,
                TradeName = m.TradeName,
                ScientificName = m.ScientificName,
                Usage = m.Usage,
                Company = m.Company,
                Agent = m.Agent,
                AgentsPhoneNumber = m.AgentsPhoneNumber,
                PharmaceuticalForm = m.PharmaceuticalForm,
                Price = m.Price,
            });

            return await PaginatedList<MedicationDetailsDto>.CreateAsync(medicationDetailsDto, request.PageNumber, request.PageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaginatedList<CompanySelectDto>> GetCompanySeletc(int? CountryId ,PaginationRequest request)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();
                var medications = this.genericRepository.GetAll<MedicationDetails>();

                if (currentUser.Role == UserRole.Agent)
                {
                    medications = medications.Where(m =>
                        m.CountryId == currentUser.CountryId.Value &&
                        m.Agent.Trim().ToLower().Contains(currentUser.AgentName.Trim().ToLower()));
                }
                else if (currentUser.Role == UserRole.Admin)
                {
                    medications = medications.Where(m => m.CountryId == currentUser.CountryId.Value);
                }
                else if (CountryId.HasValue)
                {
                    medications = medications.Where(m => m.CountryId == CountryId.Value);
                }

                // Keep one record per Company + Agent pair, while retaining the company for different agents.
                var firstMedicationIds = medications
                        .GroupBy(m => new { m.Company, m.Agent })
                        .Select(group => group.Min(m => m.Id));

                var companySelectDtos = medications
                    .Where(m => firstMedicationIds.Contains(m.Id))
                    .OrderBy(m => m.Company)
                    .ThenBy(m => m.Agent)
                    .Select(m => new CompanySelectDto
                    {
                        Id = m.Id,
                        Company = m.Company,
                        Agent = m.Agent,
                        AgentsPhoneNumber = m.AgentsPhoneNumber
                    });

                return await PaginatedList<CompanySelectDto>.CreateAsync(
                    companySelectDtos,
                    request.PageNumber,
                    request.PageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaginatedList<MedicationDetailsDto>> GetMedicationDetailsByCompany(int id, int? CountryId, PaginationRequest request)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();
                var medications = this.genericRepository.GetById<MedicationDetails>(id);
                if (medications is null)
                {
                    throw new KeyNotFoundException(AppMessages.MedicationNotFound);
                }

                var medicationsByCompany = medicationDetailsRepository.GetMedicationDetailsByCompany(medications.Company);

                if (currentUser.Role == UserRole.Agent)
                {
                    medicationsByCompany = medicationsByCompany.Where(m =>
                        m.CountryId == currentUser.CountryId.Value &&
                        m.Agent.Trim().ToLower().Contains(currentUser.AgentName.Trim().ToLower()));
                }
                else if (currentUser.Role == UserRole.Admin)
                {
                    medicationsByCompany = medicationsByCompany.Where(m => m.CountryId == currentUser.CountryId.Value);
                }
                else if (CountryId.HasValue)
                {
                    medicationsByCompany = medicationsByCompany.Where(m => m.CountryId == CountryId.Value);
                }

                var medicationDetailsDto = medicationsByCompany.Select(m => new MedicationDetailsDto
                {
                    Id = m.Id,
                    TradeName = m.TradeName,
                    ScientificName = m.ScientificName,
                    Usage = m.Usage,
                    Company = m.Company,
                    Agent = m.Agent,
                    AgentsPhoneNumber = m.AgentsPhoneNumber,
                    PharmaceuticalForm = m.PharmaceuticalForm,
                    Price = m.Price,
                });

                return await PaginatedList<MedicationDetailsDto>.CreateAsync(
                    medicationDetailsDto,
                    request.PageNumber,
                    request.PageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaginatedList<MedicationSelectDto>> GetAgentSeletc(int? CountryId, PaginationRequest request)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();
                var medications = this.genericRepository.GetAll<MedicationDetails>();

                if (currentUser.Role == UserRole.Agent)
                {
                    medications = medications.Where(m =>
                        m.CountryId == currentUser.CountryId.Value &&
                        m.Agent.Trim().ToLower().Contains(currentUser.AgentName.Trim().ToLower()));
                }
                else if (currentUser.Role == UserRole.Admin)
                {
                    medications = medications.Where(m => m.CountryId == currentUser.CountryId.Value);
                }
                else if (CountryId.HasValue)
                {
                    medications = medications.Where(m => m.CountryId == CountryId.Value);
                }

                var medicationSelectDtos = medications.Select(m => new MedicationSelectDto
                {
                    Id = m.Id,
                    Value = m.Agent,
                });

                medicationSelectDtos = medicationSelectDtos.GroupBy(m => m.Value).Select(m => m.First());

               

                return await PaginatedList<MedicationSelectDto>.CreateAsync(
                    medicationSelectDtos,
                    request.PageNumber,
                    request.PageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaginatedList<MedicationDetailsDto>> GetMedicationDetailsByAgent(int id, int? CountryId, PaginationRequest request)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();
                var medications = this.genericRepository.GetById<MedicationDetails>(id);
                if (medications is null)
                {
                    throw new KeyNotFoundException(AppMessages.MedicationNotFound);
                }

                var medicationsByCompany = medicationDetailsRepository.GetMedicationDetailsByAgent(medications.Agent);

                if (currentUser.Role == UserRole.Agent)
                {
                    medicationsByCompany = medicationsByCompany.Where(m =>
                        m.CountryId == currentUser.CountryId.Value &&
                        m.Agent.Trim().ToLower().Contains(currentUser.AgentName.Trim().ToLower()));
                }
                else if (currentUser.Role == UserRole.Admin)
                {
                    medicationsByCompany = medicationsByCompany.Where(m => m.CountryId == currentUser.CountryId.Value);
                }
                else if (CountryId.HasValue)
                {
                    medicationsByCompany = medicationsByCompany.Where(m => m.CountryId == CountryId.Value);
                }

                var medicationDetailsDto = medicationsByCompany.Select(m => new MedicationDetailsDto
                {
                    Id = m.Id,
                    TradeName = m.TradeName,
                    ScientificName = m.ScientificName,
                    Usage = m.Usage,
                    Company = m.Company,
                    Agent = m.Agent,
                    AgentsPhoneNumber = m.AgentsPhoneNumber,
                    PharmaceuticalForm = m.PharmaceuticalForm,
                    Price = m.Price,
                });

                return await PaginatedList<MedicationDetailsDto>.CreateAsync(
                    medicationDetailsDto,
                    request.PageNumber,
                    request.PageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PaginatedList<MedicationDetailsDto>> GetPharmaceuticalFormById(int id, PaginationRequest request)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();
                var medications = this.genericRepository.GetById<MedicationDetails>(id);
                if (medications is null)
                {
                    throw new KeyNotFoundException(AppMessages.MedicationNotFound);
                }

                var medicationsByCompany = medicationDetailsRepository.GetMedicationDetailsByTradeName(medications.TradeName);

                if (currentUser.Role == UserRole.Agent)
                {
                    medicationsByCompany = medicationsByCompany.Where(m =>
                        m.CountryId == currentUser.CountryId.Value &&
                        m.Agent.Trim().ToLower().Contains(currentUser.AgentName.Trim().ToLower()) && m.Id != medications.Id);
                }
                else if (currentUser.Role == UserRole.Admin)
                {
                    medicationsByCompany = medicationsByCompany.Where(m => m.CountryId == currentUser.CountryId.Value && m.Id != medications.Id);
                }
                else
                {
                    medicationsByCompany = medicationsByCompany.Where(m => m.CountryId == medications.CountryId && m.Id != medications.Id);
                }

                var medicationDetailsDto = medicationsByCompany.Select(m => new MedicationDetailsDto
                {
                    Id = m.Id,
                    TradeName = m.TradeName,
                    ScientificName = m.ScientificName,
                    Usage = m.Usage,
                    Company = m.Company,
                    Agent = m.Agent,
                    AgentsPhoneNumber = m.AgentsPhoneNumber,
                    PharmaceuticalForm = m.PharmaceuticalForm,
                    Price = m.Price,
                });

                return await PaginatedList<MedicationDetailsDto>.CreateAsync(
                    medicationDetailsDto,
                    request.PageNumber,
                    request.PageSize);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task CreateMedicationDetails(MedicationDetailsRequest request)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();
                
                var medication = new MedicationDetails();
                medication.TradeName = request.TradeName;
                medication.ScientificName = request.ScientificName;
                medication.Usage = request.Usage;
                medication.Company = request.Company;
                medication.PharmaceuticalForm = request.PharmaceuticalForm;
                medication.Price = request.Price;
                

                if (currentUser.Role == UserRole.Agent)
                {
                    medication.Agent = currentUser.AgentName;
                    medication.CountryId = currentUser.CountryId.Value;
                    medication.AgentsPhoneNumber = currentUser.PhoneNumber;
                }
                else if (currentUser.Role == UserRole.Admin)
                {
                    medication.Agent = request.Agent;
                    medication.CountryId = currentUser.CountryId.Value;
                    medication.AgentsPhoneNumber = request.AgentsPhoneNumber;
                }
                else if (currentUser.Role == UserRole.Administrator)
                {
                    medication.Agent = request.Agent;
                    medication.CountryId = request.CountryId;
                    medication.AgentsPhoneNumber = request.AgentsPhoneNumber;
                }
                else 
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }

                genericRepository.Create(medication);

                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public Task UpdateMedicationDetails(int id, MedicationDetailsRequest request)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();
                var medication = genericRepository.GetById<MedicationDetails>(id);
                if (medication is null)
                {
                    throw new KeyNotFoundException(AppMessages.MedicationNotFound);
                }

                medication.TradeName = request.TradeName.Trim();
                medication.ScientificName = request.ScientificName.Trim();
                medication.Usage = request.Usage.Trim();
                medication.Company = request.Company.Trim();
                medication.PharmaceuticalForm = request.PharmaceuticalForm.Trim();
                medication.Price = request.Price;

                if (currentUser.Role == UserRole.Agent)
                {
                    medication.Agent = currentUser.AgentName;
                    medication.CountryId = currentUser.CountryId.Value;
                    medication.AgentsPhoneNumber = currentUser.PhoneNumber;
                }
                else if (currentUser.Role == UserRole.Admin)
                {
                    medication.Agent = request.Agent;
                    medication.CountryId = currentUser.CountryId.Value;
                    medication.AgentsPhoneNumber = request.AgentsPhoneNumber;
                }
                else if (currentUser.Role == UserRole.Administrator)
                {
                    medication.Agent = request.Agent;
                    medication.CountryId = request.CountryId;
                    medication.AgentsPhoneNumber = request.AgentsPhoneNumber;
                }
                else
                {
                    throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                }

                genericRepository.Update(medication);

                return Task.CompletedTask;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task DeleteMedicationDetails(int id)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();
                var medication = genericRepository.GetById<MedicationDetails>(id);
                if (medication is null)
                {
                    throw new KeyNotFoundException(AppMessages.MedicationNotFound);
                }

                if (currentUser.Role == UserRole.Agent)
                {
                    if (medication.Agent == currentUser.AgentName && medication.CountryId == currentUser.CountryId)
                    {
                        this.genericRepository.Delete<MedicationDetails>(medication.Id);                    
                    }
                    else
                    {
                        throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                    }
                    
                }
                else if (currentUser.Role == UserRole.Admin)
                {
                    if (medication.CountryId == currentUser.CountryId)
                    {
                        this.genericRepository.Delete<MedicationDetails>(medication.Id);
                    }
                    else
                    {
                        throw new UnauthorizedAccessException(AppMessages.UnauthorizedAccess);
                    }
                }
                else if (currentUser.Role == UserRole.Administrator)
                {
                    this.genericRepository.Delete<MedicationDetails>(medication.Id);
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

        public async Task<ImportResponse> ImportMedicationDetailsAsync(IFormFile file, int CountryId)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();

                this.genericRepository.BeginTransaction();
                var medicationDetailsRecords = (await this.excelImportProvider.ReadExcelFileAsync(file)).ToList();

                var validationResult = await this.medicationDetailsValidation.ValidateImportedMedicationDetailsAsync(medicationDetailsRecords);


                var countryId = currentUser.Role == UserRole.Agent
                    ? currentUser.CountryId : currentUser.Role == UserRole.Admin 
                    ? currentUser.CountryId
                    : CountryId;
                var medicationsToCreate = new List<MedicationDetails>();
                var medicationsToUpdate = new List<MedicationDetails>();
                var medicationDetails = this.genericRepository.GetAll<MedicationDetails>().Where(m => m.CountryId == countryId);
                foreach (var item in validationResult.Records)
                {
                    var tradeName = item.TradeName.ToLower().Trim();
                    var pharmaceuticalForm = item.PharmaceuticalForm.ToLower().Trim();

                    var existingMedication = medicationDetails.Any(m =>
                            m.TradeName.ToLower().Trim() == tradeName &&
                            m.PharmaceuticalForm.ToLower().Trim() == pharmaceuticalForm);

                    if (existingMedication)
                    {
                        medicationsToUpdate.Add(new MedicationDetails
                        {
                            TradeName = tradeName,
                            ScientificName = item.ScientificName,
                            Usage = item.Usage,
                            Company = item.Company,
                            PharmaceuticalForm = pharmaceuticalForm,
                            Price = item.Price,
                            CountryId = countryId,
                            Agent = currentUser.Role == UserRole.Agent ? currentUser.AgentName : item.Agent,
                            AgentsPhoneNumber = currentUser.Role == UserRole.Agent ? currentUser.PhoneNumber : item.AgentsPhoneNumber,
                        });
                    }
                    else
                    {
                        medicationsToCreate.Add(new MedicationDetails
                        {
                            TradeName = tradeName,
                            ScientificName = item.ScientificName,
                            Usage = item.Usage,
                            Company = item.Company,
                            PharmaceuticalForm = pharmaceuticalForm,
                            Price = item.Price,
                            CountryId = countryId,
                            Agent = currentUser.Role == UserRole.Agent ? currentUser.AgentName : item.Agent,
                            AgentsPhoneNumber = currentUser.Role == UserRole.Agent ? currentUser.PhoneNumber : item.AgentsPhoneNumber,
                        });
                    }
                }

                if(medicationsToUpdate.Count > 0)
                {
                     this.genericRepository.UpdateDataEntity(medicationsToUpdate);
                }
                if (medicationsToCreate.Count > 0)
                {
                    this.genericRepository.CreateRangeDataEntity(medicationsToCreate);
                }

                this.genericRepository.CommitTransaction();

                return new ImportResponse();
            }
            catch (Exception)
            {
                this.genericRepository.RollbackTransaction();
                throw;
            }
        }
    }
}
