from aiogram import Router
from aiogram.filters import CommandStart, Command
from aiogram.types import Message
from aiogram.fsm.context import FSMContext

from ZazaBot.src.utils import text_for_helpcm
from ZazaBot.src.states.NotesState import CreateNote


#Router for commands
command_router: Router = Router()


@command_router.message(CommandStart())
async def start_command(message: Message):
    """
    Message for start command
    :param message:
    :return:
    """

    await message.answer(
        text="Привет, я Zaza бот и я помогу тебе работать с твоими заметками!"
    )


@command_router.message(Command("help"))
async def help_command(message: Message):
    """
    Message for help command
    :param message:
    :return:
    """

    message_to_user: str = "\n".join(await text_for_helpcm())
    await message.answer(
        text=f"Мой перечень команд: \n" + message_to_user
    )


@command_router.message(Command("create_note"))
async def create_note(message: Message, state: FSMContext):
    """
    User creates a note
    :param message:
    :return:
    """

    await message.answer(text="Отлично, создаем вам заметку...")
    await message.answer(text="Пожалуйста введите заголовок вашей заметки")
    await state.set_state(CreateNote.title)