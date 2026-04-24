#MealPlannerAPI.BlazorHost
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS build

RUN apt-get update && \
    apt-get install -y curl gnupg && \
    curl -fsSL https://deb.nodesource.com/setup_22.x | bash - && \
    apt-get install -y nodejs

RUN dotnet tool install -g Volo.Abp.Cli


ENV PATH="${PATH}:/root/.dotnet/tools"

WORKDIR /app

COPY . .

RUN dotnet restore "MealPlannerAPI.slnx"

WORKDIR "/app/src/MealPlannerAPI.BlazorHost"
RUN abp install-libs
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/sdk:10.0

#COPY bin/Release/net10.0/publish/app/
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MealPlannerAPI.BlazorHost.dll"]