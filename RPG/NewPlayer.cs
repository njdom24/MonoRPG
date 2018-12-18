using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
	class NewPlayer
	{
		public float bodyWidth;
		public float bodyHeight;
		private bool flipped;
		//private Animation walkRight;
		//private Animation walkUp;
		private Animation walkDown;
		private Animation curAnim;
		public enum VerticalState { Up, Down, None };
		public enum HorizontalState { Left, Right, None };
		private VerticalState prevStateV;
		private VerticalState curStateV;
		private HorizontalState prevStateH;
		private HorizontalState curStateH;
		private double timer;
		private float animSpeed;
		private Texture2D tex;
		public Body body;
		private int animIndex;
		private int x;
		private int y;
		private int runOffset;
		private float speedMult;

		private bool canMove;
		private double runTimer;
		private bool running;
		private bool startedRunning;

		private int offsetX;
		private int offsetY;

		private int colCount;
		public bool touchingWall;
		public bool touchingAngle;

		public NewPlayer(World world, ContentManager content, int posX, int posY)
		{
			startedRunning = false;
			touchingAngle = false;
			touchingWall = false;
			speedMult = 1;
			runOffset = 0;
			running = false;
			animSpeed = 0.25f;
			canMove = true;
			flipped = false;
			//body = new Body(world, new Vector2(0, 0));
			bodyWidth = ConvertUnits.ToSimUnits(13);
			bodyHeight = ConvertUnits.ToSimUnits(6);
			body = BodyFactory.CreateRectangle(world, bodyWidth, bodyHeight, 0.1f);
			//body = BodyFactory.CreateRoundedRectangle(world, ConvertUnits.ToSimUnits(13), ConvertUnits.ToSimUnits(6), ConvertUnits.ToSimUnits(3), ConvertUnits.ToSimUnits(2), 2, 0.1f, Vector2.Zero);
			//body = BodyFactory.CreateRectangle(world, 10, 10, 1, new Vector2(0,0));
			//body.Position = new Vector2(8 * 31, 23*16);
			//body = BodyFactory.CreateCircle(world, ConvertUnits.ToSimUnits(7.5f), 1);
			//body = BodyFactory.CreateEllipse(world, ConvertUnits.ToSimUnits(6.5f), ConvertUnits.ToSimUnits(3), 6, 0.1f);
			body.BodyType = BodyType.Dynamic;
			body.Position = ConvertUnits.ToSimUnits(posX * 16, posY * 16);
			body.Friction = 0;
			body.FixedRotation = true;
			//body.Restitution = 0;
			//body.LinearDamping = 0;
			body.UserData = this;
			body.Mass = 0.1f;
			//body.OnCollision += OnCollisionHandler;
			x = posX;
			y = posY;
			//body.Position = new Vector2(16 * 16, 14 * 16);
			tex = content.Load<Texture2D>("Map/Lucas/Teen");
			timer = 0;
			prevStateV = VerticalState.Up;
			curStateV = VerticalState.Up;
			prevStateH = HorizontalState.None;
			curStateH = HorizontalState.None;
			//walkRight = new Animation(9, 12, 25);
			//walkUp = new Animation(0, 3, 50);
			walkDown = new Animation(0, 3, 0);
			curAnim = walkDown;
		}
		private bool OnCollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			
			Console.WriteLine("Touched: " + fixtureB.Body.UserData);
			if (contact.IsTouching)
			{
				if (fixtureB.Body.UserData == "Spangles")
					touchingWall = true;
				if (fixtureB.Body.UserData == "Triangles")
					touchingAngle = true;
				if (fixtureB.Body.UserData is NewNPC)
				{
					running = false;
					runOffset = 0;
					animSpeed = 0.25f;
					speedMult = 1;
				}
				else if (!touchingAngle)
				{
					/*
					running = false;
					runOffset = 0;
					animSpeed = 0.25f;
					speedMult = 1;
					*/
				}
			}
			/*
			running = false;
			runOffset = 0;
			animSpeed = 0.25f;
			speedMult = 1;
			justTouched = true;
			*/
			return true;
		}
		public override string ToString()
		{
			return "pureiya";
		}
		public void HandleInput()
		{

		}

		public void Update(GameTime gameTime, bool paused)
		{
			//Console.WriteLine(body.Position);
			//Vector2 tempPos = new Vector2((int)Math.Round(ConvertUnits.ToDisplayUnits(body.Position.X)), (int)Math.Round(ConvertUnits.ToDisplayUnits(body.Position.Y)));//could present physics issues
			//tempPos.X = ConvertUnits.ToSimUnits(tempPos.X);// * 0.01f;
			//tempPos.Y = ConvertUnits.ToSimUnits(tempPos.Y);// * 0.01f;
			//body.Position = tempPos;
			
			if (!paused)
			{
				if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
				{
					if (!running)
					{
						runTimer += gameTime.ElapsedGameTime.TotalSeconds;
						runOffset = 4;
						body.LinearVelocity = Vector2.Zero;
						curAnim.resetStart();
						Move(gameTime, false);
					}
					else
					{
						runTimer += gameTime.ElapsedGameTime.TotalSeconds;
						runOffset = 4;
						running = false;
					}

					//animSpeed = 0.15f;
					//speedMult = 1.5f;
				}
				else
				{
					if (runTimer >= 0.5f)// && !justTouched)
					{
						startedRunning = true;
						running = true;
						runOffset = 5;
						animSpeed = 0.15f;
						speedMult = 1.5f;
					}
					else if (!running)
					{
						//running = false;//not needed?
						runOffset = 0;
						animSpeed = 0.25f;
						speedMult = 1;
					}
					runTimer = 0;

					if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.Right))
					{
						body.LinearVelocity = Vector2.Zero;
						//curStateH = HorizontalState.None;
						//curStateV = VerticalState.None;
						if (Keyboard.GetState().IsKeyDown(Keys.Right))// && x + 1 < colArray[y].Length)
						{
							SetStateH(HorizontalState.Right);
							//myMap.Camera.Move(new Vector2(2, 0));

							if (false)//colArray[y][x + 1] == 1)// || colArray[y][x + 1] == 3)
								animSpeed = 0.5f;
							else
							{
								offsetX = 9;
								//animSpeed = 0.25f;
								body.LinearVelocity += ConvertUnits.ToSimUnits(64, 0);
								//colArray[y][x++] = 0;
								//colArray[y][x] = 2;
							}
						}
						else if (Keyboard.GetState().IsKeyDown(Keys.Left))// && x > 0)
						{
							SetStateH(HorizontalState.Left);
							offsetX = 9;
							body.LinearVelocity += ConvertUnits.ToSimUnits(-64, 0);
						}
						else
						{
							offsetX = 0;
							SetStateH(HorizontalState.None);
						}
						if (Keyboard.GetState().IsKeyDown(Keys.Up))// && y > 0)
						{
							SetStateV(VerticalState.Up);
							offsetY = 50;
							body.LinearVelocity += ConvertUnits.ToSimUnits(0, -64);

						}
						else if (Keyboard.GetState().IsKeyDown(Keys.Down))// && y + 1 < colArray.Length)
						{
							SetStateV(VerticalState.Down);
							offsetY = 0;
							body.LinearVelocity += ConvertUnits.ToSimUnits(0, 64);
						}
						else
						{
							offsetY = 25;
							SetStateV(VerticalState.None);
						}
						body.LinearVelocity *= speedMult;
					}
					else if (!running)
						body.LinearVelocity = Vector2.Zero;

					if (running)
					{
						if(!startedRunning && body.LinearVelocity.LengthSquared() < 0.1f)
						{
							Console.WriteLine("Weird stopping mechanism");
							running = false;
							runOffset = 0;
							animSpeed = 0.25f;
							speedMult = 1;
						}
						startedRunning = false;
						body.LinearVelocity = Vector2.Zero;
						if (curStateH == HorizontalState.Left)
							body.LinearVelocity += ConvertUnits.ToSimUnits(-64, 0);
						else if (curStateH == HorizontalState.Right)
							body.LinearVelocity += ConvertUnits.ToSimUnits(64, 0);
						if (curStateV == VerticalState.Up)
							body.LinearVelocity += ConvertUnits.ToSimUnits(0, -64);
						else if (curStateV == VerticalState.Down)
							body.LinearVelocity += ConvertUnits.ToSimUnits(0, 64);
						body.LinearVelocity *= speedMult;
					}
					Move(gameTime);
				}

				

				/*
				if (body.ContactList != null)
				{
					//Console.WriteLine("billmus");
					
					//body.Position = Vector2.Zero;
					//do
					//{
					if (body.ContactList != null && body.ContactList.Contact.IsTouching)
					{
						//Console.WriteLine(colCount++);
						Console.WriteLine(body.ContactList.Contact.FixtureA.UserData);
						if(body.ContactList.Contact.FixtureA.UserData != null && body.ContactList.Contact.FixtureB.UserData != null)
							if (true)//body.ContactList.Contact.FixtureA.UserData.ToString().Equals("polygon") || body.ContactList.Contact.FixtureB.UserData.ToString().Equals("polygon"))
							{
								//Console.WriteLine("killmus");
								if (true)
								{
									running = false;
									runOffset = 0;
									animSpeed = 0.25f;
									speedMult = 1;
									justTouched = true;
									//break;
								}
							}
							else
								justTouched = false;
					}
					//while (body.ContactList.Next != null);

					

				}
				*/
		}
	}

		public void Move(GameTime gameTime, bool notRunning = true)
		{
			bool hPass = false;
			//Console.WriteLine("HSTATE: " + curStateH);
			//Console.WriteLine("VSTATE: " + curStateV);
			//Console.WriteLine(timer);
			timer += gameTime.ElapsedGameTime.TotalSeconds;
			if (timer > animSpeed)
			{
				if(notRunning)
					walkDown.advanceFrame();
				timer = 0;
			}
			if (curStateH == HorizontalState.Right && ((Keyboard.GetState().IsKeyDown(Keys.Right)) || body.LinearVelocity.X > 0))
			{
				hPass = true;
				flipped = false;
				//SetStateH(HorizontalState.Right);
				//curAnim = walkRight;
				offsetX = 9;
				//offsetY = 25;
				//animIndex = (int)State.Left;
				animIndex = walkDown.getFrame();
			}
			else if (curStateH == HorizontalState.Left && ((Keyboard.GetState().IsKeyDown(Keys.Left)) || body.LinearVelocity.X < 0))
			{
				hPass = true;
				flipped = true;
				//SetStateH(HorizontalState.Left);
				//curAnim = walkRight;
				offsetX = 9;
				//offsetY = 25;
				//animIndex = (int)State.Left;
				
				animIndex = walkDown.getFrame();
			}
			if (curStateV == VerticalState.Up && ((Keyboard.GetState().IsKeyDown(Keys.Up)) || body.LinearVelocity.Y < 0))
			{
				flipped = false;
				//SetStateV(VerticalState.Up);
				//curAnim = walkUp;
				//offsetX = 0;
				offsetY = 50;
				//animIndex = (int)State.Left;

				animIndex = walkDown.getFrame();
			}
			else if (curStateV == VerticalState.Down && ((Keyboard.GetState().IsKeyDown(Keys.Down)) || body.LinearVelocity.Y > 0))
			{
				flipped = false;
				//SetStateV(VerticalState.Down);
				//curAnim = walkDown;
				//offsetX = 0;
				offsetY = 0;
				//animIndex = (int)State.Left;

				animIndex = walkDown.getFrame();
			}
			else if (!hPass)
			{
				timer = 0;
				curAnim.resetStart();
				animIndex = curAnim.getFrame();//sets current frame to standing animation
				curAnim.resetEnd();
			}
		}
		private void SetStateH(HorizontalState s)
		{
			if (prevStateH != curStateH)
				curAnim.resetEnd();
			prevStateH = curStateH;
			curStateH = s;
		}
		private void SetStateV(VerticalState s)
		{
			if (prevStateV != curStateV)
				curAnim.resetEnd();
			prevStateV = curStateV;
			curStateV = s;
		}
		private void resetStates()
		{
			curStateH = HorizontalState.None;
			prevStateH = HorizontalState.None;
			curStateV = VerticalState.None;
			prevStateV = VerticalState.None;
		}

		public HorizontalState getStateH()
		{
			return curStateH;
		}
		public VerticalState getStateV()
		{
			return curStateV;
		}

		public void Draw(SpriteBatch sb, int mapHeight)
		{
			//Console.WriteLine("Shitfuck: " + body.Position.X + ", Fuckshit: " + ConvertUnits.ToDisplayUnits(body.Position.X));
			//Need to add +1 to animIndex if flipped
			float zIndex = 1 - ConvertUnits.ToDisplayUnits(body.Position.Y) / mapHeight;

			if (curStateH == HorizontalState.Left)
				sb.Draw(tex, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X) + 1, (int)ConvertUnits.ToDisplayUnits(body.Position.Y) - 13, 15, 25), new Rectangle((animIndex + 1 + runOffset + offsetX) * 15, curAnim.offset + offsetY, -15, 25), Color.White, 0, Vector2.Zero, SpriteEffects.None, zIndex);
			else
				sb.Draw(tex, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X) + 1, (int)ConvertUnits.ToDisplayUnits(body.Position.Y) - 13, 15, 25), new Rectangle((animIndex + runOffset + offsetX) * 15, curAnim.offset + offsetY, 15, 25), Color.White, 0, Vector2.Zero, SpriteEffects.None, zIndex);
			//sb.Draw(tex, new Rectangle((int)body.Position.X - (15 - 1) * 8, (int)body.Position.Y - (25 - 1) * 8, 16 * 15, 16 * 25), new Rectangle((animIndex + runOffset) * 15, curAnim.offset, 15, 25)), Color.White);
		}

	}
}
