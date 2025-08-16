import os
import requests
import threading
from queue import Queue
import time
import socket
import socks # Для работы с SOCKS-прокси
import argparse # Для аргументов командной строки

# --- Конфигурация по умолчанию (будет переопределена аргументами командной строки) ---
# Важно: используйте "сырые" строки (r'') для путей, чтобы избежать проблем с обратными слэшами
DEFAULT_PROXY_DIR = r'C:\Софт\ПРОКСИ ЧЕКЕР\НЕпроверянные'
DEFAULT_OUTPUT_DIR = r'C:\Софт\ПРОКСИ ЧЕКЕР\Рабочие прокси'
DEFAULT_HTTP_TIMEOUT = 15  # Таймаут в секундах для HTTP/S запросов
DEFAULT_SOCKS_TIMEOUT = 10 # Таймаут в секундах для SOCKS-соединений (часто нужен меньше)
DEFAULT_NUM_THREADS = 500 # Количество потоков для параллельной проверки
DEFAULT_MAX_PING_MS = 3000 # Максимально допустимый пинг в миллисекундах для "рабочих" прокси

# URL для проверки
TEST_URL_HTTP = 'http://www.google.com'
TEST_URL_HTTPS = 'https://www.google.com' # Используем HTTPS для проверки HTTPS-прокси
TEST_URL_SOCKS = 'http://www.google.com' # Для SOCKS можно использовать HTTP-сайт

# Глобальные переменные для путей выходных файлов (будут установлены в main)
OUTPUT_FILE_HTTP = ''
OUTPUT_FILE_SOCKS4 = ''
OUTPUT_FILE_SOCKS5 = ''

print("---")
print("Запуск программы проверки прокси...")
print("---")

def worker(proxy_queue, output_queue, max_ping_ms, http_timeout, socks_timeout):
    """
    Функция-рабочий для потоков. Берет прокси из очереди, проверяет его
    и отправляет результат в выходную очередь.
    """
    while True:
        proxy = proxy_queue.get()
        if proxy is None: # None - это сигнал для завершения работы потока
            break
        check_proxy_wrapper(proxy, output_queue, max_ping_ms, http_timeout, socks_timeout)
        proxy_queue.task_done() # Сообщаем очереди, что задача выполнена

def check_http_proxy(proxy, output_queue, max_ping_ms, timeout):
    """ Проверяет HTTP/S прокси. """
    start_time = time.time()
    response = None # Инициализируем response
    try:
        proxies = {
            'http': f'http://{proxy}',
            'https': f'http://{proxy}' # HTTP-прокси часто поддерживают и HTTPS-трафик через CONNECT
        }
        # Проверяем HTTP
        response = requests.get(TEST_URL_HTTP, proxies=proxies, timeout=timeout)
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)

        if response.status_code == 200:
            if ping_ms <= max_ping_ms:
                output_queue.put(('http', proxy, ping_ms))
                print(f"✅ HTTP/S работает: {proxy} (Пинг: {ping_ms} мс)")
                return True
            else:
                print(f"🟡 HTTP/S работает, но пинг слишком высокий ({ping_ms} мс > {max_ping_ms} мс): {proxy}")
                return False
        else:
            print(f"❌ HTTP/S не работает (Статус: {response.status_code}): {proxy}")
            return False
    except requests.exceptions.Timeout:
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)
        print(f"❌ HTTP/S не работает (Таймаут после {ping_ms} мс): {proxy}")
        return False
    except requests.exceptions.ConnectionError:
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)
        print(f"❌ HTTP/S не работает (Ошибка соединения после {ping_ms} мс): {proxy}")
        return False
    except requests.exceptions.RequestException as e:
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)
        print(f"❌ HTTP/S не работает (Неизвестная ошибка RequestException '{e}' после {ping_ms} мс): {proxy}")
        return False
    except Exception as e:
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)
        print(f"❌ HTTP/S не работает (Непредвиденная ошибка '{e}' после {ping_ms} мс): {proxy}")
        return False
    finally: # Добавлено закрытие ответа requests
        if response and hasattr(response, 'close'):
            response.close()


