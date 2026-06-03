#!/usr/bin/gnuplot

# Настройка интерактивного окна и масштаба
set terminal qt size 1200,1000
set size ratio -1

# Выносим легенду вправо за пределы графика, выравнивая справа сверху
set key outside right top

set key font "Arial,14"

set title "Сумма Минковского" font "Arial,16"
set xlabel "X"
set ylabel "Y"
set grid

# Отрисовка с использованием блоков данных и внешних переменных стилей
plot $DATA1 with lines title 'Слагаемое 1' linecolor rgb COLOR1 linewidth WIDTH1 dashtype STYLE1, \
     $DATA2 with lines title 'Слагаемое 2' linecolor rgb COLOR2 linewidth WIDTH2 dashtype STYLE2, \
     $DATA3 with lines title 'Сумма' linecolor rgb COLOR3 linewidth WIDTH3 dashtype STYLE3

