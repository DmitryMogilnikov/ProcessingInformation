import re
from typing import List

import pandas as pd


# проверяем, слово содержит русские буквы или нет
def has_cyrillic(text: str) -> bool:
    return bool(re.search('[а-яА-Я]', text))


# проверяем, слово содержит английские буквы или нет
def has_english(text: str) -> bool:
    return bool(re.search('[a-zA-Z]', text))


def drop_errors(df: pd.DataFrame, list_errors: List) -> pd.DataFrame:
    df = df.drop(index=list_errors)
    df = df.reset_index(drop=True)
    return df
