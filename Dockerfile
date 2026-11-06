# Stage 1: Build the application using .NET 10 SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["DakGharNiryatKendra.csproj", "./"]
RUN dotnet restore "DakGharNiryatKendra.csproj"

# Copy all source files and publish release
COPY . .
RUN dotnet publish "DakGharNiryatKendra.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime using .NET 10 ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copy published binaries from build stage
COPY --from=build /app/publish .

# Set default port to 10000 (Render default)
ENV ASPNETCORE_HTTP_PORTS=10000
EXPOSE 10000

# Ensure the unprivileged app user owns the app directory for SQLite file writes
USER root
RUN chown -R app:app /app
USER app

ENTRYPOINT ["dotnet", "DakGharNiryatKendra.dll"]
