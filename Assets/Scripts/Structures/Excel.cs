using System.Collections.Generic;
using Config;
using UnityEngine;

public class Excel
{
    public BuildingItemFormat[] building;
    public List<BuildingItemFormat> building0 = new List<BuildingItemFormat>();
    public List<BuildingItemFormat> building1 = new List<BuildingItemFormat>();
    public List<BuildingItemFormat> building2 = new List<BuildingItemFormat>();
    public List<BuildingItemFormat> building3 = new List<BuildingItemFormat>();
    public ArmyItemFormat[] army;
    public ArmyItemFormat[] enemy;
    public AdventureItemFormat[] adventure;
    public HeroItemFormat[] hero;

    public void RevertDictionary()
    {
        Dictionary<int, BuildingItemFormat> buildingByIndex = new Dictionary<int, BuildingItemFormat>();
        foreach (BuildingItemFormat item in building) buildingByIndex.Add(item.index, item);

        // building 排序
        foreach (BuildingItemFormat item in building)
        {
            var len = building.Length;
            for (var gap = Mathf.FloorToInt(len / 2f); gap > 0; gap = Mathf.FloorToInt(gap / 2f))
            {
                for (var i = gap; i < len; i++)
                {
                    var j = i;
                    var current = building[i];
                    while (j - gap >= 0 && current.sequence < building[j - gap].sequence)
                    {
                        building[j] = building[j - gap];
                        j = j - gap;
                    }

                    building[j] = current;
                }
            }
        }
        // building 归类
        foreach (BuildingItemFormat item in building)
        {
            switch (item.category)
            {
                case 0:
                    building0.Add(item);
                    break;
                case 1:
                    building1.Add(item);
                    break;
                case 2:
                    building2.Add(item);
                    break;
                case 3:
                    building3.Add(item);
                    break;
            }
            item.RevertCost();
        }
        foreach (var i in buildingByIndex) building[i.Key] = i.Value;
        
        foreach (ArmyItemFormat item in army)
        {
            item.RevertCost();
        }
        foreach (ArmyItemFormat item in enemy)
        {
            item.RevertCost(true);
        }
        foreach (AdventureItemFormat item in adventure)
        {
            item.RevertArmy();
        }
        foreach (var item in hero)
        {
            item.avatar = Resources.Load<Sprite>(item.path);
        }
    }
}
