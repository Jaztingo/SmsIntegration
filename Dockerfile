#Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /source
COPY . .
RUN dotnet restore "./SmsIntegration/SmsIntegration.csproj" --disable-parallel
RUN dotnet publish "./SmsIntegration/SmsIntegration.csproj" -c release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY --from=base /app ./

EXPOSE 7122

ENTRYPOINT ["dotnet", "SmsIntegration.dll"]