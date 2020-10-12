using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace DiDLLCompiler
{
    public class WindowCompiler : EditorWindow
    {
        private static CompilerParams _params;

        private ReorderableList listDependencies;

        private string PathEditorDll => $"{EditorApplication.applicationContentsPath}\\Managed";

        [MenuItem("Window/Utilities/DLL Packer")]
        static void ShowWindow()
        {
            var titleImg = EditorGUIUtility.IconContent("Project").image;
            var titleLabel = "DLL Packer";

            var w = new WindowCompiler()
            {
                titleContent = new GUIContent(titleLabel, titleImg),
                minSize = new Vector2(350, 300)
            };

            w.Init();
            w.Show();
        }

        protected void Init()
        {
            _params = new CompilerParams();
            var ds = _params.Dependencies;

            listDependencies = new ReorderableList(ds, typeof(string));

            listDependencies.drawElementCallback += (rect, index, isActive, isFocused) =>
            {
                rect.height = 19; rect.y += 1;
                var name = System.IO.Path.GetFileName(ds[index]);
                EditorGUI.LabelField(rect, name, EditorStyles.boldLabel);
                //ds[index] = EditorGUI.TextField(rect, ds[index]);
            };

            listDependencies.drawHeaderCallback += (rect) => { GUI.Label(rect, "Additional Dependencies", EditorStyles.miniBoldLabel); };
            listDependencies.onRemoveCallback += (list) => { ds.RemoveAt(list.index); };

            listDependencies.onAddCallback += (list) =>
            {
                var path = EditorUtility.OpenFilePanel("Add dependency", PathEditorDll, "dll");
                if (string.IsNullOrEmpty(path)) { return; }
                ds.Add(path);
            };
        }

        private void OnGUI()
        {
            DrawLabel();

            EditorGUILayout.BeginVertical("helpBox");
            {
                EditorGUILayout.Space(3f);
                DrawPathSaveField();

                _params.NamePackageDll = EditorGUILayout.TextField("Name Package", _params.NamePackageDll);

                EditorGUILayout.Space();
                listDependencies.DoLayoutList();
                EditorGUILayout.Space();

                DrawPathScriptsFolder();
                EditorGUILayout.Space(3f);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            if (GUILayout.Button("Compiler", GUILayout.Height(45)))
            {
                DllPacker.Compiler(_params);
            }
        }

        // ------------------------- Draw UI ------------------------- //

        private void DrawLabel()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Packer Settings", EditorStyles.boldLabel);
            EditorGUILayout.EndVertical();
        }

        private void DrawPathSaveField()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = 85;
                _params.PathSave = EditorGUILayout.TextField(new GUIContent("DLL path save"), _params.PathSave);
                EditorGUIUtility.labelWidth = 0;

                if (GUILayout.Button("Browse", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                {
                    _params.PathSave = EditorUtility.OpenFolderPanel("Add file", "", "");
                }
                if (GUILayout.Button("Reset", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                {
                    _params.PathSave = CompilerParams.GetDefaultPathSave();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPathScriptsFolder()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUIUtility.labelWidth = 85;
                _params.PathFolderScripts = EditorGUILayout.TextField(new GUIContent("Scripts folder"), _params.PathFolderScripts);
                EditorGUIUtility.labelWidth = 0;

                if (GUILayout.Button("Browse", EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
                {
                    _params.PathFolderScripts = EditorUtility.OpenFolderPanel("Add file", "Assets", "");
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
