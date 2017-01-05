using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadButton : MonoBehaviour 
{
	public void Load(string name) {
		StartCoroutine(waitBeforeLoading(name, 0.3f));
	}

	private IEnumerator waitBeforeLoading(string name, float time) {
		yield return new WaitForSeconds(time);
		SceneManager.LoadScene (name);
	}
}