using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BrewedInk.PostUSS
{
   [Serializable]
   class NodePackageModel
   {
      public static NodePackageModel DefaultPostUSSPackage = new NodePackageModel
      {
         name = "post_uss_configuration",
         description = "this is your configuration for postUSS.",
         scripts = new List<StringPair>
         {
            new StringPair("build", "postcss")
         },
         devDependencies = new List<StringPair>
         {
            new StringPair("postcss", "^8.3.11"),
            new StringPair("postcss-cli", "^9.0.2"),
            new StringPair("postcss-css-variables", "^0.18.0"),
            new StringPair("postcss-import", "^14.0.2"),
            new StringPair("postcss-nested", "^5.0.6"),
            new StringPair("postcss-nested-props", "^2.0.0"),
         }
      };


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
            for (var i = 0 ; i < pairs.Count; i ++)
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

      public static NodePackageModel FromJson(string json)
      {
         var buffer = "";
         var instance = new NodePackageModel();
         for (var i = 0; i < json.Length; i++)
         {
            var c = json[i];
            buffer += c;
            if (buffer.Equals("{\"name\":\""))
            {
               // read until the next end quote.
               i++;
               while (json[i] != '"')
               {
                  instance.name += json[i];
                  i++;
               }

               buffer = "";
            }

            if (buffer.Equals(",\"description\":\""))
            {
               i++;
               while (json[i] != '"')
               {
                  instance.description += json[i];
                  i++;
               }
               buffer = "";
            }
         }

         return instance;
      }
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