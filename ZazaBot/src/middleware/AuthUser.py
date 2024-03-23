from aiogram.dispatcher.middlewares.base import BaseMiddleware
from aiogram import types
from typing import Callable, Dict, Any, Awaitable

from ZazaBot.src.api.UserAPI import User, UserTelegram

import logging

class AuthorizationUser(BaseMiddleware):
    """
    Проверка на аунтентификацию юзера
    """

    async def __call__(
            self,
            handler: Callable[[types.TelegramObject, Dict[str, Any]], Awaitable[Any]],
            event: types.Message,
            data: Dict[str, Any]
    ):
        await self.auth_user(message=event, handler=handler, event=event, data=data)

    async def auth_user(self, message: types.Message, handler, event, data):
        # Check id

        id_photo_user: str = dict(dict(await message.from_user.get_profile_photos()).get("photos")[0][0]).get("file_id")

        user_data_to_add: UserTelegram = UserTelegram(
            message.from_user.first_name,
            message.from_user.last_name,
            message.from_user.id,
            id_photo_user
        )


        #Checking user
        user_service: User = User()

        response = user_service.add_user(
            data_to_add=user_data_to_add
        )

        if response == "User was created":
            logging.exception(msg="Request to create user - was created")
        elif response == "User is created":
            logging.info(msg="Request to create user - wis create")
            await message.answer(
                text="Вы были зарегистрированы!"
            )
        else:
            logging.error(msg="Doesnt create user")
            await message.answer(
                text="Не удалось обработать запрос..."
            )

        #Continue work
        return await handler(event, data)