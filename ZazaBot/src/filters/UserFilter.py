from aiogram.filters import BaseFilter
from aiogram import types


class DeleteUser(BaseFilter):
    async def __call__(self, btn_data: types.CallbackQuery) -> bool:
        if btn_data.data == "del_prof_btn":
            return True
        return False


class UpdateUser(BaseFilter):

    async def __call__(self, btn_data: types.CallbackQuery) -> bool:
        if btn_data.data == "upd_prof_btn":
            return True
        return False