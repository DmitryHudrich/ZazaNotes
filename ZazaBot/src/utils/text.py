
async def text_for_helpcm() -> tuple:
    """
    All commands for help command
    :return:
    """

    all_commands: tuple = (
        "/profile - Мой профиль",
        "/start - Запуск бота, начало работы",
        "/help - Помощь, здесь все известные команды, функции боту",
        "Так же ты можешь просматривать, создавать, удалять, обновлять свои заметки!"
    )

    return all_commands