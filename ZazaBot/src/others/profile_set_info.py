import logging

from typing import List


def set_name(firstname: str, lastname: str) -> List[str]:
    """
    replace special symbols in string's
    :param firstname:
    :param lastname:
    :return:
    """

    if lastname == "None" or lastname == "" or lastname is None:
        lastname = ""

    logging.info(msg="Request to change name")
    special_symbols = ["<", ">", "/"]
    to_replace_symbol = "."

    for symbol in special_symbols:
        firstname.replace(symbol, to_replace_symbol)
        lastname.replace(symbol, to_replace_symbol)

    return [firstname, lastname]