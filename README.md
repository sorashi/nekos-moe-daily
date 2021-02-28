# nekos-moe-daily

Takes a random neko from https://nekos.moe and posts it into a configured discord webhook and/or telegram users. You can run it daily as a cronjob.

## Usage
`mkdir ./data && cd data && touch config.yml`

Example config.yml
```yml
telegram_bot_token: your.token
discord_webhook: webhook.url
data_dir: data
nsfw_chance: 0.2
```

and `docker-compose up` to post the neko
