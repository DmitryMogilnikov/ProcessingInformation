from djvu import decode, const

from src.abstract_parser.abstract_parser import AbstractParser
from src.utils.path_checker import check_path


class ParserDJVU(AbstractParser):
    text: str = ''

    def __init__(self, path: str):
        check_path(path, suffix='.djvu')
        self.path = path

    def extract_all_text(self) -> str:
        with open(self.path, 'rb'):
            djvu_doc = decode.Context().new_document(decode.FileURI(self.path))
            djvu_doc.decoding_job.wait()

            text_list = list()

            for page in djvu_doc.pages:
                words = page.text.sexpr.value
                words_len = len(words)
                for elem in range(5, words_len):
                    if const.get_text_zone_type(words[elem][0]) is const.TEXT_ZONE_WORD:
                        text_list.append(words[elem][5])
                    elif const.get_text_zone_type(words[elem][0]) is const.TEXT_ZONE_CHARACTER:
                        text_list.append(words[elem][5])

        self.text = ''.join(text_list)
        print(self.text)

        if self.text:
            return self.text
