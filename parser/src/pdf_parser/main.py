from pdf_parser import ParserPDF
from src.utils.txt_writer import write_txt

if __name__ == '__main__':
    path = input('File path to parse:')
    pdf_parser = ParserPDF(path)

    pdf_parser.extract_all_text()
    write_txt(pdf_parser)
