using models.continents;

namespace Article.Services.application_interfaces.ports;

public interface IContinentContext
{
    Continent Continent { get; set; }
}
