FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 4200
EXPOSE 443

FROM node:alpine AS node-build
WORKDIR /src
COPY ["pp-frontend/pp-frontend/package.json", "."]
COPY ["pp-frontend/pp-frontend/package-lock.json", "."]
RUN npm install
COPY ["pp-frontend/pp-frontend/", "."]
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Firefly-iii-pp-Runner/Firefly-iii-pp/Firefly-iii-pp.csproj", "Firefly-iii-pp/Firefly-iii-pp.csproj"]
RUN dotnet restore "Firefly-iii-pp/Firefly-iii-pp.csproj"
COPY "Firefly-iii-pp-Runner" .
WORKDIR "/src/Firefly-iii-pp"
RUN ls
RUN dotnet build "Firefly-iii-pp.csproj" -c Release -o /app/build

FROM build AS publish
COPY --from=node-build /src/dist/pp-frontend/ /app/publish/wwwroot/
RUN dotnet publish "Firefly-iii-pp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Firefly-iii-pp.dll"]