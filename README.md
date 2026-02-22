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
  * MOST WORK WAS DONE IN LEVEL 1!!! Main Scene is behind, Level 2 is an exact copy of Level 1
  * Added a dialogue system (Easy Cutscene). It's a bit weird to use but I added a .txt inside Assets/Prefabs/Dialogue explaining how to use it.
  * Added a way to pause the Character Controls. It is in both GameManager and LevelManager. If the player doesnt move for some reason its probably an issue with it, check the references or the fields in buttons... Sorry about that!
  * Added character prefabs to use in the dialogue system. Check out Walkie! I like how it turned out. You can use it as a base in case you want some characters that talk.
  * Added a Level 1 with the start of a story, would be cool to see some other levels with other winning conditions (To set winning conditions you can modify the LevelManager instance in each level). For that one is just one race and the end of the conversations. (I also thought that it would be cool to add targets to destroy jejeje)
  * Added a Game Win screen for the level. When clicking continue it takes you to the next numbered Level. (Game Manager is always saving the current level number)
  * Had some problems because I used the GameManager singleton for some fields but it doesn't like it when changing levels >< My bad! I've been using the LevelManager as a dummy GameManager and just copy the necessary methods.
  * Found a bug when changing levels where the RacingManager doesnt want the Races to appear in the map, couldnt figure out how to fix it, hopefully someone can! (Worst case scenario I think removing the singleton from it might make it work for each scene?)
  * Implemented the unfinished audio files from Dev 5.
  * Added more SLAY sound effects for walkie talking.
  * Tried to fix the cape, I think it's a bit better??? Not sure how to make it perfect tho. I added the capsule colliders to the cloth back and reverted some lost colliders (Didnt see why they were removed so if it becomes crazy maybe delete "Chest" and "Spine 1" back)
  * Dotween is so cool???? tweening is the thing I hate the most about programming and it worked prettty welllll.
  * Hopefully my work and the comments are helpfullllllll! I hope you enjoyyyy!<3
  * I also also also love flying

##### Notes for Dev 8

AHOY, Dev 8. Ye find yerself asea, adrift 'pon ye olde Sargasso! Judgin' by this here message ye might be thinkin' that this here game be a pirating adventure... AND YOU'D BE RIGHT! HAR HAR!... right at being WRONG, HAR! Ye scurvy cur! Perhaps ye best be rethinkin' those assumptions ye be making based on the inflection of ye olde messenger! Course, I best be rethinkin' me olde assumptions about yer assumptions... perhaps ye landlubbin' butt thought the game was but a classic flight simulator? well... YOU'D BE RIGHT! HAR HAR HAR!... kinda. Ye see, I spent me efforts in two, TWO key places: FLYIN' and DESTROYIN'!!!! YAR HAR HAR~!
  * the flyin' feels GOOD now, AND IT'S ALL PARAMETERIZED, HAR HAR HAR!!!
    * the Player prefab has two children objects, "Camera Space" and "PlayerModel"
      * Camera Space has the Camera as a wee grandchild, to which is assigned a MainCamera script with a bunch o parameterrrrrs. Everything starting from "Neutral Return Speed" down affect how the camera responds to player movement. BE YE WARNED: behaviour be different if the player is moving forward or NOT moving forwarrrrrd.
      * PlayerModel has assigned to it the PlayerController scrrrrrrript! It now has a whack a parameterrrs which control the player model's movement when flyin through the airrrrr. The model now dips, and banks, and turnrrrrrns~!
    * The changes all ended up resultin' in the followin':
      * player movement don't fizzle out no more: IF YE STOP, YE STOP!
      * turnin' be responsive! the camera now moves to give yer eye some room to see where ye be goin'! HAR!
      * same goes fer climbin' and descendin'
      * the player model be rotatin' smoothly between orientations when flyin'! WHOOSH YAR HAR!
      * RMB has been DECOUPLED from movement, but now let's ye look around!
        * the laser eyes (OH THE LASERRRR EYES~!) aim even if ye be futzin' with the looky-loo rmb camerrrra fandangle~!
      * camera smooooothly returns to be rrrrright behind the player if ye let go of control input or RMB!
  * THE BUILDINS BE MINECRAFT AND YER EYE BEAMS BE YER PICKAXE! DESTRUCTION~! LOOTIN~!... okay no lootin, but flyin' through the hole ye make be right kick ass~!
    * as with the changes above, this all be tunable via the VoxelCarvable and RuntimeBuildingChunker scripts attached to the City prefab!
    * destructable objects are decided by ommission at runtime; see the excludedNamePrefixes strings in those scripts... maybe ye wanna make Level 2 be a LASER BEAM DESTRUCT-OTHON!

Speakin' of Level 2... IT'S STILL THE SAME AS LEVEL 1! DIDN'T TOUCH IT! I was so into the DESTRUCTION that I happened ta DESTROY THE MAIN SCENE! GONE! POOF! BLAMMO! 'twas more broken than me leg in the Gargantuan Squid's beak when I was all but chum, lost at sea and caught in the Storm of Agamemnon!... quite a tale, that, BUT NOW'S NOT THE TIME! 

We're here to talk about flyin' and blastin'... FLYIN'. AND. BLASTIN'! ARRRR HAR HAR HAR~!

oh... the blastin' got NO SOUND YET! yar.


### Final notes

what a game!! really nice mechanics, if you have a 5090 you can see no squares at all, or that said the leyend I don't know tbh, well make my effort to put extra history to the mechanics! tried to spawn birds in all places but the frame drop whent really low maybe because found the prefavs and I don't get it anything about 3D so make it the most efficient and run it with constrains!! and it work now you have birds getting over your flying space!! you can laser those chickens but does a hero do that??

the only thing that was not able to fix and maybe is because is 4am and my brain is liquid... o yea the reset button ... that is the only thing that need to be fix happy with the results and really well done all !!! 