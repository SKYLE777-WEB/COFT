import sys
import threading
import json
import os
import winsound
import subprocess
import queue
import datetime
import winreg # Для чтения темы Windows (хотя PyQt обычно делает это сам)
import psutil

# Импорты для PyQt6
from PyQt6.QtWidgets import (
    QApplication, QMainWindow, QWidget, QVBoxLayout, QHBoxLayout, QPushButton, QLabel, QProgressBar,
    QTextEdit, QTabWidget, QGroupBox, QCheckBox, QSizePolicy, QLineEdit, QScrollArea, QFileDialog, QMessageBox
)
from PyQt6.QtGui import QFont, QIcon, QPixmap
from PyQt6.QtCore import Qt, QThread, pyqtSignal, QTimer

from PIL import Image

# Настройка логгера
import logging
logger = logging.getLogger()
logger.setLevel(logging.INFO)
if logger.hasHandlers():
    logger.handlers.clear()
stdout_handler = logging.StreamHandler(sys.stdout)
stdout_handler.setLevel(logging.INFO)
formatter = logging.Formatter('[%(asctime)s] [%(levelname)s] %(message)s', "%H:%M:%S")
stdout_handler.setFormatter(formatter)
logger.addHandler(stdout_handler)


# --- Современная Корпоративная Цветовая Палитра ---
COLOR_PRIMARY_BG = "#0a1929" # Глубокий темно-синий
COLOR_SECONDARY_BG = "#1e293b" # Сланцево-синий для карточек
COLOR_TERTIARY_BG = "#334155" # Светлее для активных элементов
COLOR_ACCENT_GOLD = "#ffd700" # Классический золотой
COLOR_ACCENT_AMBER = "#f59e0b" # Янтарный акцент
COLOR_ACCENT_WARM = "#fb923c" # Теплый оранжевый
COLOR_TEXT_PRIMARY = "#f8fafc" # Почти белый
COLOR_TEXT_SECONDARY = "#cbd5e1" # Светло-серый
COLOR_TEXT_MUTED = "#94a3b8" # Приглушенный серый
COLOR_INPUT_BG = "#0f172a" # Очень темный для полей ввода
COLOR_INPUT_BORDER = "#475569" # Граница полей
COLOR_SUCCESS = "#10b981" # Корпоративный зеленый
COLOR_ERROR = "#ef4444" # Корпоративный красный
COLOR_BORDER_ACTIVE = "#3b82f6" # Активный синий

# --- Корпоративные Шрифты ---
FONT_TITLE = QFont("Segoe UI", 26, QFont.Weight.Bold)
FONT_HEADER = QFont("Segoe UI", 22, QFont.Weight.Bold)
FONT_LABEL = QFont("Segoe UI", 16, QFont.Weight.Normal)
FONT_ENTRY = QFont("Segoe UI", 18, QFont.Weight.Normal)
FONT_BUTTON = QFont("Segoe UI", 16, QFont.Weight.Bold)
FONT_INFO = QFont("Segoe UI", 11, QFont.Weight.Light, italic=True)
FONT_CUSTOM_CLOSE = QFont("Segoe UI", 14, QFont.Weight.Bold)

# Конфигурация файлов и их параметров
FILES_CONFIG = {
    r"C:\Софт\1TGlinkV1.0\МИН_УЧАСТНИКОВ.json": {
        "name": "Минимальное количество участников для чатов",
        "key_map": {
            "Минимум участников (публичные чаты)": "public_min_members",
            "Минимум участников (приватные чаты)": "private_min_members"
        }
    },
    r"C:\Софт\3FiltrTGV1.0\ПРОЦЕНТ.json": {
        "name": "Настройки процентов",
        "key_map": {
            "Основной процент онлайна": "percent",
            "Вторичный процент": "large_chat_percent",
            "Минимум участников для вторичного процента": "large_chat_members_threshold"
        }
    },
    r"C:\Софт\5ChekLinksHUM\Количество чатов в одном файле.json": {"name": "Количество чатов на аккаунт", "key": "chunk_size"}
}

def play_success_sound():
    """Воспроизводит звук успешного сохранения."""
    winsound.MessageBeep(winsound.MB_OK)

