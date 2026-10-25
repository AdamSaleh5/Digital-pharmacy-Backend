using ClosedXML.Excel;
using CsvHelper.Configuration;
using Infrastructure.Excel.Constants;
using Infrastructure.Excel.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Reflection;

namespace Infrastructure.Excel.Import;

public sealed class ExcelImportProvider<THeaderMap, TExcelToEntityDto> : IExcelImportProvider<THeaderMap, TExcelToEntityDto>
    where THeaderMap : ClassMap, new()
    where TExcelToEntityDto : new()
{
    public Task<IEnumerable<TExcelToEntityDto>> ReadExcelFileAsync(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.FirstOrDefault()
                ?? throw new ExcelFileHeaderException(ExcelProviderErrorsConstants.InvalidFileHeader);
            var rows = worksheet.RowsUsed().ToList();
            if (rows.Count == 0) return Task.FromResult<IEnumerable<TExcelToEntityDto>>([]);

            var headers = rows[0].CellsUsed().ToDictionary(
                cell => NormalizeHeader(cell.GetString()),
                cell => cell.Address.ColumnNumber,
                StringComparer.OrdinalIgnoreCase);
            var propertyHeaders = GetPropertyHeaders();
            foreach (var header in propertyHeaders.Values)
                if (!headers.ContainsKey(NormalizeHeader(header)))
                    throw new ExcelFileHeaderException($"{ExcelProviderErrorsConstants.InvalidFileHeader} Missing column: {header}.");

            var records = new List<TExcelToEntityDto>();
            foreach (var row in rows.Skip(1))
            {
                if (row.CellsUsed().All(cell => cell.IsEmpty())) continue;
                var record = new TExcelToEntityDto();
                foreach (var (property, header) in propertyHeaders)
                {
                    var column = headers[NormalizeHeader(header)];
                    var value = row.Cell(column).GetFormattedString().Trim();
                    if (string.IsNullOrWhiteSpace(value)) continue;
                    try { property.SetValue(record, ConvertValue(value, property.PropertyType)); }
                    catch (Exception exception)
                    {
                        throw new ExcelFileDataFormatException(
                            $"{ExcelProviderErrorsConstants.InvalidFileDataFormat} Row: {row.RowNumber()}, Column: {header}, Value: '{value}', Expected type: {property.PropertyType.Name}.", exception);
                    }
                }
                var rowNumber = typeof(TExcelToEntityDto).GetProperty("RowNumber");
                rowNumber?.SetValue(record, row.RowNumber());
                records.Add(record);
            }
            return Task.FromResult<IEnumerable<TExcelToEntityDto>>(records);
        }
        catch (ExcelFileHeaderException) { throw; }
        catch (ExcelFileDataFormatException) { throw; }
        catch (Exception exception)
        {
            throw new Exception(ExcelProviderErrorsConstants.GeneralError, exception);
        }
    }

    private static Dictionary<PropertyInfo, string> GetPropertyHeaders()
    {
        var map = new THeaderMap();
        return map.MemberMaps
            .Where(memberMap => !memberMap.Data.Ignore && memberMap.Data.Member is PropertyInfo)
            .ToDictionary(
                memberMap => (PropertyInfo)memberMap.Data.Member!,
                memberMap => memberMap.Data.Names.FirstOrDefault() ?? memberMap.Data.Member!.Name);
    }

    private static object ConvertValue(string value, Type propertyType)
    {
        var type = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
        if (type == typeof(string)) return value;
        if (type == typeof(int) && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue)) return intValue;
        if (type == typeof(double) && (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var doubleValue) || double.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out doubleValue))) return doubleValue;
        if (type == typeof(decimal) && decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var decimalValue)) return decimalValue;
        if (type == typeof(DateTime) && DateTime.TryParse(value, CultureInfo.InvariantCulture, out var dateValue)) return dateValue;
        if (type == typeof(bool) && bool.TryParse(value, out var boolValue)) return boolValue;
        throw new FormatException();
    }

    private static string NormalizeHeader(string header) => header.Trim().Replace(" ", string.Empty).Replace("_", string.Empty).ToLowerInvariant();
}
