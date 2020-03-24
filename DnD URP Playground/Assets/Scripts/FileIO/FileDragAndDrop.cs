using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using B83.Win32;


public class FileDragAndDrop : MonoBehaviour
{

    ImportObjectFromFile importer;
    NetworkHandler network;

    List<string> log = new List<string>();
    void OnEnable ()
    {
        // must be installed on the main thread to get the right thread id.
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    }

    private void Start()
    {
        //Todo: this is clunky, fix with more appropriate references
        importer = FindObjectOfType<ImportObjectFromFile>();
        network = FindObjectOfType<NetworkHandler>();
    }
    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        // do something with the dropped file names. aPos will contain the 
        // mouse position within the window where the files has been dropped.

        string str = "Dropped " + aFiles.Count + " files at: " + aPos + "\n\t" +
            aFiles.Aggregate((a, b) => a + "\n\t" + b);
        Debug.Log(str);
        log.Add(str);

        foreach (string filePath in aFiles)
        {
            string[] subs = filePath.Split(new char[] { '.' });
            string ext = subs[subs.Length - 1];

            if (ext == "obj")
            {
                importer.TryImportFile(filePath);
            }
            else if(ext == "js")
            {
                //network.OnServerFileDropped(filePath);
            }
            else
            {
                Debug.LogError("File not supported. Only upload OBJ plz");
            }
        }
    }

    private void OnGUI()
    {
        if (GUILayout.Button("clear log"))
            log.Clear();
        foreach (var s in log)
            GUILayout.Label(s);
    }
}
