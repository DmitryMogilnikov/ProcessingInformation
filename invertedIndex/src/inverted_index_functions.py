from collections import defaultdict
import nltk
import pandas as pd
import pymorphy2

from invertedIndex.src.utils import has_cyrillic, has_english

nltk.download('stopwords')
nltk.download('wordnet')
nltk.download('punkt')
russian_stopwords = nltk.corpus.stopwords.words('russian')
english_stopwords = nltk.corpus.stopwords.words('english')
word_tokenizer = nltk.WordPunctTokenizer()
morph = pymorphy2.MorphAnalyzer()


def lemmatization(df: pd.DataFrame) -> pd.DataFrame:
    list_errors = []
    for i in range(df.shape[0]):
        sub_string = df.iloc[i, 2]
        if not isinstance(sub_string, str):
            continue
        # делаем маленькие буквы, разбиваем текст на токены
        # (из 3-го столбика по каждой строке)
        word_list = nltk.word_tokenize(sub_string.lower())

        # удаляем слова, которые в англ и русс стоп-словах
        # или если они числовые (строчки)
        word_list = [word for word in word_list if (
            word not in russian_stopwords and
            word not in english_stopwords and
            not word.isnumeric() and
            (has_cyrillic(word) or has_english(word))
            )
        ]

        try:
            # нормальная форма токена
            word_list = [morph.parse(j)[0].normal_form for j in word_list]
            # заменяем 3 столбик на лемматизированный текст
            df.iloc[i, 2] = ' '.join(word_list)
        except:
            # в лист ошибок добавляем номер строки, в которой ошибка
            list_errors.append(i)

    return df, list_errors


def create_inverted_index_dict(df: pd.DataFrame) -> pd.DataFrame:
    inverted_index_dict = defaultdict(set)

    # цикл, который обрабатывает строки
    for idx in range(df.shape[0]):
        if isinstance(df.iloc[idx][2], str):
            tokens_list = list()
            # если строка не встречается в листе ошибок
            # (то что не получилось обработать на лемматизации)
            tokens_list = df.iloc[idx][2].split()
            # тут цикл, который обрабатывает токены (он смотрит, есть ли
            # какая-то запись в словаре, соответствующая конкретному слову)
            for token in tokens_list:
                inverted_index_dict[token].add(idx)
    # перобразуем set в list, для того, чтобы могли преобразовать в json
    for keys, vals in inverted_index_dict.items():
        inverted_index_dict[keys] = list(vals)

    return inverted_index_dict
