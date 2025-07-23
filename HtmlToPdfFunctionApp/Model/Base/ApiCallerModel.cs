namespace HtmlToPdfFunctionApp.Model.Base;

public class MailModel
{
    public (string, string) From { get; set; }
    public (string, string) To { get; set; }
    public (string, string) Cc { get; set; }
    public (string, string) Bcc { get; set; }
    public string? Subject { get; set; }
    public string? PlainContent { get; set; }
    public string? HtmlContent { get; set; }
#nullable disable
}

public class ApiRequest
{
    public Method Method { get; set; }
    public Dictionary<string, string> Headers { get; set; } = [];
    public Dictionary<string, string> Parameters { get; set; } = [];
    public string BaseUrl { get; set; }
#nullable enable
    public object? Body { get; set; }
    public string? Resource { get; set; }
}

public class ApiResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string? Content { get; set; }
    public IEnumerable<dynamic>? Header { get; set; }
}


