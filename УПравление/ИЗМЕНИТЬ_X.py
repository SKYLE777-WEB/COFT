import tkinter as tk
from tkinter import messagebox, Canvas, Scrollbar
import json
import os
import winsound
from tkinter import font as tkFont
import sys
from PIL import Image, ImageTk

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

# Сопоставление старых цветов с новыми для совместимости
COLOR_GRADIENT_START = COLOR_ACCENT_GOLD
COLOR_GRADIENT_MID = COLOR_ACCENT_AMBER
COLOR_GRADIENT_END = COLOR_ACCENT_WARM
COLOR_ACCENT_NEON_CYAN = COLOR_BORDER_ACTIVE
COLOR_ACCENT_NEON_GREEN = COLOR_SUCCESS
COLOR_ACCENT_HOT_PINK = COLOR_ACCENT_GOLD
COLOR_ACCENT_ELECTRIC_BLUE = COLOR_BORDER_ACTIVE
COLOR_TEXT_LIGHT = COLOR_TEXT_MUTED
COLOR_TEXT_PURE_WHITE = COLOR_TEXT_PRIMARY
COLOR_TEXT_NEON = COLOR_ACCENT_GOLD
# COLOR_INPUT_BORDER - Оставляем как есть, уже определено.

# Сопоставление для совместимости с существующим кодом
COLOR_ACCENT_PRIMARY = COLOR_ACCENT_GOLD
COLOR_ACCENT_SECONDARY = COLOR_SUCCESS

# --- Корпоративные Шрифты ---
FONT_TITLE = ("Segoe UI", 26, "bold")
FONT_HEADER = ("Segoe UI", 22, "bold")
FONT_LABEL = ("Segoe UI", 16, "normal")
FONT_ENTRY = ("Segoe UI", 18, "normal")
FONT_BUTTON = ("Segoe UI", 16, "bold")
FONT_INFO = ("Segoe UI", 11, "italic")
FONT_CUSTOM_CLOSE = ("Segoe UI", 14, "bold") # Шрифт для кнопки закрытия

# Конфигурация файлов и их параметров
FILES_CONFIG = {
    r"C:\\Софт\\1TGlinkV1.0\\МИН_УЧАСТНИКОВ.json": {"name": "Минимальное количество учасников для чатов в OkSearch (рек - от 500)", "key": "min_members"},
    r"C:\\Софт\\3FiltrTGV1.0\\ПРОЦЕНТ.json": {
        "name": "Настройки процентов",
        "key_map": { # Сопоставление отображаемых имен с фактическими ключами JSON
            "Основной процент онлайна": "percent",
            "Вторичный процент": "large_chat_percent",
            "Минимум участников для вторичного процента": "large_chat_members_threshold"
        }
    },
    r"C:\\Софт\\5ChekLinksHUM\\Количество чатов в одном файле.json": {"name": "Количество чатов на аккаунт", "key": "chunk_size"}
}

def play_success_sound():
    """Воспроизводит звук успешного сохранения."""
    winsound.MessageBeep(winsound.MB_OK)

