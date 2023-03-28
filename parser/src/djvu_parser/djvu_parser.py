import djvu.sexpr
import djvu.decode

from src.abstract_parser.abstract_parser import AbstractParser
from src.utils.path_checker import check_path

text_list = list()


class ParserDJVU(AbstractParser):
    text: str = ''

    def __init__(self, path: str):
        check_path(path, suffix='.djvu')
        self.path = path
        self.text_list = list()

    def get_text(self, words, level=0):
        if isinstance(words, djvu.sexpr.ListExpression):
            if len(words) == 0:
                return
            for child in words[5:]:
                self.get_text(child, level + 1)
        else:
            self.text_list.append(words.value)

    def extract_all_text(self) -> str:
        with open(self.path, 'rb'):
            djvu_doc = djvu.decode.Context().new_document(
                djvu.decode.FileURI(self.path))
            djvu_doc.decoding_job.wait()

            for page in djvu_doc.pages:
                words = page.text.sexpr
                self.get_text(words)

            self.text = (' '.join(self.text_list)).replace('  ', '')

        if self.text:
            return self.text
