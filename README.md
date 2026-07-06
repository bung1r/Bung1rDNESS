# Bung1r DNESS
## made by Bung1r
DNESS stands for Debugging NES (Nintentdo Entertainment System) Script 

This is a domain-specific language written in C# which works with bung1r's specific NES Emulator, boasting capabilities such as debugging and automating NES tasks. 

### PROGRESS REPORT: 

**July 5th 2026** <br>
- Did work on this project a little bit before making this log, but forgot to push to Github during that time. 
- Completed a full Lexer, Parser, and Interpreter system, with functionality 
for if statements and function calls (although no functionality with the actual
emulator as of yet)
- Goals for next time: Complete mathematical operations (+, -, /, *), and add 
other statements like while loops

### Instruction Set:<br><br>

load [ROM file path]<br>
- Loads a certain cartridge into the emulator.<br><br>

unload<br>
- Unloads an existing cartridge from the emulator.<br><br>

write [address] [data]<br>
- Writes a byte to a certain memory address.<br><br>

read [address]<br>
- Reads a byte from a certain memory address<br>
- If an address range or a list of addresses is given, the return value will be the value of those addresses in the order specified. <br><br>

dump [file_path]<br>
- Dumps a bunch of info into a specified file path. Idk what this will do yet. Will create a new file if the file does not exist.  <br><br>

press([button], [duration_ms])<br>
- Will press a button for a specified duration (in milliseconds). <br><br>

save([file_path])<br>
- Saves the game progress to the file path specified. Will create a new file if the file does not exist.<br><br>

wait([ms])<br>
- Delays the current thread for a specified amount of time (in milliseconds)<br><br>

speed([speed_mult]) <br>
- Sets the game speed to a certain value <br><br>







