using HtmlToPdfFunctionApp.Utility;

namespace HtmlToPdfFunctionApp.Core;

public static class PdfCore
{
    public static async Task<string?> GeneratePdf(string htmlContent)
    {
        string? url = null;

        //generate pdf by selectpdf
        var stream = PdfService.HtmlToPdfStream(htmlContent);

        if (stream is not null)
        {
            var file = await Storage.UploadFromStream(stream, $"{Utilities.GetGuid()}.pdf", ConfigMgr.PdfStorageContainer()); //upload pdf to azure storage

            if (file.IsNotEmpty())
                url = file;
            else
                Logger.Warning("PDF Upload failed to Azure Storage!");
        }
        else
            Logger.Warning("PDF Generation failed from SelectPdf!");

        return url;
    }
}