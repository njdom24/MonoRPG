using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    class Battle
    {
        private Texture2D background;
        private Effect effect;
        private double time;
        private RenderTarget2D firstEffect;
        private RenderTarget2D final;
        private GraphicsDevice graphicsDevice;

        public Battle(ContentManager contentManager, RenderTarget2D final, GraphicsDevice graphicsDevice, RenderTarget2D firstEffect)
        {
            background = contentManager.Load<Texture2D>("Battle/Layer");
            effect = contentManager.Load<Effect>("Battle/BattleBG");
            effect.Parameters["time"].SetValue((float)time);
            this.firstEffect = firstEffect;
            this.final = final;
            this.graphicsDevice = graphicsDevice;
        }

        public void Draw(SpriteBatch sb)
        {
            graphicsDevice.SetRenderTarget(firstEffect);
            sb.Begin(sortMode: SpriteSortMode.Immediate);
            effect.CurrentTechnique.Passes[2].Apply();
            graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            sb.Draw(background, new Rectangle(0,0, 400, 240), Color.White);//Draw to texture
            sb.End();

            graphicsDevice.SetRenderTarget(final);
            sb.Begin(sortMode: SpriteSortMode.Immediate);
            effect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            sb.Draw(firstEffect, new Rectangle(0, 0, 400, 240), Color.White);//Draw texture to buffer
            sb.End();
            
        }

        public void Update(GameTime gameTime)
        {
            time += gameTime.ElapsedGameTime.TotalSeconds;
            if (time > Math.PI)
                time -=0;
            effect.Parameters["time"].SetValue((float)time);
        }
    }
}
