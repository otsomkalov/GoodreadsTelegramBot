# Goodreads Telegram Bot

Telegram Bot to explore [Goodreads](https://www.goodreads.com/). 

## Getting Started
### Prerequisites

- [.NET Core 3.0](https://dotnet.microsoft.com/download) or higher

### Installing

**Goodreads:**
1. Go to [Goodreads API](https://www.goodreads.com/api/index)
2. Create [developer keys](https://www.goodreads.com/api/keys)
3. Copy **key** and **secret**

**Telegram:**
1. Contact to [@BotFather](https://t.me/BotFather) in Telegram
2. Create new bot
3. Copy bot token

**Project:**
1. Clone project
2. Run in folder
```
docker-compose run -d goodreadstelegrambot <telegram_bot_token> <goodreads_key> <goodreads_secret>
```
or go to **GoodreadsTelegramBot** folder and run:
```
dotnet run <telegram_bot_token> <goodreads_key> <goodreads_secret>
```

## Usage

You can try this bot in [Telegram](https://t.me/ExploreGoodreadsBot)

## Built With

* [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) - .NET Client for Telegram Bot API
* [goodreads-dotnet](https://github.com/adamkrogh/goodreads-dotnet) - Goodreads .NET API Client Library

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.