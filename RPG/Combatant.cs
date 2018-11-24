using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
	abstract class Combatant
	{
		protected int maxHealth;
		public int health;

		public abstract void TakeDamage(int damage, double combatTimer);
		public abstract bool IsDone(GameTime gameTime, double combatTimer, KeyboardState prevState);//to be used as an Update()-equivalent that returns true only when an animation (ex. flash) is finished
		public abstract void ForceFinish();

	}
}
