import requests

from ZazaBot.src.api.UserAPI import User, UserTelegram
from ZazaBot.src.data.NoteData import AddNote
from ZazaBot.src.others.config_for_bot import ConfigUserData


class Note(User):

    def __init__(self):
        #Initializa data from User class
        super().__init__()

    def add_note(self, data_to_add: AddNote):
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

        print(req.content)
        if req.status_code == 200:
            return True
        return False