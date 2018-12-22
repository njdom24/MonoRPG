
using FarseerPhysics;
using FarseerPhysics.DebugView;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
	class NewMap : Screen
	{
		private Effect effect;

		private DebugViewXNA debugView;
		private KeyboardState prevState;//used by both npcs and textboxes
		private Hud hud;
		private NewNPC[] npcs;
		private int tileWidth;
		private int tileHeight;
		private int width;
		private int height;
		private GraphicsDevice g;
		private Texture2D debug;
		//private Texture2D light;
		private ContentManager cont;
		private List<Body> blocks;
		private List<Vector2> blockDims;//x, y, width, height

		private TiledMap tMap;
		private TiledMapRenderer mapRenderer;

		private bool speaking;
		private World world;
		public NewPlayer player;
		//private NPC npc;
		private List<Vector2> verticesList;

		private Camera2D camera;

		public Camera2D Camera
		{
			get
			{
				return camera;
			}
		}

		public NewMap(GraphicsDevice pDevice, ContentManager content, int pTileWidth, int pTileHeight, int pWidth, int pHeight)
		{
			ConvertUnits.SetDisplayUnitToSimUnitRatio(100);
			verticesList = new List<Vector2>();
			//light = content.Load<Texture2D>("lightmask");
			effect = content.Load<Effect>("File");
			//effect.Parameters["lightMask"].SetValue(light);
			prevState = Keyboard.GetState();
			hud = new Hud(new string[] { "Undertale is bad,\nand so am I." }, content);
			speaking = false;
			blocks = new List<Body>();
			tileWidth = pTileWidth;
			tileHeight = pTileHeight;
			width = pWidth;
			height = pHeight;
			g = pDevice;
			cont = content;

			camera = new Camera2D(pDevice);
			debug = content.Load<Texture2D>("overworld_gutter");

			tMap = content.Load<TiledMap>("Map/Tazmily/Tazmily");
			mapRenderer = new TiledMapRenderer(pDevice);

			world = new World(Vector2.Zero);
			//world.ContactManager.OnBroadphaseCollision += BroadphaseHandler;
			world.ContactManager.EndContact += EndContactHandler;
			debugView = new DebugViewXNA(world);
			debugView.LoadContent(pDevice, content);
			//debugView.AppendFlags(DebugViewFlags.DebugPanel);
			//debugView.AppendFlags(DebugViewFlags.PolygonPoints);
			//debugView.AppendFlags(DebugViewFlags.ContactPoints);
			//debugView.AppendFlags(DebugViewFlags.AABB);
			debugView.AppendFlags(DebugViewFlags.Shape);
			debugView.DefaultShapeColor = Color.Green;

			//camera.Position = player.body.Position;
			//Console.WriteLine("Scunt: " + tMap.ObjectLayers.Count);

			ParseCollisions();
			player = new NewPlayer(world, content, 16, 23);
			//npc = new NPC(world, content, colArray, 2, false, 16, 14, new string[] {"Weebs are worse\nthan fortnite\ngamers.", "Where's the lie?" });

			//Body b1 = BodyFactory.CreateRectangle(world, 3, 3, 1);

			blockDims = new List<Vector2>();
			MakeCollisionBodies();

			npcs = new NewNPC[] {
				new NewNPC(world, content, player, 1, false, 12, 15, prevState, new string[] { "Weebs are worse\nthan fortnite\ngamers.", "Where's the lie?" }),
				//new NewNPC(world, content, player, 0, true, 18, 18, new string[] {"help"}, 4, 1)
			};
		}

		private void BroadphaseHandler(ref FixtureProxy fp1, ref FixtureProxy fp2)
		{
			//Console.WriteLine(fp1.Fixture.UserData);
			//Console.WriteLine(fp2.Fixture.UserData);
			if (fp1.Fixture.Body.UserData != null && fp1.Fixture.Body.UserData is NewPlayer)
				if(fp2.Fixture.Body.UserData != null && fp2.Fixture.Body.UserData == "polygon")
					Console.WriteLine("nurgis");
			if (fp2.Fixture.Body.UserData != null && fp2.Fixture.Body.UserData is NewPlayer)
				if (fp1.Fixture.Body.UserData != null && fp1.Fixture.Body.UserData == "polygon")
					Console.WriteLine("nurgis");
		}
		
		private void EndContactHandler(Contact contact)//unfinished
		{
			Console.WriteLine("end contact");
			NewNPC tempNPC;
			if (contact.FixtureA.Body.UserData is NewNPC)
				tempNPC = (NewNPC)contact.FixtureA.Body.UserData;
			else if (contact.FixtureB.Body.UserData is NewNPC)
				tempNPC = (NewNPC)contact.FixtureB.Body.UserData;
			else
				return;

			tempNPC.touchingPlayer = false;
		}

		public void HandleInput(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.A) && prevState.IsKeyUp(Keys.A))
				hud.finishMessage();
		}

		private void ParseCollisions()
		{
			//makes triangular collisions
		}
		public void DrawDebug(SpriteBatch sb)
		{
			for (int i = 0; i < blocks.Count; i++)
			{
				Body b = blocks[i];
				//sb.Draw(debug, new Rectangle(0, 0, 16, 16), new Rectangle(0, 0, 16, 16), Color.White);
				sb.Draw(debug, new Rectangle((int)b.Position.X - ((int)blockDims[i].X - 1) * 8, (int)b.Position.Y - ((int)blockDims[i].Y - 1) * 8, 16 * (int)blockDims[i].X, 16 * (int)blockDims[i].Y), new Rectangle(0, 0, 16, 16), Color.White);
			}
		}
		public void MakeCollisionBodies()
		{
			for (int i = 0; i < tMap.ObjectLayers[0].Objects.Length; i++)//Collision Layer
			{
				int x = (int)tMap.ObjectLayers[0].Objects[i].Position.X / 16;
				int y = (int)tMap.ObjectLayers[0].Objects[i].Position.Y / 16;
				int width = (int)tMap.ObjectLayers[0].Objects[i].Size.Width / 16;
				int height = (int)tMap.ObjectLayers[0].Objects[i].Size.Height / 16;

				//Body b = (BodyFactory.CreateRectangle(world, 16*3, 16*2, 0.2f, new Vector2(32 + 8*2, 32 + 8)));
				blocks.Add(BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(width * 16), ConvertUnits.ToSimUnits(height * 16), 0, new Vector2(ConvertUnits.ToSimUnits(x * 16 + (width - 1) * 8), ConvertUnits.ToSimUnits(y * 16 + (height - 1) * 8))));
				blocks[blocks.Count - 1].BodyType = BodyType.Static;
				blocks[blocks.Count - 1].UserData = null;
				blockDims.Add(new Vector2(width, height));
				//sb.Draw(debug, new Rectangle((int)b.Position.X - (width-1)*8, (int)b.Position.Y - (height-1)*8, 16*width, 16*height), new Rectangle(0, 0, 16, 16), Color.White);
			}
			//parsePolys("Triangles", true);
			parsePolys("Spangles", false);
		}

		void Screen.Draw(SpriteBatch pSb)
		{
			mapRenderer.Draw(tMap.GetLayer("Ground"), camera.GetViewMatrix());
			pSb.Begin(transformMatrix: camera.GetViewMatrix(), sortMode: SpriteSortMode.BackToFront);//SpriteSortMode.Immediate required for pixel shader

			//pSb.Begin();
			//pSb.Draw(debug, new Rectangle(0, 0, debug.Width, debug.Height), Color.White);
			//mapRenderer.Draw(tMap, camera.GetViewMatrix());
			foreach (NewNPC n in npcs)
				n.Draw(pSb, tMap.HeightInPixels);
			player.Draw(pSb, tMap.HeightInPixels);
			//g.SamplerStates[0] = SamplerState.PointClamp;

			Vector2 tilePos = Vector2.Zero;

			//DrawDebug(pSb);
			pSb.End();
			mapRenderer.Draw(tMap.GetLayer("Hills"), camera.GetViewMatrix());
			pSb.Begin(sortMode: SpriteSortMode.Immediate);
			effect.CurrentTechnique.Passes[1].Apply();
			if (speaking)
				hud.Draw(pSb);
			
			pSb.End();
			pSb.Begin(transformMatrix: Camera.GetViewMatrix());
			//DrawDebug(pSb);
			pSb.End();

			Matrix proj = Matrix.CreateOrthographicOffCenter(0f, 400, 240, 0f, 0f, 1f);
			Matrix view = camera.GetViewMatrix();
			debugView.RenderDebugData(ref proj, ref view);
			
		}

		void Screen.Update(GameTime gameTime)
		{
			hud.Update(gameTime, prevState);
			HandleInput(gameTime);
			

			mapRenderer.Update(tMap, gameTime);


			if (!speaking)
			{
				world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
				player.Update(gameTime, false);
				foreach (NewNPC n in npcs)
				{
					if(Keyboard.GetState().IsKeyDown(Keys.Space) && prevState.IsKeyUp(Keys.Space))
						if (n.isStopped() && n.touchingPlayer)
						{
							n.speaking = true;
							n.FacePlayer(player.getStateH(), player.getStateV());
							speaking = true;
							hud = new Hud(n.messages, cont, n.textWidth, n.textHeight);
						}

					n.Update(gameTime);
				}
				
			}
			else
			{
				player.Update(gameTime, true);
				foreach (NewNPC n in npcs)
					n.Update(gameTime);
				if (hud.messageComplete())
				{
					speaking = false;
					foreach (NewNPC n in npcs)
						n.ResetSpeaking();
				}
			}


			//camera.Position = new Vector2(player.body.Position.X - 400 / 2, player.body.Position.Y - 240 / 2 + 16);
			prevState = Keyboard.GetState();
			AdjustCamera();

		}

		private void AdjustCamera()
		{
			Vector2 tempPos = new Vector2((int)(ConvertUnits.ToDisplayUnits(player.body.Position.X) - 200), (int)(ConvertUnits.ToDisplayUnits(player.body.Position.Y) - 120 + 16 - 13));

			if (TooFarUp())
				tempPos.Y = 0;
			else if (TooFarDown())
				tempPos.Y = tMap.HeightInPixels - 240;
			if (TooFarLeft())
				tempPos.X = 0;
			else if (TooFarRight())
				tempPos.X = tMap.WidthInPixels - 400;

			//camera.Position = Vector2.Lerp(camera.Position, tempPos, 0.1f);
			camera.Position = tempPos;
		}
		private bool TooFarLeft()
		{
			return (ConvertUnits.ToDisplayUnits(player.body.Position.X) - 400 / 2 < 0);
		}

		private bool TooFarRight()
		{
			return (ConvertUnits.ToDisplayUnits(player.body.Position.X) - 400 / 2 + 400 > tMap.WidthInPixels);
		}

		private bool TooFarUp()
		{
			return (ConvertUnits.ToDisplayUnits(player.body.Position.Y) - 240 / 2 + 16 - 13 < 0);
		}

		private bool TooFarDown()
		{
			return (ConvertUnits.ToDisplayUnits(player.body.Position.Y) - 240 / 2 + 16 - 13 + 240 > tMap.HeightInPixels);
		}

		private void parsePolys(string group, bool isSensor)
		{
			string s = System.Text.Encoding.Default.GetString(Properties.Resource.Tazmily);
			string line;
			int lastIndex = 0;
			lastIndex = s.IndexOf(group);
			
			string x = "Blargle";
			string y = "Flargle";
			s = s.Substring(lastIndex);//Go to objectgroup line
			s = s.Substring(0, s.IndexOf("</objectgroup>"));
			s = s.Substring(s.IndexOf('\n') + 1);//Go to object line
			List<List<Vector2>> myList = new List<List<Vector2>>();
			while (s.Contains("<object"))
			{
				
				string p1 = "";
				string p2 = "";
				myList.Add(new List<Vector2>());
				//Console.WriteLine("barles");
				s = s.Substring(s.IndexOf("x=\""));
				s = s.Substring(s.IndexOf("\"") + 1);
				//Console.WriteLine(s);
				x = s.Substring(0, s.IndexOf('\"'));

				s = s.Substring(s.IndexOf("y=\""));
				s = s.Substring(s.IndexOf("\"") + 1);
				y = s.Substring(0, s.IndexOf('\"'));
				s = s.Substring(s.IndexOf("polygon"));
				s = s.Substring(s.IndexOf('\"') + 1);
				float offX = float.Parse(x, CultureInfo.InvariantCulture.NumberFormat) - 8;
				float offY = float.Parse(y, CultureInfo.InvariantCulture.NumberFormat) - 8;
				line = s.Substring(0, s.IndexOf('\n'));
				//Console.WriteLine(line);
				do
				{
					p1 = line.Substring(0, line.IndexOf(','));
					line = line.Substring(line.IndexOf(',') + 1);
					p2 = line.Substring(0, line.IndexOf(' '));
					line = line.Substring(line.IndexOf(' ') + 1);
					myList[myList.Count - 1].Add(ConvertUnits.ToSimUnits(float.Parse(p1, CultureInfo.InvariantCulture.NumberFormat) + offX, float.Parse(p2, CultureInfo.InvariantCulture.NumberFormat)+offY));


				} while (line.IndexOf(' ') != -1);
				p1 = line.Substring(0, line.IndexOf(','));
				line = line.Substring(line.IndexOf(',') + 1);
				p2 = line.Substring(0, line.IndexOf('\"'));
				myList[myList.Count - 1].Add(ConvertUnits.ToSimUnits(float.Parse(p1, CultureInfo.InvariantCulture.NumberFormat)+offX, float.Parse(p2, CultureInfo.InvariantCulture.NumberFormat)+offY));
				s = s.Substring(s.IndexOf("</object>") + 9);
			}
			Body tempBody;
			Console.WriteLine(s);
			foreach (List<Vector2> list in myList)
			{
				tempBody = BodyFactory.CreatePolygon(world, new FarseerPhysics.Common.Vertices(list), 0.1f);
				tempBody.IsSensor = isSensor;
				tempBody.UserData = group;
			}


		}
	}
}
