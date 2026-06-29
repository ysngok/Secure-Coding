using AuthService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AuthenticationService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseDeveloperExceptionPage();

app.MapControllers();

app.Run();
