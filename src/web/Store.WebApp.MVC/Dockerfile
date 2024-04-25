FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/web/Store.WebApp.MVC/Store.WebApp.MVC.csproj", "src/web/Store.WebApp.MVC/"]
RUN dotnet restore "src/web/Store.WebApp.MVC/Store.WebApp.MVC.csproj"
COPY . .
WORKDIR "/src/src/web/Store.WebApp.MVC"
RUN dotnet build "Store.WebApp.MVC.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Store.WebApp.MVC.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Store.WebApp.MVC.dll"]
