import emoji
from aiogram.utils.keyboard import InlineKeyboardMarkup, InlineKeyboardButton, InlineKeyboardBuilder


async def create_bt_profile() -> InlineKeyboardBuilder.as_markup:
    """
    Buttons for user profile
    :return:
    """

    user_bts: InlineKeyboardBuilder = InlineKeyboardBuilder()

    user_bts.row(
        InlineKeyboardButton(text=f"{emoji.emojize(string=':bullseye:', language='en')}  Удалить профиль", callback_data="del_prof_btn"),
        InlineKeyboardButton(text=f"{emoji.emojize(string=':magic_wand:', language='en')}  Обновить профиль", callback_data="upd_prof_btn"))

    user_bts.row(InlineKeyboardButton(text=f"{emoji.emojize(string=':books:', language='en')}  Мои заметки", callback_data="btn_my_notes"))

    return user_bts.as_markup()


async def create_bt_note(btn_id: str) -> InlineKeyboardBuilder.as_markup:
    """
    Buttons for unique note
    :return:
    """

    note_btn: InlineKeyboardBuilder = InlineKeyboardBuilder()
    note_btn.add(InlineKeyboardButton(text=f"{emoji.emojize(string=':bullseye:', language='en')}  Удалить", callback_data=btn_id))
    note_btn.add(InlineKeyboardButton(text=f"{emoji.emojize(string=':magic_wand:', language='en')}  Обновить", callback_data=btn_id+"_upd"))

    return note_btn.as_markup()


async def create_bt_start() -> InlineKeyboardBuilder.as_markup:
    """
    Buttons for start command
    :return:
    """

    btn_str: InlineKeyboardBuilder = InlineKeyboardBuilder()
    btn_str.add(InlineKeyboardButton(text=f"{emoji.emojize(string=':technologist:', language='en')}  Мой профиль", callback_data="btn_my_profile"))
    btn_str.add(InlineKeyboardButton(text=f"{emoji.emojize(string=':books:', language='en')}  Мои заметки", callback_data="btn_my_notes"))

    return btn_str.as_markup()