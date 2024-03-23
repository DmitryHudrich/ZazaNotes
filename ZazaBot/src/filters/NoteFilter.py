from aiogram.filters import BaseFilter
from aiogram import types

from ZazaBot.src.api.NoteAPI import Note


class DeleteNote(BaseFilter):

    async def __call__(self, btn_data: types.CallbackQuery) -> bool:
        """
        To dell note
        :param btn_data:
        :return:
        """

        all_notes: list = [note.get("id") for note in Note().get_notes()]
        if btn_data.data in all_notes:
            return True
        return False


class UpdateNote(BaseFilter):

    async def __call__(self, btn_data: types.CallbackQuery) -> bool:
        """
        To Update note
        :param btn_data:
        :return:
        """

        all_notes: list = [note.get("id") for note in Note().get_notes()]

        if btn_data.data[:-4] in all_notes:
            return True
        return False