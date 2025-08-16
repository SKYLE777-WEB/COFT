import pathlib
import json
import os
import re
import asyncio
import shutil
import logging
import csv
import io
import urllib.parse
import hashlib

from telegram import (
    Update,
    MessageEntity,
    ReplyKeyboardMarkup,
    KeyboardButton,
    InlineKeyboardMarkup,
    InlineKeyboardButton,
    ReplyKeyboardRemove,
    CallbackQuery,
    InputMediaPhoto
)
from telegram.constants import ParseMode
from telegram.ext import (
    ApplicationBuilder,
    CommandHandler,
    MessageHandler,
    ContextTypes,
    filters,
    CallbackQueryHandler,
    ConversationHandler
)

# --- НАСТРОЙКА ЛОГИРОВАНИЯ ---
BASE_DIR = pathlib.Path(__file__).resolve().parent
LOG_FILE_PATH = BASE_DIR / "bot_log.txt"

logging.basicConfig(
    filename=LOG_FILE_PATH,
    filemode='a',
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    level=logging.INFO
)
logger = logging.getLogger(__name__)

# --- КОНФИГУРАЦИЯ ---
TOKEN = "7848203824:AAGVFRsilL9ibTboyE46Hx-gZJw1kIptAvI"
MIN_FILE = BASE_DIR / "МИН_УЧАСТНИКОВ.json"
ARCHIVE_DIR = pathlib.Path("C:\\Софт\\1TGlinkV1.0\\АРХИВ")
TEMPLATES_DIR = pathlib.Path("C:\\Софт\\1TGlinkV1.0\\ШАБЛОНЫ")
FINAL_DEST_DIR = pathlib.Path("C:/Софт/2Onlinechat_checker V1.0")

# --- РАЗРЕШЕННЫЕ ПОЛЬЗОВАТЕЛИ ---
ALLOWED_USER_IDS = [6997178408] # Добавьте сюда Telegram ID пользователей, которым разрешен доступ

# --- КЛИЕНТЫ: КОНСТАНТЫ И ПАПКИ ---
CLIENTS_DIR = BASE_DIR / "КЛИЕНТЫ"
ACTIVE_CLIENTS_DIR = CLIENTS_DIR / "активные"
ARCHIVED_CLIENTS_DIR = CLIENTS_DIR / "архив"

# Создаем папки, если они не существуют
ARCHIVE_DIR.mkdir(parents=True, exist_ok=True)
FINAL_DEST_DIR.mkdir(parents=True, exist_ok=True)
TEMPLATES_DIR.mkdir(parents=True, exist_ok=True)
ACTIVE_CLIENTS_DIR.mkdir(parents=True, exist_ok=True)
ARCHIVED_CLIENTS_DIR.mkdir(parents=True, exist_ok=True)

# --- ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ ---

def is_allowed_user(user_id: int) -> bool:
    """Проверяет, есть ли user_id в списке разрешенных."""
    return user_id in ALLOWED_USER_IDS

def load_min_members() -> dict:
    """Загружает минимальное количество участников из файла для публичных и приватных чатов."""
    if MIN_FILE.exists():
        try:
            with open(MIN_FILE, "r", encoding="utf-8") as f:
                data = json.load(f)
                return {
                    "public_min_members": data.get("public_min_members", 0),
                    "private_min_members": data.get("private_min_members", 0)
                }
        except (json.JSONDecodeError, IOError):
            logger.error("Ошибка при загрузке МИН_УЧАСТНИКОВ.json", exc_info=True)
            pass
    return {"public_min_members": 0, "private_min_members": 0}

def save_min_members(public_value: int, private_value: int) -> None:
    """Сохраняет минимальное количество участников в файл для публичных и приватных чатов."""
    with open(MIN_FILE, "w", encoding="utf-8") as f:
        json.dump({
            "public_min_members": public_value,
            "private_min_members": private_value
        }, f, ensure_ascii=False, indent=2)

def parse_member_count(text: str) -> int:
    """Извлекает количество участников из строки (например, "-3.5k" -> 3500)."""
    text = str(text).replace('\xa0', ' ').replace('\u202f', ' ')
    if m := re.search(r"-?(\d+(?:[.,]\d+)?)k", text, re.I):
        try:
            return int(float(m.group(1).replace(",", ".")) * 1000)
        except ValueError:
            pass
    if m := re.search(r"-?(\d{1,7})", text):
        try:
            return int(m.group(1))
        except ValueError:
            pass
    return 0

def split_by_chat_type(links):
    """Разделяет ссылки на приватных и публичные."""
    private = [u for u in links if u.startswith("https://t.me/+")]
    public = [u for u in links if u.startswith("https://t.me/") and not u.startswith("https://t.me/+")]
    return private, public

def escape_markdown(text: str) -> str:
    """Экранирует специальные символы для MarkdownV2."""
    special_chars = r'_*[]()~`>#+-=|{}.!'
    for char in special_chars:
        text = text.replace(char, f'\\{char}')
    return text

# --- КЛАВИАТУРЫ ---

def main_menu_keyboard():
    buttons = [
        [KeyboardButton("Ручной режим"), KeyboardButton("Авто режим")],
        [KeyboardButton("Архив чатов"), KeyboardButton("Шаблоны")],
        [KeyboardButton("Клиенты"), KeyboardButton("Отбор чатов")]
    ]
    return ReplyKeyboardMarkup(buttons, resize_keyboard=True, one_time_keyboard=True)

def manual_mode_keyboard():
    buttons = [
        [KeyboardButton("Установить тег"), KeyboardButton("Узнать тег")],
        [KeyboardButton("Установить лимит"), KeyboardButton("Узнать лимит")],
        [KeyboardButton("✅ Готово (Получить чаты)"), KeyboardButton("Назад")],
    ]
    return ReplyKeyboardMarkup(buttons, resize_keyboard=True)

def auto_mode_keyboard():
    buttons = [[KeyboardButton("Назад")]]
    return ReplyKeyboardMarkup(buttons, resize_keyboard=True)

def archive_folder_inline_keyboard():
    if not ARCHIVE_DIR.is_dir():
        return None
    folders = sorted([f.name for f in ARCHIVE_DIR.iterdir() if f.is_dir()])
    buttons = [[InlineKeyboardButton(folder, callback_data=f"archive_{folder}")] for folder in folders]
    return InlineKeyboardMarkup(buttons) if buttons else None

def archive_menu_reply_keyboard():
    buttons = [
        [KeyboardButton("Статистика архива")],
        [KeyboardButton("Назад")]
    ]
    return ReplyKeyboardMarkup(buttons, resize_keyboard=True, one_time_keyboard=True)

def templates_menu_keyboard():
    buttons = [
        [KeyboardButton("Редактор"), KeyboardButton("Просмотр")],
        [KeyboardButton("Назад")]
    ]
    return ReplyKeyboardMarkup(buttons, resize_keyboard=True, one_time_keyboard=True)

def templates_editor_keyboard():
    buttons = [
        [KeyboardButton("Создать шаблон"), KeyboardButton("Редактировать шаблон")],
        [KeyboardButton("Удалить шаблон")],
        [KeyboardButton("Назад")]
    ]
    return ReplyKeyboardMarkup(buttons, resize_keyboard=True, one_time_keyboard=True)

def clients_menu_keyboard():
    buttons = [
        [KeyboardButton("Создать нового клиента")],
        [KeyboardButton("Посмотреть активных клиентов")],
        [KeyboardButton("Посмотреть архив клиентов")],
        [KeyboardButton("Назад")]
    ]
    return ReplyKeyboardMarkup(buttons, resize_keyboard=True, one_time_keyboard=True)

