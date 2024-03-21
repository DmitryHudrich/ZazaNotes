import requests

from ZazaBot.src.data.UserData import AddUser, UserToken, UserInfo


class User:

    def __init__(self):
        """
        Initialize data
        """

        self.app_url = "http://localhost:5000"


    def add_user(self, data_to_add: AddUser) -> bool:
        """
        Add user
        :param data_to_add:
        :return:
        """
        req = requests.post(
            url=self.app_url+"/auth/reg",
            json=data_to_add.get_dict()
        )
        print(req)
        if req.status_code == 200:
            return True
        return False


    def get_user_token(self, data_to_add: UserToken) -> bool | str:
        """
        Gett user token
        :param data_to_add:
        :return:
        """

        req = requests.post(
            url=self.app_url+"/auth/login",
            json=data_to_add.get_dict()
        )

        if req.status_code == 200:
            response = req.json()
            return response
        return False


    def get_userinfo_by_token(self, user_token: str) -> bool | str:
        """
        Get user info by token
        :param user_token:
        :return:
        """

        req = requests.get(
            url=self.app_url+"/user",
            headers={
                "Authorization": "Bearer " + user_token
            }
        )

        if req.status_code == 200:
            return req.json()
        return False


    def del_user_by_token(self, user_token: str) -> bool:
        """
        Del user by token
        :param user_token:
        :return:
        """

        req = requests.delete(
            url=self.app_url + "/user",
            headers={
                "Authorization": "Bearer " + user_token
            }
        )

        print(req)

        if req.status_code == 200:
            return True
        return False


    def get_user_info_by_telegram_id(self, tg_id: int, user_token: int):
        """
        Get user info by tg_id
        :param tg_id:
        :return:
        """

        req = requests.get(
            url=self.app_url+"/auth/telegram",
            params={
                "id": tg_id
            },
            headers={
                "Authorization": "Bearer " + user_token
            },
        )

        print(req.content)

        return req

obj_to_test = User()

data_us = UserToken(
    "thedarkfox98",
    "454590"
)

print(obj_to_test.get_user_info_by_telegram_id(234324234234234, obj_to_test.get_user_token(data_to_add=data_us)))
