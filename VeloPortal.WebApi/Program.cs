using VeloPortal.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add Auto maper service
builder.Services.AddMapperServices(builder.Configuration);
// add Custom services
builder.Services.AddAppServices(builder.Configuration);
// add Rate limit Custom services
builder.Services.AddCustomRateLimiting();
// add Custom Swagger services
builder.Services.AddCustomSwagger();
// add JWT  services
builder.Services.AddJwtServices(builder.Configuration);

var app = builder.Build();
app.UsePathBase("/veloportal"); // for sub folder hosting
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCustomSwagger(app.Environment);
app.MapControllers();

app.Run();
