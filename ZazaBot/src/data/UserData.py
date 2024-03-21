from dataclasses import dataclass
from typing import Dict


@dataclass
class UserInfo:
    firstName: str
    lastName: str
    description: str
    country: str
    photo: str

    def get_dict(self) -> dict:
        return {
            "firstName": self.firstName,
            "lastName": self.lastName,
            "description": self.description,
            "country": self.country,
            "photo": self.photo
        }

@dataclass
class AddUser:
    login: str
    info: Dict
    telegramId: int
    password: str

    def get_dict(self) -> dict:
        return {
            "login": self.login,
            "info": self.info,
            "telegramId": self.telegramId,
            "password": self.password
        }


@dataclass
class UserToken:
    login: str
    password: str

    def get_dict(self) -> dict:
        return {
            "login": self.login,
            "password": self.password
        }