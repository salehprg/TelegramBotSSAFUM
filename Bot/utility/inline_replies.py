from telethon import Button, events
from telethon.tl import types

def get_inline_keyboard(message_id):
    return [
            [
                Button.inline("تایید ✅", f"accept/{message_id}"),
                Button.inline("رد ❌", f"decline/{message_id}")
            ],
            [
                Button.inline("بررسی 🔍", f"more/{message_id}"),
                Button.inline("بررسی مجوز ⚠️", f"license_check/{message_id}")
            ]
        ]
    
def get_inspect_more_inline_keyboard(message_id):
    return [
            [
                Button.inline("تایید ✅", f"accept/{message_id}"),
                Button.inline("رد ❌", f"decline/{message_id}")
            ]
        ]
    
def get_license_check_inline_keyboard(message_id):
    return [
            [
                Button.inline("تایید ✅", f"accept/{message_id}"),
                Button.inline("رد ❌", f"decline/{message_id}")
            ]
        ]
    
def set_inspect_topic_inline_keyboard(chat_id, message_id):
    return [
            [
                Button.inline("انتخاب به عنوان گروه بازرسی 🔍", f"inspect_topic/{chat_id}/{message_id}"),
            ]
        ]
    
def set_bridgeChannel_inline_keyboard(chat_id):
    return [
            [
                Button.inline("تنظیم این کانال به عنوان Bridge", f"bridgeChannel/{chat_id}"),
            ]
        ]
    
def set_license_check_topic_inline_keyboard(chat_id, message_id):
    return [
            [
                Button.inline("انتخاب به عنوان گروه بررسی مجوز ⚠️", f"set_license_check_topic/{chat_id}/{message_id}"),
            ]
        ]
    
def set_main_alternative_group_topic_inline_keyboard(chat_id, message_id):
    return [
            [
                Button.inline("انتخاب به عنوان گروه ارسال پست ها 📩", f"set_Main_Alternative_Group_Topic/{chat_id}/{message_id}"),
            ]
        ]
    