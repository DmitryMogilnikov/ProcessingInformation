import pytest

from src.html_parser.html_parser import ParserHTML


@pytest.mark.parametrize("input_path,path_to_expected_text", [("./test/testcase_files/test1.html",   "./test/testcase_files/output/expected_html_output1.txt"), 
                                                              ("./test/testcase_files/test2.html",   "./test/testcase_files/output/expected_html_output2.txt")])
def test_extract_all_text_always_returns_expected(input_path : str, path_to_expected_text : str):
        parser : ParserHTML = ParserHTML(input_path)

        text : str = parser.extract_all_text()

        with open(path_to_expected_text, "r") as f:
            expected_text = f.read()
        assert text == expected_text

@pytest.mark.parametrize("input_path,path_to_expected_links", [("./test/testcase_files/test1.html",   "./test/testcase_files/output/expected_html_links1.txt"), 
                                                               ("./test/testcase_files/test2.html",   "./test/testcase_files/output/expected_html_links2.txt")])
def test_extract_all_links_always_returns_expected(input_path : str, path_to_expected_links : str):
        parser : ParserHTML = ParserHTML(input_path)

        links : str = parser.extract_all_links()

        with open(path_to_expected_links, "r") as f:
            expected_links = f.read()
        assert links == expected_links