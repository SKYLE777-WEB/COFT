import re
import os
import math
import json
import logging # Импортируем модуль логирования

# Конфигурация логирования
# Уровень логирования: INFO - для общей информации, WARNING - для предупреждений, ERROR - для ошибок.
# Формат сообщения: время, уровень, сообщение.
# filename: куда писать логи.
logging.basicConfig(level=logging.INFO,
                    format='%(asctime)s - %(levelname)s - %(message)s',
                    filename='script_log.log', # Имя файла для логов
                    filemode='a') # 'a' для добавления к существующему файлу, 'w' для перезаписи

# Также можно добавить обработчик для вывода логов в консоль
console_handler = logging.StreamHandler()
console_handler.setLevel(logging.INFO) # Уровень для консоли
console_handler.setFormatter(logging.Formatter('%(levelname)s - %(message)s'))
logging.getLogger().addHandler(console_handler)


def collect_links_from_files(folder: str) -> list[str]:
    txt_files = [f for f in os.listdir(folder) if f.lower().endswith(".txt")]
    all_links: list[str] = []

    logging.info(f"Начинается сбор ссылок из папки: {folder}")

    if not txt_files:
        logging.warning(f"В папке '{folder}' не найдено текстовых файлов (.txt).")
        return []

    for filename in txt_files:
        full_path = os.path.join(folder, filename)
        try:
            with open(full_path, "r", encoding="utf-8") as fp:
                data = fp.read()

            links = re.findall(r"https?://t\.me/[^\s]+", data)
            all_links.extend(links)

            logging.info(f"Файл «{filename}»: найдено {len(links)} ссылок")
        except FileNotFoundError:
            logging.error(f"Ошибка: Файл не найден по пути {full_path}. Пропускаем.")
        except Exception as e:
            logging.error(f"Ошибка при обработке файла «{filename}»: {e}")

    logging.info(f"Сбор ссылок завершен. Всего найдено: {len(all_links)}")
    return all_links


def write_chunk_file(links: list[str], index: int, folder: str, is_incomplete: bool) -> str:
    os.makedirs(folder, exist_ok=True)
    base_name = f"links_{index}"
    if is_incomplete:
        base_name += " НЕ ПОЛНЫЙ"
    filename = f"{base_name}.txt"
    filepath = os.path.join(folder, filename)
    try:
        with open(filepath, "w", encoding="utf-8") as fp:
            fp.write("\n".join(links))
        logging.info(f"Создан файл пачки: '{filename}' в папке '{folder}'. Содержит {len(links)} ссылок.")
    except Exception as e:
        logging.error(f"Ошибка при записи файла пачки '{filename}': {e}")
    return filename

def find_and_read_incomplete_file(folder: str) -> tuple[str | None, list[str]]:
    """Находит файл 'сбор*.txt', читает его и возвращает путь и содержимое."""
    try:
        for f_name in os.listdir(folder):
            if f_name.lower().startswith("сбор") and f_name.lower().endswith(".txt"):
                file_path = os.path.join(folder, f_name)
                with open(file_path, "r", encoding="utf-8") as f:
                    lines = f.read().splitlines()
                # Удаляем пустые строки на всякий случай
                lines = [line for line in lines if line.strip()]
                logging.info(f"Найден существующий файл неполного сбора: '{file_path}' с {len(lines)} ссылками.")
                return file_path, lines
    except FileNotFoundError:
        logging.info("Папка для неполных сборов не найдена, будет создана.")
    except Exception as e:
        logging.error(f"Ошибка при поиске файла неполного сбора: {e}")

    logging.info("Файл неполного сбора не найден.")
    return None, []


def append_to_incomplete(links: list[str], folder: str):
    """Добавляет ссылки в файл неполного сбора, обновляя его имя с количеством."""
    os.makedirs(folder, exist_ok=True)
    
    old_file_path, existing_links = find_and_read_incomplete_file(folder)

    # Объединяем старые и новые ссылки
    all_links = existing_links + links
    new_count = len(all_links)

    # Удаляем старый файл, если он был
    if old_file_path and os.path.exists(old_file_path):
        try:
            os.remove(old_file_path)
            logging.info(f"Старый файл неполного сбора '{old_file_path}' удален.")
        except Exception as e:
            logging.error(f"Не удалось удалить старый файл неполного сбора '{old_file_path}': {e}")

    # Создаем новый файл с актуальным количеством в названии
    new_filename = f"сбор{new_count}.txt"
    new_filepath = os.path.join(folder, new_filename)
    
    try:
        with open(new_filepath, "w", encoding="utf-8") as f:
            f.write("\n".join(all_links))
        logging.info(f"Обновлен файл неполного сбора: '{new_filepath}'. Текущее количество: {new_count}.")
    except Exception as e:
        logging.error(f"Ошибка при записи нового файла неполного сбора '{new_filepath}': {e}")


