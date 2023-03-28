import re

from bs4 import BeautifulSoup

from src.abstract_parser.abstract_parser import AbstractParser
from src.utils.path_checker import check_path


class ParserHTML(AbstractParser):
    text: str = ''

    def __init__(self, path):
        check_path(path, suffix='.html')
        self.path = path

    @staticmethod
    def tag_visible(element):
        tag_filter = ['style', 'script', 'head', 'title', 'meta', '[document]']
        if element.parent.name in tag_filter:
            return False
        elif re.match(r"[\s\r\n]+", str(element)):
            return False
        return True

    def extract_all_text(self):
        self.text = ''
        with open(self.path, "r") as file:
            contents = file.read()

            soup = BeautifulSoup(contents, 'lxml')
            texts = soup.findAll(text=True)
            visible_texts = filter(self.tag_visible, texts)
            self.text = ("\n".join(t.strip() for t in visible_texts))

        if self.text:
            return self.text

    def extract_all_links(self) -> str:
        self.text = ''
        with open(self.path, "r") as file:
            contents = file.read()
            soup = BeautifulSoup(contents, 'lxml')
            links = []

            for link in soup.find_all('a'):
                links.append(link.get('href'))

        self.text = '\n'.join(links)
        return self.text
