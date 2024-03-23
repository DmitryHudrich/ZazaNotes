from aiogram import Router, types
from aiogram.fsm.context import FSMContext


from ZazaBot.src.handlers.commands import help_command
from ZazaBot.src.states.UserState import UpdateUser
from ZazaBot.src.states.NotesState import CreateNote, UpdateNote as upd_note
from ZazaBot.src.api.NoteAPI import Note, AddNote
from ZazaBot.src.api.UserAPI import User
from ZazaBot.src.data.UserData import UserUpdate
from ZazaBot.src.data.NoteData import UpdateNote


#Message handler
message_handler: Router = Router()


@message_handler.message(CreateNote.title)
async def set_title_note(message: types.Message, state: FSMContext) -> None:
    await state.update_data(title=message.text)
    await message.answer(text="Отлично, теперь описание вашей заметки")
    await state.set_state(CreateNote.text)


@message_handler.message(CreateNote.text)
async def set_text_note(message: types.Message, state: FSMContext) -> None:
    await state.update_data(text=message.text)
    await message.answer(text="Заметка была успешно создана!")

    #Тут должен быть create note, но их нет.
    note_to_add: AddNote = AddNote(
        **(await state.get_data())
    )

    note_service = Note()

    note_service.add_note(
        data_to_add=note_to_add
    )

    await state.clear()


@message_handler.message(UpdateUser.firstName)
async def set_firstname_user(message: types.Message, state: FSMContext):
    await state.update_data(firstName=message.text)
    await state.set_state(UpdateUser.lastName)
    await message.answer(text="Введите ваше второе имя")


@message_handler.message(UpdateUser.lastName)
async def set_lastname_user(message: types.Message, state: FSMContext):
    await state.update_data(lastName=message.text)
    await state.set_state(UpdateUser.description)
    await message.answer(text="Введите описание вашего профиля")


@message_handler.message(UpdateUser.description)
async def set_description_user(message: types.Message, state: FSMContext):
    await state.update_data(description=message.text)
    await state.set_state(UpdateUser.country)
    await message.answer(text="Страна вашего проживания")


@message_handler.message(UpdateUser.country)
async def set_country_user(message: types.Message, state: FSMContext):
    await state.update_data(country=message.text)
    await state.set_state(UpdateUser.photo)
    await message.answer(text="Жду теперь вашу фотографию!")


@message_handler.message(UpdateUser.photo)
async def set_photo_user(message: types.Message, state: FSMContext):
    if message.photo:
        await message.answer(text="Отлично, профиль обновлён!")
        await state.update_data(photo=dict(message.photo[0]).get("file_id"))

        #Update user info
        usr = User().update_user(
            data_to_update=UserUpdate(**(await state.get_data()))
        )

        if usr:
            await message.answer(text="Ваш профиль был успешно обновлён!")
        else:
            await message.answer(text="Не удалось обновить ваш профиль..")

        await state.clear()
    else:
        await message.answer(text="Вы прислали не фото!")
        await state.set_state(UpdateUser.photo)


#Update Note
@message_handler.message(upd_note.title)
async def update_title_note(message: types.Message, state: FSMContext):
    await state.update_data(title=message.text)
    await state.set_state(upd_note.text)
    await message.answer(text="Введите новый текст вашей заметки")


@message_handler.message(upd_note.text)
async def update_title_note(message: types.Message, state: FSMContext):
    await state.update_data(text=message.text)

    #Update Note
    Note().update_note(
        data_to_put=UpdateNote(**await state.get_data())
    )

    await message.answer(text="Отлично, ваша заметка успешно обновлена!")
    await state.clear()


@message_handler.message()
async def all_message(
        message: types.Message
) -> None:
    await message.answer(text="Не могу обработать ваш запрос")
    await help_command(message=message)