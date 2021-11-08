using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BrewedInk.PostUSS.Npm;
using UnityEditor;
using UnityEditor.Callbacks;
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
      public WatchDirectory WatchAssets = new WatchDirectory
      {
         src = "relative path from compilePath to Assets ",
         basePath = "same as source",
         dest = "relative path to transpile dir"
      };

      public List<WatchDirectory> WatchPackages = new List<WatchDirectory>();

      [ContextMenu("NPM INSTALL")]
      public void Install()
      {
         // run an npm command...
         CompileAndSave();
         var installCommand = new NpmInstall(this);
         installCommand.Start();
         installCommand.Join();
      }

      [ContextMenu("START WATCHING")]
      public void StartWatch()
      {
         // walk backwards from the compileDir until you hit assets.
         var compileDirectory = Path.GetDirectoryName(compilePath);
         var relativeSrc = string.Join(Path.DirectorySeparatorChar.ToString(),
            compileDirectory
               .Split(Path.DirectorySeparatorChar)
               .Reverse()
               .TakeWhile(x => !string.Equals("Assets", x))
               .Select(_ => ".."));

         WatchAssets.src = relativeSrc;
         WatchAssets.basePath = relativeSrc;
         WatchAssets.dest = Path.Combine(relativeSrc, PostUssEditor.AUTOGEN_PATH_PART);

         var projectPath = Path.GetDirectoryName(Application.dataPath);
         var rootPath = Path.Combine(projectPath, "Packages");
         WatchPackages.Clear();
         foreach (var packageDir in Directory.GetDirectories(rootPath))
         {
            // go up until you hit the root of the world...
            var relativePackageDir = packageDir.Substring(projectPath.Length);
            var allToRoot = string.Join(Path.DirectorySeparatorChar.ToString(), compileDirectory
               .Split(Path.DirectorySeparatorChar)
               .Select(_ => ".."));
            var path = allToRoot + relativePackageDir;
            WatchPackages.Add(new WatchDirectory
            {
               src = path,
               basePath = path,
               dest = Path.Combine(path, PostUssEditor.AUTOGEN_PATH_PART)
            });
         }

         WatchAssets.Start(this);
         foreach (var extraWatch in WatchPackages)
         {
            extraWatch.Start(this);
         }
      }

      [DidReloadScripts]
      public static void Watch()
      {
         var config = PostUssEditor.GetConfiguration();

         config.Install();
         config.StartWatch();
      }


      private void OnValidate()
      {
         CompileAndSave();
      }

      public void CompileAndSave()
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
               // new StringPair("build", "postcss ../../../**/*.css --base ../../.. --dir ../../.. --ext uss"),
               // new StringPair("buildEnv", "postcss ../../../../${INPUT} --base ../../../.. --dir ../../../PostUSS/Build/ --ext uss"),
               // new StringPair("buildAll", "postcss ../../../**/*.css --base ../../../.. --dir ../../../PostUSS/Build/ --ext uss --config ./ -w"),
               // new StringPair("buildAll", "postcss ../../../**/*.css --base ../../../.. --dir ../../../PostUSS/Build/ --ext uss --config ./ -w"),
               new StringPair("build", "postcss ${SRC}/**/*.css --base ${BASE} --dir ${DIST} --ext uss --config ./ -w")
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

   [System.Serializable]
   public class WatchDirectory
   {
      public string src;
      public string basePath;
      public string dest;

      public NpmRun Command { get; set; }

      public void Start(PostUssConfiguration config)
      {
         Command?.Kill();
         Command = new NpmRun(config, "build", new Dictionary<string, string>
         {
            ["SRC"]=src,
            ["BASE"]=basePath,
            ["DIST"]=dest,
         });

         // Command.WriteLogToUnity = true;
         // Command.WriteCommandToUnity = true;
         Command.Start();
      }
   }

}