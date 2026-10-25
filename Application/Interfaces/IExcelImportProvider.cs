using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IExcelImportProvider<THeaderMap, TExcelToEntityDto>
    where THeaderMap : ClassMap
{
    Task<IEnumerable<TExcelToEntityDto>> ReadExcelFileAsync(IFormFile file);
}
