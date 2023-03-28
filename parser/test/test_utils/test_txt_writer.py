import pytest

from typing import List
from tempfile import gettempdir 
from os import urandom, remove
from os.path import join

from test.test_utils.parser_mock import ParserMock
from src.utils import txt_writer

from env import ROOT_DIR




@pytest.mark.parametrize("input_path,expected_output_path", [("/path/to/somewhere/input.ext",   f"{ROOT_DIR}/files/outputs/input.txt"            ),
                                                             ("/file_from_root.doc",            f"{ROOT_DIR}/files/outputs/file_from_root.txt"   ),
                                                             ("just_file.html",                 f"{ROOT_DIR}/files/outputs/just_file.txt"        ),
                                                             ("./without_extension",            f"{ROOT_DIR}/files/outputs/without_extension.txt"),
                                                             ("../../files/outputs/input.djvu", f"{ROOT_DIR}/files/outputs/input.txt"            )])
def test_get_output_path_always_returns_expected(input_path: str, expected_output_path: str):
    output_path : str = txt_writer.get_output_path(input_path)
    assert output_path == expected_output_path

def test_write_txt_with_given_path_writes_file():
    parser : ParserMock = ParserMock()
    path : str = join(gettempdir(), urandom(32).hex())

    txt_writer.write_txt(parser, path)

    with open(path) as file:
        file_content = file.read()
    remove(path)
    assert file_content == parser.text

def test_write_txt_without_path_gets_path_from_parser():
    parser : ParserMock = ParserMock()
    output_path = f"{ROOT_DIR}/files/outputs/test_input.txt"

    txt_writer.write_txt(parser)

    with open(output_path) as file:
        file_content = file.read()
    remove(output_path)
    assert file_content == parser.text