## Project in the root folder:
I have put the project int the root folder since the assignment asks that build commands can be run inside the clone of the project. dotnet run fails in a folder that only holds a .sln.

## Global interactivity:
I set the project up with global interactivity since every page needs a live connection anyway, and forgetting to add @rendermode interactiveServer on a page can cause a massive headache and unnecessary troubleshooting

## Profile class:
The profile class stores displayname, id, note, avatarcolor, year and role. Instead of having a list of threads the user has joined i decided to have a membership record that stores the userid and later on the threadid. The role enum has student, lecturer and auditor as roles.

## Chat Thread class:
the chat thread class takes id, name, created at date time, owner and a boolean to see if the thread is private. The passcode is stored in a dictionary on the server where and is linked with its thread id. a page only sends a code and recives a correct or wrong answer from the server.