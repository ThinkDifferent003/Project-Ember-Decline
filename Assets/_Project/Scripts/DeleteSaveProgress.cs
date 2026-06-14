using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
public class DeleteSaveProgress 
{
    [MenuItem("Salvataggi/Elimina File")]
    public static void DeleteFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "game_save.json");
        if (File.Exists(path)) File.Delete(path);
    }
}
