using Article.Data;
using Article.Services;
using Article.Services.application_services;
using HappyHeadlines.Monitoring;
using Options;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceLogging("article-service");
builder.AddServiceTracing("article-service");

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

app.UseServiceLogging();
app.MapControllers();
app.UseHttpsRedirection();
app.UseAuthorization();

app.Run();