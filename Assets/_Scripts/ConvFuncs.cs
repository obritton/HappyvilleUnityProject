using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine;

public class ConvFuncs {

	public static void setRandomSkin( SkeletonAnimation skelAnim )
	{
		List<Skin> skins = skelAnim.state.Data.SkeletonData.Skins;
		Skin randomSkin = skins [Random.Range(1, skins.Count)];
		skelAnim.skeleton.SetSkin (randomSkin);
	}
}
