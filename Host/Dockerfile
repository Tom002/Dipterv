#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Host/Server.csproj", "Host/"]
COPY ["Dipterv.Bll/Dipterv.Bll.csproj", "Dipterv.Bll/"]
COPY ["Dipterv.Dal/Dipterv.Dal.csproj", "Dipterv.Dal/"]
COPY ["Shared/Dipterv.Shared.csproj", "Shared/"]
COPY ["UI/UI.csproj", "UI/"]
RUN dotnet restore "Host/Server.csproj"
COPY . .
WORKDIR "/src/Host"
RUN dotnet build "Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Templates.TodoApp.Host.dll"]