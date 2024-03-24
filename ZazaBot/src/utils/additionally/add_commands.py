import emoji
from aiogram.types import BotCommand
from aiogram.types import FSInputFile
from aiogram.methods import SetMyCommands, SetMyDescription


async def set_commands_for_bot(bot) -> None:
    """
    Setting commands for bot
    :return:
    """

    await bot(SetMyCommands(
        commands=[
            BotCommand(command="my_profile", description="Мой профиль"),
            BotCommand(command="create_note", description="Создание заметки"),
            BotCommand(command="my_notes", description="Мои заметки"),
            BotCommand(command="start", description="Начало работы"),
            BotCommand(command="help", description="Помощь"),
            BotCommand(command="cancel", description="Сброс")
        ]
    ))


async def set_description_for_bot(bot):
    """
    Setting description for bot
    :param bot:
    :return:
    """

    return await bot(SetMyDescription(
            description=f"{emoji.emojize(string=':moving_hand:', language='en')} Я бот Zaza, и я помогу тебе управлять своими заметками!\n\n" \
            "Присоединяйся к нам!\n\nДля того, чтобы узнать все возможности бота введите /help",
        ),
    )
