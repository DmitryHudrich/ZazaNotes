from aiogram.dispatcher.middlewares.base import BaseMiddleware
from aiogram import types, Dispatcher

from emoji import emojize
from typing import Callable, Dict, Any, Awaitable

import cachetools

class AntiFloodMiddleware(BaseMiddleware):
    """
    Anti flood system for bot
    """

    def __init__(self, limit: int = 4) -> None:
        self.max_request = cachetools.TTLCache(maxsize=10_000, ttl=limit)

    async def __call__(self,
                            handler: Callable[[types.TelegramObject, Dict[str, Any]], Awaitable[Any]],
                            event: types.Message,
                            data: Dict[str, Any]) -> None:
        """
        Check message on flood
        :param message:
        :return:
        """

        if event.chat.id in self.max_request:
            await event.answer(text="Сработала система анти спама, пожалуйста подождите..")
            return
        else:
            self.max_request[event.chat.id] = None
        return await handler(event, data)

