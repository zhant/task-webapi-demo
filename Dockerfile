FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Cnvs.Demo.TaskManagement.WebApi/Cnvs.Demo.TaskManagement.WebApi.csproj", "Cnvs.Demo.TaskManagement.WebApi/"]
RUN dotnet restore "Cnvs.Demo.TaskManagement.WebApi/Cnvs.Demo.TaskManagement.WebApi.csproj"
COPY . .
WORKDIR "/src/Cnvs.Demo.TaskManagement.WebApi"
RUN dotnet build "Cnvs.Demo.TaskManagement.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cnvs.Demo.TaskManagement.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cnvs.Demo.TaskManagement.WebApi.dll"]
