import os

from dotenv import load_dotenv

load_dotenv()


telegram_bot_id = os.getenv("TelegramApiId")
api_app = os.getenv("API_URL")