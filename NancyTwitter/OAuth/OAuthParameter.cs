namespace NancyTwitter.OAuth
{
    using System;

    public class OAuthParameter : IEquatable<OAuthParameter>, IComparable<OAuthParameter>, IComparable
    {
        private readonly string _key;

        private OAuthParameter(string key)
        {
            _key = key;
        }

        public static readonly OAuthParameter ConsumerParameter = new OAuthParameter("oauth_consumer_key");
        public static readonly OAuthParameter Callback = new OAuthParameter("oauth_callback");
        public static readonly OAuthParameter Timestamp = new OAuthParameter("oauth_timestamp");
        public static readonly OAuthParameter SignatureMethod = new OAuthParameter("oauth_signature_method");
        public static readonly OAuthParameter Nonce = new OAuthParameter("oauth_nonce");
        public static readonly OAuthParameter Version = new OAuthParameter("oauth_version");
        public static readonly OAuthParameter Token = new OAuthParameter("oauth_token");
        public static readonly OAuthParameter Verifier = new OAuthParameter("oauth_verifier");

        public int CompareTo(OAuthParameter other)
        {
            return _key.CompareTo(other._key);
        }

        public override string  ToString()
        {
            return _key;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(OAuthParameter other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._key, _key);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (OAuthParameter)) return false;
            return Equals((OAuthParameter) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }

        public int CompareTo(object obj)
        {
            return CompareTo((OAuthParameter) obj);
        }

        public static bool operator ==(OAuthParameter left, OAuthParameter right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(OAuthParameter left, OAuthParameter right)
        {
            return !Equals(left, right);
        }

        public static implicit operator string(OAuthParameter parameter)
        {
            return parameter._key;
        }

        public static implicit operator OAuthParameter(string key)
        {
            return new OAuthParameter(key);
        }
    }
}