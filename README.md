# Unity Scene Updater
Simple script with common functions to update all Unity project scenes with one click

## What It Can Do?
- Update each scene in Unity project with AutoSave;
- Instantiate game object with specific hierarchy;
- Change object layer by layer name;
- Find child with name deep in Transform hierarchy;
- Change RectTransform Anchors and Pivot using easy-to-use presets;
- Copy Transform or RectTransform local position, rotation and scale from one object to another;
- Change Canvas and TextMeshPro Settings via script;
- Add PersistentListener to Button (can be seen and changed in Inspector).

Modify as you want to make it work for your project!

## How To Use?
1. Change `SceneUpdater` class in `Assets\Scripts\Editor\SceneUpdater.cs` and save it;
2. Add scenes you want to change to BuildSettings (scene activation doesn't matter);
3. Go to `Custom Tools -> Scene Updater` in Unity menu bar;
4. Click `Update scenes` button;
5. Wait until finished.

**WARNING!** Try to change one scene before running script on each one! Use version control!

## Tips
- Use `Utils` folder for Unity classes Extensions.
- Use `Editor` folder for Custom GUI tools. 
- Use `Template` scene to try on.