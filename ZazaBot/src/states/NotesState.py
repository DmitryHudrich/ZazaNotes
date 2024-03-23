from aiogram.fsm.state import StatesGroup, State


class CreateNote(StatesGroup):
    """
    State for create note
    """

    title: str = State()
    text: str = State()


class UpdateNote(StatesGroup):
    """
    State for update note
    """

    title: str = State()
    text: str = State()