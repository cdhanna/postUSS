using System.Collections.Generic;
using System.IO;
using BrewedInk.PostUSS.Npm;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace BrewedInk.PostUSS
{
    public static class PostUssEditor
    {
        public const string CONFIG_PATH = "Assets/PostUSS/Editor/";
        public const string NODE_PROJECT_PATH = CONFIG_PATH + "~/";
        public const string PACKAGE_JSON_PATH = NODE_PROJECT_PATH + "package.json";
        public const string CONFIG_FILE_PATH = CONFIG_PATH + "postUss.asset";

        [MenuItem("BrewedInk/Test")]
        public static void Test()
        {
            GetConfiguration();
        }

        private static PostUssConfiguration _instance;

        public static void AddCSSPath(this VisualElement self, string cssPath)
        {
            self.AddStyleSheetPath(GetUssBuildPath(cssPath));
        }

        public static string GetUssBuildPath(string cssPath)
        {
            var ussPath = Path.ChangeExtension(cssPath, "uss");
            return $"Assets/PostUSS/Build/{ussPath}";
        }

        [DidReloadScripts]
        public static void Watch()
        {
            var config = GetConfiguration();
            var command = new NpmRun(config, "buildAll");
            command.Start();
        }

        public static PostUssConfiguration GetConfiguration()
        {
            Directory.CreateDirectory(CONFIG_PATH);
            Directory.CreateDirectory(NODE_PROJECT_PATH);

            // is there a configuration file?
            if (_instance != null)
            {
                return _instance;
            }

            // TODO: Add a .gitignore automatically for the node_modules folder
            

            var config = AssetDatabase.LoadAssetAtPath<PostUssConfiguration>(CONFIG_FILE_PATH);
            if (!config)
            {
                config = ScriptableObject.CreateInstance<PostUssConfiguration>();

                config.description = "Post USS Config File";
                config.compilePath = PACKAGE_JSON_PATH;
                config.plugins = new List<StringPair>
                {
                    new StringPair("postcss", "^8.3.11"),
                    new StringPair("postcss-cli", "^9.0.2"),
                    new StringPair("postcss-css-variables", "^0.18.0"),
                    new StringPair("postcss-import", "^14.0.2"),
                    new StringPair("postcss-nested", "^5.0.6"),
                    new StringPair("postcss-nested-props", "^2.0.0"),
                };

                AssetDatabase.CreateAsset(config, CONFIG_FILE_PATH);
            }

            _instance = config;
            return config;
        }
    }
}