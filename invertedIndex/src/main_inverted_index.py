import os
import sys
sys.path.append(os.getcwd())

from work_with_files import get_data, save_inverted_index_json, save_to_csv
from inverted_index_functions import lemmatization, create_inverted_index_dict
from src.utils import drop_errors


def get_inverted_index(
    input_file_name: str,
    output_file_name_csv: str,
    index_file_name: str
) -> None:
    df = get_data(input_file_name)
    # переменные из ретерна, которые возвращает функция
    df, list_errors = lemmatization(df)
    df = drop_errors(df=df, list_errors=list_errors)
    save_to_csv(df, file_path=output_file_name_csv)
    inverted_index_dict = create_inverted_index_dict(df)
    save_inverted_index_json(
        data=inverted_index_dict,
        file_path=index_file_name,
    )


if __name__ == "__main__":
    data_path = "data"

    get_inverted_index(
        input_file_name=f"{data_path}/spbu_content.csv",
        output_file_name_csv=f"{data_path}/df_spbu.csv",
        index_file_name=f"{data_path}/spbu_result.json",
    )
