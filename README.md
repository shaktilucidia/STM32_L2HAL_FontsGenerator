# STM32_L2HAL_FontsGenerator
Fonts generator to use with STM32 L2HAL

1) Extract font using Font Forge and given script:

import os
from fontforge import *

font = open("/path/to/font/FreeSans.ttf")
for glyph in font:
    if font[glyph].isWorthOutputting():
        font[glyph].export("/path/to/extracted/font/FreeSansExtracted/" + font[glyph].glyphname + ".png", 511)

2) Resize extracted glyphs to desired height in pixels:

mogrify -resize 32x ./*.png

3) Convert glyphs to xbm format:

mogrify -format xbm ./*.png


4) Set correct path in FontGenerator's Program.cs, compile and run
