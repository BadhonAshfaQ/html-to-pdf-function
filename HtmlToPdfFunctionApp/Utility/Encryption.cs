namespace HtmlToPdfFunctionApp.Utility;

public static class Encryption
{
    public static string Encrypt(string plain) => plain.IsEmpty() ? "" : DashDotLib.Encrypt(plain);

    public static string Encrypt(string plain, string pubKey) => plain.IsEmpty() ? "" : DashDotLib.Encrypt(plain, pubKey);

    public static string Decrypt(string? cipher) => cipher?.IsEmpty() is true ? "" : DashDotLib.Decrypt(cipher);

    public static string Decrypt(string cipher, string priKey) => cipher.IsEmpty() ? "" : DashDotLib.Decrypt(cipher, priKey);

    public static string Base64Encode(string plainText) => Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));

    public static string Base64Decode(string? base64EncodedData)
    {
        try
        {
            if (base64EncodedData?.IsEmpty() is true)
                return "";

            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData ?? "");
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return "";
        }
    }

    public static string EncryptAes(string plainTxt, string key, string iv, ushort @base) => DashDotLib.EncryptAesWithKeyIv(plainTxt, key, iv, @base);

    public static string DecryptAes(string cipherTxt, string key, string iv, ushort @base) => DashDotLib.DecryptAesWithKeyIv(cipherTxt, key, iv, @base);

    public static string EncryptResponse(object res, string app)
    {
        var aes = ConfigMgr.AesKeyIv(app);

        return EncryptAes(JsonConvert.SerializeObject(res, new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        }), aes.Key, aes.Iv, aes.Base);
    }

    public static string EncryptResponse(string res, string app)
    {
        var aes = ConfigMgr.AesKeyIv(app);
        return EncryptAes(res, aes.Key, aes.Iv, aes.Base);
    }

    public static T? DecryptRequest<T>(string req, string app)
    {
        try
        {
            var aes = ConfigMgr.AesKeyIv(app);
            return JsonConvert.DeserializeObject<T>(DecryptAes(req, aes.Key, aes.Iv, aes.Base));
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"\r\nEncrypted Source: {req}");
            return Activator.CreateInstance<T>();
        }
    }

    public static string? DecryptRequest(string? req, string app)
    {
        try
        {
            var aes = ConfigMgr.AesKeyIv(app);
            return req.IsEmpty() ? null : DecryptAes(req!, aes.Key, aes.Iv, aes.Base);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"\r\nEncrypted Source: {req}");
            return null;
        }
    }

    public static string? DecryptRequest(this StringValues req, string app) => DecryptRequest(req.FirstOrDefault(), app);

    public static string EncryptEventMessage(string plainTxt, string app)
    {
        var aes = ConfigMgr.AesKeyIv(app);
        return EncryptAes(plainTxt, aes.Key, aes.Iv, aes.Base);
    }

    public static string DecryptEventMessage(string cipherTxt, string app)
    {
        try
        {
            var aes = ConfigMgr.AesKeyIv(app);
            return DecryptAes(cipherTxt, aes.Key, aes.Iv, aes.Base);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            return "";
        }
    }

    public static void BypassCertificateError()
    {
        if (!ConfigMgr.BypassCertificateError())
            return;

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
        ServicePointManager.ServerCertificateValidationCallback += (_, _, _, errors) => errors == SslPolicyErrors.None;
    }

    public static string ToCrc64(this string plain)
    {
        if (plain.IsEmpty())
            return plain;

        var crc = CrcAlgorithm.CreateCrc64();
        try
        {
            crc.Append(Encoding.ASCII.GetBytes(plain));
            return crc.ToHexString().ToLower();
        }
        finally
        {
            crc.Clear();
        }
    }

    public static string ToCrc64(this long id) => Convert.ToString(id).ToCrc64();

    public static string? ToCrc64(this long? id) => id is null ? null : Convert.ToString(id)?.ToCrc64();

    public static string ToSha256WithKey(this string value, string key) => Convert.ToBase64String(value.ToSha256BytesWithKey(key));

    public static byte[] ToSha256BytesWithKey(this string value, string key)
    {
        using var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(key));
        return hmac.ComputeHash(Encoding.ASCII.GetBytes(value));
    }

   
    public static ushort RandomNumber() => (ushort)new Random().Next(100, 9999);

    public static string ToSha512(this string input, ushort outputStringBase = 64)
    {
        var bytes = Encoding.UTF8.GetBytes(input);

        using var sha512 = SHA512.Create();
        var hashBytes = sha512.ComputeHash(bytes);

        return outputStringBase switch
        {
            64 => Convert.ToBase64String(hashBytes),
            16 => Convert.ToHexString(hashBytes),
            _ => ""
        };
    }
}