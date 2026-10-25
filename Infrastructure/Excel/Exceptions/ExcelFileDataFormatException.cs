namespace Infrastructure.Excel.Exceptions;

public sealed class ExcelFileDataFormatException : Exception
{
    public ExcelFileDataFormatException(string message, Exception? innerException = null) : base(message, innerException) { }
}
