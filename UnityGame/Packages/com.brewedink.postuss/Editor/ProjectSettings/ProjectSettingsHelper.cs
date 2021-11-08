using UnityEditor;
using UnityEngine.Experimental.UIElements;

namespace BrewedInk.PostUSS
{
   public class ProjectSettingsHelper
   {
      [SettingsProvider]
      public static SettingsProvider GetSettings()
      {
         return new SettingsProvider("Project/Post USS", SettingsScope.Project)
         {
            label = "Post USS",
            activateHandler = GetPageElement
         };
      }

      static void GetPageElement(string input, VisualElement element)
      {
         element.Clear();
         var template = EditorGUIUtility.Load("Packages/com.brewedink.postuss/Editor/ProjectSettings/ProjectSettings.uxml") as VisualTreeAsset;
         var root = template.CloneTree(null);
         root.AddCSSPath("Packages/com.brewedink.postuss/Editor/ProjectSettings/PackageTest.css");
         element.Add(root);
      }
   }
}