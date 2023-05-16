import json
import re

import pandas as pd
import pymorphy2
from tqdm.notebook import tqdm as tn

import nltk
nltk.download('stopwords')
nltk.download('wordnet')
nltk.download('punkt')
russian_stopwords = nltk.corpus.stopwords.words('russian')
english_stopwords = nltk.corpus.stopwords.words('english')
word_tokenizer = nltk.WordPunctTokenizer()

def has_cyrillic(text: str) -> bool: # проверяем, слово содержит русские буквы или нет 
    return bool(re.search('[а-яА-Я]', text))


def has_english(text: str) -> bool: # проверяем, слово содержит английские буквы или нет 
    return bool(re.search('[a-zA-z]', text))

morph = pymorphy2.MorphAnalyzer()
def lemmatization(data_spbu):
    df = data_spbu.copy()
    list_errors = []
    for i in range(df.shape[0]):
        sub_string = data_spbu.iloc[i, 2]
        if type(sub_string) is str:
            word_list = nltk.word_tokenize(sub_string.lower()) # делаем маленькие буквы, разбиваем текст на токены (из 3-го столбика по каждой строке)

            word_list = [word for word in word_list if  (
                word not in russian_stopwords and
                word not in english_stopwords and
                not word.isnumeric() and 
                (has_cyrillic(word) or has_english(word))
                )
            ] # удаляем слова, которые в англ и русс стоп-словах или если они числовые (строчки)

            try:
                word_list = [morph.parse(j)[0].normal_form for j in word_list] # нормальная форма токена
                df.iloc[i, 2] = ' '.join(word_list) # заменяем 3 столбик на лемматизированный текст
            except:
                list_errors.append(i) # в лист ошибок добавляем номер строки, в которой ошибка 
    
    return df, list_errors

def drop_errors(df, list_errors):
    return df.drop(index=list_errors) # удаляем все строки с ошибками

def create_dict(df):
    my_dict = {}

    for idx in range(df.shape[0]):  # цикл, который обрабатывает строки
        if isinstance(df.iloc[idx][3], str):
            tokens_list = list()
            tokens_list = df.iloc[idx][3].split() # если строка не встречается в листе ошибок (то что не получилось обработать на лемматизации)
            for token in tokens_list: # тут цикл, который обрабатывает токены (он смотрит, есть ли какая-то запись в словаре, соответствующая конкретному слову)
                if token in my_dict: # если ключ уже есть, то добавляем в список ссылку
                    my_dict[token].add(int(df.iloc[idx][0]))
                else:  
                    my_dict[token] = {int(df.iloc[idx][0])} # пыталась тут добавить ключ и значение (то есть ссылку), если их нет в словаре 
    for keys, vals in my_dict.items():
        my_dict[keys] = list(vals)

    
    return my_dict


