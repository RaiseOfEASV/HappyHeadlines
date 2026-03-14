using HappyHeadlines.Monitoring;
using Options;
using Draft.Data;
using Draft.Services;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceLogging("draft-service");
builder.AddServiceTracing("draft-service");
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddAppOptions(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddDataSourceAndRepositories();

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
