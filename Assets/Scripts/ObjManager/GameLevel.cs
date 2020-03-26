using UnityEngine;

public partial class GameLevel : PersistableObject
{
    [SerializeField]
    int populationLimit = int.MinValue;
    [SerializeField]
    SpawnZone spwanZone = null;

    [UnityEngine.Serialization.FormerlySerializedAs("persistableObjects")]
    [SerializeField]
    GameLevelObject[] levelObjects;

    public int PopulationLimit {
        get {
            return populationLimit;
        }
    }
    public static GameLevel Current{ get; private set;}
    
    public void SpawnShapes()
    {
        spwanZone.SpawnShapes();
    }

    private void OnEnable() 
    {
        Current = this;
        if (levelObjects == null) {
            levelObjects = new GameLevelObject[0];
        }
    }
    public override void Save(GameDataWriter writer)
    {
        writer.Write(levelObjects.Length);
        for (int i = 0; i < levelObjects.Length; i++) {
            levelObjects[i].Save(writer);
        }
    }
    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for (int i = 0; i < savedCount; i++) {
            levelObjects[i].Load(reader);
        }
    }
    public void GameUpdate()
    {
        for (int i = 0; i < levelObjects.Length; i++) {
            levelObjects[i].GameUpdate();
        }
    }
}
