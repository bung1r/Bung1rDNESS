# Bung1r DNESS
## made by Bung1r
DNESS stands for Debugging NES (Nintentdo Entertainment System) Script 

This is a domain-specific language written in C# which works with bung1r's specific NES Emulator, with a built in Lexer, Parser, and Interpreter, as well as boasting capabilities such as debugging and automating NES tasks with the use of 30+ commands. 

### PROGRESS REPORT: 

**July 5th 2026** <br>
- Did work on this project a little bit before making this log, but forgot to push to Github during that time. 
- Completed a full Lexer, Parser, and Interpreter system, with functionality 
for if statements and function calls (although no functionality with the actual
emulator as of yet)
- Goals for next time: Complete mathematical operations (+, -, /, *), and add 
other statements like while loops

**July 6th 2026** <br>
- Completed basic mathematical operations (+, -, /, and *) and made it compatable with variables (setting/creating) + if statements 
- Completed implementation of a while loop
- Completed system for variable creation and setting
- Modified some function calls
- Goals for next time: Add simple implementation with Bung1rNESEmulator (ex: cpu.a, cpu.y, cpu.x), as well as working write(), read(), and load()

**July 8th 2026** <br>
- Added a decently large about of implementation with Bung1rNESEmulator, including the things stated as "Goals for next time"
- Other methods that were added include step(), pause(), cpuwrite(), ppuwrite(), wait(), and quite a bit more.
- Added the ability to comment using two hashtags surrounding any block of text
- Made return-able functions
- Goals for next time: Add more implementation with the PPU, APU, and other systems. Add more useful methods and fix timing issues (pause will pause at different clock counts because of the speed of the nes)

**July 10th 2026** <br>
- Added a couple more methods.
- Made an input recording system (TAS)
- Added a file that beats 1-1 using my inputs (mario1-1.dness).
- Edited script.dness to be more representative of the functions this language can perform.
- Goals for next time: To be determined. This project is practically finished, and so I will retire it until I decide to work on it again.
  
### Instruction Set:<br><br>

load([ROM_Path])<br>
- Loads a certain cartridge into the emulator.<br><br>

run() <br>
- Runs the game. load() must be used in order to load the cartridge. Not the same as play() <br><br>

loadrun([ROM_Path])<br>
- Loads, then runs a certain cartridge into the emulator. Just the load and run methods in a single line<br><br>


pause() <br>
- Pauses the game <br><br>

play() <br>
- Unpauses the game <br><br>

unload() <br>
- Unloads an existing cartridge from the emulator.<br><br>

close() <br>
- Closes out of the game instance, but does not take the cartridge out.<br><br>

exit() <br>
- Exits completely out of the Windows Form display menu<br><br>

write([cpu/ppu] [address] [data]) OR write [cpu._/ppu._]<br>
- Writes a byte to a certain memory address or register as specified <br><br>

cpuwrite([addr] [data])
- Write a byte to a certain memory address as if it was written from the CPU. <br><br>

ppuwrite([addr] [data])
- Write a byte to a certain memory address as if it was written from the PPU. <br><br>

step([x])
- Steps forward x amount of clock cycles. In this emulator, 1 step = 1 ppu clock, 2 step = 1 cpu clock<br><br>

stepuntil([goal]) <br>
- Steps until the Clock Counter is equal to goal <br><br>

stepuntilasync([goal]) <br>
- Same as above, but just async

read [address]<br>
- Reads a byte from a certain memory address<br><br>

cpuread [address] [bRead]
- Reads a byte from a certain memory address as sif it was read from the CPU.<br><br>

ppuread [address] [bRead] <br>
- Reads a byte from a certain memory address as sif it was read from the PPU.<br><br>

write([location], [data]) <br>
- Writes a byte to a certain location
- Valid 'locations' include: cpu.a, cpu.x, cpu.y, etc.
- For writes to CPU or PPU memory, use cpuwrite or ppuwrite.<br><br>

cpuwrite([address], [data]) <br>
- Writes a byte to a certain address as if writing from the CPU.<br><br>

ppuwrite([address], [data]) <br>
- Writes a byte to a certain address as if writing from the PPU.<br><br>

press([button], [duration_ms])<br>
- Will press a button for a specified duration (in milliseconds). <br><br>

hold([button], [holdtime]) <br>
- Holds a button down, starting at 'holdtime' clock counter.

release([button], [releasetime]) <br>
- Releases a button, starting at 'releasetime' clock counter.<br><br>

record([true/false]) <br>
- Records input in a log.txt file, which can be copy and pasted. 
- Writes a combination of only 'hold' and 'release' methods. <br><br>

save([file_path])<br>
- Saves the game progress to the file path specified. Will create a new file if the file does not exist.<br><br>

wait([ms])<br>
- Delays the current thread for a specified amount of time (in milliseconds)<br><br>

wait([waitsteps]) <br>
- Delays the current thread for a specified amount of steps
- Not 100% accurate, since it utilizes polling (but accurate enough)
- If 100% accuracy is required, pause and use step([numsteps]) instead. <br><br>

waituntil([goal])
- Delays the current thread until clock counter/total num of steps = goal
- Utilizes polling. 
- If 100% accuracy is required, pause and use stepuntil([goal]) instead. <br><br>

var([var_name], [initial_value]) <br>
- Creates a new variable set to a specific value. <br><br>

set([var_name], [new_value]) <br>
- Sets a specific variable to a certain value. 
This value does not have to match the initial data type of the variable.<br><br>

print([content]) <br>
- Prints a line to the terminal without appending a newline character.<br><br>

println([content]) <br>
- Prints a line to the terminal with appending a newline character.<br><br>

Note: There's a chance I missed some, but here are all the important ones at least. 



