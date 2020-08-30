using StressLevelZero.Data;
using System;
using UnityEngine;

namespace ItemExporter
{
    [Serializable]
    public struct ItemData
    {
        public GameObject itemObject;
        public bool hideInMenu;
        public CategoryFilters category;

        [Range(2, 1000)]
        public int poolAmount;
    }

}