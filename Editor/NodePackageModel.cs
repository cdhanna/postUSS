using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BrewedInk.PostUSS
{
   [Serializable]
   class NodePackageModel
   {

      public string name = "";
      public string description = "";
      public List<StringPair> scripts = new List<StringPair>();
      public List<StringPair> devDependencies = new List<StringPair>();

      const string QUOTE = "\"";
      const string COLON = ":";
      const string COMMA = ",";
      const string OBJ_OPEN = "{";
      const string OBJ_CLOSE = "}";

      public string ToJson()
      {
         var sb = new StringBuilder();


         void AddKey(string name, string value)
         {
            sb.Append(QUOTE);
            sb.Append(name);
            sb.Append(QUOTE);

            sb.Append(COLON);
            sb.Append(QUOTE);
            sb.Append(value);
            sb.Append(QUOTE);
         }

         void AddMap(string name, List<StringPair> pairs)
         {
            sb.Append(QUOTE);
            sb.Append(name);
            sb.Append(QUOTE);
            sb.Append(COLON);
            sb.Append(OBJ_OPEN);
            for (var i = 0; i < pairs.Count; i++)
            {
               var pair = pairs[i];
               AddKey(pair.key, pair.value);
               if (i != pairs.Count - 1) sb.Append(COMMA);
            }

            sb.Append(OBJ_CLOSE);
         }

         sb.Append(OBJ_OPEN);
         AddKey("name", name);
         sb.Append(COMMA);
         AddKey("description", description);
         sb.Append(COMMA);
         AddMap("scripts", scripts);
         sb.Append(COMMA);
         AddMap("devDependencies", devDependencies);
         sb.Append(OBJ_CLOSE);

         return sb.ToString();
      }
   }

   [Serializable]
   public class PostCssModuleConfigurationModel
   {
      public List<PostCssPlugin> plugins;

      public string ToJavascript()
      {
         var sb = new StringBuilder();

         sb.Append("module.exports = { plugins: [ ");

         foreach (var plugin in plugins)
         {
            sb.Append("require('");
            sb.Append(plugin.name);
            sb.Append("')(");
            if (!string.IsNullOrEmpty(plugin.javascriptOption))
               sb.Append(plugin.javascriptOption);
            sb.Append("),");
         }

         sb.Append("] }");

         return sb.ToString();
      }
      /*
       * module.exports = {
    plugins: [
      require('postcss-import')(),
      require('postcss-css-variables')(),
      require('postcss-nested')(),
      require('postcss-nested-props')()
    ],
  }
       */
   }

   [Serializable]
   public class StringPair
   {
      public string key, value;

      public StringPair()
      {

      }

      public StringPair(string key, string value)
      {
         this.key = key;
         this.value = value;
      }
   }
}