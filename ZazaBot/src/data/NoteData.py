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