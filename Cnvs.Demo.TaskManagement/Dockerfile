FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Cnvs.Demo.TaskManagement/Cnvs.Demo.TaskManagement.csproj", "Cnvs.Demo.TaskManagement/"]
RUN dotnet restore "Cnvs.Demo.TaskManagement/Cnvs.Demo.TaskManagement.csproj"
COPY . .
WORKDIR "/src/Cnvs.Demo.TaskManagement"
RUN dotnet build "Cnvs.Demo.TaskManagement.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cnvs.Demo.TaskManagement.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cnvs.Demo.TaskManagement.dll"]
