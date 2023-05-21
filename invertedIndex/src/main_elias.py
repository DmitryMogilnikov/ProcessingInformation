import os
import sys

sys.path.append(os.getcwd())
from work_with_files import get_inverted_index_json, save_inverted_index_json
from elias_encoding import *
from pympler import asizeof 


def get_elias_encoding(
    input_file_name: str, output_gamma_encoded: str, output_delta_encoded: str
):
    index = get_inverted_index_json(input_file_name)
    dict_diff_encoded = {}
    dict_gamma_encoded = {}
    dict_delta_encoded = {}
    for key, list_value in index.items():
        diff_list = diff_encode(list_value)
        dict_diff_encoded[key] = diff_list
        dict_gamma_encoded[key] = gamma_encode_seq(diff_list)
        dict_delta_encoded[key] = delta_encode(diff_list)

    print(f"Размер словаря с разностями: {asizeof.asizeof(dict_diff_encoded)}.")
    print(f"Размер словаря с гамма-кодами: {asizeof.asizeof(dict_gamma_encoded)}.")
    print(f"Размер словаря с дельта-кодами: {asizeof.asizeof(dict_delta_encoded)}.")

    dict_gamma_encoded = {key: str(value) for key, value in dict_gamma_encoded.items()}
    dict_delta_encoded = {key: str(value) for key, value in dict_delta_encoded.items()}

    save_inverted_index_json(dict_gamma_encoded, file_path=output_gamma_encoded)
    save_inverted_index_json(dict_delta_encoded, file_path=output_delta_encoded)


if __name__ == "__main__":
    data_path = "data"

    get_elias_encoding(
        input_file_name=f"{data_path}/spbu_result.json",
        output_gamma_encoded=f"{data_path}/gamma_encoded_spbu.json",
        output_delta_encoded=f"{data_path}/delta_encoded_spbu.json",
    )
