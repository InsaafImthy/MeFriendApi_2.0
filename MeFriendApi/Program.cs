using MeFriendApi.Services;
using MeFriendApi.Services.Middlewares;

var builder = WebApplication.CreateBuilder(args);
ServiceRegistration.RegisterService(builder.Services);
builder.Services.AddControllers();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserAuthMiddleware>();

app.MapControllers();

app.Run();
