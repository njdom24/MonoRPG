using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
	class Battle : Screen
	{
		private Enemy flasher;
		private double flashTimer;
		private byte flashCounter;

		private Enemy knight;
		private Battler travis;

		private Texture2D blackRect;
		private Texture2D background;
		private Texture2D background2;
		
		private Effect effect;
		private Effect flash;
		private double bgTimer;
		private double waiterTimer;
		
		private RenderTarget2D firstEffect;
		private RenderTarget2D secondEffect;
		private RenderTarget2D comboEffect;
		private RenderTarget2D final;
		private GraphicsDevice graphicsDevice;
		private Hud text;
		private Hud commandName;
		private KeyboardState prevState;
		private ContentManager content;
		private int MultiSampleCount;
		private Icons options;
		private Selector selector;

		private World world;
		private Combatant waiter;

		private int offsetHeightTop;
		private int offsetHeightBottom;

		private float secondsPerBeat;
		private double combatTimer;
		private double threshHold;
		private Texture2D combatIndicator;

		private bool messageBuffered;
		private int playerMove;
		private int enemyMove;

		private enum Phase {PlayerPhase, EnemyPhase, AttackPhase, PlayerDeathPhase, EnemyDeathPhase};
		private Phase curPhase;
		private double turnWaiter;

		public Battle(ContentManager contentManager, RenderTarget2D final, GraphicsDevice graphicsDevice, PresentationParameters pp)
		{
			curPhase = Phase.PlayerPhase;
			effect = contentManager.Load<Effect>("Battle/BattleBG");
			effect.CurrentTechnique = effect.Techniques[1];
			flash = contentManager.Load<Effect>("Battle/SpriteFlash");
			combatTimer = 0;
			threshHold = 0.15;
			combatIndicator = contentManager.Load<Texture2D>("Battle/Icons/Attack");
			secondsPerBeat = 0.5f;
			world = new World(ConvertUnits.ToSimUnits(0,400));
			waiter = null;
			options = new Icons(contentManager);
			blackRect = new Texture2D(graphicsDevice, 1, 1);
			blackRect.SetData(new Color[] { Color.Black });
			travis = new Battler(contentManager, world);
			knight = new Enemy(contentManager, world, secondsPerBeat, threshHold);
			MultiSampleCount = pp.MultiSampleCount;
			Texture2D palette = contentManager.Load<Texture2D>("Battle/003Palette");
			effect.Parameters["palette"].SetValue(palette);
			effect.Parameters["paletteWidth"].SetValue((float)palette.Width);
			effect.Parameters["time"].SetValue((float)bgTimer);
			flash.Parameters["time"].SetValue((float)flashTimer);
			firstEffect = new RenderTarget2D(graphicsDevice, 400, 240, false, SurfaceFormat.Color, DepthFormat.None, MultiSampleCount, RenderTargetUsage.DiscardContents);
			secondEffect = new RenderTarget2D(graphicsDevice, 400, 240, false, SurfaceFormat.Color, DepthFormat.None, MultiSampleCount, RenderTargetUsage.DiscardContents);
			comboEffect = new RenderTarget2D(graphicsDevice, 400, 240, false, SurfaceFormat.Color, DepthFormat.None, MultiSampleCount, RenderTargetUsage.DiscardContents);
			content = contentManager;
			prevState = Keyboard.GetState();
			selector = new Selector(4, names: new string[] {"Attack", "Bag", "PSI", "Run"});
			background = contentManager.Load<Texture2D>("Battle/005");
			
			background2 = content.Load<Texture2D>("Battle/Yellow");
			
			bgTimer = 0;
			//graphicsDevice.Textures[2] = palette;
			this.final = final;//required for scaling
			this.graphicsDevice = graphicsDevice;
			text = new Hud(new string[] { "hi" }, contentManager, 48, 3, 0, 240-(5*8), canClose: true);
			text.finishText();
			commandName = new Hud(new string[] { selector.GetName() }, content, 6, 1, 400 - (8 * 9), 4, canClose: false);
			offsetHeightBottom = text.getHeight();
			offsetHeightTop = 32;
			messageBuffered = false;
			flashCounter = 1;
		}

		public void DrawBackground(SpriteBatch sb)
		{
			graphicsDevice.SetRenderTarget(firstEffect);
			sb.Begin(sortMode: SpriteSortMode.Immediate);
			effect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			sb.Draw(background, new Rectangle(0, 0, 400, 240), Color.White);//Draw to texture
			sb.End();

			//////////////////////////////////Second Background///////////////////////////////////
			/*
			graphicsDevice.SetRenderTarget(secondEffect);
			sb.Begin(sortMode: SpriteSortMode.Immediate);
			effect.CurrentTechnique.Passes[1].Apply();
			graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			sb.Draw(background2, new Rectangle(0, 0, 400, 240), Color.White);//Draw to texture
			sb.End();
			*/
			graphicsDevice.SetRenderTarget(final);
			sb.Begin(samplerState: SamplerState.PointClamp);
			
			sb.Draw(firstEffect, new Rectangle(0, 0, 400, 240), Color.White * 1f);
			//sb.Draw(secondEffect, new Rectangle(0, 0, 400, 240), Color.White * 0f);
			
			sb.End();
		}

		private void DrawHud(SpriteBatch sb)
		{
			sb.Begin(samplerState: SamplerState.PointClamp);
			sb.Draw(blackRect, new Rectangle(0, Game1.height - 35, Game1.width, 35), Color.Black);
			sb.Draw(blackRect, new Rectangle(0, 0, 400, 32), Color.Black);
			if (curPhase == Phase.PlayerPhase)
			{
				
			
				if (waiter == null)
				{
					
					commandName.Draw(sb);
				}

				
				//sb.End();

				if (waiter == null)
					options.Draw(sb, selector.GetIndex());
			}

			if (combatTimer > secondsPerBeat - threshHold)// && combatTimer < 0.6)
			{
				sb.Draw(combatIndicator, new Rectangle(200 - 12, 4, combatIndicator.Width * 2, combatIndicator.Height * 2), Color.White);
				if (combatTimer > secondsPerBeat + threshHold)
					combatTimer = threshHold;
			}
			sb.End();

			if (selector.IndexChanged())
				commandName = new Hud(new string[] { selector.GetName() }, content, 6, 1, 400-(8*9), 4, canClose: false);
		}

		public void Draw(SpriteBatch sb)
		{
			//graphicsDevice.Clear(Color.White);
			DrawBackground(sb);
			DrawHud(sb);
			if(flasher == null)
			{
				sb.Begin(samplerState: SamplerState.PointClamp);
				knight.Draw(sb, bgTimer, offsetHeightTop, offsetHeightBottom);
			}
			else
			{
				sb.Begin(samplerState: SamplerState.PointClamp, sortMode: SpriteSortMode.Immediate);
				flash.Techniques[0].Passes[0].Apply();
				knight.Draw(sb, bgTimer, offsetHeightTop, offsetHeightBottom);
			}
			sb.End();
			sb.Begin(samplerState: SamplerState.PointWrap);

			travis.Draw(sb);
			//if(waiter == null)// || messageBuffered)
			text.Draw(sb);
			sb.End();
		}

		public void Update(GameTime gameTime)
		{
			if (flasher != null)
			{
				if (flashCounter % 2 == 0)
					flashTimer -= gameTime.ElapsedGameTime.TotalSeconds;
				else
					flashTimer += gameTime.ElapsedGameTime.TotalSeconds;
				flash.Parameters["time"].SetValue((float)flashTimer);

				if (flashTimer > 0.2)// || flashTimer < 0)
				{
					flashTimer = 0.2;
					flashCounter++;
				}
				else if(flashTimer < 0)
				{
					flashTimer = 0;
					flashCounter++;
				}
				if (flashCounter > 4)
				{
					flasher = null;
					flashTimer = 0;
					flashCounter = 1;
				}
			}

			knight.Update(gameTime);

			travis.Update(gameTime, Keyboard.GetState());

			world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
			combatTimer += gameTime.ElapsedGameTime.TotalSeconds;
			waiterTimer += gameTime.ElapsedGameTime.TotalSeconds;
			turnWaiter += gameTime.ElapsedGameTime.TotalSeconds;
			commandName.finishMessage();
			commandName.Update(gameTime, prevState);
			UpdateBackground(gameTime);

			if(turnWaiter > 0.2)
			if (text.messageComplete())
			{
				if (waiter != null)
				{
					if (waiter.attacked)
					{
						if (waiter is Enemy)
						{
							if (waiter.IsDone(gameTime, combatTimer, prevState))
							{
								waiter.ForceFinish();
								waiter = null;
								turnWaiter = 0;
							}
						}
						else
						{
							if (waiter.IsDone(gameTime, waiterTimer, prevState))
							{
								waiterTimer = 0;
								waiter.ForceFinish();
								waiter = null;
								turnWaiter = 0;
							}
						}
					}
					else
					{
						waiter.TakeDamage(1, combatTimer);
						waiter.attacked = true;
					}
				}
				else
					advanceBattle();
			}
			else
				text.Update(gameTime, prevState);


			prevState = Keyboard.GetState();
		}

		private void advanceBattle()
		{
			switch (curPhase)
			{
				case Phase.PlayerPhase:
					if (waiter == null)
						selector.Update(prevState);

					if (Keyboard.GetState().IsKeyDown(Keys.Space) && prevState.IsKeyUp(Keys.Space))
					{
						if (selector.GetIndex() == 0)
						{
							playerMove = 0;
							curPhase = Phase.EnemyPhase;
						}
					}
					break;
				case Phase.EnemyPhase:
					enemyMove = 0;
					curPhase = Phase.AttackPhase;
					break;
				case Phase.AttackPhase:
					if(playerMove == 0)
					{
						knight.attacked = false;
						text = new Hud(new string[] { "@Travis's attack!" }, content, 48, 3, canClose: true);
						//knight.TakeDamage(1, combatTimer);
						waiter = knight;
						playerMove = -1;
					}
					else if(enemyMove == 0)
					{
						flasher = knight;
						flashTimer = 0;

						travis.attacked = false;
						text = new Hud(new string[] { "@Knight's attack!" }, content, 48, 3, canClose: true);
						//travis.TakeDamage(1, combatTimer);
						waiter = travis;
						enemyMove = -1;
						//flashCounter = 1;
						//flashTimer = 0;
					}
					else
					{
						curPhase = Phase.PlayerPhase;
					}
					break;
				default:
					break;
			}
		}

		private void UpdateBackground(GameTime gameTime)
		{
			bgTimer += gameTime.ElapsedGameTime.TotalSeconds;
			if (bgTimer > Math.PI * 2)
			{
				//bgTimer -= Math.PI*2;
				//Console.WriteLine("Timer reset");
			}
			effect.Parameters["time"].SetValue((float)bgTimer);
		}
	}
}
