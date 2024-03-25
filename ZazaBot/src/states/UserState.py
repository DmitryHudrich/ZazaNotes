from aiogram.fsm.state import StatesGroup, State


class UpdateUser(StatesGroup):
    """
    Update User
    """

    firstName: str = State()
    lastName: str = State()
    description: str = State()
    country: str = State()
    photo: str = State()