using System;
using System.Collections;
using System.Collections.Generic;
// using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

[CustomEditor(typeof(DialogueData))]
public class DialogueDataInspector : Editor
{
    UnityWebRequest www;
    Dialogue audioClipRecipient;
    int recipientIndex;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        DialogueData dialogueData = (DialogueData)target;

        // Draw list of dialogues
        GUILayout.Label("Dialogues");
        SerializedProperty DSO = serializedObject.FindProperty("Dialogues");
        List<Dialogue> DList = dialogueData.Dialogues;
        for(int i = 0; i < DSO.arraySize; ++i)
        {
            // Draw single dialogue listing
            GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
                
                // Draw additional butotns
                GUILayout.BeginHorizontal();
                GUILayout.Label($"# {i.ToString()}", GUILayout.Width(25));
                if (GUILayout.Button("+", GUILayout.Width(25)))
                {
                    Swap<Dialogue>(DList, i, Math.Min(i + 1, DList.Count - 1));
                    break;
                }
                if (GUILayout.Button("-", GUILayout.Width(25)))
                {
                    Swap<Dialogue>(DList, i, Math.Max(i - 1, 0));
                    break;
                }
                
                // Only allow one TTS asset to be created at a time, disable when request pending.
                GUI.enabled = www == null;
                if (GUILayout.Button("Create TTS", GUILayout.Width(75)))
                {
                    audioClipRecipient = DList[i];
                    recipientIndex = i;
                    GenerateTTSAudioClip(audioClipRecipient.text);
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();

            // Draw dialogue properties
            SerializedProperty sp = DSO.GetArrayElementAtIndex(i);
            SerializedProperty textProp = sp.FindPropertyRelative("text");
            SerializedProperty audioProp = sp.FindPropertyRelative("audioClip");
            EditorGUILayout.PropertyField(textProp, true);
            EditorGUILayout.PropertyField(audioProp, true);
            
            // Apply property changes
            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
            GUILayout.EndVertical();
        }

        if (GUILayout.Button("New Dialogue"))
        {
            dialogueData.Dialogues.Add(new Dialogue());
        }

        // Poll any pending web requests
        if (www?.isDone ?? false)
        {
            // Decode response bytes
            JObject responseData = JObject.Parse(www.downloadHandler.text);
            string encodedText = responseData["audioContent"].ToString();
            byte[] decodedBytes = Convert.FromBase64String(encodedText);

            // RESET www polling
            www = null;

            // Write bytes to MP3 file
            // string AudioClipFolder = Path.Combine(Application.dataPath, "PersonalAR", "Applications", "Experimental", "Tutorial");
            string AudioClipFolder = Path.GetDirectoryName(AssetDatabase.GetAssetPath(target.GetInstanceID()));
            string filename = $"Dialogue {recipientIndex} [{target.name}].mp3";
            string fullPath = Path.GetFullPath(Path.Combine(AudioClipFolder, filename));
            Debug.Log(fullPath);
            File.WriteAllBytes(fullPath, decodedBytes);

            // Import asset and set references
            AssetDatabase.Refresh();
            string assetPath = Path.Combine(AudioClipFolder, filename);
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
            audioClipRecipient.audioClip = clip;

            Debug.Log("TTS Asset Created.");
        }
    }

    public static void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    public void GenerateTTSAudioClip(string text)
    {
        // Create Request Body JSON
        StringBuilder sb = new StringBuilder();
        StringWriter sw = new StringWriter(sb);
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            writer.Formatting = Formatting.Indented;

            writer.WriteStartObject();
            writer.WritePropertyName("input");
                writer.WriteStartObject();
                writer.WritePropertyName("text");
                writer.WriteValue(text);
                // writer.WriteValue(
                //     @"<speak>
                //         Here are <say-as interpret-as=""characters"">SSML</say-as> samples.
                //         I can pause <break time=""3s""/>.
                //         I can speak in cardinals. Your number is <say-as interpret-as=""cardinal"">10</say-as>.
                //         Or I can speak in ordinals. You are <say-as interpret-as=""ordinal"">10</say-as> in line.
                //         Or I can even speak in digits. The digits for ten are <say-as interpret-as=""characters"">10</say-as>.
                //         I can also substitute phrases, like the <sub alias=""World Wide Web Consortium"">W3C</sub>.
                //         Finally, I can speak a paragraph with two sentences.
                //         <p><s>This is sentence one.</s><s>This is sentence two.</s></p>
                //     </speak>"
                // );
                writer.WriteEndObject();
            writer.WritePropertyName("voice");
                writer.WriteStartObject();
                writer.WritePropertyName("languageCode");
                writer.WriteValue("en-gb");
                writer.WritePropertyName("name");
                writer.WriteValue("en-GB-Standard-A");
                writer.WritePropertyName("ssmlGender");
                writer.WriteValue("FEMALE");
                writer.WriteEndObject();
            writer.WritePropertyName("audioConfig");
                writer.WriteStartObject();
                writer.WritePropertyName("audioEncoding");
                writer.WriteValue("MP3");
                writer.WriteEndObject();
            writer.WriteEndObject();
        }
        string bodyJsonString = sb.ToString();

        Debug.Log($"Sending... \n{bodyJsonString}");

        // Create HTTP POST Request
        string urlWithKey = "https://texttospeech.googleapis.com/v1/text:synthesize?key=AIzaSyA8EeaT0fYdmdQ0UrcDoMYQ9O0PkYqtlT0";
        www = new UnityWebRequest(urlWithKey, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        www.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("charset", "utf-8");

        www.SendWebRequest();
    }
}