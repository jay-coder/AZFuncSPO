using System.Collections.Generic;

namespace AZFuncSPO.SharePoint.Client
{
    // Decompile from Microsoft.SharePoint.Client.Runtime.dll Version=16.1.0.0
    // For the purposes of running on multiplatform e.g. Azure Functions V2 on Linux
    internal static class IdcrlErrorCodes
    {
        private static Dictionary<int, string> s_errorMap = new Dictionary<int, string>()
        {
          {
            296962,
            nameof (PPCRL_AUTHSTATE_S_AUTHENTICATED_OFFLINE)
          },
          {
            296963,
            nameof (PPCRL_AUTHSTATE_S_AUTHENTICATED_PASSWORD)
          },
          {
            296964,
            nameof (PPCRL_AUTHSTATE_S_AUTHENTICATED_PARTNER)
          },
          {
            297031,
            nameof (PPCRL_REQUEST_S_IO_PENDING)
          },
          {
            297056,
            nameof (PPCRL_S_NO_MORE_IDENTITIES)
          },
          {
            297057,
            nameof (PPCRL_S_TOKEN_TYPE_DOES_NOT_SUPPORT_SESSION_KEY)
          },
          {
            297058,
            nameof (PPCRL_S_NO_SUCH_CREDENTIAL)
          },
          {
            297059,
            nameof (PPCRL_S_NO_AUTHENTICATION_REQUIRED)
          },
          {
            297077,
            nameof (PPCRL_S_OK_CLIENTTIME)
          },
          {
            297193,
            nameof (PPCRL_REQUEST_S_OK_NO_SLC)
          },
          {
            297194,
            nameof (PPCRL_REQUEST_S_IO_PENDING_NO_SLC)
          },
          {
            -2147186688,
            nameof (PPCRL_AUTHSTATE_E_UNAUTHENTICATED)
          },
          {
            -2147186687,
            nameof (PPCRL_AUTHSTATE_E_EXPIRED)
          },
          {
            -2147186672,
            nameof (PPCRL_AUTHREQUIRED_E_PASSWORD)
          },
          {
            -2147186668,
            nameof (PPCRL_AUTHREQUIRED_E_UNKNOWN)
          },
          {
            -2147186656,
            nameof (PPCRL_REQUEST_E_AUTH_SERVER_ERROR)
          },
          {
            -2147186655,
            nameof (PPCRL_REQUEST_E_BAD_MEMBER_NAME_OR_PASSWORD)
          },
          {
            -2147186653,
            nameof (PPCRL_REQUEST_E_PASSWORD_LOCKED_OUT)
          },
          {
            -2147186652,
            nameof (PPCRL_REQUEST_E_PASSWORD_LOCKED_OUT_BAD_PASSWORD_OR_HIP)
          },
          {
            -2147186650,
            nameof (PPCRL_REQUEST_E_FORCE_RENAME_REQUIRED)
          },
          {
            -2147186649,
            nameof (PPCRL_REQUEST_E_FORCE_CHANGE_PASSWORD_REQUIRED)
          },
          {
            -2147186648,
            nameof (PPCRL_REQUEST_E_STRONG_PASSWORD_REQUIRED)
          },
          {
            -2147186646,
            nameof (PPCRL_REQUEST_E_PARTNER_NOT_FOUND)
          },
          {
            -2147186645,
            nameof (PPCRL_REQUEST_E_PARTNER_HAS_NO_ASYMMETRIC_KEY)
          },
          {
            -2147186644,
            nameof (PPCRL_REQUEST_E_INVALID_POLICY)
          },
          {
            -2147186643,
            nameof (PPCRL_REQUEST_E_INVALID_MEMBER_NAME)
          },
          {
            -2147186642,
            nameof (PPCRL_REQUEST_E_MISSING_PRIMARY_CREDENTIAL)
          },
          {
            -2147186641,
            nameof (PPCRL_REQUEST_E_PENDING_NETWORK_REQUEST)
          },
          {
            -2147186640,
            nameof (PPCRL_REQUEST_E_FORCE_CHANGE_SQSA)
          },
          {
            -2147186639,
            nameof (PPCRL_REQUEST_E_PASSWORD_EXPIRED)
          },
          {
            -2147186636,
            nameof (PPCRL_REQUEST_E_PROFILE_ACCRUE_REQUIRED)
          },
          {
            -2147186634,
            nameof (PPCRL_REQUEST_E_EMAIL_VALIDATION_REQUIRED)
          },
          {
            -2147186633,
            nameof (PPCRL_REQUEST_E_PARTNER_NEED_STRONGPW)
          },
          {
            -2147186632,
            nameof (PPCRL_REQUEST_E_PARTNER_NEED_STRONGPW_EXPIRY)
          },
          {
            -2147186631,
            nameof (PPCRL_REQUEST_E_AUTH_EXPIRED)
          },
          {
            -2147186623,
            nameof (PPCRL_REQUEST_E_USER_FORGOT_PASSWORD)
          },
          {
            -2147186622,
            nameof (PPCRL_REQUEST_E_USER_CANCELED)
          },
          {
            -2147186616,
            nameof (PPCRL_REQUEST_E_NO_NETWORK)
          },
          {
            -2147186615,
            nameof (PPCRL_REQUEST_E_UNKNOWN)
          },
          {
            -2147186605,
            nameof (PPCRL_REQUEST_E_KID_HAS_NO_CONSENT)
          },
          {
            -2147186603,
            nameof (PPCRL_REQUEST_E_RSTR_FAULT)
          },
          {
            -2147186601,
            nameof (PPCRL_REQUEST_E_RSTR_MISSING_BASE64CERT)
          },
          {
            -2147186600,
            nameof (PPCRL_REQUEST_E_RSTR_MISSING_TOKENTYPE)
          },
          {
            -2147186597,
            nameof (PPCRL_REQUEST_E_RSTR_MISSING_PRIVATE_KEY)
          },
          {
            -2147186596,
            nameof (PPCRL_REQUEST_E_INVALID_SERVICE_TIMESTAMP)
          },
          {
            -2147186595,
            nameof (PPCRL_REQUEST_E_INVALID_PKCS10_TIMESTAMP)
          },
          {
            -2147186594,
            nameof (PPCRL_REQUEST_E_INVALID_PKCS10)
          },
          {
            -2147186591,
            nameof (PPCRL_E_IDENTITY_NOT_AUTHENTICATED)
          },
          {
            -2147186590,
            nameof (PPCRL_E_UNABLE_TO_RETRIEVE_SERVICE_TOKEN)
          },
          {
            -2147186583,
            nameof (PPCRL_E_AUTH_SERVICE_UNAVAILABLE)
          },
          {
            -2147186582,
            nameof (PPCRL_E_INVALID_AUTH_SERVICE_RESPONSE)
          },
          {
            -2147186581,
            nameof (PPCRL_E_UNABLE_TO_INITIALIZE_CRYPTO_PROVIDER)
          },
          {
            -2147186580,
            nameof (PPCRL_E_NO_MEMBER_NAME_SET)
          },
          {
            -2147186579,
            nameof (PPCRL_E_CALLBACK_REQUIRED)
          },
          {
            -2147186577,
            nameof (PPCRL_E_INVALIDFLAGS)
          },
          {
            -2147186576,
            nameof (PPCRL_E_UNABLE_TO_RETRIEVE_CERT)
          },
          {
            -2147186575,
            nameof (PPCRL_E_INVALID_RSTPARAMS)
          },
          {
            -2147186574,
            nameof (PPCRL_E_MISSING_FILE)
          },
          {
            -2147186573,
            nameof (PPCRL_E_ILLEGAL_LOGONIDENTITY_FLAG)
          },
          {
            -2147186572,
            nameof (PPCRL_E_CERT_NOT_VALID_FOR_MINTTL)
          },
          {
            -2147186570,
            nameof (PPCRL_E_CERT_INVALID_ISSUER)
          },
          {
            -2147186569,
            nameof (PPCRL_E_NO_CERTSTORE_FOR_ISSUERS)
          },
          {
            -2147186568,
            nameof (PPCRL_E_OFFLINE_AUTH)
          },
          {
            -2147186567,
            nameof (PPCRL_E_SIGN_POP_FAILED)
          },
          {
            -2147186560,
            nameof (PPCRL_E_CERT_INVALID_POP)
          },
          {
            -2147186559,
            nameof (PPCRL_E_CALLER_NOT_SIGNED)
          },
          {
            -2147186558,
            nameof (PPCRL_E_BUSY)
          },
          {
            -2147186557,
            nameof (PPCRL_E_DOWNLOAD_FILE_FAILED)
          },
          {
            -2147186556,
            nameof (PPCRL_E_BUILD_CERT_REQUEST_FAILED)
          },
          {
            -2147186555,
            nameof (PPCRL_E_CERTIFICATE_NOT_FOUND)
          },
          {
            -2147186554,
            nameof (PPCRL_E_AUTHBLOB_TOO_LARGE)
          },
          {
            -2147186553,
            nameof (PPCRL_E_AUTHBLOB_NOT_FOUND)
          },
          {
            -2147186552,
            nameof (PPCRL_E_AUTHBLOB_INVALID)
          },
          {
            -2147186551,
            nameof (PPCRL_E_EXTPROP_NOTFOUND)
          },
          {
            -2147186550,
            nameof (PPCRL_E_RESPONSE_TOO_LARGE)
          },
          {
            -2147186548,
            nameof (PPCRL_E_USER_NOTFOUND)
          },
          {
            -2147186547,
            nameof (PPCRL_E_SIGCHECK_FAILED)
          },
          {
            -2147186545,
            nameof (PPCRL_E_CREDTARGETNAME_INVALID)
          },
          {
            -2147186544,
            nameof (PPCRL_E_CREDINFO_CORRUPTED)
          },
          {
            -2147186543,
            nameof (PPCRL_E_CREDPROP_NOTFOUND)
          },
          {
            -2147186542,
            nameof (PPCRL_E_NO_LINKEDACCOUNTS)
          },
          {
            -2147186541,
            nameof (PPCRL_E_NO_LINKEDHANDLE)
          },
          {
            -2147186540,
            nameof (PPCRL_E_CERT_CA_ROLLOVER)
          },
          {
            -2147186539,
            nameof (PPCRL_E_REALM_LOOKUP)
          },
          {
            -2147186537,
            nameof (PPCRL_E_FORBIDDEN_NAMESPACE)
          },
          {
            -2147186535,
            nameof (PPCRL_E_IDENTITY_NOCID)
          },
          {
            -2147186534,
            nameof (PPCRL_E_IE_MISCONFIGURED)
          },
          {
            -2147186532,
            nameof (PPCRL_E_NO_UI)
          },
          {
            -2147186530,
            nameof (PPCRL_E_INVALID_RPS_TOKEN)
          },
          {
            -2147186529,
            nameof (PPCRL_E_NOT_UI_ERROR)
          },
          {
            -2147186528,
            nameof (PPCRL_E_INVALID_URL)
          },
          {
            -2147186474,
            nameof (PPCRL_REQUEST_E_PARTNER_INVALID_REQUEST)
          },
          {
            -2147186473,
            nameof (PPCRL_REQUEST_E_PARTNER_REQUEST_FAILED)
          },
          {
            -2147186472,
            nameof (PPCRL_REQUEST_E_PARTNER_INVALID_SECURITY_TOKEN)
          },
          {
            -2147186471,
            nameof (PPCRL_REQUEST_E_PARTNER_AUTHENTICATION_BAD_ELEMENTS)
          },
          {
            -2147186470,
            nameof (PPCRL_REQUEST_E_PARTNER_BAD_REQUEST)
          },
          {
            -2147186469,
            nameof (PPCRL_REQUEST_E_PARTNER_EXPIRED_DATA)
          },
          {
            -2147186468,
            nameof (PPCRL_REQUEST_E_PARTNER_INVALID_TIME_RANGE)
          },
          {
            -2147186467,
            nameof (PPCRL_REQUEST_E_PARTNER_INVALID_SCOPE)
          },
          {
            -2147186466,
            nameof (PPCRL_REQUEST_E_PARTNER_RENEW_NEEDED)
          },
          {
            -2147186465,
            nameof (PPCRL_REQUEST_E_PARTNER_UNABLE_TO_RENEW)
          },
          {
            -2147186464,
            nameof (PPCRL_REQUEST_E_MISSING_HASHED_PASSWORD)
          },
          {
            -2147186463,
            nameof (PPCRL_REQUEST_E_CLIENT_DEPRECATED)
          },
          {
            -2147186462,
            nameof (PPCRL_REQUEST_E_CANCELLED)
          },
          {
            -2147186461,
            nameof (PPCRL_REQUEST_E_INVALID_PKCS10_KEYLEN)
          },
          {
            -2147186460,
            nameof (PPCRL_REQUEST_E_DUPLICATE_SERVICETARGET)
          },
          {
            -2147186459,
            nameof (PPCRL_REQUEST_E_FORCE_SIGNIN)
          },
          {
            -2147186458,
            nameof (PPCRL_REQUEST_E_PARTNER_NEED_CERTIFICATE)
          },
          {
            -2147186457,
            nameof (PPCRL_REQUEST_E_PARTNER_NEED_PIN)
          },
          {
            -2147186456,
            nameof (PPCRL_REQUEST_E_PARTNER_NEED_PASSWORD)
          },
          {
            -2147186453,
            nameof (PPCRL_REQUEST_E_SCHANNEL_ERROR)
          },
          {
            -2147186452,
            nameof (PPCRL_REQUEST_E_CERT_PARSE_ERROR)
          },
          {
            -2147186451,
            nameof (PPCRL_REQUEST_E_PARTNER_SERVER_ERROR)
          },
          {
            -2147186450,
            nameof (PPCRL_REQUEST_E_PARTNER_LOGIN)
          },
          {
            -2147186449,
            nameof (PPCRL_REQUEST_E_FLOWDISABLED)
          },
          {
            -2147186448,
            nameof (PPCRL_REQUEST_E_USER_NOT_LINKED)
          },
          {
            -2147186447,
            nameof (PPCRL_REQUEST_E_ACCOUNT_CONVERSION_NEEDED)
          },
          {
            -2147186446,
            nameof (PPCRL_REQUEST_E_PARTNER_BAD_MEMBER_NAME_OR_PASSWORD)
          },
          {
            -2147186445,
            nameof (PPCRL_REQUEST_E_BAD_MEMBER_NAME_OR_PASSWORD_FED)
          },
          {
            -2147186444,
            nameof (PPCRL_REQUEST_E_HIP_ON_FIRST_LOGIN)
          },
          {
            -2147186443,
            nameof (PPCRL_REQUEST_E_INVALID_CARDSPACE_TOKEN)
          }
        };
        public const int PPCRL_AUTHSTATE_S_AUTHENTICATED_OFFLINE = 296962;
        public const int PPCRL_AUTHSTATE_S_AUTHENTICATED_PASSWORD = 296963;
        public const int PPCRL_AUTHSTATE_S_AUTHENTICATED_PARTNER = 296964;
        public const int PPCRL_REQUEST_S_IO_PENDING = 297031;
        public const int PPCRL_S_NO_MORE_IDENTITIES = 297056;
        public const int PPCRL_S_TOKEN_TYPE_DOES_NOT_SUPPORT_SESSION_KEY = 297057;
        public const int PPCRL_S_NO_SUCH_CREDENTIAL = 297058;
        public const int PPCRL_S_NO_AUTHENTICATION_REQUIRED = 297059;
        public const int PPCRL_S_OK_CLIENTTIME = 297077;
        public const int PPCRL_REQUEST_S_OK_NO_SLC = 297193;
        public const int PPCRL_REQUEST_S_IO_PENDING_NO_SLC = 297194;
        public const int PPCRL_AUTHSTATE_E_UNAUTHENTICATED = -2147186688;
        public const int PPCRL_AUTHSTATE_E_EXPIRED = -2147186687;
        public const int PPCRL_AUTHREQUIRED_E_PASSWORD = -2147186672;
        public const int PPCRL_AUTHREQUIRED_E_UNKNOWN = -2147186668;
        public const int PPCRL_REQUEST_E_AUTH_SERVER_ERROR = -2147186656;
        public const int PPCRL_REQUEST_E_BAD_MEMBER_NAME_OR_PASSWORD = -2147186655;
        public const int PPCRL_REQUEST_E_PASSWORD_LOCKED_OUT = -2147186653;
        public const int PPCRL_REQUEST_E_PASSWORD_LOCKED_OUT_BAD_PASSWORD_OR_HIP = -2147186652;
        public const int PPCRL_REQUEST_E_FORCE_RENAME_REQUIRED = -2147186650;
        public const int PPCRL_REQUEST_E_FORCE_CHANGE_PASSWORD_REQUIRED = -2147186649;
        public const int PPCRL_REQUEST_E_STRONG_PASSWORD_REQUIRED = -2147186648;
        public const int PPCRL_REQUEST_E_PARTNER_NOT_FOUND = -2147186646;
        public const int PPCRL_REQUEST_E_PARTNER_HAS_NO_ASYMMETRIC_KEY = -2147186645;
        public const int PPCRL_REQUEST_E_INVALID_POLICY = -2147186644;
        public const int PPCRL_REQUEST_E_INVALID_MEMBER_NAME = -2147186643;
        public const int PPCRL_REQUEST_E_MISSING_PRIMARY_CREDENTIAL = -2147186642;
        public const int PPCRL_REQUEST_E_PENDING_NETWORK_REQUEST = -2147186641;
        public const int PPCRL_REQUEST_E_FORCE_CHANGE_SQSA = -2147186640;
        public const int PPCRL_REQUEST_E_PASSWORD_EXPIRED = -2147186639;
        public const int PPCRL_REQUEST_E_PROFILE_ACCRUE_REQUIRED = -2147186636;
        public const int PPCRL_REQUEST_E_EMAIL_VALIDATION_REQUIRED = -2147186634;
        public const int PPCRL_REQUEST_E_PARTNER_NEED_STRONGPW = -2147186633;
        public const int PPCRL_REQUEST_E_PARTNER_NEED_STRONGPW_EXPIRY = -2147186632;
        public const int PPCRL_REQUEST_E_AUTH_EXPIRED = -2147186631;
        public const int PPCRL_REQUEST_E_USER_FORGOT_PASSWORD = -2147186623;
        public const int PPCRL_REQUEST_E_USER_CANCELED = -2147186622;
        public const int PPCRL_REQUEST_E_NO_NETWORK = -2147186616;
        public const int PPCRL_REQUEST_E_UNKNOWN = -2147186615;
        public const int PPCRL_REQUEST_E_KID_HAS_NO_CONSENT = -2147186605;
        public const int PPCRL_REQUEST_E_RSTR_FAULT = -2147186603;
        public const int PPCRL_REQUEST_E_RSTR_MISSING_BASE64CERT = -2147186601;
        public const int PPCRL_REQUEST_E_RSTR_MISSING_TOKENTYPE = -2147186600;
        public const int PPCRL_REQUEST_E_RSTR_MISSING_PRIVATE_KEY = -2147186597;
        public const int PPCRL_REQUEST_E_INVALID_SERVICE_TIMESTAMP = -2147186596;
        public const int PPCRL_REQUEST_E_INVALID_PKCS10_TIMESTAMP = -2147186595;
        public const int PPCRL_REQUEST_E_INVALID_PKCS10 = -2147186594;
        public const int PPCRL_E_IDENTITY_NOT_AUTHENTICATED = -2147186591;
        public const int PPCRL_E_UNABLE_TO_RETRIEVE_SERVICE_TOKEN = -2147186590;
        public const int PPCRL_E_AUTH_SERVICE_UNAVAILABLE = -2147186583;
        public const int PPCRL_E_INVALID_AUTH_SERVICE_RESPONSE = -2147186582;
        public const int PPCRL_E_UNABLE_TO_INITIALIZE_CRYPTO_PROVIDER = -2147186581;
        public const int PPCRL_E_NO_MEMBER_NAME_SET = -2147186580;
        public const int PPCRL_E_CALLBACK_REQUIRED = -2147186579;
        public const int PPCRL_E_INVALIDFLAGS = -2147186577;
        public const int PPCRL_E_UNABLE_TO_RETRIEVE_CERT = -2147186576;
        public const int PPCRL_E_INVALID_RSTPARAMS = -2147186575;
        public const int PPCRL_E_MISSING_FILE = -2147186574;
        public const int PPCRL_E_ILLEGAL_LOGONIDENTITY_FLAG = -2147186573;
        public const int PPCRL_E_CERT_NOT_VALID_FOR_MINTTL = -2147186572;
        public const int PPCRL_E_CERT_INVALID_ISSUER = -2147186570;
        public const int PPCRL_E_NO_CERTSTORE_FOR_ISSUERS = -2147186569;
        public const int PPCRL_E_OFFLINE_AUTH = -2147186568;
        public const int PPCRL_E_SIGN_POP_FAILED = -2147186567;
        public const int PPCRL_E_CERT_INVALID_POP = -2147186560;
        public const int PPCRL_E_CALLER_NOT_SIGNED = -2147186559;
        public const int PPCRL_E_BUSY = -2147186558;
        public const int PPCRL_E_DOWNLOAD_FILE_FAILED = -2147186557;
        public const int PPCRL_E_BUILD_CERT_REQUEST_FAILED = -2147186556;
        public const int PPCRL_E_CERTIFICATE_NOT_FOUND = -2147186555;
        public const int PPCRL_E_AUTHBLOB_TOO_LARGE = -2147186554;
        public const int PPCRL_E_AUTHBLOB_NOT_FOUND = -2147186553;
        public const int PPCRL_E_AUTHBLOB_INVALID = -2147186552;
        public const int PPCRL_E_EXTPROP_NOTFOUND = -2147186551;
        public const int PPCRL_E_RESPONSE_TOO_LARGE = -2147186550;
        public const int PPCRL_E_USER_NOTFOUND = -2147186548;
        public const int PPCRL_E_SIGCHECK_FAILED = -2147186547;
        public const int PPCRL_E_CREDTARGETNAME_INVALID = -2147186545;
        public const int PPCRL_E_CREDINFO_CORRUPTED = -2147186544;
        public const int PPCRL_E_CREDPROP_NOTFOUND = -2147186543;
        public const int PPCRL_E_NO_LINKEDACCOUNTS = -2147186542;
        public const int PPCRL_E_NO_LINKEDHANDLE = -2147186541;
        public const int PPCRL_E_CERT_CA_ROLLOVER = -2147186540;
        public const int PPCRL_E_REALM_LOOKUP = -2147186539;
        public const int PPCRL_E_FORBIDDEN_NAMESPACE = -2147186537;
        public const int PPCRL_E_IDENTITY_NOCID = -2147186535;
        public const int PPCRL_E_IE_MISCONFIGURED = -2147186534;
        public const int PPCRL_E_NO_UI = -2147186532;
        public const int PPCRL_E_INVALID_RPS_TOKEN = -2147186530;
        public const int PPCRL_E_NOT_UI_ERROR = -2147186529;
        public const int PPCRL_E_INVALID_URL = -2147186528;
        public const int PPCRL_REQUEST_E_PARTNER_INVALID_REQUEST = -2147186474;
        public const int PPCRL_REQUEST_E_PARTNER_REQUEST_FAILED = -2147186473;
        public const int PPCRL_REQUEST_E_PARTNER_INVALID_SECURITY_TOKEN = -2147186472;
        public const int PPCRL_REQUEST_E_PARTNER_AUTHENTICATION_BAD_ELEMENTS = -2147186471;
        public const int PPCRL_REQUEST_E_PARTNER_BAD_REQUEST = -2147186470;
        public const int PPCRL_REQUEST_E_PARTNER_EXPIRED_DATA = -2147186469;
        public const int PPCRL_REQUEST_E_PARTNER_INVALID_TIME_RANGE = -2147186468;
        public const int PPCRL_REQUEST_E_PARTNER_INVALID_SCOPE = -2147186467;
        public const int PPCRL_REQUEST_E_PARTNER_RENEW_NEEDED = -2147186466;
        public const int PPCRL_REQUEST_E_PARTNER_UNABLE_TO_RENEW = -2147186465;
        public const int PPCRL_REQUEST_E_MISSING_HASHED_PASSWORD = -2147186464;
        public const int PPCRL_REQUEST_E_CLIENT_DEPRECATED = -2147186463;
        public const int PPCRL_REQUEST_E_CANCELLED = -2147186462;
        public const int PPCRL_REQUEST_E_INVALID_PKCS10_KEYLEN = -2147186461;
        public const int PPCRL_REQUEST_E_DUPLICATE_SERVICETARGET = -2147186460;
        public const int PPCRL_REQUEST_E_FORCE_SIGNIN = -2147186459;
        public const int PPCRL_REQUEST_E_PARTNER_NEED_CERTIFICATE = -2147186458;
        public const int PPCRL_REQUEST_E_PARTNER_NEED_PIN = -2147186457;
        public const int PPCRL_REQUEST_E_PARTNER_NEED_PASSWORD = -2147186456;
        public const int PPCRL_REQUEST_E_SCHANNEL_ERROR = -2147186453;
        public const int PPCRL_REQUEST_E_CERT_PARSE_ERROR = -2147186452;
        public const int PPCRL_REQUEST_E_PARTNER_SERVER_ERROR = -2147186451;
        public const int PPCRL_REQUEST_E_PARTNER_LOGIN = -2147186450;
        public const int PPCRL_REQUEST_E_FLOWDISABLED = -2147186449;
        public const int PPCRL_REQUEST_E_USER_NOT_LINKED = -2147186448;
        public const int PPCRL_REQUEST_E_ACCOUNT_CONVERSION_NEEDED = -2147186447;
        public const int PPCRL_REQUEST_E_PARTNER_BAD_MEMBER_NAME_OR_PASSWORD = -2147186446;
        public const int PPCRL_REQUEST_E_BAD_MEMBER_NAME_OR_PASSWORD_FED = -2147186445;
        public const int PPCRL_REQUEST_E_HIP_ON_FIRST_LOGIN = -2147186444;
        public const int PPCRL_REQUEST_E_INVALID_CARDSPACE_TOKEN = -2147186443;

        public static bool TryGetErrorStringId(int hr, out string stringId)
        {
            return IdcrlErrorCodes.s_errorMap.TryGetValue(hr, out stringId);
        }
    }
}
