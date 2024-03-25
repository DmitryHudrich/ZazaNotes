from aiogram import Router
from aiogram.filters import CommandStart, Command
from aiogram.types import Message
from aiogram.fsm.context import FSMContext
from aiogram.types import FSInputFile
from emoji import emojize

from src.utils import text_for_helpcm
from src.api.UserAPI import User
from src.api.NoteAPI import Note
from src.states.NotesState import CreateNote
from src.others.config_for_bot import ConfigUserData
from src.utils.text import text_for_my_profile
from src.kb.inl_kb import create_bt_profile, create_bt_note, create_bt_start


#Router for commands
command_router: Router = Router()


@command_router.message(CommandStart())
async def start_command(message: Message):
    """
    Message for start command
    :param message:
    :return:
    """

    await message.answer_animation(animation=FSInputFile('src/static/start_g.gif'), caption=f"<b>Привет {message.from_user.first_name} {emojize(string=':fire:', language='en')}</b>, я <b><i>Zaza бот</i></b> и я помогу тебе работать с твоими заметками!\n\nБолее подробная информация - {emojize(string=':new_moon:', language='en')}/help", parse_mode="HTML", reply_markup=await create_bt_start())


@command_router.message(Command("help"))
async def help_command(message: Message):
    """
    Message for help command
    :param message:
    :return:
    """

    message_to_user: str = "\n".join(await text_for_helpcm())
    await message.answer_animation(animation=FSInputFile("src/static/help_g.gif"),
                                   caption=f"{emojize(string=":bell:", language='en')} <b>Мой перечень команд:</b> \n\n" + message_to_user,
                                   parse_mode="HTML")


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
    await message.answer_photo(photo=data_user.get("info")["photo"], caption=f"{message_to_user}\n\n{emojize(string=':waxing_gibbous_moon:', language='en')}   <b>Количество заметок: {len(note)}</b>", reply_markup=await create_bt_profile(), parse_mode="HTML")


@command_router.message(Command("my_notes"))
async def my_notes(message: Message) -> None:
    """
    Getting user notes
    :param message:
    :return:
    """

    all_notes: list = Note().get_notes()
    print(all_notes)

    for note in all_notes:
        note = dict(note)
        message_note: str = f"{emojize(string=':new_moon:', language='en')}   <b>Заголовок</b>: {note.get('title')}\n\n{emojize(string=':first_quarter_moon:', language='en')}   <b>Текст</b>: \n\n{note.get('text')}"

        await message.answer(
            text=message_note,
            parse_mode="HTML",
            reply_markup=await create_bt_note(btn_id=note.get("id"))
        )


@command_router.message(Command("cancel"))
async def cancel_comman(message: Message, state: FSMContext) -> None:
    """
    Clear handler event
    :param message:
    :return:
    """

    await state.clear()
    await message.answer(text="<b>Отлично!</b> Все процессы были сброшены..", parse_mode="HTML")