using System.IO;
using UnityEngine;
using System.Collections;

public class TruckPosition : MonoBehaviour 
{

  StreamWriter writer;
  StreamReader reader;
  [SerializeField] private string filePath = "";
  [SerializeField] private string setupFileText = "";
  [SerializeField] private string readFileText = "";

	void Start () 
  {
    writer = new StreamWriter(filePath);
    writer.Write(setupFileText);
    writer.Close();
    Read();
	}

  void Read()
  {
    if (File.Exists(filePath))
    {
      reader = new StreamReader(filePath);
      readFileText = reader.ReadToEnd();
      reader.Close();
    }
  }
}
