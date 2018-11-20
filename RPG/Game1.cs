using FarseerPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using System;
using System.Diagnostics;

namespace RPG
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        //GraphicsDeviceManager graphics;
        
        private GraphicsDeviceManager manager;
        private Map myMap;
        private Battle battle;
        private SpriteBatch sb;
        private SpriteBatch render;
        private RenderTarget2D scene;
        //private Texture2D light;

        //private Texture2D tileset;

        public Game1()
        {
            manager = new GraphicsDeviceManager(this);
            manager.GraphicsProfile = GraphicsProfile.HiDef;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
            Window.IsBorderless = true;
            Window.Title = "FF";

            int scale = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 600;//400 for full, 800 for half
            Window.Position = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width/2 - 200 * scale, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - 120 * scale);
            manager.PreferredBackBufferWidth = 400*scale;
            manager.PreferredBackBufferHeight = 240*scale;

            Content.RootDirectory = "Content";
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //GraphicsDevice.GraphicsProfile = GraphicsProfile.HiDef;
            
            PresentationParameters pp = GraphicsDevice.PresentationParameters;
            scene = new RenderTarget2D(GraphicsDevice, 400, 240, false, SurfaceFormat.Color, DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.DiscardContents);
            myMap = new Map(GraphicsDevice, Content, 16, 16, 10, 10);
            battle = new Battle(Content, scene, GraphicsDevice, new RenderTarget2D(GraphicsDevice, 400, 240, false, SurfaceFormat.Color, DepthFormat.None, pp.MultiSampleCount, RenderTargetUsage.DiscardContents));//if things break, look here
            sb = new SpriteBatch(GraphicsDevice);
            render = new SpriteBatch(GraphicsDevice);

            
            //tileset = Content.Load<Texture2D>("Corneria_gutter");
            base.Initialize();

            //light = Content.Load<Texture2D>("lightmask");
            //effect = Content.Load<Effect>("File");
            //effect.Parameters["lightMask"].SetValue(light);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>  
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            sb = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected void HandleInput(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            myMap.HandleInput(gameTime);

            if (Keyboard.GetState().IsKeyDown(Keys.B))
            {
                Content.Unload();
                //Content = null;
                Content = new Microsoft.Xna.Framework.Content.ContentManager(this.Services);
                Content.RootDirectory = "Content";
                myMap = new Map(GraphicsDevice, Content, 16, 16, 10, 10);
            }

        }
        protected override void Update(GameTime gameTime)
        {
            HandleInput(gameTime);
            //myMap.Update(gameTime);
            battle.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(scene);
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            //myMap.Draw(render);
            battle.Draw(sb);
            sb.Begin();
            
            //sb.Draw(tx, new Rectangle(0, 0, tx.Width, tx.Height), Color.White);
            //DrawLayer(0, sb);
            sb.End();
            GraphicsDevice.SetRenderTarget(null);

            //sb.Begin();
            //sb.Begin();
            //sb.Draw(scene, Vector2.Zero);

            //sb.End();

            scaleToDisplay();

            // TODO: Add your drawing code here

            //base.Draw(gameTime);
        }

        private void scaleToDisplay()
        {
            float outputAspect = Window.ClientBounds.Width / (float)Window.ClientBounds.Height;
            float preferredAspect = 400 / (float)240;
            Rectangle dst;
            if (outputAspect <= preferredAspect)
            {
                // output is taller than it is wider, bars on top/bottom
                int presentHeight = (int)((Window.ClientBounds.Width / preferredAspect) + 0.5f);
                int barHeight = (Window.ClientBounds.Height - presentHeight) / 2;
                dst = new Rectangle(0, barHeight, Window.ClientBounds.Width, presentHeight);
            }
            else
            {
                // output is wider than it is tall, bars left/right
                int presentWidth = (int)((Window.ClientBounds.Height * preferredAspect) + 0.5f);
                int barWidth = (Window.ClientBounds.Width - presentWidth) / 2;
                dst = new Rectangle(barWidth, 0, presentWidth, Window.ClientBounds.Height);
            }
            GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            sb.Draw(scene, dst, Color.White);
            sb.End();
        }

        /*
        private void DrawLayer(int index, SpriteBatch batch)
        {
            int tileWidth = 16;
            int tileHeight = 16;

            Debug.WriteLine(tMap.TileLayers.Count);
            for (var i = 0; i < tMap.TileLayers[index].Tiles.Count; i++)
            {
                //Get the identification of the tile
                int gid = tMap.TileLayers[index].Tiles[i].GlobalIdentifier;

                // Empty tile, do nothing
                if (gid == 0) { }
                else
                {
                    int tileFrame = gid - 1;
                    int column = tileFrame % (tileset.Width / tileWidth);//tileset.width
                    int row = tileFrame / (tileset.Height / tileHeight);

                    float x = (i % tMap.Width) * tMap.TileWidth;
                    float y = (float)Math.Floor(i / (double)tMap.Width) * tMap.TileHeight;

                    //Put all the data together in a new rectangle
                    Rectangle tilesetRec = new Rectangle(tileWidth * column, tileHeight * row, tileWidth, tileHeight);

                    //Draw the tile that is within the tilesetRec
                    batch.Draw(tileset, new Rectangle((int)x, (int)y, tileWidth, tileHeight), tilesetRec, Color.White);
                }
            }
            
        }*/
    }
}
