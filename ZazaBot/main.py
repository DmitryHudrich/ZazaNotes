import logging

from src.TelegramBot import telegram_application

import asyncio

if __name__ == "__main__":
    #Start application
    try:
        asyncio.run(telegram_application())
    except Exception as ex:
        logging.exception(msg="End work")