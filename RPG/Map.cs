
using FarseerPhysics.Dynamics;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    class Map
    {
        private Hud hud;
        private int[][] colArray;
        private int tileWidth;
        private int tileHeight;
        private int width;
        private int height;
        private GraphicsDevice g;
        private Texture2D debug;
        private ContentManager cont;

        private List<Body> blocks;
        private List<Vector2> blockDims;//x, y, width, height

        private TiledMap tMap;
        private TiledMapRenderer mapRenderer;

        private World world;
        public Player player;

        private Camera2D camera;

        public Camera2D Camera
        {
            get
            {
                return camera;
            }
        }

        public Map(GraphicsDevice pDevice, ContentManager content, int pTileWidth, int pTileHeight, int pWidth, int pHeight)
        {
            hud = new Hud("Undertale is gay,\nand so am I.", content.Load<Texture2D>("Textbox/Chars"), content.Load<Texture2D>("Textbox/Textbox"));
            blocks = new List<Body>();
            tileWidth = pTileWidth;
            tileHeight = pTileHeight;
            width = pWidth;
            height = pHeight;
            g = pDevice;
            cont = content;

            camera = new Camera2D(pDevice);
            debug = content.Load<Texture2D>("Textbox/Chars");

            tMap = content.Load<TiledMap>("corneria");
            mapRenderer = new TiledMapRenderer(pDevice);

            world = new World(new Vector2(0, 0));
            
            //camera.Position = player.body.Position;
            Console.WriteLine("Scunt: " + tMap.ObjectLayers.Count);

            ParseCollisions();
            player = new Player(world, content, colArray);

            for (int i = 0; i < colArray.Length; i++)
            {
                for (int j = 0; j < colArray[i].Length; j++)
                    Console.Write(colArray[i][j]);
                Console.WriteLine("");
            }

            CheckCollisions();


            //blockDims = new List<Vector2>();
            //MakeCollisionBodies();
        }
        public void HandleInput(GameTime gameTime)
        {
            
        }
        private void CheckCollisions()
        {
            int x = (int)player.body.Position.X / 16;
            int y = (int)player.body.Position.Y / 16;
        }
        private void ParseCollisions()
        {
            colArray = new int[tMap.Height][];
            for (int i = 0; i < colArray.Length; i++)
                colArray[i] = new int[tMap.Width];

            for (int i = 0; i < tMap.ObjectLayers[0].Objects.Length; i++)//Collision Layer
            {
                int x = (int)tMap.ObjectLayers[0].Objects[i].Position.X/16;
                int y = (int)tMap.ObjectLayers[0].Objects[i].Position.Y/16;
                int width = (int)tMap.ObjectLayers[0].Objects[i].Size.Width / 16;
                int height = (int)tMap.ObjectLayers[0].Objects[i].Size.Height / 16;
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < width; k++)
                    {
                        colArray[y + j][x + k] = 1;
                    }
                }
            }
        }
        public void DrawDebug(SpriteBatch sb)
        {
            for(int i = 0; i < blocks.Count; i++)
            {
                Body b = blocks[i];
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
                blocks.Add(BodyFactory.CreateRectangle(world, width * 16, height * 16, 0, new Vector2(x * 16 + (width - 1) * 8, y * 16 + (height - 1) * 8)));
                blocks[blocks.Count-1].BodyType = BodyType.Static;
                blockDims.Add(new Vector2(width, height));
                //sb.Draw(debug, new Rectangle((int)b.Position.X - (width-1)*8, (int)b.Position.Y - (height-1)*8, 16*width, 16*height), new Rectangle(0, 0, 16, 16), Color.White);
            }
        }

        public void Draw(SpriteBatch pSb)
        {
            mapRenderer.Draw(tMap, camera.GetViewMatrix());
            pSb.Begin(transformMatrix: camera.GetViewMatrix());
            //pSb.Begin();
            //pSb.Draw(debug, new Rectangle(0, 0, debug.Width, debug.Height), Color.White);
            //mapRenderer.Draw(tMap, camera.GetViewMatrix());
            player.Draw(pSb);
            //g.SamplerStates[0] = SamplerState.PointClamp;

            Vector2 tilePos = Vector2.Zero;
            
            //DrawDebug(pSb);
            pSb.End();

            pSb.Begin();
            
            hud.Draw(pSb);
            pSb.End();
        }

        public void Update(GameTime gameTime)
        {
            hud.Update(gameTime);
            HandleInput(gameTime);
            AdjustCamera();
            world.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            mapRenderer.Update(tMap, gameTime);
            
            player.Update(gameTime);

            //camera.Position = new Vector2(player.body.Position.X - 400 / 2, player.body.Position.Y - 240 / 2 + 16);
            
        }

        private void AdjustCamera()
        {
            Vector2 tempPos = new Vector2(player.body.Position.X - 400 / 2, player.body.Position.Y - 240 / 2 + 16);

            if (TooFarUp())
                tempPos.Y = 0;
            else if (TooFarDown())
                tempPos.Y = tMap.HeightInPixels - 240;
            if (TooFarLeft())
                tempPos.X = 0;
            else if (TooFarRight())
                tempPos.X = tMap.WidthInPixels - 400;

            camera.Position = tempPos;
        }
        private Boolean TooFarLeft()
        {
            return (player.body.Position.X - 400 / 2 < 0);
        }

        private Boolean TooFarRight()
        {
            return (player.body.Position.X - 400 / 2 + 400 > tMap.WidthInPixels);
        }

        private Boolean TooFarUp()
        {
            return (player.body.Position.Y - 240 / 2 + 16 < 0);
        }

        private Boolean TooFarDown()
        {
            return (player.body.Position.Y - 240 / 2 + 16 + 240 > tMap.HeightInPixels);
        }
    }
}
