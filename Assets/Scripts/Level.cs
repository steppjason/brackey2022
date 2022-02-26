using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
	private string _musicTrack;
	
	public string MusicTrack{
		get{
			return _musicTrack;
		}
		set{
			this._musicTrack = value;
		}
	}
}
