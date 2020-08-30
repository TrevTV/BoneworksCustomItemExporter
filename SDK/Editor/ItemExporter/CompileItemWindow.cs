using System;
using System.IO;
using System.IO.Pipes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ItemExporter
{
    public class CompileItemWindow : EditorWindow
    {
        private ItemDescriptor[] items;

        [MenuItem("Window/Item Exporter")]
        public static void ShowWindow()
        {
            GetWindow(typeof(CompileItemWindow), false, "Item Exporter");
        }

        public void OnGUI()
        {
            GUILayout.Label("Items", EditorStyles.boldLabel);

            GUILayout.Space(20);

            foreach (ItemDescriptor item in items)
            {
                if (item == null) return;
                GUILayout.Label("GameObject : " + item.gameObject.name, EditorStyles.boldLabel);
                if (!item.isMultipleItems)
                {
                    EditorGUILayout.LabelField("Hidden In Menu", item.data.hideInMenu.ToString());
                    EditorGUILayout.LabelField("Category", item.data.category.ToString());
                    EditorGUILayout.LabelField("Pool Amount", item.data.poolAmount.ToString());
                }
                else
                {
                    EditorGUILayout.LabelField("Contains Multiple Items");
                }

                if (GUILayout.Button("Export " + item.gameObject.name))
                {
                    GameObject itemObject = item.gameObject;
                    if (itemObject != null && item != null)
                    {
                        string path = EditorUtility.SaveFilePanel("Save item file", "", itemObject.name + ".melon", "melon");

                        if (path != "")
                        {
                            string fileName = Path.GetFileName(path);
                            string folderPath = Path.GetDirectoryName(path);

                            string[] prefabs = AssetDatabase.FindAssets("CustomItems t:prefab", new[] { "Assets" });

                            if (prefabs.Length > 0)
                            {
                                bool willContinue = EditorUtility.DisplayDialog("Error",
                                    "An existing CustomItems prefab has been found, would you like to delete it before continuing?",
                                    "Yes", "Cancel");

                                if (willContinue)
                                    AssetDatabase.DeleteAsset("Assets/CustomItems.prefab");
                                else
                                    return;
                            }

                            Selection.activeObject = itemObject;
                            EditorUtility.SetDirty(item);
                            EditorSceneManager.MarkSceneDirty(itemObject.scene);
                            EditorSceneManager.SaveScene(itemObject.scene);

                            GameObject customItemsGameObject = CreateCustomItemsObject(item);

                            PrefabUtility.SaveAsPrefabAsset(customItemsGameObject, "Assets/CustomItems.prefab");
                            AssetBundleBuild assetBundleBuild = default;
                            assetBundleBuild.assetNames = new string[] {
                                "Assets/CustomItems.prefab"
                            };

                            assetBundleBuild.assetBundleName = fileName;

                            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                            BuildTarget activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

                            BuildPipeline.BuildAssetBundles(Application.temporaryCachePath, new AssetBundleBuild[]{ assetBundleBuild }, 0, EditorUserBuildSettings.activeBuildTarget);
                            EditorPrefs.SetString("currentBuildingAssetBundlePath", folderPath);
                            EditorUserBuildSettings.SwitchActiveBuildTarget(selectedBuildTargetGroup, activeBuildTarget);
                            AssetDatabase.DeleteAsset("Assets/CustomItems.prefab");
                            DestroyImmediate(customItemsGameObject);

                            if (File.Exists(path))
                                File.Delete(path);

                            File.Move(Application.temporaryCachePath + "/" + fileName, path);
                            AssetDatabase.Refresh();
                            EditorUtility.DisplayDialog("Export Successful!", "Export Successful!", "OK");
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Export Failed!", "Path is invalid.", "OK");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Exportation Failed!", "Saber GameObject is missing.", "OK");
                    }
                }
                EditorGUI.EndDisabledGroup();
                GUILayout.Space(20);
            }
        }

        private GameObject CreateCustomItemsObject(ItemDescriptor item)
        {
            GameObject customItemsGameObject = new GameObject("CustomItems");
            ItemDescriptor destroyMe = null;

            if (item.isMultipleItems)
            {
                foreach (ItemData data in item.dataList)
                {
                    GameObject newItem = Instantiate(data.itemObject, customItemsGameObject.transform);
                    newItem.name = data.itemObject.name;
                }
            }
            else
            {
                GameObject newItem = Instantiate(item.gameObject, customItemsGameObject.transform);
                newItem.name = item.name;
                destroyMe = newItem.GetComponent<ItemDescriptor>();
            }

            GameObject itemSettings = new GameObject("ItemSettings");
            itemSettings.transform.parent = customItemsGameObject.transform;

            if (item.isMultipleItems)
            {
                foreach (ItemData data in item.dataList)
                {
                    GameObject itemSettingsItem = new GameObject(data.itemObject.name);
                    itemSettingsItem.transform.parent = itemSettings.transform;

                    if (item.data.hideInMenu)
                        new GameObject("HideInMenu").transform.parent = itemSettingsItem.transform;

                    GameObject category = new GameObject("Category");
                    category.transform.parent = itemSettingsItem.transform;

                    GameObject categoryNum = new GameObject(((int)data.category).ToString());
                    categoryNum.transform.parent = category.transform;

                    GameObject pool = new GameObject("PoolAmount");
                    pool.transform.parent = itemSettingsItem.transform;

                    GameObject poolAmount = new GameObject(data.poolAmount.ToString());
                    poolAmount.transform.parent = pool.transform;
                }
            }
            else
            {
                GameObject itemSettingsItem = new GameObject(item.name);
                itemSettingsItem.transform.parent = itemSettings.transform;

                if (item.data.hideInMenu)
                    new GameObject("HideInMenu").transform.parent = itemSettingsItem.transform;

                GameObject category = new GameObject("Category");
                category.transform.parent = itemSettingsItem.transform;

                GameObject categoryNum = new GameObject(((int)item.data.category).ToString());
                categoryNum.transform.parent = category.transform;

                GameObject pool = new GameObject("PoolAmount");
                pool.transform.parent = itemSettingsItem.transform;

                GameObject poolAmount = new GameObject(item.data.poolAmount.ToString());
                poolAmount.transform.parent = pool.transform;
            }

            if (destroyMe != null)
                DestroyImmediate(destroyMe);

            return customItemsGameObject;
        }

        private void OnFocus()
        {
            items = GameObject.FindObjectsOfType<ItemDescriptor>();
        }
    }
}