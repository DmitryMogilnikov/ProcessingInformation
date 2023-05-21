from work_with_files import get_inverted_index_json
from work_with_files import save_inverted_index_json
from typing import Any, Iterable, List
import math


index = get_inverted_index_json("data\spbu_result.json")
save_file = save_inverted_index_json(data=index, file_path="data\spbu_result.json",)


def gamma_encode(number): # запускает гамма кодирование 
        binary = bin(number)[2:]  # Преобразование числа в двоичное представление
        unary_code = '0' * (len(binary) - 1) + '1'  # Унарный код
        return unary_code + binary[1:]


def gamma_encode_seq(seq: Iterable[int]) -> int:
    encoded_seq = 0
    for s in seq:
        encoded_s = gamma_encode(s)
        num_of_digits = len(str(encoded_s)) # длина строки - результата гамма-кодирования для элемента 
        encoded_seq = encoded_seq * (10**num_of_digits) + encoded_s 
    return encoded_seq


def gamma_decode(encoded_seq: int) -> List[int]:  # x = 1110
    encoded_seq = str(encoded_seq).replace("9", "0") # Возвращает копию со всеми вхождениями старой подстроки, замененной новой ("old", "new")
    decoded_seq = []
    while len(encoded_seq) > 0:
        N = encoded_seq.find("0")  # for '11110' it is 4
        bin_decoded = "1" + encoded_seq[N + 1 : 2 * N + 1]  # add most significant bit
        encoded_seq = encoded_seq[2 * N + 1 :]
        # x = '1' + x[num_of_leading_ones + 1:]
        decoded_seq.append(int(bin_decoded, base=2))
    return decoded_seq



def delta_encode(doc_ids): # зависит от гамма кодирования 
        delta_encoded = []
        for i in range(len(doc_ids)):
            if i == 0:
                delta_encoded.append(doc_ids[i])
            else:
                delta = doc_ids[i] - doc_ids[i-1]
                bit_length = int(math.log2(delta)) + 1  # Длина числа в битах
                unary_length = int(math.log2(bit_length + 1)) + 1  # Длина унарного кода
                gamma_code = gamma_encode(bit_length)  # Кодирование длины числа
                unary_code = '0' * unary_length + '1'  # Унарный код
                delta_encoded.append(gamma_code + unary_code + format(delta, '0' + str(bit_length) + 'b'))
        return delta_encoded

def delta_decode(delta_encoded): 
        doc_ids = []
        current_id = 0
        for code in delta_encoded:
            gamma_length = code.index('1') + 1  # Длина числа
            unary_length = code[:gamma_length].count('0')  # Длина унарного кода
            delta_length = gamma_length + unary_length + 1  # Длина разности
            delta = int(code[gamma_length + unary_length + 1:delta_length], 2)  # Декодирование разности
            current_id += delta  # Восстановление идентификатора документа
            doc_ids.append(current_id)
        return doc_ids

def compress_index(path): # запускает дельта кодирование 
    index = get_inverted_index_json(path)
    words = list(index.keys())
    words_count = len(words)
    for idx in range(words_count):
        print('Finished creating index {} % of all events'.format(int(100*(idx/words_count))))
        doc_ids = index[words[idx]]
        doc_ids.sort()  # Сортировка идентификаторов документов
        delta_encoded = delta_encode(doc_ids)  # Применение дельта кодирования Элиаса
        index[words[idx]] = delta_encoded
    save_inverted_index_json(data=index, file_path=r"data\spbu_result_encoding1.json")


compress_index(r"data\spbu_result_encoding1.json")