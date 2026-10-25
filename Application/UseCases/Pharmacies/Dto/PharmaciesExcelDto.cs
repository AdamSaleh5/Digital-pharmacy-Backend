using CsvHelper.Configuration.Attributes;

namespace Application.UseCases.Pharmacies.Dto;

public sealed class PharmaciesExcelDto
{
    public int RowNumber { get; set; }

    [Name("Name")]
    public string? Name { get; set; }

    [Name("Address")]
    public string? Address { get; set; }

    [Name("ManagerName")]
    public string? ManagerName { get; set; }

    [Name("PhoneNumber")]
    public string? PhoneNumber { get; set; }
}
