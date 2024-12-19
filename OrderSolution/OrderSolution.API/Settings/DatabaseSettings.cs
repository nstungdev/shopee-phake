namespace OrderSolution.API.Settings
{
    public class DatabaseSettings
    {
        public required string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }
        public int ConnectionTimeout { get; set; }
        public int RetryConnectionOnFailure { get; set; }
    }
}
