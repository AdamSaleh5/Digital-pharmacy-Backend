namespace Infrastructure.Excel.Exceptions;

public sealed class ExcelFileHeaderException : Exception
{
    public ExcelFileHeaderException(string message) : base(message) { }
}
