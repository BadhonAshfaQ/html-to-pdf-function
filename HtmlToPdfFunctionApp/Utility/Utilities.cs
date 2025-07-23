using HtmlToPdfFunctionApp.Model.Base;

namespace HtmlToPdfFunctionApp.Utility;

public static class Utilities
{
    #region Getters

    public static string GetGuid(string? format = "N") => Guid.NewGuid().ToString(format);

    public static string GetGuidFirstPart() => GetGuid(null).Split('-')[0];

    public static string GetBaseDirectory() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";

    public static string GetTime() => $"{DateTime.Now:yyyy-MM-ddTHH:mm:ss}";

    public static string SerializeObject(this object? obj)
    {
        string res;

        try
        {
            res = obj is not null ? JsonConvert.SerializeObject(obj) : "";
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            res = "";
        }

        return res;
    }

    public static string GetFirst(this string? str, int len) => str is null ? "" : str[..Math.Min(str.Length, len)];

    public static string GetLast(this string? str, int len) => str is null ? "" : str[^Math.Min(str.Length, len)..];

   


    #endregion Getters

    #region Validators

    public static bool IsNumeric(this string? str) => str is not null && Regex.IsMatch(str, "^[0-9]+$", RegexOptions.None, TimeSpan.FromSeconds(60));

    public static bool IsSuccess(this HttpStatusCode code) => code is HttpStatusCode.OK or HttpStatusCode.Created or HttpStatusCode.Accepted;

    public static bool IsException(this HttpStatusCode code) => code is HttpStatusCode.InternalServerError;

    public static bool IsPartial(this HttpStatusCode code) => code is HttpStatusCode.PartialContent;

    public static bool IsBadGateWay(this HttpStatusCode code) => code is HttpStatusCode.BadGateway or HttpStatusCode.GatewayTimeout;

    public static bool IsValidJson(this string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        json = json.Trim();

        if ((!json.StartsWith('{') || !json.EndsWith('}')) && (!json.StartsWith('[') || !json.EndsWith(']')))
            return false;

        try
        {
            _ = JToken.Parse(json);
            return true;
        }
        catch (JsonReaderException jex)
        {
            Logger.Error(jex);
            return false;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return false;
        }
    }

    public static bool IsHtml(this string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        return !doc.ParseErrors.Any();
    }

    public static bool IsEmpty(this string? val) => string.IsNullOrEmpty(val);

    public static bool IsAcEmpty(this string? val) => val.IsEmpty() || val?.Trim().ToUpperInvariant() is "NULL";

    public static bool IsNotEmpty(this string? val) => !string.IsNullOrEmpty(val);

    public static bool IsNotEmpty(this StringValues val) => !string.IsNullOrEmpty(val);

    public static bool IsNotEmpty(this RedisValue val) => !string.IsNullOrEmpty(val);

    public static bool AnyCount<T>(this IEnumerable<T>? items) => items?.Any() is true;

    public static bool AnyCount<T>(this List<T>? items) => items?.Count > 0;

    public static bool AnyCount<T1, T2>(this Dictionary<T1, T2> items) where T1 : notnull => items.Count > 0;

   
    public static async Task<(bool IsValid, string? JsonPayload)> IsValidTypeFormRequest(this HttpRequest req)
    {
        var json = await new StreamReader(req.Body).ReadToEndAsync();

        if (json.IsValidJson()
            && req.Headers.TryGetValue("Typeform-Signature", out var tfSignature)
            && json.IsValidTypeFormSignature(tfSignature.ToString()))
            return (true, json);

        return (false, null);
    }

    public static bool IsValidTypeFormSignature(this string payload, string signature)
    {
        var secret = "";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var computedSignature = $"sha256={Convert.ToBase64String(hashBytes)}";

        return computedSignature.Equals(signature, StringComparison.InvariantCultureIgnoreCase);
    }

   
    public static bool IsBadRequest(this ushort resp) => resp == ResponseMgr.BadRequest400;

    public static bool IsAccepted(this ushort resp) => resp == ResponseMgr.Accepted202;

    public static bool IsInsufficientFund(this ushort resp) => resp == ResponseMgr.PreCondition428;

    public static bool IsNotNull(this object? obj) => obj?.GetType().GetProperties()
        .Where(pi => pi.PropertyType == typeof(string))
        .Select(pi => Convert.ToString(pi.GetValue(obj)))
        .Any(v => !string.IsNullOrEmpty(v)) is true;

    public static bool IsOk(this ushort resp) => resp == ResponseMgr.Ok200;

    public static bool IsGuid(this string? guid) => guid?.Length is 32 && Guid.TryParse(guid, out _);

    public static bool IsIpAddress(this string? ip) => ip?.Length is >= 7 and <= 15 && ip.Split('.').Length is 4 && IPAddress.TryParse(ip, out _);


