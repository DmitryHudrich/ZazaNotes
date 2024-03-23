from aiogram.utils.keyboard import InlineKeyboardBuilder, InlineKeyboardButton


async def create_bt_profile() -> InlineKeyboardBuilder.as_markup:
    """
    Buttons for user profile
    :return:
    """

    user_bts: InlineKeyboardBuilder = InlineKeyboardBuilder()
    user_bts.add(InlineKeyboardButton(text="Удалить профиль", callback_data="del_prof_btn"))
    user_bts.add(InlineKeyboardButton(text="Обновить профиль", callback_data="upd_prof_btn"))

    return user_bts.as_markup()