using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
	class NewNPC
	{
		private bool flipped;
		private Animation walkDown;
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
		private NewPlayer player;
		private int steps;
		private int curStep;
		private bool backwards;
		private bool vertical;
		private double moveTimer;
		private int x;
		private int y;
		private bool paused;
		public string[] messages;
		public int textWidth;
		public int textHeight;

		private bool canMove;
		private bool isMoving;
		private Vector2 finalPos;

		private int offsetX;
		private int offsetY;

		public NewNPC(World world, ContentManager content, NewPlayer player, int steps, bool vertical, int posX, int posY, string[] messages, int textWidth = 20, int textHeight = 3)
		{
			offsetY = 27;
			this.textWidth = textWidth;
			this.textHeight = textHeight;
			this.messages = messages;
			paused = false;
			backwards = false;
			curStep = 1;
			this.steps = steps;
			this.vertical = vertical;
			animSpeed = 0.25f;
			isMoving = false;
			canMove = true;
			this.player = player;
			flipped = false;
			//body = new Body(world, new Vector2(0, 0));
			body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(17), ConvertUnits.ToSimUnits(6), 0.1f);
			//body = BodyFactory.CreateRectangle(world, 10, 10, 1, new Vector2(0,0));
			//body.Position = new Vector2(8 * 31, 23*16);
			body.BodyType = BodyType.Kinematic;
			body.Position = ConvertUnits.ToSimUnits(posX * 16, posY * 16);
			x = posX;
			y = posY;
			tex = content.Load<Texture2D>("Map/Tazmily/Hinawa/Hinawa");
			timer = 0;
			moveTimer = 0;

			walkDown = new Animation(0, 3, 0);
		}

		public void Update(GameTime gameTime)
		{
			if (!paused)
			{
				
				if (steps > 0)
				{
					//Vector2 tempPos = new Vector2((body.Position.X), (int)Math.Round(body.Position.Y));
					//body.Position = tempPos;

					if (!isMoving)
					{
						moveTimer += gameTime.ElapsedGameTime.TotalSeconds;
						if (moveTimer >= 1)
						{
							moveTimer -= gameTime.ElapsedGameTime.TotalSeconds;
							if (vertical)
							{   
								if (backwards)
								{
									//if (colArray[y + 1][x] != 2)//nothing down
									{
										if (--curStep == 1)
											backwards = false;
										moveDown();
									}
								}
								else
								{
									//if (colArray[y - 1][x] != 2)//nothing up
									{
										if (curStep++ == steps)
											backwards = true;
										moveUp();
									}
								}
								
							}
							else
							{
								
								if (backwards)
								{
									//if (colArray[y][x - 1] != 2)
									{
										if (--curStep == 1)
											backwards = false;
										moveLeft();
									}
								}
								else
								{
									//if (colArray[y][x + 1] != 2)
									{
										if (curStep++ == steps)
											backwards = true;
										moveRight();
									}
								}
								
							}
						}
						else
						{
							body.LinearVelocity = new Vector2(0, 0);
						}
					}
					else
					{
						if (!vertical)
						{
							switch (curStateH)
							{
								case HorizontalState.Left:
									if (body.Position.X <= finalPos.X)
									{
										isMoving = false;
										body.LinearVelocity = Vector2.Zero;
									}
									break;
								case HorizontalState.Right:
									if (body.Position.X >= finalPos.X)
									{
										isMoving = false;
										body.LinearVelocity = Vector2.Zero;
									}
									break;
								default:
									break;
							}
						}
						else
							switch (curStateV)
							{
								case VerticalState.Up:
									if (body.Position.Y <= finalPos.Y)
									{
										isMoving = false;
										body.LinearVelocity = Vector2.Zero;
									}
									break;
								case VerticalState.Down:
									if (body.Position.Y >= finalPos.Y)
									{
										isMoving = false;
										body.LinearVelocity = Vector2.Zero;
									}
									break;
								default:
									break;
							}
						}
					//tempPos = new Vector2((int)Math.Round(body.Position.X), (int)Math.Round(body.Position.Y));
					//body.Position = tempPos;
				}
			}
				Move(gameTime);
		}

		public void Move(GameTime gameTime, bool notRunning = true)//SAFE TO ASSUME DOESN'T WORK. WASN'T TESTED WITH NPC
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
			if (curStateH == HorizontalState.Right && body.LinearVelocity.X > 0)
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
			else if (curStateH == HorizontalState.Left && body.LinearVelocity.X < 0)
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
			if (curStateV == VerticalState.Up && body.LinearVelocity.Y < 0)
			{
				flipped = false;
				//SetStateV(VerticalState.Up);
				//curAnim = walkUp;
				//offsetX = 0;
				offsetY = 54;
				//animIndex = (int)State.Left;

				animIndex = walkDown.getFrame();
			}
			else if (curStateV == VerticalState.Down && body.LinearVelocity.Y > 0)
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
				walkDown.resetStart();
				animIndex = walkDown.getFrame();//sets current frame to standing animation
				walkDown.resetEnd();
			}
		}
		private void SetStateH(HorizontalState s)
		{
			if (prevStateH != curStateH)
				walkDown.resetEnd();
			prevStateH = curStateH;
			curStateH = s;
		}
		private void SetStateV(VerticalState s)
		{
			if (prevStateV != curStateV)
				walkDown.resetEnd();
			prevStateV = curStateV;
			curStateV = s;
		}
		public void Draw(SpriteBatch sb, int mapHeight)
		{
			float zIndex = 1 - ConvertUnits.ToDisplayUnits(body.Position.Y) / mapHeight;
			//Console.WriteLine(zIndex);
			//Need to add +1 to animIndex if flipped
			//Console.WriteLine(body.Position * 100);
			if (curStateH == HorizontalState.Left)
				sb.Draw(tex, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y) - 10 - 3, 17, 27), new Rectangle((animIndex + 1 + offsetX) * 17, walkDown.offset + offsetY, -17, 27), Color.White, 0, Vector2.Zero, SpriteEffects.None, zIndex);
			else
				sb.Draw(tex, new Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position.X), (int)ConvertUnits.ToDisplayUnits(body.Position.Y) - 10 - 3, 17, 27), new Rectangle((animIndex + offsetX) * 17, walkDown.offset + offsetY, 17, 27), Color.White, 0, Vector2.Zero, SpriteEffects.None, zIndex);
		}

		private void moveUp()
		{
			moveTimer = 0;
			SetStateV(VerticalState.Up);
			animSpeed = 0.25f;
			body.LinearVelocity = ConvertUnits.ToSimUnits(0, -64);
			finalPos = new Vector2(body.Position.X, body.Position.Y - ConvertUnits.ToSimUnits(64));
			//colArray[y--][x] = 0;
			//colArray[y][x] = 1;
			isMoving = true;
		}

		private void moveDown()
		{
			moveTimer = 0;
			SetStateV(VerticalState.Down);
			animSpeed = 0.25f;
			body.LinearVelocity = ConvertUnits.ToSimUnits(0, 64);
			finalPos = new Vector2(body.Position.X, body.Position.Y + ConvertUnits.ToSimUnits(64));
			//colArray[y++][x] = 0;
			//colArray[y][x] = 1;
			isMoving = true;
		}

		private void moveLeft()
		{
			moveTimer = 0;
			SetStateH(HorizontalState.Left);
			animSpeed = 0.25f;
			body.LinearVelocity = ConvertUnits.ToSimUnits(-64, 0);
			finalPos = new Vector2(body.Position.X - ConvertUnits.ToSimUnits(64), body.Position.Y);
			//colArray[y][x--] = 0;
			//colArray[y][x] = 1;
			isMoving = true;
		}

		private void moveRight()
		{
			moveTimer = 0;
			SetStateH(HorizontalState.Right);
			animSpeed = 0.25f;
			body.LinearVelocity = ConvertUnits.ToSimUnits(64, 0);
			finalPos = new Vector2(body.Position.X + ConvertUnits.ToSimUnits(64), body.Position.Y);
			//colArray[y][x++] = 0;
			//colArray[y][x] = 1;
			isMoving = true;
		}

		public bool checkPlayer(NewPlayer.HorizontalState Hstate, NewPlayer.VerticalState Vstate, KeyboardState prevState)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Space) && !prevState.IsKeyDown(Keys.Space))//doesnt check for OOB!
			{
				/*
				if (colArray[y][x + 1] == 2 && Hstate == NewPlayer.HorizontalState.Left)
				{
					SetState(State.Right);
					curAnim = walkLeft;
					return true;
				}
				else if (colArray[y][x - 1] == 2 && Hstate == NewPlayer.HorizontalState.Right)
				{
					SetState(State.Left);
					curAnim = walkLeft;
					return true;
				}
				else if (colArray[y - 1][x] == 2 && Vstate == NewPlayer.VerticalState.Down)
				{
					SetState(State.Up);
					curAnim = walkUp;
					return true;
				}
				else if (colArray[y + 1][x] == 2 && Vstate == NewPlayer.VerticalState.Up)
				{
					SetState(State.Down);
					curAnim = walkDown;
					return true;
				}
				*/
			}

			return false;
		}

		public bool isStopped()
		{
			return !isMoving;
		}
	}
}

