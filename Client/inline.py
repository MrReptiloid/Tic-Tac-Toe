from aiogram.types import InlineKeyboardMarkup, InlineKeyboardButton as ib

def sub_menu(link: str):
    markup = InlineKeyboardMarkup(row_width = 1)
    markup.row(ib(text = "Подписаться", url = link))
    markup.row(ib(text = "Проверить подписку", callback_data = "check_sub"))
    return markup