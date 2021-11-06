using System;
using BrewedInk.PostUSS;
using UnityEditor;
#if UNITY_2018
using UnityEngine.Experimental.UIElements;
using UnityEditor.Experimental.UIElements;

#elif UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

namespace Example
{
   public class SampleWindow : EditorWindow
   {
      [MenuItem("BrewedInk/Example Window")]
      public static void Init()
      {
         var wnd = EditorWindow.GetWindow<SampleWindow>();
         wnd.Show();
      }

      private VisualElement _windowRoot;

      private void OnEnable()
      {
         var root = this.GetRootVisualContainer();
         root.Clear();
         var uiAsset =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"Assets/Editor/SampleWindow.uxml");
         _windowRoot = uiAsset.CloneTree(null);
         _windowRoot.AddCSSPath("Assets/Editor/Sample.css");
         _windowRoot.name = nameof(_windowRoot);

         root.Add(_windowRoot);
      }
   }
}