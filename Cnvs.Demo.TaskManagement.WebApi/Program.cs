using AutoMapper;
using Cnvs.Demo.TaskManagement;
using Cnvs.Demo.TaskManagement.Configuration;
using Cnvs.Demo.TaskManagement.Storage.InMemory;
using Cnvs.Demo.TaskManagement.WebApi.Configuration;
using Cnvs.Demo.TaskManagement.WebApi.Middleware;
using Cnvs.Demo.TaskManagement.WebApi.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Task = Cnvs.Demo.TaskManagement.Domain.Task;
using TaskState = Cnvs.Demo.TaskManagement.Domain.TaskState;
using User = Cnvs.Demo.TaskManagement.Domain.User;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TaskEngineOptions>(builder.Configuration.GetSection("TaskEngine"));

builder.Services.AddTransient<ITaskEngine, TaskEngine>();
builder.Services.AddSingleton<IUserRandomizer, UserRandomizer>();
builder.Services.AddSingleton<ITaskRepository, TaskInMemoryRepository>();
builder.Services.AddSingleton<IUserRepository, UserInMemoryRepository>();

var mapperConfiguration = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

var mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

builder.Services.AddHostedService<TaskRotationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionMiddleware>();
// TODO - add other middleware here as needed

app.Run();