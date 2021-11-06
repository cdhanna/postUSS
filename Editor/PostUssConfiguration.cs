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

         var installCommand = new NpmInstall(this);
         installCommand.WriteCommandToUnity = true;
         installCommand.WriteLogToUnity = true;

         installCommand.Start();
      }

      private void OnValidate()
      {
         var model = ToModel();
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
               new StringPair("build", "")
            },
            devDependencies = plugins
         };
      }
   }
}