def process_incomplete_if_ready(chunk_size: int, incomplete_folder: str, output_folder: str, file_index_start: int) -> int:
    """Обрабатывает файл неполного сбора, если ссылок достаточно для полной пачки."""
    old_file_path, lines = find_and_read_incomplete_file(incomplete_folder)

    if not old_file_path:
        logging.info("Файл неполного сбора не существует. Нет данных для обработки.")
        return 0

    current_incomplete_count = len(lines)
    logging.info(f"В файле неполного сбора '{old_file_path}' найдено {current_incomplete_count} ссылок.")

    if current_incomplete_count >= chunk_size:
        full_chunk = lines[:chunk_size]
        remaining = lines[chunk_size:]

        # Записываем полную пачку в папку результатов
        write_chunk_file(full_chunk, file_index_start, output_folder, is_incomplete=False)
        logging.info(f"Из неполных ссылок сформирована полная пачка ({chunk_size} ссылок) и сохранена как файл №{file_index_start}.")

        # Удаляем старый файл неполного сбора
        try:
            os.remove(old_file_path)
            logging.info(f"Обработанный файл неполного сбора '{old_file_path}' удален.")
        except Exception as e:
            logging.error(f"Не удалось удалить обработанный файл неполного сбора '{old_file_path}': {e}")
        
        # Создаем новый файл для остатка, или пустой файл 'сбор0.txt', если остатка нет
        new_count = len(remaining)
        new_filename = f"сбор{new_count}.txt"
        new_filepath = os.path.join(incomplete_folder, new_filename)
        try:
            with open(new_filepath, "w", encoding="utf-8") as f:
                if remaining:
                    f.write("\n".join(remaining))
            logging.info(f"Остаток ({new_count} ссылок) записан в новый файл неполного сбора: {new_filepath}.")
        except Exception as e:
            logging.error(f"Ошибка при записи остатка в новый файл неполного сбора '{new_filepath}': {e}")
            
        return 1
    else:
        logging.info(f"Недостаточно ссылок ({current_incomplete_count}) в файле неполного сбора для формирования полной пачки (нужно {chunk_size}).")
        return 0

def load_config(config_path: str) -> int:
    logging.info(f"Загрузка конфигурации из файла: {config_path}")
    if not os.path.exists(config_path):
        logging.critical(f"Файл конфигурации не найден: {config_path}")
        raise FileNotFoundError(f"Файл конфигурации не найден: {config_path}")
    try:
        with open(config_path, "r", encoding="utf-8") as f:
            config = json.load(f)

        chunk_size = config.get("chunk_size")
        if not isinstance(chunk_size, int) or chunk_size <= 0:
            logging.critical("В Колич.чатов.json должен быть положительный 'chunk_size'")
            raise ValueError("В Количество чатов в одном файле.json должен быть положительный 'chunk_size'")
        logging.info(f"Размер пачки (chunk_size) установлен: {chunk_size}")
        return chunk_size
    except json.JSONDecodeError:
        logging.critical(f"Ошибка парсинга JSON в файле конфигурации: {config_path}. Проверьте формат файла.")
        raise ValueError(f"Неверный формат JSON в файле: {config_path}")
    except Exception as e:
        logging.critical(f"Непредвиденная ошибка при загрузке конфигурации: {e}")
        raise


def main() -> None:
    logging.info("--- Запуск скрипта обработки чатов ---")
    base = os.path.dirname(os.path.abspath(__file__))

    config_path = os.path.join(base, "Количество чатов в одном файле.json")
    source_folder = os.path.join(base, "НЕ отработанные")
    output_folder = os.path.join(base, "Чаты по пачкам")
    incomplete_folder = os.path.join(base, "НЕ ПОЛНЫЕ СОБИРАЮТСЯ")

    os.makedirs(incomplete_folder, exist_ok=True)
    
    try:
        chunk_size = load_config(config_path)
    except Exception as e:
        logging.critical(f"Скрипт не может продолжить из-за ошибки конфигурации: {e}")
        return

    next_file_index = 1
    try:
        os.makedirs(output_folder, exist_ok=True)
        existing_files = [f for f in os.listdir(output_folder) if f.startswith("links_") and f.endswith(".txt")]
        if existing_files:
            last_num = max([int(re.search(r'links_(\d+)', f).group(1)) for f in existing_files])
            next_file_index = last_num + 1
    except (ValueError, AttributeError):
        logging.warning("Не удалось определить номера существующих файлов. Нумерация начнется с 1.")
        next_file_index = 1

    logging.info(f"Следующий доступный индекс для файла пачки: {next_file_index}")

    all_links = collect_links_from_files(source_folder)
    total_links = len(all_links)

    if total_links > 0:
        logging.info(f"Всего ссылок найдено во всех исходных файлах: {total_links}")
        total_chunks_expected = math.ceil(total_links / chunk_size)
        
        for i in range(total_chunks_expected):
            start_index = i * chunk_size
            end_index = (i + 1) * chunk_size
            chunk = all_links[start_index:end_index]

            if len(chunk) == chunk_size:
                write_chunk_file(chunk, next_file_index, output_folder, is_incomplete=False)
                logging.info(f"Новая полная пачка №{next_file_index} сохранена.")
                next_file_index += 1
            else:
                append_to_incomplete(chunk, incomplete_folder)
                logging.info(f"Остаток из {len(chunk)} ссылок добавлен в файл неполного сбора.")
    else:
        logging.info("Новые ссылки в исходной папке не найдены.")

    logging.info("Попытка обработки накопленных неполных ссылок...")
    created_from_incomplete = process_incomplete_if_ready(chunk_size, incomplete_folder, output_folder, next_file_index)
    
    if created_from_incomplete:
        logging.info("Из неполных собран полноценный файл и перенесён в 'Чаты по пачкам'.")
    else:
        logging.info("Недостаточно неполных ссылок для формирования полноценной пачки на текущий момент.")

    # Финальная проверка: создаем 'сбор0.txt', если папка неполных сборов пуста
    existing_incomplete_path, _ = find_and_read_incomplete_file(incomplete_folder)
    if not existing_incomplete_path:
        try:
            with open(os.path.join(incomplete_folder, "сбор0.txt"), "w", encoding="utf-8") as f:
                pass # Создаем пустой файл
            logging.info("Папка неполных сборов была пуста. Создан файл 'сбор0.txt'.")
        except Exception as e:
            logging.error(f"Не удалось создать 'сбор0.txt': {e}")

    logging.info("--- Обработка завершена --- ✅")


if __name__ == "__main__":
    main()