import os

import pandas as pd
import pytest

from src.work_with_files import (
    get_data,
    get_inverted_index_json,
    save_inverted_index_json,
    save_to_csv,
)


def test_get_data() -> None:
    expected_data = pd.DataFrame(
        {
            0: [
                "https://www.example.com/",
                "https://www.test.com/",
                "https://www.demo.com/",
                "https://www.sample.com/",
                "https://www.data.com/",
            ],
            1: [
                "2023-05-18",
                "2023-05-19",
                "2023-05-20",
                "2023-05-21",
                "2023-05-22",
            ],
            2: [
                "Привет, это контент 1.",
                "Здесь находится контент 2.",
                "Это третий контент для примера.",
                "Вот еще один контент для теста.",
                "Последний контент в этом наборе данных.",
            ],
        }
    )

    result_data = get_data("tests/test_data/test_content.csv")
    assert (result_data[0] == expected_data[0]).all


def test_get_pdf_only() -> None:
    with pytest.raises(ValueError):
        get_data("tests/test_data/test_pdf_only.csv")


def test_get_data_empty() -> None:
    with pytest.raises(ValueError):
        get_data("tests/test_data/test_empty.csv")


def test_save_to_csv() -> None:
    data = pd.DataFrame(
        {
            0: [
                "https://www.example.com/",
                "https://www.test.com/",
                "https://www.demo.com/",
                "https://www.sample.com/",
                "https://www.data.com/",
            ],
            1: [
                "2023-05-18",
                "2023-05-19",
                "2023-05-20",
                "2023-05-21",
                "2023-05-22",
            ],
            2: [
                "Привет, это контент 1.",
                "Здесь находится контент 2.",
                "Это третий контент для примера.",
                "Вот еще один контент для теста.",
                "Последний контент в этом наборе данных.",
            ],
        }
    )
    file_path = "tests/test_data/save_to_csv_test.csv"
    save_to_csv(df=data, file_path=file_path)
    assert os.path.isfile(file_path) is True
    os.remove(file_path)


def test_save_inverted_index_json() -> None:
    data = {
        "сначала": [0, 1, 2, 3, 4, 5],
        "было": [1, 3, 5, 7, 9],
        "слово": [2, 4, 8, 16, 32],
    }
    file_path = "tests/test_data/save_inverted_index_json_test.csv"
    save_inverted_index_json(file_path=file_path, data=data)
    assert os.path.isfile(file_path) is True


def test_get_inverted_index_json() -> None:
    data = {
        "сначала": [0, 1, 2, 3, 4, 5],
        "было": [1, 3, 5, 7, 9],
        "слово": [2, 4, 8, 16, 32],
    }
    file_path = "tests/test_data/save_inverted_index_json_test.csv"
    assert get_inverted_index_json(file_path=file_path) == data
    os.remove(file_path)
