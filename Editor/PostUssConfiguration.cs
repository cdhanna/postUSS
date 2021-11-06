using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BrewedInk.PostUSS.Npm;
using UnityEngine;

namespace BrewedInk.PostUSS
{
   public class PostUssConfiguration : ScriptableObject
   {
      public string description;
      public List<StringPair> devDependencies;
      public List<PostCssPlugin> plugins;

      public string compilePath;
      public string configPath;

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
         Directory.CreateDirectory(Path.GetDirectoryName(compilePath));
         Directory.CreateDirectory(Path.GetDirectoryName(configPath));

         File.WriteAllText(compilePath, GetPackageJsonModel().ToJson());
         File.WriteAllText(configPath, GetPostCssConfigModel().ToJavascript());
      }

      private NodePackageModel GetPackageJsonModel()
      {
         var allDependencies = devDependencies.ToList();
         allDependencies.AddRange(plugins.Select(p => new StringPair(p.name, p.version)));
         return new NodePackageModel
         {
            name = "postussconfig",
            description = description,
            scripts = new List<StringPair>
            {
               new StringPair("build", "postcss ../../../**/*.css --base ../../.. --dir ../../.. --ext uss"),
               new StringPair("buildEnv", "postcss ../../../../${INPUT} --base ../../../.. --dir ../../../PostUSS/Build/ --ext uss"),
               new StringPair("buildAll", "postcss ../../../**/*.css --base ../../../.. --dir ../../../PostUSS/Build/ --ext uss --config ./ -w")
            },
            devDependencies = allDependencies
         };
      }

      private PostCssModuleConfigurationModel GetPostCssConfigModel()
      {
         return new PostCssModuleConfigurationModel
         {
            plugins = plugins
         };
      }
   }

   [Serializable]
   public class PostCssPlugin
   {
      public string name;
      public string version;
      public string javascriptOption;

      public PostCssPlugin(){}

      public PostCssPlugin(string name, string version)
      {
         this.name = name;
         this.version = version;
      }
   }

}