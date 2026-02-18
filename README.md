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
