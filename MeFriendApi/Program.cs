using MeFriendApi.Services;
using MeFriendApi.Services.Middlewares;

var builder = WebApplication.CreateBuilder(args);
ServiceRegistration.RegisterService(builder.Services);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserAuthMiddleware>();

app.MapControllers();

app.Run();
