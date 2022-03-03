#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

RUN apt-get update
RUN apt-get -y install gss-ntlmssp

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GreatLeaderXiBot/GreatLeaderXiBot.csproj", "GreatLeaderXiBot/"]
COPY ["GreatLeaderXiBot.Domain/GreatLeaderXiBot.Domain.csproj", "GreatLeaderXiBot.Domain/"]
COPY ["GreatLeaderXiBot.Common/GreatLeaderXiBot.Common.csproj", "GreatLeaderXiBot.Common/"]
COPY ["GreatLeaderXiBot.Domain.Core/GreatLeaderXiBot.Domain.Core.csproj", "GreatLeaderXiBot.Domain.Core/"]
RUN dotnet restore "GreatLeaderXiBot/GreatLeaderXiBot.csproj"
COPY . .
WORKDIR "/src/GreatLeaderXiBot"
RUN dotnet build "GreatLeaderXiBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GreatLeaderXiBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GreatLeaderXiBot.dll"]