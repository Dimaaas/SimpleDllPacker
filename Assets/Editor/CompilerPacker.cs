using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditorInternal;//parameters.ReferencedAssemblies.Add(InternalEditorUtility.GetEngineAssemblyPath());

namespace DiDLLCompiler
{
    public class DllPacker
    {
        public static void Compiler(CompilerParams parm)
        {
            Directory.CreateDirectory(parm.PathSave);

            var opts = new Dictionary<string, string> {{ "CompilerVersion", "v3.5" }};

            using (var codeProvider = new CSharpCodeProvider(opts))
            {
                var parameters = new CompilerParameters() {
                    GenerateExecutable = false,
                    OutputAssembly = parm.PathSave + parm.NamePackageDll,
                };

                parameters.ReferencedAssemblies.AddRange(parm.Dependencies.ToArray());

                var phScr = parm.GetScriptsPath();
                //var codeSources = phScr.Select(f => File.ReadAllText(f)).ToList();
                //codeSources.Add(GetCodeAssemblyVersion(parm.Version));
    
                var result = codeProvider.CompileAssemblyFromFile(parameters, parm.GetScriptsPath().ToArray()); //FromSource(parameters, codeSources.ToArray());

                if (result.Errors.HasErrors)
                {
                    Debug.LogError("Compiler Errors!!!!!");

                    foreach (CompilerError error in result.Errors)
                    {
                        var fileName = Path.GetFileName(error.FileName);
                        Debug.LogError($"{fileName}({error.Line},{error.Column}) : {error.ErrorText}");
                    }
                }
                else
                {
                    Debug.Log("Successfully Compiled!!!!!!!!");
                }
            }
        }

        //private static string GetCodeAssemblyVersion(string v)
        //    => $"[assembly: System.Reflection.AssemblyVersion(\"{v}\")]\n[assembly: System.Reflection.AssemblyFileVersion(\"{v}\")]";
    }
}