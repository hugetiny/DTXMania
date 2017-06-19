============================================================
  DTXMania .NET style
  (C) 2000 2017 DTXMania Group
============================================================

* Requirements

(1) OS ...  Windows 7 (x86, x64) / 8 (x86, x64) / 10 (x86, x64)
(2) .NET Framework ... Version 4.5.1
   (You'll need to install .NET Framework 4.5.1 additionaly on Win8.1 or before)
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

Old DTXMania support only DirectSound. It's very "laggy" sound
system especially Windows Vista and above.

Now, DTXMania support both WASAPI and ASIO on Release 096.
(Of course DirectSound is still supported)
To use WASAPI or ASIO, you can reduce the lag from hitting pads
to output the sound.

If you use Vista or later, DTXMania initially uses WASAPI.
(If you use XP, DTXMania initially uses DirectSound.)
If you want to use ASIO, you have to change CONFIGURATION-
System-SoundType to "ASIO".


If you specify "WASAPI" but your system can't use it,
DTXMania automatically try to use "ASIO".
And if "ASIO" is not used on your system, "DirectSound" is used.

** IMPORTANT NOTICE *******************************************
If you use WASAPI or ASIO, DTXMania uses different timer method
what DTXMania used on DirectSound.
It may cause "the sync issue" between BGM and song score.
(Especially you'll see the out of sync if the DTX data author
 made it by Win7+DTXCreator(DTXViewer), I believe.)

Though I managed to solve the issue, but still I can't find the way
to keep the timer compatibility.
You may have to edit the song data to keep sync again.
****************************************************************

Selected sound-type(WASAPI/ASIO/DirectSound) and sound
buffer size (= lag time) are shown on the DTXMania window title bar.
It's very helpful for you to try configuring DTXMania.
So you should use window mode during your sound configuring
on DTXMania.

Though you can reduce lags by using WASAPI/ASIO,
but, it needs your self-configuring.
Please check the notice below to configure WASAPI/ASIO.


* Notice (WASAPI)
WASAPI can use on Windows Vista and above.
You can't use it on XP.

* Notice (ASIO)
ASIO can use on Windows XP.
However, the sound device must support ASIO.

You must specify the buffer size (latancy).
You can specify it by the sound device.
(If you don't have ASIO setting tools,
 you can use "ASIO caps" (freesoft) etc)

If DTXMania fails to use ASIO device
(by nonproper buffer size, etc),
DTXMania uses DirectSound.

You also have to specify ASIODevice. It specifies
what sound device is used by DTXMania.
(If you choose WASAPI or DirectSound, DTXMania uses
 OS-default sound device)
If you specify non-existing sound device, DTXMania
may not start.
