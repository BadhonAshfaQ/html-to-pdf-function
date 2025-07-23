using HtmlToPdfFunctionApp.Core;
using HtmlToPdfFunctionApp.Model.Base;
using HtmlToPdfFunctionApp.Utility;
using HttpResponse = HtmlToPdfFunctionApp.Model.Base.HttpResponse;

namespace HtmlToPdfFunctionApp;

public static class Function
{
    #region PDF Generation

    /// <summary>
    /// This function is HTTP-triggered for generating PDF document from HTML content.
    /// </summary>
    /// <param name="req"></param>
    /// <param name="cx"></param>
    /// <returns></returns>
    [Function("PdfConverter")]
    public static async Task<IActionResult> PdfConverter([HttpTrigger(AuthorizationLevel.Function, "POST", Route = "GeneratePdf")] HttpRequest req, FunctionContext cx)
    {
        var reqTime = DateTime.Now;
        HttpResponse res;
        const string app = "pgs";

        if (ConfigMgr.PdfGenServiceIsActive())
        {
            var pdfReq = WebUtility.UrlDecode(
                Encryption.DecryptEventMessage(await new StreamReader(req.Body).ReadToEndAsync(), app));

            if (pdfReq.IsNotEmpty())
            {
                if (pdfReq.IsHtml())
                {
                    try
                    {
                        var url = await PdfCore.GeneratePdf(pdfReq);

                        if (url?.IsNotEmpty() is true)
                            res = new HttpResponse
                            {
                                Payload = new PdfConverterRes
                                {
                                    FileUrl = url,
                                    ResponseCode = ResponseMgr.Ok200,
                                    ResponseMessage = "Success."
                                },
                                StatusCode = ResponseMgr.Ok200
                            };
                        else
                            res = new HttpResponse
                            {
                                Payload = new BaseRes
                                {
                                    ResponseCode = ResponseMgr.BadGateway502,
                                    ResponseMessage = "Error with some dependencies."
                                },
                                StatusCode = ResponseMgr.BadGateway502
                            };
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, $"Exception occurred: {ex.Message}");
                        res = new HttpResponse
                        {
                            Payload = new BaseRes
                            {
                                ResponseCode = ResponseMgr.Exception500,
                                ResponseMessage = "Something didn't worked as planned."
                            },
                            StatusCode = ResponseMgr.Exception500
                        };
                    }
                }
                else
                    res = new HttpResponse
                    {
                        Payload = new BaseRes
                        {
                            ResponseCode = ResponseMgr.InvalidFormat422,
                            ResponseMessage = "Report HTML format is not valid."
                        },
                        StatusCode = ResponseMgr.BadRequest400
                    };
            }
            else
                res = new HttpResponse
                {
                    Payload = new BaseRes
                    {
                        ResponseCode = ResponseMgr.BadRequest400,
                        ResponseMessage = "PDF Conversion request cannot be blank."
                    },
                    StatusCode = ResponseMgr.BadRequest400
                };

            Logger.Information(pdfReq, res.Payload, reqTime);
            _ = cx.LogIp();
        }
        else
            res = new HttpResponse
            {
                Payload = new BaseRes
                {
                    ResponseCode = ResponseMgr.Unavailable503,
                    ResponseMessage = "PDF Generation Service is not active."
                },
                StatusCode = ResponseMgr.Unavailable503
            };

        return new ObjectResult(Encryption.EncryptResponse(res.Payload, app))
        {
            StatusCode = res.StatusCode
        };
    }

    #endregion PDF Generation

  
}