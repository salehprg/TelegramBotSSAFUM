import requests

class PostServiceApi:
    def __init__(self, base_url="http://localhost:5293"):
        self.base_url = f'{base_url}/api/posts'

    def get_post_by_id(self, post_id):
        url = f"{self.base_url}/{post_id}"
        response = requests.get(url)
        return response.json()

    def get_posts_by_chat_id(self, chat_id):
        url = f"{self.base_url}/chat/{chat_id}"
        response = requests.get(url)
        return response.json()

    def get_events_by_telegram_id(self, telegram_chat_id, telegram_post_id):
        url = f"{self.base_url}/events"
        params = {
            "telegramChatId": telegram_chat_id,
            "telegramPostId": telegram_post_id
        }
        response = requests.get(url, params=params)
        return response.json()

    def create_new_post(self, post_info):
        url = f"{self.base_url}/new"
        response = requests.post(url, json=post_info)
        return response.json()

    def remove_old_post_status(self, post_info):
        url = f"{self.base_url}/remove"
        response = requests.post(url, json=post_info)
        return response.json()

    def forward_to_group_post(self, post_info, telegram_ids):
        url = f"{self.base_url}/forward/group"
        data = {
            "postInfo": post_info,
            "telegramIds": telegram_ids
        }
        response = requests.post(url, json=data)
        return response.json()

    def forward_to_channel_post(self, post_info):
        url = f"{self.base_url}/forward/channel"
        response = requests.post(url, json=post_info)
        return response.json()

    def accept_post(self, post_info):
        url = f"{self.base_url}/accept"
        response = requests.post(url, json=post_info)
        return response.json()

    def decline_post(self, post_info):
        url = f"{self.base_url}/decline"
        response = requests.post(url, json=post_info)
        return response.json()

    def inspect_post(self, post_info, telegram_ids):
        url = f"{self.base_url}/inspect"
        data = {
            "postInfo": post_info,
            "telegramIds": telegram_ids
        }
        response = requests.post(url, json=data)
        return response.json()

    def license_check_post(self, post_info, telegram_ids):
        url = f"{self.base_url}/license/check"
        data = {
            "postInfo": post_info,
            "telegramIds": telegram_ids
        }
        response = requests.post(url, json=data)
        return response.json()

    def get_removable_events(self, post_info_id):
        url = f"{self.base_url}/{post_info_id}/removable-events"
        response = requests.get(url)
        return response.json()
