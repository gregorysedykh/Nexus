FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Nexus.API.csproj", "./"]
RUN dotnet restore "Nexus.API.csproj"
COPY . .
RUN dotnet publish "Nexus.API.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Nexus.API.dll"]