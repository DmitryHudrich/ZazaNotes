import requests

from ZazaBot.src.api.UserAPI import User, UserTelegram
from ZazaBot.src.data.NoteData import AddNote


class Note(User):

    def __init__(self):
        #Initializa data from User class
        super().__init__()

    def add_note(self, data_to_add: AddNote, data_user: UserTelegram):
        """
        Add note
        :param data_to_add:
        :return:
        """

        usr_token = self.add_user(data_to_add=data_user)
        req = requests.post(
            url=self.app_url+"/user/notes",
            headers={
              "Authorization": "Bearer " + usr_token
            },
            json=data_to_add.get_dict()
        )

        print(req.content)
        if req.status_code == 200:
            return True
        return False


note = Note()
print(note.add_note(
    data_to_add=AddNote(
        "sdasd",
        "dada"
    ),
    data_user=UserTelegram(
        "Silmarion",
        "null",
        912824014,
        "AgACAgIAAxkDAANNZf70oeZn1BLvcwzs8pAh_uNaWt4AAqmnMRvOlmg2RZUNjmfEeRUBAAMCAANhAAM0BA"
    )
))