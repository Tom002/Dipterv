#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Server/Dipterv.Server.csproj", "Server/"]
COPY ["Dipterv.Bll/Dipterv.Bll.csproj", "Dipterv.Bll/"]
COPY ["Dipterv.Dal/Dipterv.Dal.csproj", "Dipterv.Dal/"]
COPY ["Shared/Dipterv.Shared.csproj", "Shared/"]
COPY ["Client/Dipterv.Client.csproj", "Client/"]
RUN dotnet restore "Server/Dipterv.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "Dipterv.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Dipterv.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dipterv.Server.dll"]