import os

from pathlib import Path
from typing import Optional

from env import ROOT_DIR
from src.abstract_parser.abstract_parser import AbstractParser


def write_txt(
        abstract_parser: AbstractParser,
        output_path: Optional[str] = None
) -> None:
    if output_path is None:
        output_path = get_output_path(abstract_parser.path)
    with open(output_path, 'w') as txt_file:
        txt_file.write(abstract_parser.text)


def get_output_path(path: str) -> str:
    output_path = f"{ROOT_DIR}/files/outputs/"
    if not os.path.exists(output_path):
        os.makedirs(output_path)
    output_file_name = Path(path).stem
    return f'{output_path}{output_file_name}.txt'

