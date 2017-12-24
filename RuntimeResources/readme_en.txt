============================================================
  DTXMania .NET style
  (C) 2000 2018 DTXMania Group
============================================================

* Requirements

(1) OS ...  Windows 7 (x86, x64) / 8 (x86, x64) / 10 (x86, x64)
(2) .NET Framework ... Version 4.7
   (You'll need to install .NET Framework 4.7 additionaly on Win10 Creaturs Update or before)
    https://support.microsoft.com/en-us/help/3186497/the-net-framework-4-7-offline-installer-for-windows
(3) DirectX End User Runtime ... June 2010 Version or later
   (You'll need to install DirextX 9.0c additionaly on Win8 or later.)
(4) Microsoft Visual C++ 2013 Redistributable Package (x86)

If you don't install any libraries descrived above,
you'll fail to start DTXMania.


* Installing DirectX End User Runtime

About the Requiremtnts (3), this zip contains
"minimum" runtime components in "DirectX Redist" folder.
Please run DXSETUP.exe to install DirectX 9.0c (June/2010).

Especially for Win10, you cannot use web setup for DirectX.
Please use DXSETUP.exe in DirectX Redist folder.


* Installing DTXMania

You don't have to install DTXMania.
You simply put all files to any floder you want to.


* Uninstalling DTXMania

Delete all files in the DTXMania folder.
(DTXMania doesn't use registry.)


* Installing song data for DTXMania

This zip file doesn't contain any song data (DTX, GDA etc).
So, for the first time, you can't see any song list in DTXMania.
You have to employ every available means to get them :-)

If you have some song data, to install them to DTXMania,
please make subfolder in DTXManiaGR.exe's folder and
put song data into the folder.
(DTXMania doesn't check folder name. Simply you have to
 make the folder at the same place where DTXManiaGR.exe is.)

DTXMania searches all folders (includes all subfolders)
specified in "strSongDataPath" section of Config.xml.
Config.xml is automatically made after DTXMania's initial boot.

DTXMania doesn't check the depth of subfolders.
DTXMania seek all folders to the bottom of the subfolders.


[Notice]
In the initial DTXMania settings (in Config.xml files),
the folder where DTXManiaGR.exe exists
is specified as the song data folder (strSongDataPath).

And DTXMania record each your playing result
as the file ("score.ini" file).
That result file is saved to the same folder of the song data.
So you have to set write permission to the song data folder.
If the folder doesn't have the permission, DTXMania may put
some error. The result may not be recorded.

It would become a problem if your windows account doesn't have
administraor permission. You have to choose the song data folder
carefully.


* WASAPI / ASIO support

DTXMania supports WASAPI(Exclusive/Shared) and ASIO.
For Win10, low-latency WASAPI-shared mode is also supported.

To use WASAPI or ASIO, you can reduce the lag from hitting pads
to output the sound.

If you use Vista or later, DTXMania initially try to uses WASAPI-Exclusive.
(If you use Win10, DTXMania initially uses WASAPI-Shared by default.
 If you use XP, DTXMania initially uses DSound(DirectSound).)
If you want to use ASIO, you have to change CONFIGURATION-
System-Sound Option-SoundType to "ASIO".

If you specify "ASIO" but your system can't use it,
DTXMania automatically try to use "WASAPI-Exclusive".
In the same way, in case system can't use "WASAPI-Exclusive",
"WASAPI-Shared" is used. "WASAPI-Shared" can't, "DSound" is used.


Selected sound-type(ASIO/WASAPI-Exclusive/WASAPI-Shared/DirectSound) and sound
buffer size (= lag time) are shown on the DTXMania window title bar.
It's very helpful for you to try configuring DTXMania.
So you should use window mode during your sound configuring
on DTXMania.

Though you can reduce lags by using WASAPI/ASIO,
but, it needs your self-configuring.
Please check the notice below to configure WASAPI/ASIO.


* Notice (WASAPI)
If you set WASAPI-Exclusive but become WASAPI-Shared,
please try following;
1. set WASAPI-Exclusive
2. then reboot DTXMania

* Notice (ASIO)
To use ASIO, your sound device must support ASIO.
(if you don't have it, you may try "ASIO2ALL")

You must specify the buffer size (latancy).
You can specify it by the sound device.
(If you don't have ASIO setting tools,
 you can use "ASIO caps" (freesoft) etc)

If DTXMania fails to use ASIO device
(by nonproper buffer size, etc),
DTXMania uses WASAPI (Exclusive/Shared).

You also have to specify CONFIGURATION/System/Sound Options/ASIODevice.
It specifies what sound device is used by DTXMania.
(If you choose WASAPI or DirectSound, DTXMania uses
 OS-default sound device)
If you specify non-existing sound device, DTXMania
may not start.
