using Comment.Data;
using Comment.Services;
using MessageClient.Configuration;
using MessageClient.Extension;
using Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddAppOptions(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddDataSourceAndRepositories(builder.Configuration);
builder.Services.AddRabbitMqMessageClient(
    new RabbitMqClientOptions { ConnectionString = "host=rabbitmq" }, 
    new MessageHandlerOptions { SubscriptionPrefix = "comment"}
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();