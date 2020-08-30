using UnityEngine;
using System.Collections.Generic;

namespace ItemExporter
{
    public class ItemDescriptor : MonoBehaviour
    {
        public bool isMultipleItems;
        //public string itemName;
        public ItemData data;
        public List<ItemData> dataList = new List<ItemData>();
    }
}