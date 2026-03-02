# 1. BUILD STAGE: Use .NET 10 SDK to handle your v10 NuGet packages
FROM ://mcr.microsoft.com AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# 2. RUNTIME STAGE: Use .NET 8 Runtime as your target is net8.0
FROM ://mcr.microsoft.com
WORKDIR /app
COPY --from=build /app/out .

# Expose the port your app runs on
EXPOSE 8080

ENTRYPOINT ["dotnet", "XYZUniversityAPI.dll"]
