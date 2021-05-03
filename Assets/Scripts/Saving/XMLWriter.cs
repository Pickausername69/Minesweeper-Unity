using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

public static class XMLWriter
{
    public static bool SerializeObject<T>(T obj, string filename)
    {
        string folder= "Minesweeper_Solver";

        if (!typeof(T).IsSerializable) { return false; }

        string folderpath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//" + folder;
        string path = folderpath + "//" + filename + ".xml";
        if (!File.Exists(path))
        { File.Create(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "//" + folder); }

        XmlSerializer xmls = new XmlSerializer(typeof(T));

        System.IO.FileStream file = System.IO.File.Create(path);
        try
        {
            xmls.Serialize(file, obj);
            file.Close();
        }
        catch (Exception e) { return false; }
        return true;
    }
}
