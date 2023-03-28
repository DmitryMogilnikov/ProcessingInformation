from djvu_parser import ParserDJVU
from src.utils.txt_writer import write_txt

if __name__ == '__main__':
    path = input('File path to parse:')
    djvu_parser = ParserDJVU(path)

    djvu_parser.extract_all_text()
    write_txt(djvu_parser)
