#!/usr/bin/gnuplot

# Настройка интерактивного окна и масштаба
set terminal qt size 2400,2000
set size ratio -1

# Выносим легенду вправо за пределы графика, выравнивая справа сверху
set key outside right top

# Подписи в легенде
cmd = sprintf('set key font "Arial,%d"', FONT_SIZE)
eval(cmd)

# Заголовок
cmd = sprintf('set title "Сумма Минковского" font "Arial,%d"', FONT_SIZE)
eval(cmd)

# Подписи
set xlabel "X"
set ylabel "Y"
set grid lw 2
set xtics offset 0,-1
set ytics offset -1,0
TICK_SIZE = FONT_SIZE - 4
cmd = sprintf('set tics font "Arial,%d"', TICK_SIZE)
eval(cmd)

bind "r" "replot"

# Отрисовка с использованием блоков данных и внешних переменных стилей
plot $DATA1 with lines title 'Слагаемое 1' linecolor rgb COLOR1 linewidth WIDTH1 dashtype STYLE1, \
     $DATA2 with lines title 'Слагаемое 2' linecolor rgb COLOR2 linewidth WIDTH2 dashtype STYLE2, \
     $DATA3 with lines title 'Сумма' linecolor rgb COLOR3 linewidth WIDTH3 dashtype STYLE3

