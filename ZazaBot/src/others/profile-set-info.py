import logging

from typing import List


async def set_name(firstName: str, lastName: str) -> List[str]:
    """
    replace special symbols in string's
    :param firstName:
    :param lastName:
    :return:
    """

    logging.info(msg="Request to change name")
    special_symbols = ["<", ">", "/"]
    to_replace_symbol = "."

    for symbol in special_symbols:
        firstName.replace(symbol, to_replace_symbol)
        lastName.replace(symbol, to_replace_symbol)

    return [firstName, lastName]