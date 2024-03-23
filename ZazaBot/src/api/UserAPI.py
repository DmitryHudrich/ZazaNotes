import requests

from ZazaBot.src.data.UserData import AddUser, UserToken, UserInfo, UserTelegram


class User:

    def __init__(self):
        """
        Initialize data
        """

        self.app_url = "http://localhost:5000"

    def add_user(self, data_to_add: UserTelegram) -> str:
        """
        Add user
        :param data_to_add:
        :return:
        """

        req = requests.post(
            url=self.app_url+"/telegram/auth",
            json=data_to_add.get_dict()
        )

        if req.status_code == 201:
            return "User is created"
        elif req.status_code == 200:
            return "User was created"
        else:
            return "Error"

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

        if req.status_code in (200, 201):
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

        if req.status_code in (200, 201):
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

    def get_user_info_by_telegram_id(self, tg_id: int) -> bool:
        """
        Get user info by tg_id
        :param tg_id:
        :return:
        """

        req = requests.post(
            url=self.app_url+"/telegram/auth",
            params={
                "id": tg_id
            }
        )
        print(req)
        print(req.content)

        if len(req.content):
            return True
        return False

    def get_new_token(self, old_token: str) -> str:
        """
        Take new token
        :param old_token:
        :return:
        """

