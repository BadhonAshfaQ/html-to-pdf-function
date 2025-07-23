using HtmlToPdfFunctionApp.Model.Base;

namespace HtmlToPdfFunctionApp.Model.Dto;

public class PdfConverterRes : BaseRes
{
    public string? FileUrl { get; set; }
}