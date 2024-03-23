import logging

import requests

from src.api.UserAPI import User, UserTelegram
from src.data.NoteData import AddNote, UpdateNote
from src.others.config_for_bot import ConfigUserData


class Note(User):

    def __init__(self):
        #Initializa data from User class
        super().__init__()

    def add_note(self, data_to_add: AddNote, flag=True):
        """
        Add note
        :param data_to_add:
        :return:
        """

        req = requests.post(
            url=self.app_url+"/user/notes",
            headers={
              "Authorization": "Bearer " + ConfigUserData.token
            },
            json=data_to_add.get_dict()
        )

        if req.status_code == 200:
            return True
        else:
            if flag is not True:

                User().get_new_token()
                flag=False
                self.add_note(data_to_add=data_to_add)
            else:
                logging.info(msg="Doesnt to create note for user")

    def get_notes(self) -> list:
        """
        Get notes for user token
        :return:
        """

        req = requests.Session()
        req = req.get(
            url=self.app_url + "/user/notes",
            headers={
                "Authorization": "Bearer " + ConfigUserData.token
            }
        )

        return list(req.json())

    def del_note(self, id_note: str) -> bool:
        """
        Del note by id
        :param id_note:
        :return:
        """

        req = requests.Session()
        print(ConfigUserData.token)
        req = req.delete(
            url=self.app_url + "/user/notes/"+id_note,
            headers = {
                "Authorization": "Bearer " + ConfigUserData.token
            }
        )

        print(req)
        print(req.status_code)

        if req.status_code in (200, 201, 204):
            return True
        return False

    def update_note(self, data_to_put: UpdateNote) -> bool:
        """
        Update note by id
        :param data_to_put:
        :return:
        """

        req = requests.Session()
        req = req.put(
            url=self.app_url + "/user/notes",
            headers={
                "Authorization": "Bearer " + ConfigUserData.token
            },
            json=data_to_put.get_dict()
        )


        if req in (200, 201, 204): return True
        return False