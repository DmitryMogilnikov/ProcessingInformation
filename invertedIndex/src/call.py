import json
import pandas as pd
from functions import lemmatization, drop_errors, create_dict


data_path = r"C:\Users\User\Desktop\Study\2_sem\blek\data"


data_spbu = pd.read_csv(f"{data_path}\spbu_content.csv", header = None)
data_spbu.head(5)
print(1)
data_msu = pd.read_csv(f"{data_path}\msu_content.csv", header = None)
data_msu.head(5)



data_spbu = data_spbu[~data_spbu[0].str.endswith('.pdf/')] # удаляем строки из датафрейма, которые пдфки
data_spbu = data_spbu.reset_index(drop=True) # сбрасываем индексы чтобы не возникало ошибок
print(2)


print(3)
df, list_errors = lemmatization(data_spbu) # переменные из ретерна, которые возвращает функция 
print(4)




df = drop_errors(df=df, list_errors=list_errors)

df = df.reset_index(drop=True) # сбрасываем индексы чтобы не возникало ошибок


df.to_csv(f"{data_path}\df_spbu.csv") # сохраняем обработанный датафрейм в новый csv для последующего использования (токенизированный и почищенный)

df = pd.read_csv(f'{data_path}/df_spbu.csv', index_col=0,)
df.reset_index(inplace= True) #добавляем столбик индексов --> [0]




my_dict = create_dict(df)


with open(f'{data_path}/result.json', 'w') as fp: # сохраняем словарь в json
    json.dump(my_dict, fp)

with open(f'{data_path}/result.json', 'r') as fp: #считываем словарь из json
    data = json.loads(fp.read())


