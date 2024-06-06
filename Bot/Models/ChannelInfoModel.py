from typing import Optional

class ChannelInfoModel:
    def __init__(self, name: Optional[str], username: Optional[str], description: Optional[str], phone: Optional[str], channelTelegramId: int):
        self.name = name
        self.Username = username
        self.description = description
        self.phone = phone
        self.channelTelegramId = channelTelegramId

    def to_dict(self):
        return {
            "name": self.name,
            "Username": self.Username,
            "description": self.description,
            "phone": self.phone,
            "channelTelegramId": self.channelTelegramId
        }
