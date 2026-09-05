using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDependency(builder.Configuration);
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("corsapp");

app.MapControllers();

app.Run();
