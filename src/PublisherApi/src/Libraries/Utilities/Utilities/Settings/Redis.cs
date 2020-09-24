namespace Utilities.Settings
{
    public class Redis
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Password { get; set; }

        public string ConnectionString => string.IsNullOrEmpty(Password) ? $"{Host}:{Port},connectRetry=5"
                                                                         : $"{Host}:{Port},password={Password},connectRetry=5";
    }
}