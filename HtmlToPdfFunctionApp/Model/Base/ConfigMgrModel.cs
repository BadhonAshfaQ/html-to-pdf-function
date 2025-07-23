namespace HtmlToPdfFunctionApp.Model.Base;

public class LogSettingsModel
{
#nullable disable
    public string Source { get; set; }
    public string Path { get; set; }
    public string Temp { get; set; }
    public string Environ { get; set; }
    public long Size { get; set; }
    public bool WriteCloud { get; set; }
    public bool WriteAll { get; set; }
    public string GoogleProjectId { get; set; }
    public string GoogleCredential { get; set; }
}

public class EmailLoggerConfigModel
{
    public string From { get; set; }
    public string To { get; set; }
    public string Cc { get; set; }
    public string Bc { get; set; }
    public string Subject { get; set; }
    public string Template { get; set; }
    public string CssTemplate { get; set; }
}

public class KeyIvBase
{
    public string Key { get; set; }
    public string Iv { get; set; }
    public ushort Base { get; set; } = 64;
}

public class TokenSettingsModel
{
    public byte[] EncodedSecret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public double ExpiryMin { get; set; }
}

public class EmailConfigModel
{
#nullable enable
    public string? From { get; set; }
    public string? To { get; set; }
    public string? Cc { get; set; }
    public string? Bc { get; set; }
    public string? Subject { get; set; }
    public string? Template { get; set; }
    public string? CssTemplate { get; set; }

#nullable disable
}

public class KeyValueStr
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class KeyValueDec
{
#nullable enable
    public string? Key { get; set; }
    public decimal? Value { get; set; }
}

public class LatLng
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

public class CometChatConfigModel
{
#nullable disable
    public string BaseUrl { get; set; }
    public bool Unread { get; set; } = true;
    public bool Count { get; set; } = true;
    public int PerPage { get; set; } = 1000;
    public int Limit { get; set; } = 1000;
}

public class ServiceAccountModel
{
    public string Email { get; set; }
    public ushort PgpRoleId { get; set; }
    public ushort PpfRoleId { get; set; }
    public string SessionId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Picture { get; set; }
    public ushort PgpAppId { get; set; }
    public string UserAgent { get; set; }
    public string EnterpriseId { get; set; }
    public string AgencyId { get; set; }
    public string IpAddress { get; set; }
}

public class JwtToken
{
    public string Token { get; set; }
    public string Expiry { get; set; }
}


public class SlackConfigModel
{
    public string Username { get; set; }
    public SlackChannels Channels { get; set; }
    public string Template { get; set; }
    public string AirTableValuationTemplate { get; set; }
    public string PgpValuationTemplate { get; set; }
    public string SuburbReportTemplate { get; set; }
    public string RoleConversionTemplate { get; set; }
    public string Green { get; set; }
    public string Red { get; set; }
    public string Yellow { get; set; }
    public string EncodedWebHookUrl { get; set; }
}

public class SlackChannels
{
    public string PropertyResearch { get; set; }
    public string PortfolioPlanner { get; set; }
    public string MarketingPathFinder { get; set; }
}
public class StripeSlackMessage
{
    public string EventType { get; set; }
    public string Message { get; set; }
}

public class DdFxSlackMessage
{
    public DateTime NextTriggerRun { get; set; }
}


public class AgencyLeadInviteConfigModel
{
    public string Email { get; set; }
    public string AgencyName { get; set; }
    public string AgencyId { get; set; }
    public ushort RoleId { get; set; }
    public ushort InviteExpiration { get; set; }
}
