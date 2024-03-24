import requests

from src.data.UserData import AddUser, UserToken, UserInfo, UserTelegram, UserUpdate
from src.others.config_for_bot import ConfigUserData
from src.others.profile_set_info import set_name


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

        data_to_add: dict = data_to_add.get_dict()
        set_name(firstName=data_to_add.get("firstName"), lastName=data_to_add.get("lastName"))

        req = requests.Session()
        req = req.post(
            url=self.app_url+"/telegram/auth",
            json=data_to_add
        )

        ConfigUserData.cookies = dict(req.headers)
        ConfigUserData.token = req.json()

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
        session = requests.Session()
        req = session.post(
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

    def del_user_by_token(self) -> bool:
        """
        Del user by token
        :param user_token:
        :return:
        """

        req = requests.delete(
            url=self.app_url + "/user",
            headers={
                "Authorization": "Bearer " + ConfigUserData.token
            }
        )

        if req.status_code == 200:
            return True
        return False

    def get_new_token(self) -> None:
        """
        Take new token
        :param old_token:
        :return:
        """

        req = requests.Session()
        req = req.get(
            url=self.app_url+"/auth/refresh",
            headers=ConfigUserData.cookies
        )

        ConfigUserData.cookies = req.headers
        ConfigUserData.token = req.text

    def update_user(self, data_to_update: UserUpdate) -> bool:
        """
        Update user
        :param data_to_update:
        :return:
        """

        req = requests.Session()
        req = req.put(
            url=self.app_url + "/user",
            headers={
                "Authorization": "Bearer " + ConfigUserData.token
            },
            json=data_to_update.get_dict()
        )

        if req.status_code in (200, 201): return True
        return False