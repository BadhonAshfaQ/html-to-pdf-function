namespace HtmlToPdfFunctionApp.Utility;

public static class ResponseMgr
{
    public static readonly ushort Processing102 = StatusCodes.Status102Processing;

    public static readonly ushort Created201 = StatusCodes.Status201Created;
    public static readonly ushort Ok200 = StatusCodes.Status200OK;
    public static readonly ushort Accepted202 = StatusCodes.Status202Accepted;
    public static readonly ushort NoContent204 = StatusCodes.Status204NoContent;
    public static readonly ushort PartialContent206 = StatusCodes.Status206PartialContent;

    public static readonly ushort BadRequest400 = StatusCodes.Status400BadRequest;
    public static readonly ushort Unauthorized401 = StatusCodes.Status401Unauthorized;
    public static readonly ushort Forbidden403 = StatusCodes.Status403Forbidden;
    public static readonly ushort NotFound404 = StatusCodes.Status404NotFound;
    public static readonly ushort Conflict409 = StatusCodes.Status409Conflict;
    public static readonly ushort PreCondition428 = StatusCodes.Status428PreconditionRequired;
    public static readonly ushort InvalidFormat422 = StatusCodes.Status422UnprocessableEntity;

    public static readonly ushort Exception500 = StatusCodes.Status500InternalServerError;
    public static readonly ushort BadGateway502 = StatusCodes.Status502BadGateway;
    public static readonly ushort Unavailable503 = StatusCodes.Status503ServiceUnavailable;
    public static readonly ushort GatewayTimeout504 = StatusCodes.Status504GatewayTimeout;
}