using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;


namespace Mighty 
{
    public class MightyWizard : EditorWindow
    {
        const string LOGO_PATH = "Assets/Mighty/Assets/Media/MightyGamePack_Logo.png";
        const string INPUT_SETTINGS_PATH = "Mighty/Assets/Other/MightyInputSettings.txt";

        Vector2 scrollPosition;
        //bool createWithLocalMightyVersion = true;
        Texture2D mightyLogo;

        [MenuItem("Window/Mighty Wizard")]

        static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(MightyWizard), false, "Mighty Wizard");
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUIStyle.none);

            // Draw logo
            EditorGUI.DrawPreviewTexture(new Rect(0, 0, this.position.width, this.position.width), GetMightyLogo());
            GUILayout.Space(this.position.width);

            DrawSeparator();

            // Sets up ready-to-go project (folders, scenes, ui, managers, etc)
            //if (GUILayout.Button("Create Default Project Setup"))
            //{
            //    if (createWithLocalMightyVersion)
            //    {
            //        CreateLocalMightyVersion();
            //    }
            //    CreateDefaultProjectSetup(createWithLocalMightyVersion);
            //}

            //DrawSeparator();

            if (GUILayout.Button("Apply Mighty Input Settings"))
            {
                string mightyInputManagerPath = Path.Combine(Application.dataPath, INPUT_SETTINGS_PATH);
                string inputManagerPath = Path.Combine(Application.dataPath, "..", "ProjectSettings", "InputManager.asset");
                File.Copy(mightyInputManagerPath, inputManagerPath, true);
                AssetDatabase.Refresh();
                Debug.Log("[MightyWizard] Successfully applied input settings");
            }

            // GUILayout.Label("Plugins");

            //if (GUILayout.Button("Import Hierarchy2"))
            //{
            //    CopyDirectory(Path.Combine(GetMightyPackageInfo().resolvedPath, HIERARCHY2_PATH), Path.Combine(Application.dataPath, "Mighty", "ThirdParty", "Hierarchy2"));
            //    AssetDatabase.Refresh();
            //}

            //if (GUILayout.Button("Import Debug Drawing Extension"))
            //{
            //    CopyDirectory(Path.Combine(GetMightyPackageInfo().resolvedPath, DEBUGDRAWINGEXTENSION_PATH), Path.Combine(Application.dataPath, "Mighty", "ThirdParty", "DebugDrawingExtension"));
            //    AssetDatabase.Refresh();
            //}

            EditorGUILayout.EndScrollView();
        }

        void CreateLocalMightyVersion()
        {

        }

        void CreateDefaultProjectSetup(bool withLocalMightyVersion)
        {
            
        }

        Texture GetMightyLogo()
        {
            if (mightyLogo == null)
            {
                mightyLogo = (Texture2D)AssetDatabase.LoadAssetAtPath(LOGO_PATH, typeof(Texture2D));            
            }
            return mightyLogo;
        }

        void DrawSeparator()
        {
            GUILayout.Space(5);
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 1), new Color(0.1f, 0.1f, 0.1f));
            GUILayout.Space(5);
        }

        void CopyDirectory(string sourcePath, string targetPath)
        {
            Directory.CreateDirectory(targetPath);

            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }

}


