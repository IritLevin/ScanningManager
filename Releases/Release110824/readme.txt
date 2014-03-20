ScanningManager controls an array of scanners for time lapsed serial scanning.
Copyright 2010 Irit Levin Reisman published under GPLv3,
this software was developed in Prof. Nathalie Q. Balaban's lab, at the Hebrew University , Jerusalem , Israel .


Contents:
---------
AduHid.dll
EmailSMSSender.dll
EnvRomControler.dll
Interop.WIA.dll
wiaaut.dll
devcon.exe
ScanningManager.exe
ScanningManager.exe.config


Intallation:
------------
Copy all files into a designated directory.
Make sure Microsoft .NET Framework Version 2.0 is installed 
(http://www.microsoft.com/downloads/details.aspx?familyid=0856eacb-4362-4b0d-8edd-aab15c5e04f5&displaylang=en)

if there is an error activating the application, register the wiaaut dll: 
write in the command prompt:
regsvr32  "FULL PATH\wiaaut.dll"
to un-register, write:
regsvr32 -u "FULL PATH\wiaaut.dll"


Configuration:
--------------
In ScanningManager.exe.config:
Change the parameters in "Scanners and Relay parameters" and in 
"Email and SMS sender using standard SMTP" sections according to the comments.
For scanner name look at Computer Managment->Device Manager->Imaging Devices

Activation:
-----------
Run ScanningManager.exe


Software documentation:
-----------------------
See Documantation.html


Contact Details:
----------------
Prof. Nathalie Q. Balaban: nathalieqb@phys.huji.ac.il
