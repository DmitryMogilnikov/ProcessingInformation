from pathlib import Path
from typing import Union


def check_path(path: str, suffix: Union[str, list]):
    if not Path(path).is_file():
        raise FileNotFoundError("Incorrect path to file")
    if Path(path).suffix != suffix:
        raise TypeError("File has incorrect type")
