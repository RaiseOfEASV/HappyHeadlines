using HappyHeadlines.Monitoring;
using Options;
using Publisher.Data;
using Publisher.Services;

var builder = WebApplication.CreateBuilder(args);

// Add observability
builder.AddServiceLogging("publisher-service");
builder.AddServiceTracing("publisher-service");

// Add core services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// Add configuration
builder.Services.AddAppOptions(builder.Configuration);

// Add infrastructure (DB)
builder.Services.AddDataSourceAndRepositories();

// Add business logic services
builder.Services.AddApplicationServices();

// Add message bus (RabbitMQ)
builder.Services.AddMessageBus();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseServiceLogging();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
