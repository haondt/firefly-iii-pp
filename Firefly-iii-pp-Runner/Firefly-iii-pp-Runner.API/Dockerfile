#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Firefly-iii-pp-Runner.API/Firefly-iii-pp-Runner.API.csproj", "Firefly-iii-pp-Runner.API/"]
RUN dotnet restore "Firefly-iii-pp-Runner.API/Firefly-iii-pp-Runner.API.csproj"
COPY . .
WORKDIR "/src/Firefly-iii-pp-Runner.API"
RUN dotnet build "Firefly-iii-pp-Runner.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Firefly-iii-pp-Runner.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firefly-iii-pp-Runner.API.dll"]