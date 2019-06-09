AR Defense
===========================
Copyright 2019 Mat Valadez

AR Defense is a simple proof-of-concept prototype for an Augmented Reality Tower defense style game.
All art was found for free in the Unity Asset store or online.


## How to Play

Point the camera at the ground and move around slowly until the game recognizes the ground as a plane.
Then, move the reticule around to "grow" the ground plane, when the reticule is green, press the confirm button
to start the game. For best results, pay outdoors with as large an area as is comfortable to move about.

Your tower will be placed in the center of the screen. Enemy Droships will appear on the edges of the ground and spawn
enemy vehicles to attack you tower. Use the weapons you have to defend your tower from the invaders.

Currently, Only Android is supported.


## Code Notes
Dependency Injection is used throughout, see the Interfaces folder Injectable objects, notable examples listed below

Interfaces are used whenever possible. Non-specific implementations are prefixed with Simple___
All the Manager obejcts are injected using the IoC Container.
The enemy movement behaviors can be found in the Patterns folder.
The Modules folder contains Components meant to be drag and drop. They mostly respond to Broadcast on significant events (see below.)

### GameManager
Handles most of the game start up and general flow. Contains references to the major AR Components like ARSessionOrigin.
### PlaneSelector
Used to find a suitable plane to act as the world's ground, to which all other objects are anchored.
### DisplayManager
The UI is mostly expressed through "Displays". Full-screen UI pages that have a reticule with a reload meter
and any buttons that will be used. Displays are pushed and popped depending on which component is in control
and what the player needs to do.
### IFFTracker
Shows the approximate direction of objects in the game, notable the players base, and enemies.
### Weapon (ScriptableObject)
Contains the configuration and projectile prefab for a weapon.
### Cannon
Used to fire a weapon
### SimpleKillable
Any object that can give or receive damage must have an IKillable imlementation. SimpleKillable is a rudimentary implementation
of IKillable that handles taking damage and destroying the GameObject when the health runs out.
### SimpleProjectile
A projectile that fires in a straight line and is automatically destroyed after a period of time.
### ShowHealth
This Module dsplay a health meter above the object on the given trigger (usually when in the current Display's Reticule).
### PrefabOnKill
Instantiates a prefab when the IKillable is killed. Use this to create an explosion, debris, etc.


## Libraries Used
### PicnicIOC
A simple IoC library based on [MinIOC](https://github.com/microsoft/MinIoC).
### TextMeshPro
### AOFLPromises
[AofL Promises](https://github.com/AgeOfLearning/promises)
