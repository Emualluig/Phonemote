# Phonemote
 Control desktop applications from your phone


Phonemote is currently in very early development.

Phonemote currently works only with PowerPoint in a limit capacity.

There are two parts of Phonemote. 
 - PhonemoteDesktop which interops with many different desktop programs (Backend in C#, frontend in HTML+JS+CSS).
 - PhonemoteMobile (React App) allows users to control desktop program via a smartphone.

### How it works

PhonemoteDesktop detects if PowerPoint is installed and obtains a list of all open presentations.

PhonemoteDesktop then creates a server and displays on the GUI a QR cpde for PhonemoteMobile to scan.

PhonemoteMobile scans the QR code and this connects Mobile to Desktop.

PhonemoteMobile can now control the PowerPoint presentation.

### Short Term TODO
 - [ ] More Stable Connection
 - [ ] More Stable PowerPoint Interop (It sometimes crashes on opening new presentations and other events)
 - [ ] More PowerPoint Controls on PhonemoteMobile (currently it only has 'next slide')
 - [ ] Better communication of commands between Mobile and Desktop
 - [ ] Ability to select between multiple PowerPoints
 - [ ] Better GUIs
 - [ ] Documentation and Comments

### Long Term TODO
 - [ ] Display Available Interop on PhonemoteDesktop
 - [ ] More Interops
 - [ ] Better GUIs
 - [ ] Documentation and Comments

### Requirements:

Only works on Windows

Requires Assemblies (office, powerpoint, etc.) for certain interops to work.
Currently requires:
 - Powerpoint Interop:
   - Interop.Microsoft.Office.Interop.PowerPoint
   - office.dll (Found in GAC_MSIL)

