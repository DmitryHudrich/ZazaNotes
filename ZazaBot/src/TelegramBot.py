import logging

from aiogram import Bot, Dispatcher
from aiogram.fsm.storage.memory import MemoryStorage
from aiogram.methods import SetMyDescription

from ZazaBot.configuration import telegram_bot_id
from ZazaBot.src.handlers.commands import command_router
from ZazaBot.src.utils.additionally.add_commands import set_commands_for_bot, set_description_for_bot


Zaza_bot: Bot = Bot(token=telegram_bot_id)

async def telegram_application() -> None:
    """
    Start bot function
    :return:
    """

    local_storage_bot: MemoryStorage = MemoryStorage()
    dp_bot: Dispatcher = Dispatcher(bot=Zaza_bot, storage=local_storage_bot)

    #Loggin
    logging.basicConfig(level=logging.INFO)

    #Include routers
    dp_bot.include_routers(
        command_router
    )

    #Set data for bot
    await set_commands_for_bot(bot=Zaza_bot)
    await set_description_for_bot(bot=Zaza_bot)

    try:
        await dp_bot.start_polling(Zaza_bot)
    except KeyboardInterrupt:
        logging.exception(msg="Бот окончил свою работу")