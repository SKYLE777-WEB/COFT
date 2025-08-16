#!/usr/bin/env python3
"""chat_filter.py

Скрипт фильтрует Telegram-чаты на основе процента онлайна.

Исходные .txt-файлы должны лежать в папке "НЕ отработанные" рядом со скриптом.
Прошедшие фильтрацию чаты сохраняются в файл "прошли.txt" в папке "УСПЕШНО".
Остальные сохраняются в файл "не_прошли.txt" в той же папке.

Процент онлайна можно указать в файле ПРОЦЕНТ.json рядом со скриптом:
{
  "percent": 12.5,
  "large_chat_percent": 5.0,
  "large_chat_members_threshold": 1000
}
"""

from __future__ import annotations

import json
import re
import sys
from pathlib import Path
from typing import List, Tuple, Dict, Any

LINE_PATTERN = re.compile(
    r"^(?P<link>https?://t\.me/[\w\d_]+)/?\s*\|\s*"  # chat link
    r"(?P<members>[\d\s]+)\s*members\s*\|\s*"        # members
    r"(?P<online>[\d\s]+)\s*online\s*(?:\s*\([\d\.]+\%\))?$" # online (обновлено для обработки процента в скобках)
)


def _clean_int(raw: str) -> int:
    return int(raw.replace(" ", ""))


def parse_file(path: Path) -> List[Tuple[str, int, int, float]]:
    chats = []
    with path.open("r", encoding="utf-8") as f:
        for ln, line in enumerate(f, 1):
            line = line.strip()
            if not line:
                continue
            m = LINE_PATTERN.match(line)
            if not m:
                # ИЗМЕНЕНИЕ: Вместо того чтобы вызывать ошибку для строки,
                # которая не подходит под формат (например, с прочерком "-"),
                # мы просто пропускаем её и переходим к следующей.
                # Таким образом, такие строки полностью игнорируются.
                # print(f"Предупреждение: Пропущена строка с некорректным форматом в файле {path.name}, строка {ln}: {line!r}") # Раскомментируйте для отладки
                continue

            link = m.group("link")
            members = _clean_int(m.group("members"))
            online = _clean_int(m.group("online"))
            pct = online / members * 100 if members else 0.0
            chats.append((link, members, online, pct))
    return chats


def load_thresholds(config_path: Path) -> Dict[str, float]:
    defaults = {
        "percent": 10.0,
        "large_chat_percent": 5.0,
        "large_chat_members_threshold": 1000.0
    }

    if not config_path.exists():
        print("Файл ПРОЦЕНТ.json не найден. Запрашиваем значения.")
        try:
            defaults["percent"] = float(input("Введите минимальный процент онлайна для обычных чатов (например, 10): ").replace(",", "."))
            defaults["large_chat_percent"] = float(input("Введите минимальный процент онлайна для БОЛЬШИХ чатов (например, 5): ").replace(",", "."))
            defaults["large_chat_members_threshold"] = float(input("Введите порог участников для больших чатов (например, 1000): ").replace(",", "."))
            return defaults
        except ValueError:
            sys.exit("Ошибка: некорректное число.")
    try:
        data = json.loads(config_path.read_text(encoding="utf-8"))
        defaults["percent"] = float(data.get("percent", defaults["percent"]))
        defaults["large_chat_percent"] = float(data.get("large_chat_percent", defaults["large_chat_percent"]))
        defaults["large_chat_members_threshold"] = float(data.get("large_chat_members_threshold", defaults["large_chat_members_threshold"]))
        return defaults
    except Exception as e:
        sys.exit(f"Ошибка при чтении ПРОЦЕНТ.json: {e}")


def main():
    script_dir = Path(__file__).parent
    config_path = script_dir / "ПРОЦЕНТ.json"
    thresholds = load_thresholds(config_path)
    default_percent_threshold = thresholds["percent"]
    large_chat_percent_threshold = thresholds["large_chat_percent"]
    large_chat_members_threshold = thresholds["large_chat_members_threshold"]

    input_dir = script_dir / "НЕ отработанные"
    output_dir = script_dir / "УСПЕШНО"
    output_dir.mkdir(exist_ok=True)

    input_files = list(input_dir.glob("*.txt"))
    if not input_files:
        sys.exit("В папке 'НЕ отработанные' нет .txt файлов.")

    passed_lines = []
    all_failed_lines = []

    for file in input_files:
        try:
            chats = parse_file(file)
        except OSError as err:
            print(f"⚠️  Ошибка чтения файла {file.name}: {err}", file=sys.stderr)
            continue
        
        passed = []
        current_file_failed_lines = []

        for link, members, online, pct in chats:
            current_threshold = default_percent_threshold
            if members > large_chat_members_threshold:
                current_threshold = large_chat_percent_threshold

            line = f"{link} | {members:,} members | {online:,} online ({pct:.2f}%)"
            if pct >= current_threshold:
                passed.append(line)
            else:
                current_file_failed_lines.append(line)

        passed_lines.extend(passed)
        all_failed_lines.extend(current_file_failed_lines)

    if all_failed_lines:
        (output_dir / "не_прошли.txt").write_text("\n".join(all_failed_lines), encoding="utf-8")
        print(f"✅ Готово. Непрошедшие сохранены в: УСПЕШНО/не_прошли.txt")
    else:
        print("✅ Нет чатов, не прошедших фильтр.")

    if passed_lines:
        (output_dir / "прошли.txt").write_text("\n".join(passed_lines), encoding="utf-8")
        print(f"✅ Готово. Прошедшие сохранены в: УСПЕШНО/прошли.txt")
    else:
        print("❌ Нет чатов, прошедших фильтр.")


if __name__ == "__main__":
    main()