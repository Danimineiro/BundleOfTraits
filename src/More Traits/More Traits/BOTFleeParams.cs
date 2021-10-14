using System.Collections.Generic;
using Verse;

namespace More_Traits
{
	public struct BOTFleeParams
	{
		public Thing Threat { get; set; }
		public IntVec2 Distance { get; set; }
		public List<Thing> Threats { get; set; }
		public bool StayWhenNowhereToGo { get; set; }
	}
}
