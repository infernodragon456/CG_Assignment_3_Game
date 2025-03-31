#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateSampleUnits : MonoBehaviour
{
    [MenuItem("TABS Game/Create Sample Units")]
    public static void CreateUnits()
    {
        CreateUnitFolder();
        CreatePrefabsFolder();
        
        // Create a base unit prefab to use for all units
        GameObject basePrefab = CreateBasePrefab();
        
        if (basePrefab == null)
        {
            Debug.LogError("Failed to create base prefab!");
            return;
        }
        
        // Create some sample units
        CreateWarriorUnit(basePrefab);
        CreateArcherUnit(basePrefab);
        CreateGiantUnit(basePrefab);
        CreateSpeedsterUnit(basePrefab);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("Sample units created in Assets/Units/");
    }
    
    private static void CreateUnitFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Units"))
        {
            AssetDatabase.CreateFolder("Assets", "Units");
        }
    }
    
    private static void CreatePrefabsFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
    }
    
    private static GameObject CreateBasePrefab()
    {
        // Create a simple unit prefab (capsule)
        GameObject unitObj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        unitObj.name = "BaseUnitPrefab";
        
        // Add required components
        unitObj.AddComponent<Unit>();
        unitObj.AddComponent<UnityEngine.AI.NavMeshAgent>();
        
        // Configure NavMeshAgent
        var agent = unitObj.GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = 3.5f;
        agent.angularSpeed = 120f;
        agent.acceleration = 8f;
        agent.stoppingDistance = 1f;
        
        // Save the prefab
        string prefabPath = "Assets/Prefabs/BaseUnitPrefab.prefab";
        
        // Create the prefab
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(unitObj, prefabPath);
        
        // Destroy the temp GameObject
        Object.DestroyImmediate(unitObj);
        
        return prefab;
    }
    
    private static void CreateWarriorUnit(GameObject basePrefab)
    {
        UnitData warrior = ScriptableObject.CreateInstance<UnitData>();
        warrior.unitName = "Warrior";
        warrior.description = "A balanced fighter with decent health and damage.";
        warrior.cost = 100;
        warrior.health = 100;
        warrior.damage = 15;
        warrior.attackRange = 2;
        warrior.attackSpeed = 1;
        warrior.moveSpeed = 3.5f;
        warrior.unitPrefab = basePrefab; // Assign the prefab
        
        AssetDatabase.CreateAsset(warrior, "Assets/Units/Warrior.asset");
    }
    
    private static void CreateArcherUnit(GameObject basePrefab)
    {
        UnitData archer = ScriptableObject.CreateInstance<UnitData>();
        archer.unitName = "Archer";
        archer.description = "Long-range attacker with lower health but high damage.";
        archer.cost = 150;
        archer.health = 70;
        archer.damage = 25;
        archer.attackRange = 15;
        archer.attackSpeed = 0.7f;
        archer.moveSpeed = 4;
        archer.unitPrefab = basePrefab; // Assign the prefab
        
        AssetDatabase.CreateAsset(archer, "Assets/Units/Archer.asset");
    }
    
    private static void CreateGiantUnit(GameObject basePrefab)
    {
        UnitData giant = ScriptableObject.CreateInstance<UnitData>();
        giant.unitName = "Giant";
        giant.description = "Slow but very powerful unit with high health and damage.";
        giant.cost = 350;
        giant.health = 250;
        giant.damage = 50;
        giant.attackRange = 3;
        giant.attackSpeed = 0.5f;
        giant.moveSpeed = 2;
        giant.unitPrefab = basePrefab; // Assign the prefab
        
        AssetDatabase.CreateAsset(giant, "Assets/Units/Giant.asset");
    }
    
    private static void CreateSpeedsterUnit(GameObject basePrefab)
    {
        UnitData speedster = ScriptableObject.CreateInstance<UnitData>();
        speedster.unitName = "Speedster";
        speedster.description = "Fast attacker with low health but rapid strikes.";
        speedster.cost = 125;
        speedster.health = 60;
        speedster.damage = 10;
        speedster.attackRange = 1.5f;
        speedster.attackSpeed = 2f;
        speedster.moveSpeed = 6;
        speedster.unitPrefab = basePrefab; // Assign the prefab
        
        AssetDatabase.CreateAsset(speedster, "Assets/Units/Speedster.asset");
    }
}
#endif 