# --- Вспомогательный класс для запуска скрипта в отдельном потоке ---
class ScriptRunner(QThread):
    log_signal = pyqtSignal(str)
    # Можно добавить другие сигналы для обновления прогресса, статуса и т.д.

    def __init__(self, script_path):
        super().__init__()
        self.script_path = script_path
        self.process = None
        self.running = False

    def run(self):
        self.running = True
        self.log_signal.emit(f"Попытка запустить скрипт: {self.script_path}\n")

        try:
            # Флаг DETACHED_PROCESS (только для Windows) запускает дочерний процесс
            # в новой сессии, полностью отделяя его от родительского (особенно от отладчика IDE).
            # Это предотвращает "зависание" дочернего процесса, когда родительский завершается.
            creation_flags = subprocess.DETACHED_PROCESS

            self.process = subprocess.Popen(
                [sys.executable, "-u", self.script_path],
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE,
                text=True,
                bufsize=1,
                universal_newlines=True,
                creationflags=creation_flags
            )

            # Чтение вывода в отдельных потоках
            stdout_thread = threading.Thread(target=self._read_stream, args=(self.process.stdout, "STDOUT"))
            stderr_thread = threading.Thread(target=self._read_stream, args=(self.process.stderr, "STDERR"))
            stdout_thread.daemon = True
            stderr_thread.daemon = True
            stdout_thread.start()
            stderr_thread.start()

            self.process.wait() # Ожидаем завершения процесса
            if self.process.returncode == 0:
                self.log_signal.emit(f"\nСкрипт завершил работу успешно.\n")
            else:
                self.log_signal.emit(f"\nСкрипт завершен (код выхода: {self.process.returncode}). Возможно, он был прерван вручную.\n")

        except FileNotFoundError:
            self.log_signal.emit(f"Ошибка: Скрипт '{self.script_path}' не найден. Проверьте путь.\n")
        except Exception as e:
            self.log_signal.emit(f"Произошла непредвиденная ошибка при запуске скрипта: {e}\n")
        finally:
            self.running = False
            if self.process and self.process.poll() is None:
                self.process.terminate()
                self.process.wait()

    def _read_stream(self, stream, name):
        for line in iter(stream.readline, ''):
            self.log_signal.emit(f"[{name}] {line}")
        stream.close()

    def stop_script(self):
        self.running = False
        if not self.process or self.process.poll() is not None:
            self.log_signal.emit("Скрипт не запущен или уже был завершен.\n")
            return

        self.log_signal.emit(f"Завершение процесса (PID: {self.process.pid}) и всех его дочерних процессов...\n")
        try:
            # Находим родительский процесс, который мы запустили
            parent = psutil.Process(self.process.pid)
            
            # Сначала рекурсивно находим и завершаем всех потомков
            children = parent.children(recursive=True)
            if children:
                self.log_signal.emit(f"Найдены дочерние процессы: {[child.pid for child in children]}. Завершение...")
                for child in children:
                    try:
                        child.terminate() # Отправляем команду на завершение
                    except psutil.NoSuchProcess:
                        continue # Процесс уже мог завершиться сам

            # Даем потомкам время на штатное завершение
            gone, alive = psutil.wait_procs(children, timeout=3)

            # Тех, кто не завершился, убиваем принудительно
            for p in alive:
                self.log_signal.emit(f"Процесс {p.pid} не ответил, принудительное завершение (kill)...")
                p.kill()

            # Теперь завершаем сам родительский процесс
            self.log_signal.emit(f"Завершение основного процесса {parent.pid}...")
            parent.terminate()
            parent.wait(timeout=3)
            if parent.is_running():
                self.log_signal.emit(f"Основной процесс {parent.pid} не ответил, принудительное завершение (kill)...")
                parent.kill()
            
            self.log_signal.emit("Скрипт и все дочерние процессы успешно завершены.\n")

        except psutil.NoSuchProcess:
            self.log_signal.emit("Процесс уже был завершен к моменту остановки.\n")
        except Exception as e:
            self.log_signal.emit(f"Произошла ошибка при завершении дерева процессов: {e}\n")


