using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;

namespace DiDLLCompiler
{
    public class CompilerParams
    {
        public string NamePackageDll { set; get; }
        public string PathSave { set; get; }

        public string Version { set; get; }

        public List<string> Dependencies { set; get; }

        public string PathFolderScripts { set; get; }

        public CompilerParams()
        {
            NamePackageDll = $"{Application.productName}.dll";
            PathSave = GetDefaultPathSave();

            Dependencies = new List<string> {InternalEditorUtility.GetEngineCoreModuleAssemblyPath()};

            Version = "1.0.0.0";
        }

        public List<string> GetScriptsPath()
        {
            if (string.IsNullOrEmpty(PathFolderScripts))
            {
                Debug.LogError("PathFolderScripts не должен быть пустым");
                return null;
            }
            if (!Directory.Exists(PathFolderScripts))
            {
                Debug.LogError($"{PathFolderScripts} неверный путь к скриптам");
                return null;
            }

            return GetAllScriptsFolder(PathFolderScripts);
        }

        public static List<string> GetAllScriptsFolder(string path)
        {
            var list = new List<string>();

            var folder = Directory.GetDirectories(path);
            foreach (var f in folder)
            {
                list.AddRange(GetAllScriptsFolder(f));
            }

            var files = Directory.GetFiles(path);
            foreach (var f in files)
            {
                if (Path.GetExtension(f) == ".cs")
                {
                    list.Add(f);
                }
            }

            return list;
        }

        public static string GetNameDll() => Application.productName;

        public static string GetDefaultPathSave()
        {
            var defPath = Application.dataPath;
            defPath = defPath.Remove(defPath.Length - 6);
            return $"{defPath}Bin/";
        }
    }
}