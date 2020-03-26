using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shape : PersistableObject
{
    [SerializeField]
    MeshRenderer[] meshRenderers = null;
    Color[] colors;
    // public Vector3 AngularVelocity { get; set; }
    // public Vector3 Velocity { get; set; }
    private void Awake() {
        colors = new Color[meshRenderers.Length];
    }
    public int ShapeID {
        set{
            if (shapeID == int.MinValue && value != int.MinValue) {
                shapeID = value;    
            }else {
                Debug.LogError("Not allowed to change shapeId.");
            }
        }
        get{
            return shapeID;
        }
    }
    int shapeID = int.MinValue;
    public ShapeFactory OriginFactory {
        get {
            return originFactory;
        }
        set {
            if (originFactory == null) {
                originFactory = value;
            } else {
                Debug.LogError("Not allowed to change origin factory");
            }
        }
    }
    ShapeFactory originFactory;

    public int MaterialID {get; private set;}
    // public Color color;
    public void SetMaterial(Material material, int materialID) 
    {
        for (int i = 0; i < meshRenderers.Length; i++) {
            meshRenderers[i].material = material;
        }
        MaterialID = materialID;
    }

    // Property Block
    static int colorPropertyID = Shader.PropertyToID("_Color");
    static MaterialPropertyBlock sharedPropertyBlock;
    public void SetColor(Color color)
    {
        if (sharedPropertyBlock == null) {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyID, color);
        for (int i = 0; i < meshRenderers.Length; i++) {
            colors[i] = color;
            meshRenderers[i].SetPropertyBlock(sharedPropertyBlock);
        }
    }
    public void SetColor(Color color, int index)
    {
        if (sharedPropertyBlock == null) {
            sharedPropertyBlock = new MaterialPropertyBlock();
        }
        sharedPropertyBlock.SetColor(colorPropertyID, color);
        colors[index] = color;
        meshRenderers[index].SetPropertyBlock(sharedPropertyBlock);
    }
    public int ColorCount {
        get {
            return colors.Length;
        }
    }
    public override void Save(GameDataWriter writer)
    {
        base.Save(writer);
        writer.Write(colors.Length);
        for (int i = 0; i < colors.Length; i++) {
            writer.Write(colors[i]);
        }
        // writer.Write(AngularVelocity);
        // writer.Write(Velocity);
        writer.Write(Age);
        writer.Write(behaviorList.Count);
        for (int i = 0; i < behaviorList.Count; i++) {
            writer.Write( (int)behaviorList[i].BehaviorType );
            behaviorList[i].Save(writer);
        }
    }
    public override void Load(GameDataReader reader)
    {
        base.Load(reader);
        if (reader.Version >= 5) {
            LoadColors(reader);
        } else {
            SetColor(reader.Version > 0 ? reader.ReadColor() : Color.white);
        }
        
        if (reader.Version >= 6) {
            Age = reader.ReadFloat();
            int behaviorCount = reader.ReadInt();
            for (int i = 0; i < behaviorCount; i++) {
                ShapeBehavior behavior = 
                    ((ShapeBehaviorType)reader.ReadInt()).GetInstance();
                behaviorList.Add(behavior);
                behavior.Load(reader);
            }
        } else if (reader.Version >= 4) {
            AddBehavior<RotationShapeBehavior>().AngularVelocity = 
                reader.ReadVector3();
            AddBehavior<MovementShapeBehavior>().Velocity = 
                reader.ReadVector3();
        }
    }
    void LoadColors(GameDataReader reader) 
    {
        int count = reader.ReadInt();
        int max = count <= colors.Length ? count : colors.Length;
        int i = 0;
        for (; i < max; i++) {
            SetColor(reader.ReadColor(), i);
        }
        if (count > max) {
            for (; i < count; i++) {
                reader.ReadColor();
            }
        } else if (count < max) {
            for (; i < max; i++) {
                SetColor(Color.white, i);
            }
        }
    }
    public void GameUpdate()
    {
        Age += Time.deltaTime;
        for (int i = 0; i < behaviorList.Count; i++) {
            if (!behaviorList[i].GameUpdate(this)) {
                behaviorList[i].Recycle();
                behaviorList.RemoveAt(i--);
            }
        }
    }
    public void Recycle()
    {
        Age = 0f;
        InstanceId += 1;
        for (int i = 0; i < behaviorList.Count; i++) {
            behaviorList[i].Recycle();
        }
        behaviorList.Clear();
        OriginFactory.Reclaim(this);
    }

    List<ShapeBehavior> behaviorList = new List<ShapeBehavior>();
    public T AddBehavior<T>() where T : ShapeBehavior, new()
    {
        T behavior = ShapeBehaviorPool<T>.Get();
        behaviorList.Add(behavior);
        return behavior;
    }

    public float Age { get; private set; }
    public int InstanceId { get; private set; }
    public int SaveIndex { get; set; }

    public void ResolveShapeInstances()
    {
        for (int i = 0; i < behaviorList.Count; i++) {
            behaviorList[i].ResolveShapeInstances();
        }
    }
    public void Die()
    {
        Game.Instance.Kill(this);
    }

    public void MarkAsDying()
    {
        Game.Instance.MarkAsDying(this);
    }
    public bool IsMarkedAsDying {
        get {
            return Game.Instance.IsMarkedAsDying(this);
        }
    }
}

