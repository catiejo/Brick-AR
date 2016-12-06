using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadButton : MonoBehaviour 
{
	public void Load(string name) {
		SceneManager.LoadScene (name);
	}
}