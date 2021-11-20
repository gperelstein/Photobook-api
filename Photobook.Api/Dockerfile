#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Photobook.Api/Photobook.Api.csproj", "Photobook.Api/"]
COPY ["Photobook.Logic/Photobook.Logic.csproj", "Photobook.Logic/"]
COPY ["Photobook.Models/Photobook.Common.csproj", "Photobook.Common/"]
COPY ["Photobook.Data/Photobook.Data.csproj", "Photobook.Data/"]
COPY ["Photobook.Notifications/Photobook.Notifications.csproj", "Photobook.Notifications/"]
RUN dotnet restore "Photobook.Api/Photobook.Api.csproj"
COPY . .
WORKDIR "/src/Photobook.Api"
RUN dotnet build "Photobook.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Photobook.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Photobook.Api.dll"]