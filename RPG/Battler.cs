using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG
{
	class Battler : Combatant
	{
		private Texture2D portrait;
		private Texture2D text;
		private Texture2D scrollingNums;

		private int posX, posY;
		private Body body;
		private float lastForce;
		private double fullTimer;
		private double moveTimer;
		private string name = "Travis";
		private int[] lengthRef;
		private int[] letterPos;
		private Point[] locations;
		private int nameOffset;

		private int numY;
		private double numTimer;
		private int rollingHealth;
		private int healthCounter;

		public Battler(ContentManager contentManager, World world)
		{
			lengthRef = new int[] { 2, 2, 3, 2, 5, 9, 7, 2, 3, 3, 3, 5, 2, 2, 2, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 2, 3, 3, 5, 3, 4, 5, 6, 5, 5, 5, 4, 4, 5, 5, 1, 4, 5, 4, 7, 5, 5, 5, 5, 5, 5, 5, 5, 6, 7, 5, 5, 4, 5, 4, 6, 4, 5, 1, 4, 4, 4, 4, 4, 3, 4, 4, 1, 2, 4, 1, 7, 4, 4, 4, 4, 3, 4, 3, 4, 5, 7, 4, 4, 4, 2, 5, 2, 6, 7, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 };
			letterPos = new int[name.Length];
			locations = new Point[name.Length];
			health = 0;
			rollingHealth = health;

			portrait = contentManager.Load<Texture2D>("Battle/Player");
			text = contentManager.Load<Texture2D>("Textbox/Text");
			scrollingNums = contentManager.Load<Texture2D>("Battle/Numbers/ScrollingNums");//5x8
			posX = Game1.width/2 -61/2;
			posY = Game1.height - 55;
			//posY = Game1.height / 2;
			Console.WriteLine(posX + ", " + posY);
			body = new Body(world, ConvertUnits.ToSimUnits(posX, posY));
			body.BodyType = BodyType.Dynamic;
			body.IgnoreGravity = true;
			lastForce = 130f;

			int nameWidth = 0;

			for(int i = 0; i < name.Length; i++)
			{
				char letter = name.ElementAt(i);
				int line, off;
				line = (letter - ' ') / 16;
				off = (letter - ' ') % 16;
				locations[i] = new Point(16 * off, 16 * line);
				nameWidth += lengthRef[name.ElementAt(i) - ' '] + 1;
				if (i < name.Length - 1)
				{
					letterPos[i+1] = lengthRef[name.ElementAt(i) - ' '] + 1;
					if (i > 0)
						letterPos[i+1] += letterPos[i];
				}
			}
			nameWidth--;
			nameOffset = (61 - nameWidth) / 2;
			//nameWidth = letterPos[letterPos.Length - 1];
			Console.WriteLine("NameWidth: " + nameWidth);
		}
		public override void ForceFinish()
		{
			lastForce = 130f;
			moveTimer = 0;
			body.ResetDynamics();
			body.SetTransform(ConvertUnits.ToSimUnits(posX, posY), 0);
		}

		public override bool IsDone(GameTime gameTime, double combatTimer, KeyboardState prevState)
		{
			fullTimer += gameTime.ElapsedGameTime.TotalSeconds;
			if (fullTimer > 1)
			{
				fullTimer = 0;
				ForceFinish();
				return true;
			}
			else
			{
				moveTimer += gameTime.ElapsedGameTime.TotalSeconds;

				if(moveTimer > 0.1f)
				{
					lastForce *= -0.9f;
					body.ResetDynamics();
					body.ApplyForce(new Vector2(0, lastForce));
					
					//body.LinearVelocity = -body.LinearVelocity;
					moveTimer = 0;
				}
				/*
				if (Math.Abs(lastForce) < 0.2)
				{
					ForceFinish();
					lastForce = 0;
				}
				else if (moveTimer > 0.05)
				{
					Jostle(-0.75f);
					//moveDuration = moveDuration / 1.2;
					moveTimer = 0;
				}
				*/
			}
			//throw new NotImplementedException();
			return false;
		}

		public override void TakeDamage(int damage, double combatTimer)
		{

			body.ResetDynamics();
			body.ApplyForce(new Vector2(0, lastForce));
			//body.LinearVelocity = ConvertUnits.ToSimUnits(0, 150);
			//throw new NotImplementedException();
		}

		public void Draw(SpriteBatch sb)
		{
			//sb.Draw(portrait, new Rectangle(posX, posY, 61, 55), new Rectangle(0, 0, 61, 55), Color.White);
			//Console.WriteLine(ConvertUnits.ToDisplayUnits(body.Position));
			Vector2 pos = ConvertUnits.ToDisplayUnits(body.Position);
			sb.Draw(portrait, new Rectangle((int)pos.X, (int)pos.Y, 61, 55), new Rectangle(0, 0, 61, 55), Color.White);

			for(int i = 0; i < name.Length; i++)
			{
				sb.Draw(text, new Rectangle(letterPos[i] + posX + nameOffset, (int)ConvertUnits.ToDisplayUnits(body.Position.Y + 0.0001f) + 6, 16, 16), new Rectangle(locations[i].X, locations[i].Y, 16, 16), Color.Black);
			}
			sb.Draw(scrollingNums, new Rectangle(posX + 45, (int)pos.Y + 22, 5, 8), new Rectangle(0, numY, 5, 8), Color.LightBlue);

			//sb.Draw(scrollingNums, new Rectangle(posX + 45 - 8, (int)pos.Y + 22, 5, 8), new Rectangle(0, numY, 5, 8), Color.LightBlue);

			//sb.Draw(scrollingNums, new Rectangle(posX + 45 - 16, (int)pos.Y + 22, 5, 8), new Rectangle(0, numY, 5, 8), Color.LightBlue);
		}

		public void Update(GameTime gameTime, KeyboardState state)
		{
			numTimer += gameTime.ElapsedGameTime.TotalSeconds;


			if (state.IsKeyDown(Keys.A))
				health = 2;
			else if (state.IsKeyDown(Keys.S))
				health = 8;
			
			if(numTimer > 0.01)//100 pixels per second
			{
				healthCounter++;
				if(healthCounter == 10)
				{
					healthCounter = 0;
					if (rollingHealth > health)
						rollingHealth--;
					else if (rollingHealth < health)
						rollingHealth++;
				}

				if (numY / 9 != health)
				{
					numTimer = 0;
					if (rollingHealth > health)
					{
						numY--;
						if (numY == -1)
							numY = 90;
					}
					else if (rollingHealth < health)
					{
						numY++;
						if (numY == 91)
							numY = 0;
					}
				}
			}

			Console.WriteLine("Rolling: " + rollingHealth);
			Console.WriteLine("NumY: " + numY/9);
		}

		private void Jostle(float multiplier)
		{
			Console.WriteLine("finna jostle");
			body.ResetDynamics();
			body.ApplyForce(new Vector2(0, lastForce));
			lastForce *= multiplier;
		}
	}
}
