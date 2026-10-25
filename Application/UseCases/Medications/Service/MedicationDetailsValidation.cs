using Application.Interfaces;
using Application.Models;
using Application.UseCases.Medications.Dto;
using Application.UseCases.Medications.Request;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Medications.Service
{
    public class MedicationDetailsValidation : IMedicationDetailsValidation
    {
        public MedicationDetailsValidation() { }
        public async Task<ValidationResult<MedicationDetailsRequest>> ValidateImportedMedicationDetailsAsync(List<MedicationDetailsExcelDto> records)
        {
            var result = new ValidationResult<MedicationDetailsRequest> { Status = true };
            var processedMedicationDetails = new List<MedicationDetailsExcelDto>();
            var rowNumber = 2; // Start from 2 as 1 is header

            foreach (var csvMedicationDetails in records)
            {
                if (string.IsNullOrEmpty(csvMedicationDetails.TradeName) && string.IsNullOrEmpty(csvMedicationDetails.ScientificName))
                {
                    continue; // Skip empty rows
                }


                processedMedicationDetails.Add(csvMedicationDetails);

                result.Records.Add(new MedicationDetailsRequest
                {
                    TradeName = csvMedicationDetails.TradeName,
                    ScientificName = csvMedicationDetails.ScientificName,
                    Usage = csvMedicationDetails.Usage,
                    Company = csvMedicationDetails.Company,
                    Agent = csvMedicationDetails.Agent,
                    PharmaceuticalForm = csvMedicationDetails.PharmaceuticalForm,
                    Price = csvMedicationDetails.Price,
                    AgentsPhoneNumber = csvMedicationDetails.AgentsPhoneNumber,
                });

                rowNumber++;
            }

            return result;
        }
    }
}
