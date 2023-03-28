import docx
import re

from src.abstract_parser.abstract_parser import AbstractParser
from src.utils.path_checker import check_path


class ParserDOCX(AbstractParser):
    text: str = ''

    def __init__(self, path: str):
        check_path(path, suffix='.docx')
        self.path = path

    def extract_all_text(self) -> str:
        document = docx.Document(self.path)
        self.text = '\n'.join(par.text for par in document.paragraphs)
        self.text = '\n'.join(
            line.strip() for line in re.findall(
                r'.{1,100}(?:\s+|$)',
                self.text
            )
        )

        if self.text:
            return self.text
