using ProductService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ProductManagementService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseDeveloperExceptionPage();

app.MapControllers();

app.Run();
