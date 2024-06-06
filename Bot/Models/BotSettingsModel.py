from typing import Optional
from datetime import timedelta

class BotSettingsModel:
    def __init__(self,
                 alternative_main_group_id: int,
                 alternative_main_group_topic_id: int,
                 license_group_id: int,
                 license_group_topic_id: int,
                 inspect_group_id: int,
                 inspect_group_topic_id: int,
                 bridge_channel_id: int,
                 main_channel_id: int,
                 main_group_id: int,
                 main_channel_username: Optional[str] = None,
                 main_group_username: Optional[str] = None,
                 delay_between_posts: float = 0.0,
                 reminder_post_delay: timedelta = timedelta(hours=1),
                 reminder_account_delay: timedelta = timedelta(days=1)):
        
        self.alternative_main_group_id = alternative_main_group_id
        self.alternative_main_group_topic_id = alternative_main_group_topic_id
        self.license_group_id = license_group_id
        self.license_group_topic_id = license_group_topic_id
        self.inspect_group_id = inspect_group_id
        self.inspect_group_topic_id = inspect_group_topic_id
        self.bridge_channel_id = bridge_channel_id
        self.main_channel_id = main_channel_id
        self.main_group_id = main_group_id
        self.main_channel_username = main_channel_username
        self.main_group_username = main_group_username
        self.delay_between_posts = delay_between_posts
        self.reminder_post_delay = reminder_post_delay
        self.reminder_account_delay = reminder_account_delay

    def to_dict(self):
        return {
            "alternative_main_group_id": self.alternative_main_group_id,
            "alternative_main_group_topic_id": self.alternative_main_group_topic_id,
            "license_group_id": self.license_group_id,
            "license_group_topic_id": self.license_group_topic_id,
            "inspect_group_id": self.inspect_group_id,
            "inspect_group_topic_id": self.inspect_group_topic_id,
            "bridge_channel_id": self.bridge_channel_id,
            "main_channel_id": self.main_channel_id,
            "main_group_id": self.main_group_id,
            "main_channel_username": self.main_channel_username,
            "main_group_username": self.main_group_username,
            "delay_between_posts": self.delay_between_posts,
            "reminder_post_delay": self.reminder_post_delay,
            "reminder_account_delay": self.reminder_account_delay
        }
