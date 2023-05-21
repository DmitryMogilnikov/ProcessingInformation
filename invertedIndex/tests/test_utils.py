import pandas as pd
import pytest

from src.utils import has_cyrillic, has_english, drop_errors


@pytest.mark.parametrize(
    'text, expected_result',
    (
        ["test", False],
        ["", False],
        ["test_123", False],
        ["test_тест", True],
        ["test_ТЕСТ", True],
        ["TEST_test", False],
        ["!@#$%^&*()_+=/|~,", False],
    ),
)
def test_has_cyrillic(text: str, expected_result: bool) -> None:
    assert has_cyrillic(text) == expected_result


@pytest.mark.parametrize(
    'text, expected_result',
    (
        ["test", True],
        ["", False],
        ["test_123", True],
        ["TEST_тест", True],
        ["тест_ТЕСТ", False],
        ["!@#$%^&*()+=/|~,", False]
    ),
)
def test_has_english(text: str, expected_result: bool) -> None:
    assert has_english(text) == expected_result


def test_drop_errors() -> None:
    data_with_errors = pd.DataFrame({
        'link': ['test.com', 'test.ru', 'error1', 'test.kz', 'error2'],
        'text': ['test.com', 'test.ru', 'error1', 'test.kz', 'error2'],
    })
    errors = [2, 4]

    expected_data = pd.DataFrame({
        'link': ['test.com', 'test.ru', 'test.kz'],
        'text': ['test.com', 'test.ru', 'test.kz'],
    })

    result_data = drop_errors(data_with_errors, errors)
    assert (result_data == expected_data).all
