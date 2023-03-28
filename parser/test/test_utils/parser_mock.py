from src.abstract_parser.abstract_parser import AbstractParser


class ParserMock(AbstractParser):
    text: str = "Test output file content"
    path: str = "./test_input.ext"

    def extract_all_text(self):
        pass