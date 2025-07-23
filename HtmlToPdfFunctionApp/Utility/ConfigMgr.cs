namespace HtmlToPdfFunctionApp.Utility;

public static class ConfigMgr
{
    private static readonly IConfiguration Conf = Program.Configuration();

    #region KeyVault

    private static readonly string[] KvApp1Jwt = Encryption.Decrypt(Conf["KvApp1JwtSecretIssuerExpiryAudience"] ?? "").Split('|');
    private static readonly string[] KvApp2Jwt = Encryption.Decrypt(Conf["KvApp2AgentJwtSecretIssuerExpiryAudience"] ?? "").Split('|');

    public static TokenSettingsModel TokenSettings(string app) => app switch
    {
        "app1" => new TokenSettingsModel
        {
            EncodedSecret = Encoding.UTF8.GetBytes(KvApp1Jwt[0]),
            Issuer = KvApp1Jwt[1],
            ExpiryMin = Convert.ToDouble(KvApp1Jwt[2]),
            Audience = KvApp1Jwt[3]
        },
        "app2" => new TokenSettingsModel
        {
            EncodedSecret = Encoding.UTF8.GetBytes(KvApp2Jwt[0]),
            Issuer = KvApp2Jwt[1],
            ExpiryMin = Convert.ToDouble(KvApp2Jwt[2]),
            Audience = KvApp2Jwt[3]
        },
        _ => throw new NotImplementedException()
    };

    public static string SqlDbConnection() => Encryption.Decrypt(Conf["KvSqlDbConnection"]);

   
    public static string SendGridApiKey() => Conf["KvSendGridApiKey"] ?? "";

    public static KeyIvBase AesKeyIv(string app) => app switch
    {
        "app1" => new KeyIvBase
        {
            Key = Encryption.Decrypt(Conf["KvApp1AesKey"]),
            Iv = Encryption.Decrypt(Conf["KvApp1AesIv"])
        },
        "pdf" => new KeyIvBase
        {
            Key = Encryption.Decrypt(Conf["KvPdfGenServiceKey"]),
            Iv = Encryption.Decrypt(Conf["KvPdfGenServiceIv"])
        },
        _ => throw new NotImplementedException()
    };

  
    public static string StorageConnection() => Encryption.Decrypt(Conf["KvStorageConnection"]);

   
    


    #endregion KeyVault

    public static bool BypassCertificateError() => Conf.GetValue<bool>("Security:BypassCertificateError");

    public static LogSettingsModel Logger() => Conf.GetSection("Logger").Get<LogSettingsModel>() ?? new LogSettingsModel();

 

    public static ServiceAccountModel ServiceAccount()
    {
        var svc = Conf.GetSection("Pgp:ServiceAccount").Get<ServiceAccountModel>() ?? new ServiceAccountModel();
        svc.SessionId = Utilities.GetGuid();
        return svc;
    }

    #region PdfGenService

    public static bool PdfGenServiceIsActive() => Conf.GetValue<bool>("PdfGenService:IsActive");

    public static string PdfStorageContainer() => Conf["PdfGenService:StorageContainer"] ?? "";

    #endregion PdfGenService

    
    
}