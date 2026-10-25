using System.ComponentModel.DataAnnotations;

namespace Application.UseCases.Medications.Request;

public class MedicationDetailsRequest
{
    public string TradeName { get; set; } 

    public string ScientificName { get; set; } 

    public string Usage { get; set; } 

    public string Company { get; set; } 

    public string Agent { get; set; } 

    public string AgentsPhoneNumber { get; set; }

    public string PharmaceuticalForm { get; set; }

    public double Price { get; set; }

    public int CountryId { get; set; }
}
