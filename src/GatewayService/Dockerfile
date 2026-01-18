FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

#copy
COPY Carsties.sln ./
COPY src/GatewayService/GatewayService.csproj ./src/GatewayService/

#restore
RUN dotnet restore ./src/GatewayService/GatewayService.csproj

#copy all

COPY . .

#publish

RUN dotnet publish ./src/GatewayService/GatewayService.csproj -c Release -o /app/publish

#runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GatewayService.dll"]