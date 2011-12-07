namespace NancyTwitter.OAuth
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class OAuthParameterSet : IEnumerable<KeyValuePair<OAuthParameter, string>>
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;
        private readonly string _accessSecret;

        private readonly IDictionary<OAuthParameter, string> _parameters;

        public OAuthParameterSet(string consumerKey, string consumerSecret) : this(consumerKey, consumerSecret, string.Empty)
        {
        }

        public OAuthParameterSet(string consumerKey, string consumerSecret, string accessToken) : this(consumerKey, consumerSecret, accessToken, string.Empty)
        {
        }

        public OAuthParameterSet(string consumerKey, string consumerSecret, string accessToken, string accessSecret)
        {
            if (consumerKey == null) throw new ArgumentNullException("consumerKey");
            if (consumerSecret == null) throw new ArgumentNullException("consumerSecret");

            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _accessSecret = accessSecret ?? string.Empty;
            _parameters = new Dictionary<OAuthParameter, string>
                              {
                                  {OAuthParameter.ConsumerParameter, consumerKey},
                                  {OAuthParameter.SignatureMethod, "HMAC-SHA1"},
                                  {OAuthParameter.Version, "1.0"},
                                  {OAuthParameter.Nonce, CreateNonce()}
                              };

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                _parameters.Add(OAuthParameter.Token, accessToken);
            }
        }

        private static string CreateNonce()
        {
            return new Random().Next(123456, int.MaxValue).ToString("X", CultureInfo.InvariantCulture);
        }

        public string this[OAuthParameter index]
        {
            get { return _parameters[index]; }
            set { _parameters[index] = value; }
        }

        public IEnumerator<KeyValuePair<OAuthParameter, string>> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(OAuthParameter parameter, string value)
        {
            _parameters[parameter] = value;
        }

        public string GetOAuthHeaderString(Uri uri, string method)
        {
            var signature = GenerateSignature(uri, method).UrlEncode();
            var builder = new StringBuilder(@"OAuth realm=""""");
            builder.AppendFormat(@",oauth_consumer_key=""{0}""", _parameters[OAuthParameter.ConsumerParameter]);
            builder.AppendFormat(@",oauth_nonce=""{0}""", _parameters[OAuthParameter.Nonce]);
            builder.AppendFormat(@",oauth_signature_method=""{0}""", _parameters[OAuthParameter.SignatureMethod]);
            builder.AppendFormat(@",oauth_timestamp=""{0}""", _parameters[OAuthParameter.Timestamp]);

            if (_parameters.ContainsKey(OAuthParameter.Token))
            {
                builder.AppendFormat(@",oauth_token=""{0}""", _parameters[OAuthParameter.Token]);
            }

            if (_parameters.ContainsKey(OAuthParameter.Verifier))
            {
                builder.AppendFormat(@",oauth_verifier=""{0}""", _parameters[OAuthParameter.Verifier]);
            }

            builder.AppendFormat(@",oauth_version=""{0}""", _parameters[OAuthParameter.Version]);
            builder.AppendFormat(@",oauth_signature=""{0}""", signature);
            return builder.ToString();
        }

        public string GenerateSignature(Uri uri, string method)
        {
            var hmacKeyBase = _consumerSecret.UrlEncode() + "&" + _accessSecret.UrlEncode();
            using (var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(hmacKeyBase)))
            {
                var signatureBase = GenerateSignatureBase(uri, method);

                var hash = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(signatureBase));
                return Convert.ToBase64String(hash);
            }
        }

        public string GenerateSignatureBase(Uri uri, string method)
        {
            _parameters[OAuthParameter.ConsumerParameter] = _consumerKey;
            if (!_parameters.ContainsKey(OAuthParameter.Timestamp)) _parameters[OAuthParameter.Timestamp] = DateTime.UtcNow.ToUnixTime().ToString();
            var stringParameter = string.Join("&", this.OrderBy(p => p.Key).ThenBy(p => p.Value).Select(kvp => string.Format(@"{0}={1}", kvp.Key, kvp.Value.UrlEncode())));
            var builder = new StringBuilder(method.ToUpperInvariant() + "&");
            builder.Append(
                uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path, UriFormat.Unescaped).UrlEncode());
            builder.Append('&');
            builder.Append(stringParameter.UrlEncode());
            return builder.ToString();
        }
    }
}