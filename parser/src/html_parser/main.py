from html_parser import ParserHTML
from src.utils.txt_writer import write_txt

if __name__ == '__main__':
    path = input('File path to parse:')
    html_parser = ParserHTML(path)

    html_parser.extract_all_text()
    write_txt(html_parser)

    html_parser.extract_all_links()
    write_txt(html_parser, "../../files/outputs/test_links.txt")
