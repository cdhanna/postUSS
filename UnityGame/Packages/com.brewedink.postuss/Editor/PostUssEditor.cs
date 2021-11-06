using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

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
            InitProject();
        }

        public static void InitProject()
        {
            Directory.CreateDirectory(CONFIG_PATH);
            Directory.CreateDirectory(NODE_PROJECT_PATH);


            // is there a configuration file?
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

        }
    }
}