namespace HtmlToPdfFunctionApp.Model.Base;

public class HttpResponse
{
    public object? Payload { get; set; }
    public ushort StatusCode { get; set; }
}

public class BaseRes
{
    public ushort ResponseCode { get; set; }
#nullable disable
    public string ResponseMessage { get; set; }
}

public class BaseReq
{
    public string UserId { get; set; }
#nullable enable
    internal string? IpAddress { get; set; }
    public string? RequestType { get; set; } = "Automated";
#nullable disable
}

public class BareReq
{
    internal string IpAddress { get; set; }
    internal string SessionId { get; set; }
}

public class User
{
    public string Email { get; set; }
    public string RoleId { get; set; }
    public string SessionId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Picture { get; set; }
#nullable enable
    public string? AgencyId { get; set; }
    public string? EnterpriseId { get; set; }
    public string? ClientId { get; set; }
}