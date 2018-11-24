using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
	class Selector
	{
		private bool horizontal;
		private int current;
		private int length;

		public Selector(int length, bool horizontal = true)
		{
			this.length = length;
			this.horizontal = horizontal;
			current = 0;
		}

		public void Update(KeyboardState prevState)
		{
			if(Keyboard.GetState().IsKeyDown(Keys.Left) && prevState.IsKeyUp(Keys.Left))
			{
				if (current-- == 0)
					current = length-1;
			}
			else if (Keyboard.GetState().IsKeyDown(Keys.Right) && prevState.IsKeyUp(Keys.Right))
			{
				if (++current == length)
					current = 0;
			}
		}

		public int GetIndex()
		{
			return current;
		}
	}
}
