using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class GameSettings
{

    public float SoundVolume;
    public float MusicVolume;
    public int ResolutionIndex;
    public bool FullScreen;


    public GameSettings()
    {

    }

    public GameSettings(float SoundVolume, float MusicVolume, int ResolutionIndex, bool FullScreen)
    {
        this.SoundVolume = SoundVolume;
        this.MusicVolume = MusicVolume;
        this.ResolutionIndex = ResolutionIndex;
        this.FullScreen = FullScreen;
    }

    public void SaveFile()
    {
        StreamWriter writer = null;

        XmlSerializer serializer = new XmlSerializer(typeof(GameSettings));
        writer = new StreamWriter(Application.persistentDataPath + "/GameSettings.xml");


        serializer.Serialize(writer.BaseStream, new GameSettings(SoundVolume, MusicVolume, ResolutionIndex, FullScreen));

        writer.Close();
    }

    public GameSettings LoadFile()
    {
        string path = Application.persistentDataPath + "/GameSettings.xml";

        //Debug.Log(path);

        if (!File.Exists(path))
        {
            //Create file
            Debug.Log("No file");
            SaveFile();
            Debug.Log("File was created");

        }



        XmlSerializer serializer = new XmlSerializer(typeof(GameSettings));
        StreamReader reader = new StreamReader(path);

        GameSettings deserialized;

        try
        {
            deserialized = serializer.Deserialize(reader.BaseStream) as GameSettings;
        }
        catch
        {
            reader.Close();
            SaveFile();

            reader = new StreamReader(path);
            deserialized = serializer.Deserialize(reader.BaseStream) as GameSettings;
        }


        reader.Close();
        return deserialized;
        
    }
}
