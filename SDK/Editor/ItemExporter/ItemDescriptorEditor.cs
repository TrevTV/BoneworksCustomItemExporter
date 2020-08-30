using UnityEditor;
using UnityEngine;

namespace ItemExporter
{
    [CustomEditor(typeof(ItemDescriptor))]
    [CanEditMultipleObjects]
    public class ItemDescriptorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ItemDescriptor descriptor = (ItemDescriptor)target;

            descriptor.isMultipleItems = EditorGUILayout.Toggle("Is Multiple Items", descriptor.isMultipleItems);

            if (!descriptor.isMultipleItems)
            {
                SerializedObject serializedObject = new SerializedObject(target);
                SerializedProperty property = serializedObject.FindProperty("data");
                serializedObject.Update();
                EditorGUILayout.PropertyField(property, true);
                serializedObject.ApplyModifiedProperties();
                descriptor.data.itemObject = descriptor.gameObject;
            }
            else
            {
                SerializedObject serializedObject = new SerializedObject(target);
                SerializedProperty property = serializedObject.FindProperty("dataList");
                serializedObject.Update();
                EditorGUILayout.PropertyField(property, true);
                serializedObject.ApplyModifiedProperties();

                /*for (int i = 0; i < descriptor.transform.childCount; i++)
                {
                    descriptor.dataList.Add(new ItemData() { itemObject = descriptor.transform.GetChild(i).gameObject });
                }*/
            }
        }
    }

}