    public static bool IsEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            email = Regex.Replace(email, "(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

            static string DomainMapper(Match match)
            {
                var idn = new IdnMapping();

                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }


    public static bool EqualsIgnoreCase(this string? val1, string? val2) => val1?.Equals(val2, StringComparison.InvariantCultureIgnoreCase) is true;

  
    #endregion Validators

    #region Converters

    public static long ToLong(this string value)
    {
        _ = long.TryParse(value, out var longVal);
        return longVal;
    }

    public static string ToDollar(this decimal value) => $"${value:##,###}";

    public static string ToDollar(this int value) => $"${value:##,###}";

    public static string ToDollarCents(this decimal value) => $"${value:##,###.00}";

    public static string ToShortDollar(this decimal num)
    {
        var zeros = $"{num:####}".Length;
        var letters = new List<(ushort, string)>
            {
                (15, "Q"),
                (12, "T"),
                (9, "B"),
                (6, "M"),
                (3, "K")
            };

        for (var i = 0; i < letters.Count; i++)
            if (zeros > letters[i].Item1)
                return $"${num / Convert.ToDecimal(Math.Pow(10, letters[i].Item1)):F1}{letters[i].Item2}";

        return $"${num:F1}";
    }

    public static string ToFormatShortDollar(this decimal num) => num.ToShortDollar().Replace("$-", "-$");

    public static string ToTitleCase(this string s) => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());

    public static DateTime ToDateTimeKind(this int year, int month, int day) => new(year, month, day, 0, 0, 0, DateTimeKind.Unspecified);

    public static DateTime ToDateTime(this string planInputDate) => DateTime.TryParseExact($"01/{planInputDate}", "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue)
            ? dateValue
            : 1.ToDateTimeKind(1, 1);

    public static User? ToUser(this string? userId)
    {
        User? user = null;

        if (userId.IsEmpty())
            return user;

        var arr = userId?.UserIdToArray();

        if (arr?.Length is 8 or 9)
            user = new User
            {
                Email = arr[0],
                RoleId = arr[1],
                SessionId = arr[2],
                FirstName = arr[3],
                LastName = arr[4],
                Picture = arr[5],
                AgencyId = arr[6],
                EnterpriseId = arr[7],
                ClientId = arr.Length is 9 ? arr[8] : null
            };

        return user;
    }

    public static string[] UserIdToArray(this string userId) => Encryption.Decrypt(userId).Split('|');

    public static TimeSpan ToTimeSpan(this double minutes) => TimeSpan.FromMinutes(minutes);

    public static string? ToIpAddress(this FunctionContext context)
    {
        var remoteIp = context.GetHttpContext()?.Connection.RemoteIpAddress;

        if (remoteIp?.IsIPv4MappedToIPv6 is true)
            remoteIp = remoteIp.MapToIPv4();

        if (remoteIp is null)
            Logger.Warning("IP Address not detected.");

        return remoteIp?.ToString();
    }

    public static ushort? ToUShort(this string? value) => ushort.TryParse(value, out var result) ? result : null;

    public static async Task LogIp(this FunctionContext cx, [CallerMemberName] string? caller = null) => await Task.Run(() =>
    {
        Logger.Debug($"RemoteIP:{cx.ToIpAddress()}", caller: caller);
    });

    

    public static (string, string) ToOnceHubSignature(this HttpRequest req)
    {
        var res = ("", "");

        try
        {
            if (req.Headers.TryGetValue("OnceHub-Signature", out var header))
            {
                var parameters = header.ToString()
                    .Split(',')
                    .Select(part => part.Split('='))
                    .Where(keyValue => keyValue.Length is 2)
                    .ToDictionary(keyValue => keyValue[0].Trim(), keyValue => keyValue[1].Trim());

                res = (parameters.GetValueOrDefault("t", string.Empty), parameters.GetValueOrDefault("s", string.Empty));
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error parsing OnceHub Signature from request header.");
        }

        return res;
    }

    public static DateTime? ToSydneyTime(this DateTime dt) => TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"));

    public static string ToUniqueId(this string? email) => email?.ToLowerInvariant().ToSha512() ?? "";

    public static long ToId(this string? profileId) => Encryption.Decrypt(Encryption.Base64Decode(profileId)).ToLong();

    public static string ToEncodedProfileId(this long id) => Encryption.Base64Encode(Encryption.Encrypt($"{id}"));

    public static DateTime ToNextRunTime(this string cron6)
    {
        //convert 6 part cron to 5 part cron by removing the first part - the seconds part
        var cron5 = string.Join(" ", cron6.Split(' ').Skip(1));
        var schedule = CrontabSchedule.Parse(cron5);
        return schedule.GetNextOccurrence(DateTime.Now);
    }

    
   
    public static IActionResult ToFileActionResult(this byte[] fileContent, string fileName, string contentType) => new FileContentResult(fileContent, contentType)
    {
        FileDownloadName = fileName
    };

    public static List<string> ToSanitizedEmails(this List<string> atEmails)
    {
        if (!atEmails.AnyCount())
            return [];

        var result = new ConcurrentBag<string>();
        const string splitterPattern = @"[\s,;&]+"; // delimiters: space, comma, semicolon, ampersand

        Parallel.ForEach(atEmails, email =>
        {
            if (email.IsEmpty())
                return;

            Regex.Split(email, splitterPattern)
                 .Select(e => e.ToFormatEmail())
                 .Where(e => e.IsNotEmpty() && e.IsEmail())
                 .ToList()
                 .ForEach(validEmail => result.Add(validEmail));
        });

        return [.. result];
    }

    private static string ToFormatEmail(this string email) => email.IsEmpty() ? "" : email.Trim().TrimEnd('.', ',', ';').ToLower();

  
    #endregion Converters
}