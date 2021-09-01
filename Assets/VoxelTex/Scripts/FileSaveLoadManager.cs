using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class FileSaveLoadManager
{
    public static byte[] Serialize(System.Object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (MemoryStream ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    public static T Deserialize<T>(byte[] arrBytes)
    {
        using (MemoryStream memStream = new MemoryStream())
        {
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            return (T)binForm.Deserialize(memStream);
        }
    }

    public static void SaveFile(byte[] binaryData, string saveLoaction, string fileName, string format)
    {
        System.IO.File.WriteAllBytes($"{saveLoaction}/{fileName}.{format}", binaryData);
    }

    public static byte[] LoadFile(string saveLoaction, string fileName, string format)
    {
        return System.IO.File.ReadAllBytes($"{saveLoaction}/{fileName}.{format}");
    }
}
