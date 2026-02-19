# TelephoneJam
Log
* Notes for Dev 2
  * Implemented a very basic character controller, feel free to change logic however.
  * You fly be pressing w and control the direction with mouse.
  * Created a character in blender with animations. The animations were imported from mixamo.
  * The character has a cape that uses cloth physics.
  * There are also walking and falling animations that are not currently being used since the character can only fly now.
 
* Notes for Dev 3
  * Implemented a Hit point health system where the points animate, spawn, and despawn using DoTween
  * You can Heal, Take Damage, and increase Max health using the following public functions from the Player Stat script (which is attached to the player). Heal = HealHealth(int amount), Damage = ReduceHealth(int amount), Set a new Max Health = SetMaxHealth(int newMaxHealth)
  * There are capsule which demonstrates how to use the function; those capsules are attached with the HealthModifierScript, where you can specify which kind of effect you want in the OnTrigger
  * Additional Modification tips: If you want to change the Health Points themselves, use the Health Unit prefab. If you want to change the spacing between health points, use the Player Stat script
 
* Notes for Dev 4
  * Implemented a new flight camera as well as flight mechanics, focusing on boosts for going down as well as for being near objects
  * Click and hold any mouse button to rotate the camera, double click to lock the mouse
  * I love flying

* Notes for Dev 5
  * Implemented a ragdoll system when the player dies (though, the camera dosent follow and there is no respawn lol)
  * Implemented Destructable objects
  * Upgraded the visuals of the health pickups in the level
  * All pickups (damage and health) now have a RaceID field, which makes them only visible when that specific "race" is active. SET TO -1 TO HAVE THEM BE GLOBAL AND NOT TIED TO A RACE
  * Implemented time trials via "ring races", feel free to look into my pre-built races to see how these work
  * Implemented something secret if you hold LMB
  * Built a prototype level for the game to take place in
  * Might have broken some of the cape physics? but I cant tell if that is me or they just clip
  * Recorded some SLAY audio effects for the secret stuff related to LMB
  * Have fun!!
  * I also love flying

* Notes for Dev 6
  * Added a title screen with a button and an opening stinger! Custom art for the button - there are versions for unselected, hover, press, and selected. Use sprite swap for button implementations!
  * also added sfx for the button 'cracking' although i did not have time to implement it! Find this under Audio/SFX
  * Speaking of which, created custom art for the opening, health gain and health loss situations LOL
  * also made respawning possible in a very scuffed way (just reloads the scene)
  * Created some lore for this world with the death screen - there are also versions of the sheet for Game Win and Game If-You-Don't-Win-Or-Lose (labelled GameMid under Sprites)
  * Created a custom backing track for the game! it loops and stuff woah
  * i love dotween THANK YOU FOR DOTWEEN DEV 3
  * I also also love flying
