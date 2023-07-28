# Phonemote
 Control desktop applications from your phone.

Can control PowerPoint presentation from your phone. Bring your presentations to the next level without buying any expensive gear!

### PhonemoteDesktop vs PhonemoteMobile

There are two parts of Phonemote. 
 - PhonemoteDesktop which interops with many different desktop programs (Backend in C#, frontend in HTML+JS+CSS).
 - PhonemoteMobile (React App) allows users to control desktop program via a smartphone.

### How it works

PhonemoteDesktop detects if PowerPoint is installed and obtains a list of all open presentations.

PhonemoteDesktop then creates a server and displays on the GUI as QR code for PhonemoteMobile to scan.

PhonemoteMobile scans the QR code and this connects Mobile to Desktop.

PhonemoteMobile can now control the PowerPoint presentation.

### Demos:

Coming soon!

### Requirements:

Only works on Windows

Requires Assemblies (office, powerpoint, etc.) for certain interops to work.
Currently requires:
 - Powerpoint Interop:
   - Interop.Microsoft.Office.Interop.PowerPoint
   - office.dll (Found in GAC_MSIL)

