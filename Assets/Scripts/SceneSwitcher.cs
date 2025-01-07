#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : EditorWindow
{
    private const string SceneBeforePlayKey = "SCENE_BEFORE_PLAY";
    private const string SelectedGOBeforePlayKey = "SELECTED_GO_BEFORE_PLAY";

    private Vector2 _scroll;
    private static string _playFrom;

    [MenuItem("Helpers/Scene Switcher", false, 1)]
    static void ShowPackageManagerWindow()
    {
        var window = (SceneSwitcher)GetWindow(typeof(SceneSwitcher));
        window.Show();
    }

    [MenuItem("Helpers/Play From Init %g", false, 1)]
    static void PlayFromInit()
    {
        OpenAndPlayScene(EditorBuildSettings.scenes[0].path);
    }

    [MenuItem("Helpers/Play From Init %g", true, 1)]
    static bool PlayFromInitValidation()
    {
        return EditorBuildSettings.scenes.Length != 0 && EditorBuildSettings.scenes[0].enabled;
    }

    private SceneSwitcher()
    {
        titleContent.text = "Switcher";
    }

    static SceneSwitcher()
    {
        EditorApplication.playModeStateChanged += ModeChanged;
    }

    private static void OpenAndPlayScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorPrefs.SetString(SceneBeforePlayKey, SceneManager.GetActiveScene().path);

            if (Selection.activeTransform != null)
                EditorPrefs.SetString(SelectedGOBeforePlayKey, GetSiblingsID(Selection.activeTransform));

            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            EditorApplication.isPlaying = true;
        }
    }

    private static void Stopped()
    {
        if (EditorPrefs.HasKey(SceneBeforePlayKey))
        {
            string path = EditorPrefs.GetString(SceneBeforePlayKey);
            EditorPrefs.DeleteKey(SceneBeforePlayKey);
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        }

        LoadLastSelectedGO();
    }

    private static void LoadLastSelectedGO()
    {
        if (EditorPrefs.HasKey(SelectedGOBeforePlayKey))
        {
            var indexes = EditorPrefs.GetString(SelectedGOBeforePlayKey).Split(',');
            List<GameObject> rootGOs = new List<GameObject>(SceneManager.GetActiveScene().GetRootGameObjects());
            if (rootGOs.Count > 0)
            {
                // We need to iterate through root's GOs coz index in array is not same as sibling index! Thanks Unity! :-/
                Transform rootTrans = rootGOs.Find((x) => x.transform.GetSiblingIndex() == int.Parse(indexes[0])).transform;

                var transformToSelect = GetTransformBySiblingsID(indexes, rootTrans);
                Selection.activeTransform = transformToSelect;
            }

            EditorPrefs.DeleteKey(SelectedGOBeforePlayKey);
        }
    }

    /// <summary>
    /// Gets ID of transform. ID is list of sibling indexes. From root to selected object
    /// </summary>
    /// <param name="activeTransform">Transform in hierarchy</param>
    /// <param name="value"></param>
    /// <returns>ID in form of sibling indexes</returns>
    private static string GetSiblingsID(Transform activeTransform, string value = "")
    {
        value += activeTransform.GetSiblingIndex().ToString();

        if (activeTransform.parent == null)
        {
            // Reverse string
            char[] charArray = value.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        value += ",";

        return GetSiblingsID(activeTransform.parent, value);
    }

    /// <summary>
    /// Returns transform by sibling ID
    /// </summary>
    /// <param name="siblingList"></param>
    /// <param name="root"></param>
    /// <param name="index"></param>
    /// <returns>Transform</returns>
    private static Transform GetTransformBySiblingsID(string[] siblingList, Transform root, int index = 1)
    {
        if (index >= siblingList.Length)
            return root;

        return GetTransformBySiblingsID(siblingList, root.GetChild(int.Parse(siblingList[index])), index + 1);
    }


    private static void ModeChanged(PlayModeStateChange state)
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
        {
            Stopped();
        }
    }

    private void OnGUI()
    {
        var scenes = EditorBuildSettings.scenes.Where(x => x.enabled).ToArray();
        GUILayout.BeginVertical(EditorStyles.helpBox);

        _scroll = GUILayout.BeginScrollView(_scroll);
        var tmpColor = GUI.backgroundColor;
        if (scenes.Length == 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("No scenes added or enabled in build settings...");
            GUILayout.EndHorizontal();
        }
        else if (EditorApplication.isCompiling || EditorApplication.isPlaying)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Cannot switch scenes in play mode or during compilation...");
            GUILayout.EndHorizontal();
        }
        else
        {
            foreach (var scene in scenes)
            {
                var sceneName = Path.GetFileNameWithoutExtension(scene.path);
                if (sceneName == SceneManager.GetActiveScene().name)
                {
                    GUI.backgroundColor = Color.green;
                }
                else
                {
                    GUI.backgroundColor = tmpColor;
                }

                Rect currentRect = EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                if (GUILayout.Button("Play", GUILayout.Width(64)))
                {
                    // return back to previous scene
                    OpenAndPlayScene(scene.path);
                    return;
                }

                if (sceneName != SceneManager.GetActiveScene().name)
                {
                    if (GUILayout.Button("Open", GUILayout.Width(64)))
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                            EditorSceneManager.OpenScene(scene.path);
                        return;
                    }
                }
                else
                {
                    GUILayout.Space(68);
                }
                //ping
                Event ev = Event.current;
                if (ev.type == EventType.MouseDown && currentRect.Contains(ev.mousePosition))
                {
                    if (ev.clickCount > 1)
                    {
                        EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(scene.path));
                    }
                }

                //end ping
                //					GUILayout.FlexibleSpace();
                GUILayout.Label(sceneName);
                EditorGUILayout.EndHorizontal();

            }
        }
        GUI.backgroundColor = tmpColor;
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
}
#endif
