from abc import ABC, abstractmethod


class AbstractParser(ABC):
    text: str
    path: str

    @abstractmethod
    def extract_all_text(self):
        pass
