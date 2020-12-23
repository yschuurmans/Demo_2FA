#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["2FA Demo/2FA Demo.csproj", "2FA Demo/"]
RUN dotnet restore "2FA Demo/2FA Demo.csproj"
COPY . .
WORKDIR "/src/2FA Demo"
RUN dotnet build "2FA Demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "2FA Demo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "2FA Demo.dll"]