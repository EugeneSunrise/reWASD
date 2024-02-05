# Notice 06.01.2024
**6.02.2024 NEED SOME TESTER -> TELEGRAM: @sunrjze
- If there is any news I will update the readme immediately

# Wanna ask me something or You need help?
Use "Discussions tab"

# Current Status
- 7.0v x86 - Cracked features (not license), Arm - Full work!
- 6.6v x86 - Work via dnSpy

# Special Thanks
- Bless [Eddy](https://github.com/RedDot-3ND7355)❤️ ([reWASD](https://github.com/RedDot-3ND7355/reWASD))

# Guide - 7v x86/arm
x86 - only cracked features (not lic).
reWASD has are several versions, the version for Windows x86/x64 and Windows ARM Architecture.
Step by step for the littlest ones:
- First of all, delete your current version of reWasd program
- Go to [Releases](https://github.com/EugeneSunrise/reWASD/releases) and pick your version
- Do you see the Assets tab? Fine!
- Download installer in Assets for cracked version - reWASD700-8378(ARM).exe or reWASD700-8447.exe
- Install!
- Download reWASD.dll and reWASDCommon.dll(or reWASDEngine.dll for ARM) in Assets
- Disable reWASD (if enabled)
- Move reWASD.dll and reWASDCommon.dll( or reWASDEngine.dll) WITH REPLACEMENT in main reWasd folder
- Start reWASD, all features are available!

# Guide - 6.6v x86
YT blocks so i have uploaded to mega cloud
- Video Guide [Click](https://mega.nz/file/tTYGWZiB#LA9Sr4kOIswYFSf51fuN4wg88qOIQjaY_Y-B-4qNzM8)
- Text Guide:
1. Extract the files from the archive to the folder with reWASD
2. Change 4 paths in the file "1337_NI66A_PUSSYSLAYER_BREAKPOINT.xml". 
"C:\Program Files\reWASD\reWASDEngine.exe" to your path
3. Launch dnSpy.exe and drop reWASDEngine.exe into dnSpy
4. Add breapoint window in dnSpy ->Debug->Windows->Breakpoints
5. Import "1337_NI66A_PUSSYSLAYER_BREAKPOINT.xml" with special button in breapoint window
6. You should see 4 breakpoints, if there are less than 4 then you have done something wrong.
7. Launch reWASDEngine.exe(not reWASD.exe) only from dnSpy!!!!
8. Close reWASD only from tray windows and then close dnSpy

# Applying config and nothing happens? 6.6 ver.
FIRSTLY, CHECK VIDEO GUIDE AND DISCUSSIONS, TAKE CARE OF YOUR AND MY TIME❤️

Most likely, you have not imported breakpoints
- There may be two reasons why they are not imported:
1. You need to select the current assembly in dnSpy, that is, click on reWASDEngine in Assembly editor, I did it in the video at 2:15.
2. Change the paths to the file reWASDEngine.exe in the file with breakpoints (1337_pus...xml), there are only 4 lines where you need to set the correct path to the file, in the video it is 1:46 minute (also in the video there is only 1 path because today the fix came out and now there are 4)

# Everything was working for a few days and suddenly an error? (XamlReader.WrapException....)
Defender killed some reWASD files (that you moved from archive to folder), add reWASD folder or reWASD files to defender (or your antivirus) whitelist.
![Screenshot](https://github.com/EugeneSunrise/reWASD/assets/56397706/3a5da084-68e6-41a0-b477-b735840ed18b)

# Antivirus says there are trojans in the files? // Trojan:Win32/Znyonm
Quick answer - it's false positive, if you don't trust it so don't download it.
In the archive is dnSpy - a program for reversing .net applications, it is scolded by antivirus also the other files have no signature, so it can also be a red factor for antivirus. You can download dnSpy from the official repository on github (and configure it yourself). If you still think I'm trying to plant a trojan and upload hentai to your PC so just don't download the crack and don't bother me.

# Startup automation for dnSpy
Use autohotkey, make a simple file including lines below, and move it to startup folder, then it can be automaticly run when You start PC and placed in tray menu:
- Run, D:\reWASD1\dnSpy-net-win32\dnSpy.exe
- Sleep 3000
- Send {F5}
- Sleep 2000
- Send {Enter}
- Sleep 1000
- Winhide dnSpy
  
# Preview
![Screenshot 29-11-2023 at 22-15-35](https://github.com/EugeneSunrise/reWASD/assets/56397706/1d3e6290-73b2-4d19-a826-17667841aaed)
