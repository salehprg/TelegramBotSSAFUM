import requests

class BotSettingsServiceApi:
    def __init__(self, base_url):
        self.base_url = base_url.rstrip('/')
        self.headers = {
            'Content-Type': 'application/json',
            # Add any other headers you may need like authorization headers
        }
        
        self.botsettings = self.get_bot_settings()

    def set_bot_settings(self, bot_settings):
        url = f"{self.base_url}/api/bot/settings/set"
        response = requests.post(url, json=bot_settings, headers=self.headers)
        return response.json()

    def get_bot_settings(self):
        url = f"{self.base_url}/api/bot/settings/get"
        response = requests.get(url, headers=self.headers)
        return response.json()
