
using models.continents;

namespace Options;

public class AppOptions
{
    public string JwtSecret { get; set; } = string.Empty;
    public string DbConnectionString { get; set; } = string.Empty;
    public string Global { get; set; } = string.Empty;
    public string Europe { get; set; } = string.Empty;
    public string Africa { get; set; } = string.Empty;
    public string Asia { get; set; } = string.Empty;
    public string Australia { get; set; } = string.Empty;
    public string SouthAmerica { get; set; } = string.Empty;
    public string NorthAmerica { get; set; } = string.Empty;
    public string Antarctica { get; set; } = string.Empty;

    public string GetConnectionString(Continent continent)
    {
        return continent switch
        {
            Continent.Global => Global,
            Continent.Europe => Europe,
            Continent.Africa => Africa,
            Continent.Asia => Asia,
            Continent.Australia => Australia,
            Continent.SouthAmerica => SouthAmerica,
            Continent.NorthAmerica => NorthAmerica,
            Continent.Antarctica => Antarctica,
            _ => string.Empty
        };
    }
    
}

