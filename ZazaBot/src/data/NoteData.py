from dataclasses import dataclass


@dataclass
class AddNote:
    title: str
    text: str

    def get_dict(self) -> dict:
        return {
            "title": self.title,
            "text": self.text
        }


@dataclass
class UpdateNote:
    guid: str
    title: str
    text: str

    def get_dict(self) -> dict:
        return {
            "guid": self.guid,
            "title": self.title,
            "text": self.text
        }