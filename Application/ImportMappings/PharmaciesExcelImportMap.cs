using Application.UseCases.Pharmacies.Dto;
using CsvHelper.Configuration;
using System.Globalization;

namespace Application.ImportMappings;

public sealed class PharmaciesExcelImportMap : ClassMap<PharmaciesExcelDto>
{
    public PharmaciesExcelImportMap()
    {
        AutoMap(CultureInfo.InvariantCulture);
        Map(m => m.RowNumber).Ignore();
        Map(m => m.Name).Name("Name");
        Map(m => m.Address).Name("Address");
        Map(m => m.ManagerName).Name("ManagerName");
        Map(m => m.PhoneNumber).Name("PhoneNumber");
    }
}
