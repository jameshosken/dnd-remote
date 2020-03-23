using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StartNodeServer : MonoBehaviour
{
    Process process;
    public bool StartServerProcess(string filePath)
    {
    
        try
        {
            
            string startCommand = @"node '" + filePath + "'";
            UnityEngine.Debug.LogWarning(startCommand);

            process = new Process();

            process.StartInfo.FileName = "powershell.exe";
            process.StartInfo.Arguments = startCommand;

            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            process.StartInfo.UseShellExecute = true;

            bool success = process.Start(); //Start cmd process


            return success;
            //process.WaitForInputIdle();

        }
        catch (System.Exception e)
        {
            print(e);
        }

        return false;
    }

    private void OnDestroy()
    {
        //process.Kill();
    }
    private void OnApplicationQuit()
    {
        //process.Close();
    }
}


/*Archive
 * 
            StreamReader reader = new StreamReader(jsFilePath);
            string jsFileString = "";

            while (!reader.EndOfStream)
            {
                jsFileString += reader.ReadLine();
            }

            reader.Close();

            jsFileString = "console.log('hello');";
            UnityEngine.Debug.Log("File Path:");
            UnityEngine.Debug.Log(jsFilePath);



            UnityEngine.Debug.Log("JS File Contents:");
            UnityEngine.Debug.Log(jsFileString);

 * */
