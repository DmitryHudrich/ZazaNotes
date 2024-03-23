from aiogram import Router, types
from aiogram.fsm.context import FSMContext


from ZazaBot.src.handlers.commands import help_command
from ZazaBot.src.states.NotesState import CreateNote
from ZazaBot.src.api.NoteAPI import Note, AddNote


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
        await state.get_data()
    )

    print(note_to_add.get_dict())
    await state.clear()

@message_handler.message()
async def all_message(
        message: types.Message
) -> None:
    await message.answer(text="Не могу обработать ваш запрос")
    await help_command(message=message)