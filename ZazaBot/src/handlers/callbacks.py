from aiogram import types, Router
from aiogram.fsm.context import FSMContext

from ZazaBot.src.filters.UserFilter import DeleteUser, UpdateUser
from ZazaBot.src.api.UserAPI import User
from ZazaBot.src.states.UserState import UpdateUser as upd_usr


#Callbacks router

clb_router: Router = Router()


@clb_router.callback_query(DeleteUser())
async def delete_user(msg: types.CallbackQuery):
    await msg.answer(text="Вы удалили свой профиль")
    User().del_user_by_token()


@clb_router.callback_query(UpdateUser())
async def update_user(msg: types.CallbackQuery, state: FSMContext):
    await msg.answer(text="Обновление профиля....")
    await state.set_state(upd_usr.firstName)
    await msg.message.answer(text="Введите ваше первое имя")
