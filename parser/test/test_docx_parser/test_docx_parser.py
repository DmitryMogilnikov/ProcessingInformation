import pytest

from src.docx_parser.docx_parser import ParserDOCX


@pytest.mark.parametrize("input_path,path_to_expected_text", [("./test/testcase_files/test1.docx",   "./test/testcase_files/output/expected_docx_output1.txt")])
def test_extract_all_text_always_returns_expected(input_path : str, path_to_expected_text : str):
        parser : ParserDOCX = ParserDOCX(input_path)

        text : str = parser.extract_all_text()

        with open(path_to_expected_text, "r") as f:
            expected_text = f.read()
        assert text == expected_text