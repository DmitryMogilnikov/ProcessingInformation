import math
from typing import Any, Iterable, List

from BitVector import BitVector

from work_with_files import get_inverted_index_json, save_inverted_index_json


def diff_encode(num_list: List[int]) -> List[int]:
    diff_list = []
    num_list = sorted(num_list)
    for i, num in enumerate(num_list):
        if len(diff_list) != 0:
            diff_list.append(num - num_list[i - 1])
        else:
            diff_list.append(num)

    return diff_list


def diff_decode(diff_list: List[int]) -> List[int]:
    num_list = []
    for diff in diff_list:
        if len(num_list) != 0:
            num_list.append(diff + num_list[-1])
        else:
            num_list.append(diff)

    return num_list


def _gamma_encode(number):  # запускает гамма кодирование
    binary = bin(number)[2:]  # Преобразование числа в двоичное представление
    unary_code = "0" * (len(binary) - 1)  # Унарный код
    return unary_code + binary


def gamma_encode_seq(num_list: List[int]) -> BitVector:
    encoded_list = []
    for num in num_list:
        encoded_list.append(_gamma_encode(num + 1))
    return BitVector(bitstring="".join(encoded_list))


def gamma_decode(encoded_seq: BitVector) -> List[int]:
    current_position = 0
    decoded_seq = []
    while current_position < len(encoded_seq):
        N = encoded_seq.next_set_bit(current_position)  # для '00001' это  4
        next_position = (
            2 * N + 1 - current_position
        )  # начало (индекс) следующего числа(закодированного)
        bin_decoded = encoded_seq[N : next_position]
        current_position = next_position
        decoded_seq.append(int(str(bin_decoded), base=2) - 1)
    return decoded_seq


def delta_encode(doc_ids):  # зависит от гамма кодирования
    delta_encoded = []
    for num in doc_ids:
        inc_num = num + 1
        bit_length = int(math.log2(inc_num)) + 1  # Длина числа в битах
        gamma_code = _gamma_encode(bit_length)  # Кодирование длины числа
        delta_encoded.append(
            gamma_code + bin(inc_num)[3:]
        )  # дельта кодирование первого числа и разностей м/д последующими
    return BitVector(bitstring="".join(delta_encoded))


def delta_decode(delta_encoded):
    doc_ids = []
    current_position = 0
    while current_position < len(delta_encoded):
        num_len_position = delta_encoded.next_set_bit(current_position)
        gamma_length = (
            num_len_position - current_position + 1) # сколько нужно цифр, что получить -> N->(сколько нужно последующих цифр, чтоб получить исходное число)
        decoded_num_position = num_len_position + gamma_length
        N_bin = delta_encoded[num_len_position : decoded_num_position]
        N = int(str(N_bin), base=2)  # переводим N в обычное (исходное) число
        next_position = decoded_num_position + (N - 1)
        decoded_num_bin = "1" + str(delta_encoded[decoded_num_position:next_position])
        decoded_num = int(decoded_num_bin, base=2) - 1  # исходное число
        current_position = next_position
        doc_ids.append(decoded_num)
    return doc_ids


def compress_index(path):  # запускает дельта кодирование
    index = get_inverted_index_json(path)
    words = list(index.keys())
    words_count = len(words)
    for idx in range(words_count):
        print(
            "Finished creating index {} % of all events".format(
                int(100 * (idx / words_count))
            )
        )
        doc_ids = index[words[idx]]
        doc_ids.sort()  # Сортировка идентификаторов документов
        delta_encoded = delta_encode(doc_ids)  # Применение дельта кодирования Элиаса
        index[words[idx]] = delta_encoded
    save_inverted_index_json(data=index, file_path=r"data\spbu_result_encoding1.json")


# compress_index(r"data\spbu_result_encoding1.json")
