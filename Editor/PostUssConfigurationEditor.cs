using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BrewedInk.PostUSS
{
   [CustomEditor(typeof(PostUssConfiguration))]
   public class PostUssConfigurationEditor : Editor
   {
      Dictionary<PostCssPlugin, PostCssPlugin> pluginToData;
      public PostUssConfiguration previousSelf;

      public override void OnInspectorGUI()
      {
         var self = target as PostUssConfiguration;

         void ResetEditablePlugins()
         {
            pluginToData = new Dictionary<PostCssPlugin, PostCssPlugin>();
            pluginToData = self.plugins.ToDictionary(p => p, p => new PostCssPlugin
            {
               name = p.name,
               version = p.version,
               javascriptOption = p.javascriptOption
            });
         }

         if (self != previousSelf || pluginToData == null)
         {
            ResetEditablePlugins();
         }

         bool IsDifferent()
         {
            if (pluginToData.Count != self.plugins.Count) return true;

            foreach (var plugin in self.plugins)
            {
               if (!pluginToData.TryGetValue(plugin, out var editablePlugin))
               {
                  return true;
               }
               if (!string.Equals(plugin.version, editablePlugin.version))
               {
                  return true;
               }
            }

            return false;
         }

         EditorGUILayout.LabelField("Post CSS Configuration");

         // EditorGUI.indentLevel++;
         // list out the plugins, in order.

         var boxStyle = GUI.skin.FindStyle("Box");
         boxStyle = new GUIStyle(boxStyle)
         {

         };

         EditorGUILayout.BeginVertical(boxStyle);
         var toRemove = new List<PostCssPlugin>();
         foreach (var kvp in pluginToData)
         {
            var editablePlugin = kvp.Value;

            EditorGUILayout.BeginHorizontal();
            if (string.IsNullOrEmpty(kvp.Key.name))
            {
               editablePlugin.name = EditorGUILayout.TextField(editablePlugin.name);
            }
            else
            {
               EditorGUILayout.SelectableLabel(editablePlugin.name);
            }
            editablePlugin.version = EditorGUILayout.TextField(editablePlugin.version);
            GUIStyle iconButtonStyle = GUI.skin.FindStyle("IconButton") ?? EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("IconButton");
            GUIContent content = new GUIContent(EditorGUIUtility.Load("icons/_Popup.png") as Texture2D);

            if (EditorGUILayout.DropdownButton(content, FocusType.Keyboard,
               iconButtonStyle))
            {
               GenericMenu menu = new GenericMenu();

               menu.AddItem(new GUIContent("Remove"),false, () =>
               {
                  pluginToData.Remove(kvp.Key);
               });
               // display the menu
               menu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();
         }

         foreach (var remove in toRemove)
         {
            pluginToData.Remove(remove);
         }
         EditorGUILayout.EndVertical();

         // EditorGUI.indentLevel--;

         EditorGUILayout.BeginHorizontal();
         GUILayout.FlexibleSpace();
         if (GUILayout.Button("+", EditorStyles.toolbarButton))
         {
            pluginToData.Add(new PostCssPlugin("", ""), new PostCssPlugin("", ""));
         }
         EditorGUILayout.EndHorizontal();

         EditorGUILayout.BeginHorizontal();
         var wasEnabled = GUI.enabled;
         GUI.enabled = IsDifferent();
         GUILayout.FlexibleSpace();

         if (GUILayout.Button("Revert"))
         {
            ResetEditablePlugins();
         }
         if (GUILayout.Button("Apply"))
         {
            Debug.Log("APPLY");
            self.plugins = pluginToData.Values.ToList();
            ResetTarget();
            EditorUtility.SetDirty(self);
            self.Install();
            self.StartWatch();
         }

         GUI.enabled = wasEnabled;
         EditorGUILayout.EndHorizontal();

         previousSelf = self;
      }
   }
}