using Application.Models;
using Application.UseCases.Medications.Dto;
using Application.UseCases.Medications.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMedicationDetailsValidation
    {
        Task<ValidationResult<MedicationDetailsRequest>> ValidateImportedMedicationDetailsAsync(List<MedicationDetailsExcelDto> records);
    }
}
