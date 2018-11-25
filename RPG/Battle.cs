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
		private Texture2D blackRect;
		private Texture2D background;
		private Texture2D background2;
		private Enemy knight;
		private Effect effect;
		private double bgTimer;
		
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
		private Icon[] options;
		private Selector selector;

		private World world;
		private Combatant waiter;

		private int offsetHeightTop;
		private int offsetHeightBottom;

		private float secondsPerBeat;
		private double combatTimer;
		private double threshHold;
		private Texture2D combatIndicator;

		public Battle(ContentManager contentManager, RenderTarget2D final, GraphicsDevice graphicsDevice, PresentationParameters pp)
		{
			combatTimer = 0;
			threshHold = 0.15;
			combatIndicator = contentManager.Load<Texture2D>("Battle/Icons/Attack");
			secondsPerBeat = 0.6f;
			world = new World(new Vector2(0, 9.8f));
			waiter = null;
			options = new Icon[] { new Icon(contentManager, "Attack"), new Icon(contentManager, "Bag"), new Icon(contentManager, "Spells"), new Icon(contentManager, "Run") };
			blackRect = new Texture2D(graphicsDevice, 1, 1);
			blackRect.SetData(new Color[] { Color.Black });
			knight = new Enemy(contentManager.Load<Texture2D>("Battle/Enemies/Knight"), contentManager.Load<Texture2D>("Battle/Icons/MusicNote"), contentManager.Load<Texture2D>("Battle/Icons/HitMarker"), world, secondsPerBeat, threshHold);
			MultiSampleCount = pp.MultiSampleCount;
			firstEffect = new RenderTarget2D(graphicsDevice, 400, 240, false, SurfaceFormat.Color, DepthFormat.None, MultiSampleCount, RenderTargetUsage.DiscardContents);
			secondEffect = new RenderTarget2D(graphicsDevice, 400, 240, false, SurfaceFormat.Color, DepthFormat.None, MultiSampleCount, RenderTargetUsage.DiscardContents);
			comboEffect = new RenderTarget2D(graphicsDevice, 400, 240, false, SurfaceFormat.Color, DepthFormat.None, MultiSampleCount, RenderTargetUsage.DiscardContents);
			content = contentManager;
			prevState = Keyboard.GetState();
			selector = new Selector(4);
			background = contentManager.Load<Texture2D>("Battle/BG");
			background2 = content.Load<Texture2D>("Battle/Yellow");
			effect = contentManager.Load<Effect>("Battle/BattleBG");
			bgTimer = 0;
			effect.Parameters["time"].SetValue((float)bgTimer);
			this.final = final;
			this.graphicsDevice = graphicsDevice;
			text = new Hud(new string[] { "hi" }, contentManager, 48, 3, 0, 240-(5*8), canClose: true);
			text.finishText();
			commandName = new Hud(new string[] { "Attack" }, contentManager, 10, 1, 240 - 80, 0, canClose: false);
			offsetHeightBottom = text.getHeight();
			offsetHeightTop = 32;
		}

		public void DrawBackground(SpriteBatch sb)
		{
			graphicsDevice.SetRenderTarget(firstEffect);
			sb.Begin(sortMode: SpriteSortMode.Immediate);
			effect.CurrentTechnique.Passes[2].Apply();
			graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			sb.Draw(background, new Rectangle(0, 0, 400, 240), Color.White);//Draw to texture
			sb.End();

			graphicsDevice.SetRenderTarget(secondEffect);
			sb.Begin(sortMode: SpriteSortMode.Immediate);
			effect.CurrentTechnique.Passes[0].Apply();
			graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			sb.Draw(firstEffect, new Rectangle(0, 0, 400, 240), Color.White);//Draw texture to buffer
			sb.End();

			graphicsDevice.SetRenderTarget(comboEffect);
			sb.Begin();
			sb.Draw(secondEffect, new Rectangle(0, 0, 400, 240), Color.White);
			sb.End();

			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			graphicsDevice.SetRenderTarget(firstEffect);
			sb.Begin(sortMode: SpriteSortMode.Immediate);
			effect.CurrentTechnique.Passes[2].Apply();
			graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			sb.Draw(background2, new Rectangle(0, 0, 400, 240), Color.White);//Draw to texture
			sb.End();

			graphicsDevice.SetRenderTarget(secondEffect);
			sb.Begin(sortMode: SpriteSortMode.Immediate);
			effect.CurrentTechnique.Passes[1].Apply();
			graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			sb.Draw(firstEffect, new Rectangle(0, 0, 400, 240), Color.White);//Draw texture to buffer
			sb.End();

			graphicsDevice.SetRenderTarget(final);
			sb.Begin();

			sb.Draw(comboEffect, new Rectangle(0, 0, 400, 240), Color.White);
			sb.Draw(secondEffect, new Rectangle(0, 0, 400, 240), Color.White * 0.25f);
			
			sb.End();
		}

		private void DrawHud(SpriteBatch sb)
		{
			sb.Begin();
			sb.Draw(blackRect, new Rectangle(0, 0, 400, 32), Color.Black);
			commandName.Draw(sb);

			if(combatTimer > secondsPerBeat - threshHold)// && combatTimer < 0.6)
			{
				sb.Draw(combatIndicator, new Rectangle(200 - 12, 4, combatIndicator.Width * 2, combatIndicator.Height * 2), Color.White);
				if (combatTimer > secondsPerBeat + threshHold)
					combatTimer = threshHold;
			}
			sb.End();

			for (int i = 0; i < options.Length; i++)
			{
				options[i].Draw(sb, i * 16 + 4, 4, i == selector.GetIndex());
			}
			commandName = new Hud(new string[] { options[selector.GetIndex()].GetName() }, content, 6, 1, 400-(8*9), 4, canClose: false);
		}

		void Screen.Draw(SpriteBatch sb)
		{
			graphicsDevice.Clear(Color.White);
			DrawBackground(sb);
			DrawHud(sb);

			sb.Begin();
			knight.Draw(sb, bgTimer, offsetHeightTop, offsetHeightBottom);
			text.Draw(sb);
			sb.End();
			
		}

		void Screen.Update(GameTime gameTime)
		{
			knight.Update(gameTime);
			world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
			combatTimer += gameTime.ElapsedGameTime.TotalSeconds;
			commandName.finishMessage();
			commandName.Update(gameTime, prevState);
			UpdateBackground(gameTime);
			
			if(waiter != null)
			{
				//Console.WriteLine(text.messageComplete());
				//text.Update(gameTime, prevState);
				if(text.messageComplete() || waiter.IsDone(gameTime, combatTimer, prevState))
				{
					waiter.ForceFinish();
					waiter = null;
				}
			}
			else
				if (text.messageComplete())
				{
					advanceBattle();
				}
				else
					text.Update(gameTime, prevState);


			prevState = Keyboard.GetState();
		}

		private void advanceBattle()
		{
			selector.Update(prevState);
			if (Keyboard.GetState().IsKeyDown(Keys.Space) && prevState.IsKeyUp(Keys.Space))
			{
				if (selector.GetIndex() == 0)
				{
					knight.TakeDamage(5, combatTimer);
					waiter = knight;
					text = new Hud(new string[] { "Knight has " + knight.health + " health!" }, content, 48, 3, 0, 240 - (5 * 8), canClose: true);
				}
			}
		}

		private void UpdateBackground(GameTime gameTime)
		{
			bgTimer += gameTime.ElapsedGameTime.TotalSeconds;
			if (bgTimer > Math.PI * 2)
				bgTimer -= 0;
			effect.Parameters["time"].SetValue((float)bgTimer);
		}
	}
}
