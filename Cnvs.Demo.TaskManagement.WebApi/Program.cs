using AutoMapper;
using Cnvs.Demo.TaskManagement;
using Cnvs.Demo.TaskManagement.WebApi.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Task = Cnvs.Demo.TaskManagement.Domain.Task;
using TaskState = Cnvs.Demo.TaskManagement.Domain.TaskState;
using User = Cnvs.Demo.TaskManagement.Domain.User;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ITaskEngine, TaskEngine>();
builder.Services.AddSingleton<IUserRandomizer, UserRandomizer>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

app.Run();