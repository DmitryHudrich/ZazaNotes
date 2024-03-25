from aiogram import types, Router
from aiogram.fsm.context import FSMContext

from src.filters.UserFilter import DeleteUser, UpdateUser
from src.filters.NoteFilter import DeleteNote, UpdateNote
from src.api.UserAPI import User
from src.api.NoteAPI import Note
from src.states.UserState import UpdateUser as upd_usr
from src.states.NotesState import UpdateNote as upd_note
from src.others.config_for_bot import ConfigUserData
from src.handlers.commands import my_profile, my_notes

#Callbacks router

clb_router: Router = Router()


####Users####
@clb_router.callback_query(DeleteUser())
async def delete_user(msg: types.CallbackQuery):
    await msg.answer(text="Вы удалили свой профиль")
    await msg.message.delete()
    User().del_user_by_token()


@clb_router.callback_query(UpdateUser())
async def update_user(msg: types.CallbackQuery, state: FSMContext):
    await msg.answer(text="Обновление профиля....")
    await state.set_state(upd_usr.firstName)
    await msg.message.answer(text="Введите ваше первое имя")


####Notes####
@clb_router.callback_query(DeleteNote())
async def delete_note(clb_data: types.CallbackQuery):
    await clb_data.message.delete()
    await clb_data.answer(text="Вы удалили заметку")
    Note().del_note(id_note=clb_data.data)


@clb_router.callback_query(UpdateNote())
async def update_note(clb_data: types.CallbackQuery, state: FSMContext):
    await clb_data.answer(text="Обновляем вашу заметку..")
    await state.set_state(upd_note.title)
    await state.update_data(guid=clb_data.data[:-4])
    await clb_data.message.answer(text="Ваш новый заголовок: ")


####OTHERS#####
@clb_router.callback_query()
async def other_callbacks(clb_data: types.CallbackQuery):
    """
    Reaction to other callback btn
    :param clb_data:
    :return:
    """


    match clb_data.data:
        case "btn_my_profile":
            ConfigUserData.is_redirect = "/my_profile"
            ConfigUserData.id_user = clb_data.message.from_user.id
            await my_profile(clb_data.message)
        case "btn_my_notes":
            ConfigUserData.is_redirect = "/my_notes"
            ConfigUserData.id_user = clb_data.message.from_user.id
            await my_notes(clb_data.message)
        case _:
            await clb_data.message.answer(text="Не могу обработать ваш запрос")