
async def text_for_helpcm() -> tuple:
    """
    All commands for help command
    :return:
    """

    all_commands: tuple = (
        "/my_profile - Мой профиль",
        "/start - Запуск бота, начало работы",
        "/help - Помощь, здесь все известные команды, функции боту",
        "Так же ты можешь просматривать, создавать, удалять, обновлять свои заметки!"
    )

    return all_commands


async def text_for_my_profile(data_my_profile: dict) -> str:
    """
    My profile data
    :param data_my_profile:
    :return:
    """

    my_profile_text: str = f"""
    <b><i>Мой профиль</i></b>\n\n
    <b>Моё имя</b>: {data_my_profile.get('info')['firstName'] + ' ' + (data_my_profile.get('info')['lastName'] if data_my_profile.get('info')['lastName'] else '')}\n\n
    <b>Описание профиля</b>: \n\n{data_my_profile.get('info')['description'] if data_my_profile.get('info')['description'] else 'Отсутствует'}\n\n
    """
    return my_profile_text

