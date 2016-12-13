# Burglary
In-progress Unity game with an emphasis on realistic thieving mechanics which require forethought and patience to overcome.

Current build available here: https://jgthibodeau.github.io/Burglary-Game/

Xbox 360 controller recomended, and required for some mechanics currently.

---

#### Normal Controls
- Start - Pause
- Left Stick - Move
- Right Stick - Adjust Camera
- A - Interact with objects (unlock, open, close)
- B - Cancel interaction

#### Lockpicking
To pick a lock, you need to set each tumbler to the correct height. In order to set a tumbler, the lockpick must be moved to it, and you must apply the appropriate amount of tension.
- Left Stick X - Move the pick between tumblers, a click signifies a new tumbler.
- Right Trigger - Adjust pick height. A shaking lockpick indicates you are lifting the tumbler that is currently setable, this tumbler will fall if you stop applying tension. A click indicates that the tumbler has reached the correct height.
  - In a future update, I may change it so that the lockpick shakes when you are lifting any tumbler not just the currently setable one.
- Left Trigger - Adjust tension wrench. As each tumbler is set, more tension needs to be applied. When the wrench begins shaking, you are applying the correct tension for this tumbler. Apply too much tension and the tumbler will not move. Apply too little and the tumbler will drop.

#### Dial Locks
To get past a dial lock, you need to rotate the dial to the correct positions in order. Rotate counter-clockwise to the first, then reverse direction for each subsequent one.
- Left Stick - Rotate the dial. A click signifies that the correct number has been set and to change directions.
  - Rotate counter clockwise past 0 at least 3 times to reset the lock.
- Right Trigger - Open the safe once all numbers are entered.

#### Guards
Guards have a set patrol path that they will walk between. If the player is detected, they will give chase until they can no longer see the player, and will call in reinforcements when the player is initially seen. They have 3 fields of view:
- Front view - The player is imediately seen if they enter this field.
- Peripheral & back view - The guard will rotate towards the player if they are in these fields in an attempt to directly see them.

---

#### TODO
- Paintings
  - Use a knife to cut out the painting, better cuts keep its worth higher but if in a pinch you can move quicker and sloppier
- Inventory
  - Way to swap tools and view current stolen goods
- Shop
  - Sell stolen goods, purchase new tools
- Guards
  - Remain on alert after player spotted
  - Call in backup
  - Distractable by throwing items/making noise
- Map
  - Interactive map that allows the player to draw on it and mark points of interest as they play the level
- Sounds
  - Moving at different speeds will produce sound that guards can hear. Need a way to display sound to the player, possibly make visible sound wave circles
