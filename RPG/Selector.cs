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
		private Keys increment, decrement;
		private int current;
		private int length;
		private int lastFrameIndex;

		public Selector(int length, bool horizontal = true)
		{
			this.length = length;
			if(horizontal)
			{
				increment = Keys.Right;
				decrement = Keys.Left;
			}
			else
			{
				increment = Keys.Up;
				decrement = Keys.Down;
			}
			current = 0;

			lastFrameIndex = 0;
		}

		public void Update(KeyboardState prevState)
		{
			lastFrameIndex = current;

			if(Keyboard.GetState().IsKeyDown(decrement) && prevState.IsKeyUp(decrement))
			{
				if (current-- == 0)
					current = length-1;
			}
			else if (Keyboard.GetState().IsKeyDown(increment) && prevState.IsKeyUp(increment))
			{
				if (++current == length)
					current = 0;
			}
		}

		public int GetIndex()
		{
			return current;
		}

		public bool IndexChanged()
		{
			Console.WriteLine(lastFrameIndex);
			return (lastFrameIndex != current);
		}
	}
}
