import os
from typing import List, Tuple

BASE_DIR = os.path.dirname(__file__)
INPUT_FOLDER = os.path.join(BASE_DIR, "НЕ отработанные")
OUTPUT_FOLDER = os.path.join(BASE_DIR, "Результаты")
DUPLICATES_FILENAME = "удалённые_ссылки.txt"
CLEANED_FILENAME = "без_дубликатов.txt"

def normalize(url: str) -> str:
    url = url.strip().lower()
    return url[:-1] if url.endswith("/") else url

def deduplicate_links(lines: List[str]) -> Tuple[List[str], List[str]]:
    seen = set()
    unique = []
    dups = []
    for raw in lines:
        link = raw.strip()
        if not link:
            continue
        key = normalize(link)
        if key in seen:
            dups.append(link)
        else:
            seen.add(key)
            unique.append(link)
    return unique, dups

def process_file(file_path: str, output_folder: str):
    with open(file_path, "r", encoding="utf-8", errors="ignore") as f:
        lines = f.readlines()

    unique, dups = deduplicate_links(lines)

    base_name = os.path.splitext(os.path.basename(file_path))[0]

    cleaned_path = os.path.join(output_folder, f"{base_name}_{CLEANED_FILENAME}")
    dups_path = os.path.join(output_folder, f"{base_name}_{DUPLICATES_FILENAME}")

    with open(cleaned_path, "w", encoding="utf-8") as f:
        f.write("\n".join(unique))

    with open(dups_path, "w", encoding="utf-8") as f:
        f.write("\n".join(dups))

    print(f"✔ {file_path}: удалено {len(dups)} дубликатов, осталось {len(unique)} ссылок")

def main():
    os.makedirs(OUTPUT_FOLDER, exist_ok=True)

    txt_files = [
        f for f in os.listdir(INPUT_FOLDER)
        if f.lower().endswith(".txt")
    ]

    if not txt_files:
        print("Нет .txt файлов в папке:", INPUT_FOLDER)
        return

    for filename in txt_files:
        full_path = os.path.join(INPUT_FOLDER, filename)
        process_file(full_path, OUTPUT_FOLDER)

if __name__ == "__main__":
    main()
