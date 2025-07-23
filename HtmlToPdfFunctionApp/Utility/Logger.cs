using HtmlToPdfFunctionApp.Model.Base;

namespace HtmlToPdfFunctionApp.Utility;

public static class Logger
{
    private static readonly LogSettingsModel Config = ConfigMgr.Logger();

    public static void Information(string message, [CallerMemberName] string? caller = null) =>
        Log(message, LogEventLevel.Information, caller);

    public static void Information(string? reqStr, object? res, DateTime reqTime,
        [CallerMemberName] string? caller = null)
    {
        var resTime = DateTime.Now;

        _ = Task.Run(() =>
        {
            Log(
                $"Method: {caller}\r\nRequest Time: {reqTime:s}\r\nRequest: {reqStr.GetFirst(200)}\r\n\r\nResponse Time: {resTime:s}\r\nResponse: {JsonConvert.SerializeObject(res).GetLast(200)}",
                LogEventLevel.Information, caller);
        });
    }

    public static void Information(object? req, object? res, DateTime reqTime, [CallerMemberName] string? caller = null)
    {
        var resTime = DateTime.Now;

        var reqStr = JsonConvert.SerializeObject(req);
        reqStr = Config.WriteAll ? reqStr : reqStr.GetFirst(200);
        var resStr = JsonConvert.SerializeObject(res);
        resStr = Config.WriteAll ? resStr : resStr.GetLast(200);
        Log(
            $"Request Time: {reqTime:s}\r\nRequest: {reqStr}\r\n\r\nResponse Time: {resTime:s}\r\nResponse: ...{resStr}",
            LogEventLevel.Information, caller);
    }

    public static void Warning(string message, [CallerMemberName] string? caller = null) =>
        Log(message, LogEventLevel.Warning, caller);

    public static void Error(Exception exception, string? detail = null, [CallerMemberName] string? caller = null) =>
        Log(
            detail is null
                ? $"Exception error occurred: {exception.Message}"
                : $"Exception error occurred: {exception.Message}\r\nDetail: {detail}", LogEventLevel.Error, caller,
            exception);

    public static void Fatal(Exception exception, string? request = null, [CallerMemberName] string? caller = null) =>
        Log($"Exception error occurred: {exception.Message}\r\nSource: {request}\r\n", LogEventLevel.Fatal, caller,
            exception);

    public static void Verbose(string message, Exception? exception = null, [CallerMemberName] string? caller = null) =>
        Log(message, LogEventLevel.Verbose, caller, exception);

    public static void Debug(string message, Exception? exception = null, [CallerMemberName] string? caller = null) =>
        Log(message, LogEventLevel.Debug, caller, exception);

    private static void Log(string message, LogEventLevel logType, string? caller, Exception? exception = null)
    {
        _ = Task.Run(() =>
        {
            try
            {
                if (Config.WriteCloud)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                    ServicePointManager.ServerCertificateValidationCallback +=
                        (_, _, _, errors) => errors == SslPolicyErrors.None;

                    var config = new GoogleCloudLoggingSinkOptions
                    {
                        ProjectId = Config.GoogleProjectId,
                        GoogleCredentialJson = Encryption.Decrypt(Config.GoogleCredential),
                        LogName = Config.Environ,
                        Labels = { { "Source", Config.Source }, { "Method", caller ?? "" } }
                    };

                    using var log = new LoggerConfiguration()
                        .WriteTo.GoogleCloudLogging(config)
                        .MinimumLevel.Verbose()
                        .CreateLogger();

                    log.Write(logType, exception, $"Method: {caller}\r\n{message}");
                }
                else
                {
                    using var log = new LoggerConfiguration()
                        .WriteTo.File(string.Format(Config.Path, Config.Source, logType.ToString()), shared: true,
                            rollOnFileSizeLimit: true, fileSizeLimitBytes: Config.Size,
                            rollingInterval: RollingInterval.Day)
                        .MinimumLevel.Verbose()
                        .CreateLogger();

                    log.Write(logType, exception,
                        $"\r\nMethod: {caller}\r\n{message}\r\n_______________________________________");
                }
            }
            catch (Exception ex)
            {
                EventLog(message, ex);
            }
        });
    }

    private static void EventLog(string message, Exception? exception = null, [CallerMemberName] string? caller = null)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return;

        var evl = new EventLog
        {
            Source = Config.Source
        };
        evl.WriteEntry($"Error in: {MethodBase.GetCurrentMethod()?.Name}");
        evl.WriteEntry($"Method: {caller}\r\nMessage: {message}\r\n_______________________________________");
        evl.WriteEntry($"Error encountered in {caller}. Error: {exception?.Message}\r\nDetail: {exception?.StackTrace}",
            EventLogEntryType.Error);
    }
}