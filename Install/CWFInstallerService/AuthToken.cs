namespace Idera.InstallationHelper
{
	using System;
	using System.Net;
using System.Runtime.Serialization;

	/// <summary>
	/// Represents authentication information used to connect to a service.
	/// </summary>
	[Serializable]
	public class AuthToken
		: IEquatable<AuthToken>
	{
		private readonly string username;
		private readonly string password;
		[OptionalField]
		private int hashcode;

		public static readonly AuthToken Empty = new AuthToken(string.Empty, string.Empty);

        public static void AssertNotNull<T>(T arg, string argName) where T : class
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }

		public AuthToken(string username, string password)
		{
			AssertNotNull(username, "username");
			AssertNotNull(password, "password");

			this.username = username;
			this.password = password;

			GenerateHashcode();			
		}

		public AuthToken(NetworkCredential credential)
		{
			AssertNotNull(credential, "credential");

			this.username = string.Format("{0}\\{1}", credential.Domain, credential.UserName);
			this.password = credential.Password;

			GenerateHashcode();
		}

		public static implicit operator AuthToken(NetworkCredential credential)
		{
			return credential != null ? new AuthToken(credential) : null;
		}

		public string Username
		{
			get
			{
				return username;
			}
		}

		public string Password
		{
			get
			{
				return password;
			}
		}

		#region System.Object Overrides

		public override bool Equals(object obj)
		{
			AuthToken other = obj as AuthToken;
			return other != null ? this.Equals(other) : false;
		}

		public override int GetHashCode()
		{
			return this.hashcode;
		}

		public override string ToString()
		{
			return string.Format("AuthToken [Username: {0}, Password Hash: {1:x8}]", this.username, this.password.GetHashCode());
		}

		#endregion

		#region IEquatable<AuthToken> Members

		public bool Equals(AuthToken other)
		{
			return other != null ?
				this.username.Equals(other.username, StringComparison.CurrentCultureIgnoreCase) &&
				this.password.Equals(other.password) : false;
		}

		#endregion

		public NetworkCredential ToNetworkCredential()
		{
			string[] usernameParts = username.Split(new char[] { '\\' }, 2);
			return usernameParts.Length > 1 ? new NetworkCredential(usernameParts[1], password, usernameParts[0]) : new NetworkCredential(username, password);
		}

		[OnDeserialized]
		private void OnDeserializedMethod(StreamingContext context)
		{
			GenerateHashcode();
		}

		private void GenerateHashcode()
		{
			this.hashcode = GetHashCode(
				this.username != null ? this.username.ToLower() : string.Empty,
				this.password != null ? this.password : string.Empty
				);
		}

        public static int GetHashCode(params object[] fields)
        {
            int hash = 23;

            for (int i = 0; i < fields.Length; i++)
                hash = ((hash << 5) * 37) ^ (fields[i] != null ? fields[i].GetHashCode() : 0);

            return hash;
        }

	}
}
