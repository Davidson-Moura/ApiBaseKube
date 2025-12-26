FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8081
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Web.csproj", "WebService/"]
RUN dotnet restore "WebService/Web.csproj"

COPY . .
WORKDIR "/src/WebService"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]

# dotnet publish Web.csproj -c Release -t:PublishContainer
