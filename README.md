# Keylogger 
Usage: .\Keylogger [arguments]

-r Try assign registry autorun values. Will try LocalMachine, if fails, then tries CurrentUser.
-f Writes keyboard input to a file located at executing location. File = keys.txt
-c Writes keyboard input to console.
-n Writes keyboard input over UDP network. Usage: -n [IP address] [port]