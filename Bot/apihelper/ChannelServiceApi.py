import requests

class ChannelServiceApi:
    def __init__(self, base_url):
        self.base_url = base_url.rstrip('/')
        self.headers = {
            'Content-Type': 'application/json',
            # Add any other headers you may need like authorization headers
        }

    def get_channel_by_id(self, channel_id):
        url = f"{self.base_url}/api/channels/{channel_id}"
        response = requests.get(url, headers=self.headers)
        return response.json()

    def get_channel_by_telegram_id(self, telegram_channel_id):
        url = f"{self.base_url}/api/channels/telegram/{telegram_channel_id}"
        response = requests.get(url, headers=self.headers)
        return response.json()
    
    def get_deactivated_channels(self):
        url = f"{self.base_url}/api/channels/deactivated"
        response = requests.get(url, headers=self.headers)
        return response.json()

    def add_channel(self, channel_info):
        url = f"{self.base_url}/api/channels/add"
        response = requests.post(url, json=channel_info, headers=self.headers)
        return response.json()

    def add_multiple_channels(self, channel_info_list):
        url = f"{self.base_url}/api/channels/add-multiple"
        response = requests.post(url, json=channel_info_list, headers=self.headers)
        return response.json()
    
    def update_multiple(self, channel_info_list):
        url = f"{self.base_url}/api/channels/update-multiple"
        response = requests.put(url, json=channel_info_list, headers=self.headers)
        return response.json()

    # Add methods for other endpoints similarly
