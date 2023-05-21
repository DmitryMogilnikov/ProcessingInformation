import json
import os
from typing import Dict
import pandas as pd



def get_data(path_to_file: str):
    if (os.stat(path_to_file).st_size == 0):
        raise ValueError("File is empty!") # проверяем, что файл не пустой
    
    df = pd.read_csv(path_to_file, header = None)
    df = df[~df[0].str.endswith('.pdf/')] # удаляем строки из датафрейма, которые пдфки

    if df.empty:
        raise ValueError("No links in file!")
    
    df = df.reset_index(drop=True) # сбрасываем индексы чтобы не возникало ошибок
    return df

def save_to_csv(df: pd.DataFrame, file_path: str) -> None:
    df.to_csv(file_path)


def save_inverted_index_json(data: Dict, file_path: str) -> None:
    with open(file_path, 'w') as file: # сохраняем словарь в json
        json.dump(data, file)


def get_inverted_index_json(file_path: str) -> Dict:
    with open(file_path, 'r') as file: #считываем словарь из json
        data =  json.loads(file.read())
    return data

