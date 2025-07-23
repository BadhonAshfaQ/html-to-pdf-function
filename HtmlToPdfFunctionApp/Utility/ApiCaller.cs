using HtmlToPdfFunctionApp.Model.Base;

namespace HtmlToPdfFunctionApp.Utility;

public static class ApiCaller
{
    public static async Task<ApiResponse?> RestClientAsync(ApiRequest req, bool retry = true, [CallerMemberName] string? caller = null)
    {
        ApiResponse? res;

        try
        {
            var client = new RestClient(req.BaseUrl ?? "");
            var request = new RestRequest(req.Resource, req.Method);

            Parallel.ForEach(req.Headers, header =>
            {
                var (key, value) = header;
                lock (request)
                    _ = request.AddHeader(key, value);
            });

            if (req.Parameters.AnyCount())
                Parallel.ForEach(req.Parameters, parameter =>
                {
                    var (key, value) = parameter;
                    lock (request)
                        _ = request.AddParameter(key, value);
                });

            if (req.Body is not null)
                _ = request.AddJsonBody(req.Body);

            Encryption.BypassCertificateError();
            var response = await client.ExecuteAsync(request);

            if (response.StatusCode.IsBadGateWay() && retry)
            {
                await Task.Delay(500);
                return await RestClientAsync(req, false);
            }

            _ = Task.Run(() =>
            {
                try
                {
                    Logger.Verbose($"HTTP Caller: {caller}\r\nBase URL: {req.BaseUrl}\r\nRoute URL: {req.Resource}\r\nMethod: {req.Method}\r\n\r\nREQUEST\r\nBody: {req.Body?.SerializeObject()}\r\nParameters: {req.Parameters.SerializeObject()}\r\n\r\nRESPONSE\r\nStatusCode: {response.StatusCode}\r\nBody: {response.Content}\r\nErrorMessage: {response.ErrorMessage}\r\nErrorException: {response.ErrorException}");
                }
                catch (Exception ey)
                {
                    Logger.Error(ey, $"Exception while writing HTTP call log: {ey.Message}");
                }
            });

            res = new ApiResponse
            {
                Content = response.Content,
                StatusCode = response.StatusCode,
                Header = response.Headers
            };
        }
        catch (Exception ex)
        {
            Logger.Error(ex, detail: JsonConvert.SerializeObject(req));
            res = null;
        }

        return res;
    }

    public static async Task<ApiResponse?> RestClientAsync(RestClient client, ApiRequest req, bool retry = true, [CallerMemberName] string? caller = null)
    {
        ApiResponse? res;

        try
        {
            var request = new RestRequest(req.Resource, req.Method);

            Parallel.ForEach(req.Headers, header =>
            {
                var (key, value) = header;
                lock (request)
                    _ = request.AddHeader(key, value);
            });

            if (req.Parameters.AnyCount())
                Parallel.ForEach(req.Parameters, parameter =>
                {
                    var (key, value) = parameter;
                    lock (request)
                        _ = request.AddParameter(key, value);
                });

            if (req.Body is not null)
                _ = request.AddJsonBody(req.Body);

            Encryption.BypassCertificateError();
            var response = await client.ExecuteAsync(request);

            if (response.StatusCode.IsBadGateWay() && retry)
            {
                await Task.Delay(500);
                return await RestClientAsync(client, req, false, caller);
            }

            _ = Task.Run(() =>
            {
                try
                {
                    Logger.Verbose($"HTTP Caller: {caller}\r\nBase URL: {req.BaseUrl}\r\nRoute URL: {req.Resource}\r\nMethod: {req.Method}\r\n\r\nREQUEST\r\nBody: {req.Body?.SerializeObject()}\r\nParameters: {req.Parameters.SerializeObject()}\r\n\r\nRESPONSE\r\nStatusCode: {response.StatusCode}\r\nBody: {response.Content}\r\nErrorMessage: {response.ErrorMessage}\r\nErrorException: {response.ErrorException}");
                }
                catch (Exception ey)
                {
                    Logger.Error(ey, $"Exception while writing HTTP call log: {ey.Message}");
                }
            });

            res = new ApiResponse
            {
                Content = response.Content,
                StatusCode = response.StatusCode,
                Header = response.Headers
            };
        }
        catch (Exception ex)
        {
            Logger.Error(ex, detail: JsonConvert.SerializeObject(req));
            res = null;
        }

        return res;
    }

    public static async Task<bool> SendGridEmail(MailModel mail)
    {
        var res = false;

        try
        {
            var client = new SendGridClient(ConfigMgr.SendGridApiKey());
            var msg = new SendGridMessage
            {
                From = new EmailAddress(mail.From.Item1, mail.From.Item2),
                Subject = mail.Subject,
                PlainTextContent = mail.PlainContent,
                HtmlContent = mail.HtmlContent
            };

            if (mail.To.Item1.IsNotEmpty())
                msg.AddTo(new EmailAddress(mail.To.Item1, mail.To.Item2));
            if (mail.Cc.Item1.IsNotEmpty())
                msg.AddCc(new EmailAddress(mail.Cc.Item1, mail.Cc.Item2));
            if (mail.Bcc.Item1.IsNotEmpty())
                msg.AddBcc(new EmailAddress(mail.Bcc.Item1, mail.Bcc.Item2));

            var resp = await client.SendEmailAsync(msg).ConfigureAwait(true);

            if (resp.StatusCode is HttpStatusCode.Accepted or HttpStatusCode.OK)
                res = true;
            else
                Logger.Warning($"Email possibly not delivered. \r\nResponse StatusCode: {resp.StatusCode}\r\nResponse Header: {JsonConvert.SerializeObject(resp.Headers)}\r\nResponse Body: {JsonConvert.SerializeObject(resp.Body)}");
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }

        return res;
    }
}