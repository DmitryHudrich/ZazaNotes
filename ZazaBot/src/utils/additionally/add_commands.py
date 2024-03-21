from aiogram.types import BotCommand
from aiogram.methods import SetMyCommands


async def set_commands_for_bot(bot) -> None:
    """
    Setting for bot
    :return:
    """

    await bot(SetMyCommands(
        commands=[
            BotCommand(command="my_profile", description="Мой профиль"),
            BotCommand(command="start", description="Начало работы"),
            BotCommand(command="help", description="Помощь")
        ]
    ))