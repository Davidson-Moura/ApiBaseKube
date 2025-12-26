FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["ApiService.csproj", "ApiService/"]
RUN dotnet restore "ApiService/ApiService.csproj"

COPY . .
WORKDIR "/src/ApiService"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ApiService.dll"]

# dotnet publish ApiService.csproj -c Release -t:PublishContainer