def clients_cancel_keyboard():
    buttons = [[KeyboardButton("Отмена")]]
    return ReplyKeyboardMarkup(buttons, resize_keyboard=True, one_time_keyboard=True)

def client_management_keyboard(is_active: bool):
    """Создает клавиатуру управления клиентом. Callback-данные не содержат имя клиента."""
    status_button = InlineKeyboardButton("🗄 Архивировать", callback_data="client_action_archive") if is_active \
        else InlineKeyboardButton("✅ Сделать активным", callback_data="client_action_unarchive")

    buttons = [
        [
            InlineKeyboardButton("➕ Добавить заметку", callback_data="client_action_add_note"),
            InlineKeyboardButton("✏️ Редактировать заметки", callback_data="client_action_edit_note")
        ],
        [
            InlineKeyboardButton("🖼 Добавить фото", callback_data="client_action_add_photo_menu")
        ],
        [status_button],
        [InlineKeyboardButton("❌ Удалить клиента", callback_data="client_action_delete")],
        [InlineKeyboardButton("⬅️ Назад к списку", callback_data="client_back_to_menu")]
    ]
    return InlineKeyboardMarkup(buttons)

# --- ОБРАБОТЧИКИ СОСТОЯНИЙ И КОМАНД ---
def clear_user_state(context: ContextTypes.DEFAULT_TYPE):
    keys_to_clear = [
        "mode_selected", "awaiting_min_input", "awaiting_tag_input",
        "current_tag", "user_links", "awaiting_archive_selection",
        "public_min_members", "private_min_members",
        "awaiting_template_action", "awaiting_client_name",
        "awaiting_client_note_for", "awaiting_client_photo_for",
        "awaiting_client_edit_note_for", "current_client_name", "photo_type",
        "awaiting_auto_file", "auto_chat_type", "files_to_move", "files_to_delete",
        "deleted_links", "summary_message_id", "links_to_delete_map", "next_callback_id"
    ]
    for key in keys_to_clear:
        context.user_data.pop(key, None)

# --- СОСТОЯНИЯ ДЛЯ CONVERSATION HANDLER ---
CHOOSING_CLIENT = 0
EDITING_CLIENT_NAME = 1
EDITING_CLIENT_NOTE = 2
PHOTO_TYPE = 3
AWAITING_PHOTO = 4
CHOOSING_AUTO_MODE_TYPE = 5
AWAITING_AUTO_TAG = 6
AWAITING_AUTO_FILE_UPLOAD = 7
SELECTING_FOLDER = 8 # Новое состояние для отбора чатов

# --- ГЛАВНЫЕ ХЕНДЛЕРЫ ---
async def start(update: Update, context: ContextTypes.DEFAULT_TYPE):
    """Обработка команды /start и проверка доступа."""
    if not is_allowed_user(update.effective_user.id):
        await update.message.reply_text("У вас нет доступа к этому боту.")
        return
    await main_menu(update, context)

async def main_menu(update: Update, context: ContextTypes.DEFAULT_TYPE):
    """Отображает главное меню после успешной аутентификации."""
    if isinstance(update, CallbackQuery):
        message_to_reply = update.message
    else:
        message_to_reply = update.message
    clear_user_state(context)
    await message_to_reply.reply_text(
        "Привет! Выберите режим работы бота:",
        reply_markup=main_menu_keyboard()
    )
    return ConversationHandler.END

