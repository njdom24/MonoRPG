using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
	class Animation//Assumes the standing animation is the second frame of the walking animation
	{
		private int start;
		private int end;
		public int offset;

		private int curFrame;

		public Animation(int start, int end, int offset = 0)
		{
			this.start = start;
			this.end = end;
			curFrame = end;
			this.offset = offset;
		}

		public void advanceFrame()
		{
			curFrame++;
			if (curFrame > end)
				curFrame = start;
		}

		public int getFrame()
		{
			return curFrame;
		}

		public void resetStart()
		{
			curFrame = start;
		}

		public void resetEnd()
		{
			curFrame = end;
		}
	}
}
