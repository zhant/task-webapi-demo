using AutoMapper;
using Cnvs.Demo.TaskManagement;
using Cnvs.Demo.TaskManagement.Storage;
using Cnvs.Demo.TaskManagement.WebApi.Middleware;
using Cnvs.Demo.TaskManagement.WebApi.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Task = Cnvs.Demo.TaskManagement.Domain.Task;
using TaskState = Cnvs.Demo.TaskManagement.Domain.TaskState;
using User = Cnvs.Demo.TaskManagement.Domain.User;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ITaskEngine, TaskEngine>();
builder.Services.AddSingleton<IUserRandomizer, UserRandomizer>();
builder.Services.AddTransient<ITaskRepository, TaskRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

var config = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<Cnvs.Demo.TaskManagement.Dto.User, User>();
    cfg.CreateMap<User, Cnvs.Demo.TaskManagement.Dto.User>();

    cfg.CreateMap<Cnvs.Demo.TaskManagement.Dto.Task, Task>()
        .ForMember(dest => dest.State, opt => opt.MapFrom(src => (TaskState)src.State));

    cfg.CreateMap<Task, Cnvs.Demo.TaskManagement.Dto.Task>()
        .ForMember(dest => dest.State,
            opt => opt.MapFrom(src => (Cnvs.Demo.TaskManagement.Dto.TaskState)src.State));
});

IMapper mapper = new Mapper(config);
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

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