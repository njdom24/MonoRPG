using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
	class Enemy : Combatant
	{
		private Body body;
		private Texture2D sprite;
		private bool visible;
		private double moveTimer;
		private int offsetTop;
		private int offsetBottom;
		private float lastForce;
		private double moveDuration;
		private double secondsPerBeat;
		private double threshHold;
		private bool noteHit;
		private int noteCount;

		public Enemy(Texture2D sprite, World world, double secondsPerBeat, double threshHold = 0, int offsetTop = 0, int offsetBottom = 0)
		{
			noteHit = true;//default for note hit at start
			noteCount = 0;
			this.secondsPerBeat = secondsPerBeat;
			this.threshHold = threshHold;
			moveDuration = 0.05;
			lastForce = 400;
			this.offsetTop = offsetTop;
			this.offsetBottom = offsetBottom;
			body = new Body(world, new Vector2((400 - sprite.Width) / 2, (240 - offsetBottom + offsetTop - sprite.Height) / 2));
			body.SetTransform(new Vector2((400 - sprite.Width) / 2, (240 - offsetBottom + offsetTop - sprite.Height) / 2), 0);
			body.BodyType = BodyType.Dynamic;
			body.Mass = 0.1f;
			this.sprite = sprite;
			health = 100;
			visible = true;
			moveTimer = 0.05;
		}

		public void Draw(SpriteBatch sb, int offsetTop = 0, int offsetBottom = 0)
		{
			if(visible)
				sb.Draw(sprite, new Rectangle((int)body.Position.X, (int)body.Position.Y, sprite.Width, sprite.Height), Color.White);
			//sb.Draw(sprite, new Rectangle((400 - sprite.Width) / 2, (240 - offsetBottom + offsetTop - sprite.Height) / 2, sprite.Width, sprite.Height), Color.White);
		}

		public override void ForceFinish()
		{
			lastForce = 400;
			moveTimer = moveDuration;
			body.ResetDynamics();
			body.SetTransform(new Vector2((400 - sprite.Width) / 2, (240 - offsetBottom + offsetTop - sprite.Height) / 2), 0);
		}

		private void checkAttack()
		{

		}

		public override bool IsDone(GameTime gameTime, double combatTimer, KeyboardState prevState)//TODO: Check for multiple hits per beat
		{
			if (noteCount == 16)
				return true;

			moveTimer -= gameTime.ElapsedGameTime.TotalSeconds;
			
			Console.WriteLine(lastForce);
			if (combatTimer > secondsPerBeat + threshHold)//if time window ends
			{
				if (noteHit)
					noteHit = false;
				else
					return true;
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Space) && prevState.IsKeyUp(Keys.Space))
			{
				if (combatTimer > secondsPerBeat - threshHold)//during time window
				{
					if (noteHit)
						return true;

					AdditionalDamage();
				}
				else//exit when a beat is missed
					return true;
			}

			//if (timer < 0)
			//return true;

			if (Math.Abs(lastForce) < 20)
			{
				ForceFinish();
				lastForce = 0;
			}
			else
				if (moveTimer < 0)
			{
				Jostle(-0.75f);
				//moveDuration = moveDuration / 1.2;
				moveTimer = moveDuration;
			}

			

			return false;
		}

		public override void TakeDamage(int damage, double combatTimer)
		{
			Jostle(-2.5f);
			//body.ApplyForce(new Vector2(lastForce,0));
			//body.LinearVelocity = new Vector2(10, 10);
			health -= damage;
			//visible = false;
			noteHit = (combatTimer > secondsPerBeat - threshHold);
			if (noteHit)
				noteCount++;
		}

		private void AdditionalDamage()
		{
			ForceFinish();
			Jostle(-1.5f);
			health--;
			noteHit = true;
			noteCount++;
		}

		private void Jostle(float multiplier)
		{
			body.ResetDynamics();
			body.ApplyForce(new Vector2(lastForce, 0));
			lastForce = lastForce * multiplier;
		}
	}
}
