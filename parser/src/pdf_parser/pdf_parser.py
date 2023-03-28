import io

from pdfminer.layout import LAParams
from pdfminer.converter import TextConverter
from pdfminer.pdfinterp import PDFPageInterpreter, PDFResourceManager
from pdfminer.pdfpage import PDFPage

from src.abstract_parser.abstract_parser import AbstractParser
from src.utils.path_checker import check_path


class ParserPDF(AbstractParser):
    text: str = ''

    def __init__(self, path: str):
        check_path(path, suffix='.pdf')
        self.path = path

    def extract_all_text(
            self,
            caching: bool = True,
            codec: str = "utf-8",
    ) -> str:
        laparams = LAParams()

        with open(self.path, 'rb') as file:
            resource_manager = PDFResourceManager()
            output_string = io.StringIO()
            converter = TextConverter(
                resource_manager,
                output_string,
                codec=codec,
                laparams=laparams
            )
            interpreter = PDFPageInterpreter(resource_manager, converter)

            for page in PDFPage.get_pages(file, caching=caching):
                interpreter.process_page(page)
            self.text = output_string.getvalue()

        if self.text:
            return self.text
