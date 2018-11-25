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
		private double moveTimer;
		private int offsetTop;
		private int offsetBottom;
		private float lastForce;
		private double moveDuration;
		private double secondsPerBeat;
		private double threshHold;
		private bool noteHit;
		private int noteCount;
		private Texture2D musicNote;
		private Texture2D hitMarker;
		private Vector2[] notePositions;
		private bool[] visibility;
		private Vector2 defaultPos;
		private int centerX;
		private int centerY;

		private Vector2 notePos;
		private double noteTimer;

		private int verticalRadius = 15;
		private int horizontalRadius = 80;

		private bool visible;

		private int[] visOrder;

		private double piTimer;
		private double hitTimer;
		private Random offsetter;
		private int hitX;
		private int hitY;

		public Enemy(Texture2D sprite, Texture2D musicNote, Texture2D hitMarker, World world, double secondsPerBeat, double threshHold = 0, int offsetTop = 0, int offsetBottom = 0)
		{
			offsetter = new Random();
			hitX = 0;
			hitY = 0;
			hitTimer = 0;
			this.hitMarker = hitMarker;
			piTimer = 0;
			notePositions = new Vector2[16];
			visOrder = new int[] { 10, 2, 5, 13, 7, 15, 9, 1, 11, 3, 14, 6, 12, 4, 0, 8 };
			visibility = new bool[16];
			visible = true;
			noteTimer = 0;
			notePos = Vector2.Zero;
			this.musicNote = musicNote;
			defaultPos = new Vector2((400 - sprite.Width) / 2, (240 - offsetBottom + offsetTop - sprite.Height) / 2);
			noteHit = true;
			noteCount = 0;
			this.secondsPerBeat = secondsPerBeat;
			this.threshHold = threshHold;
			moveDuration = 0.05;
			lastForce = 450;
			this.offsetTop = offsetTop;
			this.offsetBottom = offsetBottom;
			body = new Body(world, new Vector2((400 - sprite.Width) / 2, (240 - offsetBottom + offsetTop - sprite.Height) / 2));
			body.SetTransform(new Vector2((400 - sprite.Width) / 2, (240 - offsetBottom + offsetTop - sprite.Height) / 2), 0);
			body.BodyType = BodyType.Dynamic;
			body.Mass = 0.1f;
			this.sprite = sprite;
			health = 100;
			moveTimer = 0.05;

			centerX = (int)defaultPos.X + (sprite.Width - musicNote.Width) / 2 + 6;
			centerY = (int)defaultPos.Y + (sprite.Height - musicNote.Height) / 2 - 30;

			for (int i = 0; i < notePositions.Length; i++)
			{
				notePositions[i] = new Vector2(centerX + horizontalRadius * (float)Math.Cos(i * Math.PI / 8), centerY - verticalRadius * (float)Math.Sin(i * Math.PI / 8));
			}
		}

		public void Draw(SpriteBatch sb, double piTimer, int offsetTop = 0, int offsetBottom = 0)//make own timer eventually
		{
			sb.Draw(sprite, new Rectangle((int)body.Position.X, (int)body.Position.Y, sprite.Width, sprite.Height), Color.White);
			if(hitTimer > 0)
			{
				sb.Draw(hitMarker, new Rectangle(centerX + hitX, centerY + hitMarker.Height/2 + hitY, hitMarker.Width, hitMarker.Height), Color.White);
			}
			for (int i = notePositions.Length-1; i >= 0; i--)
			{
				if (visibility[i])
				{
					sb.Draw(musicNote, new Rectangle((int)notePositions[i].X, (int)notePositions[i].Y, musicNote.Width, musicNote.Height), Color.White);
				}
			}
			//sb.Draw(musicNote, new Rectangle((int)notePos.X, (int)notePos.Y, musicNote.Width, musicNote.Height), Color.White);
		}

		public void UpdateNotes(double elapsedTime)
		{
			for (int i = 0; i < notePositions.Length; i++)
			{
				notePositions[i] = new Vector2(centerX + horizontalRadius * (float)Math.Cos((Math.PI / 8 * i) + piTimer), centerY - verticalRadius * (float)Math.Sin((Math.PI / 8 * i) + piTimer));
			}

			//notePos = new Vector2((int)(centerX + horizontalRadius * Math.Cos(seconds)), (int)(centerY - verticalRadius * Math.Sin(seconds)));
		}

		public void Update(GameTime gameTime)
		{
			piTimer += (16) * (gameTime.ElapsedGameTime.TotalSeconds / secondsPerBeat * Math.PI / 16);
			if(hitTimer > 0)
				hitTimer -= gameTime.ElapsedGameTime.TotalSeconds;
			Console.WriteLine(hitTimer);
		}

		public override void ForceFinish()
		{
			lastForce = 450;
			moveTimer = moveDuration;
			body.ResetDynamics();
			body.SetTransform(defaultPos, 0);
		}

		private bool FinishCombo()
		{
			visibility = new bool[16];
			return true;
		}

		public override bool IsDone(GameTime gameTime, double combatTimer, KeyboardState prevState)//TODO: Check for multiple hits per beat
		{
			UpdateNotes(gameTime.ElapsedGameTime.TotalSeconds);
			if (noteCount == 16)
				return FinishCombo();

			moveTimer -= gameTime.ElapsedGameTime.TotalSeconds;
			
			if (combatTimer > secondsPerBeat + threshHold)//if time window ends
			{
				if (noteHit)
					noteHit = false;
				else
					return FinishCombo();
			}

			if (Keyboard.GetState().IsKeyDown(Keys.Space) && prevState.IsKeyUp(Keys.Space))
			{
				if (combatTimer > secondsPerBeat - threshHold)//during time window
				{
					if (noteHit)
						return FinishCombo();

					AdditionalDamage();
				}
				else//exit when a beat is missed
					return FinishCombo();
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

		private void MakeVisible()
		{
			/*
			int lowestIndex = 0;
			double lowestHeight = 9;
			for (int i = 0; i < notePositions.Length; i++)
			{
				if (notePositions[i].Y > lowestHeight)
				{
					lowestIndex = i;
					lowestHeight = notePositions[i].Y;
				}
			}
			visibility[lowestIndex] = true;
			*/
			//visibility[12] = true;
			hitTimer = 0.05;
			hitX = offsetter.Next(-10, 10);
			hitY = offsetter.Next(-10, 10);
			visibility[visOrder[noteCount-1]] = true;
		}

		public override void TakeDamage(int damage, double combatTimer)
		{
			visibility = new bool[16];
			noteCount = 1;
			Jostle(-2.5f);
			piTimer = combatTimer;
			health -= damage;
			noteHit = (combatTimer > secondsPerBeat - threshHold);

			MakeVisible();
		}

		private void AdditionalDamage()
		{
			ForceFinish();
			Jostle(-1.5f);
			health--;
			noteHit = true;
			noteCount++;
			piTimer += 0.2;

			MakeVisible();
		}

		private void Jostle(float multiplier)
		{
			body.ResetDynamics();
			body.ApplyForce(new Vector2(lastForce, 0));
			lastForce = lastForce * multiplier;
		}
	}
}