def check_socks_proxy(proxy_addr, socks_version, output_queue, max_ping_ms, socks_timeout, http_timeout):
    """ Проверяет SOCKS4 или SOCKS5 прокси. """
    host, port = proxy_addr.split(':')
    port = int(port)
    start_time = time.time()

    s = None # Инициализируем s как None
    session = None # Инициализируем session
    response = None # Инициализируем response
    try:
        s = socks.socksocket()
        s.set_proxy(socks_version, host, port)
        s.settimeout(socks_timeout) # Используем отдельный таймаут для SOCKS-соединения
        
        # requests.Session() будет использовать установленный socks-прокси
        session = requests.Session()
        session.proxies = {
            'http': f'{ "socks4" if socks_version == socks.SOCKS4 else "socks5" }://{proxy_addr}',
            'https': f'{ "socks4" if socks_version == socks.SOCKS4 else "socks5" }://{proxy_addr}'
        }
        
        # Используем http_timeout для HTTP-запроса через SOCKS
        response = session.get(TEST_URL_SOCKS, timeout=http_timeout) 
        
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)

        proxy_type_str = "socks4" if socks_version == socks.SOCKS4 else "socks5"

        if response.status_code == 200:
            if ping_ms <= max_ping_ms:
                output_queue.put((proxy_type_str, proxy_addr, ping_ms))
                print(f"✅ {proxy_type_str.upper()} работает: {proxy_addr} (Пинг: {ping_ms} мс)")
                return True
            else:
                print(f"🟡 {proxy_type_str.upper()} работает, но пинг слишком высокий ({ping_ms} мс > {max_ping_ms} мс): {proxy_addr}")
                return False
        else:
            print(f"❌ {proxy_type_str.upper()} не работает (Статус: {response.status_code}): {proxy_addr}")
            return False

    except (socks.ProxyConnectionError, socks.GeneralProxyError) as e:
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)
        proxy_type_str = "SOCKS4" if socks_version == socks.SOCKS4 else "SOCKS5"
        print(f"❌ {proxy_type_str} не работает (Ошибка прокси-соединения '{e}' после {ping_ms} мс): {proxy_addr}")
        return False
    except ConnectionRefusedError:
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)
        proxy_type_str = "SOCKS4" if socks_version == socks.SOCKS4 else "SOCKS5"
        print(f"❌ {proxy_type_str} не работает (Соединение отклонено после {ping_ms} мс): {proxy_addr}")
        return False
    except socket.timeout:
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)
        proxy_type_str = "SOCKS4" if socks_version == socks.SOCKS4 else "SOCKS5"
        print(f"❌ {proxy_type_str} не работает (Таймаут сокета после {ping_ms} мс): {proxy_addr}")
        return False
    except requests.exceptions.Timeout:
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)
        proxy_type_str = "SOCKS4" if socks_version == socks.SOCKS4 else "SOCKS5"
        print(f"❌ {proxy_type_str} не работает (Таймаут HTTP-запроса через прокси после {ping_ms} мс): {proxy_addr}")
        return False
    except requests.exceptions.ConnectionError:
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)
        proxy_type_str = "SOCKS4" if socks_version == socks.SOCKS4 else "SOCKS5"
        print(f"❌ {proxy_type_str} не работает (Ошибка соединения HTTP-запроса через прокси после {ping_ms} мс): {proxy_addr}")
        return False
    except requests.exceptions.RequestException as e:
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)
        proxy_type_str = "SOCKS4" if socks_version == socks.SOCKS4 else "SOCKS5"
        print(f"❌ {proxy_type_str} не работает (Неизвестная ошибка RequestException '{e}' после {ping_ms} мс): {proxy_addr}")
        return False
    except Exception as e:
        end_time = time.time()
        ping_ms = round((end_time - start_time) * 1000)
        proxy_type_str = "SOCKS4" if socks_version == socks.SOCKS4 else "SOCKS5"
        print(f"❌ {proxy_type_str} не работает (Непредвиденная ошибка '{e}' после {ping_ms} мс): {proxy_addr}")
        return False
    finally:
        if s and isinstance(s, socket.socket):
            s.close()
        if response and hasattr(response, 'close'): # Проверяем, что response определен и имеет метод close
            response.close()
        if session and isinstance(session, requests.Session): # Проверяем, что session определен
            session.close()

def check_proxy_wrapper(proxy, output_queue, max_ping_ms, http_timeout, socks_timeout):
    """
    Обертка для проверки прокси. Пытается определить тип прокси (SOCKS5, SOCKS4, HTTP/S)
    и затем проверить его.
    """
    # Сначала пробуем как SOCKS5
    if check_socks_proxy(proxy, socks.SOCKS5, output_queue, max_ping_ms, socks_timeout, http_timeout):
        return
    
    # Если не SOCKS5, пробуем как SOCKS4
    if check_socks_proxy(proxy, socks.SOCKS4, output_queue, max_ping_ms, socks_timeout, http_timeout):
        return

    # Если не SOCKS4, пробуем как HTTP/S прокси
    if check_http_proxy(proxy, output_queue, max_ping_ms, http_timeout):
        return

    # Если ни один тип не подошел
    print(f"🔴 Не удалось определить тип или прокси не работает/слишком высокий пинг: {proxy}")


