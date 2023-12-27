# CustomFilter
The best [OpenTabletDriver](https://github.com/OpentabletDriver/OpenTabletDriver) filter to ever grace this planet!

[![Total Download Count](https://img.shields.io/github/downloads/adryzz/CustomFilter/total.svg)](https://github.com/adryzz/CustomFilter/releases)

Allows you to use any mathematical expression that can be evaluated to a number as a filtering stage!

Uses [AngouriMath](https://github.com/asc-community/AngouriMath) to automatically compile your expressions into code at runtime, to achieve the best performance possible.

All the math is done on [complex numbers](https://en.wikipedia.org/wiki/Complex_number), but the end result is just the [real](https://docs.microsoft.com/en-us/dotnet/api/system.numerics.complex.real?view=net-6.0) part of it

## Simple mode

![image](https://user-images.githubusercontent.com/46694241/169646212-e162bbdf-99c4-428b-97a0-283034d05fed.png)

The Simple Mode is the fastest but is limited in the number of samples you can use.
Here's the supported parameters for both expressions:

`x` = The X coordinate

`y` = The Y coordinate

`p` = The pressure

`tx` = The tilt X component

`ty` = The tilt Y component

`d` = The hover distance

`lx` = The last X coordinate

`ly` = The last Y coordinate

`lp` = The last pressure

`ltx` = The last tilt X component

`lty` = The last tilt Y component

`ld` = The last hover distance

`mx` = Max X coordinate

`my` = Max Y coordinate

`mp` = Max pressure

`cx` = Last computed X coordinate

`cy` = Last computed Y coordinate

`cp` = Last computed pressure

#### Example: EMA smoothing
![image](https://user-images.githubusercontent.com/46694241/152674407-eaccdf71-6fb2-448a-9eb4-6bc1c820bac0.png)
