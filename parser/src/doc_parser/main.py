from doc_parser import ParserDOC
from src.utils.txt_writer import write_txt

if __name__ == '__main__':
    path = input('File path to parse:')
    doc_parser = ParserDOC(path)

    doc_parser.extract_all_text()
    write_txt(doc_parser)
