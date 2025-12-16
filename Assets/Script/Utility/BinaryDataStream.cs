using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BinaryDataStream
{
    public static void Save<T>(T SerializableObject, string filename)
    {
        string path = Application.persistentDataPath + "/saves/" ;
        Directory.CreateDirectory(path);

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = new FileStream(path + filename + ".dat", FileMode.Create);

        try
        {
            formatter.Serialize(fileStream, SerializableObject);
        }
        catch(SerializationException e)
        {
            Debug.Log("save filed error " + e.Message);
        }
        finally
        {
            fileStream.Close();
        }
    }

    public static bool Exists(string filename)
    {
        string path = Application.persistentDataPath + "/saves/";
        string fullFileName = filename + ".dat";
        return File.Exists(path + fullFileName);
    }

    public static T Read<T>(string filename)
    {
        string path = Application.persistentDataPath + "/saves/";
        FileStream fileStream = new FileStream(path + filename + ".dat", FileMode.Open);

        T returnType = default(T);
        BinaryFormatter formatter = new BinaryFormatter();

        try
        {
            returnType = (T)formatter.Deserialize(fileStream);
        }
        catch(SerializationException e)
        {
            Debug.Log("load filed error " + e.Message);
        }
        finally
        {
            fileStream.Close();
        }

        return returnType;
    }
}
