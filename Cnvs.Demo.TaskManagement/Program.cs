using Cnvs.Demo.TaskManagement;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddHostedService<TaskRotationService>(); })
    .Build();

host.Run();