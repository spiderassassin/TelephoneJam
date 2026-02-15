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
