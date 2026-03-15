namespace Options;

public class AppOptions
{
    public string DbConnectionString { get; set; } = string.Empty;
    public string DraftServiceUrl { get; set; } = string.Empty;
    public RabbitMqOptions RabbitMq { get; set; } = new();
}

public class RabbitMqOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "admin";
    public string VirtualHost { get; set; } = "/";
}
