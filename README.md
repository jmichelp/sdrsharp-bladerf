sdrsharp-bladerf
================

bladeRF driver for SDR#


Installation
============

1. Copy the Release\SDRSharp.BladeRF.dll into SDR# installation directory
2. If required, copy all DLL files from LibBladeRF subdirectory to SDR# installation directory
3. Add the following line in the frontendPlugins sections of SDRSharp.exe.config file:

	&lt;add key="BladeRF" value="SDRSharp.BladeRF.BladeRFIO,SDRSharp.BladeRF" /&gt;


4. Launch SDR# and cross fingers :)


Compilation
===========

If you need/want to compile this DLL, you need to copy the following two DLL files
from SDR# installation directory to Release and Debug directories:

1. SDRSharp.Radio.dll
2. SDRSharp.Common.dll
