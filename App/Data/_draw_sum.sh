#!/bin/bash

TEMPLATE_GP="_draw_sum.gp"

# Дефолтные значения стилей и размера шрифта (сделали крупнее по умолчанию)
FONT_SIZE="14"

FILE1=""
COLOR1="red";   WIDTH1="2";   STYLE1="1"

FILE2=""
COLOR2="blue";  WIDTH2="2";   STYLE2="1"

FILE3=""
COLOR3="green"; WIDTH3="4";   STYLE3="1"

show_help() {
    echo "Использование: $0 [опции]"
    echo "Опции общего вида:"
    echo "  -fs  Размер шрифта для всех подписей (по умолчанию: 14)"
    echo "Опции для каждого многоугольника (N = 1, 2, 3):"
    echo "  -fN  Путь к файлу данных (обязательно)"
    echo "  -cN  Цвет линии"
    echo "  -wN  Толщина линии"
    echo "  -sN  Стиль линии"
    exit 1
}

if [ "$#" -eq 0 ]; then show_help; fi

# Парсинг аргументов
while [[ "$#" -gt 0 ]]; do
    case "$1" in
        -fs) FONT_SIZE="$2"; shift 2 ;;
        
        -f1) FILE1="$2"; shift 2 ;;
        -c1) COLOR1="$2"; shift 2 ;;
        -w1) WIDTH1="$2"; shift 2 ;;
        -s1) STYLE1="$2"; shift 2 ;;
        
        -f2) FILE2="$2"; shift 2 ;;
        -c2) COLOR2="$2"; shift 2 ;;
        -w2) WIDTH2="$2"; shift 2 ;;
        -s2) STYLE2="$2"; shift 2 ;;
        
        -f3) FILE3="$2"; shift 2 ;;
        -c3) COLOR3="$2"; shift 2 ;;
        -w3) WIDTH3="$2"; shift 2 ;;
        -s3) STYLE3="$2"; shift 2 ;;
        
        -h|--help) show_help ;;
        *) echo "Неизвестный параметр: $1"; show_help ;;
    esac
done

if [ -z "$FILE1" ] || [ -z "$FILE2" ] || [ -z "$FILE3" ]; then
    echo "Ошибка: Укажите файлы для всех трех многоугольников (-f1, -f2, -f3)."
    exit 1
fi

for file in "$FILE1" "$FILE2" "$FILE3"; do
    if [ ! -f "$file" ]; then echo "Ошибка: Файл '$file' не найден."; exit 1; fi
done

if [ ! -f "$TEMPLATE_GP" ]; then
    echo "Ошибка: Шаблон GNUPlot с именем '$TEMPLATE_GP' не найден."
    exit 1
fi

# Функция автоматического замыкания контуров
close_polygon() {
    local input_file="$1"
    local first_point=""
    while IFS= read -r line || [ -n "$line" ]; do
        if [[ "$line" =~ ^[[:space:]]*# ]]; then echo "$line"; continue; fi
        if [[ -z "${line//[[:space:]]/}" ]]; then
            if [ -n "$first_point" ]; then echo "$first_point"; first_point=""; fi
            echo ""
        else
            if [ -z "$first_point" ]; then first_point="$line"; fi
            echo "$line"
        fi
    done < "$input_file"
    if [ -n "$first_point" ]; then echo "$first_point"; fi
}

# Временный файл данных
TEMP_DATA=$(mktemp /tmp/gnuplot_data.XXXXXX.gp)

echo "\$DATA1 << EOD" > "$TEMP_DATA"
close_polygon "$FILE1" >> "$TEMP_DATA"
echo "EOD" >> "$TEMP_DATA"

echo "\$DATA2 << EOD" >> "$TEMP_DATA"
close_polygon "$FILE2" >> "$TEMP_DATA"
echo "EOD" >> "$TEMP_DATA"

echo "\$DATA3 << EOD" >> "$TEMP_DATA"
close_polygon "$FILE3" >> "$TEMP_DATA"
echo "EOD" >> "$TEMP_DATA"

# Передаем FONT_SIZE в блок -e вместе со стилями
gnuplot -e "FONT_SIZE=$FONT_SIZE; \
            COLOR1='$COLOR1'; WIDTH1=$WIDTH1; STYLE1=$STYLE1; \
            COLOR2='$COLOR2'; WIDTH2=$WIDTH2; STYLE2=$STYLE2; \
            COLOR3='$COLOR3'; WIDTH3=$WIDTH3; STYLE3=$STYLE3" \
        "$TEMP_DATA" "$TEMPLATE_GP" -persist

rm -f "$TEMP_DATA"
