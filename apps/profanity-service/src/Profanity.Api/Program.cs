using HappyHeadlines.Logging;
using Options;
using Profanity.Data;
using Profanity.Services;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceLogging("profanity-service");
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
