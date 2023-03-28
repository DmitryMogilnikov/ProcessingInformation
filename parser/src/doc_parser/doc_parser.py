import textract

from src.abstract_parser.abstract_parser import AbstractParser
from src.utils.path_checker import check_path


class ParserDOC(AbstractParser):
    text: str = ''

    def __init__(self, path: str):
        check_path(path, suffix='.doc')
        self.path = path

    def extract_all_text(self) -> str:
        self.text = textract.process(self.path).decode("utf-8")

        if self.text:
            return self.text