class ParameterManagerApp:
    def __init__(self, master):
        self.master = master
        self.master.title("Система управления параметрами")
        self.master.configure(bg=COLOR_PRIMARY_BG)
        
        # --- ВАЖНОЕ ИЗМЕНЕНИЕ: СКРЫВАЕМ СТАНДАРТНУЮ СТРОКУ ЗАГОЛОВКА ---
        self.master.overrideredirect(True) # Это убирает стандартную рамку и заголовок окна

        self.screen_width = self.master.winfo_screenwidth()
        self.screen_height = self.master.winfo_screenheight()
        # Разворачиваем окно на весь экран
        self.master.geometry(f"{self.screen_width}x{self.screen_height}+0+0")

        self.setup_icon()
        
        # --- НОВАЯ ФУНКЦИЯ: СОЗДАЕМ КАСТОМНУЮ СТРОКУ ЗАГОЛОВКА ---
        self.create_custom_title_bar()

        self.create_main_content_area()

        self.entries = {} # Словарь для хранения ссылок на виджеты Entry

        self.create_header() # Этот заголовок будет внутри прокручиваемой области, под кастомной строкой заголовка
        self.load_and_create_gui_elements()
        self.create_control_buttons()
        self.create_info_panel()

        self.bind_keyboard_shortcuts()

        # Update scroll region after all widgets are packed
        self.scrollable_frame.bind("<Configure>", self.on_frame_configure)

    def setup_icon(self):
        """Пытается установить иконку приложения."""
        try:
            script_dir = getattr(sys, '_MEIPASS', os.path.abspath(os.path.dirname(__file__)))
            icon_path = os.path.join(script_dir, 'app_icon.ico')
            if os.path.exists(icon_path):
                self.master.iconbitmap(icon_path)
        except tk.TclError:
            print("Не удалось загрузить иконку (Tkinter TclError).")
        except Exception as e:
            print(f"Произошла ошибка при загрузке иконки: {e}")

    # Методы для перетаскивания окна (уже были, но теперь привязаны к custom_title_bar)
    def start_move(self, event):
        self._x = event.x
        self._y = event.y

    def stop_move(self, event):
        self._x = None
        self._y = None

    def do_move(self, event):
        if self._x is not None and self._y is not None:
            deltax = event.x - self._x
            deltay = event.y - self._y
            x = self.master.winfo_x() + deltax
            y = self.master.winfo_y() + deltay
            self.master.geometry(f"+{x}+{y}")

    def create_custom_title_bar(self):
        """Создает пользовательскую строку заголовка для окна."""
        self.custom_title_bar = tk.Frame(self.master, bg=COLOR_TERTIARY_BG, height=35, relief="flat") # Цвет кастомной полосы
        self.custom_title_bar.pack(side="top", fill="x")

        # Название приложения
        self.custom_title_label = tk.Label(self.custom_title_bar,
                                           text="⚙️ Система Управления Параметрами",
                                           font=("Segoe UI", 12, "bold"), # Можно настроить шрифт
                                           bg=COLOR_TERTIARY_BG, # Цвет фона метки
                                           fg=COLOR_TEXT_PRIMARY, # Цвет текста метки
                                           padx=10,
                                           anchor="w")
        self.custom_title_label.pack(side="left", fill="x", expand=True)

        # Кнопка закрытия
        self.close_button = tk.Button(self.custom_title_bar,
                                      text="✕", # Символ крестика
                                      command=self.master.destroy,
                                      font=FONT_CUSTOM_CLOSE, # Специальный шрифт для кнопки закрытия
                                      bg=COLOR_TERTIARY_BG, # Цвет фона кнопки
                                      fg=COLOR_TEXT_PRIMARY, # Цвет текста кнопки
                                      activebackground=COLOR_ERROR, # Цвет при наведении
                                      activeforeground=COLOR_TEXT_PRIMARY,
                                      relief="flat",
                                      bd=0,
                                      padx=10, pady=0,
                                      cursor="hand2")
        self.close_button.pack(side="right", fill="y")

        # Привязка событий для перетаскивания окна (для всей полосы и для текста)
        self.custom_title_bar.bind("<ButtonPress-1>", self.start_move)
        self.custom_title_bar.bind("<ButtonRelease-1>", self.stop_move)
        self.custom_title_bar.bind("<B1-Motion>", self.do_move)
        
        self.custom_title_label.bind("<ButtonPress-1>", self.start_move)
        self.custom_title_label.bind("<ButtonRelease-1>", self.stop_move)
        self.custom_title_label.bind("<B1-Motion>", self.do_move)
        
        # Эффекты наведения для кнопки закрытия
        self.close_button.bind("<Enter>", lambda e: self.close_button.config(bg=COLOR_ERROR))
        self.close_button.bind("<Leave>", lambda e: self.close_button.config(bg=COLOR_TERTIARY_BG))


    def create_main_content_area(self):
        """Создает область прокрутки для основного содержимого."""
        # Canvas теперь должен быть дочерним элементом master, как и custom_title_bar
        self.canvas = Canvas(self.master, bg=COLOR_PRIMARY_BG, highlightthickness=0)
        self.canvas.pack(side="left", fill="both", expand=True)

        self.scrollbar = Scrollbar(self.master, orient="vertical", command=self.canvas.yview,
                                   width=0, # Делает полосу прокрутки невидимой (дизайн пользователя)
                                   troughcolor=COLOR_PRIMARY_BG,
                                   bg=COLOR_PRIMARY_BG, activebackground=COLOR_PRIMARY_BG,
                                   highlightbackground=COLOR_PRIMARY_BG, bd=0)
        self.scrollbar.pack(side="right", fill="y")

        self.canvas.configure(yscrollcommand=self.scrollbar.set)
        # Привязка для стандартной прокрутки колесиком мыши
        self.canvas.bind_all("<MouseWheel>", self.on_mouse_wheel)

        self.scrollable_frame = tk.Frame(self.canvas, bg=COLOR_PRIMARY_BG)
        # Размещаем фрейм в окне Canvas
        self.canvas.create_window((0, 0), window=self.scrollable_frame, anchor="nw")

    def on_frame_configure(self, event):
        """Обновляет область прокрутки Canvas при изменении размера фрейма."""
        self.canvas.configure(scrollregion=self.canvas.bbox("all"))

    def on_mouse_wheel(self, event):
        """Обрабатывает прокрутку колесиком мыши."""
        self.canvas.yview_scroll(int(-1*(event.delta/120)), "units")

    def create_header(self):
        """Создает корпоративный заголовок приложения."""
        # Этот заголовок теперь находится ВНУТРИ прокручиваемой области
        header_frame = tk.Frame(self.scrollable_frame, bg=COLOR_PRIMARY_BG)
        header_frame.pack(pady=(20, 10))

        title_label = tk.Label(header_frame,
                              text="⚙️ Система Управления Параметрами",
                              font=FONT_TITLE,
                              fg=COLOR_ACCENT_GOLD,
                              bg=COLOR_PRIMARY_BG)
        title_label.pack()

        subtitle_label = tk.Label(header_frame,
                                 text="          ____________________________________________________________------------------------------------------------------------------------------------Конфигурация системных настроек------------------------------------------------------------------------------------____________________________________________________________          ",
                                 font=FONT_INFO,
                                 fg=COLOR_TEXT_MUTED,
                                 bg=COLOR_PRIMARY_BG)
        subtitle_label.pack(pady=(8, 0))

    def read_json_value(self, path, key_info):
        """
        Читает значения из JSON файла.
        Если key_info содержит 'key_map', читает значения для всех сопоставленных ключей.
        Возвращает None на ошибку или если файл не содержит ожидаемых данных.
        """
        try:
            with open(path, 'r', encoding='utf-8') as f:
                data = json.load(f)
                if key_info is None and isinstance(data, (int, float)):
                    return data
                elif isinstance(data, dict):
                    if "key_map" in key_info:
                        values = {}
                        for _, internal_key in key_info["key_map"].items():
                            values[internal_key] = data.get(internal_key, "")
                        return values
                    elif key_info in data:
                        return data.get(key_info, "")
                # Если файл существует, но содержит неожиданный формат данных
                print(f"Предупреждение: Файл {path} содержит неожиданный формат данных.")
                return None
        except (FileNotFoundError, json.JSONDecodeError) as e:
            # Обработка случаев, когда файл не найден или имеет неверный формат JSON
            print(f"Ошибка чтения файла {path}: {e}")
            return None
        except Exception as e:
            print(f"Неизвестная ошибка при чтении файла {path}: {e}")
            return None

    def write_json_value(self, path, key_info, updated_values_or_single_value):
        """
        Записывает значения в JSON файл.
        Обновляет только те ключи, которые указаны в key_map, сохраняя другие.
        """
        try:
            data_to_write = {}
            # Попытка прочитать существующие данные, если файл не пуст
            if os.path.exists(path) and os.path.getsize(path) > 0:
                try:
                    with open(path, 'r', encoding='utf-8') as f:
                        existing_data = json.load(f)
                        if isinstance(existing_data, dict):
                            data_to_write = existing_data
                        elif key_info is not None and "key_map" not in key_info:
                            # Для файлов, которые ожидают один ключ в словаре,
                            # но содержат не-словарь, инициализируем пустым словарем
                            pass # data_to_write уже {}
                        else:
                            # Для файлов, которые ожидают простое значение,
                            # если там словарь, это будет перезаписано.
                            # Если там не-словарь и не простое значение, будет перезаписано.
                            pass # data_to_write уже {}

                except json.JSONDecodeError:
                    print(f"Предупреждение: Файл {path} поврежден или пуст, будет перезаписан.")
                    data_to_write = {}
            else:
                print(f"Файл {path} не найден или пуст. Будет создан новый файл.")

            if key_info is None: # Файл содержит простое значение (int/float)
                data_to_write = updated_values_or_single_value
            elif "key_map" in key_info: # Файл сопоставлен с несколькими ключами в словаре
                if isinstance(updated_values_or_single_value, dict):
                    data_to_write.update(updated_values_or_single_value)
                else:
                    # Этого не должно произойти, если логика save_all корректна
                    print(f"Ошибка: ожидался словарь для key_map, но получено {type(updated_values_or_single_value)}")
            else: # Файл с одним конкретным ключом в словаре (например, "chunk_size")
                if isinstance(data_to_write, dict):
                    data_to_write[key_info] = updated_values_or_single_value
                else:
                    # Если существующие данные не были словарем, но должны быть для этого типа файла
                    data_to_write = {key_info: updated_values_or_single_value}

            # Убедимся, что директория существует перед записью
            os.makedirs(os.path.dirname(path), exist_ok=True)

            with open(path, 'w', encoding='utf-8') as f:
                json.dump(data_to_write, f, indent=2, ensure_ascii=False)
            return True
        except Exception as e:
            messagebox.showerror("Ошибка записи", f"Не удалось записать в файл:\n{path}\n\nОшибка: {e}")
            print(f"Ошибка при записи в {path}: {e}")
            return False

    def load_and_create_gui_elements(self):
        """Загружает данные и динамически создает элементы GUI для каждого файла."""
        for path, info in FILES_CONFIG.items():
            # Внешний контейнер для карточки
            card_outer = tk.Frame(self.scrollable_frame, bg=COLOR_PRIMARY_BG)
            card_outer.pack(pady=20, padx=50, fill="x")

            # Основная карточка с корпоративным стилем
            card_frame = tk.Frame(card_outer,
                                 bg=COLOR_SECONDARY_BG,
                                 padx=35, pady=25,
                                 relief="flat",
                                 bd=1)
            card_frame.pack(fill="x")

            # Заголовок карточки
            title_frame = tk.Frame(card_frame, bg=COLOR_SECONDARY_BG)
            title_frame.pack(fill="x", pady=(0, 20))

            card_title = tk.Label(title_frame,
                                 text=info['name'],
                                 font=FONT_HEADER,
                                 fg=COLOR_TEXT_PRIMARY,
                                 bg=COLOR_SECONDARY_BG,
                                 wraplength=800,
                                 justify="left")
            card_title.pack(anchor="w")

            # Золотая разделительная линия
            separator = tk.Frame(card_frame, bg=COLOR_ACCENT_GOLD, height=2)
            separator.pack(fill="x", pady=(0, 20))

            if "key_map" in info:
                current_values = self.read_json_value(path, info)
                if current_values is None: # Обработка ошибки чтения файла
                    current_values = {} # По умолчанию пустой словарь, если файл плохой/отсутствует
                    messagebox.showwarning("Ошибка чтения", f"Не удалось прочитать или декодировать JSON из файла:\n{path}\nПожалуйста, убедитесь, что файл существует и корректен.")

                self.entries.update({path: {"entries": {}, "key_info": info}})

                for display_name, internal_key in info["key_map"].items():
                    # Контейнер для каждого параметра
                    param_frame = tk.Frame(card_frame, bg=COLOR_SECONDARY_BG)
                    param_frame.pack(fill="x", pady=8)

                    # Метка параметра
                    param_label = tk.Label(param_frame,
                                           text=f"{display_name}:",
                                           font=FONT_LABEL,
                                           fg=COLOR_TEXT_SECONDARY,
                                           bg=COLOR_SECONDARY_BG)
                    param_label.pack(anchor="w", pady=(0, 5))

                    # Поле ввода
                    param_entry = tk.Entry(param_frame,
                                           font=FONT_ENTRY,
                                           justify='center',
                                           width=145,
                                           bg=COLOR_INPUT_BG,
                                           fg=COLOR_TEXT_PRIMARY,
                                           insertbackground=COLOR_ACCENT_GOLD,
                                           relief="flat",
                                           highlightthickness=2,
                                           highlightbackground=COLOR_INPUT_BORDER,
                                           highlightcolor=COLOR_ACCENT_GOLD,
                                           bd=1,
                                           selectbackground=COLOR_ACCENT_AMBER,
                                           selectforeground=COLOR_PRIMARY_BG)

                    # Инициализируем значением, или пустой строкой, если current_values - не словарь
                    initial_value = str(current_values.get(internal_key, "")) if isinstance(current_values, dict) else ""
                    param_entry.insert(0, initial_value)
                    param_entry.pack(pady=(0, 10))
                    self.entries.get(path)["entries"].update({internal_key: param_entry})

                    self.bind_entry_focus_effects(param_entry)

            else: # Для файла с одиночным значением или одним ключом в словаре
                val = self.read_json_value(path, info["key"])
                if val is None: # Обработка ошибки чтения файла
                    val = "" # По умолчанию пустая строка, если файл плохой/отсутствует
                    messagebox.showwarning("Ошибка чтения", f"Не удалось прочитать или декодировать JSON из файла:\n{path}\nПожалуйста, убедитесь, что файл существует и корректен.")

                single_entry = tk.Entry(card_frame,
                                       font=FONT_ENTRY,
                                       justify='center',
                                       width=145,
                                       bg=COLOR_INPUT_BG,
                                       fg=COLOR_TEXT_PRIMARY,
                                       insertbackground=COLOR_ACCENT_GOLD,
                                       relief="flat",
                                       highlightthickness=2,
                                       highlightbackground=COLOR_INPUT_BORDER,
                                       highlightcolor=COLOR_ACCENT_GOLD,
                                       bd=1,
                                       selectbackground=COLOR_ACCENT_AMBER,
                                       selectforeground=COLOR_PRIMARY_BG)
                single_entry.insert(0, str(val))
                single_entry.pack(pady=15)
                self.entries.update({path: {"entry": single_entry, "key_info": info["key"]}})
                self.bind_entry_focus_effects(single_entry)

    def bind_entry_focus_effects(self, entry_widget):
        """Привязывает эффекты фокусировки для поля ввода."""
        def on_focus_in(event):
            entry_widget.config(highlightbackground=COLOR_ACCENT_GOLD, bg=COLOR_TERTIARY_BG)
        def on_focus_out(event):
            entry_widget.config(highlightbackground=COLOR_INPUT_BORDER, bg=COLOR_INPUT_BG)

        entry_widget.bind("<FocusIn>", on_focus_in)
        entry_widget.bind("<FocusOut>", on_focus_out)

    def save_all(self):
        """Сохраняет все измененные значения в соответствующие JSON файлы."""
        success_overall = True
        for path, data in self.entries.items():
            if "entries" in data: # Если это файл с несколькими изменяемыми ключами (с key_map)
                updated_values = {}
                for internal_key, entry_widget in data["entries"].items():
                    value_str = entry_widget.get().strip().replace(",", ".") # Заменяем запятую на точку для чисел
                    try:
                        # Преобразуем в float или int в зависимости от наличия десятичной точки
                        val = float(value_str) if "." in value_str else int(value_str)
                        updated_values[internal_key] = val
                    except ValueError:
                        winsound.MessageBeep(winsound.MB_ICONHAND)
                        # Находим отображаемое имя для сообщения об ошибке
                        display_name_for_error = next(
                            (k for k, v in data["key_info"]["key_map"].items() if v == internal_key),
                            internal_key # Запасной вариант, если отображаемое имя не найдено
                        )
                        messagebox.showerror("Ошибка ввода", f"Неверное значение для '{display_name_for_error}' в файле:\n{path}")
                        return # Прекращаем сохранение при первой ошибке
                # Вызываем write_json_value с полным info и собранными значениями
                if not self.write_json_value(path, data["key_info"], updated_values):
                    success_overall = False
            else: # Для файла с одиночным изменяемым значением
                value_str = data["entry"].get().strip().replace(",", ".")
                try:
                    val = float(value_str) if "." in value_str else int(value_str)
                    if not self.write_json_value(path, data["key_info"], val):
                        success_overall = False
                except ValueError:
                    winsound.MessageBeep(winsound.MB_ICONHAND)
                    messagebox.showerror("Ошибка ввода", f"Неверное значение для файла:\n{path}")
                    return # Прекращаем сохранение при первой ошибке

        if success_overall:
            play_success_sound() # Воспроизводим звук успеха, если все сохранено без ошибок
            # messagebox.showinfo("Сохранение завершено", "Все параметры успешно сохранены!") # Эту строку нужно закомментировать или удалить

    def create_control_buttons(self):
        """Создает кнопки управления "Сохранить" и "Закрыть"."""
        buttons_frame = tk.Frame(self.scrollable_frame, bg=COLOR_PRIMARY_BG)
        buttons_frame.pack(pady=(40, 30))

        # Кнопка "Сохранить"
        self.save_btn = tk.Button(buttons_frame,
                                  text="💾  Сохранить изменения",
                                  command=self.save_all,
                                  font=FONT_BUTTON,
                                  bg=COLOR_SUCCESS,
                                  fg=COLOR_TEXT_PRIMARY,
                                  padx=30, pady=12,
                                  relief="flat",
                                  activebackground=COLOR_ACCENT_AMBER,
                                  activeforeground=COLOR_PRIMARY_BG,
                                  cursor="hand2",
                                  bd=0)
        self.save_btn.pack(side="left", padx=25)

        # Кнопка "Закрыть" (теперь эта кнопка дублирует функционал кнопки в custom_title_bar)
        self.close_btn = tk.Button(buttons_frame,
                                   text="❌  Закрыть приложение",
                                   command=self.master.destroy,
                                   font=FONT_BUTTON,
                                   bg=COLOR_ERROR,
                                   fg=COLOR_TEXT_PRIMARY,
                                   padx=30, pady=12,
                                   relief="flat",
                                   activebackground="#dc2626",
                                   activeforeground=COLOR_TEXT_PRIMARY,
                                   cursor="hand2",
                                   bd=0)
        self.close_btn.pack(side="right", padx=25)

        # Эффекты наведения для кнопок
        self.save_btn.bind("<Enter>", self.on_save_enter)
        self.save_btn.bind("<Leave>", self.on_save_leave)
        self.close_btn.bind("<Enter>", self.on_close_enter)
        self.close_btn.bind("<Leave>", self.on_close_leave)

    def on_save_enter(self, event):
        self.save_btn.config(bg=COLOR_ACCENT_GOLD, fg=COLOR_PRIMARY_BG)

    def on_save_leave(self, event):
        self.save_btn.config(bg=COLOR_SUCCESS, fg=COLOR_TEXT_PRIMARY)

    def on_close_enter(self, event):
        self.close_btn.config(bg="#dc2626")

    def on_close_leave(self, event):
        self.close_btn.config(bg=COLOR_ERROR)

    def create_info_panel(self):
        """Создает информационную панель с подсказками по клавишам."""
        info_panel = tk.Frame(self.scrollable_frame, bg=COLOR_PRIMARY_BG)
        info_panel.pack(pady=(20, 15))

        info_text = tk.Label(info_panel,
                             text="⌨️ ESC - Выход  •  Tab - Навигация  •  Enter - Сохранить",
                             font=FONT_INFO,
                             fg=COLOR_TEXT_MUTED,
                             bg=COLOR_PRIMARY_BG)
        info_text.pack()

    def bind_keyboard_shortcuts(self):
        """Привязывает глобальные горячие клавиши."""
        self.master.bind("<Return>", lambda e: self.save_all())
        self.master.bind("<Escape>", lambda e: self.master.destroy())

if __name__ == "__main__":
    root = tk.Tk()
    app = ParameterManagerApp(root)
    root.mainloop()