async def go_back_to_main_menu(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    clear_user_state(context)
    await update.message.reply_text("Возврат в главное меню...", reply_markup=ReplyKeyboardRemove())
    await main_menu(update, context)
    return ConversationHandler.END

async def done(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    current_tag = context.user_data.get("current_tag")
    if not current_tag:
        await update.message.reply_text("Ошибка: Сначала установите тег через кнопку 'Установить тег'.")
        return
    links = context.user_data.get("user_links", [])
    
    private_links, public_links = split_by_chat_type(links)
    all_unique_links = sorted(list(set(links)))
    
    files_to_move = []
    files_to_delete = []
    files_sent = False

    # Обработка публичных чатов
    unique_public_links = sorted(list(set(public_links)))
    public_filename = f"{current_tag}_{len(unique_public_links)}_публичных.txt"
    try:
        with open(public_filename, "w", encoding="utf-8") as f:
            if unique_public_links:
                f.write("\n".join(unique_public_links))
        files_to_move.append(str(pathlib.Path(public_filename).resolve()))
        with open(public_filename, "rb") as doc:
            await update.message.reply_document(document=doc)
        files_sent = True
    except Exception as e:
        await update.message.reply_text(f"Произошла ошибка при создании файла '{public_filename}': {e}")
        logger.error(f"Ошибка при создании файла: {public_filename}", exc_info=True)

    # Обработка приватных чатов
    unique_private_links = sorted(list(set(private_links)))
    private_filename = f"{current_tag}_{len(unique_private_links)}_приватных.txt"
    try:
        with open(private_filename, "w", encoding="utf-8") as f:
            if unique_private_links:
                f.write("\n".join(unique_private_links))
        files_to_move.append(str(pathlib.Path(private_filename).resolve()))
        with open(private_filename, "rb") as doc:
            await update.message.reply_document(document=doc)
        files_sent = True
    except Exception as e:
        await update.message.reply_text(f"Произошла ошибка при создании файла '{private_filename}': {e}")
        logger.error(f"Ошибка при создании файла: {private_filename}", exc_info=True)
            
    # Обработка всех чатов (файл для удаления)
    all_filename = f"{current_tag}_{len(all_unique_links)}_все.txt"
    try:
        with open(all_filename, "w", encoding="utf-8") as f:
            if all_unique_links:
                f.write("\n".join(all_unique_links))
        files_to_delete.append(str(pathlib.Path(all_filename).resolve()))
        with open(all_filename, "rb") as doc:
            await update.message.reply_document(document=doc)
        files_sent = True
    except Exception as e:
        await update.message.reply_text(f"Произошла ошибка при создании файла '{all_filename}': {e}")
        logger.error(f"Ошибка при создании файла: {all_filename}", exc_info=True)

    context.user_data["files_to_move"] = files_to_move
    context.user_data["files_to_delete"] = files_to_delete
    
    if files_sent:
        keyboard = [[InlineKeyboardButton("отправить в папку", callback_data="move_to_folder")]]
        await update.message.reply_text("Готово! Файлы с чатами отправлены.", reply_markup=InlineKeyboardMarkup(keyboard))
        context.user_data.pop("user_links", None)
    else:
        await update.message.reply_text("Не найдено ссылок для формирования файлов.")
        clear_user_state(context)
    return ConversationHandler.END


async def move_files_to_final_folder(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    query = update.callback_query
    await query.answer()

    files_to_move = context.user_data.get("files_to_move", [])
    files_to_delete = context.user_data.get("files_to_delete", [])

    if not files_to_move and not files_to_delete:
        await query.edit_message_text(text=f"{query.message.text}\n\n(Файлы уже перемещены/удалены или не найдены)")
        return
    
    await context.bot.send_message(
        chat_id=update.effective_chat.id, 
        text="Начинаю перемещение файлов... Проверьте консоль для подробностей."
    )

    try:
        moved_count = 0
        for file_path_str in files_to_move:
            source_path = pathlib.Path(file_path_str)
            if source_path.exists():
                dest_path = FINAL_DEST_DIR / source_path.name
                print(f"Попытка переместить файл: '{source_path}' в '{dest_path}'")
                shutil.move(str(source_path), str(dest_path))
                print(f"✅ Успешно перемещен: {dest_path.name}")
                moved_count += 1
            else:
                print(f"❌ Файл не найден для перемещения: {source_path}")

        deleted_count = 0
        for file_path_str in files_to_delete:
            file_path = pathlib.Path(file_path_str)
            if file_path.exists():
                print(f"Попытка удалить файл: '{file_path}'")
                os.remove(file_path)
                print(f"✅ Успешно удален: {file_path.name}")
                deleted_count += 1
            else:
                print(f"❌ Файл не найден для удаления: {file_path}")

        await query.edit_message_reply_markup(reply_markup=None)
        await context.bot.send_message(
            chat_id=update.effective_chat.id,
            text=f"Успешно перемещено {moved_count} файла(ов) и удалено {deleted_count} файла(ов)."
        )
    except Exception as e:
        await context.bot.send_message(chat_id=update.effective_chat.id, text=f"❌ Произошла ошибка при перемещении/удалении: {e}")
        logger.error(f"Ошибка при перемещении/удалении файлов: {e}", exc_info=True)
    finally:
        clear_user_state(context)
    return ConversationHandler.END

# --- ГЛАВНЫЙ ОБРАБОТЧИК СООБЩЕНИЙ (ДИСПЕТЧЕР) ---
async def handle_message(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id):
        await update.message.reply_text("У вас нет доступа к этому боту.")
        return ConversationHandler.END
    
    if not update.message or not update.message.text: return ConversationHandler.END
    message_text = update.message.text
    
    if message_text == "Отмена":
        clear_user_state(context)
        await update.message.reply_text("Действие отменено.", reply_markup=clients_menu_keyboard())
        return ConversationHandler.END

    if client_name := context.user_data.get("awaiting_client_edit_note_for"):
        await edit_client_note(update, context, client_name, message_text)
        return ConversationHandler.END
        
    if client_name := context.user_data.get("awaiting_client_name"):
        await create_client_dossier(update, context, message_text)
        return ConversationHandler.END
    if client_name := context.user_data.get("awaiting_client_note_for"):
        await add_note_to_client(update, context, client_name, message_text)
        return ConversationHandler.END

    if context.user_data.get("awaiting_min_input"):
        parts = message_text.split(':')
        if len(parts) == 2:
            try:
                public_val, private_val = int(parts[0].strip()), int(parts[1].strip())
                if public_val < 0 or private_val < 0: raise ValueError
                save_min_members(public_val, private_val)
                await update.message.reply_text(
                    f"✅ Минимальный лимит установлен:\nПубличные: {public_val}\nприватных: {private_val}",
                    reply_markup=manual_mode_keyboard()
                )
            except (ValueError, TypeError):
                await update.message.reply_text("❌ Ошибка: введите два целых числа через двоеточие (e.g., 500:100).", reply_markup=manual_mode_keyboard())
            finally:
                context.user_data.pop("awaiting_min_input", None)
        else:
            await update.message.reply_text("❌ Ошибка: введите два числа через двоеточие (e.g., 500:100).", reply_markup=manual_mode_keyboard())
        return ConversationHandler.END

    if context.user_data.get("awaiting_tag_input"):
        if not re.fullmatch(r"[а-яА-ЯёЁa-zA-Z0-9\s_.-]+", message_text):
            await update.message.reply_text("❌ Ошибка: тег содержит недопустимые символы.", reply_markup=manual_mode_keyboard())
        else:
            tag = message_text.lower()
            context.user_data["current_tag"] = tag
            await update.message.reply_text(f"✅ Тег '{tag}' установлен.", reply_markup=manual_mode_keyboard())
        context.user_data.pop("awaiting_tag_input", None)
        return ConversationHandler.END

    if context.user_data.get("mode_selected") == "manual":
        await collect_links_from_message(update, context)
        return ConversationHandler.END
        
    return ConversationHandler.END

# --- ЛОГИКА РЕЖИМОВ ---
async def handle_menu_choice(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    choice = update.message.text
    clear_user_state(context)

    if choice == "Ручной режим":
        context.user_data["mode_selected"] = "manual"
        current_limits = load_min_members()
        context.user_data["public_min_members"] = current_limits["public_min_members"]
        context.user_data["private_min_members"] = current_limits["private_min_members"]
        await update.message.reply_text("Вы в 'Ручном режиме'.\nПересылайте сообщения с чатами, используйте кнопки для настроек.", reply_markup=manual_mode_keyboard())
    elif choice == "Авто режим":
        context.user_data["mode_selected"] = "auto"
        keyboard = [[KeyboardButton("публичные"), KeyboardButton("приватных")], [KeyboardButton("Назад")]]
        await update.message.reply_text("Выберите тип чатов для обработки:", reply_markup=ReplyKeyboardMarkup(keyboard, resize_keyboard=True))
        return CHOOSING_AUTO_MODE_TYPE
    elif choice == "Архив чатов":
        context.user_data["mode_selected"] = "archive"
        archive_inline_kb = archive_folder_inline_keyboard()
        if archive_inline_kb:
            await update.message.reply_text("Выберите папку архива для просмотра файлов:", reply_markup=archive_inline_kb)
        else:
            await update.message.reply_text("В архиве нет папок для выбора.", reply_markup=archive_menu_reply_keyboard())
        await update.message.reply_text("Дополнительные действия:", reply_markup=archive_menu_reply_keyboard())
    elif choice == "Шаблоны":
        context.user_data["mode_selected"] = "templates"
        await update.message.reply_text("Вы в меню 'Шаблоны'. Выберите действие:", reply_markup=templates_menu_keyboard())
    elif choice == "Клиенты":
        context.user_data["mode_selected"] = "clients"
        await update.message.reply_text("Вы в меню 'Клиенты'.", reply_markup=clients_menu_keyboard())
        return CHOOSING_CLIENT
    return ConversationHandler.END

async def handle_templates_menu_buttons(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    choice = update.message.text
    if choice == "Редактор":
        await update.message.reply_text("Функция редактирования шаблонов в текущем формате не поддерживается.")
    elif choice == "Просмотр":
        await view_templates(update, context)
    return ConversationHandler.END

async def handle_manual_mode_buttons(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    choice = update.message.text
    if choice == "Установить тег":
        context.user_data["awaiting_tag_input"] = True
        await update.message.reply_text("Какой тег хотите установить?")
    elif choice == "Узнать тег":
        current_tag = context.user_data.get("current_tag", "не установлен")
        await update.message.reply_text(f"Текущий тег: '{current_tag}'")
    elif choice == "Установить лимит":
        context.user_data["awaiting_min_input"] = True
        await update.message.reply_text("Введите лимит для публичных и приватных чатов в формате ПУБЛИЧНЫЕ:приватных (e.g., 500:100):")
    elif choice == "Узнать лимит":
        current_limits = load_min_members()
        public_min = context.user_data.get("public_min_members", current_limits["public_min_members"])
        private_min = context.user_data.get("private_min_members", current_limits["private_min_members"])
        await update.message.reply_text(f"Текущий лимит:\nПубличные: {public_min}\nприватных: {private_min}")
    elif choice == "✅ Готово (Получить чаты)":
        await done(update, context)
    return ConversationHandler.END

async def view_templates(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    templates = sorted([f.stem for f in TEMPLATES_DIR.glob("*.json")])
    if not templates:
        await update.message.reply_text("Шаблонов пока нет.")
        return
    buttons = [[InlineKeyboardButton(t, callback_data=f"show_template_{t}")] for t in templates]
    await update.message.reply_text("Выберите шаблон для просмотра:", reply_markup=InlineKeyboardMarkup(buttons))
    return ConversationHandler.END

# --- СИСТЕМА ШАБЛОНОВ ---
def load_template_blocks(template_name: str):
    template_file = TEMPLATES_DIR / f"{template_name}.json"
    if not template_file.exists(): return None
    try:
        with open(template_file, "r", encoding="utf-8") as f: return json.load(f)
    except (json.JSONDecodeError, IOError) as e:
        logger.error(f"Ошибка при загрузке шаблона '{template_name}': {e}")
        return None

async def show_template_block(message, context, template_name: str, block_id: str):
    if not is_allowed_user(message.chat_id): return
    template_blocks = load_template_blocks(template_name)
    if not template_blocks or str(block_id) not in template_blocks:
        await message.reply_text("Блок шаблона не найден.")
        return
    block_data = template_blocks[str(block_id)]
    text = block_data.get("text", "Текст не найден.")
    buttons_data = block_data.get("buttons", [])
    inline_buttons = []
    for btn in buttons_data:
        btn_list = [InlineKeyboardButton(btn.get("text"), callback_data=f"block_{template_name}_{btn.get('targetBlockId')}")]
        if btn_list: inline_buttons.append(btn_list)
    reply_markup = InlineKeyboardMarkup(inline_buttons) if inline_buttons else None
    def escape_html(text): return text.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
    formatted_text = f"<pre>{escape_html(text)}</pre>"
    await message.reply_text(formatted_text, reply_markup=reply_markup, parse_mode=ParseMode.HTML)

async def handle_template_callback(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    query = update.callback_query
    await query.answer()
    data = query.data
    if data.startswith("show_template_"):
        template_name = data.split("show_template_", 1)[1]
        await show_template_block(query.message, context, template_name, 0)
    elif data.startswith("block_"):
        parts = data.split('_', 2)
        template_name, block_id = parts[1], parts[2]
        await show_template_block(query.message, context, template_name, block_id)

# --- ЛОГИКА АРХИВА ---
async def handle_archive_callback(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    query = update.callback_query
    await query.answer()
    if query.data.startswith("archive_"):
        folder_name = query.data.split('_', 1)[1]
        found_folder = ARCHIVE_DIR / folder_name
        if not found_folder.is_dir():
            await query.message.chat.send_message("Папка не найдена.")
            return
        await query.message.edit_text(f"Отправляю файлы из архива '{found_folder.name}'...")
        files = list(found_folder.rglob('*.txt'))
        if not files:
            await query.message.chat.send_message("В выбранном архиве нет .txt файлов.")
        else:
            for file_path in files:
                try:
                    await query.message.chat.send_document(document=open(file_path, "rb"))
                except Exception as e:
                    await query.message.chat.send_message(f"Не удалось отправить '{file_path.name}': {e}")
                    logger.error(f"Ошибка при отправке файла из архива '{file_path.name}': {e}", exc_info=True)
        await query.message.chat.send_message("Все файлы из архива отправлены. Возврат в главное меню...")
        await main_menu(query, context)
    return ConversationHandler.END

async def show_archive_stats(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    if not ARCHIVE_DIR.is_dir():
        await update.message.reply_text("Папка архива не найдена.")
        return
        
    await update.message.reply_text("Подсчитываю уникальные чаты из файлов 'сбор...' и 'links...'")
    
    folder_unique_links = {}
    all_txt_files = list(ARCHIVE_DIR.glob('**/*.txt'))
    
    files_to_process = []
    for file_path in all_txt_files:
        file_name = file_path.name.lower()
        if file_name.startswith('сбор') or file_name.startswith('links'):
            files_to_process.append(file_path)
            
    if not files_to_process:
        await update.message.reply_text("В архиве не найдено файлов, начинающихся на 'сбор' или 'links'.", reply_markup=main_menu_keyboard())
        return ConversationHandler.END

    for file_path in files_to_process:
        try:
            relative_path = file_path.relative_to(ARCHIVE_DIR)
            top_level_folder = relative_path.parts[0] if relative_path.parts else "Корень архива"
            folder_unique_links.setdefault(top_level_folder, set())
            
            with open(file_path, 'r', encoding='utf-8') as f:
                for line in f:
                    cleaned_line = line.strip()
                    if cleaned_line.startswith('https://t.me/'):
                        folder_unique_links[top_level_folder].add(cleaned_line)

        except Exception as e:
            logger.error(f"Ошибка чтения файла {file_path}: {e}")

    report_message = "📊 Статистика по уникальным чатам (из файлов 'сбор...' и 'links...'):\n\n"
    total_archive_links = 0
    
    if folder_unique_links:
        report_message += "Количество уникальных чатов в каждой папке:\n"
        for folder, unique_links_set in sorted(folder_unique_links.items()):
            count = len(unique_links_set)
            report_message += f"  - 📁 {folder}: {count} чатов\n"
            total_archive_links += count
    else:
        report_message += "В подходящих файлах не найдено ссылок.\n"
        
    report_message += f"\n📈 Всего уникальных чатов во всем архиве: {total_archive_links}"
    
    await update.message.reply_text(report_message, reply_markup=main_menu_keyboard())
    clear_user_state(context)
    return ConversationHandler.END

# --- КЛИЕНТЫ: ЛОГИКА ---
async def start_clients_menu(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    await update.message.reply_text("Вы в меню 'Клиенты'.", reply_markup=clients_menu_keyboard())
    return CHOOSING_CLIENT

async def handle_clients_menu(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    choice = update.message.text
    if choice == "Создать нового клиента":
        await update.message.reply_text("Введите имя нового клиента:", reply_markup=clients_cancel_keyboard())
        return EDITING_CLIENT_NAME
    elif choice == "Посмотреть активных клиентов":
        await list_clients(context, update.message.chat_id, is_active=True)
    elif choice == "Посмотреть архив клиентов":
        await list_clients(context, update.message.chat_id, is_active=False)
    return CHOOSING_CLIENT

async def create_client_dossier(update: Update, context: ContextTypes.DEFAULT_TYPE, client_name: str):
    if not is_allowed_user(update.effective_user.id): return
    client_path = ACTIVE_CLIENTS_DIR / client_name
    if client_path.exists():
        await update.message.reply_text(f"❌ Клиент с именем '{client_name}' уже существует.", reply_markup=clients_menu_keyboard())
        return CHOOSING_CLIENT
    try:
        client_path.mkdir()
        (client_path / "notes.txt").touch()
        await update.message.reply_text(f"✅ Клиент '{client_name}' создан.", reply_markup=ReplyKeyboardRemove())
        context.user_data['current_client_name'] = client_name
        await show_client_dossier(context, update.message.chat_id, client_path, is_active=True)
    except Exception as e:
        await update.message.reply_text(f"Не удалось создать папку для клиента: {e}")
        logger.error(f"Не удалось создать папку для клиента: {e}", exc_info=True)
    return CHOOSING_CLIENT

async def list_clients(context: ContextTypes.DEFAULT_TYPE, chat_id: int, is_active: bool):
    if not is_allowed_user(chat_id): return
    base_path = ACTIVE_CLIENTS_DIR if is_active else ARCHIVED_CLIENTS_DIR
    status_text = "активных" if is_active else "архивных"
    clients = sorted([d.name for d in base_path.iterdir() if d.is_dir()])
    if not clients:
        await context.bot.send_message(chat_id, f"Список {status_text} клиентов пуст.", reply_markup=clients_menu_keyboard())
        return
    buttons = [[InlineKeyboardButton(name, callback_data=f"client_view_{name}")] for name in clients]
    await context.bot.send_message(chat_id, f"Выберите клиента из {status_text} списка:", reply_markup=InlineKeyboardMarkup(buttons))
    return CHOOSING_CLIENT

async def handle_photo_type_callback(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    query = update.callback_query
    await query.answer()
    client_name = context.user_data.get('current_client_name')
    if not client_name:
        await query.message.reply_text("Ошибка: клиент не выбран. Пожалуйста, вернитесь к списку.")
        logger.error("Ошибка в handle_photo_type_callback: client_name не найден в user_data")
        return CHOOSING_CLIENT

    if query.data.endswith('_correspondence'):
        photo_type = "переписка"
    elif query.data.endswith('_documents'):
        photo_type = "документы"
    else:
        await query.message.reply_text("Неизвестный тип фото.")
        logger.warning(f"Получен неизвестный тип фото: {query.data}")
        return CHOOSING_CLIENT

    context.user_data['awaiting_client_photo_for'] = client_name
    context.user_data['photo_type'] = photo_type
    logger.info(f"✅ Для клиента '{client_name}' установлен ожидаемый тип фото: '{photo_type}'. Данные сохранены в user_data.")
    
    await query.message.reply_text(f"Отправьте фото для '{client_name}' в категорию '{photo_type}':", reply_markup=clients_cancel_keyboard())
    return AWAITING_PHOTO

async def handle_client_callback(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    query = update.callback_query
    await query.answer()
    data = query.data
    logger.info(f"Получен callback от клиента: {data}")

    if data == "client_back_to_menu":
        await query.message.delete()
        context.user_data.pop('current_client_name', None)
        await context.bot.send_message(query.message.chat_id, "Вы в меню 'Клиенты'.", reply_markup=clients_menu_keyboard())
        return CHOOSING_CLIENT

    parts = data.split('_', 2)
    callback_type = parts[1]

    if callback_type == 'view':
        client_name = parts[2]
        context.user_data['current_client_name'] = client_name
        is_active = (ACTIVE_CLIENTS_DIR / client_name).exists()
        client_path = (ACTIVE_CLIENTS_DIR if is_active else ARCHIVED_CLIENTS_DIR) / client_name
        
        await query.message.delete()
        await show_client_dossier(context, query.message.chat_id, client_path, is_active)
        return CHOOSING_CLIENT

    elif callback_type == 'action':
        action = parts[2]
        client_name = context.user_data.get('current_client_name')
        if not client_name:
            await query.edit_message_text("Ошибка: клиент не выбран. Пожалуйста, вернитесь к списку и выберите клиента.", reply_markup=None)
            logger.error("Ошибка в handle_client_callback: client_name не найден в user_data")
            return CHOOSING_CLIENT
        
        chat_id = query.message.chat_id
        is_active = (ACTIVE_CLIENTS_DIR if client_name in [d.name for d in ACTIVE_CLIENTS_DIR.iterdir()] else False)
        client_path = (ACTIVE_CLIENTS_DIR if is_active else ARCHIVED_CLIENTS_DIR) / client_name

        if action in ["archive", "unarchive"]:
            new_path = ARCHIVED_CLIENTS_DIR / client_name if action == "archive" else ACTIVE_CLIENTS_DIR / client_name
            try:
                shutil.move(str(client_path), str(new_path))
                await query.message.delete()
                await context.bot.send_message(chat_id, f"✅ Клиент '{client_name}' был {'заархивирован' if action == 'archive' else 'сделан активным'}.")
                await list_clients(context, chat_id, is_active=not is_active)
            except Exception as e:
                await context.bot.send_message(chat_id, f"❌ Произошла ошибка при перемещении клиента: {e}")
                logger.error(f"Ошибка при перемещении клиента '{client_name}': {e}", exc_info=True)

        elif action == "delete":
            try:
                shutil.rmtree(client_path)
                await query.edit_message_text(f"✅ Личное дело клиента '{client_name}' было полностью удалено.", reply_markup=None)
            except Exception as e:
                await query.edit_message_text(f"❌ Ошибка при удалении клиента: {e}")
                logger.error(f"Ошибка при удалении клиента '{client_name}': {e}", exc_info=True)

        elif action == "add_note":
            context.user_data['awaiting_client_note_for'] = client_name
            await context.bot.send_message(chat_id, f"Введите новую заметку для клиента '{client_name}':", reply_markup=clients_cancel_keyboard())
        
        elif action == "edit_note":
            context.user_data['awaiting_client_edit_note_for'] = client_name
            notes_file = client_path / "notes.txt"
            notes_content = "Заметок пока нет."
            if notes_file.exists() and notes_file.read_text(encoding='utf-8').strip():
                notes_content = notes_file.read_text(encoding='utf-8')
            
            escaped_notes_content = escape_markdown(notes_content)
            
            await context.bot.send_message(
                chat_id,
                f"📝 *Редактирование заметок для '{escape_markdown(client_name)}'*\n\n**Текущий текст:**\n\\-\\-\-\n{escaped_notes_content}\n\\-\\-\\- \\n\nОтправьте полный новый текст, который заменит старый\\.",
                parse_mode=ParseMode.MARKDOWN_V2,
                reply_markup=clients_cancel_keyboard()
            )

        elif action == "add_photo_menu":
            client_name = context.user_data['current_client_name']
            photo_menu_keyboard = InlineKeyboardMarkup([
                [InlineKeyboardButton("🖼 Переписка", callback_data="client_photo_type_correspondence")],
                [InlineKeyboardButton("📄 Документы", callback_data="client_photo_type_documents")]
            ])
            await context.bot.send_message(chat_id, f"Выберите тип фото для клиента '{client_name}':", reply_markup=photo_menu_keyboard)
            return PHOTO_TYPE

    return CHOOSING_CLIENT

async def show_client_dossier(context, chat_id, client_path: pathlib.Path, is_active: bool):
    if not is_allowed_user(chat_id): return
    notes_file = client_path / "notes.txt"
    notes_content = "Заметок пока нет."
    if notes_file.exists() and notes_file.read_text(encoding='utf-8').strip():
        notes_content = notes_file.read_text(encoding='utf-8')
    
    escaped_client_name = escape_markdown(client_path.name)
    escaped_notes_content = escape_markdown(notes_content)

    await context.bot.send_message(
        chat_id, 
        f"\\-\\-\\- Личное дело: {escaped_client_name} \\-\\-\\- \n\n📝 Заметки:\n{escaped_notes_content}",
        parse_mode=ParseMode.MARKDOWN_V2
    )

    corr_path = client_path / "переписка"
    docs_path = client_path / "документы"

    # Сбор фотографий для переписки
    photos_corr = list(corr_path.glob('*.jpg')) + list(corr_path.glob('*.png'))
    if photos_corr:
        await context.bot.send_message(chat_id, "🖼 Переписка:")
        media_group = []
        for photo_path in photos_corr:
            media_group.append(InputMediaPhoto(media=open(photo_path, 'rb')))
            if len(media_group) == 10:
                await context.bot.send_media_group(chat_id, media_group)
                media_group = []
        if media_group:
            await context.bot.send_media_group(chat_id, media_group)
    
    # Сбор фотографий для документов
    photos_docs = list(docs_path.glob('*.jpg')) + list(docs_path.glob('*.png'))
    if photos_docs:
        await context.bot.send_message(chat_id, "📄 Документы:")
        media_group = []
        for photo_path in photos_docs:
            media_group.append(InputMediaPhoto(media=open(photo_path, 'rb')))
            if len(media_group) == 10:
                await context.bot.send_media_group(chat_id, media_group)
                media_group = []
        if media_group:
            await context.bot.send_media_group(chat_id, media_group)
                
    if not photos_corr and not photos_docs:
        await context.bot.send_message(chat_id, "Фотографий нет.")

    await context.bot.send_message(chat_id, "Выберите действие:", reply_markup=client_management_keyboard(is_active))

async def add_note_to_client(update: Update, context: ContextTypes.DEFAULT_TYPE, client_name: str, text: str):
    if not is_allowed_user(update.effective_user.id): return
    is_active = (ACTIVE_CLIENTS_DIR / client_name).exists()
    client_path = (ACTIVE_CLIENTS_DIR if is_active else ARCHIVED_CLIENTS_DIR) / client_name
    
    if not client_path.exists():
        await update.message.reply_text(f"Клиент '{client_name}' не найден.")
    else:
        try:
            with open(client_path / "notes.txt", "a", encoding="utf-8") as f:
                f.write(f"\n- {text}")
            await update.message.reply_text(f"✅ Заметка добавлена клиенту '{client_name}'.", reply_markup=ReplyKeyboardRemove())
        except Exception as e:
            await update.message.reply_text(f"❌ Ошибка при добавлении заметки: {e}", reply_markup=ReplyKeyboardRemove())
            
    await show_client_dossier(context, update.message.chat_id, client_path, is_active)

async def edit_client_note(update: Update, context: ContextTypes.DEFAULT_TYPE, client_name: str, new_text: str):
    if not is_allowed_user(update.effective_user.id): return
    is_active = (ACTIVE_CLIENTS_DIR / client_name).exists()
    client_path = (ACTIVE_CLIENTS_DIR if is_active else ARCHIVED_CLIENTS_DIR) / client_name
    
    if not client_path.exists():
        await update.message.reply_text(f"Клиент '{client_name}' не найден.")
    else:
        try:
            with open(client_path / "notes.txt", "w", encoding="utf-8") as f:
                f.write(new_text)
            await update.message.reply_text(f"✅ Заметки для клиента '{client_name}' обновлены.", reply_markup=ReplyKeyboardRemove())
        except Exception as e:
            await update.message.reply_text(f"❌ Ошибка при обновлении заметки: {e}", reply_markup=ReplyKeyboardRemove())
            
    await show_client_dossier(context, update.message.chat_id, client_path, is_active)

async def add_photo_to_client(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    logger.info("Функция add_photo_to_client вызвана.")
    
    client_name = context.user_data.get('awaiting_client_photo_for')
    photo_type = context.user_data.get('photo_type')
    
    if not client_name or not photo_type:
        await update.message.reply_text("❌ Ошибка: Не удалось определить, для какого клиента и в какую категорию сохранить фото. Пожалуйста, попробуйте снова, начиная с меню клиента.")
        return CHOOSING_CLIENT
    
    is_active = (ACTIVE_CLIENTS_DIR / client_name).exists()
    client_path = (ACTIVE_CLIENTS_DIR if is_active else ARCHIVED_CLIENTS_DIR) / client_name
    target_path = client_path / photo_type

    if not client_path.exists():
        await update.message.reply_text(f"Клиент '{client_name}' не найден. Не могу сохранить фото.")
    else:
        try:
            target_path.mkdir(parents=True, exist_ok=True)
            
            file_to_download = await (update.message.photo[-1] if update.message.photo else update.message.document).get_file()
            
            file_extension = file_to_download.file_path.split('.')[-1]
            file_name = f"{client_name}_{file_to_download.file_unique_id}.{file_extension}"
            save_path = target_path / file_name
            
            await file_to_download.download_to_drive(save_path)
            
            await update.message.reply_text("✅ Фото успешно сохранено. Отправьте еще одно или нажмите 'Отмена'.", reply_markup=clients_cancel_keyboard())
            await update.message.delete()
        except Exception as e:
            await update.message.reply_text(f"❌ Ошибка при сохранении фото: {e}")
            logger.error(f"Ошибка при сохранении фото для клиента '{client_name}': {e}", exc_info=True)
    
    return AWAITING_PHOTO

async def cancel(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    if client_name := context.user_data.get('awaiting_client_photo_for'):
        is_active = (ACTIVE_CLIENTS_DIR / client_name).exists()
        client_path = (ACTIVE_CLIENTS_DIR if is_active else ARCHIVED_CLIENTS_DIR) / client_name
        await update.message.reply_text("Загрузка фото завершена.", reply_markup=ReplyKeyboardRemove())
        context.user_data.pop('awaiting_client_photo_for', None)
        context.user_data.pop('photo_type', None)
        await show_client_dossier(context, update.message.chat_id, client_path, is_active)
        return CHOOSING_CLIENT
    
    clear_user_state(context)
    await update.message.reply_text("Действие отменено.", reply_markup=clients_menu_keyboard())
    return CHOOSING_CLIENT

# --- СБОР ССЫЛОК ---
async def collect_links_from_message(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id): return
    if context.user_data.get("mode_selected") != "manual": return
    if not update.message.text and not update.message.caption: return
    if not update.message.entities and not update.message.caption_entities: return
    if not context.user_data.get("current_tag"):
        await update.message.reply_text("Сначала установите тег.")
        return

    text = update.message.text or update.message.caption
    entities = update.message.entities or update.message.caption_entities
    public_min = context.user_data.get("public_min_members", load_min_members()["public_min_members"])
    private_min = context.user_data.get("private_min_members", load_min_members()["private_min_members"])
    user_links = context.user_data.setdefault("user_links", [])
    added_count = 0

    for entity in entities:
        if entity.type not in (MessageEntity.URL, MessageEntity.TEXT_LINK): continue
        url = entity.url or text[entity.offset:entity.offset + entity.length]
        is_private = url.startswith("https://t.me/+")
        current_limit = private_min if is_private else public_min

        line_text = ""
        current_offset = 0
        for line in text.split('\n'):
            line_end = current_offset + len(line)
            if (current_offset <= entity.offset < line_end):
                line_text = line; break
            current_offset += len(line) + 1
        if not line_text: continue
        
        if parse_member_count(line_text) < current_limit: continue
        if url not in user_links:
            user_links.append(url); added_count += 1
            
    if added_count > 0:
        await update.message.reply_text(f"➕ Добавлено {added_count} новых ссылок. Всего: {len(user_links)}.")
    else:
        await update.message.reply_text(f"❌ Новых ссылок не добавлено (всего: {len(user_links)}).")

# --- АВТОМАТИЧЕСКИЙ РЕЖИМ ---
async def handle_auto_mode_entry(update: Update, context: ContextTypes.DEFAULT_TYPE):
    context.user_data["mode_selected"] = "auto"
    keyboard = [[KeyboardButton("публичные"), KeyboardButton("приватных")], [KeyboardButton("Назад")]]
    await update.message.reply_text("Выберите тип чатов для обработки:", reply_markup=ReplyKeyboardMarkup(keyboard, resize_keyboard=True))
    return CHOOSING_AUTO_MODE_TYPE

async def handle_auto_mode_type_choice(update: Update, context: ContextTypes.DEFAULT_TYPE):
    choice = update.message.text
    
    if choice == "приватных":
        await update.message.reply_text("Данная функция в разработке.", reply_markup=main_menu_keyboard())
        return ConversationHandler.END

    context.user_data["auto_chat_type"] = choice
    await update.message.reply_text(f"Выбран режим '{choice}'. Пожалуйста, введите ключевое слово (тег) для файла:", reply_markup=auto_mode_keyboard())
    return AWAITING_AUTO_TAG

async def handle_auto_tag_input(update: Update, context: ContextTypes.DEFAULT_TYPE):
    tag = update.message.text
    if not re.fullmatch(r"[а-яА-ЯёЁa-zA-Z0-9\s_.-]+", tag):
        await update.message.reply_text("❌ Ошибка: тег содержит недопустимые символы. Попробуйте еще раз.")
        return AWAITING_AUTO_TAG
    context.user_data["current_tag"] = tag
    await update.message.reply_text(f"✅ Тег '{tag}' установлен. Теперь отправьте CSV-файл.", reply_markup=auto_mode_keyboard())
    return AWAITING_AUTO_FILE_UPLOAD

async def process_auto_file(update: Update, context: ContextTypes.DEFAULT_TYPE):
    if not is_allowed_user(update.effective_user.id):
        return ConversationHandler.END
    
    if not update.message.document or update.message.document.mime_type != 'text/csv':
        await update.message.reply_text("❌ Ошибка: Пожалуйста, отправьте файл в формате .csv или нажмите 'Назад'.", reply_markup=auto_mode_keyboard())
        return AWAITING_AUTO_FILE_UPLOAD
        
    try:
        file_id = update.message.document.file_id
        file = await context.bot.get_file(file_id)
        
        file_stream = io.BytesIO()
        await file.download_to_memory(file_stream)
        file_stream.seek(0)
        
        csv_reader = csv.DictReader(io.TextIOWrapper(file_stream, 'utf-8'))
        
        all_links = []
        
        for row in csv_reader:
            username = row.get("username", "")
            chat_type = row.get("type", "")
            
            url = None
            if username and username.startswith("@"):
                url = f"https://t.me/{username[1:]}"
            elif chat_type == "private group" and row.get("invite_link"):
                url = row["invite_link"]
                
            if url:
                all_links.append(url)

        private_links, public_links = split_by_chat_type(all_links)
        unique_public_links = sorted(list(set(public_links)))
        unique_private_links = sorted(list(set(private_links)))
        all_unique_links = sorted(list(set(all_links)))
        
        current_tag = context.user_data.get("current_tag", "авто")
        
        files_to_move = []
        files_to_delete = []
        files_sent = False

        # Обработка публичных чатов (создается всегда)
        public_filename = f"{current_tag}_{len(unique_public_links)}_публичных.txt"
        try:
            with open(public_filename, "w", encoding="utf-8") as f:
                if unique_public_links:
                    f.write("\n".join(unique_public_links))
            files_to_move.append(str(pathlib.Path(public_filename).resolve()))
            with open(public_filename, "rb") as doc:
                await update.message.reply_document(document=doc)
            files_sent = True
        except Exception as e:
            await update.message.reply_text(f"Произошла ошибка при создании/отправке файла '{public_filename}': {e}")
            logger.error(f"Ошибка при создании/отправке файла: {public_filename}", exc_info=True)

        # Обработка приватных чатов (создается всегда)
        private_filename = f"{current_tag}_{len(unique_private_links)}_приватных.txt"
        try:
            with open(private_filename, "w", encoding="utf-8") as f:
                if unique_private_links:
                    f.write("\n".join(unique_private_links))
            files_to_move.append(str(pathlib.Path(private_filename).resolve()))
            with open(private_filename, "rb") as doc:
                await update.message.reply_document(document=doc)
            files_sent = True
        except Exception as e:
            await update.message.reply_text(f"Произошла ошибка при создании/отправке файла '{private_filename}': {e}")
            logger.error(f"Ошибка при создании/отправке файла: {private_filename}", exc_info=True)
            
        # Обработка всех чатов (файл для удаления, создается всегда)
        all_filename = f"{current_tag}_{len(all_unique_links)}_все.txt"
        try:
            with open(all_filename, "w", encoding="utf-8") as f:
                if all_unique_links:
                    f.write("\n".join(all_unique_links))
            files_to_delete.append(str(pathlib.Path(all_filename).resolve()))
            with open(all_filename, "rb") as doc:
                await update.message.reply_document(document=doc)
            files_sent = True
        except Exception as e:
            await update.message.reply_text(f"Произошла ошибка при создании/отправке файла '{all_filename}': {e}")
            logger.error(f"Ошибка при создании/отправке файла: {all_filename}", exc_info=True)


        context.user_data["files_to_move"] = files_to_move
        context.user_data["files_to_delete"] = files_to_delete

        if files_sent:
            keyboard = [[InlineKeyboardButton("отправить в папку", callback_data="move_to_folder")]]
            await update.message.reply_text("✅ Готово! Файлы с чатами отправлены.", reply_markup=InlineKeyboardMarkup(keyboard))
        else:
            await update.message.reply_text("❌ В файле не найдено подходящих ссылок.")

        return ConversationHandler.END

    except Exception as e:
        logger.error(f"Ошибка при обработке CSV-файла: {e}", exc_info=True)
        await update.message.reply_text(f"❌ Произошла ошибка при обработке файла: {e}")
        return AWAITING_AUTO_FILE_UPLOAD

# --- ФУНКЦИОНАЛ "ОТБОР ЧАТОВ" ---

async def start_chat_selection(update: Update, context: ContextTypes.DEFAULT_TYPE):
    """Начало процесса отбора чатов. Показывает папки из архива."""
    if not is_allowed_user(update.effective_user.id): return ConversationHandler.END
    
    clear_user_state(context)
    
    if not ARCHIVE_DIR.is_dir():
        await update.message.reply_text("Папка архива не найдена.", reply_markup=main_menu_keyboard())
        return ConversationHandler.END

    folders = sorted([f.name for f in ARCHIVE_DIR.iterdir() if f.is_dir()])
    if not folders:
        await update.message.reply_text("В архиве нет папок для выбора.", reply_markup=main_menu_keyboard())
        return ConversationHandler.END

    buttons = [[InlineKeyboardButton(folder, callback_data=f"select_folder_{folder}")] for folder in folders]
    keyboard = InlineKeyboardMarkup(buttons)
    await update.message.reply_text("Выберите папку для отбора чатов:", reply_markup=keyboard)
    return SELECTING_FOLDER

async def send_links_from_folder(update: Update, context: ContextTypes.DEFAULT_TYPE):
    """
    Ищет файлы рекурсивно (во всех подпапках).
    """
    query = update.callback_query
    await query.answer()
    folder_name = query.data.split("select_folder_", 1)[1]
    folder_path = ARCHIVE_DIR / folder_name

    if not folder_path.is_dir():
        await query.edit_message_text("Ошибка: Папка не найдена.")
        return ConversationHandler.END

    await query.edit_message_text(f"Начинаю отправку ссылок из папки '{folder_name}' и всех её подпапок...")

    files_to_process = []
    # Рекурсивный поиск во всех подпапках с помощью **/*.txt
    for file_path in folder_path.glob('**/*.txt'):
        file_name = file_path.name.lower()
        if file_name.startswith('сбор') or file_name.startswith('links'):
            files_to_process.append(file_path)

    if not files_to_process:
        await context.bot.send_message(query.message.chat_id, "В этой папке и её подпапках не найдено подходящих файлов ('сбор...' или 'links...').")
        return ConversationHandler.END

    context.user_data['links_to_delete_map'] = {}
    context.user_data['next_callback_id'] = 0
    
    links_sent_count = 0
    for file_path in files_to_process:
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                links = [line.strip() for line in f if line.strip().startswith('https://t.me/')]

            if links:
                 await context.bot.send_message(query.message.chat_id, f"--- Файл: {file_path.relative_to(ARCHIVE_DIR)} ---")

            for link in links:
                callback_id = context.user_data['next_callback_id']
                context.user_data['next_callback_id'] += 1

                # Сохраняем все необходимые данные в маппинге
                context.user_data['links_to_delete_map'][callback_id] = {
                    'file_path': str(file_path),
                    'link': link
                }

                callback_data = f"del_{callback_id}" # Короткая, уникальная callback_data
                keyboard = InlineKeyboardMarkup([[InlineKeyboardButton("Удалить", callback_data=callback_data)]])

                await context.bot.send_message(
                    chat_id=query.message.chat_id,
                    text=link,
                    reply_markup=keyboard
                )
                links_sent_count += 1
        except Exception as e:
            await context.bot.send_message(query.message.chat_id, f"Ошибка при чтении файла {file_path.name}: {e}")
            logger.error(f"Ошибка чтения файла {file_path}: {e}")

    if links_sent_count == 0:
        await context.bot.send_message(query.message.chat_id, "В подходящих файлах не найдено ссылок.")
    
    await context.bot.send_message(
        chat_id=query.message.chat_id, 
        text=f"✅ Отправка завершена. Всего отправлено: {links_sent_count} ссылок.",
        reply_markup=main_menu_keyboard()
    )
    return ConversationHandler.END

async def delete_link_from_file(update: Update, context: ContextTypes.DEFAULT_TYPE):
    """
    Удаляет ссылку из файла (включая вложенные) и обновляет отчет об удаленных.
    """
    query = update.callback_query
    await query.answer()

    try:
        callback_id = int(query.data.split('_', 1)[1])
    except (ValueError, IndexError):
        await query.message.edit_text(f"{query.message.text}\n\n(Ошибка: неверный формат callback_data)", reply_markup=None)
        return

    links_to_delete_map = context.user_data.get('links_to_delete_map', {})
    if callback_id not in links_to_delete_map:
        await query.message.edit_text(f"{query.message.text}\n\n(Ошибка: информация для удаления не найдена)", reply_markup=None)
        return

    link_data = links_to_delete_map.pop(callback_id)
    file_path_str = link_data['file_path']
    link_to_delete_text = link_data['link']
    file_path = pathlib.Path(file_path_str)

    if not file_path.exists():
        await context.bot.send_message(chat_id=query.message.chat_id, text=f"Ошибка: Файл {file_path.name} не найден.")
        await query.message.delete()
        return

    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            lines = f.readlines()
        
        lines_to_keep = [line for line in lines if line.strip() != link_to_delete_text]

        with open(file_path, 'w', encoding='utf-8') as f:
            f.writelines(lines_to_keep)
        
        await query.message.delete()
        
        deleted_links_list = context.user_data.setdefault('deleted_links', [])
        if link_to_delete_text not in deleted_links_list:
            deleted_links_list.append(link_to_delete_text)

        escaped_links = [escape_markdown(link) for link in deleted_links_list]
        report_text = "🗑️ *Удаленные чаты:*\n\n" + "\n".join(f"\\- {link}" for link in escaped_links)

        summary_message_id = context.user_data.get('summary_message_id')
        
        if summary_message_id:
            # Предотвращаем ошибку, если текст не изменился
            current_text = context.bot_data.get(f"summary_text_{summary_message_id}", "")
            if current_text != report_text:
                await context.bot.edit_message_text(
                    chat_id=query.message.chat_id,
                    message_id=summary_message_id,
                    text=report_text,
                    parse_mode=ParseMode.MARKDOWN_V2
                )
                context.bot_data[f"summary_text_{summary_message_id}"] = report_text
        else:
            sent_message = await context.bot.send_message(
                chat_id=query.message.chat_id,
                text=report_text,
                parse_mode=ParseMode.MARKDOWN_V2
            )
            context.user_data['summary_message_id'] = sent_message.message_id
            context.bot_data[f"summary_text_{sent_message.message_id}"] = report_text

    except Exception as e:
        await context.bot.send_message(chat_id=query.message.chat_id, text=f"Произошла ошибка при удалении ссылки: {e}")
        logger.error(f"Ошибка при удалении ссылки '{link_to_delete_text}' из файла '{file_path}': {e}", exc_info=True)


if __name__ == "__main__":
    app = ApplicationBuilder().token(TOKEN).build()
    
    main_conv_handler = ConversationHandler(
        entry_points=[
            MessageHandler(filters.Regex("^Клиенты$"), start_clients_menu),
            MessageHandler(filters.Regex("^Авто режим$"), handle_auto_mode_entry),
        ],
        states={
            CHOOSING_CLIENT: [
                MessageHandler(filters.Regex("^(Создать нового клиента|Посмотреть активных клиентов|Посмотреть архив клиентов)$"), handle_clients_menu),
                CallbackQueryHandler(handle_client_callback, pattern=r"^client_"),
            ],
            EDITING_CLIENT_NAME: [MessageHandler(filters.TEXT & ~filters.COMMAND & ~filters.Regex("^Отмена$"), create_client_dossier)],
            PHOTO_TYPE: [CallbackQueryHandler(handle_photo_type_callback, pattern=r"^client_photo_type_")],
            AWAITING_PHOTO: [MessageHandler(filters.PHOTO | filters.Document.IMAGE, add_photo_to_client)],
            CHOOSING_AUTO_MODE_TYPE: [MessageHandler(filters.Regex("^(публичные|приватных)$"), handle_auto_mode_type_choice)],
            AWAITING_AUTO_TAG: [MessageHandler(filters.TEXT & ~filters.COMMAND, handle_auto_tag_input)],
            AWAITING_AUTO_FILE_UPLOAD: [MessageHandler(filters.Document.MimeType('text/csv'), process_auto_file)],
        },
        fallbacks=[
            MessageHandler(filters.Regex("^Отмена$"), cancel),
            MessageHandler(filters.Regex("^Назад$"), go_back_to_main_menu)
        ],
    )
    
    chat_selection_conv_handler = ConversationHandler(
        entry_points=[MessageHandler(filters.Regex("^Отбор чатов$"), start_chat_selection)],
        states={
            SELECTING_FOLDER: [CallbackQueryHandler(send_links_from_folder, pattern=r"^select_folder_")]
        },
        fallbacks=[MessageHandler(filters.Regex("^Назад$"), go_back_to_main_menu)],
    )

    app.add_handler(CommandHandler("start", start))
    app.add_handler(MessageHandler(filters.Regex("^Назад$"), go_back_to_main_menu))
    app.add_handler(MessageHandler(filters.Regex("^(Ручной режим|Архив чатов|Шаблоны)$"), handle_menu_choice))
    app.add_handler(MessageHandler(
        filters.Regex("^(Установить тег|Узнать тег|Установить лимит|Узнать лимит|✅ Готово \\(Получить чаты\\))$"),
        handle_manual_mode_buttons))
    app.add_handler(MessageHandler(filters.Entity(MessageEntity.URL) | filters.Entity(MessageEntity.TEXT_LINK), collect_links_from_message))
    app.add_handler(CallbackQueryHandler(move_files_to_final_folder, pattern=r"^move_to_folder$"))
    app.add_handler(MessageHandler(filters.Regex("^Статистика архива$"), show_archive_stats))
    app.add_handler(CallbackQueryHandler(handle_archive_callback, pattern=r"^archive_"))
    app.add_handler(MessageHandler(filters.Regex("^(Редактор|Просмотр)$"), handle_templates_menu_buttons))
    app.add_handler(CallbackQueryHandler(handle_template_callback, pattern=r"^(show_template_|block_)"))
    
    app.add_handler(main_conv_handler)
    app.add_handler(chat_selection_conv_handler)
    
    app.add_handler(CallbackQueryHandler(delete_link_from_file, pattern=r"^del_"))

    app.add_handler(MessageHandler(filters.TEXT & ~filters.COMMAND, handle_message))
    
    print("Бот запущен...")
    app.run_polling()