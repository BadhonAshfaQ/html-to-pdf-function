namespace HtmlToPdfFunctionApp.Utility;

public static class PdfService
{
    public static Stream? HtmlToPdfStream(string htmlContent)
    {
        Stream? res = null;

        try
        {
            var converter = new HtmlToPdf();

            // convert HTML to PDF
            var doc = converter.ConvertHtmlString(htmlContent);

            // get pdf document bytes
            var bytes = doc.Save();

            doc.Close();

            //returning byte stream
            res = new MemoryStream(bytes);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to generate PDF from HTML. Please investigate!");
        }

        return res;
    }
}