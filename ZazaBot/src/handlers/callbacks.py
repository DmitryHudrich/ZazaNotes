from aiogram import types, Router
from aiogram.fsm.context import FSMContext

from ZazaBot.src.filters.UserFilter import DeleteUser, UpdateUser
from ZazaBot.src.filters.NoteFilter import DeleteNote, UpdateNote
from ZazaBot.src.api.UserAPI import User
from ZazaBot.src.api.NoteAPI import Note
from ZazaBot.src.states.UserState import UpdateUser as upd_usr
from ZazaBot.src.states.NotesState import UpdateNote as upd_note


#Callbacks router

clb_router: Router = Router()


####Users####
@clb_router.callback_query(DeleteUser())
async def delete_user(msg: types.CallbackQuery):
    await msg.answer(text="Вы удалили свой профиль")
    User().del_user_by_token()


@clb_router.callback_query(UpdateUser())
async def update_user(msg: types.CallbackQuery, state: FSMContext):
    await msg.answer(text="Обновление профиля....")
    await state.set_state(upd_usr.firstName)
    await msg.message.answer(text="Введите ваше первое имя")


####Notes####
@clb_router.callback_query(DeleteNote())
async def delete_note(clb_data: types.CallbackQuery):
    await clb_data.answer(text="Вы удалили заметку")
    print(clb_data.data)
    Note().del_note(id_note=clb_data.data)


@clb_router.callback_query(UpdateNote())
async def update_note(clb_data: types.CallbackQuery, state: FSMContext):
    await clb_data.answer(text="Обновляем вашу заметку..")
    await state.set_state(upd_note.title)
    await state.update_data(guid=clb_data.data[:-4])
    await clb_data.message.answer(text="Ваш новый заголовок: ")

