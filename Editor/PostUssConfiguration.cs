using System;
using System.Collections.Generic;
using System.IO;
using BrewedInk.PostUSS.Npm;
using UnityEngine;

namespace BrewedInk.PostUSS
{
   public class PostUssConfiguration : ScriptableObject
   {
      public string description;
      public List<StringPair> plugins;

      public string compilePath;

      [ContextMenu("NPM INSTALL")]
      public void Install()
      {
         // run an npm command...
         Debug.Log("Running NPM Install");

         CompilePackageJson();
         var installCommand = new NpmInstall(this);
         installCommand.WriteLogToUnity = true;

         installCommand.Start();
      }

      [ContextMenu("NPM RUN BUILD")]
      public void Build()
      {
         var runCommand = new NpmRun(this, "build");
         runCommand.WriteLogToUnity = true;
         runCommand.WriteCommandToUnity = true;
         runCommand.Start();
      }

      private void OnValidate()
      {
         CompilePackageJson();
      }

      private void CompilePackageJson()
      {
         var model = ToModel();
         Directory.CreateDirectory(Path.GetDirectoryName(compilePath));
         File.WriteAllText(compilePath, model.ToJson());
      }

      private NodePackageModel ToModel()
      {
         return new NodePackageModel
         {
            name = "postussconfig",
            description = description,
            scripts = new List<StringPair>
            {
               new StringPair("build", "postcss ../../../**/*.css --base ../../.. --dir ../../.. --ext uss"),
               new StringPair("buildEnv", "postcss ../../../../${INPUT} --base ../../../.. --dir ../../../PostUSS/Build/ --ext uss"),
               new StringPair("buildAll", "postcss ../../../**/*.css --base ../../../.. --dir ../../../PostUSS/Build/ --ext uss -w")
            },
            devDependencies = plugins
         };
      }
   }
}