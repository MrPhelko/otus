#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["OtusUsersApp.csproj", "."]
RUN dotnet restore "./OtusUsersApp.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "OtusUsersApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OtusUsersApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OtusUsersApp.dll"]