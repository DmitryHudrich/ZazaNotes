from emoji import emojize
async def text_for_helpcm() -> tuple:
    """
    All commands for help command
    :return:
    """

    all_commands: tuple = (
        f"{emojize(string=':new_moon:', language='en')} <b>/my_profile</b> - <i>Мой профиль</i>",
        f"{emojize(string=':new_moon:', language='en')} <b>/my_notes</b> - <i>Мои заметки</i>",
        f"{emojize(string=':new_moon:', language='en')} <b>/start</b> - <i>Запуск бота, начало работы</i>",
        f"{emojize(string=':new_moon:', language='en')} <b>/help</b> - <i>Помощь, здесь все известные команды, функции боту</i>",
        f"{emojize(string=':new_moon:', language='en')} <b>/cancel</b> - <i>Сброс состояния</i>",
        f"\n\n{emojize(string=':new_moon:', language='en')} Так же ты можешь просматривать, создавать, удалять, обновлять свои заметки!"
    )

    return all_commands


async def text_for_my_profile(data_my_profile: dict) -> str:
    """
    My profile data
    :param data_my_profile:
    :return:
    """

    my_profile_text: str = f"""{emojize(string=':new_moon:', language='en')}   <b><i>Мой профиль</i></b>\n\n{emojize(string=':waxing_crescent_moon:', language='en')}   <b>Моё имя</b>: {data_my_profile.get('info')['firstName'] + ' ' + (data_my_profile.get('info')['lastName'] if data_my_profile.get('info')['lastName'] else '')}\n\n{emojize(string=':first_quarter_moon:', language='en')}   <b>Описание профиля</b>: \n\n{data_my_profile.get('info')['description'] if data_my_profile.get('info')['description'] else 'Отсутствует'}\n\n"""
    return my_profile_text

