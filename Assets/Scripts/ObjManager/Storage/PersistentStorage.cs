﻿using UnityEngine;
using System.IO;

public class PersistentStorage : MonoBehaviour
{
    string savePath;
    private void Awake() {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }
    public void Save(PersistableObject obj, int version)
    {
        using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create))) {
            writer.Write(-version);
            obj.Save(new GameDataWriter(writer));
        }
    }
    public void Load(PersistableObject obj)
    {
        // using(var reader = new BinaryReader(File.Open(savePath, FileMode.Open))) {
        //     obj.Load(new GameDataReader(reader, -reader.ReadInt32()));
        // }
        byte[] data = File.ReadAllBytes(savePath);
        var reader = new BinaryReader(new MemoryStream(data));
        obj.Load(new GameDataReader(reader, -reader.ReadInt32()));
    }
}
