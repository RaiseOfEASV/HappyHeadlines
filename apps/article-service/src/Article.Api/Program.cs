
using Article.Data;
using Article.Services;
using Article.Services.application_services;
using Options;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddAppOptions(builder.Configuration);
builder.Services.AddDataSourceAndRepositories(builder.Configuration);
builder.Services.AddApplicationServices();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapMetrics();
app.UseHttpsRedirection();
app.UseAuthorization();

app.Run();