import pytest

from src.doc_parser.doc_parser import ParserDOC


@pytest.mark.parametrize("input_path,path_to_expected_text", [("./test/testcase_files/test1.doc",   "./test/testcase_files/output/expected_doc_output1.txt")])
def test_extract_all_text_always_returns_expected(input_path : str, path_to_expected_text : str):
        parser : ParserDOC = ParserDOC(input_path)

        text : str = parser.extract_all_text()

        with open(path_to_expected_text, "r") as f:
            expected_text = f.read()
        assert text == expected_text