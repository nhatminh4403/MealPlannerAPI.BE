#MealPlannerAPI.DbMigrator
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app
COPY . .

RUN dotnet restore "MealPlannerAPI.slnx"
COPY . .

WORKDIR "src/MealPlannerAPI.DbMigrator"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/sdk:10.0

# COPY bin/Release/net10.0/publish/app/

WORKDIR /app
COPY --from=build /app/publish .

# ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "MealPlannerAPI.DbMigrator.dll"]