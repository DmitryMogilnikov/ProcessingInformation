# TO DO: доделать проверку при инициализации пути, если будет время


from pathlib import Path

from src.abstract_parser.abstract_parser import AbstractParser
from src.pdf_parser.pdf_parser import ParserPDF
from src.docx_parser.docx_parser import ParserDOCX
from src.html_parser.html_parser import ParserHTML
from src.utils.txt_writer import write_txt
from utils.path_checker import check_path
if __name__ == '__main__':
    type_parser = {
        '.pdf': ParserPDF,
        '.docx': ParserDOCX,
        '.html': ParserHTML,
    }

    path = input('File path to parse:')
    check_path(path, list(type_parser.keys()))

    def get_parser(file_path) -> AbstractParser:
        parser = type_parser.get(Path(file_path).suffix)
        parser.path = file_path
        return parser

    par = get_parser(path)
    par.extract_all_text()
    write_txt(par)
