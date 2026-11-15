FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app

COPY CompuCare.Platform/*.csproj CompuCare.Platform/
RUN dotnet restore ./CompuCare.Platform

COPY . .
RUN dotnet publish ./CompuCare.Platform -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=builder /app/out .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "CompuCare.Platform.dll"]
