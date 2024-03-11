/* THIS CODE WAS AUTO GENERATED LOL */

using UnityEditor;
using UnityEditor.SceneManagement;

public static partial class LoadSceneMenuItemsGenerator {

    private static void LoadScene(string name) {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        EditorSceneManager.OpenScene(name);
    }

    private static class MenuItems {

        [MenuItem("Load Scene/Main Menu", priority = 1)]
        private static void LoadScene1() => LoadScene("Assets/Scenes/Main Menu.unity");

        [MenuItem("Load Scene/SampleScene", priority = 2)]
        private static void LoadScene2() => LoadScene("Assets/Scenes/SampleScene.unity");

        [MenuItem("Load Scene/Tutorial", priority = 3)]
        private static void LoadScene3() => LoadScene("Assets/Scenes/Tutorial.unity");
    }
}
