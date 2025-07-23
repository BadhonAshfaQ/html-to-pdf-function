namespace HtmlToPdfFunctionApp.Utility;

public static class FileService
{
    public static string? ReadFile(string filePath)
    {
        try
        {
            return File.ReadAllLines(filePath).FirstOrDefault();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error occurred on reading file for already sent items");
        }

        return string.Empty;
    }

    public static void WriteToFile(string filePath, string? text)
    {
        try
        {
            File.WriteAllText(filePath, text);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error occurred on writing file for SetNextMessageId.");
        }
    }
}