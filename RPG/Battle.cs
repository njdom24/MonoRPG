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

        public Battle(ContentManager contentManager)
        {
            background = contentManager.Load<Texture2D>("Battle/Background");
            effect = contentManager.Load<Effect>("Battle/BattleBG");
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin(sortMode: SpriteSortMode.Immediate);
            effect.CurrentTechnique.Passes[0].Apply();
            sb.Draw(background, new Rectangle(0,0, background.Width, background.Height), Color.White);
            sb.End();
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
