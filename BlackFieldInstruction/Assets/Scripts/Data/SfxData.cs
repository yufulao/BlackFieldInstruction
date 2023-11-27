using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SfxData", menuName = "ScriptableObjects/SfxData", order = 1)]
public class SfxData : ScriptableObject
{
	[Serializable]
	public class SFXDataEntry
	{
		public string name;

		public List<string> maudioClipPaths;

		[Range(0f, 1f)]
		public float volume;

		public bool oneShot;
	}

	public List<SFXDataEntry> data;

	public enum Flow
    {
		EditSlotBattleSlot,
		Slot,
		EditBattle,
		EditSlotBattle,
		EditBattleSlot
    }
}
