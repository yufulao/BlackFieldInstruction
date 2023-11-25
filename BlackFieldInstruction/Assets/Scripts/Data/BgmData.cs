using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BgmData", menuName = "ScriptableObjects/BgmData", order = 1)]
public class BgmData : ScriptableObject
{
	[Serializable]
	public class BGMDataEntry
	{
		public string name;

		public AudioClip audioClip;

	}

	public List<BGMDataEntry> data;
}