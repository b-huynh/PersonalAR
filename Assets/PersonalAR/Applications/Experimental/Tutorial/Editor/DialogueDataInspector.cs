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

    TextAsset apiKeyFile;

    public override void OnInspectorGUI()
    {
        apiKeyFile =
            (TextAsset)EditorGUILayout.ObjectField("API Key JSON File", apiKeyFile, typeof(TextAsset), false);

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
                GUILayout.Label($"#{i.ToString()}", GUILayout.Width(35));
                if (GUILayout.Button("Move Down", GUILayout.Width(100)))
                {
                    Swap<Dialogue>(DList, i, Math.Min(i + 1, DList.Count - 1));
                    break;
                }
                if (GUILayout.Button("Move Up", GUILayout.Width(100)))
                {
                    Swap<Dialogue>(DList, i, Math.Max(i - 1, 0));
                    break;
                }
                
                // Only allow one TTS asset to be created at a time, disable when request pending.
                GUI.enabled = www == null;
                if (GUILayout.Button("Create TTS", GUILayout.Width(100)))
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

        GUILayout.Space(20);
        if (GUILayout.Button("New Dialogue Entry"))
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

        // Create HTTP POST Request
        if (apiKeyFile.text == null)
            Debug.LogError("Missing API Key File");

        JsonTextReader reader = new JsonTextReader(new StringReader(apiKeyFile.text));
        string apiKey = "";
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.PropertyName &&
                (string)reader.Value == "google_tts_api_key")
            {
                apiKey = reader.ReadAsString();
                Debug.Log(apiKey);
            }
        }

        if (apiKey == "")
            Debug.LogError("Failed to read API Key File");
        
        string urlWithKey =
            $"https://texttospeech.googleapis.com/v1/text:synthesize?key={apiKey}";
        www = new UnityWebRequest(urlWithKey, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        www.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("charset", "utf-8");

        www.SendWebRequest();
    }
}