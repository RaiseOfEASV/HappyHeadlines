namespace Options;

public class AppOptions
{
    public string DbConnectionString { get; set; } = string.Empty;
    public string RedisConnectionString { get; set; } = string.Empty;
    public string ArticleServiceUrl { get; set; } = string.Empty;
    public RabbitMqOptions RabbitMq { get; set; } = new();
    public SmtpOptions Smtp { get; set; } = new();
}

public class RabbitMqOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "admin";
    public string Password { get; set; } = "admin";
    public string VirtualHost { get; set; } = "/";
}

public class SmtpOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromEmail { get; set; } = "newsletter@happyheadlines.com";
    public string FromName { get; set; } = "HappyHeadlines Newsletter";
}