def main():
    global OUTPUT_FILE_HTTP, OUTPUT_FILE_SOCKS4, OUTPUT_FILE_SOCKS5

    parser = argparse.ArgumentParser(description="Multi-threaded proxy checker.")
    parser.add_argument('--input-dir', type=str, default=DEFAULT_PROXY_DIR,
                        help=f'Directory containing .txt files with proxies (default: {DEFAULT_PROXY_DIR}).')
    parser.add_argument('--output-dir', type=str, default=DEFAULT_OUTPUT_DIR,
                        help=f'Directory to save working proxies (default: {DEFAULT_OUTPUT_DIR}).')
    parser.add_argument('--http-timeout', type=int, default=DEFAULT_HTTP_TIMEOUT,
                        help=f'Timeout in seconds for HTTP/S requests (default: {DEFAULT_HTTP_TIMEOUT}).')
    parser.add_argument('--socks-timeout', type=int, default=DEFAULT_SOCKS_TIMEOUT,
                        help=f'Timeout in seconds for SOCKS connections (default: {DEFAULT_SOCKS_TIMEOUT}).')
    parser.add_argument('--threads', type=int, default=DEFAULT_NUM_THREADS,
                        help=f'Number of threads for parallel checking (default: {DEFAULT_NUM_THREADS}).')
    parser.add_argument('--max-ping', type=int, default=DEFAULT_MAX_PING_MS,
                        help=f'Maximum allowed ping in milliseconds for a "working" proxy (default: {DEFAULT_MAX_PING_MS}).')
    
    args = parser.parse_args()

    # Присвоение значений из args
    proxy_dir = args.input_dir
    output_dir = args.output_dir
    http_timeout = args.http_timeout
    socks_timeout = args.socks_timeout
    num_threads = args.threads
    max_ping_ms = args.max_ping

    # Создание основной выходной папки
    os.makedirs(output_dir, exist_ok=True)

    # --- ИЗМЕНЕНИЕ: Определяем пути для сохранения рабочих прокси с подпапками ---
    HTTP_OUTPUT_SUBDIR = os.path.join(output_dir, 'http')
    SOCKS4_OUTPUT_SUBDIR = os.path.join(output_dir, 'socks4')
    SOCKS5_OUTPUT_SUBDIR = os.path.join(output_dir, 'socks5')

    os.makedirs(HTTP_OUTPUT_SUBDIR, exist_ok=True)
    os.makedirs(SOCKS4_OUTPUT_SUBDIR, exist_ok=True)
    os.makedirs(SOCKS5_OUTPUT_SUBDIR, exist_ok=True)

    OUTPUT_FILE_HTTP = os.path.join(HTTP_OUTPUT_SUBDIR, 'working_proxies.txt')
    OUTPUT_FILE_SOCKS4 = os.path.join(SOCKS4_OUTPUT_SUBDIR, 'working_proxies.txt')
    OUTPUT_FILE_SOCKS5 = os.path.join(SOCKS5_OUTPUT_SUBDIR, 'working_proxies.txt')

    print(f"Прокси-файлы будут проверяться из: {os.path.abspath(proxy_dir)}")
    print(f"Рабочие прокси будут сохраняться в: {os.path.abspath(output_dir)}")
    print(f"  HTTP: {os.path.abspath(HTTP_OUTPUT_SUBDIR)}")
    print(f"  SOCKS4: {os.path.abspath(SOCKS4_OUTPUT_SUBDIR)}")
    print(f"  SOCKS5: {os.path.abspath(SOCKS5_OUTPUT_SUBDIR)}")
    print(f"Используется {num_threads} потоков с таймаутом {http_timeout} секунд (HTTP/S) и {socks_timeout} секунд (SOCKS).")
    print(f"Максимально допустимый пинг для рабочих прокси: {max_ping_ms} мс.")
    print("---")
    
    # Проверка существования входной папки
    if not os.path.exists(proxy_dir):
        print(f"Ошибка: Каталог '{proxy_dir}' не найден.")
        print(f"Пожалуйста, создайте папку с именем '{os.path.basename(proxy_dir)}' и поместите в нее файлы .txt с прокси,")
        print(f"либо укажите правильный путь с помощью аргумента --input-dir.")
        return

    proxy_files = [f for f in os.listdir(proxy_dir) if f.endswith('.txt') and os.path.isfile(os.path.join(proxy_dir, f))]
    
    if not proxy_files:
        print(f"Файлы .txt с прокси не найдены в '{proxy_dir}'. Пожалуйста, поместите ваши списки прокси (например, proxies.txt) в этот каталог,")
        print(f"либо укажите правильный путь с помощью аргумента --input-dir.")
        return

    all_proxies = set()
    print("\nНачинаем загрузку прокси из найденных файлов:")
    for filename in proxy_files:
        filepath = os.path.join(proxy_dir, filename)
        print(f"  Загрузка из: {filepath}")
        try:
            with open(filepath, 'r') as f:
                for line in f:
                    proxy = line.strip()
                    if proxy:
                        all_proxies.add(proxy)
        except Exception as e:
            print(f"  Ошибка чтения файла {filepath}: {e}")

    print(f"Всего загружено {len(all_proxies)} уникальных прокси.")

    if not all_proxies:
        print("Прокси не найдены в предоставленных файлах. Выход.")
        return

    print("\nНачинаем проверку прокси. Пожалуйста, подождите...")

    proxy_queue = Queue()
    output_queue = Queue() # Теперь будет хранить (тип, прокси, пинг)

    # Заполняем очередь прокси
    for proxy in all_proxies:
        proxy_queue.put(proxy)

    threads = []
    for _ in range(num_threads):
        t = threading.Thread(target=worker, args=(proxy_queue, output_queue, max_ping_ms, http_timeout, socks_timeout))
        t.start()
        threads.append(t)

    # Ждем завершения обработки всех прокси
    proxy_queue.join()
    print("\nВсе прокси обработаны. Ожидание завершения потоков...")

    # Отправляем сигнал завершения в каждый поток
    for _ in range(num_threads):
        proxy_queue.put(None) 
    for t in threads:
        t.join() # Ждем завершения каждого потока
    print("Все потоки завершены. Начинаем сохранение результатов.")

    # --- Сохраняем рабочие прокси по типам ---
    working_proxies_by_type = {
        'http': [],
        'socks4': [],
        'socks5': []
    }

    # Считываем уже существующие рабочие прокси, чтобы не перезаписать их
    def load_existing_proxies(filepath):
        existing_proxies = set()
        if os.path.exists(filepath):
            try:
                with open(filepath, 'r') as f:
                    for line in f:
                        proxy = line.strip()
                        if proxy:
                            existing_proxies.add(proxy)
            except Exception as e:
                print(f"Ошибка при загрузке существующих прокси из {filepath}: {e}")
        return existing_proxies

    existing_http = load_existing_proxies(OUTPUT_FILE_HTTP)
    existing_socks4 = load_existing_proxies(OUTPUT_FILE_SOCKS4)
    existing_socks5 = load_existing_proxies(OUTPUT_FILE_SOCKS5)

    # Добавляем новые рабочие прокси, избегая дубликатов с уже существующими
    while not output_queue.empty():
        proxy_type, proxy_addr, ping = output_queue.get()
        # Проверяем, существует ли прокси уже в соответствующих списках
        if proxy_type == 'http' and proxy_addr not in existing_http:
            working_proxies_by_type['http'].append(proxy_addr)
            existing_http.add(proxy_addr) # Добавляем в сет, чтобы избежать дубликатов из текущей сессии
        elif proxy_type == 'socks4' and proxy_addr not in existing_socks4:
            working_proxies_by_type['socks4'].append(proxy_addr)
            existing_socks4.add(proxy_addr)
        elif proxy_type == 'socks5' and proxy_addr not in existing_socks5:
            working_proxies_by_type['socks5'].append(proxy_addr)
            existing_socks5.add(proxy_addr)

    total_working_proxies = 0

    # Теперь записываем все прокси (старые + новые уникальные) обратно в файлы
    for proxy_type, current_session_new_proxies in working_proxies_by_type.items(): # current_session_new_proxies не используется напрямую
        output_filepath = ''
        all_proxies_for_type = set() # Собираем все уникальные прокси для текущего типа

        if proxy_type == 'http':
            output_filepath = OUTPUT_FILE_HTTP
            all_proxies_for_type.update(existing_http)
        elif proxy_type == 'socks4':
            output_filepath = OUTPUT_FILE_SOCKS4
            all_proxies_for_type.update(existing_socks4)
        elif proxy_type == 'socks5':
            output_filepath = OUTPUT_FILE_SOCKS5
            all_proxies_for_type.update(existing_socks5)
        
        # Если есть прокси для записи, то записываем
        if all_proxies_for_type:
            with open(output_filepath, 'w') as f: # Используем 'w' для перезаписи, чтобы убрать возможные дубликаты
                for proxy in sorted(list(all_proxies_for_type)): # Сортируем для порядка
                    f.write(proxy + '\n')
            
            print(f"\n---")
            print(f"Всего {len(all_proxies_for_type)} уникальных рабочих {proxy_type.upper()} прокси (включая старые и новые).")
            print(f"Рабочие {proxy_type.upper()} прокси сохранены в: {output_filepath}")
            total_working_proxies += len(all_proxies_for_type)

    if total_working_proxies == 0:
        print(f"\n---")
        print(f"Рабочие прокси не найдены ни одного типа (с пингом <= {max_ping_ms} мс).")
    else:
        print(f"\n---")
        print(f"Всего найдено {total_working_proxies} уникальных рабочих прокси различных типов.")
    
    print("---")
    print("Программа завершила работу.")
    print("---")

if __name__ == '__main__':
    main()