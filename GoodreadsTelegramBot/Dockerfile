FROM mcr.microsoft.com/dotnet/core/runtime:latest AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:latest AS build
WORKDIR /src
COPY ["GoodreadsTelegramBot/GoodreadsTelegramBot.csproj", "GoodreadsTelegramBot/"]
RUN dotnet restore "GoodreadsTelegramBot/GoodreadsTelegramBot.csproj"
COPY . .
WORKDIR "/src/GoodreadsTelegramBot"
RUN dotnet build "GoodreadsTelegramBot.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "GoodreadsTelegramBot.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "GoodreadsTelegramBot.dll"]
