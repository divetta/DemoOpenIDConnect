public class MyConstants
{
    // IdP Local
    #region IdP Local
    public const string Authority = "https://localhost:44301";

    public const string UserInfoEndpoint = Authority + "/connect/userinfo";
    public const string TokenEndpoint = Authority + "/connect/token";

    public const string ClientAppClientId = "mvc-and-js-auto";
    public const string ClientAppClientSecret = "super-secret";

    public const string IntrospectionClientId = "simple-api";
    public const string IntrospectionClientSecret = "secret";

    public const string ClientAppWebFormsClientId = "web-forms";
    public const string ClientAppWebFormsClientSecret = "super-secret-webforms";
    #endregion
}
