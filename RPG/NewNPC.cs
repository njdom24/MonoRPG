using FarseerPhysics;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
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
using static RPG.NewPlayer;

namespace RPG
{
	class NewNPC
	{
		public float bodyWidth;
		public float bodyHeight;
		private bool flipped;
		private Animation walkDown;
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
		public bool speaking;
		public string[] messages;
		public int textWidth;
		public int textHeight;

		public bool touchingPlayer;
		private bool isMoving;
		private Vector2 finalPos;

		private int offsetX;
		private int offsetY;

		private Fixture leftFixt;
		public bool touchingLeft, touchingRight, touchingUp, touchingDown;

		public NewNPC(World world, ContentManager content, NewPlayer player, int steps, bool vertical, int posX, int posY, KeyboardState prevState, string[] messages, int textWidth = 20, int textHeight = 3)
		{
			if (vertical)
			{
				offsetX = 0;
				offsetY = 0;
			}
			else
			{
				offsetX = 9;
				offsetY = 27;
			}
			this.textWidth = textWidth;
			this.textHeight = textHeight;
			this.messages = messages;
			speaking = false;
			backwards = false;
			curStep = 1;
			this.steps = steps;
			this.vertical = vertical;
			animSpeed = 0.25f;
			isMoving = false;
			this.player = player;
			flipped = false;
			//body = new Body(world, new Vector2(0, 0));
			bodyWidth = ConvertUnits.ToSimUnits(17);
			bodyHeight = ConvertUnits.ToSimUnits(6);
			body = BodyFactory.CreateRectangle(world, bodyWidth, bodyHeight, 0.1f);
			body.UserData = this;
			//body = BodyFactory.CreateRectangle(world, 10, 10, 1, new Vector2(0,0));
			//body.Position = new Vector2(8 * 31, 23*16);
			body.BodyType = BodyType.Kinematic;
			body.Position = ConvertUnits.ToSimUnits(posX * 16, posY * 16);
			body.OnCollision += OnCollisionHandler;
			//leftFixt = FixtureFactory.AttachEdge(Vector2.Zero, new Vector2(0, 1), body);
			x = posX;
			y = posY;
			tex = content.Load<Texture2D>("Map/Tazmily/Hinawa/Hinawa");
			timer = 0;
			moveTimer = 0;

			walkDown = new Animation(0, 3, 0);
		}

		//Checks the player's directional state on contact to change the NPC's directional state
		private bool OnCollisionHandler(Fixture fixtureA, Fixture fixtureB, Contact contact)
		{
			Console.WriteLine("on collision");
			NewPlayer temp;
			if (fixtureA.Body.UserData is NewPlayer)
				temp = (NewPlayer)fixtureA.Body.UserData;
			else if (fixtureB.Body.UserData is NewPlayer)
				temp = (NewPlayer)fixtureB.Body.UserData;
			else
			{
				Console.WriteLine("SCREE");
				return true;
			}
			touchingPlayer = true;
			touchingUp = false;
			touchingDown = false;
			touchingLeft = false;
			touchingRight = false;

			if (temp.getStateH() == HorizontalState.Left)//turn right
			{
				touchingLeft = true;
				//offsetX = 9;
				//flipped = false;
			}
			else if (temp.getStateH() == HorizontalState.Right)//turn left
			{
				touchingRight = true;
				//flipped = true;
				//offsetX = 9;
			}

			if (temp.getStateV() == VerticalState.Down)//turn up
			{
				touchingDown = true;
				//offsetY = 54;
			}
			else if (temp.getStateV() == VerticalState.Up)//turn down
			{
				touchingUp = true;
				//offsetY = 0;
			}
			//if(temp.getStateH() == )
			/*
			touchingLeft = true;

			float distX = temp.body.Position.X - body.Position.X;
			float distY = temp.body.Position.Y - body.Position.Y;

			distX = Math.Abs(distX) - 0.5f*(bodyWidth + temp.bodyWidth);
			distY = Math.Abs(distY) - 0.5f*(bodyHeight + temp.bodyHeight);
			if (distX > distY)
			{
				if (temp.body.Position.X > body.Position.X)
					Console.WriteLine("RIGHT: " + distX);
				else
					Console.WriteLine("LEFT: " + distX);
			}
			else
			{
				if (temp.body.Position.Y > body.Position.Y)
					Console.WriteLine("DOWN: " + distX);
				else
					Console.WriteLine("UP: " + distX);
			}
			/*
			if (temp.body.Position.X < body.Position.X)
				Console.WriteLine("LEFT");
			else if (temp.body.Position.X > body.Position.X)
				Console.WriteLine("RIGHT");
			else if (temp.body.Position.Y < body.Position.Y)
				Console.WriteLine("UP");
			else if (temp.body.Position.Y > body.Position.Y)
				Console.WriteLine("DOWN");
			*/

			return true;
		}
		public void ResetSpeaking()
		{
			speaking = false;
			if (offsetY >= 3 * 27)
				offsetY -= 3 * 27;
		}
		public void Update(GameTime gameTime)
		{
			if (!speaking)
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
				Move(gameTime);
			}
			else
			{
				moveTimer += gameTime.ElapsedGameTime.TotalSeconds;
				if (moveTimer >= 0.2f)
				{
					moveTimer = 0;
					if (offsetY >= 27 * 3)
					{
						offsetY -= 3 * 27;
					}
					else
					{
						offsetY += 3 * 27;
					}
				}
			}
				
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
			if (flipped)
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

		public void FacePlayer()
		{
			moveTimer = 0;
			if (touchingLeft)//face right
			{
				Console.WriteLine("left");
				offsetX = 9;
				flipped = false;
			}
			else if (touchingRight)//face left
			{
				Console.WriteLine("right");
				flipped = true;
				offsetX = 9;
			}
			else
			{
				Console.WriteLine("hNone");
				offsetX = 0;
			}

			if (touchingDown)//face down
			{
				Console.WriteLine("down");
				offsetY = 54;
			}
			else if (touchingUp)//face up
			{
				Console.WriteLine("up");
				offsetY = 0;
			}
			else
			{
				Console.WriteLine("vNone");
				offsetY = 27;
			}

			offsetY += 27 * 3;

			Console.WriteLine("OffsetX: " + offsetX);
			Console.WriteLine("OffsetY: " + offsetY);
		}

		public bool isStopped()
		{
			return !isMoving;
		}
	}
}

