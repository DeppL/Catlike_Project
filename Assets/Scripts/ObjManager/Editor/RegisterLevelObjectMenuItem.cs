using UnityEngine;
using UnityEditor;

static class RegisterLevelObjectMenuItem
{
    const string menuItem = "GameObject/Register Level Object";
    [MenuItem(menuItem, true)]
    static bool ValidateRegisterLevelObject ()
    {
        if (Selection.objects.Length == 0) {
            return false;
        }
        foreach (var item in Selection.objects) {
            if (!(item is GameObject)) {
                return false;
            }
        }
        return true;
    }
    [MenuItem(menuItem)]
    static void ResigterLevelObjects ()
    {
        foreach (var item in Selection.objects) {
            Register(item as GameObject);
        }
    }

    static void Register (GameObject o)
    {
        if (PrefabUtility.GetPrefabAssetType(o) != PrefabAssetType.NotAPrefab) {
            Debug.LogWarning(o.name + " is a prefab asset", o);
            return;
        }
        var levelObject = o.GetComponent<GameLevelObject>();
        if (levelObject == null) {
            Debug.LogWarning(o.name + " isn't a game level object.", o);
            return;
        }

        foreach (GameObject rootObject in o.scene.GetRootGameObjects()) {
            var gamelevel = rootObject.GetComponent<GameLevel>();
            if (gamelevel != null) {
                if (gamelevel.HasLevelObject(levelObject)) {
                    Debug.LogWarning((o.name + " is already registered.", o));
                    return;
                }

                Undo.RecordObject(gamelevel, "Register Level Object.");
                gamelevel.RegisterLevelObject(levelObject);
                Debug.Log(
                    o.name + " registered to game level " +
                    gamelevel.name + " in scene " + o.scene.name + ".", o
                );
                return;
            }
        }
        Debug.LogWarning(o.name + " isn't part of a game level.", o);
    }
}
