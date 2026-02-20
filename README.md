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
  * Speaking of which, created custom art for the opening, health gain and health loss situations.
  * also made respawning possible in a very scuffed way (just reloads the scene)
  * Created some lore for this world with the Game Over screen (when you lose all your health) - there are also versions of the sheet for Game Win and Game If-You-Don't-Win-Or-Lose (labelled GameMid under Sprites)
  * Created a custom backing track for the game! it loops and stuff woah
  * cape still clips, idk how to fix this.
  * i also think raycasts are still on for buttons in some instances, oops.
  * recorded a SLAY sound effect for the race mode start - didnt have time to implement oops
  * i love dotween THANK YOU FOR DOTWEEN DEV 2
  * i hope the stuff is useful! Have fun!! :3
  * I also also love flying

* Notes for Dev 7
  * MOST WORK WAS DONE IN LEVEL 1!!! Main Scene is behind
  * Added a dialogue system (Easy Cutscene). It's a bit weird to use but I added a .txt inside Assets/Prefabs/Dialogue explaining how to use it.
  * Added character prefabs to use in the dialogue system. Check out Walkie! I like how it turned out. You can use it as a base in case you want some characters that talk.
  * Added a Level 1 with the start of a story, would be cool to see some other levels with other winning conditions (To set winning conditions you can modify the LevelManager instance in each level). For that one is just one race and the end of the conversations. (I also thought that it would be cool to add targets to destroy jejeje)
  * Added a Game Win screen for the level. When clicking continue it takes you to the next numbered Level. (Game Manager is always saving the current level number)
  * Had some problems because I used the GameManager singleton for some fields but it doesn't like it when changing levels >< Sorry about that. I've been using the LevelManager as a dummy GameManager and just copy the necessary methods.
  * Implemented the unfinished audio files from Dev 5.
  * Added more SLAY sound effects for walkie talking.
  * Tried to fix the cape, I think it's a bit better??? Not sure how to make it perfect tho. I added the capsule colliders to the cloth back and reverted some lost colliders (Didnt see why they were removed so if it becomes crazy maybe delete "Chest" and "Spine 1" back)
