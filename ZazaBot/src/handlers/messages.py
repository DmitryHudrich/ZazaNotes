from aiogram import Router, types
from ZazaBot.src.handlers.commands import help_command

#Message handler
message_handler: Router = Router()


@message_handler.message()
async def all_message(
        message: types.Message
) -> None:
    await message.answer(text="Не могу обработать ваш запрос")
    await help_command(message=message)