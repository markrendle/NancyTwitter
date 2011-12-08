namespace NancyTwitter.OAuth
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    public class OAuthParameterSet : IEnumerable<KeyValuePair<OAuthParameter, string>>
    {
        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random());
        private readonly string _consumerSecret;
        private readonly string _tokenSecret;

        private readonly IDictionary<OAuthParameter, string> _parameters;

        public OAuthParameterSet(string consumerKey, string consumerSecret) : this(consumerKey, consumerSecret, string.Empty)
        {
        }

        public OAuthParameterSet(string consumerKey, string consumerSecret, string token) : this(consumerKey, consumerSecret, token, string.Empty)
        {
        }

        public OAuthParameterSet(string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            if (consumerKey == null) throw new ArgumentNullException("consumerKey");
            if (consumerSecret == null) throw new ArgumentNullException("consumerSecret");

            _consumerSecret = consumerSecret;
            _tokenSecret = tokenSecret ?? string.Empty;
            _parameters = new Dictionary<OAuthParameter, string>
                              {
                                  {OAuthParameter.ConsumerKey, consumerKey},
                                  {OAuthParameter.SignatureMethod, "HMAC-SHA1"},
                                  {OAuthParameter.Version, "1.0"},
                                  {OAuthParameter.Nonce, CreateNonce()}
                              };

            if (!string.IsNullOrWhiteSpace(token))
            {
                _parameters.Add(OAuthParameter.Token, token);
            }
        }

        public string GetOAuthHeaderString(Uri uri, string method)
        {
            var signature = GenerateSignature(uri, method).UrlEncode();
            var builder = BuildParameterString();
            return @"OAuth realm=""""," + builder + string.Format(@",oauth_signature=""{0}""", signature);
        }

        private string BuildParameterString()
        {
            var parameters = OrderParameters()
                .Where(kvp => kvp.Key != OAuthParameter.Callback)
                .Select(kvp => string.Format(@"{0}=""{1}""", kvp.Key, kvp.Value));

            return string.Join(",", parameters);
        }

        private static string CreateNonce()
        {
            return Random.Value.Next(123456, int.MaxValue).ToString("X", CultureInfo.InvariantCulture);
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

        private string GenerateSignature(Uri uri, string method)
        {
            var hash = _consumerSecret.UrlEncode() + "&" + _tokenSecret.UrlEncode();
            using (var hasher = new HMACSHA1(Encoding.UTF8.GetBytes(hash)))
            {
                var signatureBase = GenerateSignatureBase(uri, method);

                var signature = hasher.ComputeHash(Encoding.UTF8.GetBytes(signatureBase));
                return Convert.ToBase64String(signature);
            }
        }

        private string GenerateSignatureBase(Uri uri, string method)
        {
            EnsureTimestamp();
            var stringParameter = string.Join("&", OrderParameters().Select(kvp => string.Format(@"{0}={1}", kvp.Key, kvp.Value.UrlEncode())));
            return string.Format("{0}&{1}&{2}",
                                 method.ToUpperInvariant(),
                                 uri.GetComponents(UriComponents.SchemeAndServer | UriComponents.Path,
                                                   UriFormat.Unescaped).UrlEncode(),
                                 stringParameter.UrlEncode());
        }

        private IEnumerable<KeyValuePair<OAuthParameter, string>> OrderParameters()
        {
            return this.OrderBy(p => p.Key).ThenBy(p => p.Value);
        }

        private void EnsureTimestamp()
        {
            if (!_parameters.ContainsKey(OAuthParameter.Timestamp))
                _parameters[OAuthParameter.Timestamp] = DateTime.UtcNow.ToUnixTime().ToString(CultureInfo.InvariantCulture);
        }
    }
}