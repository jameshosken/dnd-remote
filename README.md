# dnd-remote
 
An attempt to make DnD easier to play remotely in these socially-isolated times. 

## Overview

The goal here is to have a DM running the game with control over all objects, and several players with limited control over their own character. 

## 3rd party tools
These tools have been .gitignored, meaning you will have to install them yourself. 

For the node.js packages, cd to the /server/ directory and `npm install`, which will automatically install the packages you need. 

For the Unity assets, you'll need to add them individually. I have added my 3rd party packages into a folder in Assets/ called "3RDPARTY", which .gitignore will ignore. You don't have to, but make sure the .gitignore file conatins a reference to any 3rd party apps you use.

- node: server/app.js runs on node.js
    - express
    - socketio
- SocketIO for Unity
    - make sure to download the (free) asset SocketIO for Unity and include it in your project. Project will break without this. 


## Goals/Roadmap

This is a very much living document and any input is welcome!

### Immediate Goals
1. Fog of War/hidden objects
2. DM Multi-select, multi-move, and mult-object interaction
3. Generic Set Decor 
4. Enhanced map creation tools

### Future Goals
1. Stats integration (HP, AC)
2. VFX for spellcasting/attacks/deaths

### Reach Goals
1. AR/mobile Player interface


## Get Involved
For ideas/feature requests please submit an issue on GitHub. Feel free to fork the repo and start playing around with the code. If you have changes you'd like to make, go ahead and submit a pull request! To help keep track of stuff, please make sure there's an issue for each pull request you make. Think of the 'issue' as the problem/concept/feature your pull request addresses.



