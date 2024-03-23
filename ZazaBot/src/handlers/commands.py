from aiogram import Router
from aiogram.filters import CommandStart, Command
from aiogram.types import Message
from aiogram.fsm.context import FSMContext

from src.utils import text_for_helpcm
from src.api.UserAPI import User
from src.api.NoteAPI import Note
from src.states.NotesState import CreateNote
from src.others.config_for_bot import ConfigUserData
from src.utils.text import text_for_my_profile
from src.kb.inl_kb import create_bt_profile, create_bt_note

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


@command_router.message(Command("my_profile"))
async def my_profile(message: Message):
    """
    Profile user
    :param message:
    :return:
    """

    usr = User()
    data_user: dict = dict(usr.get_userinfo_by_token(user_token=ConfigUserData.token))
    note = Note().get_notes()
    message_to_user: str = await text_for_my_profile(data_my_profile=data_user)
    await message.answer_photo(photo=data_user.get("info")["photo"], caption=f"<b>Количество заметок: {len(note)}</b>", reply_markup=await create_bt_profile(), parse_mode="HTML")


@command_router.message(Command("my_notes"))
async def my_notes(message: Message):
    """
    Getting user notes
    :param message:
    :return:
    """

    all_notes: list = Note().get_notes()
    print(all_notes)

    for note in all_notes:
        note = dict(note)
        message_note: str = f"<b>Заголовок</b>: {note.get('title')}\n\n<b>Текст</b>: \n\n{note.get('text')}"

        await message.answer(
            text=message_note,
            parse_mode="HTML",
            reply_markup=await create_bt_note(btn_id=note.get("id"))
        )