using Article.Services.application_interfaces.ports;
using models.continents;

namespace Article.Data.configuration;

public class ContinentContext:IContinentContext
{

    public Continent Continent { get; set; } = Continent.Global;
}