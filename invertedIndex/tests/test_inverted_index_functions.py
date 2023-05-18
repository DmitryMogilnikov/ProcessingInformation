import pandas as pd

from src.inverted_index_functions import (create_inverted_index_dict,
                                          lemmatization)
from src.utils import drop_errors
from src.work_with_files import get_data


def test_lemmatization() -> None:
    file_path = "tests/test_data/test_content.csv"
    df = pd.read_csv(file_path, header=None)
    result_df, result_list_errors = lemmatization(df=df)
    exception_result = pd.DataFrame(
        {
            0: [
                "https://www.example.com/",
                "https://www.test.com/",
                "https://www.demo.com/",
                "https://www.data.pdf/",
                "https://www.sample.com/",
                "https://www.data.com/",
            ],
            1: [
                "2023-05-18",
                "2023-05-19",
                "2023-05-20",
                "2023-05-22",
                "2023-05-21",
                "2023-05-22",
            ],
            2: [
                "Привет, это контент 1.",
                "находиться контент",
                "это третий контент пример",
                "pdf файл который должный удалённый",
                "контент тест",
                "последний контент набор данные",
            ],
        }
    )
    assert result_list_errors == []
    assert (result_df == exception_result).all


def test_lemmatization_with_clear_pdf() -> None:
    file_path = "tests/test_data/test_content.csv"
    df = get_data(file_path)

    result_df, result_list_errors = lemmatization(df=df)
    print(result_df)
    exception_result = pd.DataFrame(
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
                "находиться контент",
                "это третий контент пример",
                "контент тест",
                "последний контент набор данные",
            ],
        }
    )
    assert result_list_errors == []
    assert (result_df == exception_result).all


def test_create_inverted_index_dict() -> None:
    file_path = "tests/test_data/test_content.csv"
    df = get_data(file_path)

    result_df, result_list_errors = lemmatization(df=df)
    result_df = drop_errors(df=result_df, list_errors=result_list_errors)
    inverted_index_dict = create_inverted_index_dict(result_df)
    exception_result = {
        "привет": [0],
        "это": [0, 2],
        "контент": [0, 1, 2, 3, 4],
        "находиться": [1],
        "третий": [2],
        "пример": [2],
        "тест": [3],
        "последний": [4],
        "набор": [4],
        "данные": [4],
    }
    for ind in inverted_index_dict:
        assert inverted_index_dict[ind] == exception_result[ind]
