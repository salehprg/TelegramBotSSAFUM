from datetime import datetime, timedelta

from persiantools.jdatetime import JalaliDateTime


class MessagesStatus:
    @staticmethod
    def pending_message(postdatetime):
        
        gregorian_datetime  = datetime(postdatetime.year, postdatetime.month, postdatetime.day,
                                   postdatetime.hour, postdatetime.minute, postdatetime.second) + timedelta(hours=3 , minutes=30)
        
        persian_datetime = JalaliDateTime(gregorian_datetime)
        persian_month_names = [
            "فروردین", "اردیبهشت", "خرداد",
            "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر",
            "دی", "بهمن", "اسفند"
        ]

        formatted_date = persian_datetime.strftime('%d {} %Y ساعت %H:%M').format(persian_month_names[persian_datetime.month - 1])

        teststr = f"تاریخ پست : {formatted_date}"
        
        return teststr
    
    @staticmethod
    def accept_message(name, username):
        return f"وضعیت : تایید شده ✅\n توسط : {name} - @{username}"

    @staticmethod
    def decline_message(name, username):
        return f"وضعیت : رد شده ❌\n توسط : {name} - @{username}"

    @staticmethod
    def more_message(name, username):
        return f"وضعیت : ارسال برای بازرسی 🔍\n توسط : {name} - @{username}"

    @staticmethod
    def license_check_message(name, username):
        return f"وضعیت : ارسال برای بررسی مجوز ⚠️\n توسط : {name} - @{username}"