class MainWindow(QMainWindow):
    def __init__(self):
        super().__init__()
        self.setWindowTitle("Система Управления Параметрами и Скриптами")
        self.set_stylesheet()
        self.init_ui()
        self.script_runner = None
        self.is_paused = False # Пока не используется для внешних скриптов

        self.setup_icon()

    def set_stylesheet(self):
        """Устанавливает глобальный стиль для приложения."""
        self.setStyleSheet(f"""
            QMainWindow {{ background: {COLOR_PRIMARY_BG}; }}
            QLabel, QCheckBox, QPushButton, QGroupBox, QTextEdit, QLineEdit {{ color: {COLOR_TEXT_PRIMARY}; }}
            
            /* Кнопки управления */
            QPushButton {{
                font-weight: bold;
                background: {COLOR_SECONDARY_BG};
                color: {COLOR_ACCENT_GOLD};
                border-radius: 12px;
                padding: 14px 6px;
                font-size: 16px;
                border: 1px solid {COLOR_INPUT_BORDER};
            }}
            QPushButton:hover {{
                background: {COLOR_ACCENT_GOLD};
                color: {COLOR_PRIMARY_BG};
                border: 1px solid {COLOR_ACCENT_GOLD};
            }}
            QPushButton:pressed {{
                background: {COLOR_ACCENT_AMBER};
                color: {COLOR_PRIMARY_BG};
            }}
            QPushButton:disabled {{
                background: {COLOR_INPUT_BORDER};
                color: {COLOR_TEXT_MUTED};
                border: 1px solid {COLOR_INPUT_BORDER};
            }}

            /* Кнопки старт/стоп/пауза */
            QPushButton#startButton {{ background: {COLOR_SUCCESS}; }}
            QPushButton#startButton:hover {{ background: #0c8c62; }}
            QPushButton#startButton:pressed {{ background: #086b4a; }}
            QPushButton#startButton:disabled {{ background: {COLOR_INPUT_BORDER}; }}

            QPushButton#stopButton {{ background: {COLOR_ERROR}; }}
            QPushButton#stopButton:hover {{ background: #b22222; }}
            QPushButton#stopButton:pressed {{ background: #8c1a1a; }}
            QPushButton#stopButton:disabled {{ background: {COLOR_INPUT_BORDER}; }}

            QPushButton#pauseButton {{ background: {COLOR_ACCENT_AMBER}; }}
            QPushButton#pauseButton:hover {{ background: #c27e08; }}
            QPushButton#pauseButton:pressed {{ background: #9c6406; }}
            QPushButton#pauseButton:disabled {{ background: {COLOR_INPUT_BORDER}; }}

            /* Прогресс-бар */
            QProgressBar {{
                background: {COLOR_INPUT_BG};
                color: {COLOR_TEXT_PRIMARY};
                border-radius: 8px;
                text-align: center;
                border: 1px solid {COLOR_INPUT_BORDER};
            }}
            QProgressBar::chunk {{ background: {COLOR_ACCENT_GOLD}; border-radius: 8px; }}

            /* Вкладки */
            QTabWidget::pane {{ border: 2px solid {COLOR_ACCENT_GOLD}; border-radius: 8px; }}
            QTabBar::tab {{
                background: {COLOR_SECONDARY_BG};
                color: {COLOR_TEXT_SECONDARY};
                border-top-left-radius: 8px;
                border-top-right-radius: 8px;
                padding: 8px 15px;
                margin-right: 2px;
                border: 1px solid {COLOR_INPUT_BORDER};
                border-bottom: none;
            }}
            QTabBar::tab:selected {{
                background: {COLOR_PRIMARY_BG};
                color: {COLOR_ACCENT_GOLD};
                border: 1px solid {COLOR_ACCENT_GOLD};
                border-bottom: none;
            }}
            QTabBar::tab:hover {{
                background: {COLOR_TERTIARY_BG};
            }}

            /* Групповые рамки */
            QGroupBox {{
                border: 2px solid {COLOR_ACCENT_GOLD};
                border-radius: 8px;
                margin-top: 10px;
                padding-top: 1px;
                background: {COLOR_SECONDARY_BG};
            }}
            QGroupBox::title {{
                subcontrol-origin: margin;
                subcontrol-position: top left;
                padding: 0 5px;
                background: {COLOR_SECONDARY_BG};
                color: {COLOR_ACCENT_GOLD};
                font-weight: bold;
                font-size: 14px;
                left: 10px;
            }}

            /* Текстовые поля (логи) */
            QTextEdit {{
                background: {COLOR_INPUT_BG};
                color: {COLOR_TEXT_SECONDARY};
                border-radius: 8px;
                font-size: 14px;
                padding: 10px;
                border: 1px solid {COLOR_INPUT_BORDER};
            }}
            
            /* Поля ввода (параметры) */
            QLineEdit {{
                background: {COLOR_INPUT_BG};
                color: {COLOR_TEXT_PRIMARY};
                border-radius: 8px;
                padding: 10px;
                font-size: 16px;
                border: 2px solid {COLOR_INPUT_BORDER};
                selection-background-color: {COLOR_ACCENT_AMBER};
                selection-color: {COLOR_PRIMARY_BG};
            }}
            QLineEdit:focus {{
                border: 2px solid {COLOR_ACCENT_GOLD};
                background: {COLOR_TERTIARY_BG};
            }}

            /* Чекбоксы (кнопки с переключением) */
            QCheckBox {{
                color: {COLOR_TEXT_PRIMARY};
                spacing: 5px;
            }}
            QCheckBox::indicator {{
                width: 18px;
                height: 18px;
                border: 1px solid {COLOR_INPUT_BORDER};
                border-radius: 4px;
                background-color: {COLOR_INPUT_BG};
            }}
            QCheckBox::indicator:checked {{
                background-color: {COLOR_ACCENT_GOLD};
                border: 1px solid {COLOR_ACCENT_GOLD};
            }}
            QCheckBox::indicator:hover {{
                border: 1px solid {COLOR_ACCENT_AMBER};
            }}
        """)

    def setup_icon(self):
        """Пытается установить иконку приложения."""
        try:
            script_dir = getattr(sys, '_MEIPASS', os.path.abspath(os.path.dirname(__file__)))
            icon_path_ico = os.path.join(script_dir, 'app_icon.ico')
            icon_path_png = os.path.join(script_dir, 'app_icon.png')

            if os.path.exists(icon_path_ico):
                self.setWindowIcon(QIcon(icon_path_ico))
            elif os.path.exists(icon_path_png):
                # PyQt может загружать PNG напрямую, но Pillow может быть полезен для обработки
                # img = Image.open(icon_path_png)
                # img = img.resize((32, 32), Image.Resampling.LANCZOS)
                # q_img = ImageQt(img) # Если нужна конвертация через Pillow
                # self.setWindowIcon(QIcon(QPixmap.fromImage(q_img)))
                self.setWindowIcon(QIcon(icon_path_png)) # Просто загружаем PNG
            else:
                logger.warning(f"Предупреждение: Файл иконки (app_icon.ico или app_icon.png) не найден в {script_dir}.")
        except Exception as e:
            logger.error(f"Произошла ошибка при загрузке иконки: {e}")

    def init_ui(self):
        central_widget = QWidget()
        main_layout = QHBoxLayout(central_widget)
        main_layout.setContentsMargins(20, 20, 20, 20)
        main_layout.setSpacing(20)

        # --- Левая панель (Параметры и Управление) ---
        left_panel_layout = QVBoxLayout()
        left_panel_layout.setSpacing(20)

        # Заголовок приложения
        title_label = QLabel("⚙️ СИСТЕМА УПРАВЛЕНИЯ ПАРАМЕТРАМИ И СКРИПТАМИ")
        title_label.setFont(FONT_TITLE)
        title_label.setStyleSheet(f"color: {COLOR_ACCENT_GOLD};")
        title_label.setAlignment(Qt.AlignmentFlag.AlignCenter)
        left_panel_layout.addWidget(title_label)

        subtitle_label = QLabel("____________________________________________________________------------------------------------------------------------------------------------Конфигурация системных настроек------------------------------------------------------------------------------------____________________________________________________________")
        subtitle_label.setFont(FONT_INFO)
        subtitle_label.setStyleSheet(f"color: {COLOR_TEXT_MUTED};")
        subtitle_label.setAlignment(Qt.AlignmentFlag.AlignCenter)
        left_panel_layout.addWidget(subtitle_label)

        # Область прокрутки для параметров
        scroll_area = QScrollArea()
        scroll_area.setWidgetResizable(True)
        scroll_area.setStyleSheet(f"QScrollArea {{ border: none; background: {COLOR_PRIMARY_BG}; }}")
        scroll_content = QWidget()
        self.params_layout = QVBoxLayout(scroll_content)
        self.params_layout.setContentsMargins(0, 0, 0, 0) # Убираем отступы внутри scroll_content
        self.params_layout.setSpacing(20)
        scroll_area.setWidget(scroll_content)
        left_panel_layout.addWidget(scroll_area, stretch=4)


        self.entries = {} # Словарь для хранения ссылок на виджеты QLineEdit
        self.load_and_create_gui_elements() # Загрузка параметров и создание полей ввода

        # Кнопки Сохранить и Закрыть
        buttons_frame_layout = QHBoxLayout()
        buttons_frame_layout.setSpacing(20)
        buttons_frame_layout.setContentsMargins(0, 0, 0, 0)

        self.save_btn = QPushButton("💾  Сохранить изменения")
        self.save_btn.setFont(FONT_BUTTON)
        self.save_btn.clicked.connect(self.save_all)
        self.save_btn.setObjectName("saveButton") # Для стилизации
        buttons_frame_layout.addWidget(self.save_btn)

        self.close_btn = QPushButton("❌  Закрыть приложение")
        self.close_btn.setFont(FONT_BUTTON)
        self.close_btn.clicked.connect(self.close)
        self.close_btn.setObjectName("closeButton") # Для стилизации
        buttons_frame_layout.addWidget(self.close_btn)

        left_panel_layout.addLayout(buttons_frame_layout)

        main_layout.addLayout(left_panel_layout, 3) # Левая панель занимает 3 части

        # --- Правая панель (Управление Скриптом и Логи) ---
        right_panel_layout = QVBoxLayout()
        right_panel_layout.setSpacing(20)

        script_control_group = QGroupBox("Управление Скриптом и Логи")
        script_control_group.setFont(FONT_HEADER)
        script_control_layout = QVBoxLayout(script_control_group)
        script_control_layout.setContentsMargins(20, 30, 20, 20)
        script_control_layout.setSpacing(15)

        # Кнопки управления скриптом
        control_buttons_layout = QHBoxLayout()
        control_buttons_layout.setSpacing(10)

        self.start_script_btn = QPushButton("Начать")
        self.start_script_btn.setFont(FONT_BUTTON)
        self.start_script_btn.clicked.connect(self.start_script)
        self.start_script_btn.setObjectName("startButton")
        control_buttons_layout.addWidget(self.start_script_btn)

        self.pause_script_btn = QPushButton("Пауза")
        self.pause_script_btn.setFont(FONT_BUTTON)
        self.pause_script_btn.clicked.connect(self.pause_script)
        self.pause_script_btn.setEnabled(False) # Пауза пока не реализована
        self.pause_script_btn.setObjectName("pauseButton")
        control_buttons_layout.addWidget(self.pause_script_btn)

        self.stop_script_btn = QPushButton("Завершить")
        self.stop_script_btn.setFont(FONT_BUTTON)
        self.stop_script_btn.clicked.connect(self.stop_script)
        self.stop_script_btn.setEnabled(False)
        self.stop_script_btn.setObjectName("stopButton")
        control_buttons_layout.addWidget(self.stop_script_btn)

        script_control_layout.addLayout(control_buttons_layout)

        # Лог-дисплей
        log_label = QLabel("Логи процессов автоматизации:")
        log_label.setFont(FONT_LABEL)
        script_control_layout.addWidget(log_label)

        self.log_text = QTextEdit()
        self.log_text.setReadOnly(True)
        self.log_text.setFont(QFont("Consolas", 10))
        script_control_layout.addWidget(self.log_text, stretch=1)

        # Кнопки управления логом
        log_control_buttons_layout = QHBoxLayout()
        log_control_buttons_layout.setSpacing(10)

        clear_log_button = QPushButton("Очистить Лог")
        clear_log_button.setFont(QFont("Segoe UI", 10, QFont.Weight.Bold))
        clear_log_button.clicked.connect(self.clear_log)
        log_control_buttons_layout.addWidget(clear_log_button)

        save_log_button = QPushButton("Сохранить Лог")
        save_log_button.setFont(QFont("Segoe UI", 10, QFont.Weight.Bold))
        save_log_button.clicked.connect(self.save_log)
        log_control_buttons_layout.addWidget(save_log_button)

        script_control_layout.addLayout(log_control_buttons_layout)

        self.autoscroll_checkbox = QCheckBox("Автопрокрутка")
        self.autoscroll_checkbox.setChecked(True)
        self.autoscroll_checkbox.setFont(QFont("Segoe UI", 10))
        script_control_layout.addWidget(self.autoscroll_checkbox, alignment=Qt.AlignmentFlag.AlignRight)

        self.status_bar_label = QLabel("Готов к запуску")
        self.status_bar_label.setFont(QFont("Segoe UI", 12))
        self.status_bar_label.setStyleSheet(f"background: {COLOR_SECONDARY_BG}; color: {COLOR_TEXT_PRIMARY}; padding: 5px;")
        self.status_bar_label.setAlignment(Qt.AlignmentFlag.AlignLeft | Qt.AlignmentFlag.AlignVCenter)
        script_control_layout.addWidget(self.status_bar_label)

        right_panel_layout.addWidget(script_control_group, stretch=1)

        # Информационная панель
        info_panel = QGroupBox("Подсказки")
        info_panel.setFont(QFont("Segoe UI", 13, QFont.Weight.Bold))
        info_layout = QVBoxLayout(info_panel)
        info_layout.setContentsMargins(15, 25, 15, 15)
        info_layout.setSpacing(5)

        info_text = QLabel("⌨️ ESC - Выход  •  Tab - Навигация  •  Enter - Сохранить параметры")
        info_text.setFont(FONT_INFO)
        info_text.setStyleSheet(f"color: {COLOR_TEXT_MUTED};")
        info_layout.addWidget(info_text)
        right_panel_layout.addWidget(info_panel)


        main_layout.addLayout(right_panel_layout, 5) # Правая панель занимает 5 частей

        self.setCentralWidget(central_widget)
        self.showMaximized() # Разворачиваем окно на весь экран

    def read_json_value(self, path, key_info):
        """Читает значения из JSON файла."""
        try:
            with open(path, 'r', encoding='utf-8') as f:
                data = json.load(f)
                if key_info is None and isinstance(data, (int, float)):
                    return data
                elif isinstance(data, dict):
                    if isinstance(key_info, dict) and "key_map" in key_info:
                        values = {}
                        for _, internal_key in key_info["key_map"].items():
                            values[internal_key] = data.get(internal_key, "")
                        return values
                    elif key_info in data:
                        return data.get(key_info, "")
                logger.warning(f"Предупреждение: Файл {path} содержит неожиданный формат данных.")
                return None
        except (FileNotFoundError, json.JSONDecodeError) as e:
            logger.error(f"Ошибка чтения файла {path}: {e}")
            return None
        except Exception as e:
            logger.error(f"Неизвестная ошибка при чтении файла {path}: {e}")
            return None

    def write_json_value(self, path, key_info, updated_values_or_single_value):
        """Записывает значения в JSON файл."""
        try:
            data_to_write = {}
            if os.path.exists(path) and os.path.getsize(path) > 0:
                try:
                    with open(path, 'r', encoding='utf-8') as f:
                        existing_data = json.load(f)
                        if isinstance(existing_data, dict):
                            data_to_write = existing_data
                        elif key_info is not None and (not isinstance(key_info, dict) or "key_map" not in key_info):
                            pass
                        else:
                            pass
                except json.JSONDecodeError:
                    logger.warning(f"Предупреждение: Файл {path} поврежден или пуст, будет перезаписан.")
                    data_to_write = {}
            else:
                logger.info(f"Файл {path} не найден или пуст. Будет создан новый файл.")

            if key_info is None:
                data_to_write = updated_values_or_single_value
            elif isinstance(key_info, dict) and "key_map" in key_info:
                if isinstance(updated_values_or_single_value, dict):
                    data_to_write.update(updated_values_or_single_value)
                else:
                    logger.error(f"Ошибка: ожидался словарь для key_map, но получено {type(updated_values_or_single_value)}")
            else:
                if isinstance(data_to_write, dict):
                    data_to_write[key_info] = updated_values_or_single_value
                else:
                    data_to_write = {key_info: updated_values_or_single_value}

            os.makedirs(os.path.dirname(path), exist_ok=True)

            with open(path, 'w', encoding='utf-8') as f:
                json.dump(data_to_write, f, indent=2, ensure_ascii=False)
            return True
        except Exception as e:
            QMessageBox.critical(self, "Ошибка записи", f"Не удалось записать в файл:\n{path}\n\nОшибка: {e}")
            logger.error(f"Ошибка при записи в {path}: {e}")
            return False

    def load_and_create_gui_elements(self):
        """Загружает данные и динамически создает элементы GUI для каждого файла."""
        for path, info in FILES_CONFIG.items():
            card_group = QGroupBox()
            card_group.setStyleSheet(f"""
                QGroupBox {{
                    border: 2px solid {COLOR_ACCENT_GOLD};
                    border-radius: 8px;
                    margin-top: 10px;
                    padding-top: 1px;
                    background: {COLOR_SECONDARY_BG};
                }}
                QGroupBox::title {{
                    subcontrol-origin: margin;
                    subcontrol-position: top left;
                    padding: 0 5px;
                    background: {COLOR_SECONDARY_BG};
                    color: {COLOR_ACCENT_GOLD};
                    font-weight: bold;
                    font-size: 14px;
                    left: 10px;
                }}
            """)
            card_layout = QVBoxLayout(card_group)
            card_layout.setContentsMargins(35, 25, 35, 25)
            card_layout.setSpacing(15)

            card_title = QLabel(info['name'])
            card_title.setFont(FONT_HEADER)
            card_title.setStyleSheet(f"color: {COLOR_TEXT_PRIMARY};")
            card_title.setWordWrap(True)
            card_layout.addWidget(card_title)

            separator = QWidget()
            separator.setFixedHeight(2)
            separator.setStyleSheet(f"background: {COLOR_ACCENT_GOLD};")
            card_layout.addWidget(separator)

            if "key_map" in info:
                current_values = self.read_json_value(path, info)
                if current_values is None:
                    current_values = {}
                    QMessageBox.warning(self, "Ошибка чтения", f"Не удалось прочитать или декодировать JSON из файла:\n{path}\nПожалуйста, убедитесь, что файл существует и корректен.")

                self.entries.update({path: {"entries": {}, "key_info": info}})

                for display_name, internal_key in info["key_map"].items():
                    param_label = QLabel(f"{display_name}:")
                    param_label.setFont(FONT_LABEL)
                    param_label.setStyleSheet(f"color: {COLOR_TEXT_SECONDARY};")
                    card_layout.addWidget(param_label)

                    param_entry = QLineEdit()
                    param_entry.setFont(FONT_ENTRY)
                    param_entry.setAlignment(Qt.AlignmentFlag.AlignCenter)
                    param_entry.setPlaceholderText("Введите значение...")

                    initial_value = str(current_values.get(internal_key, "")) if isinstance(current_values, dict) else ""
                    param_entry.setText(initial_value)
                    card_layout.addWidget(param_entry)
                    self.entries.get(path)["entries"].update({internal_key: param_entry})

            else:
                val = self.read_json_value(path, info.get("key"))
                if val is None:
                    val = ""
                    QMessageBox.warning(self, "Ошибка чтения", f"Не удалось прочитать или декодировать JSON из файла:\n{path}\nПожалуйста, убедитесь, что файл существует и корректен.")

                single_entry = QLineEdit()
                single_entry.setFont(FONT_ENTRY)
                single_entry.setAlignment(Qt.AlignmentFlag.AlignCenter)
                single_entry.setPlaceholderText("Введите значение...")
                single_entry.setText(str(val))
                card_layout.addWidget(single_entry)
                self.entries.update({path: {"entry": single_entry, "key_info": info.get("key")}})

            self.params_layout.addWidget(card_group)

    def save_all(self):
        """Сохраняет все измененные значения в соответствующие JSON файлы."""
        success_overall = True
        for path, data in self.entries.items():
            if "entries" in data:
                updated_values = {}
                for internal_key, entry_widget in data["entries"].items():
                    value_str = entry_widget.text().strip().replace(",", ".")
                    try:
                        val = float(value_str) if "." in value_str else int(value_str)
                        updated_values[internal_key] = val
                    except ValueError:
                        winsound.MessageBeep(winsound.MB_ICONHAND)
                        display_name_for_error = next(
                            (k for k, v in data["key_info"]["key_map"].items() if v == internal_key),
                            internal_key
                        )
                        QMessageBox.warning(self, "Ошибка ввода", f"Неверное значение для '{display_name_for_error}' в файле:\n{path}\nОжидается число.")
                        return
                if not self.write_json_value(path, data["key_info"], updated_values):
                    success_overall = False
            else:
                value_str = data["entry"].text().strip().replace(",", ".")
                try:
                    val = float(value_str) if "." in value_str else int(value_str)
                    if not self.write_json_value(path, data["key_info"], val):
                        success_overall = False
                except ValueError:
                    winsound.MessageBeep(winsound.MB_ICONHAND)
                    QMessageBox.warning(self, "Ошибка ввода", f"Неверное значение для файла:\n{path}\nОжидается число.")
                    return

        if success_overall:
            play_success_sound()
            self.status_bar_label.setText("Все параметры успешно сохранены!")
        else:
            self.status_bar_label.setText("Ошибка при сохранении некоторых параметров.")

    def append_log(self, text):
        """Добавляет сообщение в лог-дисплей."""
        self.log_text.append(text.strip()) # strip() для удаления лишних переносов строк
        if self.autoscroll_checkbox.isChecked():
            self.log_text.verticalScrollBar().setValue(self.log_text.verticalScrollBar().maximum())

    def start_script(self):
        """Начинает выполнение внешнего скрипта."""
        if self.script_runner and self.script_runner.isRunning():
            self.append_log("Скрипт уже запущен.\n")
            return

        self.clear_log()
        self.append_log("Запуск скрипта...\n")
        self.status_bar_label.setText("Скрипт запущен...")

        self.start_script_btn.setEnabled(False)
        self.pause_script_btn.setEnabled(False) # Пауза пока не реализована для внешних скриптов
        self.stop_script_btn.setEnabled(True)

        script_dir = os.path.dirname(os.path.abspath(__file__))
        script_to_run = os.path.join(script_dir, 'ГЛАВА.py') # Убедитесь, что этот файл существует

        self.script_runner = ScriptRunner(script_to_run)
        self.script_runner.log_signal.connect(self.append_log)
        self.script_runner.finished.connect(self.script_finished)
        self.script_runner.start()

    def stop_script(self):
        """Завершает выполнение внешнего скрипта."""
        if self.script_runner and self.script_runner.isRunning():
            reply = QMessageBox.question(self, "Завершение скрипта",
                                         "Скрипт все еще запущен. Вы уверены, что хотите его завершить?",
                                         QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No)
            if reply == QMessageBox.StandardButton.Yes:
                self.script_runner.stop_script()
                self.script_runner.wait() # Ждем завершения потока
                self.script_finished() # Обновляем состояние кнопок
            else:
                self.append_log("Завершение скрипта отменено.\n")
        else:
            self.append_log("Скрипт не запущен или уже завершен.\n")
            self.script_finished() # Убедимся, что кнопки в правильном состоянии

    def pause_script(self):
        """Функция паузы для внешнего скрипта. Прямая пауза подпроцесса сложна."""
        self.append_log("Функция 'Пауза' для внешнего скрипта не реализована напрямую. Скрипт продолжит работу.\n")
        # Для реальной паузы subprocess.Popen требуется отправка сигналов (например, SIGSTOP/SIGCONT),
        # что зависит от ОС и самого скрипта. Это выходит за рамки простой реализации.

    def script_finished(self):
        """Слот, вызываемый по завершении потока скрипта."""
        self.status_bar_label.setText("Скрипт завершил работу.")
        self.start_script_btn.setEnabled(True)
        self.pause_script_btn.setEnabled(False)
        self.stop_script_btn.setEnabled(False)

    def clear_log(self):
        """Очищает содержимое лог-дисплея."""
        self.log_text.clear()
        self.append_log("Лог очищен.\n")

    def save_log(self):
        """Сохраняет содержимое лог-дисплея в файл."""
        file_path, _ = QFileDialog.getSaveFileName(self, "Сохранить логи",
                                                   f"log_{datetime.datetime.now().strftime('%Y%m%d_%H%M%S')}.txt",
                                                   "Text Files (*.txt);;All Files (*.*)")
        if file_path:
            try:
                with open(file_path, "w", encoding="utf-8") as f:
                    f.write(self.log_text.toPlainText())
                self.append_log(f"Лог сохранен в: {file_path}\n")
            except Exception as e:
                QMessageBox.critical(self, "Ошибка сохранения", f"Ошибка при сохранении лога: {e}")
                self.append_log(f"Ошибка при сохранении лога: {e}\n")

    def terminate_python_subprocesses(self):
        """
        Находит и принудительно завершает все процессы 'python.exe' или 'pythonw.exe',
        кроме текущего процесса этого приложения.
        """
        try:
            current_pid = os.getpid()
            self.append_log("Завершение дочерних процессов Python...")
            terminated_count = 0
            # Итерируемся по всем запущенным процессам
            for proc in psutil.process_iter(['pid', 'name']):
                # Ищем процессы с именем python.exe (для консоли) или pythonw.exe (для GUI без консоли)
                if proc.info['name'].lower() in ['python.exe', 'pythonw.exe'] and proc.info['pid'] != current_pid:
                    self.append_log(f"Найден лишний процесс Python (PID: {proc.info['pid']}). Попытка завершения...")
                    try:
                        p = psutil.Process(proc.info['pid'])
                        p.terminate() # Отправляем сигнал на завершение
                        terminated_count += 1
                    except (psutil.NoSuchProcess, psutil.AccessDenied):
                        self.append_log(f"Не удалось завершить процесс с PID {proc.info['pid']}. Доступ запрещен или он уже завершен.")
            
            if terminated_count > 0:
                self.append_log(f"Успешно отправлен сигнал на завершение {terminated_count} процессов Python.")
            else:
                self.append_log("Лишних процессов Python для завершения не найдено.")
        except Exception as e:
            self.append_log(f"Произошла ошибка при поиске и завершении процессов: {e}")

    def closeEvent(self, event):
        """Обрабатывает закрытие окна и завершает все дочерние процессы Python."""
        should_close = True
        
        # Сначала стандартно спрашиваем пользователя, если основной скрипт запущен
        if self.script_runner and self.script_runner.isRunning():
            reply = QMessageBox.question(self, "Выход",
                                         "Скрипт все еще запущен. Вы уверены, что хотите выйти? Он будет завершен.",
                                         QMessageBox.StandardButton.Yes | QMessageBox.StandardButton.No)
            
            if reply == QMessageBox.StandardButton.Yes:
                self.append_log("Пользователь подтвердил выход. Завершение основного скрипта...")
                self.script_runner.stop_script()
                self.script_runner.wait() # Ждем завершения потока
            else:
                self.append_log("Выход отменен пользователем.")
                should_close = False # Отменяем закрытие
        
        # Если закрытие подтверждено (или скрипт не был запущен)
        if should_close:
            self.append_log("Приложение закрывается. Поиск и завершение всех подзадач Python...")
            self.terminate_python_subprocesses() # <--- ВЫЗОВ НОВОГО МЕТОДА
            event.accept() # Разрешаем закрытие окна
        else:
            event.ignore() # Запрещаем закрытие окна

if __name__ == "__main__":
    app = QApplication(sys.argv)
    window = MainWindow()
    window.show() # Используем show() вместо showMaximized() для тестирования,
                  # но showMaximized() также будет работать.
    sys.exit(app.exec())